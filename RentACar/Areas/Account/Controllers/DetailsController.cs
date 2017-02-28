using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RentACar.Models;
using RentACar.Areas.Account.Models;

namespace RentACar.Areas.Account.Controllers
{
    [Authorize]
    public class DetailsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public DetailsController() { }

        public DetailsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Account/Details
        public ActionResult Index()
        {
            ViewBag.SubTitle = "Details";

            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            var model = new DetailsViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.UserDetails.FirstName,
                LastName = user.UserDetails.LastName,
                Age = user.UserDetails.Age.GetValueOrDefault(),
                Address = user.UserDetails.Address,
                City = user.UserDetails.City,
                Country = user.UserDetails.Country
            };

            return View(model);
        }

        // POST: /Account/Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(DetailsViewModel model)
        {
            ViewBag.SubTitle = "Details";

            if (!ModelState.IsValid)
            {
                return View();
            }

            MyUser user = UserManager.FindByName<MyUser, int>(model.UserName);

            user.UserDetails.FirstName = model.FirstName;
            user.UserDetails.LastName = model.LastName;
            user.UserDetails.Age = model.Age;
            user.UserDetails.Address = model.Address;
            user.UserDetails.City = model.City;
            user.UserDetails.Country = model.Country;

            // Returns result if user is updated or not
            IdentityResult result = await UserManager.UpdateAsync(user);

            return View();
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
    }
}