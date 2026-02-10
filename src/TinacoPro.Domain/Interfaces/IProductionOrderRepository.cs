using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IProductionOrderRepository
{
    Task<IEnumerable<ProductionOrder>> GetAllAsync();
    Task<ProductionOrder?> GetByIdAsync(int id);
    Task<ProductionOrder> AddAsync(ProductionOrder order);
    Task UpdateAsync(ProductionOrder order);
    Task DeleteAsync(int id);
}
