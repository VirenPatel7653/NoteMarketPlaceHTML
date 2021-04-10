using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlace.Controllers
{
    [Authorize(Roles = "Admin,Super Admin")]
    public class ManageAdminController : Controller
    {
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();

        [HttpGet]
        public ActionResult ManageCategory()
        {
            List<AdminManageCategoryViewModel> listOfCategories = new List<AdminManageCategoryViewModel>();
            List<NoteCategory> listOfnoteCategory = dbObj.NoteCategories.ToList();
            foreach(NoteCategory n in listOfnoteCategory)
            {
                AdminManageCategoryViewModel model = new AdminManageCategoryViewModel();
                model.ID = n.ID;
                model.Category = n.Name;
                model.Description = n.Description;
                model.Active = n.IsActive ? "Yes" : "No";
                if(n.CreatedDate != null)
                {
                    model.DateAdded = DateTime.Parse(n.CreatedDate.ToString());
                }
                var user = dbObj.Users.Where(a => a.ID == n.CreatedBy).FirstOrDefault();
                model.AddedBy = user.FirstName + " " + user.LastName;
                listOfCategories.Add(model);
            }
            return View(listOfCategories.OrderByDescending(a=>a.DateAdded));
        }
        
        [HttpGet]
        public ActionResult SaveCategory()
        {

            return View();
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            NoteCategory model = dbObj.NoteCategories.Where(a => a.ID == id).FirstOrDefault();
            return View("SaveCategory",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategory(NoteCategory model)
        {
            ModelState.Remove("ID");
            if(!ModelState.IsValid)
            {
                TempData["Error"] = "Something went wrong";
                return View("SaveCategory", model);
            }
            if(model.ID ==0)
            {
                model.CreatedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                dbObj.NoteCategories.Add(model);
                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " added Successfully";
            }
            else
            {
                NoteCategory m = dbObj.NoteCategories.Where(a => a.ID == model.ID).FirstOrDefault();

                m.Name = model.Name;
                m.Description = model.Description;
                m.IsActive = true;
                m.ModifiedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                m.ModifiedDate = DateTime.Now;
               
                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " edited Successfully";
            }
            
            return RedirectToAction("ManageCategory");
        }

        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            NoteCategory model = dbObj.NoteCategories.Where(a => a.ID == id).FirstOrDefault();
            model.IsActive = false;
            dbObj.SaveChanges();
            TempData["Success"] = model.Name + " deleted Successfully.";
            return RedirectToAction("ManageCategory");
        }

        [HttpGet]
        public ActionResult ManageType()
        {
            List<AdminManageTypeViewModel> listOfTypes = new List<AdminManageTypeViewModel>();
            List<NoteType> listOfnoteType = dbObj.NoteTypes.ToList();
            foreach (NoteType n in listOfnoteType)
            {
                AdminManageTypeViewModel model = new AdminManageTypeViewModel();
                model.ID = n.ID;
                model.Type = n.Name;
                model.Description = n.Description;
                model.Active = n.IsActive ? "Yes" : "No";
                if (n.CreatedDate != null)
                {
                    model.DateAdded = DateTime.Parse(n.CreatedDate.ToString());
                }
                var user = dbObj.Users.Where(a => a.ID == n.CreatedBy).FirstOrDefault();
                model.AddedBy = user.FirstName + " " + user.LastName;
                listOfTypes.Add(model);
            }
            return View(listOfTypes.OrderByDescending(a => a.DateAdded));
        }

        [HttpGet]
        public ActionResult SaveType()
        {

            return View();
        }

        [HttpGet]
        public ActionResult EditType(int id)
        {
            NoteType model = dbObj.NoteTypes.Where(a => a.ID == id).FirstOrDefault();
            return View("SaveType", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveType(NoteType model)
        {
            ModelState.Remove("ID");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Something went wrong";
                return View("SaveType", model);
            }
            if (model.ID == 0)
            {
                model.CreatedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                dbObj.NoteTypes.Add(model);
                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " added Successfully";
            }
            else
            {
                NoteType m = dbObj.NoteTypes.Where(a => a.ID == model.ID).FirstOrDefault();

                m.Name = model.Name;
                m.Description = model.Description;
                m.IsActive = true;
                m.ModifiedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                m.ModifiedDate = DateTime.Now;

                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " edited Successfully";
            }

            return RedirectToAction("ManageType");
        }

        [HttpGet]
        public ActionResult DeleteType(int id)
        {
            NoteType model = dbObj.NoteTypes.Where(a => a.ID == id).FirstOrDefault();
            model.IsActive = false;
            dbObj.SaveChanges();
            TempData["Success"] = model.Name + " deleted Successfully.";
            return RedirectToAction("ManageType");
        }


        [HttpGet]
        public ActionResult ManageCountry()
        {
            List<AdminManageCountryViewModel> listOfCountries = new List<AdminManageCountryViewModel>();
            List<Country> listOfnoteCountry = dbObj.Countries.ToList();
            foreach (Country n in listOfnoteCountry)
            {
                AdminManageCountryViewModel model = new AdminManageCountryViewModel();
                model.ID = n.ID;
                model.CountryCode = n.CountryCode;
                model.CountryName = n.Name;
                model.Active = n.IsActive ? "Yes" : "No";
                if (n.CreatedDate != null)
                {
                    model.DateAdded = DateTime.Parse(n.CreatedDate.ToString());
                }
                var user = dbObj.Users.Where(a => a.ID == n.CreatedBy).FirstOrDefault();
                model.AddedBy = user.FirstName + " " + user.LastName;
                listOfCountries.Add(model);
            }
            return View(listOfCountries.OrderByDescending(a => a.DateAdded));
        }

        [HttpGet]
        public ActionResult SaveCountry()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditCountry(int id)
        {
            Country model = dbObj.Countries.Where(a => a.ID == id).FirstOrDefault();
            return View("SaveCountry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCountry(Country model)
        {
            ModelState.Remove("ID");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Something went wrong";
                return View("SaveCountry", model);
            }
            if (model.ID == 0)
            {
                model.CreatedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                model.CreatedDate = DateTime.Now;
                model.IsActive = true;
                dbObj.Countries.Add(model);
                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " added Successfully";
            }
            else
            {
                Country m = dbObj.Countries.Where(a => a.ID == model.ID).FirstOrDefault();

                m.Name = model.Name;
                m.CountryCode = "+"+model.CountryCode;
                m.IsActive = true;
                m.ModifiedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                m.ModifiedDate = DateTime.Now;

                dbObj.SaveChanges();
                TempData["Success"] = model.Name + " edited Successfully";
            }

            return RedirectToAction("ManageCountry");
        }

        [HttpGet]
        public ActionResult DeleteCountry(int id)
        {
            Country model = dbObj.Countries.Where(a => a.ID == id).FirstOrDefault();
            model.IsActive = false;
            dbObj.SaveChanges();
            TempData["Success"] = model.Name + " deleted Successfully.";
            return RedirectToAction("ManageCountry");
        }

        [HttpGet]
        public ActionResult ManageAdministrator()
        {
            List<AdministratorModel> listOfAdmins = new List<AdministratorModel>();
            int admin_id = dbObj.UserRoles.Where(a => a.Name == "Admin").FirstOrDefault().ID;
            List<User> listOfUsers = dbObj.Users.Where(a=>a.RoleID==admin_id).ToList();
            foreach (User n in listOfUsers)
            {
                AdministratorModel model = new AdministratorModel();

                model.ID = n.ID;
                var userProfile = dbObj.UserProfiles.Where(a => a.UserID == n.ID).FirstOrDefault();
                model.FirstName = n.FirstName;
                model.LastName = n.LastName;
                model.EmailID = n.EmailID;
                model.PhoneNumber = userProfile.Phonenumber;
                model.Active = n.IsActive ? "Yes" : "No";
                if (n.CreatedDate != null)
                {
                    model.DateAdded = DateTime.Parse(n.CreatedDate.ToString());
                }

                listOfAdmins.Add(model);
            }
            return View(listOfAdmins.OrderByDescending(a => a.DateAdded));
        }
        [HttpGet]
        public ActionResult SaveAdministrator()
        {
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
              .Select(x =>
              new SelectListItem()
              {
                  Text = x.CountryCode,
                  Value = x.ID.ToString()
              }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select Country Code",
                Selected = true
            };
            CountryList.Insert(0, countrytip);
            AdministratorViewModel model = new AdministratorViewModel();
            model.CountryList = CountryList;
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveAdministrator(AdministratorViewModel model)
        {
            ModelState.Remove("Admin.ID");
            if (!ModelState.IsValid)
            {
                List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
                .Select(x =>
                 new SelectListItem()
                 {
                     Text = x.CountryCode,
                     Value = x.ID.ToString()
                 }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = "-1",
                    Text = "Select Country Code",
                    Selected = true
                };
                CountryList.Insert(0, countrytip);
                model.CountryList = CountryList;
                TempData["Error"] = "Something went wrong.";
                return View("SaveAdministrator", model);
            }
            if(model.Admin.ID == 0)
            {
                User user = new User();
                user.FirstName = model.Admin.FirstName;
                user.LastName = model.Admin.LastName;
                user.EmailID = model.Admin.EmailID;
                user.Password = Crypto.EncryptBase64(model.Admin.FirstName + "@1234");
                int admin_id = dbObj.UserRoles.Where(a => a.Name == "Admin").FirstOrDefault().ID;
                user.RoleID = admin_id;
                user.IsActive = true;
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                dbObj.Users.Add(user);
                UserProfile up = new UserProfile();
                up.UserID = user.ID;
                up.Phonenumber = model.Admin.PhoneNumber;
                up.Phonenumber_CountryCode = model.Admin.CountryPhoneCode;
                up.CreatedDate = DateTime.Now;

                up.CreatedBy = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                dbObj.UserProfiles.Add(up);
                dbObj.SaveChanges();
                TempData["Success"] = model.Admin.FirstName + " added Successfully";

            }
            else
            {
                int actionby_id = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
                User user = dbObj.Users.Where(a => a.ID == model.Admin.ID).FirstOrDefault();
                user.FirstName = model.Admin.FirstName;
                user.LastName = model.Admin.LastName;
                user.EmailID = model.Admin.EmailID;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = actionby_id;
                user.IsActive = true;
                UserProfile up = dbObj.UserProfiles.Where(a => a.UserID == model.Admin.ID).FirstOrDefault();
                up.UserID = user.ID;
                up.Phonenumber = model.Admin.PhoneNumber;
                up.Phonenumber_CountryCode = model.Admin.CountryPhoneCode;
                up.ModifiedDate = DateTime.Now;
                up.ModifiedBy = actionby_id;
                dbObj.SaveChanges();
                TempData["Success"] = model.Admin.FirstName + " edited Successfully";

            }

            return RedirectToAction("ManageAdministrator");
        }

        [HttpGet]
        public ActionResult EditAdministrator(int id)
        {
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
              .Select(x =>
              new SelectListItem()
              {
                  Text = x.CountryCode,
                  Value = x.ID.ToString()
              }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select Country Code",
                Selected = true
            };
            CountryList.Insert(0, countrytip);
            AdministratorViewModel model = new AdministratorViewModel();
            model.Admin = new AdministratorModel();
            User n = dbObj.Users.Where(a => a.ID == id).FirstOrDefault();
            UserProfile userProfile = dbObj.UserProfiles.Where(a => a.UserID == id).FirstOrDefault();
            model.Admin.ID = n.ID;
            model.Admin.FirstName = n.FirstName;
            model.Admin.LastName = n.LastName;
            model.Admin.EmailID = n.EmailID;
            model.Admin.CountryPhoneCode = userProfile.Phonenumber_CountryCode;
            model.Admin.PhoneNumber = userProfile.Phonenumber;
            model.CountryList = CountryList;
            return View("SaveAdministrator", model);
        }

        [HttpGet]
        public ActionResult DeleteAdministrator(int id)
        {
            User n = dbObj.Users.Where(a => a.ID == id).FirstOrDefault();
            n.IsActive = false;
            dbObj.SaveChanges();
            TempData["Success"] = n.FirstName + " deleted Successfully.";
            return RedirectToAction("ManageAdministrator");
        }

        [HttpGet]
        public ActionResult ManageSystemConfiguration()
        {
            ManageSystemConfigurationModel model = new ManageSystemConfigurationModel();
            
            SystemConfiguration DefaultMemberDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultMemberDisplayPicture" && a.IsActive == true).FirstOrDefault();
            if(DefaultMemberDisplayPicture !=null)
            {
                model.DefaultMemberDisplayPicture = DefaultMemberDisplayPicture.Value;
            }
            SystemConfiguration DefaultNoteDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultNoteDisplayPicture" && a.IsActive == true).FirstOrDefault();
            if (DefaultNoteDisplayPicture != null)
            {
                model.DefaultNoteDisplayPicture = DefaultNoteDisplayPicture.Value;
            }
            SystemConfiguration SupportEmailAddress = dbObj.SystemConfigurations.Where(a => a.Key == "SupportEmailAddress" && a.IsActive == true).FirstOrDefault();
            if (SupportEmailAddress != null)
            {
                model.SupportEmailAddress = SupportEmailAddress.Value;
            }
            SystemConfiguration SupportContactNumber = dbObj.SystemConfigurations.Where(a => a.Key == "SupportContactNumber" && a.IsActive == true).FirstOrDefault();
            if (SupportContactNumber != null)
            {
                model.SupportContactNumber = SupportContactNumber.Value;
            }
            SystemConfiguration EmailAddressesForNotify = dbObj.SystemConfigurations.Where(a => a.Key == "EmailAddressesForNotify" && a.IsActive == true).FirstOrDefault();
            if (EmailAddressesForNotify != null)
            {
                model.EmailAddressesForNotify = EmailAddressesForNotify.Value;
            }
            SystemConfiguration FBICON = dbObj.SystemConfigurations.Where(a => a.Key == "FBICON" && a.IsActive == true).FirstOrDefault();
            if (FBICON != null)
            {
                model.FBICON = FBICON.Value;
            }
            SystemConfiguration LNICON = dbObj.SystemConfigurations.Where(a => a.Key == "LNICON" && a.IsActive == true).FirstOrDefault();
            if (LNICON != null)
            {
                model.LNICON = LNICON.Value;
            }
            SystemConfiguration TWITTERICON = dbObj.SystemConfigurations.Where(a => a.Key == "TWITTERICON" && a.IsActive == true).FirstOrDefault();
            if (TWITTERICON != null)
            {
                model.TWITTERICON = TWITTERICON.Value;
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveManageSystemConfiguration(ManageSystemConfigurationModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("ManageSystemConfiguration");
            }
            int reported_id = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            SystemConfiguration SupportEmailAddress = dbObj.SystemConfigurations.Where(a => a.Key == "SupportEmailAddress" && a.IsActive == true).FirstOrDefault();
            if(SupportEmailAddress==null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "SupportEmailAddress";
                entry.Value = model.SupportEmailAddress;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                SupportEmailAddress.Value = model.SupportEmailAddress;
                SupportEmailAddress.ModifiedDate = DateTime.Now;
                SupportEmailAddress.ModifiedBy = reported_id;

            }
            SystemConfiguration SupportContactNumber = dbObj.SystemConfigurations.Where(a => a.Key == "SupportContactNumber" && a.IsActive == true).FirstOrDefault();
            if (SupportContactNumber == null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "SupportContactNumber";
                entry.Value = model.SupportContactNumber;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                SupportContactNumber.Value = model.SupportContactNumber;
                SupportContactNumber.ModifiedDate = DateTime.Now;
                SupportContactNumber.ModifiedBy = reported_id;

            }
            
            SystemConfiguration EmailAddressesForNotify = dbObj.SystemConfigurations.Where(a => a.Key == "EmailAddressesForNotify" && a.IsActive == true).FirstOrDefault();
            if (EmailAddressesForNotify == null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "EmailAddressesForNotify";
                entry.Value = model.EmailAddressesForNotify;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                EmailAddressesForNotify.Value = model.EmailAddressesForNotify;
                EmailAddressesForNotify.ModifiedDate = DateTime.Now;
                EmailAddressesForNotify.ModifiedBy = reported_id;

            }
            SystemConfiguration FBICON = dbObj.SystemConfigurations.Where(a => a.Key == "FBICON" && a.IsActive == true).FirstOrDefault();
            if (FBICON == null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "FBICON";
                entry.Value = model.FBICON;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                FBICON.Value = model.FBICON;
                FBICON.ModifiedDate = DateTime.Now;
                FBICON.ModifiedBy = reported_id;

            }
            SystemConfiguration LNICON = dbObj.SystemConfigurations.Where(a => a.Key == "LNICON" && a.IsActive == true).FirstOrDefault();
            if (LNICON == null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "LNICON";
                entry.Value = model.LNICON;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                LNICON.Value = model.LNICON;
                LNICON.ModifiedDate = DateTime.Now;
                LNICON.ModifiedBy = reported_id;

            }
            SystemConfiguration TWITTERICON = dbObj.SystemConfigurations.Where(a => a.Key == "TWITTERICON" && a.IsActive == true).FirstOrDefault();
            if (TWITTERICON == null)
            {
                SystemConfiguration entry = new SystemConfiguration();
                entry.Key = "TWITTERICON";
                entry.Value = model.TWITTERICON;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = reported_id;
                entry.IsActive = true;
                dbObj.SystemConfigurations.Add(entry);
            }
            else
            {
                TWITTERICON.Value = model.TWITTERICON;
                TWITTERICON.ModifiedDate = DateTime.Now;
                TWITTERICON.ModifiedBy = reported_id;

            }
            dbObj.SaveChanges();
            try
            {
                if (model.DisplayMemberPicture != null && model.DisplayMemberPicture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.DisplayMemberPicture.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/Default/"), _FileName);
                    model.DisplayMemberPicture.SaveAs(_path);
                    
                    SystemConfiguration DefaultMemberDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultMemberDisplayPicture" && a.IsActive == true).FirstOrDefault();
                    if (DefaultMemberDisplayPicture == null)
                    {
                        SystemConfiguration entry = new SystemConfiguration();
                        entry.Key = "DefaultMemberDisplayPicture";
                        entry.Value = "UploadedFiles/Default/" + _FileName;
                        entry.CreatedDate = DateTime.Now;
                        entry.CreatedBy = reported_id;
                        entry.IsActive = true;
                        dbObj.SystemConfigurations.Add(entry);
                    }
                    else
                    {
                        DefaultMemberDisplayPicture.Value = "UploadedFiles/Default/" + _FileName;
                        DefaultMemberDisplayPicture.ModifiedDate = DateTime.Now;
                        DefaultMemberDisplayPicture.ModifiedBy = reported_id;

                    }
                    dbObj.SaveChanges();
                }
            }
            catch
            {
                TempData["Error"] = "Display Member Picture upload failed!!";
                return View(model);
            }

            try
            {
                if(model.DisplayNotePicture != null && model.DisplayNotePicture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.DisplayNotePicture.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/Default/"), _FileName);
                    model.DisplayNotePicture.SaveAs(_path);
                    SystemConfiguration DefaultNoteDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultNoteDisplayPicture" && a.IsActive == true).FirstOrDefault();
                    if (DefaultNoteDisplayPicture == null)
                    {
                        SystemConfiguration entry = new SystemConfiguration();
                        entry.Key = "DefaultNoteDisplayPicture";
                        entry.Value = "UploadedFiles/Default/" + _FileName;
                        entry.CreatedDate = DateTime.Now;
                        entry.CreatedBy = reported_id;
                        entry.IsActive = true;
                        dbObj.SystemConfigurations.Add(entry);
                    }
                    else
                    {
                        DefaultNoteDisplayPicture.Value = "UploadedFiles/Default/" + _FileName;
                        DefaultNoteDisplayPicture.ModifiedDate = DateTime.Now;
                        DefaultNoteDisplayPicture.ModifiedBy = reported_id;

                    }
                    dbObj.SaveChanges();
                }
            }
            catch
            {
                TempData["Error"] = "Display Note Picture upload failed!!";
                return View(model);
            }
           
            TempData["Success"] = "Manage System Configuration updated successfully.";

            return RedirectToAction("Dashboard","Admin");
        }

        [HttpGet]
        public ActionResult MyProfile()
        {
            
            AdminProfileModel model = new AdminProfileModel();
            User user = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault();
            model.ID = user.ID;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.EmailID;
            UserProfile up = dbObj.UserProfiles.Where(a => a.UserID == user.ID).FirstOrDefault();
            if(up!=null)
            {
                model.Phonenumber = up.Phonenumber;
                model.Phonenumber_CountryCode = up.Phonenumber_CountryCode;
                model.SecondaryEmailAddress = up.SecondaryEmailAddress;
                model.ProfilePictureName = up.ProfilePicture;
                
            }
            model.Email = user.EmailID;
            model.CountryList = getCountryList();
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMyProfile(AdminProfileModel model)
        {
            if (!ModelState.IsValid)
            {

                model.CountryList = getCountryList();
                TempData["Error"] = "Something went wrong.";
                return View("SaveMyProfile", model);
            }
            int actionby_id = dbObj.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().ID;
            User user = dbObj.Users.Where(a => a.ID == model.ID).FirstOrDefault();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailID = model.Email;
            user.ModifiedDate = DateTime.Now;
            user.ModifiedBy = actionby_id;
            user.IsActive = true;
            UserProfile up = dbObj.UserProfiles.Where(a => a.UserID == model.ID).FirstOrDefault();
           if(up==null)
            {
                UserProfile entry = new UserProfile();
                entry.UserID = model.ID;
                entry.UserID = user.ID;
                entry.Phonenumber = model.Phonenumber;
                entry.Phonenumber_CountryCode = model.Phonenumber_CountryCode;
                entry.SecondaryEmailAddress = model.SecondaryEmailAddress;
                entry.CreatedDate = DateTime.Now;
                entry.CreatedBy = actionby_id;

                try
                {
                    if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.ProfilePicture.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/User_Profile/"), _FileName);
                        model.ProfilePicture.SaveAs(_path);
                        entry.ProfilePicture = "UploadedFiles/User_Profile/" + _FileName;
                    }
                }
                catch
                {
                    TempData["Error"] = "Profile  Picture upload failed!!";
                    model.CountryList = getCountryList();
                    return View("SaveMyProfile", model);
                }


                dbObj.UserProfiles.Add(entry);
                dbObj.SaveChanges();
            }
           else
            {
                try
                {
                    if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.ProfilePicture.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/User_Profile/"), _FileName);
                        model.ProfilePicture.SaveAs(_path);
                        up.ProfilePicture = "UploadedFiles/User_Profile/" + _FileName;
                    }
                }
                catch
                {
                    TempData["Error"] = "Profile  Picture upload failed!!";
                    model.CountryList = getCountryList();
                    return View("SaveMyProfile", model);
                }

                up.UserID = user.ID;
                up.Phonenumber = model.Phonenumber;
                up.Phonenumber_CountryCode = model.Phonenumber_CountryCode;
                up.SecondaryEmailAddress = model.SecondaryEmailAddress;
                up.ModifiedDate = DateTime.Now;
                up.ModifiedBy = actionby_id;
                dbObj.SaveChanges();

            }

            TempData["Success"] = model.FirstName + " updated Successfully";

            return RedirectToAction("Dashboard","Admin");
        }

        [NonAction]
        public List<SelectListItem> getCountryList()
        {
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
                .Select(x =>
                 new SelectListItem()
                 {
                     Text = x.CountryCode,
                     Value = x.ID.ToString()
                 }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select Country Code",
                Selected = true
            };
            CountryList.Insert(0, countrytip);
            return CountryList;
        }
    }
}