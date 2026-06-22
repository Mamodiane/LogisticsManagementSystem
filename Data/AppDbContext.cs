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
        public DbSet<Driver> Drivers { get; set; }
        //
        public DbSet<AddressChangeRequest> AddressChangeRequests { get; set; }

        public DbSet<DeliveryProof> DeliveryProofs { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}