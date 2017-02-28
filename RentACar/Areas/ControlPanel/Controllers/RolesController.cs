using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RentACar.Models;

namespace RentACar.Areas.ControlPanel.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RolesController : Controller
    {        
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ControlPanel/Roles
        public async Task<ActionResult> Index(RoleMessageId? message)
        {
            ViewBag.StatusMessage =
                message == RoleMessageId.AddRole ? "Role has been successfully added."
                : message == RoleMessageId.ChangeRole ? "Role has been successfully changed."
                : message == RoleMessageId.AssignedRole ? "Role cannot be deleted. Users are assigned to selected role."
                : message == RoleMessageId.RemoveRole ? "Role has been successfully removed."
                : message == RoleMessageId.Error ? "An error has occurred."
                : "";

            if (message == RoleMessageId.Error || message == RoleMessageId.AssignedRole)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            return View(await db.Roles.OrderBy(m => m.Id).ToListAsync());
        }

        // GET: ControlPanel/Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ControlPanel/Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] MyRole myRole)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(myRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { Message = RoleMessageId.AddRole });
        }

        // GET: ControlPanel/Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyRole myRole = db.Roles.Find(id);
            if (myRole == null)
            {
                return HttpNotFound();
            }
            return View(myRole);
        }

        // POST: ControlPanel/Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] MyRole myRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(myRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = RoleMessageId.ChangeRole });
            }
            return View(myRole);
        }

        // GET: ControlPanel/Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyRole myRole = db.Roles.Find(id);
            if (myRole == null)
            {
                return HttpNotFound();
            }
            return View(myRole);
        }

        // POST: ControlPanel/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var role = db.Roles.Find(id);

            if (role.Users.Count > 0)
            {
                return RedirectToAction("Index", new { Message = RoleMessageId.AssignedRole });
            }

            MyRole myRole = db.Roles.Find(id);
            db.Roles.Remove(myRole);
            db.SaveChanges();
            
            return RedirectToAction("Index", new { Message = RoleMessageId.RemoveRole });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum RoleMessageId
        {
            AddRole,
            ChangeRole,
            AssignedRole,
            RemoveRole,
            Error
        }
    }
}
