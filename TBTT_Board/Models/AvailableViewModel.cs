using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TBTT_Board.Models
{
    public class AvailableViewModel
    {
        public ObjectId Id { get; set; }
        public Int32 AvailableListID;
        public string MembershipID;
        public string MemberName;
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        public bool NodeState;
        public DateTime BoardStartDate;
        public DateTime BoardEndDate;
        public DateTime UpdatedDate;

        public IEnumerable<AvailableViewModel> AvailableList { get; set; }
        public IEnumerable<CourtViewModel> CourtList { get; set; }
    }
}
