using System.Text.Json.Serialization;

namespace LogisticsManagementSystem.Models
{
    public class DeliveryProof
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public string ReceiverName { get; set; } = string.Empty;

        public string SignatureName { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Shipment? Shipment { get; set; }
    }
}