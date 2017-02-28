using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RentACar.Models;
using System.Linq;

namespace RentACar.Areas.MyDesk.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class BillsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyDesk/Bills
        public async Task<ActionResult> Index(string sortOrder, BillMessageId? message)
        {
            ViewBag.StatusMessage =
                message == BillMessageId.AddBill ? "Bill has been successfully added."
                : message == BillMessageId.ChangeBill ? "Bill has been successfully changed."
                : message == BillMessageId.RemoveBill ? "Bill has been successfully removed."
                : message == BillMessageId.PayBill ? "Bill has been successfully paid."
                : message == BillMessageId.Error ? "An error has occured."
                : "";

            if (message == BillMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            var bills = db.Bills.Include(b => b.Rent);

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

        // GET: MyDesk/Bills/Create
        public ActionResult Create(int id)
        {
            var rent = db.Rents.Find(id);

            var bill = new Bill
            {
                RentId = id,
                Rent = rent,
                Date = DateTime.Now
            };

            bill.CalculateTotalCost(rent.StartDate, rent.EndDate, rent.Car.CostPerDay);

            db.Rents.Find(bill.RentId).Billed = true;
            db.Bills.Add(bill);
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = BillMessageId.AddBill });
        }

        // GET: MyDesk/Bills/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = await db.Bills.FindAsync(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            ViewBag.RentId = new SelectList(db.Rents, "RentId", "RentId", bill.RentId);
            return View(bill);
        }

        // POST: MyDesk/Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RentId,Date,Cost")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = BillMessageId.ChangeBill });
            }
            ViewBag.RentId = new SelectList(db.Rents, "RentId", "RentId", bill.RentId);
            return View(bill);
        }

        // GET: MyDesk/Bills/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = await db.Bills.FindAsync(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // POST: MyDesk/Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Bill bill = await db.Bills.FindAsync(id);
            db.Bills.Remove(bill);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { Message = BillMessageId.RemoveBill });
        }

        // GET: MyDesk/Bills/Pay/5
        public async Task<ActionResult> Pay(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Bill bill = await db.Bills.FindAsync(id);
            if(bill == null)
            {
                return HttpNotFound();
            }

            bill.Rent.Paid = true;

            db.Entry(bill).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = BillMessageId.PayBill });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum BillMessageId
        {
            AddBill,
            ChangeBill,
            RemoveBill,
            PayBill,
            Error
        }
    }
}
