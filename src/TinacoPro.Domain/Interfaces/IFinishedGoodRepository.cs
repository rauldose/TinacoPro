using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IFinishedGoodRepository
{
    Task<IEnumerable<FinishedGood>> GetAllAsync();
    Task<FinishedGood?> GetByIdAsync(int id);
    Task<IEnumerable<FinishedGood>> GetByProductIdAsync(int productId);
    Task<FinishedGood> AddAsync(FinishedGood finishedGood);
    Task UpdateAsync(FinishedGood finishedGood);
    Task DeleteAsync(int id);
}
