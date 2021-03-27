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
                var buyer_phonenumber = dbObj.UserProfiles.Where(a => a.UserID == buyer.ID).FirstOrDefault();
                b.Buyer = buyer.EmailID;
                if(buyer_phonenumber!=null)
                {
                    b.BuyerPhoneNumber = buyer_phonenumber.Phonenumber;

                }
                b.SellType = d.IsPaid ? "Paid" : "Free";
                b.SellingPrice = Decimal.Parse(d.PurchasedPrice.ToString());
                b.DownloadedDate = DateTime.Parse(d.CreatedDate.ToString());
                buyerreq.Add(b);
            }
           
            
            return View(buyerreq.OrderByDescending(a=>a.DownloadedDate));
        }

        [HttpGet]
        [Authorize]
        public ActionResult AllowDownload(int id)
        {
            Download d = dbObj.Downloads.Where(a => a.ID == id).FirstOrDefault();
            d.IsSellerHasAllowedDownload = true;
            dbObj.SaveChanges();
         
            var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();
            var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
            string subject = seller.FirstName +" Allows you to download a note";
            string body = "Hello " + buyer.FirstName + ",<br> We would like to inform you that," + seller.FirstName + " Allows you to download a note." +
                "Please login and see My Download tabs to download particular note." +
                "<br>Regards,<br>Notes Marketplace";
            SendEmail(buyer.EmailID, subject, body);
            TempData["Success"] = "Successfully Done.";

            return RedirectToAction("BuyerRequest");
        }

        
        [HttpGet]
        public ActionResult MyDownloadsNotes()
        {
            string emailID = User.Identity.Name;
            var id = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault().ID;
            MyDownloadsViewModel mdv = new MyDownloadsViewModel();
            List<MyDownloadsModel> downloadlist = new List<MyDownloadsModel>();
            List<Download> downloads = dbObj.Downloads.Where(a => a.Downloader == id && a.IsSellerHasAllowedDownload == true).ToList();
            foreach (Download d in downloads)
            {
                MyDownloadsModel b = new MyDownloadsModel();
                b.ID = d.ID;
                b.NoteID = d.NoteID;
                b.Title = d.NoteTitle;
                b.Category = d.NoteCategory;
                var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();
                b.Seller = seller.EmailID;
                b.SellType = d.IsPaid ? "Paid" : "Free";
                if(b.SellType == "Paid")
                {
                    b.SellingPrice = Decimal.Parse(d.PurchasedPrice.ToString());

                }

                b.DownloadedDate = DateTime.Parse(d.CreatedDate.ToString());
                downloadlist.Add(b);
            }
            mdv.ListOfDownoads = downloadlist.OrderByDescending(a => a.DownloadedDate).ToList();
            return View(mdv);
        }

        [HttpGet]
        public void DownloadNote(int id)
        {
            string email = User.Identity.Name;
            int buyerID = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault().ID;
            Download isExist = dbObj.Downloads.Where(a => a.NoteID == id && a.Downloader == buyerID).FirstOrDefault();
            isExist.IsAttachmentDownloaded = true;
            dbObj.SaveChanges();
            SellerNotesAttachment note_att = dbObj.SellerNotesAttachments.Where(a => a.NoteID == id).FirstOrDefault();

            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + note_att.FileName);
            Response.TransmitFile(Server.MapPath("~/" + note_att.FilePath));
            Response.End();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(MyDownloadsViewModel model)
        {
            Download d = dbObj.Downloads.Where(a => a.ID == model.RatingModel.AgainstDownloadsID).FirstOrDefault();
            SellerNotesReview isalready = dbObj.SellerNotesReviews.Where(a => a.AgainstDownloadsID == model.RatingModel.AgainstDownloadsID && a.ReviewedByID == d.Downloader).FirstOrDefault(); 
            if(isalready ==null)
            {
                SellerNotesReview review = new SellerNotesReview();
                review.AgainstDownloadsID = model.RatingModel.AgainstDownloadsID;
                review.NoteID = d.NoteID;
                review.ReviewedByID = d.Downloader;
                review.Ratings = model.RatingModel.Rating;
                review.Comments = model.RatingModel.Comments;
                review.CreatedDate = DateTime.Now;
                review.CreatedBy = d.Downloader;
                review.IsActive = true;
                dbObj.SellerNotesReviews.Add(review);
                dbObj.SaveChanges();
               

            }
            else
            {
                isalready.Ratings = model.RatingModel.Rating;
                isalready.Comments = model.RatingModel.Comments;
                isalready.ModifiedDate = DateTime.Now;
                isalready.ModifiedBy = d.Downloader;
                dbObj.SaveChanges();

            }
            TempData["Success"] = "Your review added successfully";
            return RedirectToAction("MyDownloadsNotes","ManageNotes");
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInAppropriate(MyDownloadsViewModel model)
        {
            Download d = dbObj.Downloads.Where(a => a.ID == model.InAppropriateModel.AgainstDownloadsID).FirstOrDefault();
            SellerNotesReportedIssue isalready = dbObj.SellerNotesReportedIssues.Where(a => a.AgainstDownloadsID == model.InAppropriateModel.AgainstDownloadsID && a.ReportedByID == d.Downloader).FirstOrDefault();
            if (isalready == null)
            {
                SellerNotesReportedIssue review = new SellerNotesReportedIssue();
                review.AgainstDownloadsID = model.InAppropriateModel.AgainstDownloadsID;
                review.NoteID = d.NoteID;
                review.ReportedByID = d.Downloader;
                review.Remarks = model.InAppropriateModel.Remarks;
                review.CreatedDate = DateTime.Now;
                review.CreatedBy = d.Downloader;
                dbObj.SellerNotesReportedIssues.Add(review);
                dbObj.SaveChanges();
            }
            else
            {
                isalready.Remarks = model.InAppropriateModel.Remarks;
                isalready.ModifiedDate = DateTime.Now;
                isalready.ModifiedBy = d.Downloader;
                dbObj.SaveChanges();
            }
            var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
            var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();

            var adminemails = dbObj.SystemConfigurations.Where(a => a.Key == "EmailAddressesForNotify").FirstOrDefault().Value;
            string [] admins=adminemails.Split(',');
            var email = ConfigurationManager.AppSettings["username"].ToString();
            var passsword = ConfigurationManager.AppSettings["password"].ToString();
            /*var fromEmail = new MailAddress(email, "Note Marketplace");*/
            var fromEmailPassword = passsword;

            
            MailMessage mailmessage = new MailMessage();
            mailmessage.From = new MailAddress(email, "Note Marketplace");
            mailmessage.Subject =buyer.FirstName+" Reported an issue for"+d.NoteTitle;
            mailmessage.Body = "Hello Admins,<br/> We want to inform you that,"+ buyer.FirstName+" Reported an issue for " +
                seller.FirstName+"’s Note with title "+d.NoteTitle+
                " Please look at the notes and take required actions.<br/> " +
                "Regards, <br/>Notes Marketplace";
            mailmessage.IsBodyHtml = true;
            foreach(string e in admins)
            { 
                mailmessage.To.Add(new MailAddress(e));
            }
            string emailSenderHost = ConfigurationManager.AppSettings["smtp"].ToString();
            int emailSenderPort = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
            Boolean emailIsSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);

            var smtp = new SmtpClient
            {
                Host = emailSenderHost,
                Port = emailSenderPort,
                EnableSsl = emailIsSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailmessage.From.Address, fromEmailPassword)
            };
            
            smtp.Send(mailmessage);
            TempData["Success"] = "Your spam report added successfully";

            return RedirectToAction("MyDownloadsNotes", "ManageNotes");
        }

        [HttpGet]
        public ActionResult MySoldNotes()
        {
            string emailID = User.Identity.Name;
            var id = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault().ID;
            List<MySoldNotesModel> mysoldnote = new List<MySoldNotesModel>();
            List<Download> downloads = dbObj.Downloads.Where(a => a.Seller == id && a.IsSellerHasAllowedDownload == true).ToList();
            foreach (Download d in downloads)
            {
                MySoldNotesModel b = new MySoldNotesModel();
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
                mysoldnote.Add(b);
            }


            return View(mysoldnote.OrderByDescending(a => a.DownloadedDate));
        }

        [HttpGet]
        public ActionResult MyRejectedNotes()
        {
            string emailID = User.Identity.Name;
            var id = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault().ID;
            var ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Rejected").FirstOrDefault().ID;

            List<MyRejectedNotesModel> myrejectednote = new List<MyRejectedNotesModel>();
            List<SellerNote> notes = dbObj.SellerNotes.Where(a => a.SellerID == id && a.Status == ref_id).ToList();
            foreach (SellerNote n in notes)
            {
                MyRejectedNotesModel my = new MyRejectedNotesModel();
                my.NoteID = n.ID;
                my.Title = n.Title;
                my.Category = dbObj.NoteCategories.Where(a=>a.ID ==n.Category).FirstOrDefault().Name;
                my.Remarks = n.AdminRemarks;
                myrejectednote.Add(my);
            }


            return View(myrejectednote);
        }

        [NonAction]
        public void SendEmail(String tomail, String subject, String body)
        {
            var email = ConfigurationManager.AppSettings["username"].ToString();
            var passsword = ConfigurationManager.AppSettings["password"].ToString();
            var fromEmail = new MailAddress(email, "Note Marketplace");
            var toEmail = new MailAddress(tomail);
            var fromEmailPassword = passsword;

            string emailSenderHost = ConfigurationManager.AppSettings["smtp"].ToString();
            int emailSenderPort = Convert.ToInt16(ConfigurationManager.AppSettings["portnumber"]);
            Boolean emailIsSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSSL"]);

            var smtp = new SmtpClient
            {
                Host = emailSenderHost,
                Port = emailSenderPort,
                EnableSsl = emailIsSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}