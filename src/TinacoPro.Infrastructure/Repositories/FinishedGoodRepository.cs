using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class FinishedGoodRepository : IFinishedGoodRepository
{
    private readonly TinacoProDbContext _context;

    public FinishedGoodRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FinishedGood>> GetAllAsync()
    {
        return await _context.FinishedGoods
            .Include(fg => fg.Product)
            .Include(fg => fg.ProductionOrder)
            .OrderByDescending(fg => fg.ProductionDate)
            .ToListAsync();
    }

    public async Task<FinishedGood?> GetByIdAsync(int id)
    {
        return await _context.FinishedGoods
            .Include(fg => fg.Product)
            .Include(fg => fg.ProductionOrder)
            .FirstOrDefaultAsync(fg => fg.Id == id);
    }

    public async Task<IEnumerable<FinishedGood>> GetByProductIdAsync(int productId)
    {
        return await _context.FinishedGoods
            .Include(fg => fg.Product)
            .Include(fg => fg.ProductionOrder)
            .Where(fg => fg.ProductId == productId)
            .OrderByDescending(fg => fg.ProductionDate)
            .ToListAsync();
    }

    public async Task<FinishedGood> AddAsync(FinishedGood finishedGood)
    {
        _context.FinishedGoods.Add(finishedGood);
        await _context.SaveChangesAsync();
        return finishedGood;
    }

    public async Task UpdateAsync(FinishedGood finishedGood)
    {
        _context.FinishedGoods.Update(finishedGood);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var finishedGood = await _context.FinishedGoods.FindAsync(id);
        if (finishedGood != null)
        {
            _context.FinishedGoods.Remove(finishedGood);
            await _context.SaveChangesAsync();
        }
    }
}
