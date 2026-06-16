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
        Task MarkShipmentAsCollectedAsync(int driverId, int shipmentId);
        Task MarkShipmentAsInTransitAsync(int driverId, int shipmentId);
        Task MarkShipmentAsDeliveredAsync(int driverId, int shipmentId);
    }
}