using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdministratorModel
    {
        public int ID { get; set; }


        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailID { get; set; }

        public string CountryPhoneCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateAdded { get; set; }
        public string Active { get; set; }

       
    }
}