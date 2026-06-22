using System.ComponentModel.DataAnnotations;
using LogisticsManagementSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticsManagementSystem.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public string SenderName { get; set; } = string.Empty;

        [Required]
        public string SenderPhone { get; set; } = string.Empty;

        [Required]
        public string ReceiverName { get; set; } = string.Empty;

        [Required]
        public string ReceiverPhone { get; set; } = string.Empty;

        [Required]
        public string PickupAddress { get; set; } = string.Empty;

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }

        public ShipmentType Type { get; set; } = ShipmentType.Parcel;

        public DeliveryPriority Priority { get; set; } = DeliveryPriority.Normal;

        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

        public int? DriverId { get; set; }

        public Driver? Driver { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<ShipmentStatusHistory> StatusHistory { get; set; }
            = new List<ShipmentStatusHistory>();

        public ICollection<AddressChangeRequest> AddressChangeRequests { get; set; }
    = new List<AddressChangeRequest>();
        public DeliveryProof? DeliveryProof { get; set; }
    }
}