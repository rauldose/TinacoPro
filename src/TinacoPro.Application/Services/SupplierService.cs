using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _repository.GetAllAsync();
        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            Name = s.Name,
            ContactName = s.ContactName,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            IsActive = s.IsActive
        });
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
    {
        var supplier = await _repository.GetByIdAsync(id);
        if (supplier == null) return null;

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactName = supplier.ContactName,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            IsActive = supplier.IsActive
        };
    }

    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(supplier);

        return new SupplierDto
        {
            Id = created.Id,
            Name = created.Name,
            ContactName = created.ContactName,
            Phone = created.Phone,
            Email = created.Email,
            Address = created.Address,
            IsActive = created.IsActive
        };
    }

    public async Task UpdateSupplierAsync(SupplierDto dto)
    {
        var supplier = await _repository.GetByIdAsync(dto.Id);
        if (supplier != null)
        {
            supplier.Name = dto.Name;
            supplier.ContactName = dto.ContactName;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;
            supplier.IsActive = dto.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(supplier);
        }
    }

    public async Task DeleteSupplierAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
