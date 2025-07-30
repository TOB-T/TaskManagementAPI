# Task Management API

A robust and cleanly structured RESTful API for managing tasks and categories, built with .NET 8 and Onion Architecture.

## Architecture Overview

* **Domain** (`TaskManagement.Domain`): Core entities and interfaces.
* **Application** (`TaskManagement.Application`): Business logic, DTOs, and use cases.
* **Infrastructure** (`TaskManagement.Infrastructure`): Data access (EF Core In-Memory, repositories) and external services.
* **API** (`TaskManagement.API`): Controllers, middleware, and configuration.
* **Tests** (`TaskManagement.Tests`): Unit tests covering all layers.

## Key Features

* Full CRUD for tasks and categories
* Filtering by category, priority, and completion status
* Search in titles and descriptions
* Paginated responses with metadata
* Mark tasks as completed
* Input validation and error handling
* Swagger UI with Basic Auth for documentation
* In-memory database for quick setup
* Unit tests using xUnit and Moq

## Technology Stack

* **.NET 8**
* **ASP.NET Core Web API**
* **Entity Framework Core** (In-Memory Provider)
* **Swagger/OpenAPI**
* **xUnit & Moq**
* **Onion (Clean) Architecture**
* **Dependency Injection**
* **Microsoft.Extensions.Logging**

## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Git](https://git-scm.com/)

### Setup

1. **Clone the repo**

   ```bash
   ```

git clone [https://github.com/yourusername/TaskManagementAPI.git](https://github.com/yourusername/TaskManagementAPI.git)
cd TaskManagementAPI

````
2. **Restore and build**
   ```bash
dotnet restore
dotnet build
````

3. **Run the API**

   ```bash
   ```

dotnet run --project TaskManagement.API

````
4. **Verify**
   - API: `https://localhost:5001` or `http://localhost:5000`
   - Swagger UI: `https://localhost:5001/swagger`
     - **Username:** `admin`
     - **Password:** `swagger123`

## Testing

Run all tests with:
```bash
dotnet test
````

## Project Structure

```
TaskManagement/
├─ TaskManagement.Domain/
├─ TaskManagement.Application/
├─ TaskManagement.Infrastructure/
├─ TaskManagement.API/
├─ TaskManagement.Tests/
└─ TaskManagement.sln
```

## API Endpoints

### Tasks

* `GET /api/tasks`
* `GET /api/tasks/{id}`
* `POST /api/tasks`
* `PUT /api/tasks/{id}`
* `DELETE /api/tasks/{id}`
* `PATCH /api/tasks/{id}/complete`

Supports query parameters: `categoryId`, `priority`, `isCompleted`, `search`, `page`, `pageSize`.

### Categories

* `GET /api/categories`
* `GET /api/categories/{id}`

## Sample Request

```bash
curl -X POST "https://localhost:5001/api/tasks" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Finish docs",
    "description": "Complete API documentation",
    "priority": 2,
    "categoryId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "dueDate": "2025-12-31T23:59:59Z"
  }'
```

## Contributing

1. Fork the repo
2. Create a branch (`git checkout -b feature/xyz`)
3. Commit your changes
4. Push and open a PR

## License

This project is licensed under MIT. See [LICENSE](LICENSE).
