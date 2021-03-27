using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class MyDownloadsModel
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Seller { get; set; }
       
        public string SellType { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime DownloadedDate { get; set; }
    }
}