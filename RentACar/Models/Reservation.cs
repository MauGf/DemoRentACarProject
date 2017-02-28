using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Reservation
    {
        [Display(Name = "Reservation ID")]
        public int ReservationId { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Reserved")]
        public bool Reserved { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int CarId { get; set; }

        // Relationships
        public virtual Car Car { get; set; }
        public virtual MyUser User { get; set; }

        public Reservation()
        {
            this.Reserved = true;
        }

        public bool StartRenting()
        {
            try
            {
                this.Reserved = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDate()
        {
            if(this.Date >= DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }
    }
}
