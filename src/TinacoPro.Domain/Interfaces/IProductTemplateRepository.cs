using TinacoPro.Domain.Entities;

namespace TinacoPro.Domain.Interfaces;

public interface IProductTemplateRepository
{
    Task<IEnumerable<ProductTemplate>> GetAllAsync();
    Task<ProductTemplate?> GetByIdAsync(int id);
    Task<ProductTemplate?> GetByIdWithPartsAsync(int id);
    Task<ProductTemplate> CreateAsync(ProductTemplate template);
    Task UpdateAsync(ProductTemplate template);
    Task DeleteAsync(int id);
    Task<IEnumerable<TemplatePart>> GetRootPartsAsync(int templateId);
    Task<IEnumerable<TemplatePart>> GetChildPartsAsync(int parentPartId);
    Task<TemplatePart?> GetPartByIdAsync(int partId);
    Task<TemplatePart> AddPartAsync(TemplatePart part);
    Task UpdatePartAsync(TemplatePart part);
    Task DeletePartAsync(int partId);
    Task<decimal> CalculateTemplateCostAsync(int templateId);
}
