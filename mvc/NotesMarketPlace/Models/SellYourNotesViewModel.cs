using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class SellYourNotesViewModel
    {
        public List<DashboardNotesDetailsModel> ListOfPublished { get; set; }
        public List<DashboardNotesDetailsModel> ListOfProgress { get; set; }

        public int NoOfSoldNotes { get; set; }
        public int MoneyEarned { get; set; }
        public int NoOfMyDownloads { get; set; }
        public int NoOfMyRejectedNotes { get; set; }
        public int NoOfBuyerRequests { get; set; }

    }
}