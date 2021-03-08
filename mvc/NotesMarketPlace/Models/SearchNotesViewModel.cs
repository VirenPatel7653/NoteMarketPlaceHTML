using NotesMarketPlace.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Models
{
    public class SearchNotesViewModel
    {
        public List<NoteViewModel> ListOfNotes { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> UniversityList { get; set; }
        public List<SelectListItem> CourseList { get; set; }
        public List<SelectListItem> RatingList { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string University { get; set; }
        public string Course { get; set; }
        public string Rating { get; set; }
        public string Search { get; set; }

    }
}