using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoodenAPI_S.Models
{
    public class WoodenDbContext : IdentityDbContext<ApplicationUser>
    {
        public WoodenDbContext(DbContextOptions<WoodenDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Country> Country { get; set; }
    }
}
