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
    public class IndexModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public IndexModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder)
        {

            string _email = User.FindFirst(ClaimTypes.Name).Value;

            Customer customer = await _context.Customer.FirstOrDefaultAsync(m => m.Email == _email);

            if (customer == null)
            {
                return RedirectToPage("../Customers/MyDetails");
            }

            var booking = (IQueryable<Booking>)_context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom);

            booking = booking.Where(b => b.CustomerEmail.Contains(customer.Email));

            // Sort the movies by specified order
            switch (sortOrder)
            {
                case "checkIn_asc":
                    booking = booking.OrderBy(b => b.CheckIn);
                    break;
                case "checkIn_desc":
                    booking = booking.OrderByDescending(b => b.CheckIn);
                    break;
                case "cost_asc":
                    booking = booking.OrderBy(b => (double)b.Cost);
                    break;
                case "cost_desc":
                    booking = booking.OrderByDescending(b => (double)b.Cost);
                    break;
                default:
                    booking = booking.OrderBy(b => b.CheckIn);
                    break;
            }


            ViewData["NextCheckIn"] = sortOrder != "checkIn_asc" ? "checkIn_asc" : "checkIn_desc";
            ViewData["NextCost"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";

            Booking = await booking.AsNoTracking().ToListAsync();

            return Page();
        }
    }
}
