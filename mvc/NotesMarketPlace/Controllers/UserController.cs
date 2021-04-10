using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NotesMarketPlace.Controllers
{
    [Authorize(Roles = "Member")]
    public class UserController : Controller
    {
        
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();
        
        [HttpGet]
        public ActionResult UserProfile()
        {
         
            string emailID = User.Identity.Name;
            var existUser = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            List<SelectListItem> GenderList = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Gender" && a.IsActive == true)
                .Select(x => 
                new SelectListItem() 
                { 
                    Text = x.Value,
                    Value = x.ID.ToString() 
                }).ToList();
            var gendertip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select your gender",
                Selected = true
            };
            GenderList.Insert(0, gendertip);
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.CountryCode,
                   Value = x.CountryCode
               }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "Select your country",
                Selected = true
            };
            CountryList.Insert(0, countrytip);


            UserProfileModel user = new UserProfileModel()
            {
                UserID = existUser.ID,
                FirstName = existUser.FirstName,
                LastName = existUser.LastName,
                EmailID = existUser.EmailID,
                GenderList = GenderList,
                CountryList = CountryList
            };

            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UserProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                List<SelectListItem> GenderList = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Gender" && a.IsActive == true)
                 .Select(x =>
                 new SelectListItem()
                 {
                     Text = x.Value,
                     Value = x.ID.ToString()
                 }).ToList();
                var gendertip = new SelectListItem()
                {
                    Value = "-1",
                    Text = "Select your gender",
                    Selected = true
                };
                GenderList.Insert(0, gendertip);
                List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
                   .Select(x =>
                   new SelectListItem()
                   {
                       Text = x.CountryCode,
                       Value = x.CountryCode
                   }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Select your country",
                    Selected = true
                };
                CountryList.Insert(0, countrytip);
                model.GenderList = GenderList;
                model.CountryList = CountryList;

                return View(model);
            }

            ManageConfigurationModel m = new ManageConfigurationModel();
            m.DefaultMemberDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultMemberDisplayPicture").FirstOrDefault().Value;
            
            
            var existUser = dbObj.Users.Where(a => a.ID == model.UserID).FirstOrDefault();
            existUser.FirstName = model.FirstName;
            existUser.LastName = model.LastName;

            
            UserProfile user = new UserProfile();
            if(model.ID !=0)
            {
                user = dbObj.UserProfiles.Where(a => a.ID == model.ID).FirstOrDefault();
                user.ModifiedBy = existUser.ID;
                user.ModifiedDate = DateTime.Now;
            }
            user.UserID = model.UserID;
            user.DOB = (model.DOB != null) ? model.DOB : null;
            int unknown_id = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Gender" && a.Value == "Unknown").FirstOrDefault().ID;
            user.Gender = (model.Gender != -1) ? (model.Gender) : unknown_id;
            user.Phonenumber_CountryCode = (model.Phonenumber_CountryCode != null)? model.Phonenumber_CountryCode:null;
            user.Phonenumber = (model.Phonenumber != null)? model.Phonenumber:null;
            try
            {
                if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(model.ProfilePicture.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/User_Profile/"), _FileName);
                    model.ProfilePicture.SaveAs(_path);
                    user.ProfilePicture = "UploadedFiles/User_Profile/" + _FileName;
                }
                else if(model.ProfilePictureName ==null)
                {
                    user.ProfilePicture = m.DefaultMemberDisplayPicture;
                }
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                TempData["Error"] = "File upload failed!!";

                return View("UserProfile");
            }
            user.AddressLine1 = model.AddressLine1;
            user.AddressLine2 = (model.AddressLine2 != null) ? model.AddressLine2 : null;
            user.City = model.City;
            user.State = model.State;
            user.ZipCode = model.ZipCode;
            user.Country = model.Country;
            user.University = (model.University != null) ? model.University : null;
            user.College = (model.College != null) ? model.College : null;
            user.CreatedDate = DateTime.Now;
            user.CreatedBy = model.UserID;
            if(model.ID ==0)
            {
                dbObj.UserProfiles.Add(user);
                TempData["Suceess"] = "Your profile details added successfully.";

            }
            else
            {
                TempData["Suceess"] = "Your profile details updated successfully.";

            }
            dbObj.SaveChanges();

            ModelState.Clear();

            return RedirectToAction("SearchNotes","Notes");
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            string email = User.Identity.Name;
            var u = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault();
            var existUser = dbObj.UserProfiles.Where(a => a.UserID == u.ID).FirstOrDefault();
            UserProfileModel user = new UserProfileModel();
            user.ID = existUser.ID;
            user.UserID = u.ID;
            user.FirstName = u.FirstName;
            user.LastName = u.LastName;
            user.EmailID = u.EmailID;
            user.Gender = existUser.Gender != -1 ? Convert.ToInt32(existUser.Gender) : -1;
            user.DOB = (existUser.DOB != null) ? existUser.DOB : null;
            user.Phonenumber_CountryCode = (existUser.Phonenumber_CountryCode != null) ? existUser.Phonenumber_CountryCode : null;
            user.Phonenumber = (existUser.Phonenumber != null) ? existUser.Phonenumber : null;
            user.ProfilePictureName = existUser.ProfilePicture.Split('/').Last();
            user.AddressLine1 = existUser.AddressLine1;
            user.AddressLine2 = (existUser.AddressLine2 != null) ? existUser.AddressLine2 : null;
            user.City = existUser.City;
            user.State = existUser.State;
            user.ZipCode = existUser.ZipCode;
            user.Country = existUser.Country;
            user.University = (existUser.University != null) ? existUser.University : null;
            user.College = (existUser.College != null) ? existUser.College : null;
            List<SelectListItem> GenderList = dbObj.ReferenceDatas.Where(a => a.RefCategory == "Gender" && a.IsActive == true)
                 .Select(x =>
                 new SelectListItem()
                 {
                     Text = x.Value,
                     Value = x.ID.ToString()
                 }).ToList();
            var gendertip = new SelectListItem()
            {
                Value = "-1",
                Text = "Select your gender",
                Selected = true
            };
            GenderList.Insert(0, gendertip);
            List<SelectListItem> CountryList = dbObj.Countries.Where(a => a.IsActive == true)
               .Select(x =>
               new SelectListItem()
               {
                   Text = x.CountryCode,
                   Value = x.CountryCode
               }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "Select your country",
                Selected = true
            };
            CountryList.Insert(0, countrytip);

            user.GenderList = GenderList;
            user.CountryList = CountryList;

            return View("UserProfile", user);
        }


    }
}