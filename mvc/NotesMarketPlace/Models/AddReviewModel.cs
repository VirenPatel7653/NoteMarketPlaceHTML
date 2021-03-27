using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AddReviewModel
    {
        public int NoteID { get; set; }
        public int ReviewByID { get; set; }
        public int AgainstDownloadsID { get; set; }
        [Required]
        public decimal Rating { get; set; }
        [Required]
        public string Comments { get; set; }

     
    }
}