using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBTT_Board.Helper
{
    public class TBTTDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }


    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }
}
