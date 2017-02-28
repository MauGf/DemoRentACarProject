using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Bill
    {
        public Bill() { }

        [Display(Name = "Rent ID")]
        [Key, ForeignKey("Rent")]
        public int RentId { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Cost")]
        public double Cost { get; set; }

        public virtual Rent Rent { get; set; }

        public void CalculateTotalCost(DateTime startDate, DateTime endDate, double costPerDay)
        {
            var days = (int) endDate.Subtract(startDate).TotalDays + 1;
            this.Cost = days * costPerDay;
        }
    }
}
