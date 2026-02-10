using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class RawMaterialRepository : IRawMaterialRepository
{
    private readonly TinacoProDbContext _context;

    public RawMaterialRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RawMaterial>> GetAllAsync()
    {
        return await _context.RawMaterials.ToListAsync();
    }

    public async Task<RawMaterial?> GetByIdAsync(int id)
    {
        return await _context.RawMaterials.FindAsync(id);
    }

    public async Task<RawMaterial> AddAsync(RawMaterial material)
    {
        _context.RawMaterials.Add(material);
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task UpdateAsync(RawMaterial material)
    {
        _context.RawMaterials.Update(material);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var material = await _context.RawMaterials.FindAsync(id);
        if (material != null)
        {
            _context.RawMaterials.Remove(material);
            await _context.SaveChangesAsync();
        }
    }
}
