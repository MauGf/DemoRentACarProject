using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RentACar.Models;
using System.Linq;

namespace RentACar.Areas.MyDesk.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class CarsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyDesk/Cars
        public async Task<ActionResult> Index(string sortOrder, CarMessageId? message)
        {
            ViewBag.StatusMessage =
                message == CarMessageId.CarAdded ? "Car has been successfully added."
                : message == CarMessageId.CarEdited ? "Car has been successfully edited."
                : message == CarMessageId.CarDeleted ? "Car has been successfully deleted."
                : message == CarMessageId.Error ? "An error has occurred."
                : "";

            if (message == CarMessageId.Error)
            {
                ViewBag.StatusClass = "alert-danger";
            }
            else
            {
                ViewBag.StatusClass = "alert-success";
            }

            var cars = db.Cars.Include(c => c.Brand).Include(c => c.Type);

            ViewBag.SortByName =
                sortOrder == "SortByNameAsc" ? "SortByNameDesc" : "SortByNameAsc";

            switch (sortOrder)
            {
                case "SortByNameAsc":
                    return View(await cars.OrderBy(m => m.Brand.Name).ThenBy(m => m.Model).ToListAsync());
                case "SortByNameDesc":
                    return View(await cars.OrderByDescending(m => m.Brand.Name).ThenByDescending(m => m.Model).ToListAsync());
            }
            return View(await cars.ToListAsync());
        }

        // GET: MyDesk/Cars/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = await db.Cars.FindAsync(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: MyDesk/Cars/Create
        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name");
            ViewBag.CarTypeId = new SelectList(db.CarTypes, "CarTypeId", "Name");
            return View();
        }

        // POST: MyDesk/Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CarId,Model,CostPerDay,NumberTotal,CarTypeId,BrandId")] Car car, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if(imageFile != null)
                {
                    imageFile.SaveAs(HttpContext.Server.MapPath("~/Images/Cars/") + imageFile.FileName);
                    car.ImageUrl = "/Images/Cars/" + imageFile.FileName;
                }

                db.Cars.Add(car);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = CarMessageId.CarAdded });
            }

            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", car.BrandId);
            ViewBag.CarTypeId = new SelectList(db.CarTypes, "CarTypeId", "Name", car.CarTypeId);
            return View(car);
        }

        // GET: MyDesk/Cars/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = await db.Cars.FindAsync(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", car.BrandId);
            ViewBag.CarTypeId = new SelectList(db.CarTypes, "CarTypeId", "Name", car.CarTypeId);
            return View(car);
        }

        // POST: MyDesk/Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CarId,Model,CostPerDay,NumberTotal,NumberReserved,NumberInUse,CarTypeId,BrandId")] Car car, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    //var currentCarImageUrl = db.Entry(car).OriginalValues.GetValue<string>("ImageUrl");

                    //if (System.IO.File.Exists(Server.MapPath("~/Images/Cars/" + currentCarImageUrl)))
                    //{
                    //    System.IO.File.Delete(Server.MapPath("~/Images/Cars/" + currentCarImageUrl));
                    //}

                    imageFile.SaveAs(HttpContext.Server.MapPath("~/Images/Cars/") + imageFile.FileName);
                    car.ImageUrl = "/Images/Cars/" + imageFile.FileName;
                }

                db.Entry(car).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Message = CarMessageId.CarEdited });
            }
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", car.BrandId);
            ViewBag.CarTypeId = new SelectList(db.CarTypes, "CarTypeId", "Name", car.CarTypeId);
            return View(car);
        }

        // GET: MyDesk/Cars/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = await db.Cars.FindAsync(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: MyDesk/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Car car = await db.Cars.FindAsync(id);
            db.Cars.Remove(car);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { Message = CarMessageId.CarDeleted });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum CarMessageId
        {
            CarAdded,
            CarEdited,
            CarDeleted,
            Error
        }
    }
}
