using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TBTT_Data.Entities
{
    public class Game
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public Int32 GameID;
        [BsonElement]
        public string CourtName;
        [BsonElement]
        public string MembershipID;
        [BsonElement]
        public string MemberName;
        [BsonElement]
        public string MembershipType; //M-Member, G-Guest, N-Non-Member
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime GameStartDate;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, DateOnly = true)]
        public DateTime GameStartShortDate;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime GameEndDate;
        [BsonElement]
        public Int32 OrderID;
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedDate;
        [BsonElement]
        public bool IsDoubles;

        public IEnumerable<Game> GameList { get; set; }
    }
}
