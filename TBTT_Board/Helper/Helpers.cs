using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBTT_Logging;

namespace TBTT_Board.Helper
{
    public static class Helpers
    {

        public static LogDetail GetFlogDetail(string message, Exception ex)
        {
            return new LogDetail
            {
                Product = "TBTT",
                Location = "TBTT_Board",    // this application
                Layer = "Presentation Layer",                  // unattended executable invoked somehow
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }
    }
}
