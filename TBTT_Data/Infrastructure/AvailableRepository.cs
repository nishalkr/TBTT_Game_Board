using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Interface;
using TBTT_Data.Entities;
using TBTT_Data.Infrastructure;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TBTT_Data.Infrastructure
{
    public class AvailableRepository:  BaseRepository, IAvailableRepository
    {
        private IConfiguration configuration;
        private  IMongoCollection<Available> _availableList;
        private readonly int AvailableListInActivePeriod = 60; //InActive for 60 minutes will be removed from the list.
        public  AvailableRepository()
        {
            configuration = base.configuration;
            _availableList = GetDatabase.GetCollection<Available>("TBTT_Board_AvailableList");
            AvailableListInActivePeriod = Convert.ToInt32(configuration.GetSection("AvailableListInActivePeriod").Value);
            
        }


        public Available GetAvailableList(string AvailableName)
        {
            Available available = new Available();
            DateTime todaysDate = DateTime.Now.Date;
            try
            {
                DeleteFromAvailableList();
                if ((AvailableName == string.Empty) || (AvailableName == null))
                {
                    available.AvailableList = _availableList.Find(available => available.BoardStartDate == todaysDate).ToList();
                }
                else
                {
                    available.AvailableList = _availableList.Find(available => available.BoardStartDate == todaysDate && available.MemberName.ToLower().Contains(AvailableName.ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
            return available;
        }


        public Available GetAvailableListByMemberID(string memberID)
        {
            Available available = new Available();            
            DateTime todaysDateUTC = DateTime.Parse(DateTime.Now.ToShortDateString());
            try
                
            {
                available = _availableList.Find(available => available.BoardStartDate == todaysDateUTC && available.MembershipID == memberID).Limit(1).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }

            return available;
        }

        public int AddToAvailableList(Available paramAvailable)
        {
            int intReturnValue = 0;
            Available available = new Available();
            DateTime todaysDate = DateTime.Now.Date;
            int maxAvailableListID = 1;
            try
            {
                available = _availableList.Find(available => available.MemberName == paramAvailable.MemberName && available.BoardStartDate == todaysDate).FirstOrDefault();

                if (available == null)
                {
                    Available orderAvailable = new Available();
                    orderAvailable = _availableList.Find(available => available.BoardStartDate == todaysDate).Limit(1).SortByDescending(available => available.AvailableListID).FirstOrDefault();
                    if (orderAvailable != null)
                    {
                        maxAvailableListID = orderAvailable.AvailableListID + 1;
                    }
                    paramAvailable.AvailableListID = maxAvailableListID;
                    _availableList.InsertOne(paramAvailable);
                }
                else
                {
                    intReturnValue = -2;
                }
            }
            catch(Exception ex)
            {
                intReturnValue = -1;
            }
            
            return intReturnValue;
        }

        public int DeleteFromAvailableList()
        {
            int intReturnValue = 0;
            DateTime availablePeriodUTC = DateTime.Now.AddMinutes(-1 * AvailableListInActivePeriod);
            try
            {

                var result = _availableList.DeleteMany(available => available.UpdatedDate <= availablePeriodUTC);              
                
            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }


        public int DeleteFromAvailableListByMemberID(string memberID)
        {
            int intReturnValue = 1;
            Available available = new Available();
            DateTime todaysDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                     
            try
            {
                var result = _availableList.DeleteOne(available => available.BoardStartDate == todaysDate && available.MembershipID == memberID);

            }
            catch (Exception ex)
            {
                intReturnValue = -1;
            }

            return intReturnValue;
        }
    }
}
