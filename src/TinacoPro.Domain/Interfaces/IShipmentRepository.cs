using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IShipmentRepository
{
    Task<IEnumerable<Shipment>> GetAllAsync();
    Task<Shipment?> GetByIdAsync(int id);
    Task<IEnumerable<Shipment>> GetByStatusAsync(ShipmentStatus status);
    Task<IEnumerable<Shipment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Shipment> CreateAsync(Shipment shipment);
    Task UpdateAsync(Shipment shipment);
    Task DeleteAsync(int id);
}
