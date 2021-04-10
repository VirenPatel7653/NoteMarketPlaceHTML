using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminPublishedViewModel
    {
        public IEnumerable<AdminPublishedNotesModel> listOfNotes { get; set; }
        
        public AdminRejectedNotesModel unPublishedNotes { get; set; }
        
    }
}