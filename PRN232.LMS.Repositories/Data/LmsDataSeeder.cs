using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public static class LmsDataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Semester>().HasData(BuildSemesters());
        modelBuilder.Entity<Subject>().HasData(BuildSubjects());
        modelBuilder.Entity<Student>().HasData(BuildStudents());
        modelBuilder.Entity<Course>().HasData(BuildCourses());
        modelBuilder.Entity<Enrollment>().HasData(BuildEnrollments());
        modelBuilder.Entity<User>().HasData(BuildUsers());
    }

    private static IEnumerable<Semester> BuildSemesters()
    {
        return Enumerable.Range(1, 5).Select(index => new Semester
        {
            SemesterId = index,
            SemesterName = $"Semester {index}",
            StartDate = new DateTime(2026, ((index - 1) * 2) + 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, ((index - 1) * 2) + 2, 28, 0, 0, 0, DateTimeKind.Utc)
        });
    }

    private static IEnumerable<Subject> BuildSubjects()
    {
        string[] names =
        [
            "Programming C#",
            "REST API Basics",
            "Database Systems",
            "Web Application Development",
            "Software Testing",
            "Cloud Deployment",
            "Object-Oriented Programming",
            "Data Structures",
            "Software Architecture",
            "Project Management"
        ];

        return names.Select((name, index) => new Subject
        {
            SubjectId = index + 1,
            SubjectCode = $"PRN{232 + index}",
            SubjectName = name,
            Credit = (index % 3) + 2
        });
    }

    private static IEnumerable<Student> BuildStudents()
    {
        string[] familyNames = ["Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do"];
        string[] givenNames = ["An", "Binh", "Chi", "Dung", "Hanh", "Khoa", "Linh", "Minh", "Nam", "Quyen"];

        return Enumerable.Range(1, 50).Select(index =>
        {
            string familyName = familyNames[(index - 1) % familyNames.Length];
            string givenName = givenNames[(index - 1) % givenNames.Length];

            return new Student
            {
                StudentId = index,
                FullName = $"{familyName} {givenName} {index:00}",
                Email = $"student{index:00}@lms.local",
                DateOfBirth = new DateTime(2000 + (index % 5), ((index - 1) % 12) + 1, ((index - 1) % 27) + 1, 0, 0, 0, DateTimeKind.Utc)
            };
        });
    }

    private static IEnumerable<Course> BuildCourses()
    {
        return Enumerable.Range(1, 20).Select(index => new Course
        {
            CourseId = index,
            CourseName = $"LMS Course {index:00}",
            SemesterId = ((index - 1) % 5) + 1,
            SubjectId = ((index - 1) % 10) + 1
        });
    }

    private static IEnumerable<Enrollment> BuildEnrollments()
    {
        string[] statuses = ["Active", "Completed", "Dropped", "Pending"];

        return Enumerable.Range(1, 500).Select(index => new Enrollment
        {
            EnrollmentId = index,
            StudentId = ((index - 1) % 50) + 1,
            CourseId = (((index - 1) * 7) % 20) + 1,
            EnrollDate = new DateTime(2026, ((index - 1) % 12) + 1, ((index - 1) % 27) + 1, 0, 0, 0, DateTimeKind.Utc),
            Status = statuses[(index - 1) % statuses.Length]
        });
    }

    private static IEnumerable<User> BuildUsers()
    {
        return
        [
            new User
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "AQAAAAIAAYagAAAAEAARIjNEVWZ3iJmqu8zd7v9beM38/rDCq5QIQb9fxyMcTbTS7+2d/1D1jeksUJiSHA==",
                Role = "Admin"
            },
            new User
            {
                UserId = 2,
                Username = "student",
                PasswordHash = "AQAAAAIAAYagAAAAEBAhMkNUZXaHmKm6u9zd/g+Moi/X/8e6K5vowoVxX3V8sxKlZw6e8oWnp1y49EQJhw==",
                Role = "Student"
            }
        ];
    }
}
