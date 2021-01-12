using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TBTT_Board.Models
{
    public class WaitingViewModel
    {
        public ObjectId Id { get; set; }
        public Int32 WaitingListID;
        public string MembershipID;
        public string MemberName;
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        public bool NodeState;
        public DateTime BoardStartDate;
        public DateTime BoardEndDate;
        public Int32 OrderID;
        public DateTime UpdatedDate;

        public IEnumerable<WaitingViewModel> WaitingList { get; set; }
    }
}
