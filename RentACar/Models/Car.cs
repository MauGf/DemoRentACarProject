using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Car
    {
        private int _NumberInUse;
        private int _NumberReserved;

        [Required]
        [Display(Name = "Car ID")]
        public int CarId { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Cost per day")]
        public double CostPerDay { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int NumberTotal { get; set; }

        [Required]
        [Display(Name = "Reserved")]
        
        public int NumberReserved
        {
            get { return _NumberReserved; }
            set { this._NumberReserved = this.NumberOfReservedCars(); }
        }

        [Required]
        [Display(Name = "In use")]
        public int NumberInUse
        {
            get { return _NumberInUse; }
            set { this._NumberInUse = this.NumberOfInUseCars(); }
        }

        // Foreign keys
        public int CarTypeId { get; set; }
        public int BrandId { get; set; }

        // Relations
        public virtual CarType Type { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual IEnumerable<Rent> Rents { get; set; }
        public virtual IEnumerable<Reservation> Reservations { get; set; }

        public Car()
        {
            this.Rents = new HashSet<Rent>();
            this.Reservations = new HashSet<Reservation>();
        }

        public bool isAvailable()
        {
            if (this.NumberOfReservedCars() + this.NumberOfInUseCars() < this.NumberTotal )
            {
                return true;
            }
            return false;
        }

        public int NumberOfReservedCars()
        {
            return new ApplicationDbContext()
                .Reservations.Where(m => m.CarId == this.CarId && m.Reserved == true).Count();
        }

        public int NumberOfInUseCars()
        {
            return new ApplicationDbContext()
                .Rents.Where(m => m.CarId == this.CarId && m.Returned == false).Count();
        }
    }
}
