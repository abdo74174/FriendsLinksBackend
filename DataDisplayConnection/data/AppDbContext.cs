using DataDisplayConnection.models;
using Microsoft.EntityFrameworkCore;

namespace DataDisplayConnection.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProfileClass> Profiles { get; set; }
    }
}
