# Restaurant API

A minimal-API ASP.NET Core backend for managing a restaurant's menu and orders, with a summary report, built with EF Core and SQLite.

## Tech stack

- **.NET 10** / ASP.NET Core Minimal APIs
- **Entity Framework Core 10** with the **SQLite** provider
- **Scalar** for interactive API documentation (OpenAPI UI)

## Project structure

```
RestaurantApi/
├── DTOs/                # Request/response records (Menus, Orders, Reports)
├── Endpoints/           # Minimal API route groups (MenuEndpoints, OrderEndpoints, ReportEndpoints)
├── Filters/             # RejectEmptyIdFilter (rejects Guid.Empty route ids)
├── Infrastructure/      # ApplicationDbContext (EF Core model + relationships)
├── Migrations/          # EF Core migrations
├── Models/              # Menu, Order entities and the OrderStatus enum
├── Services/            # Business logic (MenuService, OrderService, ReportService)
└── Program.cs           # App composition root
scripts/
└── seed.sh              # Loads sample menus and orders through the running API
```

## Domain model

- **Menu** — a dish with a name, description, and price. Has many `Orders`.
- **Order** — an order for a quantity of a given `Menu` on a given `Date`, with an `OrderStatus` (`Pending`, `InProgress`, `Completed`, `Cancelled`). `UnitPrice`/`TotalPrice` are computed from the related `Menu`.

Relationship: `Menu` 1─* `Order` (`Restrict` on delete — a menu with existing orders can't be deleted).

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
| **Orders** | `GET /orders`, `GET /orders/{id}`, `POST /orders`, `PATCH /orders/{id}`, `DELETE /orders/{id}` |
| **Reports** | `GET /reports/summary?startDate=&endDate=` |

`PATCH` endpoints are partial updates — only the fields present in the request body are changed; omitted fields keep their current value.

### Dates

Dates are `DateOnly` values and must use ISO format `yyyy-MM-dd`, quoted in JSON bodies (e.g. `"date": "2026-10-31"`). Other formats such as `31-10-2026` are rejected with `400 Bad Request`.

### Listing orders

`GET /orders` returns orders sorted by date, newest first. All query parameters are optional and can be combined:

| Parameter | Description |
|---|---|
| `startDate` | Only orders on or after this date |
| `endDate` | Only orders on or before this date |
| `search` | Case-insensitive text matched against the order status or menu name |

Example: `GET /orders?search=pizza&startDate=2026-10-28&endDate=2026-10-30`. If `startDate` is after `endDate`, the API returns `400`.

### Summary report

`GET /reports/summary?startDate=2026-10-01&endDate=2026-10-31` (both dates required, inclusive) returns the number of orders in the range, the revenue from `Completed` orders, and a count per status (every status is always present):

```json
{
  "startDate": "2026-10-01",
  "endDate": "2026-10-31",
  "totalRevenue": 147.00,
  "totalOrders": 14,
  "orderStatusSummary": { "Pending": 3, "InProgress": 2, "Completed": 7, "Cancelled": 2 }
}
```

### Typical flow

1. `POST /menus` — create a menu item.
2. `POST /orders` — place an order against that menu (new orders always start as `Pending`).
3. `PATCH /orders/{id}` with `{ "status": "Completed" }` — change the order's status.
4. `GET /reports/summary` — see totals and per-status counts for a date range.

### Sample data

With the API running, load sample menus and orders (all four statuses) through the API:

```bash
./scripts/seed.sh                          # defaults to http://localhost:5262
./scripts/seed.sh http://localhost:5262    # or pass a base URL
```

Each run adds new records, so run it once per database. To start over, stop the API and delete `RestaurantApi/restaurant.db`; it is recreated on the next start.

## Configuration

Connection string lives in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=restaurant.db"
}
```

The SQLite database file (`restaurant.db`) is created automatically and is git-ignored — it's local runtime state, not source.
