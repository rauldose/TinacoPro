using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class FinishedGoodsService
{
    private readonly IFinishedGoodRepository _finishedGoodRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public FinishedGoodsService(
        IFinishedGoodRepository finishedGoodRepository,
        IProductRepository productRepository,
        IProductionOrderRepository orderRepository)
    {
        _finishedGoodRepository = finishedGoodRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<FinishedGoodsDto>> GetAllAsync()
    {
        var finishedGoods = await _finishedGoodRepository.GetAllAsync();
        return finishedGoods.Select(fg => new FinishedGoodsDto
        {
            Id = fg.Id,
            ProductId = fg.ProductId,
            ProductName = fg.Product?.Name ?? "Unknown",
            ProductionOrderId = fg.ProductionOrderId,
            OrderNumber = fg.ProductionOrder?.OrderNumber ?? "Unknown",
            TemplateId = fg.TemplateId,
            TemplateName = fg.Template?.Name,
            Quantity = fg.Quantity,
            CurrentStock = fg.CurrentStock,
            ProductionDate = fg.ProductionDate,
            BatchNumber = fg.BatchNumber,
            Notes = fg.Notes,
            ActualMaterialCost = fg.ActualMaterialCost,
            ActualLaborCost = fg.ActualLaborCost
        });
    }

    public async Task<FinishedGoodsDto?> GetByIdAsync(int id)
    {
        var finishedGood = await _finishedGoodRepository.GetByIdAsync(id);
        if (finishedGood == null) return null;

        return new FinishedGoodsDto
        {
            Id = finishedGood.Id,
            ProductId = finishedGood.ProductId,
            ProductName = finishedGood.Product?.Name ?? "Unknown",
            ProductionOrderId = finishedGood.ProductionOrderId,
            OrderNumber = finishedGood.ProductionOrder?.OrderNumber ?? "Unknown",
            TemplateId = finishedGood.TemplateId,
            TemplateName = finishedGood.Template?.Name,
            Quantity = finishedGood.Quantity,
            CurrentStock = finishedGood.CurrentStock,
            ProductionDate = finishedGood.ProductionDate,
            BatchNumber = finishedGood.BatchNumber,
            Notes = finishedGood.Notes,
            ActualMaterialCost = finishedGood.ActualMaterialCost,
            ActualLaborCost = finishedGood.ActualLaborCost
        };
    }

    public async Task<IEnumerable<FinishedGoodsDto>> GetByProductIdAsync(int productId)
    {
        var finishedGoods = await _finishedGoodRepository.GetByProductIdAsync(productId);
        return finishedGoods.Select(fg => new FinishedGoodsDto
        {
            Id = fg.Id,
            ProductId = fg.ProductId,
            ProductName = fg.Product?.Name ?? "Unknown",
            ProductionOrderId = fg.ProductionOrderId,
            OrderNumber = fg.ProductionOrder?.OrderNumber ?? "Unknown",
            TemplateId = fg.TemplateId,
            TemplateName = fg.Template?.Name,
            Quantity = fg.Quantity,
            CurrentStock = fg.CurrentStock,
            ProductionDate = fg.ProductionDate,
            BatchNumber = fg.BatchNumber,
            Notes = fg.Notes,
            ActualMaterialCost = fg.ActualMaterialCost,
            ActualLaborCost = fg.ActualLaborCost
        });
    }

    public async Task<FinishedGoodsDto> CreateAsync(CreateFinishedGoodsDto dto)
    {
        var finishedGood = new FinishedGood
        {
            ProductId = dto.ProductId,
            ProductionOrderId = dto.ProductionOrderId,
            Quantity = dto.Quantity,
            CurrentStock = dto.CurrentStock,
            ProductionDate = DateTime.UtcNow,
            BatchNumber = dto.BatchNumber,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _finishedGoodRepository.AddAsync(finishedGood);
        var product = await _productRepository.GetByIdAsync(created.ProductId);
        var order = await _orderRepository.GetByIdAsync(created.ProductionOrderId);

        return new FinishedGoodsDto
        {
            Id = created.Id,
            ProductId = created.ProductId,
            ProductName = product?.Name ?? "Unknown",
            ProductionOrderId = created.ProductionOrderId,
            OrderNumber = order?.OrderNumber ?? "Unknown",
            TemplateId = created.TemplateId,
            TemplateName = created.Template?.Name,
            Quantity = created.Quantity,
            CurrentStock = created.CurrentStock,
            ProductionDate = created.ProductionDate,
            BatchNumber = created.BatchNumber,
            Notes = created.Notes,
            ActualMaterialCost = created.ActualMaterialCost,
            ActualLaborCost = created.ActualLaborCost
        };
    }

    public async Task UpdateQuantityAsync(int id, decimal newStock)
    {
        var finishedGood = await _finishedGoodRepository.GetByIdAsync(id);
        if (finishedGood != null)
        {
            finishedGood.CurrentStock = newStock;
            finishedGood.UpdatedAt = DateTime.UtcNow;
            await _finishedGoodRepository.UpdateAsync(finishedGood);
        }
    }

    public async Task DeleteAsync(int id)
    {
        await _finishedGoodRepository.DeleteAsync(id);
    }
}
