using NotesMarketPlace.Context;
using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NotesMarketPlace.Controllers
{
    public class AccountController : Controller
    {
        
        NotesMarketPlaceEntities dbObj = new NotesMarketPlaceEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            string message = "";
          
                var v = dbObj.Users.Where(a => a.EmailID == model.EmailID).FirstOrDefault();
                var userroleId = dbObj.UserRoles.Where(a => a.Name == "Member").FirstOrDefault().ID;
            if (v.RoleID == userroleId)
            {
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View("Login");
                    }
                    if (string.Compare(Crypto.EncryptBase64(model.Password), v.Password) == 0)
                    {
                        int timeout = model.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(model.EmailID, model.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["username"] = v.EmailID;
                        Session["userId"] = v.ID;
                        var existuser = dbObj.UserProfiles.Where(a => a.UserID == v.ID).FirstOrDefault();
                        if (existuser == null)
                        {
                            TempData["Suceess"] = "You are Loggedin Successfully.";
                            return RedirectToAction("UserProfile", "User");
                        }
                        TempData["Suceess"] = "You are Loggedin Successfully.";
                        return RedirectToAction("SearchNotes", "Notes");
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }

                ViewBag.Message = message;
                return View("Login");
            }
            else
            {
                if (v != null)
                {
                    if (string.Compare(Crypto.EncryptBase64(model.Password), v.Password) == 0)
                    {
                        int timeout = model.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(model.EmailID, model.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["username"] = v.EmailID;
                        Session["userId"] = v.ID;
                        TempData["Suceess"] = "You are Loggedin Successfully.";
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }

                ViewBag.Message = message;
                return View("Login");

            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Something went Wrong";
                return View("SignUp");
            }
            var isExist = IsEmailExist(model.EmailID);
            if(isExist)
            {
                ModelState.AddModelError("EmailExist", "Email already exist");
                return View(model);
            }

            var role = dbObj.UserRoles.SingleOrDefault(m => m.Name.Equals("Member"));
            var user = new User();
            user.ActivationCode = Guid.NewGuid();
            user.RoleID = role.ID;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailID = model.EmailID;
            user.Password = Crypto.EncryptBase64(model.Password);
            user.IsEmailVerified = false;
            user.CreatedDate = DateTime.Now;
            dbObj.Users.Add(user);
            dbObj.SaveChanges();
            user.CreatedBy = user.ID;
            dbObj.SaveChanges();


            ViewData["success"] = "Your account has been successfully created.";
            try
            {
                SendVerificationLinkEmail(user);
            }
            catch (Exception e)
            {

            }
            
            ModelState.Clear();
            return View("SignUp");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["error"] = "Something went Wrong";
                return View("ForgotPassword");
            }
            var existUser = dbObj.Users.Where(a => a.EmailID == model.EmailID).FirstOrDefault();
            if(existUser == null)
            {
                ModelState.AddModelError("EmailNotExist", "Email does not exist");
                return View("ForgotPassword");
            }
            var randomPassword = GeneratePassword(8);
            existUser.Password = Crypto.EncryptBase64(randomPassword);
            dbObj.SaveChanges();

            string subject = "Note Marketplace - New Temporary Password has been created for you";

            string body = "Hello,<br/>We have generated a new password for you "
                + " <br/>Password:"+ Crypto.DecryptBase64(existUser.Password) + "<br/>Regards,<br/>Notes Marketplace";

            try
            {
                SendEmail(existUser.EmailID, subject, body);
            }
            catch (Exception e)
            {

            }
            ViewData["success"] = "Your password has been changed successfully and newly generated password is sent on your registered email address.";
            ModelState.Clear();

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            string email = User.Identity.Name;
            var existUser = dbObj.Users.Where(a => a.EmailID == email).FirstOrDefault();
            if(string.Compare(Crypto.EncryptBase64(model.OldPassword), existUser.Password) == 0)
            {
                existUser.Password = Crypto.EncryptBase64(model.NewPassword);
                dbObj.SaveChanges();
                TempData["Success"] = "Your Password has been changed successfully.";
                return RedirectToAction("Login", "Account");
                 
            }
            ViewBag.Message = "Invalid credential provided";
            return View();
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            
            bool Status = false;

            var v = dbObj.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
            if (v != null)
            {
                v.IsEmailVerified = true;
                v.IsActive = true;
                dbObj.SaveChanges();
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request";
            }

            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            var existUser = dbObj.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            return existUser != null;
        }


        [NonAction]
        public void SendVerificationLinkEmail(User user)
        {
            var verifyUrl = "/Account/VerifyAccount/" + user.ActivationCode.ToString();
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var toEmail = new MailAddress(user.EmailID);
            string subject = "Note Marketplace - Email Verification";

            string body1 = "Hello "+user.FirstName+" "+user.LastName+" ,"+
                "<br/>Thank you for signing up with us. Please click on below link to verify your email address and to do login."
                +" <br/><br/><a href='" + link + "'>Verify Email Address link </a><br/>Regards,<br/>Notes Marketplace";
            var email = ConfigurationManager.AppSettings["username"].ToString();
            var passsword = ConfigurationManager.AppSettings["password"].ToString();
            var fromEmail = new MailAddress(email, "Note Marketplace");
            var fromEmailPassword = passsword;
            var mail = new MailMessage(fromEmail, toEmail);
            mail.Subject = subject;
            string filePath = Server.MapPath(Url.Content("~/EmailTemplates/EmailVerification.html"));
            StreamReader str = new StreamReader(filePath);
            string body = str.ReadToEnd();
            str.Close();
            body = body.Replace("[Username]", user.FirstName);
            body = body.Replace("[link]", link);
            AlternateView altView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            string imageSource = Server.MapPath(Url.Content("~/Content/images/User-Profile/logo.png"));
            LinkedResource PictureRes = new LinkedResource(imageSource, MediaTypeNames.Image.Jpeg);
            PictureRes.ContentId = "YourPictureId";
            altView.LinkedResources.Add(PictureRes);
            mail.AlternateViews.Add(altView);
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            smtp.Send(mail);
        }

        [NonAction]
        public void SendEmail(String tomail,String subject,String body)
        {
            var email = ConfigurationManager.AppSettings["username"].ToString();
            var passsword = ConfigurationManager.AppSettings["password"].ToString();
            var fromEmail = new MailAddress(email,"Note Marketplace");
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

        [NonAction]
        public static string GeneratePassword(int lengthOfPassword)
        {
            const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
            const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC_CHARACTERS = "0123456789";
            const string SPECIAL_CHARACTERS = @"!#$%&*@\";
            const string SPACE_CHARACTER = " ";

            string characterSet = "";

            characterSet += LOWERCASE_CHARACTERS;

            characterSet += UPPERCASE_CHARACTERS;

            characterSet += NUMERIC_CHARACTERS;
            characterSet += SPECIAL_CHARACTERS;

            characterSet += SPACE_CHARACTER;

            char[] password = new char[lengthOfPassword];
            int characterSetLength = characterSet.Length;

            System.Random random = new System.Random();
            for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
            {
                password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

                bool moreThanTwoIdenticalInARow =
                    characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
                    && password[characterPosition] == password[characterPosition - 1]
                    && password[characterPosition - 1] == password[characterPosition - 2];

                if (moreThanTwoIdenticalInARow)
                {
                    characterPosition--;
                }
            }

            return string.Join(null, password);
        }

    }
}