using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminDashboardViewModel
    {
        
        public List<AdminPublishedNotesModel> ListOfPublish { get; set; }
        public AdminRejectedNotesModel unPublishedNotes { get; set; }

        public int NoOfNotesInReviewForPublish { get; set; }
        public int NoOfNewNotesDownloaded { get; set; }
        public int NoOfNewRegistration { get; set; }
    }
}