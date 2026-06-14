using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.Enums;
using LogisticsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsManagementSystem.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _context;

        public ShipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shipment>> GetAllShipmentsAsync()
        {
            return await _context.Shipments.ToListAsync();
        }

        public async Task<Shipment?> GetShipmentByIdAsync(int id)
        {
            return await _context.Shipments.FindAsync(id);
        }

        public async Task<Shipment?> GetShipmentByTrackingNumberAsync(string trackingNumber)
        {
            trackingNumber = trackingNumber.Trim().ToUpper();

            return await _context.Shipments
                .FirstOrDefaultAsync(s => s.TrackingNumber.ToUpper() == trackingNumber);
        }

        public async Task AddShipmentAsync(Shipment shipment)
        {
            shipment.TrackingNumber = $"LMS-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            shipment.Status = ShipmentStatus.Pending;
            shipment.CreatedDate = DateTime.UtcNow;

            await _context.Shipments.AddAsync(shipment);
            await _context.SaveChangesAsync();

            await AddStatusHistoryAsync(
                shipment.Id,
                shipment.Status,
                "Shipment created"
            );

            await _context.SaveChangesAsync();
        }

        public async Task UpdateShipmentAsync(Shipment shipment)
        {
            var existingShipment = await _context.Shipments.FindAsync(shipment.Id);
            var oldStatus = existingShipment.Status;

            if (existingShipment == null)
            {
                throw new KeyNotFoundException($"Shipment with id {shipment.Id} was not found.");
            }

            existingShipment.SenderName = shipment.SenderName;
            existingShipment.SenderPhone = shipment.SenderPhone;
            existingShipment.ReceiverName = shipment.ReceiverName;
            existingShipment.ReceiverPhone = shipment.ReceiverPhone;
            existingShipment.PickupAddress = shipment.PickupAddress;
            existingShipment.DeliveryAddress = shipment.DeliveryAddress;
            existingShipment.Weight = shipment.Weight;
            existingShipment.Type = shipment.Type;
            existingShipment.Priority = shipment.Priority;
            existingShipment.Status = shipment.Status;
            existingShipment.DriverId = shipment.DriverId;

            if (oldStatus != shipment.Status)
            {
                await AddStatusHistoryAsync(
                    existingShipment.Id,
                    shipment.Status,
                    $"Status changed from {oldStatus} to {shipment.Status}"
                );
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteShipmentAsync(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with id {id} was not found.");
            }

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
        }


        private async Task AddStatusHistoryAsync(
        int shipmentId,
        ShipmentStatus status,
        string notes)
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
    }
}