using Microsoft.EntityFrameworkCore;
using WebSpaceManager.Entities;

namespace WebSpaceManager.DbAccess
{
    public class SpaceManagerDb : DbContext
    {
        public DbSet<Space> Spaces { get; set; }

        public DbSet<Equipment> Equipment { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public SpaceManagerDb(DbContextOptions options) : base(options) {}
    }
}
