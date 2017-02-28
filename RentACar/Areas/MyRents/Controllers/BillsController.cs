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
    public class BillsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyRents/Bills
        public async Task<ActionResult> Index(string sortOrder)
        {
            var userId = User.Identity.GetUserId<int>();
            var bills = db.Bills.Include(b => b.Rent).Where(b => b.Rent.UserId == userId);

            ViewBag.SortById = String.IsNullOrEmpty(sortOrder) ? "SortByIdDesc" : "SortByIdAsc";
            ViewBag.SortByDate =
                sortOrder == "SortByDateAsc" ? "SortByDateDesc" : "SortByDateAsc";

            switch (sortOrder)
            {
                case "SortByIdAsc":
                    return View(await bills.OrderBy(m => m.RentId).ToListAsync());
                case "SortByIdDesc":
                    return View(await bills.OrderByDescending(m => m.RentId).ToListAsync());
                case "SortByDateAsc":
                    return View(await bills.OrderBy(m => m.Date).ToListAsync());
                case "SortByDateDesc":
                    return View(await bills.OrderByDescending(m => m.Date).ToListAsync());
            }
            return View(await bills.ToListAsync());
        }

        // GET: MyRents/Bills/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId<int>();
            Bill bill = await db.Bills.FindAsync(id);

            if (bill == null || bill.Rent.UserId != userId)
            {
                return HttpNotFound();
            }
            return View(bill);
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
