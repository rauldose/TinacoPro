# TinacoPro - Inventory & Production Management System

A comprehensive Blazor Server application for managing inventory and production of Tinacos (water tanks).

## Features

- **Product Catalog**: Manage products with different sizes, models, and capacities
- **Raw Material Inventory**: Track raw materials with stock levels, units, and costs
- **Production Orders**: Create and manage production orders with automatic stock consumption
- **Finished Goods Inventory**: Track finished products from production orders
- **Supplier Management**: Maintain supplier information and contacts
- **Dashboard**: Real-time overview of production, inventory, and order status
- **Reports**: Daily, weekly, and monthly production reports and inventory status
- **User Management**: Basic authentication and role-based access control (Admin, Manager, Operator)

## Technology Stack

- **.NET 8**: Latest .NET framework
- **Blazor Server**: Interactive server-side UI framework
- **SQLite**: Lightweight database for data persistence
- **Entity Framework Core**: ORM for database access
- **Bootstrap 5**: Responsive UI framework
- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and Web layers

## Project Structure

```
TinacoPro/
├── src/
│   ├── TinacoPro.Domain/          # Domain entities and interfaces
│   ├── TinacoPro.Application/     # Business logic and DTOs
│   ├── TinacoPro.Infrastructure/  # Data access and repositories
│   └── TinacoPro.Web/            # Blazor Server UI
└── TinacoPro.sln
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Running the Application

1. Clone the repository:
```bash
git clone https://github.com/rauldose/TinacoPro.git
cd TinacoPro
```

2. Build the solution:
```bash
dotnet build
```

3. Run the application:
```bash
cd src/TinacoPro.Web
dotnet run
```

4. Open your browser and navigate to:
```
http://localhost:5039
```

### Default Login

- **Username**: admin
- **Password**: admin123

## Sample Data

The application comes pre-seeded with:
- 3 Sample Products (Tinaco Estándar, Tinaco Grande, Tinaco Premium)
- 3 Raw Materials (Polietileno, Colorante Negro, Aditivo UV)
- 2 Suppliers (Proveedora de Plásticos SA, Químicos Industriales)
- 1 Admin User

## Architecture

The application follows Clean Architecture principles:

- **Domain Layer**: Contains core business entities and interfaces
- **Application Layer**: Contains business logic, services, and DTOs
- **Infrastructure Layer**: Contains data access implementations and database context
- **Web Layer**: Contains Blazor components and UI logic

## Database

The application uses SQLite for data storage. The database file (`tinacopro.db`) is created automatically on first run in the Web project directory.

## Features Details

### Product Management
- Create, edit, and view products
- Track product specifications (name, model, size, capacity)
- Manage product status (active/inactive)
- Define bill of materials for each product

### Raw Material Inventory
- Track current stock levels
- Set minimum stock thresholds
- Manage unit costs
- Stock in/out operations (planned feature)
- Low stock alerts

### Production Orders
- Create production orders for specific products
- Track order status (Pending, In Progress, Completed, Cancelled)
- Automatic raw material consumption on order completion
- Order history and tracking

### Dashboard
- Real-time KPIs (Active Products, Low Stock Items, In Progress Orders, Completed Today)
- Production overview with weekly and monthly statistics
- Inventory status monitoring
- Low stock alerts

### Reports
- Production summary (daily, weekly, monthly)
- Inventory status reports
- Product catalog summaries
- Production by product analysis

## License

This project is licensed under the MIT License.