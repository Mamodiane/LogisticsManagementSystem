namespace LogisticsManagementSystem.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalShipments { get; set; }
        public int Pending { get; set; }
        public int Assigned { get; set; }
        public int Collected { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
        public int FailedDelivery { get; set; }
        public int Returned { get; set; }
        public int AvailableDrivers { get; set; }
    }
}