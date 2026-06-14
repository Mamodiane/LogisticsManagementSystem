using LogisticsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; set; }
    }
}