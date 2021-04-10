using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminMemberDetailsNotesModel
    {
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string NoOfDownloadedNotes { get; set; }
        public int TotalEarnings { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime DateAdded { get; set; }
    }
}