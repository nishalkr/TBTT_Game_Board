using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TBTT_Data.Entities
{
    public class Waiting
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public Int32 WaitingListID;
        [BsonElement]
        public string MembershipID;
        [BsonElement]
        public string MemberName;
        [BsonElement]
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        [BsonElement]
        public bool NodeState;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, DateOnly = true)]
        public DateTime BoardStartDate;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BoardEndDate;
        [BsonElement]
        public Int32 OrderID;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedDate;

        public IEnumerable<Waiting> WaitingList { get; set; }
    }
}
