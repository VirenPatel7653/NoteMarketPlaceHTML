using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Controllers
{
    [Authorize]
    public class ManageNotesController : Controller
    {
        // GET: ManageNotes
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();
        [HttpGet]
        public ActionResult BuyerRequest()
        {
            string emailID = User.Identity.Name;
            var id = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault().ID;
            List<BuyerRequestsModel> buyerreq = new List<BuyerRequestsModel>();
            List<Download> downloads = dbObj.Downloads.Where(a => a.Seller == id && a.IsSellerHasAllowedDownload == false).ToList();
            foreach(Download d in downloads)
            {
                BuyerRequestsModel b = new BuyerRequestsModel();
                b.ID = d.ID;
                b.NoteID = d.NoteID;
                b.Title = d.NoteTitle;
                b.Category = d.NoteCategory;
                var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
                var buyer_phonenumber = dbObj.UserProfiles.Where(a => a.UserID == buyer.ID).FirstOrDefault().Phonenumber;
                b.Buyer = buyer.EmailID;
                b.BuyerPhoneNumber = buyer_phonenumber;
                b.SellType = d.IsPaid ? "Paid" : "Free";
                b.SellingPrice = Decimal.Parse(d.PurchasedPrice.ToString());
                b.DownloadedDate = DateTime.Parse(d.CreatedDate.ToString());
                buyerreq.Add(b);
            }
           
            
            return View(buyerreq.OrderByDescending(a=>a.DownloadedDate));
        }

        [HttpGet]
        public ActionResult AllowDownload(int id)
        {
            Download d = dbObj.Downloads.Where(a => a.ID == id).FirstOrDefault();
            d.IsSellerHasAllowedDownload = true;
            dbObj.SaveChanges();
            return RedirectToAction("BuyerRequest");
        }

      

    }
}