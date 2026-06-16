using System.ComponentModel.DataAnnotations;

namespace LogisticsManagementSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}