using LogisticsManagementSystem.Models;

namespace LogisticsManagementSystem.Repositories
{
    public interface IDriverRepository
    {
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByIdAsync(int id);
        Task AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task DeleteDriverAsync(int id);

        Task<IEnumerable<Shipment>> GetAssignedShipmentsAsync(int driverId);
        // Mark shipment as collected, in transit, delivered
        Task MarkShipmentAsCollectedAsync(int driverId, int shipmentId);
        // Mark shipment as in transit
        Task MarkShipmentAsInTransitAsync(int driverId, int shipmentId);
        // Mark shipment as delivered
        Task MarkShipmentAsDeliveredAsync(int driverId, int shipmentId);

        // Mark shipment as failed delivery
        Task MarkShipmentAsFailedDeliveryAsync(int driverId, int shipmentId, string reason);

        // Mark shipment as returned
        Task MarkShipmentAsReturnedAsync(int driverId, int shipmentId);
    }
}