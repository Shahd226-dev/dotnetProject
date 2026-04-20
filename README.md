# Course Management System API

ASP.NET Core Web API project for managing students, instructors, courses, enrollments, and authentication.

## Assignment Coverage

This project keeps the existing domain models and implements the required Web Engineering assignment topics:

- Entity relationships (one-to-one, one-to-many, many-to-many)
- Service layer with dependency injection
- Create, Update, and Response DTOs
- Data Annotation validation on DTOs
- JWT authentication
- Role-based authorization
- LINQ Select projections into response DTOs
- AsNoTracking on read-only queries
- Async EF Core database operations
- Swagger endpoint documentation
- Hangfire recurring background job

## Technologies Used

- ASP.NET Core Web API: REST API framework and middleware pipeline.
- Entity Framework Core: ORM for database access and relationship mapping.
- SQLite / SQL Server Provider for EF Core: local file DB by default with optional SQL Server support.
- JWT Bearer Authentication: stateless token-based authentication.
- Hangfire + MemoryStorage: background job scheduling and dashboard.
- Data Annotations: request validation attributes on DTOs.
- Swagger (Swashbuckle): interactive API documentation and testing UI.

## Project Structure (Key Parts)

- Controllers: request/response endpoints
- Services/Interfaces: business logic contracts
- Services/Implementations: service layer implementation using AppDbContext
- DTOs: create/update/read request-response contracts
- Models: EF Core entity models
- Data/AppDbContext.cs: DbSets and relationship configuration
- Middleware/ExceptionMiddleware.cs: global error handling

## Entity Relationships

- One-to-one: Instructor <-> InstructorProfile
- One-to-many: Instructor -> Courses
- Many-to-many: Student <-> Course through Enrollment

## Authentication and Authorization

- JWT login endpoint: POST /api/auth/login
- Register endpoint: POST /api/auth/register
- Revoke endpoint: POST /api/auth/revoke

- Access token is returned as an HttpOnly cookie named access_token.
- APIs still support Authorization header bearer tokens when needed.

- If token is sent in request header:

Authorization: Bearer <jwt-token>

- Role-based endpoint protection is implemented using Authorize(Roles = ...)
  - Example roles: Admin, Instructor, User

## Hangfire

- Dashboard path: /hangfire

## Why HTTP-only Cookies Are Commonly Used

Although this project uses bearer tokens in the Authorization header (as required by the assignment), HTTP-only cookies are widely used in industry for security reasons:

- JavaScript cannot read HTTP-only cookies, which reduces token theft risk from XSS attacks.
- Cookies can be configured with Secure and SameSite flags to reduce MITM and CSRF risk.
- Browser-managed cookie handling can simplify session lifecycle patterns in web apps.

For APIs used by multiple clients, Authorization headers remain common and are also secure when combined with HTTPS, short token lifetimes, and proper token storage practices.

## Prerequisites

- .NET SDK 8.0+
- No external DB required for local run (SQLite file DB is default)

## Configuration

Update connection strings and JWT configuration in:

- appsettings.json
- appsettings.Development.json

## Run the API

1. Restore packages
   dotnet restore
2. Apply migrations
   dotnet tool run dotnet-ef database update
3. Run API
   dotnet run

Swagger UI is available at:

- /swagger

## EF Core Migrations

Create migration:

dotnet tool run dotnet-ef migrations add <MigrationName>

Apply migration:

dotnet tool run dotnet-ef database update

## API Endpoint Documentation

Detailed endpoint list and role requirements:

- docs/API_ENDPOINTS.md

## Swagger / Postman Screenshots

Add your proof screenshots here before submission:

- docs/screenshots/

Suggested captures:

- Successful login and returned JWT
- Authorized Admin-only POST endpoint
- Protected endpoint returning 401 without token
- Validation failure returning 400

## Notes for Submission

Before submission, ensure:

- Migrations exist in Migrations/ and database was updated
- Swagger endpoints are accessible and tested
- Screenshots are added under docs/screenshots/
