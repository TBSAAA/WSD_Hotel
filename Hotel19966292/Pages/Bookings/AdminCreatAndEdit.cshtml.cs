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
    public class AdminCreatAndEditModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public AdminCreatAndEditModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Myself { get; set; }
        public IList<Booking> BookedRooms { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "FullName");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");

            // Creat page
            if (id == null)
            {
                ViewData["ExistInDB"] = "false";
                return Page();
            }

            // Edit page
            ViewData["ExistInDB"] = "true";
            Myself = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom).AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {

            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "FullName");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");

            // Creat page
            if (id == null)
            {
                ViewData["ExistInDB"] = "false";
            }
            // Edit page
            else
            {
                ViewData["ExistInDB"] = "true";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var roomID = new SqliteParameter("@RoomID", Myself.RoomID);
            var checkIn = new SqliteParameter("@CheckIn", Myself.CheckIn);
            var checkOut = new SqliteParameter("@CheckOut", Myself.CheckOut);

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

            BookedRooms = await bookedRooms.AsNoTracking().ToListAsync();
            int i = 0;

            // Check if that order is the customer's own.
            // just for edit page
            if ((string)ViewData["ExistInDB"] == "true")
            {
                foreach (var flag in BookedRooms)
                {
                    if (flag.ID == Myself.ID || flag.RoomID == Myself.RoomID)
                    {
                        i++;
                    }            
                }
            }

            if (BookedRooms.Count() == 0 || i == 1)
            {
                // for edit page
                if ((string)ViewData["ExistInDB"] == "true")
                {
                    _context.Attach(Myself).State = EntityState.Modified;
                }
                // for creat page
                else
                {
                    Booking add = new Booking
                    {
                        RoomID = Myself.RoomID,
                        CustomerEmail = Myself.CustomerEmail,
                        CheckIn = Myself.CheckIn,
                        CheckOut = Myself.CheckOut,
                        Cost = Myself.Cost
                    };
                    _context.Booking.Add(add);
                }

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
