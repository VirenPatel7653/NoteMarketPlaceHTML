using NotesMarketPlace.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Models
{
    public class AdministratorViewModel
    {
        public AdministratorModel Admin { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
    }
}