using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Models
{
    public class UserDetails
    {
        [Display(Name = "User ID")]
        [Key, ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
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

        public virtual MyUser User { get; set; }

        public UserDetails() { }

        public UserDetails(int userId, string firstName, string lastName)
        {
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}
