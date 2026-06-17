namespace LogisticsManagementSystem.DTOs
{
    public class ShipmentTrackingDto
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<TrackingHistoryDto> History { get; set; } = new();
    }

    public class TrackingHistoryDto
    {
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}