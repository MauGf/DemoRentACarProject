using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class CarType
    {
        [Required]
        [Display(Name = "Type ID")]
        public int CarTypeId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Name { get; set; }

        public virtual IEnumerable<Car> Cars { get; set; }

        public CarType() { }
    }
}
