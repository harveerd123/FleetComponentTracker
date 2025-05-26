using FleetComponentTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetComponentTracker.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //Setup Database sets so we can query them like C# objects (abstraction shown here with EF as no need to write out raw SQL queries)
        public DbSet<FleetComponentTracker.Models.Components> Components { get; set; }

        public DbSet<Vehicles> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure relationship between Components & Vehicles
            modelBuilder.Entity<FleetComponentTracker.Models.Components>()
                .HasOne(c => c.Vehicle) //Component has 1 Vehicle relation
                .WithMany() // That Vehicle can have many Components
                .HasForeignKey(c => c.VehicleNumber) //FK on Components is VehicleNumber
                .HasConstraintName("FK_Components_Vehicles"); //Name the FK constraint in the database
        }
    }
}
