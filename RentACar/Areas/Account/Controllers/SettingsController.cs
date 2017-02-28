using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RentACar.Areas.Account.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RentACar.Areas.Account.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public SettingsController() { }

        public SettingsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Account/Settings
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.SubTitle = "Settings";

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.ChangeUsernameSuccess ? "Your username has been changed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            if(message == ManageMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }
            
            return View();
        }

        // GET: /Account/Settings/ChangeUsername
        public ActionResult ChangeUsername()
        {
            return View();
        }

        // POST: /Account/Settings/ChangeUsername
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUsername(ChangeUsernameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            if (UserManager.CheckPassword(user, model.CurrentPassword))
            {
                user.UserName = model.NewUsername;
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Settings", new { area = "Account", Message = ManageMessageId.ChangeUsernameSuccess });
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

        // GET: /Account/Settings/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/Settings/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Settings", new { area = "Account", Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        // GET: /Account/Settings/DeleteAccount
        public ActionResult DeleteAccount()
        {
            return View();
        }

        //
        // POST: /Account/Settings/DeleteAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            if (UserManager.CheckPassword(user, model.Password))
            {
                // Delete UserDetails (connection with User [FK])
                user.RemoveUserDetails();

                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                AddErrors(result);
                model.Checked = false;
                return View(model);
            }
            else
            {
                IdentityResult uncorrectPassword = IdentityResult.Failed("You entered a wrong password.");
                AddErrors(uncorrectPassword);
                model.Checked = false;
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
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangeUsernameSuccess,
            ChangeEmailSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
    }
}