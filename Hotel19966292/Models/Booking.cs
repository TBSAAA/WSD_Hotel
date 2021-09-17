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
        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        // foreign key
        [DataType(DataType.EmailAddress)]
        //[EmailAddress], implied above
        public String CustomerEmail { get; set; }

        [Display(Name = "Check In Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }

        [Display(Name = "Check Out Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        public decimal Cost { get; set; }

        //Navigational Properties
        public Room TheRoom { get; set; }
        public Customer TheCustomer { get; set; }
    }
}
