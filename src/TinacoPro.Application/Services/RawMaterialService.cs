using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class RawMaterialService
{
    private readonly IRawMaterialRepository _repository;

    public RawMaterialService(IRawMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RawMaterialDto>> GetAllMaterialsAsync()
    {
        var materials = await _repository.GetAllAsync();
        return materials.Select(m => new RawMaterialDto
        {
            Id = m.Id,
            Name = m.Name,
            Code = m.Code,
            Unit = m.Unit,
            Category = m.Category.ToString(),
            CurrentStock = m.CurrentStock,
            MinimumStock = m.MinimumStock,
            UnitCost = m.UnitCost,
            IsActive = m.IsActive
        });
    }

    public async Task<RawMaterialDto?> GetMaterialByIdAsync(int id)
    {
        var material = await _repository.GetByIdAsync(id);
        if (material == null) return null;

        return new RawMaterialDto
        {
            Id = material.Id,
            Name = material.Name,
            Code = material.Code,
            Unit = material.Unit,
            Category = material.Category.ToString(),
            CurrentStock = material.CurrentStock,
            MinimumStock = material.MinimumStock,
            UnitCost = material.UnitCost,
            IsActive = material.IsActive
        };
    }

    public async Task<RawMaterialDto> CreateMaterialAsync(CreateRawMaterialDto dto)
    {
        var material = new RawMaterial
        {
            Name = dto.Name,
            Code = dto.Code,
            Unit = dto.Unit,
            CurrentStock = 0,
            MinimumStock = dto.MinimumStock,
            UnitCost = dto.UnitCost,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(material);

        return new RawMaterialDto
        {
            Id = created.Id,
            Name = created.Name,
            Code = created.Code,
            Unit = created.Unit,
            Category = created.Category.ToString(),
            CurrentStock = created.CurrentStock,
            MinimumStock = created.MinimumStock,
            UnitCost = created.UnitCost,
            IsActive = created.IsActive
        };
    }

    public async Task UpdateMaterialAsync(RawMaterialDto dto)
    {
        var material = await _repository.GetByIdAsync(dto.Id);
        if (material != null)
        {
            material.Name = dto.Name;
            material.Code = dto.Code;
            material.Unit = dto.Unit;
            material.MinimumStock = dto.MinimumStock;
            material.UnitCost = dto.UnitCost;
            material.IsActive = dto.IsActive;
            material.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(material);
        }
    }

    public async Task DeleteMaterialAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
