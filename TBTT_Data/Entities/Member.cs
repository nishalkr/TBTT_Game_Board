using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace TBTT_Data.Entities
{
    public class Member
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string MemberID;
        [BsonElement]
        public string MemberName;
        [BsonElement]
        public string AliasName;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, DateOnly = true)]
        public DateTime DateOfBirth;
        [BsonElement]
        public string Gender;
        [BsonElement]
        public Int32 Score;
        [BsonElement]
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        [BsonElement]
        public string BillingType; //M-Monthly, Y-Yearly, D-Daily
        [BsonElement]
        public Int32 Status; //1-Active, 0-In-Active
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedDate;

        public IEnumerable<Member> MemberList { get; set; }
    }
}
