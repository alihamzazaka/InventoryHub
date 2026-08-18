# InventoryHub

Full-stack inventory management application built with ASP.NET Core Minimal API and Blazor WebAssembly.

## Features
- RESTful inventory CRUD API
- Search and category filtering
- Pagination
- Consistent JSON responses
- Validation and structured errors
- CORS configuration
- In-memory caching
- Blazor HttpClient integration
- Loading and error states
- Responsive inventory dashboard

## Run
```bash
dotnet run --project src/InventoryHub.Api
```
Then run the Blazor client:
```bash
dotnet run --project src/InventoryHub.Client
```

See `docs/PROJECT_SUMMARY.md` for the course reflection and integration details.
