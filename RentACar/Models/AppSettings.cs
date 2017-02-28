using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class AppSettings
    {
        public int AppSettingsId { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Username")]
        public string EmailUsername { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string EmailPassword { get; set; }
    }
}
