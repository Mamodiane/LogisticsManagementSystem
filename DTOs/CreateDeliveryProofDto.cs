namespace LogisticsManagementSystem.DTOs
{
    public class CreateDeliveryProofDto
    {
        public string ReceiverName { get; set; } = string.Empty;

        public string SignatureName { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}