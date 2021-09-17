using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Hotel19966292.Data;
using Hotel19966292.Models;

namespace Hotel19966292.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly Hotel19966292.Data.ApplicationDbContext _context;

        public IndexModel(Hotel19966292.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Room> Room { get;set; }

        public async Task OnGetAsync()
        {
            Room = await _context.Room.ToListAsync();
        }
    }
}
