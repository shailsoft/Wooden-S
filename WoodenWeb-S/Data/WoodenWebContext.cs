using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WoodenWeb_S.Models;

namespace WoodenWeb_S.Data
{
    public class WoodenWebContext : DbContext
    {
        public WoodenWebContext (DbContextOptions<WoodenWebContext> options)
            : base(options)
        {
        }

        public DbSet<WoodenWeb_S.Models.AppUser> AppUser { get; set; }
    }
}
