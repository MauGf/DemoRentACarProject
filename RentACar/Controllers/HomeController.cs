using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using RentACar.Models;
using System.Net;

namespace RentACar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "If you have any question, feel free to ask. <br> We will answer as soon as possible.";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Contact(Contact model)
        {
            model.Message = Request.Form["message"];

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                MailMessage mail = new MailMessage();

                AppSettings appSettings;

                using (var db = new ApplicationDbContext())
                {
                    appSettings = db.AppSettings.FirstOrDefault();
                }

                // Message details
                mail.From = new MailAddress(model.Email);
                mail.Subject = model.Subject;
                mail.IsBodyHtml = false;
                mail.To.Add(appSettings.EmailAddress);

                string sender = "Sender: " + model.Name + " (" + model.Email + ")" + "\n\n";
                string body = model.Message + "\n\n";
                string sendFrom = "Sent from: IN Car Rent";

                mail.Body = sender + body + sendFrom;

                // Email configuration
                // "Allow less secure apps" must be enable (Google Account)
                using (var gmailClient = new Features.GmailService(appSettings.EmailUsername, appSettings.EmailPassword))
                {
                    gmailClient.Send(mail);
                    mail.Dispose();
                }

                ViewBag.Message = "<br> Your message has been sent successfuly.";
                return View();
            }
            catch
            {
                ViewBag.Message = "<br> Your message has not been sent. Please try again. <br>";
                return View();
            }
        }
        
        public ActionResult CarList()
        {
            List<Car> cars;

            using (var db = new ApplicationDbContext())
            {
                cars = db.Cars.Include("Brand")
                    .OrderBy(m => m.Brand.Name).ThenBy(m => m.Model).ToList();
            }

            return View(cars);
        }
    }
}