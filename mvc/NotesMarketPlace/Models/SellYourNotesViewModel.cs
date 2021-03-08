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

    }
}