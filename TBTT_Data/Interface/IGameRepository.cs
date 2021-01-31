using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Entities;

namespace TBTT_Data.Interface
{
    public interface IGameRepository: IBaseRepository
    {
        int AddToGameBoard(Game paramGame, int intMemberCount);
        Game GetGameBoard(string courtName);

        bool IsValidToDeleteFromGameBoard(string courtName);
        Game GetGameBoardOrderByOrderID(string courtName);

        Game GetGameBoardByLimitedNumber(string courtName, int intMemberCount = 4);
        int DeleteFromGameBoard(string courtName, int intMemberCount);

        Game GetGameBoardByMemberName(string memberName);
        Game GetGameBoardByCourtAndMemberName(string courtName, string memberName);
        int DeleteFromGameBoardByMemberName(string courtName, string memberName);
        int DeleteFromGameBoardOnVisibility(string courtName);
        
    }
}
