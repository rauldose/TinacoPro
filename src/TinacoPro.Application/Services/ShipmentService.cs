using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IFinishedGoodRepository _finishedGoodRepository;
    private readonly IProductRepository _productRepository;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IFinishedGoodRepository finishedGoodRepository,
        IProductRepository productRepository)
    {
        _shipmentRepository = shipmentRepository;
        _finishedGoodRepository = finishedGoodRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ShipmentDto>> GetAllShipmentsAsync()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        return shipments.Select(MapToDto);
    }

    public async Task<ShipmentDto?> GetShipmentByIdAsync(int id)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        return shipment != null ? MapToDto(shipment) : null;
    }

    public async Task<IEnumerable<ShipmentDto>> GetShipmentsByStatusAsync(ShipmentStatus status)
    {
        var shipments = await _shipmentRepository.GetByStatusAsync(status);
        return shipments.Select(MapToDto);
    }

    public async Task<IEnumerable<ShipmentDto>> GetShipmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var shipments = await _shipmentRepository.GetByDateRangeAsync(startDate, endDate);
        return shipments.Select(MapToDto);
    }

    public async Task<ShipmentDto> CreateShipmentAsync(CreateShipmentDto dto)
    {
        // Validate product exists
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {dto.ProductId} not found");

        // Generate shipment number
        var shipmentNumber = await GenerateShipmentNumberAsync();

        // Create shipment entity
        var shipment = new Shipment
        {
            ShipmentNumber = shipmentNumber,
            ProductId = dto.ProductId,
            FinishedGoodId = dto.FinishedGoodId,
            Quantity = dto.Quantity,
            CustomerName = dto.CustomerName,
            CustomerContact = dto.CustomerContact,
            DestinationAddress = dto.DestinationAddress,
            DestinationCity = dto.DestinationCity,
            DestinationZone = dto.DestinationZone,
            ShipmentDate = dto.ShipmentDate,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Status = ShipmentStatus.Pending,
            Notes = dto.Notes
        };

        // Deplete finished goods stock
        if (dto.FinishedGoodId.HasValue)
        {
            // Specific batch selected — deplete from that batch
            await DepleteFinishedGoodsStockAsync(dto.FinishedGoodId.Value, dto.Quantity);
        }
        else
        {
            // No specific batch selected — auto-deplete from available batches for this product (FIFO)
            var assignedBatchId = await AutoDepleteFinishedGoodsAsync(dto.ProductId, dto.Quantity);
            if (assignedBatchId.HasValue)
            {
                shipment.FinishedGoodId = assignedBatchId.Value;
            }
        }

        var createdShipment = await _shipmentRepository.CreateAsync(shipment);
        
        // Reload with navigation properties
        var result = await _shipmentRepository.GetByIdAsync(createdShipment.Id);
        return MapToDto(result!);
    }

    public async Task<ShipmentDto> UpdateShipmentAsync(int id, CreateShipmentDto dto)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment with ID {id} not found");

        // If shipment is already delivered or cancelled, don't allow updates
        if (shipment.Status == ShipmentStatus.Delivered || shipment.Status == ShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Cannot update shipment with status {shipment.Status}");

        // Restore old stock if FinishedGoodId changed
        if (shipment.FinishedGoodId.HasValue && shipment.FinishedGoodId != dto.FinishedGoodId)
        {
            await RestoreFinishedGoodsStockAsync(shipment.FinishedGoodId.Value, shipment.Quantity);
        }

        // Update shipment properties
        shipment.ProductId = dto.ProductId;
        shipment.FinishedGoodId = dto.FinishedGoodId;
        shipment.Quantity = dto.Quantity;
        shipment.CustomerName = dto.CustomerName;
        shipment.CustomerContact = dto.CustomerContact;
        shipment.DestinationAddress = dto.DestinationAddress;
        shipment.DestinationCity = dto.DestinationCity;
        shipment.DestinationZone = dto.DestinationZone;
        shipment.ShipmentDate = dto.ShipmentDate;
        shipment.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        shipment.Notes = dto.Notes;

        // Deplete new stock if FinishedGoodId specified
        if (dto.FinishedGoodId.HasValue && shipment.FinishedGoodId != dto.FinishedGoodId)
        {
            await DepleteFinishedGoodsStockAsync(dto.FinishedGoodId.Value, dto.Quantity);
        }

        await _shipmentRepository.UpdateAsync(shipment);
        
        var result = await _shipmentRepository.GetByIdAsync(id);
        return MapToDto(result!);
    }

    public async Task UpdateShipmentStatusAsync(int id, ShipmentStatus newStatus)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment with ID {id} not found");

        shipment.Status = newStatus;

        // Set actual delivery date when status changes to Delivered
        if (newStatus == ShipmentStatus.Delivered && !shipment.ActualDeliveryDate.HasValue)
        {
            shipment.ActualDeliveryDate = DateTime.UtcNow;
        }

        await _shipmentRepository.UpdateAsync(shipment);
    }

    public async Task CancelShipmentAsync(int id)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment with ID {id} not found");

        if (shipment.Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered shipment");

        if (shipment.Status == ShipmentStatus.Cancelled)
            throw new InvalidOperationException("Shipment is already cancelled");

        // Restore finished goods stock if it was depleted
        if (shipment.FinishedGoodId.HasValue)
        {
            await RestoreFinishedGoodsStockAsync(shipment.FinishedGoodId.Value, shipment.Quantity);
        }

        shipment.Status = ShipmentStatus.Cancelled;
        await _shipmentRepository.UpdateAsync(shipment);
    }

    /// <summary>
    /// Auto-dispatches pending shipments whose shipment date has arrived or passed.
    /// Returns the count of shipments that were auto-transitioned to InTransit.
    /// </summary>
    public async Task<int> AutoDispatchPendingShipmentsAsync()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        var pendingReady = shipments
            .Where(s => s.Status == ShipmentStatus.Pending && s.ShipmentDate.Date <= DateTime.UtcNow.Date)
            .ToList();

        foreach (var shipment in pendingReady)
        {
            shipment.Status = ShipmentStatus.InTransit;
            shipment.UpdatedAt = DateTime.UtcNow;
            await _shipmentRepository.UpdateAsync(shipment);
        }

        return pendingReady.Count;
    }

    /// <summary>
    /// Auto-marks InTransit shipments as Delivered when their expected delivery date has passed.
    /// Returns the count of shipments that were auto-delivered.
    /// </summary>
    public async Task<int> AutoDeliverShipmentsAsync()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        var inTransitReady = shipments
            .Where(s => s.Status == ShipmentStatus.InTransit 
                && s.ExpectedDeliveryDate.HasValue 
                && s.ExpectedDeliveryDate.Value.Date <= DateTime.UtcNow.Date)
            .ToList();

        foreach (var shipment in inTransitReady)
        {
            shipment.Status = ShipmentStatus.Delivered;
            shipment.ActualDeliveryDate = DateTime.UtcNow;
            shipment.UpdatedAt = DateTime.UtcNow;
            await _shipmentRepository.UpdateAsync(shipment);
        }

        return inTransitReady.Count;
    }

    private async Task<string> GenerateShipmentNumberAsync()
    {
        var allShipments = await _shipmentRepository.GetAllAsync();
        var count = allShipments.Count() + 1;
        return $"SHIP-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
    }

    private async Task DepleteFinishedGoodsStockAsync(int finishedGoodId, decimal quantity)
    {
        var finishedGood = await _finishedGoodRepository.GetByIdAsync(finishedGoodId);
        if (finishedGood == null)
            throw new InvalidOperationException($"Finished good with ID {finishedGoodId} not found");

        if (finishedGood.Quantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {finishedGood.Quantity}, Required: {quantity}");

        finishedGood.Quantity -= quantity;
        await _finishedGoodRepository.UpdateAsync(finishedGood);
    }

    /// <summary>
    /// Auto-depletes finished goods stock for a product when no specific batch is selected.
    /// Uses FIFO (oldest production date first) and can span multiple batches if needed.
    /// Returns the ID of the primary batch used, or null if no stock was available.
    /// </summary>
    private async Task<int?> AutoDepleteFinishedGoodsAsync(int productId, decimal quantity)
    {
        var batches = await _finishedGoodRepository.GetByProductIdAsync(productId);
        var availableBatches = batches
            .Where(fg => fg.Quantity > 0)
            .OrderBy(fg => fg.ProductionDate)
            .ToList();

        if (!availableBatches.Any())
            return null;

        var totalAvailable = availableBatches.Sum(fg => fg.Quantity);
        if (totalAvailable < quantity)
            throw new InvalidOperationException($"Insufficient finished goods stock for product ID {productId}. Available: {totalAvailable}, Required: {quantity}");

        int? primaryBatchId = null;
        var remaining = quantity;

        foreach (var batch in availableBatches)
        {
            if (remaining <= 0) break;

            primaryBatchId ??= batch.Id;

            var depleteAmount = Math.Min(batch.Quantity, remaining);
            batch.Quantity -= depleteAmount;
            remaining -= depleteAmount;
            await _finishedGoodRepository.UpdateAsync(batch);
        }

        return primaryBatchId;
    }

    private async Task RestoreFinishedGoodsStockAsync(int finishedGoodId, decimal quantity)
    {
        var finishedGood = await _finishedGoodRepository.GetByIdAsync(finishedGoodId);
        if (finishedGood == null)
            throw new InvalidOperationException($"Finished good with ID {finishedGoodId} not found");

        finishedGood.Quantity += quantity;
        await _finishedGoodRepository.UpdateAsync(finishedGood);
    }

    private ShipmentDto MapToDto(Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            ProductId = shipment.ProductId,
            ProductName = shipment.Product?.Name ?? string.Empty,
            FinishedGoodId = shipment.FinishedGoodId,
            Quantity = shipment.Quantity,
            CustomerName = shipment.CustomerName,
            CustomerContact = shipment.CustomerContact,
            DestinationAddress = shipment.DestinationAddress,
            DestinationCity = shipment.DestinationCity,
            DestinationZone = shipment.DestinationZone,
            ShipmentDate = shipment.ShipmentDate,
            ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
            ActualDeliveryDate = shipment.ActualDeliveryDate,
            Status = shipment.Status.ToString(),
            Notes = shipment.Notes,
            CreatedAt = shipment.CreatedAt
        };
    }
}
