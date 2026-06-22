using LogisticsManagementSystem.Enums;

namespace LogisticsManagementSystem.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}