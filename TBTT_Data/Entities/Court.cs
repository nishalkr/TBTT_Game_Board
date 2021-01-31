using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TBTT_Data.Entities
{
    public class Court
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string CourtName;
        [BsonElement]
        public string Visibility;
        [BsonElement]
        public string IsDoubles;

        public IEnumerable<Court> CourtList { get; set; }
    }
}
