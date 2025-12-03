using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Models;
namespace PrivateWorkshop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<WorkshopSlot> WorkshopSlots { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<WorkshopSlot>()
                .HasIndex(ws => new { ws.WorkshopId, ws.Date, ws.Duration })
                .IsUnique(); // no 2 rows in same slot

            builder.Entity<WorkshopSlot>()
                .Property(ws => ws.RowVersion)
                .IsRowVersion();
        }

    }
}
