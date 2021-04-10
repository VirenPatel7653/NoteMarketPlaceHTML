using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class ManageSystemConfigurationModel
    {
        [Display(Name = "Support emails address")]
        [Required]
        public string SupportEmailAddress { get; set; }
        [Display(Name = "Support phone number")]
        [Required]
        public string SupportContactNumber { get; set; }

        [Display(Name = "Email Address(es) (for various events system will send notifications to these users)")]
        [Required]
        public string EmailAddressesForNotify { get; set; }
        public string DefaultNoteDisplayPicture { get; set; }
        public string DefaultMemberDisplayPicture { get; set; }
        [Display(Name = "Facebook URL")]
        public string FBICON { get; set; }
        [Display(Name = "Twitter URL")]
        public string TWITTERICON { get; set; }
        [Display(Name = "Linkedin URL")]
        public string LNICON { get; set; }
        [Display(Name = "Default profile picture(if seller do not upload)")]

        public HttpPostedFileBase DisplayMemberPicture { get; set; }
        [Display(Name = "Default image for notes(if seller do not upload)")]

        public HttpPostedFileBase DisplayNotePicture { get; set; }
    }

}