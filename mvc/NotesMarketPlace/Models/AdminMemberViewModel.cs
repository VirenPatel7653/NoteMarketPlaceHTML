using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminMemberViewModel
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int UnderReviewNotes { get; set; }
        public int PublishedNotes { get; set; }
        public int DownloadedNotes { get; set; }
        public int TotalExpenses { get; set; }
        public int TotalEaring { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}