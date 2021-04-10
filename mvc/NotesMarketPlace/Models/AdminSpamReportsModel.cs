using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminSpamReportsModel
    {
        public int ReportedIssueID { get; set; }
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ReportedBy { get; set; }
        public string Remarks { get; set; }
        public DateTime DateAdded { get; set; }
    }
}