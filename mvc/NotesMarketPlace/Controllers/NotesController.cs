using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Controllers
{
    public class NotesController : Controller
    {
        // GET: Notes
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();

        [HttpPost]
        public ActionResult getNotes(string Search, string CountryList, string CategoryList, string RatingList, string UniversityList, string CourseList, string TypeList)
        {

            if (Search == "" && CountryList == "" && CategoryList == "" && RatingList == "" && UniversityList == "" && CourseList == "" && TypeList == "")
            {
                int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published").FirstOrDefault().ID;
                List<SellerNote> listOfNotes1 = dbObj.SellerNotes.Where(a => a.Status == ref_id).ToList();
                List<NoteViewModel> listOfNotesDetails = new List<NoteViewModel>();

                foreach (SellerNote note in listOfNotes1)
                {
                    NoteViewModel n = new NoteViewModel();
                    n.ID = note.ID;
                    n.SellerID = note.SellerID;
                    n.Title = note.Title;
                    n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                    n.DisplayPicture = "/" + note.DisplayPicture;

                    var uploadObj = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).FirstOrDefault();
                    n.UploadNotes = uploadObj.FilePath;
                    n.UploadNotesName = uploadObj.FileName;
                    if (note.NoteType != null)
                    {
                        n.NoteType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
                    }
                    n.NumberofPages = note.NumberofPages;
                    n.Description = note.Description;
                    n.UniversityName = note.UniversityName;
                    if (note.Country != null)
                    {
                        n.Country = dbObj.Countries.Where(a => a.ID == note.Country).FirstOrDefault().Name;
                    }
                    n.Course = note.Course;
                    n.CourseCode = note.CourseCode;
                    n.Professor = note.Professor;
                    var sellingMode = note.IsPaid ? "P" : "F";
                    n.IsPaid = dbObj.ReferenceDatas.Where(a => a.DataValue == sellingMode).FirstOrDefault().Value;
                    n.SellingPrice = note.SellingPrice;
                    n.NotesPreview = note.NotesPreview;
                    if (note.PublishedDate != null)
                    {
                        n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

                    }

                    n.CountOfSpamReport = dbObj.SellerNotesReportedIssues.Where(a => a.NoteID == note.ID).Count();
                    var ratingObj = dbObj.SellerNotesReviews.Where(a => a.NoteID == note.ID).ToList();
                    if (ratingObj.Count() != 0)
                    {

                        n.Rating = ratingObj.Average(a => a.Ratings);
                        n.NoOfReviews = ratingObj.Count();
                    }


                    listOfNotesDetails.Add(n);


                }
                return Json(listOfNotesDetails);
            }


            int Type = (TypeList != "") ? int.Parse(TypeList) : 0;
            int Country = (CountryList != "") ? int.Parse(CountryList) : 0;
            int Category = (CategoryList != "") ? int.Parse(CategoryList) : 0;
            int NoteID = (RatingList != "") ? int.Parse(RatingList) : 0;

            List<SellerNote> listOfNotes = new List<SellerNote>();
            List<SellerNote> temp = new List<SellerNote>();
            if (CountryList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.Country == Country).ToList();
                listOfNotes.AddRange(temp);
            }
            if (CategoryList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.Category == Category).ToList();
                listOfNotes.AddRange(temp);
            }
            if (RatingList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.ID == NoteID).ToList();
                listOfNotes.AddRange(temp);
            }
            if (UniversityList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.UniversityName == UniversityList).ToList();
                listOfNotes.AddRange(temp);
            }
            if (CourseList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.Course == CourseList).ToList();
                listOfNotes.AddRange(temp);
            }
            if (TypeList != null)
            {
                temp = dbObj.SellerNotes.Where(a => a.NoteType == Type).ToList();
                listOfNotes.AddRange(temp);
            }
            if (Search != null)
            {
                temp = dbObj.SellerNotes.Where(a =>
                a.Title.Contains(Search.Trim()) ||
                a.Description.Contains(Search.Trim()) ||
                a.UniversityName.Contains(Search.Trim()) ||
                a.CourseCode.Contains(Search.Trim()) ||
                a.Course.Contains(Search.Trim()) ||
                a.Professor.Contains(Search.Trim())
                ).ToList();
                listOfNotes.AddRange(temp);
            }

            List<NoteViewModel> listOfNotesDetails1 = new List<NoteViewModel>();

            foreach (SellerNote note in listOfNotes.Distinct())
            {
                NoteViewModel n = new NoteViewModel();
                n.ID = note.ID;
                n.SellerID = note.SellerID;
                n.Title = note.Title;
                n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                n.DisplayPicture = "/" + note.DisplayPicture;

                var uploadObj = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).FirstOrDefault();
                n.UploadNotes = uploadObj.FilePath;
                n.UploadNotesName = uploadObj.FileName;
                if (note.NoteType != null)
                {
                    n.NoteType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
                }
                n.NumberofPages = note.NumberofPages;
                n.Description = note.Description;
                n.UniversityName = note.UniversityName;
                if (note.Country != null)
                {
                    n.Country = dbObj.Countries.Where(a => a.ID == note.Country).FirstOrDefault().Name;
                }
                n.Course = note.Course;
                n.CourseCode = note.CourseCode;
                n.Professor = note.Professor;
                var sellingMode = note.IsPaid ? "P" : "F";
                n.IsPaid = dbObj.ReferenceDatas.Where(a => a.DataValue == sellingMode).FirstOrDefault().Value;
                n.SellingPrice = note.SellingPrice;
                n.NotesPreview = note.NotesPreview;
                if (note.PublishedDate != null)
                {
                    n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

                }

                n.CountOfSpamReport = dbObj.SellerNotesReportedIssues.Where(a => a.NoteID == note.ID).Count();
                var ratingObj = dbObj.SellerNotesReviews.Where(a => a.NoteID == note.ID).ToList();
                if (ratingObj.Count() != 0)
                {

                    n.Rating = ratingObj.Average(a => a.Ratings);
                    n.NoOfReviews = ratingObj.Count();
                }
                listOfNotesDetails1.Add(n);


            }


            return Json(listOfNotesDetails1);
        }


        [HttpGet]
        public ActionResult SearchNotes()
        {
            SearchNotesViewModel model = new SearchNotesViewModel();
            List<SelectListItem> CategoryList = dbObj.NoteCategories.Where(a => a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.Name,
                   Value = x.ID.ToString()
               }).ToList();

            List<SelectListItem> TypeList = dbObj.NoteTypes.Where(a => a.IsActive == true)
              .Select(x =>
              new SelectListItem()
              {
                  Text = x.Name,
                  Value = x.ID.ToString()
              }).ToList();

            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
             .Select(x =>
             new SelectListItem()
             {
                 Text = x.Name,
                 Value = x.ID.ToString()
             }).ToList();

            List<SelectListItem> UniversityList = dbObj.SellerNotes
             .Select(x =>
             new SelectListItem()
             {
                 Text = x.UniversityName,
                 Value = x.UniversityName
             }).Distinct().ToList();

            List<SelectListItem> CourseList = dbObj.SellerNotes
            .Select(x =>
            new SelectListItem()
            {
                Text = x.Course,
                Value = x.Course
            }).Distinct().ToList();

            List<SelectListItem> RatingList = dbObj.SellerNotesReviews
            .Select(x =>
            new SelectListItem()
            {
                Text = x.Ratings.ToString(),
                Value = x.NoteID.ToString()
            }).Distinct().ToList();


            model.CategoryList = CategoryList;
            model.CourseList = CourseList;
            model.UniversityList = UniversityList;
            model.TypeList = TypeList;
            model.RatingList = RatingList;
            model.CountryList = CountryList;
            return View(model);
        }

        [HttpGet]
        public ActionResult NoteDetails(int ID)
        {
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == ID).FirstOrDefault();
            NoteViewModel n = new NoteViewModel();
            n.ID = note.ID;
            n.SellerID = note.SellerID;
            n.Title = note.Title;
            n.Category = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
            n.DisplayPicture = note.DisplayPicture;
            var uploadObj = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).FirstOrDefault();
            n.UploadNotes = uploadObj.FilePath;
            n.UploadNotesName = uploadObj.FileName;
            if (note.NoteType != null)
            {
                n.NoteType = dbObj.NoteTypes.Where(a => a.ID == note.NoteType).FirstOrDefault().Name;
            }
            n.NumberofPages = note.NumberofPages;
            n.Description = note.Description;
            n.UniversityName = note.UniversityName;
            if (note.Country != null)
            {
                n.Country = dbObj.Countries.Where(a => a.ID == note.Country).FirstOrDefault().Name;
            }
            n.Course = note.Course;
            n.CourseCode = note.CourseCode;
            n.Professor = note.Professor;
            var sellingMode = note.IsPaid ? "P" : "F";
            n.IsPaid = dbObj.ReferenceDatas.Where(a => a.DataValue == sellingMode).FirstOrDefault().Value;
            n.SellingPrice = note.SellingPrice;
            n.NotesPreview = note.NotesPreview;
            if (note.PublishedDate != null)
            {
                n.PublishedDate = DateTime.Parse(note.PublishedDate.ToString());

            }

            n.CountOfSpamReport = dbObj.SellerNotesReportedIssues.Where(a => a.NoteID == note.ID).Count();
            var ratingObj = dbObj.SellerNotesReviews.Where(a => a.NoteID == note.ID).ToList();
            if (ratingObj.Count() != 0)
            {

                n.Rating = ratingObj.Average(a => a.Ratings);
                n.NoOfReviews = ratingObj.Count();
            }
            List<SellerNotesReview> listOfReview = dbObj.SellerNotesReviews.Where(a => a.NoteID == ID).ToList();
            n.Reviews = new List<CustomerReviewModel>();
            foreach(SellerNotesReview r in listOfReview)
            {
                CustomerReviewModel m = new CustomerReviewModel();
                m.ID = r.ID;
                m.NoteID = r.NoteID;
                m.ReviewByID = r.ReviewedByID;
                var user = dbObj.Users.Where(a => a.ID == r.ReviewedByID).FirstOrDefault();
                m.ReviewerFullName = user.FirstName + user.LastName;
                m.ReviewerPhoto = dbObj.UserProfiles.Where(a => a.UserID == r.ReviewedByID).FirstOrDefault().ProfilePicture;
                m.Comments = r.Comments;
                m.Ratings = r.Ratings;
                n.Reviews.Add(m);
            }
            return View(n);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SellYourNotes()
        {
            SellYourNotesViewModel model = new SellYourNotesViewModel();
            string emailID = User.Identity.Name;
            var id = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault().ID;
            int publish_id = dbObj.ReferenceDatas.Where(a => a.Value == "Published" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id1 = dbObj.ReferenceDatas.Where(a => a.Value == "In Review" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id2 = dbObj.ReferenceDatas.Where(a => a.Value == "Submitted For Review" && a.IsActive == true).FirstOrDefault().ID;
            int progress_id3 = dbObj.ReferenceDatas.Where(a => a.Value == "Draft" && a.IsActive == true).FirstOrDefault().ID;
            List<DashboardNotesDetailsModel> ListOfPubilsh = new List<DashboardNotesDetailsModel>();
            List<DashboardNotesDetailsModel> ListOfProgress = new List<DashboardNotesDetailsModel>();
            List<SellerNote> publishStatus = dbObj.SellerNotes.Where(a => a.Status == publish_id && a.SellerID == id  && a.IsActive == true).ToList();
            List<SellerNote> progressStatus = dbObj.SellerNotes.Where(a => a.SellerID == id && a.IsActive == true && (a.Status == progress_id1 || a.Status == progress_id2 || a.Status == progress_id3)).ToList();
            foreach (var a in publishStatus)
            {
                DashboardNotesDetailsModel d = new DashboardNotesDetailsModel();
                d.NoteID = a.ID;
                d.Title = a.Title;
                d.Category = dbObj.NoteCategories.Where(x => x.ID == a.Category).FirstOrDefault().Name;
                if (a.PublishedDate != null)
                {
                    d.AddedDate = DateTime.Parse(a.PublishedDate.ToString());
                }
                d.SellType = a.IsPaid ? "Paid" : "Free";
                if (d.SellType == "Paid")
                {
                    d.SellingPrice = Decimal.Parse(a.SellingPrice.ToString());
                }
                ListOfPubilsh.Add(d);
            }
            foreach (var a in progressStatus)
            {
                DashboardNotesDetailsModel d = new DashboardNotesDetailsModel();
                d.NoteID = a.ID;
                d.Title = a.Title;
                d.Category = dbObj.NoteCategories.Where(x => x.ID == a.Category).FirstOrDefault().Name;
                d.AddedDate = DateTime.Parse(a.CreatedDate.ToString());
                d.Status = dbObj.ReferenceDatas.Where(x => x.ID == a.Status).FirstOrDefault().Value;
                ListOfProgress.Add(d);
            }

            model.ListOfPublished = ListOfPubilsh;
            model.ListOfProgress = ListOfProgress;

          
            model.NoOfSoldNotes = dbObj.Downloads.Where(a => a.Seller == id && a.IsSellerHasAllowedDownload == true).ToList().Count();
            model.NoOfMyDownloads = dbObj.Downloads.Where(a => a.Downloader == id && a.IsSellerHasAllowedDownload == true).ToList().Count();
            int rejected_id = dbObj.ReferenceDatas.Where(a => a.Value == "Rejected" && a.IsActive == true).FirstOrDefault().ID;

            model.NoOfMyRejectedNotes = dbObj.SellerNotes.Where(a => a.SellerID == id &&  a.Status == rejected_id).ToList().Count();
            model.NoOfBuyerRequests = dbObj.Downloads.Where(a => a.Seller == id && a.IsSellerHasAllowedDownload == false).ToList().Count();
            List<Download> listdo = dbObj.Downloads.Where(a => a.Seller == id && a.IsSellerHasAllowedDownload == true).ToList();
            model.MoneyEarned = Convert.ToInt32(listdo.Sum(a => a.PurchasedPrice));
            return View(model);
        }
        [HttpGet]
        [Authorize]
        public ActionResult SaveNotes()
        {
            AddNoteModel note = getAllList();
            return View(note);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SaveNotes(AddNoteModel model, string submit)
        {
            if(model.ID!=0)
            {
                ModelState.Remove("UploadNotes");
            }

            if (!ModelState.IsValid)
            {
                AddNoteModel note_list = getAllList();
                model.CategoryList = note_list.CategoryList;
                model.CountryList = note_list.CountryList;
                model.TypeList = note_list.TypeList;
                model.SellingModeList = note_list.SellingModeList;
                TempData["Error"] = "Please enter valid information";

                return View("SaveNotes", model);
            }
            string emailID = User.Identity.Name;
            var existUser = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            SellerNote note = new SellerNote();
            SellerNotesAttachment note_attach = new SellerNotesAttachment();
            if (model.ID != 0)
            {
                note = dbObj.SellerNotes.Where(a => a.ID == model.ID).FirstOrDefault();
                note_attach = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).FirstOrDefault();
                int id = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                note.ModifiedBy = id;
                note.ModifiedDate = DateTime.Now;
            }


            try
            {
                if (model.DisplayPicture != null && model.DisplayPicture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.DisplayPicture.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/Display_Picture/"), _FileName);
                    model.DisplayPicture.SaveAs(_path);
                    note.DisplayPicture = "UploadedFiles/Display_Picture/" + _FileName;
                }
                else
                {
                    ManageConfigurationModel m = new ManageConfigurationModel();
                    m.DefaultNoteDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultNoteDisplayPicture").FirstOrDefault().Value;
                    note.DisplayPicture = m.DefaultNoteDisplayPicture;
                }

            }
            catch
            {
                
                TempData["Error"] = "Display Picture upload failed!!";

                return View(model);
            }

            try
            {
                if (model.UploadNotes != null && model.UploadNotes.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.UploadNotes.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/Upload_Notes/"), _FileName);
                    model.UploadNotes.SaveAs(_path);
                    note_attach.FilePath = "UploadedFiles/Upload_Notes/" + _FileName;
                    note_attach.FileName = _FileName;
                }
              
            }
            catch
            {
                TempData["Error"] = "Display Picture upload failed!!";

                return View(model);
            }

            try
            {
                if (model.NotesPreview != null && model.NotesPreview.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.NotesPreview.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/Notes_Preview/"), _FileName);
                    model.NotesPreview.SaveAs(_path);
                    note.NotesPreview = "UploadedFiles/Notes_Preview/" + _FileName;
                }
                else
                {
                    note.NotesPreview = null;
                }
            }
            catch
            {
                TempData["Error"] = "Display Picture upload failed!!";

                return View(model);
            }

            note.SellerID = existUser.ID;
            note.Title = model.Title;
            note.Category = model.Category;
            note.NoteType = (model.NoteType != -1 ? model.NoteType : null);
            note.NumberofPages = (model.NumberofPages != null ? model.NumberofPages : null);
            note.Description = model.Description;
            note.Country = (model.Country != -1 ? model.Country : null);
            note.UniversityName = (model.UniversityName != null ? model.UniversityName : null);
            note.Course = (model.Course != null ? model.Course : null);
            note.CourseCode = (model.CourseCode != null ? model.CourseCode : null);
            note.Professor = (model.Professor != null ? model.Professor : null);
            note.IsPaid = (model.IsPaid != "F" ? true : false);
            note.SellingPrice = (model.SellingPrice != null) ? model.SellingPrice : 0;
            note.IsActive = true;
            if (submit.Equals("Save"))
            {
                ReferenceData r = dbObj.ReferenceDatas.Where(m => m.Value == "Draft" && m.RefCategory == "Notes Status" && m.IsActive).FirstOrDefault();
                note.Status = r.ID;
            }
            else
            {
                ReferenceData r = dbObj.ReferenceDatas.Where(m => m.Value == "Submitted For Review" && m.RefCategory == "Notes Status" && m.IsActive).FirstOrDefault();
                note.Status = r.ID;

                var adminemails = dbObj.SystemConfigurations.Where(a => a.Key == "EmailAddressesForNotify").FirstOrDefault().Value;
                string[] admins = adminemails.Split(',');
                var email = ConfigurationManager.AppSettings["username"].ToString();
                var passsword = ConfigurationManager.AppSettings["password"].ToString();
                var fromEmailPassword = passsword;
                MailMessage mailmessage = new MailMessage();
                mailmessage.From = new MailAddress(email, "Note Marketplace");
                mailmessage.Subject = existUser.FirstName + " sent his note for review";
                mailmessage.Body = "Hello Admins,<br/> We want to inform you that," + existUser.FirstName + " sent his note "+
                 model.Title+" for review.Please look at the notes and take required actions. <br/>" +
                   
                    "Regards, <br/>Notes Marketplace";
                mailmessage.IsBodyHtml = true;
                foreach (string e in admins)
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


            }
            if (model.ID == 0)
            {
                note.CreatedDate = DateTime.Now;
                note.CreatedBy = existUser.ID;
                note_attach.CreatedDate = DateTime.Now;
                note_attach.CreatedBy = existUser.ID;

                dbObj.SellerNotes.Add(note);
                dbObj.SaveChanges();
                note_attach.NoteID = note.ID;
                dbObj.SellerNotesAttachments.Add(note_attach);

            }
            

            dbObj.SaveChanges();

            ModelState.Clear();

            TempData["Success"] = "Note added successfully.";

            return RedirectToAction("SellYourNotes", "Notes");
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditNotes(int id)
        {

            AddNoteModel note = getAllList();
            SellerNote n = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();

          

            List<ReferenceData> rl = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Selling Mode" && a.IsActive == true).ToList();
            List<SelectListItem> selectListsmode = new List<SelectListItem>();
            foreach(var t in rl)
            {
                if(t.Value.Equals("Paid"))
                {
                    if (n.IsPaid)
                    {
                        selectListsmode.Add(new SelectListItem { Value = t.DataValue, Text = t.Value ,Selected = true});
                    }
                    else
                    {
                        selectListsmode.Add(new SelectListItem { Value = t.DataValue, Text = t.Value });

                    }
                }
                else
                {
                    if (n.IsPaid)
                    {
                        selectListsmode.Add(new SelectListItem { Value = t.DataValue, Text = t.Value});
                    }
                    else
                    {
                        selectListsmode.Add(new SelectListItem { Value = t.DataValue, Text = t.Value,Selected = true });

                    }
                }
            }

            note.SellingModeList = selectListsmode;
            note.ID = n.ID;
            note.SellerID = n.SellerID;
            note.Title = n.Title;
            note.Category = n.Category;
            byte[] b = Encoding.ASCII.GetBytes(n.DisplayPicture);
            if(n.DisplayPicture!=null)
            {
                note.DisplayPictureName = n.DisplayPicture.Split('/').Last();
            }
            if (n.NotesPreview != null)
            {
                note.DisplayPictureName = n.NotesPreview.Split('/').Last();
            }
            note.UploadNotesName = dbObj.SellerNotesAttachments.Where(a => a.NoteID == n.ID).FirstOrDefault().FileName.ToString();
            note.NoteType = n.NoteType;
            note.Description = n.Description;
            note.NumberofPages = n.NumberofPages;
            note.Course = n.Course;
            note.CourseCode = n.CourseCode;
            note.UniversityName = n.UniversityName;
            note.Professor = n.Professor;
            note.SellingPrice = n.SellingPrice;
            note.IsPaid = n.IsPaid.ToString();
            
            return View("SaveNotes", note);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CloneNotes(int id)
        {
            int ref_id = dbObj.ReferenceDatas.Where(a => a.Value == "Draft").FirstOrDefault().ID;
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();
            note.Status = ref_id;
            dbObj.SaveChanges();
            TempData["Success"] = "Your note clone successfully";
            return Redirect("~/Notes/Editnotes/"+id);
        }



        [HttpGet]
        [Authorize]
        public void DownloadNote(int id, string uploadNotes, string uploadNotesName)
        {
            string email = User.Identity.Name;
            int buyerID = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault().ID;
            Download isExist = dbObj.Downloads.Where(a => a.NoteID == id && a.Downloader == buyerID).FirstOrDefault();
            if (isExist == null)
            {
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();
                Download d = new Download();
                d.NoteID = note.ID;
                d.Seller = note.SellerID;
                d.Downloader = buyerID;
                d.IsSellerHasAllowedDownload = true;
                d.AttachmentPath = uploadNotes;
                d.AttachmentDownloadedDate = DateTime.Now;
                d.IsAttachmentDownloaded = true;
                d.IsPaid = false;
                d.PurchasedPrice = 0;
                d.NoteTitle = note.Title;
                d.NoteCategory = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                d.CreatedBy = buyerID;
                d.CreatedDate = DateTime.Now;
                dbObj.Downloads.Add(d);
                dbObj.SaveChanges();
            }
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + uploadNotesName);
            Response.TransmitFile(Server.MapPath("~/" + uploadNotes));
            Response.End();
        }

        [HttpGet]
        [Authorize]
        public ActionResult DownloadPaidNotes(int id, string uploadNotes)
        {
            string emailID = User.Identity.Name;
            var existUser = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            int buyerID = existUser.ID;

            if (!dbObj.Downloads.Any(a => a.NoteID == id && a.Downloader == buyerID))
            {
                SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();
                Download d = new Download();
                d.NoteID = note.ID;
                d.Seller = note.SellerID;
                d.Downloader = buyerID;
                d.IsSellerHasAllowedDownload = false;
                d.AttachmentPath = uploadNotes;
                d.AttachmentDownloadedDate = DateTime.Now;
                d.IsAttachmentDownloaded = false;
                d.IsPaid = true;
                d.PurchasedPrice = note.SellingPrice;
                d.NoteTitle = note.Title;
                d.NoteCategory = dbObj.NoteCategories.Where(a => a.ID == note.Category).FirstOrDefault().Name;
                d.CreatedBy = buyerID;
                d.CreatedDate = DateTime.Now;
                dbObj.Downloads.Add(d);
                dbObj.SaveChanges();

            }
            var Seller_id = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault().SellerID;
            var Seller = dbObj.Users.Where(a => a.ID == Seller_id).FirstOrDefault();

            string subject = existUser.FirstName + " wants to purchase your notes";

            string body = "Hello " + Seller.FirstName + " ,<br/>We would like to inform you that, " + existUser.FirstName
               + "wants to purchase your notes. Please see Buyer Requests tab and allow download access to Buyer if you have received the payment from him.<br/>Regards,<br/>Notes Marketplace";

            SendEmail(Seller.EmailID, subject, body);
            return RedirectToAction("SearchNotes");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteNotes(int id)
        {
            SellerNote note = dbObj.SellerNotes.Where(a => a.ID == id).FirstOrDefault();

            SellerNotesAttachment n_att = dbObj.SellerNotesAttachments.Where(a => a.NoteID == note.ID).FirstOrDefault();
            dbObj.SellerNotesAttachments.Remove(n_att);
            dbObj.SellerNotes.Remove(note);

            dbObj.SaveChanges();
            TempData["Success"] = "Note deleted successfully.";

            return RedirectToAction("SellYourNotes");
        }

        [NonAction]
        public AddNoteModel getAllList()
        {
            List<SelectListItem> CategoryList = dbObj.NoteCategories.Where(a => a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.Name,
                   Value = x.ID.ToString()
               }).ToList();
            var categorytip = new SelectListItem()
            {
                Value = null,
                Text = "Select your Note Category",
                Selected = true
            };
            CategoryList.Insert(0, categorytip);

            List<SelectListItem> TypeList = dbObj.NoteTypes.Where(a => a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.Name,
                   Value = x.ID.ToString()
               }).ToList();
            var typetip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select your Note Type",
                Selected = true
            };
            TypeList.Insert(0, typetip);
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
              .Select(x =>
              new SelectListItem()
              {
                  Text = x.Name,
                  Value = x.ID.ToString()
              }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select your Country",
                Selected = true
            };
            CountryList.Insert(0, countrytip);

            List<SelectListItem> SellingModeList = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Selling Mode" && a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.Value,
                   Value = x.DataValue
               }).ToList();

            AddNoteModel note = new AddNoteModel()
            {
                CategoryList = CategoryList,
                TypeList = TypeList,
                CountryList = CountryList,
                SellingModeList = SellingModeList,

            };
            return (note);

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