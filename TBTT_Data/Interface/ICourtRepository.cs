using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Entities;

namespace TBTT_Data.Interface
{
    public interface ICourtRepository : IBaseRepository
    {

        Court GetCourtList();
        Court GetCourtByName(string courtName);
    }
}
