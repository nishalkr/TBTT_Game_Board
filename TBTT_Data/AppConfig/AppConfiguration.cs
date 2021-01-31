using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TBTT_Data.AppConfig
{
    public class AppConfiguration
    {
        public readonly string _connectionString = string.Empty;
        public readonly string _databaseName = string.Empty;
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _connectionString = root.GetSection("TBTTDatabaseSettings").GetSection("ConnectionString").Value;
            _databaseName = root.GetSection("TBTTDatabaseSettings").GetSection("DatabaseName").Value;            
        }
        public string ConnectionString
        {
            get => _connectionString;
        }

        public string DatabaseName
        {
            get => _databaseName;
        }


    }
}
