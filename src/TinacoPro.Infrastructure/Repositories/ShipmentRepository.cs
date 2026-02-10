using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly TinacoProDbContext _context;

    public ShipmentRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return await _context.Shipments
            .Include(s => s.Product)
            .Include(s => s.FinishedGood)
            .OrderByDescending(s => s.ShipmentDate)
            .ToListAsync();
    }

    public async Task<Shipment?> GetByIdAsync(int id)
    {
        return await _context.Shipments
            .Include(s => s.Product)
            .Include(s => s.FinishedGood)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Shipment>> GetByStatusAsync(ShipmentStatus status)
    {
        return await _context.Shipments
            .Include(s => s.Product)
            .Include(s => s.FinishedGood)
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.ShipmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Shipments
            .Include(s => s.Product)
            .Include(s => s.FinishedGood)
            .Where(s => s.ShipmentDate >= startDate && s.ShipmentDate <= endDate)
            .OrderByDescending(s => s.ShipmentDate)
            .ToListAsync();
    }

    public async Task<Shipment> CreateAsync(Shipment shipment)
    {
        shipment.CreatedAt = DateTime.UtcNow;
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();
        return shipment;
    }

    public async Task UpdateAsync(Shipment shipment)
    {
        shipment.UpdatedAt = DateTime.UtcNow;
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment != null)
        {
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
        }
    }
}
