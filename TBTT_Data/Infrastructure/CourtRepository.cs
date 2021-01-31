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
   public class CourtRepository : BaseRepository, ICourtRepository
    {
        private IMongoCollection<Court> _courtList;

        public CourtRepository()
        {
            _courtList = GetDatabase.GetCollection<Court>("TBTT_Board_CourtList");
        }

        public Court GetCourtList()
        {
            Court court = new Court();

            try
            {

                court.CourtList = _courtList.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            return court;
        }

        public Court GetCourtByName(string courtName)
        {
            Court court = new Court();

            try
            {

                court = _courtList.Find(court => court.CourtName == courtName).Limit(1).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw;
            }

            return court;
        }

    }
}
