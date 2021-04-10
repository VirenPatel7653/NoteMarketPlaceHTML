using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class AdminManageTypeViewModel
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public string Active { get; set; }
        public DateTime DateAdded { get; set; }
    }
}