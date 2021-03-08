using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class NoteViewModel
    {
        public int ID { get; set; }
        public int SellerID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string DisplayPicture { get; set; }
        public string UploadNotes { get; set; }
        public string UploadNotesName { get; set; }
        public string NoteType { get; set; }
        public Nullable<int> NumberofPages { get; set; }
        public string Description { get; set; }
        public string UniversityName { get; set; }
        public string Country { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public string IsPaid { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public string NotesPreview { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CountOfSpamReport { get; set; }
        public decimal Rating { get; set; }
        public int NoOfReviews { get; set; }
    }
}