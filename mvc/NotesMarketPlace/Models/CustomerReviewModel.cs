using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class CustomerReviewModel
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int ReviewByID { get; set; }
        public string ReviewerFullName { get; set; }
        public string ReviewerPhoto { get; set; }
        public decimal Ratings { get; set; }
        public string Comments { get; set; }
        public DateTime createdDate { get; set; }
    }
}