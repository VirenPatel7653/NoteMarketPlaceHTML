using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminDownloadedNotesModel
    {
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Buyer { get; set; }
        public int BuyerID { get; set; }
        public string Seller { get; set; }
        public int SellerID { get; set; }
        public string SellType { get; set; }
        public decimal SellPrice { get; set; }

        public DateTime DownloadedDate { get; set; }
    }
}