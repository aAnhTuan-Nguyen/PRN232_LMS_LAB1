# AGENTS.md

## Goal

Build an ASP.NET Core RESTful API for PRN232 Lab 1: a Learning Management System (LMS).

Follow this file as the source of truth. If the user only asks for planning or documentation, do not implement code.

## Architecture

Use strict 3-layer architecture:

- `PRN232.[ProjectName].API`: controllers, filters, middleware/API configuration, Swagger, HTTP response concerns.
- `PRN232.[ProjectName].Services`: business logic, business models, request models, response models, validators, mappings.
- `PRN232.[ProjectName].Repositories`: EF Core DbContext, entity models, Fluent API configurations, generic repositories, Unit of Work.

Rules:

- Controllers must be thin and must not contain business logic.
- Services contain business rules and coordinate use cases.
- Repositories contain data access only and must not contain business logic.
- Do not return entity models directly from API endpoints.
- Do not use request/response models in the repository layer.
- Do not bypass the service layer from controllers.

## Database And EF Core

Use PostgreSQL with Entity Framework Core Code First.

Requirements:

- Use PostgreSQL Docker image `postgres:16`.
- Use EF Core Fluent API for table mapping, keys, relationships, lengths, required fields, indexes, and delete behavior.
- Prefer separate configuration classes with `IEntityTypeConfiguration<TEntity>` when mappings become non-trivial.
- Do not rely only on data annotations for database schema rules.
- Use migrations or a clear code-first database creation flow.

Required tables:

- `Semester(SemesterId int, SemesterName nvarchar(100), StartDate datetime, EndDate datetime)`
- `Course(CourseId int, CourseName nvarchar(100), SemesterId int)`
- `Subject(SubjectId int, SubjectCode varchar(20), SubjectName nvarchar(100), Credit int)`
- `Student(StudentId int, FullName nvarchar(100), Email varchar(100), DateOfBirth datetime)`
- `Enrollment(EnrollmentId int, StudentId int, CourseId int, EnrollDate datetime, Status varchar(20))`

Additional tables or columns are allowed when useful for the LMS domain.

Seed at least:

- 5 semesters
- 50 students
- 10 subjects
- 20 courses
- 500 enrollments

## Repository And Unit Of Work

Use Generic Repository and Unit of Work.

Expected shape:

- `IGenericRepository<TEntity>` for common query/CRUD operations.
- `IUnitOfWork` to expose repositories and commit changes.
- `SaveChangesAsync` belongs to Unit of Work, not individual repositories.
- Repositories should return entities or queryable data for the service layer to process.
- Services decide business rules, validation flow, response mapping, and use-case behavior.

## Model Types

Use 4 model types:

- Entity Model: database mapping only.
- Business Model: internal business processing.
- Request Model: client input.
- Response Model: API output.

Rules:

- Entity models stay inside repository/data access boundaries unless used internally by services.
- Request models are validated before business processing.
- Response models are the only models returned to API clients.

## Validation

Use FluentValidation version 12.

Requirements:

- Add validators for request models.
- Keep validation rules out of controllers.
- Return validation errors in the standard API response format.
- Do not implement unnecessary advanced validation unless explicitly requested.

Preferred validation response format:

```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": {
    "email": ["Email is invalid"]
  }
}
```

## Error Handling And API Response Format

All API responses must use a consistent response wrapper:

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {},
  "errors": null
}
```

List APIs must also include pagination metadata:

```json
"pagination": {
  "page": 1,
  "pageSize": 10,
  "totalItems": 100,
  "totalPages": 10
}
```

Create an API filter for error handling so exceptions and validation failures are returned in the same frontend-friendly format.

Filter requirements:

- Convert validation failures to HTTP `400`.
- Convert not-found cases to HTTP `404`.
- Convert unexpected exceptions to HTTP `500`.
- Do not leak stack traces or internal exception details to the frontend.
- Keep the response shape consistent for success and failure.

Use proper HTTP status codes:

- `200`: Success
- `201`: Created
- `400`: Bad Request
- `404`: Not Found
- `500`: Internal Server Error

## REST API Design

Use RESTful resource endpoints with plural nouns:

- `GET /api/students`
- `GET /api/students/{id}`
- `POST /api/students`
- `PUT /api/students/{id}`
- `DELETE /api/students/{id}`
- `GET /api/enrollments/{id}`

Do not use action-style endpoints:

- `/api/getStudents`
- `/api/createEnrollment`

For `GET /api/{resources}/{id}`:

- Return complete related data for the resource.
- Avoid circular references and infinite recursion.
- Return `404` if the resource does not exist.

## Collection APIs

All list APIs must support:

- Searching: `?search=nguyen`
- Sorting: `?sort=fullName,-dateOfBirth`
- Paging: `?page=2&size=10`
- Field selection: `?fields=studentId,fullName,email`
- Expansion: `?expand=student,course`

Combined example:

```http
GET /api/enrollments?search=active&sort=-enrollDate&page=1&size=20&fields=enrollmentId,status&expand=student,course
```

## Docker

Include:

- `Dockerfile`
- `docker-compose.yml`

Docker Compose must run:

- API container
- PostgreSQL 16 database container using image `postgres:16`

Configure the API database connection through Docker Compose environment variables or configuration.

## Swagger / OpenAPI

Swagger/OpenAPI is required and must document:

- Endpoints
- Request models
- Response models
- HTTP status codes
- Testing through Swagger UI

## Out Of Scope Unless Explicitly Requested

- Authentication / Authorization
- JWT security
- Unit tests / integration tests
- Complex role-based workflows
- Frontend application

Note: FluentValidation and the API error filter are required, so they are not out of scope.

## Completion Checklist

- 3-layer architecture is respected.
- Project names follow `PRN232.[ProjectName].*`.
- PostgreSQL is used.
- Docker Compose uses `postgres:16`.
- EF Core Code First uses Fluent API.
- Generic Repository is implemented.
- Unit of Work is implemented.
- 4 model types are used correctly.
- FluentValidation 12 validates request models.
- Error filter returns frontend-friendly standard responses.
- REST endpoints use plural resource nouns.
- List APIs support search, sort, paging, field selection, and expansion.
- Pagination metadata is included.
- Swagger/OpenAPI is configured.
- Seed data meets the minimum required counts.

## Agent Working Rules

- Ask before making broad code changes.
- Do not implement code when the user only asks for planning or documentation.
- Keep changes scoped to the user's explicit request.
- Before editing multiple files, explain the intended changes briefly.
- Do not overwrite user changes.
- Use existing project conventions when a project already exists.
