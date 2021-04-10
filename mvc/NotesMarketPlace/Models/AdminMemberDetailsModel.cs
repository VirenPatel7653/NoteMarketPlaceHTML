using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminMemberDetailsModel
    {
        public List<AdminMemberDetailsNotesModel> ListOfNotes { get; set; }

        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Phonenumber { get; set; }
        public string ProfilePicture { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }


    }
}