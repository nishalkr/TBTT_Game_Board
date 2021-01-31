using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;


namespace TBTT_Board.Models
{
    public class GameViewModel
    { 

            public ObjectId Id { get; set; }
            public Int32 GameID;
            public string CourtName;
            public string MembershipID;
            public string MemberName;
            public string MembershipType; //M-Member, G-Guest, N-Non-Member
            public string NodeCounter;
            public DateTime GameStartDate;
            public DateTime MinGameStartDate;
            public string GameStopWatchstartMin;
            public DateTime GameStartShortDate;
            public DateTime GameEndDate;
            public Int32 OrderID;
            public DateTime UpdatedDate;
            public bool IsDoubles;

        public IEnumerable<GameViewModel> GameList { get; set; }

    }
}
