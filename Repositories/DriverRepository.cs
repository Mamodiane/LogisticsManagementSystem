using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.DTOs;
using LogisticsManagementSystem.Enums;
using LogisticsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using LogisticsManagementSystem.Enums;

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

        public async Task<IEnumerable<Shipment>> GetAssignedShipmentsAsync(int driverId)
        {
            return await _context.Shipments
                .Where(s => s.DriverId == driverId)
                .ToListAsync();
        }

        public async Task MarkShipmentAsCollectedAsync(int driverId, int shipmentId)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.Assigned)
            {
                throw new InvalidOperationException("Shipment can only be collected when status is Assigned.");
            }

            shipment.Status = ShipmentStatus.Collected;

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Driver {driverId} marked shipment as Collected"
            );

            await _context.SaveChangesAsync();
        }

        public async Task MarkShipmentAsInTransitAsync(int driverId, int shipmentId)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.Collected)
            {
                throw new InvalidOperationException("Shipment can only move to InTransit after Collected.");
            }

            shipment.Status = ShipmentStatus.InTransit;

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Driver {driverId} marked shipment as InTransit"
            );

            await _context.SaveChangesAsync();
        }

        public async Task MarkShipmentAsDeliveredAsync(int driverId, int shipmentId)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.InTransit)
            {
                throw new InvalidOperationException("Shipment can only be delivered after InTransit.");
            }

            shipment.Status = ShipmentStatus.Delivered;

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Driver {driverId} marked shipment as Delivered"
            );

            await _context.SaveChangesAsync();
        }

        private async Task AddStatusHistoryAsync(int shipmentId, ShipmentStatus status, string notes)
        {
            var history = new ShipmentStatusHistory
            {
                ShipmentId = shipmentId,
                Status = status,
                Notes = notes,
                ChangedAt = DateTime.UtcNow
            };

            await _context.ShipmentStatusHistories.AddAsync(history);
        }

        //
        public async Task MarkShipmentAsFailedDeliveryAsync(int driverId, int shipmentId, string reason)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.InTransit)
            {
                throw new InvalidOperationException("Shipment can only be marked as Failed Delivery when it is InTransit.");
            }

            shipment.Status = ShipmentStatus.FailedDelivery;

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Failed delivery: {reason}"
            );

            await _context.SaveChangesAsync();
        }

        //Return shipment
        public async Task MarkShipmentAsReturnedAsync(int driverId, int shipmentId)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.FailedDelivery)
            {
                throw new InvalidOperationException("Only failed deliveries can be returned.");
            }

            shipment.Status = ShipmentStatus.Returned;

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Driver {driverId} marked shipment as Returned"
            );

            await _context.SaveChangesAsync();
        }
        // Add delivery proof after marking as delivered
        public async Task AddDeliveryProofAsync(
    int driverId,
    int shipmentId,
    CreateDeliveryProofDto request)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == shipmentId && s.DriverId == driverId);

            if (shipment == null)
            {
                throw new KeyNotFoundException("Shipment not found or not assigned to this driver.");
            }

            if (shipment.Status != ShipmentStatus.Delivered)
            {
                throw new InvalidOperationException("Delivery proof can only be added after shipment is Delivered.");
            }

            var existingProof = await _context.DeliveryProofs
                .FirstOrDefaultAsync(p => p.ShipmentId == shipmentId);

            if (existingProof != null)
            {
                throw new InvalidOperationException("Delivery proof already exists for this shipment.");
            }

            var deliveryProof = new DeliveryProof
            {
                ShipmentId = shipment.Id,
                ReceiverName = request.ReceiverName,
                SignatureName = request.SignatureName,
                Notes = request.Notes,
                DeliveredAt = DateTime.UtcNow
            };

            await _context.DeliveryProofs.AddAsync(deliveryProof);

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                $"Delivery proof submitted. Received by {request.ReceiverName}"
            );

            await _context.SaveChangesAsync();
        }
        // Get driver by application user id
        public async Task<Driver?> GetDriverByApplicationUserIdAsync(int applicationUserId)
        {
            return await _context.Drivers
                .FirstOrDefaultAsync(d => d.ApplicationUserId == applicationUserId);
        }
        // Link a driver profile to an application user
        public async Task LinkDriverToUserAsync(int driverId, int applicationUserId)
        {
            var driver = await _context.Drivers.FindAsync(driverId);

            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found.");
            }

            var user = await _context.Users.FindAsync(applicationUserId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.Role != UserRole.Driver)
            {
                throw new InvalidOperationException("Only users with Driver role can be linked to a driver profile.");
            }

            driver.ApplicationUserId = applicationUserId;

            await _context.SaveChangesAsync();
        }
    }
}