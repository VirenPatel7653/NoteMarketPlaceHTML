using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AddInAppropriateModel
    {
        public int NoteID { get; set; }
        public int ReportedByID { get; set; }
        public int AgainstDownloadsID { get; set; }
        
        [Required]
        public string Remarks { get; set; }
    }
}