using ITSAssignment.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITSAssignment.Web
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Mumineen> Mumineen { get; set; }
    }
}
