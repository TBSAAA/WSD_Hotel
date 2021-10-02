using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Hotel19966292.Models
{
    public class Statistics
    {
        [Display(Name = "Number of Customers")]
        public int CustomersCount { get; set; }

        [Display(Name = "Customer PostCode")]
        public String PostCode { get; set; }

        [Display(Name = "Room ID")]
        public int RoomID { get; set; }

        [Display(Name = "Number of Bookings")]
        public int RoomBookingCount { get; set; }
    }
}
