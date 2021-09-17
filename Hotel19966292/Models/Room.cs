using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel19966292.Models
{
    public class Room
    {
        [Key, Required]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"[G123]{1}")]
        public String Level { get; set; }

        [RegularExpression(@"[123]{1}")]
        [Display(Name = "Bed Count")]
        public int BedCount { get; set; }

        [Range(50, 300)]
        public decimal Price { get; set; }

        public ICollection<Booking> TheBookings { get; set; }
    }
}
