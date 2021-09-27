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
    public class AdminIndexModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public AdminIndexModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; }

        public async Task OnGetAsync()
        {
            Booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom).ToListAsync();
        }
    }
}
