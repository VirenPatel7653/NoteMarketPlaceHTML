using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Models
{
    public class AddNoteModel
    {
        public int SellerID { get; set; }
        public int Status { get; set; }

        [Required]
        public string Title { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int Category { get; set; }

        [Display(Name = "Display Picture")]
        public HttpPostedFileBase DisplayPicture { get; set; }

        [Required(ErrorMessage ="Upload Notes is Required")]
        [Display(Name = "Upload Notes")]
        
        public HttpPostedFileBase UploadNotes { get; set; }

        public IEnumerable<SelectListItem> TypeList { get; set; }

        [Display(Name = "Note Type")]
        public Nullable<int> NoteType { get; set; }

        [Display(Name = "Number Of Pages")]
        public Nullable<int> NumberofPages { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Institution Name")]
        public string UniversityName { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; }
        public Nullable<int> Country { get; set; }

        [Display(Name = "Course Name")]
        public string Course { get; set; }

        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Professor / Lecturer")]
        public string Professor { get; set; }
        public IEnumerable<SelectListItem> SellingModeList { get; set; }

        [Required(ErrorMessage = "Sell For is required.")]
        [Display(Name = "Sell For")]
        public string IsPaid { get; set; }
        [RequiredIf("IsPaid == 'P'",ErrorMessage ="Selling Price is required.")]
        [Display(Name = "Sell Price")]
        public Nullable<decimal> SellingPrice { get; set; }

        [Display(Name = "Note Preview")]
        public HttpPostedFileBase NotesPreview { get; set; }

    }
}