# TinacoPro - Next Features for Future PRs

## Overview

This document outlines the remaining features and improvements planned for TinacoPro. These features are prioritized and ready for implementation in future pull requests.

---

## 🚀 Priority 1: Core Production Features

### 1.1 Template-Based Automatic Inventory Depletion

**Status:** Backend partially complete, needs integration

**Description:**
Automatically deplete raw material inventory when production orders complete, based on the product's template Bill of Materials (BOM).

**Technical Requirements:**
- Update `ProductionOrderService.CompleteOrderAsync()` method
- Load template and traverse part hierarchy when order completes
- Calculate material requirements: `TemplatePart.Quantity × ProductionOrder.Quantity`
- Validate sufficient stock before completion
- Deplete `RawMaterial.CurrentStock` for each material in template
- Handle insufficient stock errors gracefully
- Log material consumption for audit trail

**Implementation Steps:**
1. Add template loading to `CompleteOrderAsync()`
2. Implement recursive BOM traversal method
3. Add stock validation before depletion
4. Update stock levels atomically (transaction)
5. Add error handling for insufficient stock
6. Create material consumption log entries
7. Add unit tests for depletion logic

**Estimated Effort:** 2-3 hours

**Files to Modify:**
- `src/TinacoPro.Application/Services/ProductionOrderService.cs`
- `src/TinacoPro.Infrastructure/Repositories/ProductTemplateRepository.cs`
- `src/TinacoPro.Domain/Entities/MaterialConsumptionLog.cs` (new)

---

### 1.2 Finished Goods Inventory Management

**Status:** Entity exists, needs service and UI

**Description:**
Complete finished goods inventory system to track produced items ready for sale/shipment.

**Features:**
- View all finished goods with quantities
- Automatic creation when production order completes
- Manual adjustments (damage, returns, etc.)
- Stock levels and location tracking
- Integration with production completion
- Integration with shipping (stock depletion)

**Technical Requirements:**
- Create `FinishedGoodsService` in Application layer
- Create `FinishedGoodsDto` for data transfer
- Implement CRUD operations
- Create `FinishedGoods.razor` page
- Add to navigation menu
- Link to ProductionOrder completion
- Link to Shipment creation

**Implementation Steps:**
1. Create FinishedGoodsService with methods:
   - `GetAllAsync()`
   - `GetByIdAsync(id)`
   - `GetByProductIdAsync(productId)`
   - `CreateAsync(dto)`
   - `UpdateQuantityAsync(id, quantity)`
   - `DeleteAsync(id)`
2. Create FinishedGoodsDto
3. Create FinishedGoods.razor page with MudTable
4. Add New/Edit/Delete dialogs
5. Add to NavMenu
6. Update ProductionOrderService to create finished goods on completion
7. Test integration

**Estimated Effort:** 2-3 hours

**Files to Create:**
- `src/TinacoPro.Application/Services/FinishedGoodsService.cs`
- `src/TinacoPro.Application/DTOs/FinishedGoodsDto.cs`
- `src/TinacoPro.Web/Components/Pages/FinishedGoods.razor`

**Files to Modify:**
- `src/TinacoPro.Application/Services/ProductionOrderService.cs`
- `src/TinacoPro.Web/Components/Layout/NavMenu.razor`
- `src/TinacoPro.Web/Program.cs` (DI registration)

---

## 🌳 Priority 2: Template Builder Enhancements

### 2.1 TreeView with Context Menu

**Status:** Backend complete, UI needs TreeView implementation

**Description:**
Replace the current flat list of parts with an interactive hierarchical TreeView that shows the BOM structure visually.

**Features:**
- MudTreeView component showing parent-child relationships
- Expand/collapse nodes
- Visual indentation showing hierarchy depth
- Icons for different part types
- Right-click context menu on parts:
  - Add Child Part
  - Edit Part
  - Delete Part
  - Move Up/Down (reorder siblings)
- Drag-and-drop reordering (optional, future enhancement)

**Technical Requirements:**
- Implement MudTreeView in TemplateBuilder.razor
- Create recursive TreeItem component for parts
- Implement context menu with MudMenu
- Add part selection to show details
- Maintain tree state on operations
- Update part positions on reorder

