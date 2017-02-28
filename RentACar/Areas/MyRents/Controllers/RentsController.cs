using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RentACar.Models;
using Microsoft.AspNet.Identity;

namespace RentACar.Areas.MyRents.Controllers
{
    [Authorize(Roles = "User")]
    public class RentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyRents/Rents
        public async Task<ActionResult> Index(string sortOrder)
        {
            var userId = User.Identity.GetUserId<int>();
            var rents = db.Rents.Include(r => r.Bill).Include(r => r.Car).Include(r => r.User)
                .Where(r => r.UserId == userId);

            ViewBag.SortById = String.IsNullOrEmpty(sortOrder) ? "SortByIdDesc" : "SortByIdAsc";
            ViewBag.SortByDate =
                sortOrder == "SortByDateAsc" ? "SortByDateDesc" : "SortByDateAsc";

            switch (sortOrder)
            {
                case "SortByIdAsc":
                    return View(await rents.OrderBy(m => m.RentId).ToListAsync());
                case "SortByIdDesc":
                    return View(await rents.OrderByDescending(m => m.RentId).ToListAsync());
                case "SortByDateAsc":
                    return View(await rents.OrderBy(m => m.StartDate).ToListAsync());
                case "SortByDateDesc":
                    return View(await rents.OrderByDescending(m => m.StartDate).ToListAsync());
            }
            return View(await rents.ToListAsync());
        }

        // GET: MyRents/Rents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId<int>();
            Rent rent = await db.Rents.FindAsync(id);

            if (rent == null || rent.UserId != userId)
            {
                return HttpNotFound();
            }
            return View(rent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
