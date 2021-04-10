using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminRejectedNotesModel
    {
        public int NoteID { get; set; }
        public int ActionBy { get; set; }

        [Required(ErrorMessage ="Remarks is required")]
        public string AdminRemarks { get; set; }
    }
}