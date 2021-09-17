using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hotel19966292.Models;

namespace Hotel19966292.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hotel19966292.Models.Room> Room { get; set; }
        public DbSet<Hotel19966292.Models.Booking> Booking { get; set; }
        public DbSet<Hotel19966292.Models.Customer> Customer { get; set; }
    }
}
