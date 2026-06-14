using LogisticsManagementSystem.Models;

namespace LogisticsManagementSystem.Repositories
{
    public interface IShipmentRepository
    {
        Task<IEnumerable<Shipment>> GetAllShipmentsAsync();
        Task<Shipment?> GetShipmentByIdAsync(int id);
        Task<Shipment?> GetShipmentByTrackingNumberAsync(string trackingNumber);
        Task AddShipmentAsync(Shipment shipment);
        Task UpdateShipmentAsync(Shipment shipment);
        Task DeleteShipmentAsync(int id);
    }
}