using RentACar.Areas.ControlPanel.Models;
using RentACar.Models;
using System.Linq;
using System.Web.Mvc;

namespace RentACar.Areas.ControlPanel.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AppSettingsController : Controller
    {
        // GET: ControlPanel/Application
        public ActionResult Index(AppSettingsMessageId? message)
        {
            ViewBag.StatusMessage =
                message == AppSettingsMessageId.ChangedEmailSettings ? "Email settings has been successfully changed."
                : message == AppSettingsMessageId.Error ? "An error has occurred."
                : "";

            if (message == AppSettingsMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            ApplicationEmailViewModel model;

            using (var db = new ApplicationDbContext())
            {
                var appSettings = db.AppSettings.FirstOrDefault();

                if (appSettings == null)
                {
                    return View();
                }

                model = new ApplicationEmailViewModel
                {
                    EmailAddress = appSettings.EmailAddress,
                    Username = appSettings.EmailUsername,
                    Password = appSettings.EmailPassword
                };
            }

            return View(model);
        }

        // POST: ControlPanel/Application/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(ApplicationEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new ApplicationDbContext())
            {
                var appSettings = db.AppSettings.FirstOrDefault();

                if (appSettings == null)
                {
                    appSettings = new AppSettings
                    {
                        EmailAddress = model.EmailAddress,
                        EmailUsername = model.Username,
                        EmailPassword = model.Password
                    };

                    db.AppSettings.Add(appSettings);
                    db.SaveChanges();
                }
                else
                {
                    appSettings.EmailAddress = model.EmailAddress;
                    appSettings.EmailUsername = model.Username;
                    appSettings.EmailPassword = model.Password;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "AppSettings", new { area = "ControlPanel", Message = AppSettingsMessageId.ChangedEmailSettings });
        }

        public enum AppSettingsMessageId
        {
            ChangedEmailSettings,
            Error
        }
    }
}