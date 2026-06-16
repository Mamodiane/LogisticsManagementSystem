using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsManagementSystem.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver?> GetDriverByIdAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        public async Task AddDriverAsync(Driver driver)
        {
            await _context.Drivers.AddAsync(driver);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            var existingDriver = await _context.Drivers.FindAsync(driver.Id);

            if (existingDriver == null)
            {
                throw new KeyNotFoundException($"Driver with id {driver.Id} was not found.");
            }

            existingDriver.FullName = driver.FullName;
            existingDriver.PhoneNumber = driver.PhoneNumber;
            existingDriver.LicenseNumber = driver.LicenseNumber;
            existingDriver.IsAvailable = driver.IsAvailable;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver with id {id} was not found.");
            }

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
}