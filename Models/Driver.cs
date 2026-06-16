using System.Text.Json.Serialization;

namespace LogisticsManagementSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [JsonIgnore]
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}