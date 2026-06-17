namespace LogisticsManagementSystem.DTOs
{
    public class CreateAddressChangeRequestDto
    {
        public int ShipmentId { get; set; }

        public string RequestedDeliveryAddress { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
    }
}