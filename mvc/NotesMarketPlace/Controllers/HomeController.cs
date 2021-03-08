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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactU model)
        {
            if(!ModelState.IsValid)
            {
                return View("ContactUs");
            }
            model.QueryDate = DateTime.Now;
            dbObj.ContactUs.Add(model);
            dbObj.SaveChanges();

            ManageConfigurationModel m = new ManageConfigurationModel();
            m.SupportEmailAddress = dbObj.SystemConfigurations.Where(a => a.Key == "SupportEmailAddress").FirstOrDefault().Value;

            string subject = model.FullName + "- Query";

            string body = "Hello,<br/><br/>" + model.Comments
               + "<br/><br/>Regards,<br/>" + model.FullName;

            SendEmail(m.SupportEmailAddress, subject, body);
            ModelState.Clear();
            ViewBag.Message = "Your Query has been sent successfully.";
            return RedirectToAction("ContactUs");
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Footer()
        {
            ManageConfigurationModel model = new ManageConfigurationModel();
            model.FBICON = dbObj.SystemConfigurations.Where(a => a.Key == "FBICON").FirstOrDefault().Value;
            model.TWITTERICON = dbObj.SystemConfigurations.Where(a => a.Key == "TWITTERICON").FirstOrDefault().Value;
            model.LNICON = dbObj.SystemConfigurations.Where(a => a.Key == "LNICON").FirstOrDefault().Value;
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
                    model.DefaultMemberDisplayPicture = dbObj.SystemConfigurations.Where(a => a.Key == "DefaultMemberDisplayPicture").FirstOrDefault().Value;

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