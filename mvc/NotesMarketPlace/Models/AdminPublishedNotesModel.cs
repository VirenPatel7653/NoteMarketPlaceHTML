using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminPublishedNotesModel
    {
        public int NoteID { get; set; }
        public int SellerID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string NoteSize { get; set; }
        public string SellType { get; set; }
        public string Publisher { get; set; }
        public string ApprovedBy { get; set; }
        public string NoOfDownloads { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}