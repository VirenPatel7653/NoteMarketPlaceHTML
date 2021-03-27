using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class MyDownloadsViewModel
    {
        public List<MyDownloadsModel> ListOfDownoads { get; set; }
        public AddReviewModel RatingModel { get; set; }
        public AddInAppropriateModel InAppropriateModel { get; set; }
        

    }
}