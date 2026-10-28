# Restaurant API

A minimal-API ASP.NET Core backend for managing a restaurant's menu, orders, and pre-receipts, built with EF Core and SQLite.

## Tech stack

- **.NET 10** / ASP.NET Core Minimal APIs
- **Entity Framework Core 10** with the **SQLite** provider
- **Scalar** for interactive API documentation (OpenAPI UI)

## Project structure

```
RestaurantApi/
├── DTOs/                # Request/response records (Menus, Orders, PreReceipts)
├── Endpoints/           # Minimal API route groups (MenuEndpoints, OrderEndpoints, PreReceiptEndpoints)
├── Enums/               # OrderStatus
├── Filters/             # RejectEmptyIdFilter (rejects Guid.Empty route ids)
├── Infrastructure/      # ApplicationDbContext (EF Core model + relationships)
├── Migrations/          # EF Core migrations
├── Models/               # Menu, Order, PreReceipt entities
├── Services/            # Business logic (MenuService, OrderService, PreReceiptService)
└── Program.cs           # App composition root
```

## Domain model

- **Menu** — a dish with a name, description, and price. Has many `Orders`.
- **Order** — an order for a quantity of a given `Menu`, with an `OrderStatus` (`Pending`, `InProgress`, `Completed`, `Cancelled`). `UnitPrice`/`TotalPrice` are computed from the related `Menu`.
- **PreReceipt** — a receipt generated for a **Completed** order (one-to-one with `Order`). `OrderName`/`UnitPrice`/`TotalPrice` are computed from the related `Order` (and its `Menu`).

Relationships: `Menu` 1─* `Order` (`Restrict` on delete — a menu with existing orders can't be deleted), `Order` 1─1 `PreReceipt` (`Cascade` on delete — deleting an order removes its receipt).

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [`dotnet-ef`](https://learn.microsoft.com/ef/core/cli/dotnet) tool (`dotnet tool install --global dotnet-ef`)

### Run the API

```bash
cd RestaurantApi
dotnet run
```

The database schema is created/updated automatically on startup (`Database.Migrate()` runs in `Program.cs`), so no manual migration step is required for a fresh clone.

By default it listens on `http://localhost:5262` (see `Properties/launchSettings.json`; use `dotnet run --launch-profile https` for `https://localhost:7256` as well).

### Explore/test the API

- **Scalar UI** (interactive docs, "try it" requests): `http://localhost:5262/scalar`
- **Raw OpenAPI document**: `http://localhost:5262/openapi/v1.json`

### Database migrations

If you change the models or `ApplicationDbContext`, add a new migration:

```bash
cd RestaurantApi
dotnet ef migrations add <MigrationName>
```

The app applies pending migrations automatically on the next `dotnet run`, or you can apply them manually with:

```bash
dotnet ef database update
```

## API overview

| Resource | Endpoints |
|---|---|
| **Menus** | `GET /menus`, `GET /menus/{id}`, `POST /menus`, `PATCH /menus/{id}`, `DELETE /menus/{id}` |
| **Orders** | `GET /orders?search=`, `GET /orders/{id}`, `POST /orders`, `PATCH /orders/{id}`, `PATCH /orders/{id}/status`, `DELETE /orders/{id}` |
| **PreReceipts** | `GET /preReceipts`, `GET /preReceipts/{id}`, `POST /preReceipts`, `DELETE /preReceipts/{id}` |

`PATCH` endpoints are partial updates — only the fields present in the request body are changed; omitted fields keep their current value.

### Typical flow

1. `POST /menus` — create a menu item.
2. `POST /orders` — place an order against that menu.
3. `PATCH /orders/{id}/status` — mark the order `Completed`.
4. `POST /preReceipts` — generate a receipt for the completed order.

## Configuration

Connection string lives in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=restaurant.db"
}
```

The SQLite database file (`restaurant.db`) is created automatically and is git-ignored — it's local runtime state, not source.
