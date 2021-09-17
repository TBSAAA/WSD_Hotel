using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel19966292.Models
{
    public class Customer
    {
        [Key, Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Surname")]
        [RegularExpression(@"[A-Z][a-z'-]{1,19}", ErrorMessage = "must start with a captical letter and continue with 2-20 lower-case letters or hyphen or apostrophe.")]
        public string Surname { get; set; }

        [Required, Display(Name = "Given Name")]
        [RegularExpression(@"[A-Z][a-z'-]{1,19}", ErrorMessage = "must start with a captical letter and continue with 2-20 lower-case letters or hyphen or apostrophe.")]
        public string GivenName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{GivenName} {Surname}";

        [Required]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"[0-9]{4}$", ErrorMessage = "4 digits")]
        public string Postcode { get; set; }

        // Navigation properties
        public ICollection<Booking> TheBookings { get; set; }
    }
}
