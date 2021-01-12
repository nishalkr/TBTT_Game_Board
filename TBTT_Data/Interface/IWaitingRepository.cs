using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Entities;

namespace TBTT_Data.Interface
{
    public interface IWaitingRepository : IBaseRepository
    {
        Waiting GetWaitingList(string playingName);
        int AddToWaitingList(Waiting paramWaiting);

        int WaitingListUpdateStatus(Waiting paramWaiting);
        int AddToWaitingListFromGameBoard(Game gameDTO);

        int DeleteFromWaitingListByMemberID(string memberID);
        int DeleteFromWaitingList();

        Waiting GetWaitingListByMemberName(string memberName);

        Waiting GetWaitingListByMemberID(string memberID);
    }
}
