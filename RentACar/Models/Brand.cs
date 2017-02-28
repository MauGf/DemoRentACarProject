using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class Brand
    {
        [Required]
        [Display(Name = "Brand ID")]
        public int BrandId { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual IEnumerable<Car> Cars { get; set; }

        public Brand() { }
    }
}