**Implementation Steps:**
1. Replace current part list with MudTreeView
2. Create TreeItemData class for TreeView nodes
3. Implement BuildTree() method to convert flat parts to tree
4. Add context menu with MudMenu + @oncontextmenu
5. Implement context menu actions:
   - OpenAddChildDialog(parentPart)
   - OpenEditDialog(part)
   - DeletePart(part) with confirmation
   - MovePartUp(part)
   - MovePartDown(part)
6. Add visual styling for tree (icons, indentation)
7. Test tree operations

**Estimated Effort:** 3-4 hours

**Files to Modify:**
- `src/TinacoPro.Web/Components/Pages/TemplateBuilder.razor`

**UI Mock:**
```
Template: Tinaco 1100L
├─ 🏗️ Tank Body [Right-click menu]
│  ├─ 📦 PE-HD 15kg
│  ├─ 📦 Colorante 0.2kg
│  └─ 🔧 Rotomolding 45min
├─ 🏗️ Accessories
│  ├─ ⚙️ Float Valve
│  └─ ⚙️ Lid
└─ 🏗️ Packaging
```

---

### 2.2 Enhanced Material Selection

**Status:** Partially implemented, needs UX improvements

**Description:**
Improve the material selection process when adding Material-type parts to templates.

**Features:**
- Searchable material dropdown
- Show current stock in dropdown
- Show unit cost in dropdown
- Auto-populate quantity from material defaults
- Auto-populate unit cost from selected material
- Highlight low-stock materials
- Quick add new material button

**Implementation Steps:**
1. Enhance material dropdown with MudAutocomplete
2. Add material search/filter
3. Display stock levels in dropdown items
4. Auto-fill cost when material selected
5. Add visual indicators for low stock
6. Add "Quick Add Material" button in dialog

**Estimated Effort:** 1-2 hours

---

## 📦 Priority 3: Shipping and Delivery Tracking

### 3.1 Shipment Management System

**Status:** Not started

**Description:**
Complete shipment tracking system for finished goods deliveries to customers.

**Features:**
- Create shipments from finished goods
- Track shipment status (Pending, In Transit, Delivered, Cancelled)
- Customer information
- Destination/address
- Shipping date and expected delivery date
- Automatic finished goods stock depletion on shipment
- Shipment history
- Print shipping labels (future)

**Database Schema:**
```sql
Shipments
- Id (int, PK)
- ShipmentNumber (string, unique)
- ProductId (int, FK)
- FinishedGoodId (int, FK, nullable)
- Quantity (int)
- CustomerName (string)
- CustomerContact (string)
- DestinationAddress (string)
- DestinationCity (string)
- DestinationZone (string)
- ShipmentDate (DateTime)
- ExpectedDeliveryDate (DateTime, nullable)
- ActualDeliveryDate (DateTime, nullable)
- Status (enum: Pending, InTransit, Delivered, Cancelled)
- Notes (string, nullable)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
```

**Technical Requirements:**
- Create Shipment entity
- Create database migration
- Create ShipmentRepository and interface
- Create ShipmentService
- Create ShipmentDto
- Create Shippings.razor page
- Integrate with FinishedGoodsService for stock depletion

**Implementation Steps:**
1. Create Shipment entity in Domain layer
2. Add DbSet and configuration to DbContext
3. Create and apply migration
4. Create IShipmentRepository interface
5. Implement ShipmentRepository
6. Create ShipmentService with methods:
   - `GetAllShipmentsAsync()`
   - `GetShipmentByIdAsync(id)`
   - `CreateShipmentAsync(dto)` - depletes finished goods
   - `UpdateShipmentAsync(dto)`
   - `UpdateShipmentStatusAsync(id, status)`
   - `CancelShipmentAsync(id)` - restores finished goods
   - `GetShipmentsByStatusAsync(status)`
   - `GetShipmentsByDateRangeAsync(start, end)`
7. Create ShipmentDto
8. Create Shippings.razor page with:
   - MudTable showing all shipments
   - Status filters
   - Date range filter
   - New Shipment dialog
   - Edit Shipment dialog
   - Status update buttons
   - Customer search
9. Add to NavMenu
10. Register services in DI
11. Add seed data for testing
12. Test shipment creation and stock depletion

