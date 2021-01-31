using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TBTT_Board.Models
{
    public class CourtViewModel
    {
         public ObjectId Id { get; set; }
        public string CourtName;
        public string Visibility;

        public IEnumerable<CourtViewModel> CourtList { get; set; }
        }

}
