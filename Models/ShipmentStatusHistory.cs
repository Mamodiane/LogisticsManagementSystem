using LogisticsManagementSystem.Enums;

namespace LogisticsManagementSystem.Models
{
    public class ShipmentStatusHistory
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public ShipmentStatus Status { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public Shipment? Shipment { get; set; }
    }
}