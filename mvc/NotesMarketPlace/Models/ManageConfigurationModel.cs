using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class ManageConfigurationModel
    {
        public string SupportEmailAddress { get; set; }
        public string SupportContactNumber { get; set; }
        public string EmailAddressesForNotify { get; set; }
        public string DefaultNoteDisplayPicture { get; set; }
        public string DefaultMemberDisplayPicture { get; set; }
        public string FBICON { get; set; }
        public string TWITTERICON { get; set; }
        public string LNICON { get; set; }

    }
}