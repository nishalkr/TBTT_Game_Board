using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TBTT_Board.Models
{
    public class MemberViewModel
    {
        public ObjectId Id { get; set; }
        public string MemberID;
        public string MemberName;
        public string AliasName;
        public DateTime DateOfBirth;
        public string Gender;
        public Int32 Score;
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        public string BillingType; //M-Monthly, Y-Yearly, D-Daily
        public Int32 Status; //1-Active, 0-In-Active
        public DateTime UpdatedDate;

        public IEnumerable<MemberViewModel> MemberList { get; set; }
    }
}
