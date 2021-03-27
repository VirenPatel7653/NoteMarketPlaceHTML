using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*\d)(?=.*[@$!%#?&])[a-zA-Z\d@$!%#?&]{6,24}$", ErrorMessage = "It must be between 6 and 24 characters long & it must have at least 1 lowercase character & it must have at least 1 special character & it must have at least 1-digit character & It must not contain whitespaces")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string NewConfirmPassword { get; set; }
    }
}