using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Hotel19966292.Models
{
    public class Booking
    {
        // primary key
        [Key, Required]
        public int ID { get; set; }

        // foreign key
        [Required]
        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        // foreign key
        [Required]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress], implied above
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Check In Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }

        [Required]
        [Display(Name = "Check Out Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        public decimal Cost { get; set; }

        //Navigational Properties
        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }
    }
}