**Estimated Effort:** 3-4 hours

**Files to Create:**
- `src/TinacoPro.Domain/Entities/Shipment.cs`
- `src/TinacoPro.Domain/Interfaces/IShipmentRepository.cs`
- `src/TinacoPro.Infrastructure/Repositories/ShipmentRepository.cs`
- `src/TinacoPro.Application/Services/ShipmentService.cs`
- `src/TinacoPro.Application/DTOs/ShipmentDto.cs`
- `src/TinacoPro.Web/Components/Pages/Shippings.razor`

**Files to Modify:**
- `src/TinacoPro.Infrastructure/Data/TinacoProDbContext.cs`
- `src/TinacoPro.Application/Services/FinishedGoodsService.cs`
- `src/TinacoPro.Web/Components/Layout/NavMenu.razor`
- `src/TinacoPro.Web/Program.cs`

---

## 🎨 Priority 4: UI/UX Improvements

### 4.1 Complete Localization

**Status:** Partially implemented (English/Spanish)

**Description:**
Complete the localization system across all pages.

**Pages Needing Translation:**
- Products.razor (partial)
- RawMaterials.razor (partial)
- ProductionOrders.razor (partial)
- Suppliers.razor (partial)
- Reports.razor (partial)
- TemplateBuilder.razor (not started)
- FinishedGoods.razor (not created yet)
- Shippings.razor (not created yet)

**Estimated Effort:** 2-3 hours

---

### 4.2 Form Validation Enhancement

**Status:** Basic validation exists

**Description:**
Add comprehensive validation to all forms with helpful error messages.

**Features:**
- Required field validation
- Format validation (email, phone, etc.)
- Range validation (min/max values)
- Stock validation (prevent negative)
- Unique constraint validation
- Real-time validation feedback
- Custom validation messages in both languages

**Estimated Effort:** 2-3 hours

---

### 4.3 Dashboard Enhancements

**Status:** Basic KPIs implemented

**Description:**
Add more interactive visualizations to the dashboard.

**Features:**
- Production trend charts (daily/weekly/monthly)
- Inventory status charts
- Low stock alert panel
- Recent orders timeline
- Quick actions panel
- Refresh button with auto-refresh option
- Export dashboard data

**Technical Requirements:**
- Add charting library (e.g., ApexCharts.NET or MudBlazor Charts)
- Enhance DashboardService with time-series data
- Create chart components
- Add real-time updates (SignalR optional)

**Estimated Effort:** 3-4 hours

---

## 🔧 Priority 5: Technical Improvements

### 5.1 Authentication and Authorization

**Status:** Not implemented (admin user exists but not enforced)

**Description:**
Implement proper authentication and role-based access control.

**Features:**
- Login page
- User authentication with BCrypt
- Role-based authorization (Admin, Manager, Operator, Viewer)
- Page-level permissions
- Action-level permissions
- Session management
- Remember me functionality
- Password change

**Estimated Effort:** 4-5 hours

---

### 5.2 Audit Trail

**Status:** Not implemented

**Description:**
Track all data changes for compliance and troubleshooting.

**Features:**
- Log all Create/Update/Delete operations
- Store who, what, when, where
- View audit history per entity
- Filter audit logs
- Export audit reports

**Technical Requirements:**
- Create AuditLog entity
- Implement audit interceptor
- Create audit service
- Create audit viewer page

**Estimated Effort:** 3-4 hours

---

### 5.3 Unit and Integration Tests

**Status:** No test infrastructure

**Description:**
Add comprehensive test coverage.

**Test Types:**
- Unit tests for services (xUnit)
- Integration tests for repositories
- UI component tests (bUnit)
- End-to-end tests (Playwright)

**Estimated Effort:** 8-10 hours

---

### 5.4 Error Handling and Logging

**Status:** Basic error handling

**Description:**
Improve error handling and add structured logging.

**Features:**
- Centralized exception handling
- User-friendly error messages
- Structured logging (Serilog)
- Log levels configuration
- Log to file/database
- Error notification system

**Estimated Effort:** 2-3 hours

---

## 📊 Priority 6: Reporting Enhancements

### 6.1 Advanced Reports

**Status:** Basic reports exist

**Description:**
Add more comprehensive reporting capabilities.

