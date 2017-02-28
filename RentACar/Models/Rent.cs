using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Rent
    {
        [Display(Name = "Rent ID")]
        public int RentId { get; set; }

        [Display(Name = "Start date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Paid")]
        public bool Paid { get; set; }

        [Display(Name = "Billed")]
        public bool Billed { get; set; }

        [Display(Name = "Returned")]
        public bool Returned { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int CarId { get; set; }

        // Relations
        public virtual MyUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual Bill Bill { get; set; }

        public Rent()
        {
            this.Returned = false;
        }

        public bool ReturnCar()
        {
            try
            {
                this.Returned = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDate()
        {
            if(this.StartDate <= this.EndDate && this.StartDate >= DateTime.Now.Date && this.EndDate >= DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }
    }
}
