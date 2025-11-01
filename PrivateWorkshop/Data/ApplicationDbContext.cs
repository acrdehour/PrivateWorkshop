using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Models;
namespace PrivateWorkshop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Workshop> Workshops { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        
    }
}
