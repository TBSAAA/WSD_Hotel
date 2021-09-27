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
    [Authorize(Roles = "administrators")]
    public class AdminCreateModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;
        public AdminCreateModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            ViewData["CustomerFullName"] = new SelectList(_context.Customer, "Email", "FullName");
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
            ViewData["CustomerFullName"] = new SelectList(_context.Customer, "Email", "FullName");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var roomID = new SqliteParameter("@RoomID", Booking.RoomID);
            var checkIn = new SqliteParameter("@CheckIn", Booking.CheckIn);
            var checkOut = new SqliteParameter("@CheckOut", Booking.CheckOut);


            /* 
             
             A -----[-----]------
             
             B ---[-----]--------   case 1
             
             B --------[-----]---   case 2

             B -------[--]-------   case 3

             B --[------------]--   case 4
             
             */

            var bookedRooms = _context.Booking.FromSqlRaw("" +
                "SELECT * " +
                "FROM Booking " +
                "WHERE RoomID = @RoomID AND " +
                "   CheckIn < @CheckOut AND " +
                "   @CheckIn < CheckOut", roomID, checkIn, checkOut);


            BookedRooms = await bookedRooms.ToListAsync();

            if (BookedRooms.Count() == 0)
            {
                Booking booking = new Booking
                {
                    RoomID = Booking.RoomID,
                    CustomerEmail = Booking.CustomerEmail,
                    CheckIn = Booking.CheckIn,
                    CheckOut = Booking.CheckOut,
                    Cost = Booking.Cost
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

                return RedirectToPage("./AdminIndex");
            }
            else
            {
                ViewData["SuccessDB"] = "fail";
            }
            return Page();
        }
    }
}

