using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class DashboardNotesDetailsModel
    {
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string SellType { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime AddedDate { get; set; }
    }
}