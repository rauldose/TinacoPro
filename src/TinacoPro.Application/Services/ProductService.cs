using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IProductTemplateRepository _templateRepository;

    public ProductService(
        IProductRepository repository,
        IProductTemplateRepository templateRepository)
    {
        _repository = repository;
        _templateRepository = templateRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Model = p.Model,
            Size = p.Size,
            Capacity = p.Capacity,
            Description = p.Description,
            IsActive = p.IsActive,
            TemplateId = p.TemplateId,
            TemplateName = p.Template?.Name,
            MaterialCost = p.MaterialCost,
            LaborCost = p.LaborCost
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Model = product.Model,
            Size = product.Size,
            Capacity = product.Capacity,
            Description = product.Description,
            IsActive = product.IsActive,
            TemplateId = product.TemplateId,
            TemplateName = product.Template?.Name,
            MaterialCost = product.MaterialCost,
            LaborCost = product.LaborCost
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Model = dto.Model,
            Size = dto.Size,
            Capacity = dto.Capacity,
            Description = dto.Description,
            TemplateId = dto.TemplateId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // If template is assigned, sync costs from template
        if (dto.TemplateId.HasValue)
        {
            await SyncCostsFromTemplate(product, dto.TemplateId.Value);
        }

        var created = await _repository.AddAsync(product);

        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Model = created.Model,
            Size = created.Size,
            Capacity = created.Capacity,
            Description = created.Description,
            IsActive = created.IsActive,
            TemplateId = created.TemplateId,
            TemplateName = created.Template?.Name,
            MaterialCost = created.MaterialCost,
            LaborCost = created.LaborCost
        };
    }

    public async Task UpdateProductAsync(ProductDto dto)
    {
        var product = await _repository.GetByIdAsync(dto.Id);
        if (product != null)
        {
            var previousTemplateId = product.TemplateId;
            
            product.Name = dto.Name;
            product.Model = dto.Model;
            product.Size = dto.Size;
            product.Capacity = dto.Capacity;
            product.Description = dto.Description;
            product.IsActive = dto.IsActive;
            product.TemplateId = dto.TemplateId;
            product.UpdatedAt = DateTime.UtcNow;

            // If template changed, sync costs from new template
            if (dto.TemplateId.HasValue && dto.TemplateId != previousTemplateId)
            {
                await SyncCostsFromTemplate(product, dto.TemplateId.Value);
            }

            await _repository.UpdateAsync(product);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    private async Task SyncCostsFromTemplate(Product product, int templateId)
    {
        var template = await _templateRepository.GetByIdWithPartsAsync(templateId);
        if (template != null)
        {
            product.MaterialCost = template.TotalMaterialCost;
            product.LaborCost = template.TotalLaborCost;
        }
    }

    /// <summary>
    /// Updates product costs when template costs change
    /// </summary>
    public async Task SyncProductCostsFromTemplate(int templateId)
    {
        var products = await _repository.GetAllAsync();
        var productsWithTemplate = products.Where(p => p.TemplateId == templateId);
        
        foreach (var product in productsWithTemplate)
        {
            await SyncCostsFromTemplate(product, templateId);
            await _repository.UpdateAsync(product);
        }
    }
}
