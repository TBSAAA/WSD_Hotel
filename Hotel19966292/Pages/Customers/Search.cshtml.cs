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

namespace Hotel19966292.Pages.Customers
{
    [Authorize(Roles = "customers")]
    public class SearchModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public SearchModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SearchViewModel Myself { get; set; }
        public IList<Room> Rooms { get; set; }



        public IActionResult OnGet()
        {
            ViewData["BedCount"] = new SelectList(new int[] { 1, 2, 3 });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["BedCount"] = new SelectList(new int[] { 1, 2, 3 });
            var bedCount = new SqliteParameter("@BedCount", Myself.BedCount);
            var checkIn = new SqliteParameter("@CheckIn", Myself.CheckIn);
            var checkOut = new SqliteParameter("@CheckOut", Myself.CheckOut);

            var rooms = _context.Room.FromSqlRaw("" +
                "SELECT * " +
                "FROM Room " +
                "WHERE BedCount = @BedCount " +
                "AND ID NOT IN " +
                "(SELECT RoomID " +
                "From Booking " +
                "WHERE CheckIn < @CheckOut " +
                "AND @CheckIn < CheckOut)", bedCount, checkIn, checkOut);


            // SELECT * FROM Room WHERE BedCount = 2 AND ID NOT IN (SELECT RoomID From Booking WHERE CheckIn < 2021-10-20 AND 2021-10-15 < CheckOut)

            Rooms = await rooms.ToListAsync();

            if(Rooms.Count() == 0)
            {
                ViewData["SuccessDB"] = "fail";
            }
            else
            {
                ViewData["SuccessDB"] = "true";
            }
            
            return Page();
        }
    }
}
