using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RentACar.Areas.Account.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RentACar.Areas.Account.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public EmailController() { }

        public EmailController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Account/Email
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.SentConfirmationEmail ? "Confirmation email has been sent to your email address."
                : message == ManageMessageId.ConfirmedEmailSuccess ? "Your email has been confirmed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            if (message == ManageMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            EmailViewModel model = new EmailViewModel
            {
                EmailAddress = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };

            return View(model);
        }

        // GET: /Account/Email/ConfirmEmail
        public ActionResult ConfirmEmail(string confirmedEmail)
        {
            if (confirmedEmail == "success" )
            {
                ViewBag.Result = "Your email has been successfully confirmed.";
            }
            else
            {
                ViewBag.Result = "An error has occured. Your email has not been confirmed successfully.";
            }
            return View();
        }

        // GET: /Account/Email/SendConfirmationEmail
        public async Task<ActionResult> SendConfirmationEmail(ManageMessageId? message)
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Auth",
                new { area = "Account", userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            if (message != null)
            {
                return RedirectToAction("Index", "Email", new { area = "Account", Message = message });
            }
            return RedirectToAction("Index", "Email", new { area = "Account", Message = ManageMessageId.SentConfirmationEmail });
        }

        // GET: /Account/Email/ChangeEmail
        public ActionResult ChangeEmail()
        {
            return View();
        }

        // POST: /Account/Email/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            if (UserManager.CheckPassword(user, model.CurrentPassword))
            {
                user.Email = model.NewEmail;

                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (UserManager.IsEmailConfirmed(user.Id))
                    {
                        user.EmailConfirmed = false;
                        await UserManager.UpdateAsync(user);
                    }
                    return RedirectToAction("SendConfirmationEmail", "Email", new { area = "Account", Message = ManageMessageId.ChangeEmailSuccess });
                }
                AddErrors(result);
                return View();
            }
            else
            {
                IdentityResult uncorrectPassword = IdentityResult.Failed("You entered a wrong password.");
                AddErrors(uncorrectPassword);
                model.CurrentPassword = "";
                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        // Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            SentConfirmationEmail,
            ConfirmedEmailSuccess,
            ChangeEmailSuccess,
            Error
        }
    }
}