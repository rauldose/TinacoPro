using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IRawMaterialRepository
{
    Task<IEnumerable<RawMaterial>> GetAllAsync();
    Task<RawMaterial?> GetByIdAsync(int id);
    Task<RawMaterial> AddAsync(RawMaterial material);
    Task UpdateAsync(RawMaterial material);
    Task DeleteAsync(int id);
}
