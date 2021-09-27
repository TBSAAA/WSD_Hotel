using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel19966292.Data;
using Hotel19966292.Models;
using Microsoft.Data.Sqlite;

namespace Hotel19966292.Pages.Bookings
{
    [Authorize(Roles = "customers")]
    public class CreateModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public CreateModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public BookingViewModel Booking { get; set; }
        public IList<Booking> BookedRooms { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // retrieve the logged-in user's email
            // need to add "using System.Security.Claims;"
            string _email = User.FindFirst(ClaimTypes.Name).Value;

            Customer customer = await _context.Customer.FirstOrDefaultAsync(m => m.Email == _email);

            if (customer == null)
            {
                return RedirectToPage("../Customers/MyDetails");
            }
            
            var roomID = new SqliteParameter("@RoomID", Booking.RoomID);
            var checkIn = new SqliteParameter("@CheckIn", Booking.CheckIn);
            var checkOut = new SqliteParameter("@CheckOut", Booking.CheckOut);


            /* 
             
             A -----[-----]------
             
             B ---[-----]--------   case 1
             
             B --------[-----]---   case 2

             B -------[--]-------   case 3
             
             */


            var bookedRooms = _context.Booking.FromSqlRaw("" +
                "SELECT * " +
                "FROM Booking " +
                "WHERE RoomID = @RoomID ADN " +
                "   CheckIn < @CheckOut AND " +
                "   @CheckIn < CheckOut", roomID, checkIn, checkOut);
            //var bookedRooms = _context.Booking.FromSqlRaw("" +
            //    "SELECT * " +
            //    "FROM Booking " +
            //    "WHERE RoomID = @RoomID", roomID);


            BookedRooms = await bookedRooms.ToListAsync();

            if(BookedRooms.Count() == 0)
            {

                Room theRoom = await _context.Room.FindAsync(Booking.RoomID);
                int days = (Booking.CheckOut - Booking.CheckIn).Days;

                Booking booking = new Booking
                {
                    RoomID = Booking.RoomID,
                    CustomerEmail = _email,
                    CheckIn = Booking.CheckIn,
                    CheckOut = Booking.CheckOut,
                    Cost = days * theRoom.Price
                };

                _context.Booking.Add(booking);

                try  // catching the conflict of editing this record concurrently
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                ViewData["SuccessDB"] = "success";
                ViewData["Room"] = booking.RoomID;
                ViewData["Level"] = theRoom.Level;
                ViewData["CheckIn"] = booking.CheckIn.ToString("d");
                ViewData["CheckOut"] = booking.CheckOut.ToString("d");
                ViewData["Cost"] = booking.Cost;

            }
            else
            {
                ViewData["SuccessDB"] = "fail";
            }
            return Page();
        }
    }
}
