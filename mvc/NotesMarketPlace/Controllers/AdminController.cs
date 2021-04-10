using Ionic.Zip;
using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Controllers
{
    [Authorize(Roles ="Admin,Super Admin")]
    public class AdminController : Controller
    {
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();
        [HttpGet]
        public ActionResult Dashboard()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published").FirstOrDefault().ID;
            
            List<SellerNote> listOfNotes = dbObj.SellerNotes.Where(a => a.Status == ref_id).ToList();
            List<AdminPublishedNotesModel> listOfNotesDetails = new List<AdminPublishedNotesModel>();
            listOfNotes = listOfNotes.Where(a => a.PublishedDate > DateTime.Now.AddMonths(-6)).ToList();
            foreach (SellerNote note in listOfNotes)
            {
                AdminPublishedNotesModel n = new AdminPublishedNotesModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                if (note.NoteType != null)
                {
                    n.SellType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
                }
                n.SellingPrice =(decimal)note.SellingPrice;
                if (note.PublishedDate != null)
                {
                    n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

                }
                double totalsize = 0;
                List<SellerNotesAttachment> notesAttachment = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).ToList();
                foreach(SellerNotesAttachment nat in notesAttachment)
                {
                    FileInfo info = new FileInfo(Server.MapPath("~/" + nat.FilePath));
                     totalsize = totalsize + info.Length;
                }
                double bytessize = totalsize;
                if (bytessize>1024 && bytessize < 1000000)
                {
                    double sizenote = (bytessize / (1024));
                    sizenote = Math.Round(sizenote,2);
                    n.NoteSize = sizenote.ToString() + " KB";
                }
                else if (bytessize > 1000000)
                {
                    double sizenote = (bytessize / (1024*1024));
                    sizenote = Math.Round(sizenote, 2);
                    n.NoteSize = sizenote.ToString() + " MB";
                }
                else
                {
                    
                    n.NoteSize = bytessize.ToString() + " Bytes";
                }
                n.NoOfDownloads = dbObj.Downloads.Where(a => a.NoteID == note.ID && a.IsAttachmentDownloaded == true).Count().ToString();
                var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Publisher = seller.FirstName + " " + seller.LastName;

                listOfNotesDetails.Add(n);

            }
            int progress_id1 = dbObj.ReferenceDatas.Where(a => a.Value == "In Review" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id2 = dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review" && a.IsActive == true).FirstOrDefault().ID;
            model.NoOfNotesInReviewForPublish = dbObj.SellerNotes.Where(a =>a.IsActive == true && (a.Status == progress_id1 || a.Status == progress_id2 )).Count();
            int role_id = dbObj.UserRoles.Where(a => a.Name == "Member").FirstOrDefault().ID;
            List<User> listOfUsers = dbObj.Users.Where(a => a.IsActive==true && a.RoleID == role_id).ToList();
            model.NoOfNewRegistration = listOfUsers.Where(a => a.CreatedDate > DateTime.Now.AddDays(-7)).Count();
            List<Download> listOfDownloads = dbObj.Downloads.Where(a => a.IsAttachmentDownloaded == true).ToList();
            model.NoOfNewNotesDownloaded = listOfDownloads.Where(a => a.AttachmentDownloadedDate > DateTime.Now.AddDays(-7)).Count();
            model.ListOfPublish = listOfNotesDetails.OrderByDescending(a=>a.NoOfDownloads).ToList();
            
            return View(model);
        }

        [HttpGet]
        public ActionResult NotesUnderReview()
        {
            AdminNotesUnderReviewViewModel model = new AdminNotesUnderReviewViewModel();
            List<AdminNotesUnderReviewModel> listOfNotes = new List<AdminNotesUnderReviewModel>();
            int inreview_id=dbObj.ReferenceDatas.Where(a => a.Value == "In Review").FirstOrDefault().ID;
            int submittedForReview_id=dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review").FirstOrDefault().ID;
            List<SellerNote> notes = dbObj.SellerNotes.Where(a => a.Status == inreview_id || a.Status == submittedForReview_id).ToList();
            foreach(SellerNote note in notes)
            {
                AdminNotesUnderReviewModel n = new AdminNotesUnderReviewModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
               
                if (note.CreatedDate != null)
                {
                    n.DateAdded = DateTime.Parse(note.CreatedDate.ToString());

                }
                var Seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Seller = Seller.FirstName + " " + Seller.LastName;
                n.SellerID = note.SellerID;
                n.Status = dbObj.ReferenceDatas.Where(a => a.ID == note.Status).FirstOrDefault().Value;
                listOfNotes.Add(n);
            }
            model.listOfNotes = listOfNotes.OrderBy(a => a.DateAdded);

            return View(model);
        }

        [HttpGet]
        public ActionResult DownloadNote(int id)
        {
            using (ZipFile zip = new ZipFile())
            {
                string title = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault().Title;

                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName(title);
                List<SellerNotesAttachment> files = dbObj.SellerNotesAttachments.Where(a => a.NoteID == id).ToList();
                foreach (SellerNotesAttachment file in files)
                {
                    zip.AddFile(Server.MapPath("~/" + file.FilePath), title);
                }
                string zipName = title + ".zip";
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", zipName);
                }
            }
        }
        
        [HttpGet]
        public ActionResult ApproveNotes(int id,string view)
        {

            int approve_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();
            note.Status = approve_id;
            note.PublishedDate = DateTime.Now;
            note.AdminRemarks = null;
            note.ActionedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            dbObj.SaveChanges();
            
            TempData["Success"] = note.Title+" Approved Successfully";
            
            if(view =="RejectedNotes")
            {
                return RedirectToAction("RejectedNotes");

            }
            else
            {
                 return RedirectToAction("NotesUnderReview");
            }
            

        }

        [HttpGet]
        public ActionResult InReviewNotes(int id)
        {

            int inreview_id = dbObj.ReferenceDatas.Where(a => a.Value == "In Review" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();
            note.Status = inreview_id;
            dbObj.SaveChanges();
            TempData["Success"] = note.Title + " added to In Review Successfully";
            return RedirectToAction("NotesUnderReview");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRejectedNotes(AdminNotesUnderReviewViewModel model)
        {
            int rejected_id = dbObj.ReferenceDatas.Where(a => a.Value == "Rejected" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == model.rejectedNotes.NoteID).FirstOrDefault();
            note.AdminRemarks = model.rejectedNotes.AdminRemarks;
            note.ActionedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            note.Status = rejected_id;
            note.ModifiedDate = DateTime.Now;
            dbObj.SaveChanges();
            TempData["Success"] = note.Title + " rejected Successfully";
            return RedirectToAction("NotesUnderReview");
        }

        [HttpGet]
        public ActionResult PublishedNotes()
        {
            AdminPublishedViewModel model = new AdminPublishedViewModel();
            int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published").FirstOrDefault().ID;
            List<SellerNote> listOfNotes = dbObj.SellerNotes.Where(a => a.Status == ref_id).ToList();
            List<AdminPublishedNotesModel> listOfNotesDetails = new List<AdminPublishedNotesModel>();

            foreach (SellerNote note in listOfNotes)
            {
                AdminPublishedNotesModel n = new AdminPublishedNotesModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                if (note.NoteType != null)
                {
                    n.SellType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
                }
                n.SellingPrice = (decimal)note.SellingPrice;
                if (note.PublishedDate != null)
                {
                    n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

                }
                var admin = dbObj.Users.Where(a => a.ID == note.ActionedBy).FirstOrDefault();
                n.ApprovedBy = admin.FirstName + " " + admin.LastName;
                n.NoOfDownloads = dbObj.Downloads.Where(a => a.NoteID == note.ID && a.IsAttachmentDownloaded == true).Count().ToString();
                var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Publisher = seller.FirstName + " " + seller.LastName;
                n.SellerID = note.SellerID;
                listOfNotesDetails.Add(n);

            }
            model.listOfNotes = listOfNotesDetails.OrderByDescending(a => a.PublishedDate);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUnpublishedNotes(AdminPublishedViewModel model)
        {
            int rejected_id = dbObj.ReferenceDatas.Where(a => a.Value == "Removed" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == model.unPublishedNotes.NoteID).FirstOrDefault();
            note.AdminRemarks = model.unPublishedNotes.AdminRemarks;
            note.ActionedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            note.Status = rejected_id;
            note.IsActive = false;
            dbObj.SaveChanges();
            var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
            string subject = "Sorry! We need to remove your notes from our portal.";
            string body = "Hello " + seller.FirstName + ",<br> We want to inform you that, your note " + note.Title + "<br>Please find our remarks as below -<br>" +
               note.AdminRemarks +
                "<br>Regards,<br>Notes Marketplace";
            try
            {
                SendEmail(seller.EmailID, subject, body);
            }
            catch(Exception e)
            {

            }
            TempData["Success"] = note.Title + " Removed Successfully";
            return RedirectToAction("PublishedNotes");
        }

        [HttpGet]
        public ActionResult DownloadedNotes()
        {
            List<Download> listOfNotes = dbObj.Downloads.Where(a => a.IsAttachmentDownloaded == true && a.Seller != a.Downloader).ToList();
            List<AdminDownloadedNotesModel> listOfNotesDetails = new List<AdminDownloadedNotesModel>();

            foreach (Download d in listOfNotes)
            {
                AdminDownloadedNotesModel downloadNote = new AdminDownloadedNotesModel();
                downloadNote.NoteID = d.NoteID;
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == d.NoteID).FirstOrDefault();
                downloadNote.Title = note.Title;
                downloadNote.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                downloadNote.SellType = note.IsPaid ? "Paid" : "Free";
                downloadNote.SellPrice = (decimal)note.SellingPrice;
                if (d.AttachmentDownloadedDate != null)
                {
                    downloadNote.DownloadedDate = DateTime.Parse(d.AttachmentDownloadedDate.ToString());

                }
              
                var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();
                downloadNote.Seller = seller.FirstName + " " + seller.LastName;
                downloadNote.SellerID = seller.ID;

                var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
                downloadNote.Buyer = buyer.FirstName + " " + buyer.LastName;
                downloadNote.BuyerID = buyer.ID;

                listOfNotesDetails.Add(downloadNote);

            }
            return View(listOfNotesDetails.OrderByDescending(a => a.DownloadedDate));
        }

        [HttpGet]
        public ActionResult RejectedNotes()
        {
            AdminPublishedViewModel model = new AdminPublishedViewModel();
            int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Rejected").FirstOrDefault().ID;
            List<SellerNote> listOfNotes = dbObj.SellerNotes.Where(a => a.Status == ref_id).ToList();
            List<AdminRejectedNotesViewModel> listOfNotesDetails = new List<AdminRejectedNotesViewModel>();

            foreach (SellerNote note in listOfNotes)
            {
                AdminRejectedNotesViewModel n = new AdminRejectedNotesViewModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                
                if (note.ModifiedDate != null)
                {
                    n.DateAdded = DateTime.Parse(note.ModifiedDate.ToString());

                }
                
                var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Seller = seller.FirstName + " " + seller.LastName;
                n.SellerID = seller.ID;
                var admin = dbObj.Users.Where(a => a.ID == note.ActionedBy).FirstOrDefault();
                n.RejectedBy = admin.FirstName + " " + admin.LastName;
                n.Remarks = note.AdminRemarks;
                

                listOfNotesDetails.Add(n);

            }
          
            return View(listOfNotesDetails.OrderByDescending(a => a.DateAdded));
        }

        [HttpGet]
        public ActionResult DownloadedSpecific(int id)
        {
            List<Download> listOfNotes = dbObj.Downloads.Where(a => a.IsAttachmentDownloaded == true && a.NoteID == id && a.Seller != a.Downloader).ToList();
            List<AdminDownloadedNotesModel> listOfNotesDetails = new List<AdminDownloadedNotesModel>();

            foreach (Download d in listOfNotes)
            {
                AdminDownloadedNotesModel downloadNote = new AdminDownloadedNotesModel();
                downloadNote.NoteID = d.NoteID;
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == d.NoteID).FirstOrDefault();
                downloadNote.Title = note.Title;
                downloadNote.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                downloadNote.SellType = note.IsPaid ? "Paid" : "Free";
                downloadNote.SellPrice = (decimal)note.SellingPrice;
                if (d.AttachmentDownloadedDate != null)
                {
                    downloadNote.DownloadedDate = DateTime.Parse(d.AttachmentDownloadedDate.ToString());

                }

                var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();
                downloadNote.Seller = seller.FirstName + " " + seller.LastName;
                downloadNote.SellerID = seller.ID;

                var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
                downloadNote.Buyer = buyer.FirstName + " " + buyer.LastName;
                downloadNote.BuyerID = buyer.ID;

                listOfNotesDetails.Add(downloadNote);

            }
            return View("DownloadedNotes", listOfNotesDetails.OrderByDescending(a => a.DownloadedDate));
        }

        [HttpGet]
        public ActionResult Members()
        {
            List<AdminMemberViewModel> members = new List<AdminMemberViewModel>();
           
            int progress_id1 = dbObj.ReferenceDatas.Where(a => a.Value == "In Review" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id2 = dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review" && a.IsActive == true).FirstOrDefault().ID;
            int publish_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published" && a.IsActive == true).FirstOrDefault().ID;
            int member_role = dbObj.UserRoles.Where(a => a.Name== "Member" && a.IsActive == true).FirstOrDefault().ID;
            List<User> users = dbObj.Users.Where(a =>a.RoleID == member_role && a.IsActive == true).ToList();
            
            foreach (var u in users)
            {
                AdminMemberViewModel m = new AdminMemberViewModel();
                m.MemberID = u.ID;
                m.FirstName = u.FirstName;
                m.LastName = u.LastName;
                m.Email = u.EmailID;
                m.JoiningDate = DateTime.Parse(u.CreatedDate.ToString());
                m.DownloadedNotes = dbObj.Downloads.Where(a => a.Downloader == u.ID && a.Seller != u.ID && a.IsAttachmentDownloaded == true).Count();
                m.UnderReviewNotes = dbObj.SellerNotes.Where(a =>a.SellerID == u.ID && (a.Status == progress_id1 || a.Status == progress_id2)).Count();
                m.PublishedNotes = dbObj.SellerNotes.Where(a => a.Status == publish_id && a.SellerID == u.ID).Count();
                List<Download> expenselist = dbObj.Downloads.Where(a => a.Downloader == u.ID && a.Seller != u.ID).ToList();
                m.TotalExpenses = Convert.ToInt32(expenselist.Sum(a => a.PurchasedPrice));
                List<Download> earninglist = dbObj.Downloads.Where(a => a.Seller == u.ID && a.Downloader!=u.ID && a.IsSellerHasAllowedDownload == true).ToList();
                m.TotalEaring = Convert.ToInt32(earninglist.Sum(a => a.PurchasedPrice));

                members.Add(m);
               
            }

            return View(members.OrderByDescending(a=>a.JoiningDate));
        }

        [HttpGet]
        public ActionResult NotesUnderReviewSpecificMember(int id)
        {
            AdminNotesUnderReviewViewModel model = new AdminNotesUnderReviewViewModel();
            List<AdminNotesUnderReviewModel> listOfNotes = new List<AdminNotesUnderReviewModel>();
            int inreview_id = dbObj.ReferenceDatas.Where(a => a.Value == "In Review").FirstOrDefault().ID;
            int submittedForReview_id = dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review").FirstOrDefault().ID;
            List<SellerNote> notes = dbObj.SellerNotes.Where(a => a.SellerID==id && (a.Status == inreview_id || a.Status == submittedForReview_id)).ToList();
            foreach (SellerNote note in notes)
            {
                AdminNotesUnderReviewModel n = new AdminNotesUnderReviewModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;

                if (note.CreatedDate != null)
                {
                    n.DateAdded = DateTime.Parse(note.CreatedDate.ToString());

                }
                var Seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Seller = Seller.FirstName + " " + Seller.LastName;
                n.Status = dbObj.ReferenceDatas.Where(a => a.ID == note.Status).FirstOrDefault().Value;
                listOfNotes.Add(n);
            }
            model.listOfNotes = listOfNotes.OrderBy(a => a.DateAdded);

            return View("NotesUnderReview",model);
        }

        [HttpGet]
        public ActionResult PublishedNotesSpecificMember(int id)
        {
            AdminPublishedViewModel model = new AdminPublishedViewModel();
            int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published").FirstOrDefault().ID;
            List<SellerNote> listOfNotes = dbObj.SellerNotes.Where(a => a.SellerID == id && a.Status == ref_id).ToList();
            List<AdminPublishedNotesModel> listOfNotesDetails = new List<AdminPublishedNotesModel>();

            foreach (SellerNote note in listOfNotes)
            {
                AdminPublishedNotesModel n = new AdminPublishedNotesModel();
                n.NoteID = note.ID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                if (note.NoteType != null)
                {
                    n.SellType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
                }
                n.SellingPrice = (decimal)note.SellingPrice;
                if (note.PublishedDate != null)
                {
                    n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

                }
                var admin = dbObj.Users.Where(a => a.ID == note.ActionedBy).FirstOrDefault();
                n.ApprovedBy = admin.FirstName + " " + admin.LastName;
                n.NoOfDownloads = dbObj.Downloads.Where(a => a.NoteID == note.ID && a.IsAttachmentDownloaded == true).Count().ToString();
                var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
                n.Publisher = seller.FirstName + " " + seller.LastName;
                listOfNotesDetails.Add(n);

            }
            model.listOfNotes = listOfNotesDetails.OrderBy(a => a.PublishedDate);
            return View("PublishedNotes",model);
        }

        [HttpGet]
        public ActionResult DownloadedSpecificMember(int id)
        {
            List<Download> listOfNotes = dbObj.Downloads.Where(a => a.IsAttachmentDownloaded == true && a.Seller == id && a.Downloader!=id).ToList();
            List<AdminDownloadedNotesModel> listOfNotesDetails = new List<AdminDownloadedNotesModel>();

            foreach (Download d in listOfNotes)
            {
                AdminDownloadedNotesModel downloadNote = new AdminDownloadedNotesModel();
                downloadNote.NoteID = d.NoteID;
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == d.NoteID).FirstOrDefault();
                downloadNote.Title = note.Title;
                downloadNote.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                downloadNote.SellType = note.IsPaid ? "Paid" : "Free";
                downloadNote.SellPrice = (decimal)note.SellingPrice;
                if (d.AttachmentDownloadedDate != null)
                {
                    downloadNote.DownloadedDate = DateTime.Parse(d.AttachmentDownloadedDate.ToString());

                }

                var seller = dbObj.Users.Where(a => a.ID == d.Seller).FirstOrDefault();
                downloadNote.Seller = seller.FirstName + " " + seller.LastName;
                downloadNote.SellerID = seller.ID;

                var buyer = dbObj.Users.Where(a => a.ID == d.Downloader).FirstOrDefault();
                downloadNote.Buyer = buyer.FirstName + " " + buyer.LastName;
                downloadNote.BuyerID = buyer.ID;

                listOfNotesDetails.Add(downloadNote);

            }
            return View("DownloadedNotes", listOfNotesDetails.OrderByDescending(a => a.DownloadedDate));
        }

        [HttpGet]
        public ActionResult MemberDetails(int id)
        {
            AdminMemberDetailsModel member = new AdminMemberDetailsModel();
            member.ListOfNotes = new List<AdminMemberDetailsNotesModel>();
            User userbasic = dbObj.Users.Where(a => a.ID == id).FirstOrDefault();
            UserProfile user = dbObj.UserProfiles.Where(a => a.UserID == id).FirstOrDefault();
            member.ID = id;
            member.FirstName = userbasic.FirstName;
            member.LastName = userbasic.LastName;
            member.EmailID = userbasic.EmailID;
            member.Phonenumber = user.Phonenumber;
            member.DOB = user.DOB;
            member.College = user.College;
            member.University = user.University;
            member.AddressLine1 = user.AddressLine1;
            member.AddressLine2 = user.AddressLine2;
            member.City = user.City;
            member.State = user.State;
            member.Country = user.Country;
            member.ZipCode = user.ZipCode;
            member.ProfilePicture = user.ProfilePicture;
            int progress_id1 = dbObj.ReferenceDatas.Where(a => a.Value == "In Review" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id2 = dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review" && a.IsActive == true).FirstOrDefault().ID;
            int publish_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published" && a.IsActive == true).FirstOrDefault().ID;
            List<SellerNote> notes = dbObj.SellerNotes.Where(a => a.SellerID == id && (a.Status == progress_id1 || a.Status == progress_id2 || a.Status == publish_id)).ToList();
            foreach(var n in notes)
            {
                AdminMemberDetailsNotesModel note = new AdminMemberDetailsNotesModel();
                note.NoteID = n.ID;
                note.Title = n.Title;
                note.Category = dbObj.NoteCategories.Where(a => a.ID == n.Category).FirstOrDefault().Name;
                note.Status = dbObj.ReferenceDatas.Where(a => a.ID == n.Status).FirstOrDefault().Value;
                note.NoOfDownloadedNotes = dbObj.Downloads.Where(a => a.NoteID == n.ID && a.IsAttachmentDownloaded == true).Count().ToString();
                List<Download> earninglist = dbObj.Downloads.Where(a => a.Seller == n.ID && a.Downloader != user.ID && a.IsSellerHasAllowedDownload == true).ToList();
                note.TotalEarnings = Convert.ToInt32(earninglist.Sum(a => a.PurchasedPrice));
                if (n.CreatedDate != null)
                {
                    note.DateAdded = DateTime.Parse(n.CreatedDate.ToString());

                }
                if (n.PublishedDate != null)
                {
                    note.PublishedDate = DateTime.Parse(n.PublishedDate.ToString());

                }

                member.ListOfNotes.Add(note);

            }

            return View(member);
        }

        [HttpGet]
        public ActionResult DeactivateMember(int id)
        {
            List<SellerNote> notes = dbObj.SellerNotes.Where(a => a.SellerID == id).ToList();
            int rejected_id = dbObj.ReferenceDatas.Where(a => a.Value == "Removed" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            foreach(SellerNote n in notes)
            {
                n.Status = rejected_id;
                n.IsActive = false;
                List<SellerNotesAttachment> note_att = dbObj.SellerNotesAttachments.Where(a => a.NoteID == n.ID).ToList();
                foreach(SellerNotesAttachment n_at in note_att)
                {
                    n_at.IsActive = false;
                }
                
            }

            User u = dbObj.Users.Where(a => a.ID == id).FirstOrDefault();
            u.IsActive = false;
            dbObj.SaveChanges();
            TempData["Success"] = u.FirstName + " Deactivated Successfully.";
            return RedirectToAction("Members");
        }

        [HttpGet]
        public ActionResult SpamReports()
        {
            List<AdminSpamReportsModel> listOfReports = new List<AdminSpamReportsModel>();
            List<SellerNotesReportedIssue> listOfIssue = dbObj.SellerNotesReportedIssues.ToList();
            foreach(SellerNotesReportedIssue issue in listOfIssue)
            {
                AdminSpamReportsModel model = new AdminSpamReportsModel();
                model.NoteID = issue.NoteID;
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == issue.NoteID).FirstOrDefault();
                model.Title = note.Title;
                model.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                model.Remarks = issue.Remarks;
                User reported = dbObj.Users.Where(a => a.ID == issue.ReportedByID).FirstOrDefault();
                model.ReportedBy = reported.FirstName + " " + reported.LastName;
                model.ReportedIssueID = issue.ID;
                if (issue.CreatedDate != null)
                {
                    model.DateAdded = DateTime.Parse(issue.CreatedDate.ToString());

                }
                listOfReports.Add(model);
            }
            return View(listOfReports.OrderByDescending(a=>a.DateAdded));

        }
        
        [HttpGet]
        public ActionResult DeleteReportedIssue(int id)
        {
            var report = dbObj.SellerNotesReportedIssues.Where(a => a.ID == id).FirstOrDefault();
            dbObj.SellerNotesReportedIssues.Remove(report);
            dbObj.SaveChanges();
            TempData["Success"] = "ReportedIssue deleted successfully";
            return RedirectToAction("SpamReports");
        }
        [HttpGet]
        public ActionResult DeleteReview(int id)
        {
            var report = dbObj.SellerNotesReviews.Where(a => a.ID == id).FirstOrDefault();
            var note_id = report.NoteID;
            dbObj.SellerNotesReviews.Remove(report);
            dbObj.SaveChanges();
            TempData["Success"] = "Review deleted successfully";
            return RedirectToAction("NoteDetails","Notes",new { id = note_id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUnpublishedNotesDashboard(AdminDashboardViewModel model)
        {
            int rejected_id = dbObj.ReferenceDatas.Where(a => a.Value == "Removed" && a.RefCategory == "Notes Status").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == model.unPublishedNotes.NoteID).FirstOrDefault();
            note.AdminRemarks = model.unPublishedNotes.AdminRemarks;
            note.ActionedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            note.Status = rejected_id;
            note.IsActive = false;
            dbObj.SaveChanges();

            var seller = dbObj.Users.Where(a => a.ID == note.SellerID).FirstOrDefault();
            string subject = "Sorry! We need to remove your notes from our portal.";
            string body = "Hello " + seller.FirstName + ",<br> We want to inform you that, your note " + note.Title + "<br>Please find our remarks as below -<br>" +
               note.AdminRemarks +
                "<br>Regards,<br>Notes Marketplace";

            try
            {
                SendEmail(seller.EmailID, subject, body);
            }
            catch (Exception e)
            {

            }

            TempData["Success"] = note.Title + " Removed Successfully";
            return RedirectToAction("Dashboard");
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