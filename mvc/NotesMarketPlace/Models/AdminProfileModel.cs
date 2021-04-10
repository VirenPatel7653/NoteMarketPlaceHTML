using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Models
{
    public class AdminProfileModel
    {

        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Secondary EmailAddress")]
        [EmailAddress]
        public string SecondaryEmailAddress { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public string Phonenumber_CountryCode { get; set; }
        public string Phonenumber { get; set; }
        [Display(Name = "Profile Picture Name")]
        public string ProfilePictureName { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }

    }
}