using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TBTT_Logging
{
    
    public  class Logger
    {
        private static readonly ILogger _Logger;
        //private static object configuration;
        private static IConfiguration configuration;

        static Logger()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
          
            configuration = builder.Build();

            var mondoDBConnectionString = configuration.GetSection("TBTTDatabaseSettings:ConnectionString").Value;
            string databaseName = configuration.GetSection("TBTTDatabaseSettings:DatabaseName").Value;
            string mongoConnection = string.Concat(mondoDBConnectionString, "/", databaseName);

            _Logger = new LoggerConfiguration()
                   .MinimumLevel.Information()
                    .WriteTo.MongoDB(mongoConnection, collectionName: "TBTT_Log")
                    .CreateLogger();


        }

        public static void WritePerf(LogDetail infoToLog)
        {
            _Logger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }

        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _Logger.Write(LogEventLevel.Error, "{@FlogDetail}", infoToLog);
        }

        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics = Convert.ToBoolean(configuration.GetSection("EnableDiagnostics").Value);
            if (!writeDiagnostics)
                return;

            _Logger.Write(LogEventLevel.Information, "{@FlogDetail}", infoToLog);
        }

        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }

    }


}
