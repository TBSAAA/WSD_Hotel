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
    public class EditModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public EditModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        //public BookingViewModel BookingModify { get; set; }
        public IList<Booking> BookedRooms { get; set; }
        public Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom).FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null)
            {
                return NotFound();
            }
           ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "FullName");
           ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "FullName");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");

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

                _context.Attach(booking).State = EntityState.Modified;

                try  // catching the conflict of editing this record concurrently
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(Booking.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("./AdminIndex");
            }
            else
            {
                ViewData["SuccessDB"] = "fail";
            }

            return Page();




            //_context.Attach(Booking).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!BookingExists(Booking.ID))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return RedirectToPage("./AdminIndex");
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
    }
}
