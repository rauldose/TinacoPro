using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface ISiteRepository
{
    Task<IEnumerable<Site>> GetAllAsync();
    Task<Site?> GetByIdAsync(int id);
    Task<Site> AddAsync(Site site);
    Task UpdateAsync(Site site);
    Task DeleteAsync(int id);
}
