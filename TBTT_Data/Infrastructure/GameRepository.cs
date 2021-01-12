using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Interface;
using TBTT_Data.Entities;
using TBTT_Data.Infrastructure;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TBTT_Data.Infrastructure
{
    public class GameRepository : BaseRepository, IGameRepository
    {
        private IConfiguration configuration;
        private IMongoCollection<Game> _gameBoard;
        private IMongoCollection<Member> _memberList;
        private readonly int gameListInActivePeriod = 19; //Inactive for 20 minutes will be removed from the list.
        private readonly int courtMemberDeleteTimeOut = 5; //Inactive for 20 minutes will be removed from the list.
        private readonly int courtMemberQueueTimeIn = 10; //In Doubles 4 active players, and additional 4 active players can join after 10 minutes.
        public GameRepository()
        {
            configuration = base.configuration;
            gameListInActivePeriod = Convert.ToInt32(configuration.GetSection("gameListInActivePeriod").Value);
            courtMemberDeleteTimeOut = Convert.ToInt32(configuration.GetSection("courtMemberDeleteTimeOut").Value);
            courtMemberQueueTimeIn = Convert.ToInt32(configuration.GetSection("courtMemberQueueTimeIn").Value);
            _gameBoard = GetDatabase.GetCollection<Game>("TBTT_Board_GameBoard");
        }

        public Game GetGameBoard(string courtName)
        {

            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            Game game = new Game();

            try
            {
                game.GameList = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).SortBy(game => game.GameStartDate).ToList();
            }
            catch (Exception)
            {

                throw;
            }            
           
            return game;
        }

        public Game GetGameBoardByMemberName(string memberName)
        {

            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            Game game = new Game();

            try
            {
                game = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.MemberName == memberName).Limit(1).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
                       

            return game;
        }

        public Game GetGameBoardByCourtAndMemberName(string courtName ,string memberName)
        {

            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
           
            Game game = new Game();

            try
            {
                game.GameList = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName && game.MemberName == memberName).SortByDescending(game => game.GameStartDate).Limit(1).ToList();
               
            }
            catch (Exception)
            {
                throw;
            }


            return game;
        }

        public Game GetGameBoardByLimitedNumber(string courtName, int intMemberCount = 4)
        {

            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            Game game = new Game();

            try
            {
                game.GameList = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).SortBy(game => game.GameStartDate).SortBy(game => game.OrderID).Limit(intMemberCount).ToList();
            }
            catch (Exception)
            {

                throw;
            }

            return game;
        }

        public Game GetGameBoardOrderByOrderID(string courtName)
        {

            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            Game game = new Game();

            try
            {
                game.GameList = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).SortBy(game => game.GameStartDate).SortBy(game => game.OrderID).ToList();
            }
            catch (Exception)
            {

                throw;
            }            

            return game;
        }

        public int AddToGameBoard(Game paramGame, int intMemberCount = 4)
        {
            int intReturnValue = 1;
            Game game = new Game();
  
            long intCountMemberCount = 0;
            int intGameID = 1;
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            int orderID = 0;
            int intMaxOrderID = 1;
            Member mem = null;

            Game gameMinMember = new Game();
            DateTime checkDate;
            DateTime todayUTC = DateTime.Now;
            DateTime formatedUTCToday;
            DateTime formatedUTCCheckDate;
            bool isValidToAdd = true;
            bool isDoubles = true;

            try
            {

                intCountMemberCount = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == paramGame.CourtName).Count();

                
                if (intMemberCount>= 4) //Means its a doubles court. This allow 4 people to play, and additional 4 people to queueu.
                {
                    if (intCountMemberCount >= 8) //Maximum allowed on doubles court is 8 members
                    {
                        intReturnValue = -2;
                        return intReturnValue;
                    }
                }
                else
                {
                    isDoubles = false;

                    if (intCountMemberCount >= 4) //Maximum allowed on Singles court is 4 members
                    {
                        intReturnValue = -2;
                        return intReturnValue;
                    }
                }

                if (intCountMemberCount>= intMemberCount)
                {
                    gameMinMember = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == paramGame.CourtName).Limit(1).SortBy(game => game.GameStartDate).FirstOrDefault();

                    if (gameMinMember != null)
                    {
                        checkDate = gameMinMember.GameStartDate.AddMinutes(courtMemberQueueTimeIn);

                        DateTime.TryParse(todayUTC.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCToday);
                        DateTime.TryParse(checkDate.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCCheckDate);

                        TimeSpan ts = formatedUTCToday - formatedUTCCheckDate;

                        if (ts.TotalMinutes <= 0)
                        {

                            isValidToAdd = false;
                        }
                    }

                    if (!isValidToAdd)
                    {
                        intReturnValue = -2;
                        return intReturnValue;
                    }

                }             
                


                game = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == paramGame.CourtName && game.MembershipID == paramGame.MembershipID).FirstOrDefault();

                if (game == null)
                {
                    paramGame.GameStartDate = DateTime.Now;
                    paramGame.GameStartShortDate = todaysDate;

                    Game gameFindID = new Game();

                    gameFindID = _gameBoard.Find(game => game.GameStartShortDate == todaysDate).Limit(1).SortByDescending(game => game.GameStartDate).SortByDescending(game => game.OrderID).FirstOrDefault();
                    if (gameFindID != null)
                    {
                        intGameID = gameFindID.GameID + 1;
                        intMaxOrderID = gameFindID.OrderID + 1;
                    }
                    
                    paramGame.GameID = intGameID;
                    paramGame.UpdatedDate = DateTime.Now;
                    paramGame.OrderID = intMaxOrderID;
                    paramGame.IsDoubles = isDoubles;

                    _gameBoard.InsertOne(paramGame);

                }
            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }

        public bool IsValidToDeleteFromGameBoard(string courtName)
        {
            bool isValidToDelete = false;
            string minGameStartDate = string.Empty;
            Game minGame = new Game();
            Game gameList = new Game();
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime checkDate;
            DateTime todayUTC = DateTime.Now;
            DateTime formatedUTCToday;
            DateTime formatedUTCCheckDate;

            try
            {
                minGame = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).Limit(1).SortBy(game => game.GameStartDate).FirstOrDefault();


                if (minGame != null)
                {
                    checkDate = minGame.GameStartDate.AddMinutes(gameListInActivePeriod);

                    DateTime.TryParse(todayUTC.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCToday);
                    DateTime.TryParse(checkDate.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCCheckDate);

                    TimeSpan ts = formatedUTCToday - formatedUTCCheckDate;

                    if (ts.TotalMinutes >= 0)
                    {

                        isValidToDelete = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }


            return isValidToDelete;
        }

        public int DeleteFromGameBoardByMemberName(string courtName, string memberName)
        {

            Game game = new Game();    
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime checkDate;
            DateTime todayUTC = DateTime.Now;
            DateTime formatedUTCToday;
            DateTime formatedUTCCheckDate;
            int intReturnValue = 0;


            try
            {
                game = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName && game.MemberName == memberName).Limit(1).SortBy(game => game.GameStartDate).FirstOrDefault();
                if (game != null)
                {
                    checkDate = game.GameStartDate.AddMinutes(courtMemberDeleteTimeOut);

                    DateTime.TryParse(todayUTC.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCToday);
                    DateTime.TryParse(checkDate.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCCheckDate);

                    TimeSpan ts = formatedUTCToday - formatedUTCCheckDate;

                    if (ts.TotalMinutes <= 0)
                    {

                        var result = _gameBoard.DeleteMany(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName && game.MemberName == memberName);
                        intReturnValue = 1;
                    }

                }

             }
            catch (Exception)
            {

                throw;
            }

            return intReturnValue;
        }

        public int DeleteFromGameBoard(string courtName, int intMemberCount = 4)
        {
            int intReturnValue = 0;
            string minGameStartDate = string.Empty;
            Game minGame = new Game();
            Game gameList = new Game();
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime checkDate;
            DateTime todayUTC = DateTime.Now;
            DateTime formatedUTCToday;
            DateTime formatedUTCCheckDate;

            Game gameItems = new Game();
            string preSQl = "{ _id: { $in: [";
            string postSQL = "]}}";
            string finalSQL = string.Empty;
            string strFinalIDs = string.Empty;
            StringBuilder strIDs = new StringBuilder();
            char[] charsToTrim = { ',' };

            try
            {
                minGame = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).Limit(1).SortBy(game => game.GameStartDate).FirstOrDefault();


                if (minGame != null)
                {
                    checkDate = minGame.GameStartDate.AddMinutes(gameListInActivePeriod);

                    DateTime.TryParse(todayUTC.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCToday);
                    DateTime.TryParse(checkDate.ToString("MM/dd/yyyy h:mm tt"), out formatedUTCCheckDate);

                    TimeSpan ts = formatedUTCToday - formatedUTCCheckDate;

                    if (ts.TotalMinutes >= 0)
                    {

                        gameItems.GameList = _gameBoard.Find(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName).Limit(intMemberCount).SortBy(game => game.GameStartDate).ToList();

                        if ((gameItems != null) && (gameItems.GameList != null))
                        {

                            foreach (var item in gameItems.GameList)
                            {
                                strIDs.Append("ObjectId('");
                                strIDs.Append(item.Id);
                                strIDs.Append("'),");
                                
                            }

                            if (strIDs.Length > 0)
                            {
                                if (strIDs.ToString().Trim().EndsWith(","))
                                {
                                    strFinalIDs = strIDs.ToString().Trim().TrimEnd(charsToTrim);
                                }
                                else
                                {
                                    strFinalIDs = strIDs.ToString().Trim();
                                }
                            }

                            

                            finalSQL = preSQl + strFinalIDs + postSQL;

                            if (strIDs.Length > 0)
                            {
                                var result = _gameBoard.DeleteMany(finalSQL);
                                UpdateDefinition<Game> updateDefinition = Builders<Game>.Update.Set(x => x.GameStartDate, todayUTC);
                                _gameBoard.UpdateMany(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName, updateDefinition);
                            }

                           
                            // var result = _gameBoard.DeleteMany(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName);
                            intReturnValue = 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return intReturnValue;
        }

        public int DeleteFromGameBoardOnVisibility(string courtName)
        {
            int intReturnValue = 0;
            string minGameStartDate = string.Empty;
            Game minGame = new Game();
            Game gameList = new Game();
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            DateTime checkDate;
            DateTime todayUTC = DateTime.Now;
            DateTime formatedUTCToday;
            DateTime formatedUTCCheckDate;

            try
            {
                var result = _gameBoard.DeleteMany(game => game.GameStartShortDate == todaysDate && game.CourtName == courtName);
                intReturnValue = 1;

            }
            catch (Exception)
            {

                throw;
            }


            return intReturnValue;
        }
    }

}
      

