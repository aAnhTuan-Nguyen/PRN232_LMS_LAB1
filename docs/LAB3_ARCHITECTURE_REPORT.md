# PRN232 LMS Lab 3 - gRPC and Microservices Architecture

## Service decomposition

The Lab 2 monolith is split into four deployable applications. Identity Service owns authentication, authorization data, JWT generation, users, and refresh tokens. Student Service owns the Student aggregate and all student CRUD operations. Course Service owns semesters, subjects, courses, and enrollment records. API Gateway is a YARP reverse proxy; it does not own business data or business logic.

Each business service retains the required three layers:

- API: REST controllers, gRPC host, JWT pipeline, Swagger, error response wrapper, and Serilog request logging.
- Services: request/response/business models, FluentValidation, use cases, and gRPC client adapters.
- Repositories: entities, Fluent EF Core mapping, DbContext, Generic Repository, Unit of Work, seed data, and migrations.

The PRN232.LMS.Contracts project is intentionally limited to Protos/lms.proto. It generates strongly typed client/server contracts and contains no entity or database code.

## Database design

The Docker topology uses three isolated postgres:16 containers:

| Database | Owned tables | Seed data |
| --- | --- | --- |
| identity-db | User, RefreshToken | admin/student accounts |
| student-db | Student | 50 students |
| course-db | Semester, Subject, Course, Enrollment | 5 semesters, 10 subjects, 20 courses, 500 enrollments |

Enrollment.StudentId is indexed but is deliberately not a relational foreign key. Student Service remains the source of truth for student existence. This prevents the Course Service from directly accessing Student Service data.

## Gateway and authentication

YARP routes /api/auth/* to Identity Service, /api/students/* to Student Service, and /api/courses/*, /api/semesters/*, /api/subjects/*, /api/enrollments/* to Course Service. The same mappings exist under /api/v1/*.

Login and refresh-token routes are anonymous. Gateway validates a shared JWT issuer, audience, signing key, and expiry before forwarding protected write/admin routes. Every downstream service independently validates the same token, so a caller cannot bypass authorization by calling a service port directly. Admin-only delete and user-list APIs require the Admin role claim.

Swagger is enabled on the three business services:

- Identity: http://localhost:8081/swagger
- Student: http://localhost:8082/swagger
- Course: http://localhost:8083/swagger

## gRPC flow

The required enrollment flow is:

    Client -> API Gateway -> Course Service -> StudentLookup gRPC -> Student Service

StudentLookup.GetStudent verifies a StudentId before Course Service writes an enrollment. It returns a typed reply containing exists, studentId, fullName, and email. A missing student produces an API 404; no enrollment is stored.

StudentLookup.GetStudents performs batch lookups for expand=student and course-student listings. A second internal contract, CourseEnrollmentLookup, lets Student Service return expand=enrollments without reading Course Service's database. The corresponding gRPC ports remain internal to the Compose network.

## Deployment and demonstration

Copy .env.example to .env, choose a PostgreSQL password and a JWT secret of at least 32 characters, then run:

    docker compose up --build

The Compose file starts the Gateway, three services, and three PostgreSQL 16 databases. Every app emits Serilog JSON request logs with request method, path, status code, and elapsed time.

Import docs/postman/PRN232_LMS_Lab3.postman_collection.json into Postman. It demonstrates login/JWT generation, protected access, a 401 case, successful enrollment with Student gRPC verification, and rejection of an unknown student.
