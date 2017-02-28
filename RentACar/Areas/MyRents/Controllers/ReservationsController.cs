using System;
using System.Collections.Generic;
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
    public class ReservationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyRents/Reservations
        public async Task<ActionResult> Index(string sortOrder, ReservationMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ReservationMessageId.ReservationAdded ? "Reservation has been successfully added."
                : message == ReservationMessageId.ReservationEdited ? "Reservation has been successfully edited."
                : message == ReservationMessageId.ReservationDeleted ? "Reservation has been successfully deleted."
                : message == ReservationMessageId.ReservationError ? "Reservation was not successfull. Capacity is full."
                : message == ReservationMessageId.Error ? "An error has occurred."
                : "";

            if (message == ReservationMessageId.Error || message == ReservationMessageId.ReservationError)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            var userId = User.Identity.GetUserId<int>();

            var reservations = db.Reservations.Include(r => r.Car).Include(r => r.User).Where(m => m.UserId == userId);

            ViewBag.SortById = String.IsNullOrEmpty(sortOrder) ? "SortByIdDesc" : "SortByIdAsc";

            switch (sortOrder)
            {
                case "SortByDateAsc":
                    return View(await reservations.OrderBy(m => m.Date).ToListAsync());
                case "SortByDateDesc":
                    return View(await reservations.OrderByDescending(m => m.Date).ToListAsync());
            }
            return View(await reservations.ToListAsync());
        }

        // GET: MyRents/Reservations/Create
        public ActionResult Create()
        {
            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model");
            return View();
        }

        // POST: MyRents/Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ReservationId,Date,CarId")] Reservation reservation)
        {
            reservation.UserId = User.Identity.GetUserId<int>();

            if (ModelState.IsValid && reservation.CheckDate())
            {
                db.Reservations.Add(reservation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationAdded });
            }

            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model", reservation.CarId);
            return View(reservation);
        }

        // GET: MyRents/Reservations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Reservation reservation = await db.Reservations.FindAsync(id);
            var userId = User.Identity.GetUserId<int>();

            if (reservation == null || reservation.UserId != userId)
            {
                return HttpNotFound();
            }

            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model", reservation.CarId);

            return View(reservation);
        }

        // POST: MyRents/Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReservationId,Date,CarId")] Reservation reservation)
        {
            reservation.UserId = User.Identity.GetUserId<int>();

            if (ModelState.IsValid && reservation.CheckDate())
            {
                db.Entry(reservation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationEdited });
            }

            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });
            ViewBag.CarId = new SelectList(cars, "CarId", "Model", reservation.CarId);
            return View(reservation);
        }

        // GET: MyRents/Reservations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Reservation reservation = await db.Reservations.FindAsync(id);
            var userId = User.Identity.GetUserId<int>();

            if (reservation == null || reservation.UserId != userId)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: MyRents/Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reservation reservation = await db.Reservations.FindAsync(id);
            db.Reservations.Remove(reservation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationDeleted });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum ReservationMessageId
        {
            ReservationAdded,
            ReservationEdited,
            ReservationDeleted,
            ReservationError,
            Error
        }
    }
}
