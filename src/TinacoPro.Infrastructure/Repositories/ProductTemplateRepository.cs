using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;

namespace TinacoPro.Infrastructure.Repositories;

public class ProductTemplateRepository : IProductTemplateRepository
{
    private readonly TinacoProDbContext _context;

    public ProductTemplateRepository(TinacoProDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductTemplate>> GetAllAsync()
    {
        return await _context.ProductTemplates
            .Include(t => t.Parts)
            .Include(t => t.Products)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<ProductTemplate?> GetByIdAsync(int id)
    {
        return await _context.ProductTemplates
            .Include(t => t.Parts)
            .Include(t => t.Products)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ProductTemplate?> GetByIdWithPartsAsync(int id)
    {
        return await _context.ProductTemplates
            .Include(t => t.Parts)
                .ThenInclude(p => p.Children)
            .Include(t => t.Parts)
                .ThenInclude(p => p.RawMaterial)
            .Include(t => t.Products)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ProductTemplate> CreateAsync(ProductTemplate template)
    {
        _context.ProductTemplates.Add(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task UpdateAsync(ProductTemplate template)
    {
        _context.ProductTemplates.Update(template);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var template = await _context.ProductTemplates.FindAsync(id);
        if (template != null)
        {
            _context.ProductTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TemplatePart>> GetRootPartsAsync(int templateId)
    {
        return await _context.TemplateParts
            .Include(p => p.Children)
            .Include(p => p.RawMaterial)
            .Where(p => p.TemplateId == templateId && p.ParentPartId == null)
            .OrderBy(p => p.Position)
            .ToListAsync();
    }

    public async Task<IEnumerable<TemplatePart>> GetChildPartsAsync(int parentPartId)
    {
        return await _context.TemplateParts
            .Include(p => p.Children)
            .Include(p => p.RawMaterial)
            .Where(p => p.ParentPartId == parentPartId)
            .OrderBy(p => p.Position)
            .ToListAsync();
    }

    public async Task<TemplatePart?> GetPartByIdAsync(int partId)
    {
        return await _context.TemplateParts
            .Include(p => p.ParentPart)
            .Include(p => p.Children)
            .Include(p => p.RawMaterial)
            .FirstOrDefaultAsync(p => p.Id == partId);
    }

    public async Task<TemplatePart> AddPartAsync(TemplatePart part)
    {
        _context.TemplateParts.Add(part);
        await _context.SaveChangesAsync();
        return part;
    }

    public async Task UpdatePartAsync(TemplatePart part)
    {
        _context.TemplateParts.Update(part);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePartAsync(int partId)
    {
        var part = await _context.TemplateParts
            .Include(p => p.Children)
            .FirstOrDefaultAsync(p => p.Id == partId);
        
        if (part != null)
        {
            // Delete children recursively
            foreach (var child in part.Children.ToList())
            {
                await DeletePartAsync(child.Id);
            }
            
            _context.TemplateParts.Remove(part);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> CalculateTemplateCostAsync(int templateId)
    {
        var rootParts = await GetRootPartsAsync(templateId);
        decimal totalCost = 0;

        foreach (var part in rootParts)
        {
            totalCost += await CalculatePartCostRecursiveAsync(part);
        }

        return totalCost;
    }

    private async Task<decimal> CalculatePartCostRecursiveAsync(TemplatePart part)
    {
        decimal partCost = (part.UnitCost * part.Quantity) + part.LaborCost;

        if (part.Children != null && part.Children.Any())
        {
            foreach (var child in part.Children)
            {
                var fullChild = await GetPartByIdAsync(child.Id);
                if (fullChild != null)
                {
                    partCost += await CalculatePartCostRecursiveAsync(fullChild);
                }
            }
        }

        return partCost;
    }
}
