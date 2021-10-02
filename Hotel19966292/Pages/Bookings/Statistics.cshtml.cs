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
    public class StatisticsModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public StatisticsModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // For passing the results to the Content file
        public IList<Statistics> CustomerStats { get; set; }
        public IList<Statistics> RoomStats { get; set; }

        // GET: Movies/CalcGenreStats
        public async Task<IActionResult> OnGetAsync()
        {
            // divide the customers into groups by Postcode
            var customerG = _context.Customer.GroupBy(c => c.Postcode);

            // for each group, get its genre value and the number of movies in this group
            CustomerStats = await customerG.Select(n => new Statistics { PostCode = n.Key, CustomersCount = n.Count() }).ToListAsync();

            // divide the movies into groups by genre
            var roomG = _context.Booking.GroupBy(r => r.RoomID);

            // for each group, get its genre value and the number of movies in this group
            RoomStats = await roomG.Select(n => new Statistics { RoomID = n.Key, RoomBookingCount = n.Count() }).ToListAsync();

            return Page();
        }
    }
}
