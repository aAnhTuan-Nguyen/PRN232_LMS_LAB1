using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

internal static class LmsMapping
{
    public static SemesterResponse ToSemesterResponse(Semester semester, bool includeCourses)
    {
        return new SemesterResponse
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Courses = includeCourses
                ? semester.Courses.Select(course => ToCourseSummary(course)).ToList()
                : null
        };
    }

    public static SubjectResponse ToSubjectResponse(Subject subject, bool includeCourses)
    {
        return new SubjectResponse
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit,
            Courses = includeCourses
                ? subject.Courses.Select(course => ToCourseSummary(course)).ToList()
                : null
        };
    }

    public static CourseResponse ToCourseResponse(
        Course course,
        bool includeSemester,
        bool includeSubject,
        bool includeEnrollments)
    {
        return new CourseResponse
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            Semester = includeSemester && course.Semester is not null ? ToSemesterSummary(course.Semester) : null,
            SubjectId = course.SubjectId,
            Subject = includeSubject && course.Subject is not null ? ToSubjectSummary(course.Subject) : null,
            Enrollments = includeEnrollments
                ? course.Enrollments.Select(enrollment => ToEnrollmentSummary(enrollment, includeCourse: false)).ToList()
                : null
        };
    }

    public static StudentResponse ToStudentResponse(Student student, bool includeEnrollments)
    {
        return new StudentResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = includeEnrollments
                ? student.Enrollments.Select(enrollment => ToEnrollmentSummary(enrollment, includeCourse: true)).ToList()
                : null
        };
    }

    public static EnrollmentResponse ToEnrollmentResponse(
        Enrollment enrollment,
        bool includeStudent,
        bool includeCourse)
    {
        return new EnrollmentResponse
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            Student = includeStudent && enrollment.Student is not null ? ToStudentSummary(enrollment.Student) : null,
            CourseId = enrollment.CourseId,
            Course = includeCourse && enrollment.Course is not null ? ToCourseSummary(enrollment.Course) : null,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status
        };
    }

    private static SemesterSummaryResponse ToSemesterSummary(Semester semester)
    {
        return new SemesterSummaryResponse
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName
        };
    }

    private static SubjectSummaryResponse ToSubjectSummary(Subject subject)
    {
        return new SubjectSummaryResponse
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName
        };
    }

    private static CourseSummaryResponse ToCourseSummary(Course course)
    {
        return new CourseSummaryResponse
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            Semester = course.Semester is not null ? ToSemesterSummary(course.Semester) : null,
            Subject = course.Subject is not null ? ToSubjectSummary(course.Subject) : null
        };
    }

    private static StudentSummaryResponse ToStudentSummary(Student student)
    {
        return new StudentSummaryResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email
        };
    }

    private static EnrollmentSummaryResponse ToEnrollmentSummary(Enrollment enrollment, bool includeCourse)
    {
        return new EnrollmentSummaryResponse
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status,
            Course = includeCourse && enrollment.Course is not null ? ToCourseSummary(enrollment.Course) : null
        };
    }
}
