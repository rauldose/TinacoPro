using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class ProductionOrderRepository : IProductionOrderRepository
{
    private readonly TinacoProDbContext _context;

    public ProductionOrderRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductionOrder>> GetAllAsync()
    {
        return await _context.ProductionOrders
            .Include(o => o.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<ProductionOrder?> GetByIdAsync(int id)
    {
        return await _context.ProductionOrders
            .Include(o => o.Product)
            .ThenInclude(p => p.ProductMaterials)
            .ThenInclude(pm => pm.RawMaterial)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<ProductionOrder> AddAsync(ProductionOrder order)
    {
        _context.ProductionOrders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(ProductionOrder order)
    {
        _context.ProductionOrders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _context.ProductionOrders.FindAsync(id);
        if (order != null)
        {
            _context.ProductionOrders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
