using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RentACar.Models;
using RentACar.Areas.ControlPanel.Models;

namespace RentACar.Areas.ControlPanel.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ControlPanel/Users
        public async Task<ActionResult> Index(string sortOrder, UserMessageId? message)
        {
            ViewBag.StatusMessage =
                message == UserMessageId.ChangeUser ? "User has been successfully changed."
                : message == UserMessageId.RemoveUser ? "User has been successfully removed."
                : message == UserMessageId.Error ? "An error has occurred."
                : "";

            if (message == UserMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            List<UserListViewModel> userList = new List<UserListViewModel>();

            var users = new List<MyUser>();

            using (var db = new ApplicationDbContext())
            {
                users = await db.Users.OrderBy(m => m.Id).ToListAsync();

                foreach (MyUser singleUser in users)
                {
                    IList<string> userRole;

                    using (var userManager = new ApplicationUserManager(new MyUserStore(new ApplicationDbContext())))
                    {
                        userRole = await userManager.GetRolesAsync(singleUser.Id);
                    };

                    var user = new UserListViewModel
                    {
                        UserId = singleUser.Id,
                        UserName = singleUser.UserName,
                        FirstName = singleUser.UserDetails.FirstName,
                        LastName = singleUser.UserDetails.LastName,
                        Role = userRole.Count > 0 ? userRole[0].ToString() : ""
                    };
                    userList.Add(user);
                }
            }

            ViewBag.SortById = String.IsNullOrEmpty(sortOrder) ? "SortByIdDesc" : "SortByIdAsc";
            ViewBag.SortByUsername =
                sortOrder == "SortByUsernameAsc" ? "SortByUsernameDesc" : "SortByUsernameAsc";

            switch (sortOrder)
            {
                case "SortByIdAsc":
                    return View(userList.OrderBy(m => m.UserId));
                case "SortByIdDesc":
                    return View(userList.OrderByDescending(m => m.UserId));
                case "SortByUsernameAsc":
                    return View(userList.OrderBy(m => m.UserName));
                case "SortByUsernameDesc":
                    return View(userList.OrderByDescending(m => m.UserName));
            }

            return View(userList);
        }

        // GET: ControlPanel/Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyUser myUser = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (myUser == null)
            {
                return HttpNotFound();
            }

            var model = new UserDetailsViewModel
            {
                UserId = myUser.Id,
                Role = "",
                UserName = myUser.UserName,
                Password = "",
                EmailAddress = myUser.Email,
                EmailAddressConfirmed = myUser.EmailConfirmed,
                AccessFailedCount = myUser.AccessFailedCount,
                LockoutEnabled = myUser.LockoutEnabled,
                LockoutEndDate = myUser.LockoutEndDateUtc,
                FirstName = myUser.UserDetails.FirstName != null ? myUser.UserDetails.FirstName : "--",
                LastName = myUser.UserDetails.LastName != null ? myUser.UserDetails.LastName : "--",
                Age = myUser.UserDetails.Age,
                Address = myUser.UserDetails.Address != null ? myUser.UserDetails.Address : "--",
                City = myUser.UserDetails.City != null ? myUser.UserDetails.City : "--",
                Country = myUser.UserDetails.Country != null ? myUser.UserDetails.Country : "--"
            };

            return View(model);
        }

        // GET: ControlPanel/Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyUser myUser = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (myUser == null)
            {
                return HttpNotFound();
            }
            // ViewBag.Id = new SelectList(db.UserDetails, "UserId", "FirstName", myUser.Id);
            
            var model = new UserDetailsViewModel
            {
                UserId = myUser.Id,
                Role = db.Roles.FirstOrDefault(m => m.Users.FirstOrDefault(n => n.UserId == myUser.Id).RoleId == m.Id).Id.ToString(),
                UserName = myUser.UserName,
                Password = "",
                EmailAddress = myUser.Email,
                EmailAddressConfirmed = myUser.EmailConfirmed,
                AccessFailedCount = myUser.AccessFailedCount,
                LockoutEnabled = myUser.LockoutEnabled,
                LockoutEndDate = myUser.LockoutEndDateUtc,
                FirstName = myUser.UserDetails.FirstName,
                LastName = myUser.UserDetails.LastName,
                Age = myUser.UserDetails.Age,
                Address = myUser.UserDetails.Address,
                City = myUser.UserDetails.City,
                Country = myUser.UserDetails.Country
            };

            ViewBag.Roles = new SelectList(db.Roles.AsEnumerable().ToList(),
                "Id", "Name", db.Roles.FirstOrDefault(m => m.Id.ToString() == model.Role));

            return View(model);
        }

        // POST: ControlPanel/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {
                    MyUser user = db.Users.Find(model.UserId);

                    user.Id = model.UserId;
                    user.UserName = model.UserName;
                    user.Email = model.EmailAddress;
                    user.EmailConfirmed = model.EmailAddressConfirmed;
                    user.AccessFailedCount = model.AccessFailedCount;
                    user.LockoutEnabled = model.LockoutEnabled;
                    user.LockoutEndDateUtc = model.LockoutEndDate;

                    user.UserDetails.FirstName = model.FirstName;
                    user.UserDetails.LastName = model.LastName;
                    user.UserDetails.Age = model.Age;
                    user.UserDetails.Address = model.Address;
                    user.UserDetails.City = model.City;
                    user.UserDetails.Country = model.Country;

                    MyUserRole currentRole = db.Users.FirstOrDefault(m => m.Id == model.UserId)
                        .Roles.FirstOrDefault();

                    db.Users.FirstOrDefault(m => m.Id == model.UserId)
                        .Roles.Remove(currentRole);
                    db.Users.FirstOrDefault(m => m.Id == model.UserId)
                        .Roles.Add(new MyUserRole { RoleId = int.Parse(model.Role), UserId = model.UserId });

                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    ApplicationUserManager userManager = new ApplicationUserManager(new MyUserStore(new ApplicationDbContext()));

                    // Dodati da se novi password salje ;)

                    await userManager.RemovePasswordAsync(model.UserId);
                    await userManager.AddPasswordAsync(model.UserId, model.Password);
                }
                return RedirectToAction("Index", new { Message = UserMessageId.ChangeUser });
            }

            ViewBag.Roles = new SelectList(db.Roles.AsEnumerable().ToList(),
                "Id", "Name", db.Roles.FirstOrDefault(m => m.Id.ToString() == model.Role));

            ViewBag.Id = new SelectList(db.UserDetails, "Id", "Role", model.Role);
            return View(model);
        }

        // GET: ControlPanel/Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyUser myUser = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (myUser == null)
            {
                return HttpNotFound();
            }
            return View(myUser);
        }

        // POST: ControlPanel/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MyUser myUser = await db.Users.FirstOrDefaultAsync(m => m.Id == id);
            db.Users.Find(id).RemoveUserDetails();
            db.Users.Remove(myUser);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { Message = UserMessageId.RemoveUser });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum UserMessageId
        {
            ChangeUser,
            RemoveUser,
            Error
        }
    }
}
