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
    public class HomeController : Controller
    {
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();
        public ActionResult Index()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            var email = User.Identity.Name;
            ContactU model = new ContactU();
            if(email!="")
            {
                User u = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault();
                model.FullName = u.FirstName + " " + u.LastName;
                model.Email = u.EmailID;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactU model)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Error = "Please enter valid information.";
                return View("ContactUs");
            }
            model.QueryDate = DateTime.Now;
            ManageConfigurationModel m = new ManageConfigurationModel();
            m.SupportEmailAddress = dbObj.SystemConfigurations.Where(a => a.Key == "SupportEmailAddress").FirstOrDefault().Value;

            string subject = model.FullName + "- Query";

            string body = "Hello,<br/><br/>" + model.Comments
               + "<br/><br/>Regards,<br/>" + model.FullName;
            try 
            {
                SendEmail(m.SupportEmailAddress, subject, body);
                
            }
            catch(Exception e)
            {
                ViewBag.Error = "Something went wrong.";
            }
            dbObj.ContactUs.Add(model);
            dbObj.SaveChanges();
            ModelState.Clear();
            ViewBag.Success = "Your Query has been sent successfully.";
            var email = User.Identity.Name;
            ContactU model1 = new ContactU();
            if (email != "")
            {
                User u = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault();
                model1.FullName = u.FirstName + " " + u.LastName;
                model1.Email = u.EmailID;
            }
            return View("ContactUs",model1);
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Footer()
        {
            ManageConfigurationModel model = new ManageConfigurationModel();
             
            SystemConfiguration FBICON = dbObj.SystemConfigurations.Where(a => a.Key == "FBICON").FirstOrDefault();
            if(FBICON!=null)
            {
                model.FBICON = FBICON.Value;
            }
            SystemConfiguration TWITTERICON = dbObj.SystemConfigurations.Where(a => a.Key == "TWITTERICON").FirstOrDefault();
            if (TWITTERICON != null)
            {
                model.TWITTERICON = TWITTERICON.Value;
            }
            SystemConfiguration LNICON = dbObj.SystemConfigurations.Where(a => a.Key == "LNICON").FirstOrDefault();
            if (LNICON != null)
            {
                model.LNICON = LNICON.Value;
            }
            if (Request.IsAuthenticated)
            {
                var email = User.Identity.Name;
                var exist_user_id = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault().ID;
                var user = dbObj.UserProfiles.Where(a => a.UserID == exist_user_id).FirstOrDefault();
                if(user!=null && user.ProfilePicture !=null)
                {
                    model.DefaultMemberDisplayPicture = user.ProfilePicture;
                }
                else
                {
                
                    SystemConfiguration DefaultMemberDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultMemberDisplayPicture").FirstOrDefault();
                    if (DefaultMemberDisplayPicture != null)
                    {
                        model.DefaultMemberDisplayPicture = DefaultMemberDisplayPicture.Value;
                    }
                }
            }
            return Json(model);
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