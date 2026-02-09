using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
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
            IsActive = p.IsActive
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
            IsActive = product.IsActive
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
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(product);

        return new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Model = created.Model,
            Size = created.Size,
            Capacity = created.Capacity,
            Description = created.Description,
            IsActive = created.IsActive
        };
    }

    public async Task UpdateProductAsync(ProductDto dto)
    {
        var product = await _repository.GetByIdAsync(dto.Id);
        if (product != null)
        {
            product.Name = dto.Name;
            product.Model = dto.Model;
            product.Size = dto.Size;
            product.Capacity = dto.Capacity;
            product.Description = dto.Description;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(product);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
