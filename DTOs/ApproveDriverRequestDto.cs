namespace LogisticsManagementSystem.DTOs
{
    public class ApproveDriverRequestDto
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;
    }
}