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
    public class WaitingRepository:  BaseRepository, IWaitingRepository
    {
        private IConfiguration configuration;
        private  IMongoCollection<Waiting> _waitingList;
        private readonly int WaitingListInActivePeriod = 40; //Inactive for 40 minutes will be removed from the list.
        public WaitingRepository()
        {
            configuration = base.configuration;
            WaitingListInActivePeriod = Convert.ToInt32(configuration.GetSection("WaitingListInActivePeriod").Value);
            _waitingList = GetDatabase.GetCollection<Waiting>("TBTT_Board_WaitingList");         
        }


        public Waiting GetWaitingList(string playingName)
        {
            Waiting waiting = new Waiting();
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());

            try
            {
                DeleteFromWaitingList();
                if ((playingName == string.Empty) || (playingName == null))
                {
                    waiting.WaitingList = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC).SortBy(waiting => waiting.OrderID).SortBy(waiting => waiting.Id).ToList();
                }
                else
                {
                    waiting.WaitingList = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC && waiting.MemberName.ToLower().Contains(playingName.ToLower())).SortBy(waiting => waiting.OrderID).SortBy(waiting => waiting.Id).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
           
            return waiting;
        }

        public Waiting GetWaitingListByMemberName(string memberName)
        {
            Waiting waiting = new Waiting();
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());

            try
            {
                waiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC && waiting.MemberName == memberName).Limit(1).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            
            return waiting;
        }

        public Waiting GetWaitingListByMemberID(string memberID)
        {
            Waiting waiting = new Waiting();
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());

            try
            {
                waiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC && waiting.MembershipID == memberID).Limit(1).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }

            return waiting;
        }

        public int AddToWaitingList(Waiting paramWaiting)
        {
            int intReturnValue = 1;
            Waiting waiting = new Waiting();
            int intOrderID = 1;
            int intWaitingListID = 1;
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());
            try
            {
                waiting = _waitingList.Find(waiting => waiting.MembershipID == paramWaiting.MembershipID).FirstOrDefault();

                if (waiting == null)
                { 
                    paramWaiting.BoardStartDate = todaysDateUTC;
                    Waiting orderWaiting = new Waiting();
                    orderWaiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC).Limit(1).SortByDescending(waiting => waiting.OrderID).FirstOrDefault();
                     if (orderWaiting != null)
                    {
                        intOrderID = orderWaiting.OrderID + 1;
                    }

                    orderWaiting = new Waiting();
                    orderWaiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC).Limit(1).SortByDescending(waiting => waiting.WaitingListID).FirstOrDefault();
                    if (orderWaiting != null)
                    {
                        intWaitingListID = orderWaiting.WaitingListID + 1;
                    }


                    paramWaiting.WaitingListID = intWaitingListID;
                    paramWaiting.OrderID = intOrderID;
                    paramWaiting.UpdatedDate = DateTime.Now;
                    paramWaiting.NodeState = true;

                _waitingList.InsertOne(paramWaiting);
                 
                }
            }
            catch(Exception ex)
            {
                intReturnValue = -1;
            }
            
            return intReturnValue;
        }

        public int AddToWaitingListFromGameBoard(Game gameDTO)
        {
            int intReturnValue = 1;
            int intOrderID = 1;
            int intWaitingListID = 1;
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());
            Waiting orderWaiting = new Waiting();
            Waiting paramWaiting = new Waiting();
            Waiting waitingCheck = new Waiting();


            try
            {
                orderWaiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC).Limit(1).SortByDescending(waiting => waiting.OrderID).FirstOrDefault();
                if (orderWaiting != null)
                {
                    intOrderID = orderWaiting.OrderID + 1;
                }

                orderWaiting = _waitingList.Find(waiting => waiting.BoardStartDate == todaysDateUTC).Limit(1).SortByDescending(waiting => waiting.WaitingListID).FirstOrDefault();
                if (orderWaiting != null)
                {
                    intWaitingListID = orderWaiting.WaitingListID + 1;
                }

                foreach (var item in gameDTO.GameList)
                {
                    waitingCheck = new Waiting();

                    waitingCheck = _waitingList.Find(waiting => waiting.MembershipID == item.MembershipID).FirstOrDefault();
                   
                    if (waitingCheck == null)
                    {
                        paramWaiting = new Waiting();
                        paramWaiting.WaitingListID = intWaitingListID;
                        paramWaiting.OrderID = intOrderID;
                        paramWaiting.MembershipID = item.MembershipID;
                        paramWaiting.MemberName = item.MemberName;
                        paramWaiting.MembershipType = item.MembershipType;
                        paramWaiting.BoardStartDate = DateTime.Parse(item.GameStartDate.ToShortDateString());
                        paramWaiting.UpdatedDate = DateTime.Now;
                        paramWaiting.NodeState = true;

                        _waitingList.InsertOne(paramWaiting);

                        intWaitingListID += 1;
                        intOrderID += 1;

                    }


                }
            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }

            public int WaitingListUpdateStatus(Waiting paramWaiting)
        {
            int intReturnValue = 1;
            Waiting waiting = new Waiting();
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());

            try
            {
                
                var filter = new FilterDefinitionBuilder<Waiting>().Where(x => x.BoardStartDate == todaysDateUTC && x.MembershipID == paramWaiting.MembershipID);
                var options = new FindOneAndUpdateOptions<Waiting, Waiting>() { IsUpsert = false };
                var update = new UpdateDefinitionBuilder<Waiting>().Set(x => x.NodeState, paramWaiting.NodeState);
                

                 _waitingList.FindOneAndUpdate(filter, update, options);
            }
            catch (Exception ex)
            {

                intReturnValue = -1;
            }

            return intReturnValue;
        }

        public int DeleteFromWaitingListByMemberID(string memberID)
        {
            int intReturnValue = 1;
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());

            //DateTime WaitingDateUTC = Convert.ToDateTime(todaysDateUTC).ToUniversalTime().ToShortDateString());

            try
            {
                var result = _waitingList.DeleteOne(waiting => waiting.BoardStartDate == todaysDateUTC && waiting.MembershipID == memberID);

            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }

        public int DeleteFromWaitingList()
        {
            int intReturnValue = 0;
            Waiting waiting = new Waiting();
            DateTime waitingPeriod = DateTime.Now.AddMinutes(-1 * WaitingListInActivePeriod);

            try
            {
                var result = _waitingList.DeleteMany(waiting => waiting.UpdatedDate <= waitingPeriod);

            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }
    }
}
