using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ProductTemplateService
{
    private readonly IProductTemplateRepository _templateRepository;
    private readonly IRawMaterialRepository _materialRepository;

    public ProductTemplateService(
        IProductTemplateRepository templateRepository,
        IRawMaterialRepository materialRepository)
    {
        _templateRepository = templateRepository;
        _materialRepository = materialRepository;
    }

    public async Task<IEnumerable<ProductTemplateDto>> GetAllTemplatesAsync()
    {
        var templates = await _templateRepository.GetAllAsync();
        return templates.Select(MapToDto);
    }

    public async Task<ProductTemplateDto?> GetTemplateByIdAsync(int id)
    {
        var template = await _templateRepository.GetByIdWithPartsAsync(id);
        return template != null ? MapToDto(template) : null;
    }

    public async Task<ProductTemplateDto> CreateTemplateAsync(ProductTemplateDto dto)
    {
        var template = new ProductTemplate
        {
            Name = dto.Name,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? string.Empty : dto.Description,
            ModelType = dto.ModelType,
            IsActive = dto.IsActive
        };

        var created = await _templateRepository.CreateAsync(template);
        return MapToDto(created);
    }

    public async Task UpdateTemplateAsync(ProductTemplateDto dto)
    {
        var template = await _templateRepository.GetByIdAsync(dto.Id);
        if (template == null) return;

        template.Name = dto.Name;
        template.Description = string.IsNullOrWhiteSpace(dto.Description) ? string.Empty : dto.Description;
        template.ModelType = dto.ModelType;
        template.IsActive = dto.IsActive;

        await _templateRepository.UpdateAsync(template);
    }

    public async Task DeleteTemplateAsync(int id)
    {
        await _templateRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TemplatePartDto>> GetRootPartsAsync(int templateId)
    {
        var parts = await _templateRepository.GetRootPartsAsync(templateId);
        return parts.Select(MapPartToDto);
    }

    public async Task<TemplatePartDto> AddPartAsync(TemplatePartDto dto)
    {
        var part = new TemplatePart
        {
            TemplateId = dto.ProductTemplateId,
            ParentPartId = dto.ParentPartId,
            Name = dto.Name,
            PartType = dto.PartType,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            UnitCost = dto.UnitCost,
            LaborCost = dto.LaborCost,
            EstimatedMinutes = dto.EstimatedMinutes,
            Position = dto.Position,
            RawMaterialId = dto.RawMaterialId
        };

        // If linked to raw material, get cost from material
        if (part.RawMaterialId.HasValue)
        {
            var material = await _materialRepository.GetByIdAsync(part.RawMaterialId.Value);
            if (material != null)
            {
                part.UnitCost = material.UnitCost;
                part.Unit = material.Unit;
            }
        }

        var created = await _templateRepository.AddPartAsync(part);
        
        // Recalculate template costs
        await RecalculateTemplateCostsAsync(dto.ProductTemplateId);
        
        return MapPartToDto(created);
    }

    public async Task UpdatePartAsync(TemplatePartDto dto)
    {
        var part = await _templateRepository.GetPartByIdAsync(dto.Id);
        if (part == null) return;

        part.Name = dto.Name;
        part.PartType = dto.PartType;
        part.Quantity = dto.Quantity;
        part.Unit = dto.Unit;
        part.UnitCost = dto.UnitCost;
        part.LaborCost = dto.LaborCost;
        part.EstimatedMinutes = dto.EstimatedMinutes;
        part.Position = dto.Position;
        part.RawMaterialId = dto.RawMaterialId;

        await _templateRepository.UpdatePartAsync(part);
        
        // Recalculate template costs
        await RecalculateTemplateCostsAsync(part.TemplateId);
    }

    public async Task DeletePartAsync(int partId)
    {
        var part = await _templateRepository.GetPartByIdAsync(partId);
        if (part == null) return;

        var templateId = part.TemplateId;
        await _templateRepository.DeletePartAsync(partId);
        
        // Recalculate template costs
        await RecalculateTemplateCostsAsync(templateId);
    }

    public async Task<decimal> CalculateTemplateCostAsync(int templateId)
    {
        return await _templateRepository.CalculateTemplateCostAsync(templateId);
    }

    private async Task RecalculateTemplateCostsAsync(int templateId)
    {
        var template = await _templateRepository.GetByIdWithPartsAsync(templateId);
        if (template == null) return;

        decimal materialCost = 0;
        decimal laborCost = 0;
        int totalMinutes = 0;

        foreach (var part in template.Parts.Where(p => p.ParentPartId == null))
        {
            var costs = CalculatePartCostsRecursive(part);
            materialCost += costs.materialCost;
            laborCost += costs.laborCost;
            totalMinutes += costs.minutes;
        }

        template.TotalMaterialCost = materialCost;
        template.TotalLaborCost = laborCost;
        template.TotalEstimatedMinutes = totalMinutes;

        await _templateRepository.UpdateAsync(template);
    }

    private (decimal materialCost, decimal laborCost, int minutes) CalculatePartCostsRecursive(TemplatePart part)
    {
        decimal materialCost = part.PartType == "Material" ? part.UnitCost * part.Quantity : 0;
        decimal laborCost = part.LaborCost;
        int minutes = part.EstimatedMinutes;

        if (part.Children != null && part.Children.Any())
        {
            foreach (var child in part.Children)
            {
                var childCosts = CalculatePartCostsRecursive(child);
                materialCost += childCosts.materialCost;
                laborCost += childCosts.laborCost;
                minutes += childCosts.minutes;
            }
        }

        return (materialCost, laborCost, minutes);
    }

    private ProductTemplateDto MapToDto(ProductTemplate template)
    {
        return new ProductTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            ModelType = template.ModelType,
            IsActive = template.IsActive,
            TotalMaterialCost = template.TotalMaterialCost,
            TotalLaborCost = template.TotalLaborCost,
            TotalEstimatedMinutes = template.TotalEstimatedMinutes,
            RootParts = template.Parts?
                .Where(p => p.ParentPartId == null)
                .OrderBy(p => p.Position)
                .Select(MapPartToDto)
                .ToList() ?? new List<TemplatePartDto>(),
            ProductCount = template.Products?.Count ?? 0
        };
    }

    private TemplatePartDto MapPartToDto(TemplatePart part)
    {
        return new TemplatePartDto
        {
            Id = part.Id,
            ProductTemplateId = part.TemplateId,
            ParentPartId = part.ParentPartId,
            Name = part.Name,
            PartType = part.PartType,
            Quantity = part.Quantity,
            Unit = part.Unit,
            UnitCost = part.UnitCost,
            LaborCost = part.LaborCost,
            EstimatedMinutes = part.EstimatedMinutes,
            Position = part.Position,
            RawMaterialId = part.RawMaterialId,
            RawMaterialName = part.RawMaterial?.Name,
            Children = part.Children?
                .OrderBy(c => c.Position)
                .Select(MapPartToDto)
                .ToList() ?? new List<TemplatePartDto>()
        };
    }
}
