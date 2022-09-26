using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace RPMFuelService.Data
{
 
    public class FuelDbContext : DbContext
    {
        public FuelDbContext(DbContextOptions<FuelDbContext> options) : base(options)
        {
        }

        public DbSet<FuelData> FuelDatas { get; set; }
     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FuelData>().ToTable("FuelData");
        }
    }
}
