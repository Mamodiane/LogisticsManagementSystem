using LogisticsManagementSystem.Enums;
using System.Text.Json.Serialization;

namespace LogisticsManagementSystem.Models
{
    public class AddressChangeRequest
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        public string CurrentDeliveryAddress { get; set; } = string.Empty;

        public string RequestedDeliveryAddress { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public AddressChangeRequestStatus Status { get; set; }
            = AddressChangeRequestStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public string ReviewNotes { get; set; } = string.Empty;

        [JsonIgnore]
        public Shipment? Shipment { get; set; }
    }
}