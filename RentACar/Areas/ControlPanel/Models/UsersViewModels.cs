using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Areas.ControlPanel.Models
{
    public class UserListViewModel
    {
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Current role")]
        public string Role { get; set; }
    }

    public class UserDetailsViewModel
    {
        [Required]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Email confirmed")]
        public bool EmailAddressConfirmed { get; set; }

        [Display(Name = "Failed login count")]
        public int AccessFailedCount { get; set; }

        [Display(Name = "Lockout")]
        public bool LockoutEnabled { get; set; }

        [Display(Name = "Lockout end date")]
        public DateTime? LockoutEndDate { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}
