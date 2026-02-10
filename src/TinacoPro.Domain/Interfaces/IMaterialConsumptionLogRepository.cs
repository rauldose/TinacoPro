using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IMaterialConsumptionLogRepository
{
    Task<MaterialConsumptionLog> AddAsync(MaterialConsumptionLog log);
    Task<IEnumerable<MaterialConsumptionLog>> GetByProductionOrderIdAsync(int productionOrderId);
    Task<IEnumerable<MaterialConsumptionLog>> GetByMaterialIdAsync(int materialId);
}
