using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminNotesUnderReviewViewModel
    {
        public IEnumerable<AdminNotesUnderReviewModel> listOfNotes { get; set; }

       public AdminRejectedNotesModel rejectedNotes { get; set; }
    }
}