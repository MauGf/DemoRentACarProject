using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RentACar.Models;

namespace RentACar.Areas.MyDesk.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class ReservationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyDesk/Reservations
        public async Task<ActionResult> Index(string sortOrder, ReservationMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ReservationMessageId.ReservationAdded ? "Reservation has been successfully added."
                : message == ReservationMessageId.ReservationEdited ? "Reservation has been successfully edited."
                : message == ReservationMessageId.ReservationDeleted ? "Reservation has been successfully deleted."
                : message == ReservationMessageId.ReservationRented ? "Reservation has been changed to rent."
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

            var res = db.Reservations.ToList();
            var reservations = db.Reservations.Include(r => r.Car).Include(r => r.User);

            ViewBag.SortById = String.IsNullOrEmpty(sortOrder) ? "SortByIdDesc" : "SortByIdAsc";
            ViewBag.SortByDate =
                sortOrder == "SortByDateAsc" ? "SortByDateDesc" : "SortByDateAsc";

            switch (sortOrder)
            {
                case "SortByIdAsc":
                    return View(await reservations.OrderBy(m => m.ReservationId).ToListAsync());
                case "SortByIdDesc":
                    return View(await reservations.OrderByDescending(m => m.ReservationId).ToListAsync());
                case "SortByDateAsc":
                    return View(await reservations.OrderBy(m => m.Date).ToListAsync());
                case "SortByDateDesc":
                    return View(await reservations.OrderByDescending(m => m.Date).ToListAsync());
            }

            return View(await reservations.ToListAsync());
        }

        // GET: MyDesk/Reservations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // GET: MyDesk/Reservations/Create
        public ActionResult Create()
        {
            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });

            IEnumerable<MyUser> users = db.Users
                .ToList()
                .Select(m => new MyUser
                {
                    Id = m.Id,
                    UserName = m.UserDetails.FirstName + " " + m.UserDetails.LastName
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model");
            ViewBag.UserId = new SelectList(users, "Id", "UserName");
            return View();
        }

        // POST: MyDesk/Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ReservationId,Date,UserId,CarId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if(db.Cars.Find(reservation.CarId).isAvailable() && reservation.CheckDate())
                {
                    db.Reservations.Add(reservation);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationAdded });
                }
            }

            return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationError });
        }

        // GET: MyDesk/Reservations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
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

            IEnumerable<MyUser> users = db.Users
                .ToList()
                .Select(m => new MyUser
                {
                    Id = m.Id,
                    UserName = m.UserDetails.FirstName + " " + m.UserDetails.LastName
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model", reservation.CarId);
            ViewBag.UserId = new SelectList(users, "Id", "UserName", reservation.UserId);
            return View(reservation);
        }

        // POST: MyDesk/Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReservationId,Date,UserId,CarId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var car = db.Cars.Find(reservation.CarId);
                if(car.isAvailable() && reservation.CheckDate())
                {
                    db.Entry(reservation).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationEdited });
                }
            }

            return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationError });
        }

        // GET: MyDesk/Reservations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: MyDesk/Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reservation reservation = await db.Reservations.FindAsync(id);
            if (db.Cars.Find(reservation.CarId).NumberOfReservedCars() > 0)
            {
                db.Reservations.Remove(reservation);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationDeleted });
        }

        public ActionResult RentIt(int id)
        {
            var reservation = db.Reservations.Find(id);

            IEnumerable<Car> cars = db.Cars
                .ToList()
                .Select(m => new Car
                {
                    CarId = m.CarId,
                    Model = m.Brand.Name + " " + m.Model
                });

            IEnumerable<MyUser> users = db.Users
                .ToList()
                .Select(m => new MyUser
                {
                    Id = m.Id,
                    UserName = m.UserDetails.FirstName + " " + m.UserDetails.LastName
                });

            ViewBag.CarId = new SelectList(cars, "CarId", "Model", reservation.CarId);
            ViewBag.UserId = new SelectList(users, "Id", "UserName", reservation.UserId);

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RentIt(Reservation reservation, DateTime endDate)
        {
            var oldReservation = db.Reservations.Find(reservation.ReservationId);
            
            db.Rents.Add(new Rent
            {
                Car = reservation.Car,
                CarId = reservation.CarId,
                StartDate = DateTime.Now,
                EndDate = endDate,
                Paid = false,
                User = reservation.User,
                UserId = reservation.UserId
            });

            oldReservation.StartRenting();

            db.Reservations.Remove(oldReservation);
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ReservationMessageId.ReservationRented });
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
            ReservationRented,
            Error
        }
    }
}
