using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Entities;

namespace TBTT_Data.Interface
{
    public interface IAvailableRepository : IBaseRepository
    {
        Available GetAvailableList(string AvailableName);
        Available GetAvailableListByMemberID(string memberID);
        int AddToAvailableList(Available paramAvailable);
        int DeleteFromAvailableListByMemberID(string memberID);
        int DeleteFromAvailableList();
    }
}
