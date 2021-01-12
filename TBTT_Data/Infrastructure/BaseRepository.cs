using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data;
using System.Data.Odbc;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using MongoDB;
using MongoDB.Driver;

namespace TBTT_Data.Infrastructure
{
    public abstract class BaseRepository : IBaseRepository, IDisposable
    {
        private IMongoClient _client;
        private string _databaseName;
        private IMongoDatabase _database;
        public IConfiguration configuration;
        public BaseRepository()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
        }

        public IMongoDatabase GetDatabase
        {
            get {
                var mondoDBConnectionString = configuration.GetSection("TBTTDatabaseSettings:ConnectionString").Value;
                string databaseName = configuration.GetSection("TBTTDatabaseSettings:DatabaseName").Value;
                _databaseName = databaseName;
                _client = new MongoClient(mondoDBConnectionString);
                _database = _client.GetDatabase(_databaseName);
                return _database; 
            }
        }

        public void Dispose()
        {
            _client = null;
            _database = null;
        }


    }
}
