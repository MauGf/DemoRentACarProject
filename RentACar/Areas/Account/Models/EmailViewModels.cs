using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Areas.Account.Models
{
    public class EmailViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Confirmed")]
        public bool EmailConfirmed { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Required]
        [Display(Name = "New email")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }

        [Required]
        [Display(Name = "Confirm email")]
        [Compare("NewEmail", ErrorMessage = "The new email and confirmation email do not match.")]
        [DataType(DataType.EmailAddress)]
        public string ConfirmEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }
    }
}
