using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class SiteRepository : ISiteRepository
{
    private readonly TinacoProDbContext _context;

    public SiteRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Site>> GetAllAsync()
    {
        return await _context.Sites.ToListAsync();
    }

    public async Task<Site?> GetByIdAsync(int id)
    {
        return await _context.Sites.FindAsync(id);
    }

    public async Task<Site> AddAsync(Site site)
    {
        _context.Sites.Add(site);
        await _context.SaveChangesAsync();
        return site;
    }

    public async Task UpdateAsync(Site site)
    {
        _context.Sites.Update(site);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var site = await _context.Sites.FindAsync(id);
        if (site != null)
        {
            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
        }
    }
}
