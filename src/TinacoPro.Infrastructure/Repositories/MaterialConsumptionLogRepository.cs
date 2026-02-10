using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class MaterialConsumptionLogRepository : IMaterialConsumptionLogRepository
{
    private readonly TinacoProDbContext _context;

    public MaterialConsumptionLogRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<MaterialConsumptionLog> AddAsync(MaterialConsumptionLog log)
    {
        _context.MaterialConsumptionLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<MaterialConsumptionLog>> GetByProductionOrderIdAsync(int productionOrderId)
    {
        return await _context.MaterialConsumptionLogs
            .Include(l => l.RawMaterial)
            .Where(l => l.ProductionOrderId == productionOrderId)
            .OrderBy(l => l.ConsumedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaterialConsumptionLog>> GetByMaterialIdAsync(int materialId)
    {
        return await _context.MaterialConsumptionLogs
            .Include(l => l.ProductionOrder)
            .Where(l => l.RawMaterialId == materialId)
            .OrderByDescending(l => l.ConsumedAt)
            .ToListAsync();
    }
}