**New Reports:**
- Cost analysis report
- Profit margin analysis
- Material usage report
- Production efficiency report
- Supplier performance report
- Shipment tracking report
- Custom report builder

**Estimated Effort:** 4-5 hours

---

### 6.2 Report Export Formats

**Status:** Excel export implemented

**Description:**
Add more export formats.

**Formats:**
- PDF export
- CSV export
- JSON export (for API consumption)
- Print-friendly HTML

**Estimated Effort:** 2-3 hours

---

## 🔄 Priority 7: Integration and Automation

### 7.1 Barcode/QR Code Support

**Status:** Not implemented

**Description:**
Add barcode scanning for inventory management.

**Features:**
- Generate product barcodes
- Generate material barcodes
- Scan barcode for quick lookup
- Print barcode labels
- Mobile-friendly scanning

**Estimated Effort:** 3-4 hours

---

### 7.2 Email Notifications

**Status:** Not implemented

**Description:**
Send email notifications for important events.

**Notifications:**
- Low stock alerts
- Production order completion
- Shipment status updates
- Daily summary reports

**Estimated Effort:** 2-3 hours

---

## 📱 Priority 8: Mobile Optimization

### 8.1 Mobile-Responsive Improvements

**Status:** Partially responsive

**Description:**
Optimize UI for mobile devices.

**Improvements:**
- Mobile-friendly tables
- Touch-friendly buttons
- Simplified mobile navigation
- Mobile-specific views
- Offline support (PWA)

**Estimated Effort:** 4-5 hours

---

## 🗂️ Recommended Implementation Order

**Phase 1 - Core Production (Week 1-2):**
1. Template-based inventory depletion
2. Finished goods inventory
3. Template Builder TreeView

**Phase 2 - Shipping & Tracking (Week 3):**
4. Shipment management system
5. Enhanced material selection

**Phase 3 - Polish & Quality (Week 4):**
6. Complete localization
7. Form validation enhancement
8. Dashboard enhancements

**Phase 4 - Enterprise Features (Week 5-6):**
9. Authentication and authorization
10. Audit trail
11. Unit and integration tests

**Phase 5 - Advanced Features (Week 7-8):**
12. Advanced reports
13. Barcode support
14. Email notifications
15. Mobile optimization

---

## 📝 Notes for Implementers

### General Guidelines

1. **Follow existing patterns**: Use the same structure as existing features
2. **Clean architecture**: Maintain separation of concerns
3. **MudBlazor components**: Use MudBlazor for all UI components
4. **Dark theme**: Ensure new features work in both dark and light modes
5. **Localization**: Add translation keys for all UI text
6. **Error handling**: Always handle errors gracefully
7. **Performance**: Use `AsSplitQuery()` for multiple includes
8. **Documentation**: Update this file as features are completed

### Code Style

- Follow C# naming conventions
- Use async/await for all I/O operations
- Use DTOs for data transfer between layers
- Keep methods small and focused
- Add XML documentation comments for public APIs

### Testing

- Test all new features manually before committing
- Take screenshots of UI changes
- Test both dark and light themes
- Test in different screen sizes
- Test error scenarios

---

## ✅ Completion Checklist for Each Feature

When implementing a feature, ensure:

- [ ] Code compiles without errors or warnings
- [ ] Feature works as expected in dark theme
- [ ] Feature works as expected in light theme
- [ ] All dialogs open and close properly
- [ ] All buttons perform their actions
- [ ] Error handling is in place
- [ ] Success/error notifications are shown
- [ ] Data persists correctly to database
- [ ] Navigation menu is updated (if needed)
- [ ] Localization keys are added
- [ ] Code follows existing patterns
- [ ] No console errors in browser
- [ ] No EF Core warnings in logs
- [ ] Manual testing completed
- [ ] Screenshots taken of UI changes
- [ ] This document updated to reflect completion

---

## 📞 Questions or Issues?

If you encounter issues or have questions while implementing these features:

1. Check existing code patterns in similar features
2. Review the current PR description for architectural decisions
3. Test in isolation before integrating
4. Document any deviations from the plan
5. Update this file with lessons learned

---

**Last Updated:** 2026-02-10  
**Document Version:** 1.0  
**Status:** Ready for implementation
