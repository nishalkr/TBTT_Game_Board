using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using MongoDB;
using MongoDB.Driver;

namespace TBTT_Data
{
    public interface IBaseRepository
    {
        IMongoDatabase GetDatabase { get; }
    }
}
