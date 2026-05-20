using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

internal static class LmsMapping
{
    public static SemesterResponse ToSemesterResponse(Semester semester, bool includeCourses)
    {
        SemesterBusinessModel business = ToSemesterBusiness(semester);

        return new SemesterResponse
        {
            SemesterId = business.SemesterId,
            SemesterName = business.SemesterName,
            StartDate = business.StartDate,
            EndDate = business.EndDate,
            Courses = includeCourses
                ? semester.Courses.Select(course => ToCourseSummary(course)).ToList()
                : null
        };
    }

    public static SubjectResponse ToSubjectResponse(Subject subject, bool includeCourses)
    {
        SubjectBusinessModel business = ToSubjectBusiness(subject);

        return new SubjectResponse
        {
            SubjectId = business.SubjectId,
            SubjectCode = business.SubjectCode,
            SubjectName = business.SubjectName,
            Credit = business.Credit,
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
        CourseBusinessModel business = ToCourseBusiness(course);

        return new CourseResponse
        {
            CourseId = business.CourseId,
            CourseName = business.CourseName,
            SemesterId = business.SemesterId,
            Semester = includeSemester && course.Semester is not null ? ToSemesterSummary(course.Semester) : null,
            SubjectId = business.SubjectId,
            Subject = includeSubject && course.Subject is not null ? ToSubjectSummary(course.Subject) : null,
            Enrollments = includeEnrollments
                ? course.Enrollments.Select(enrollment => ToEnrollmentSummary(enrollment, includeCourse: false)).ToList()
                : null
        };
    }

    public static StudentResponse ToStudentResponse(Student student, bool includeEnrollments)
    {
        StudentBusinessModel business = ToStudentBusiness(student);

        return new StudentResponse
        {
            StudentId = business.StudentId,
            FullName = business.FullName,
            Email = business.Email,
            DateOfBirth = business.DateOfBirth,
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
        EnrollmentBusinessModel business = ToEnrollmentBusiness(enrollment);

        return new EnrollmentResponse
        {
            EnrollmentId = business.EnrollmentId,
            StudentId = business.StudentId,
            Student = includeStudent && enrollment.Student is not null ? ToStudentSummary(enrollment.Student) : null,
            CourseId = business.CourseId,
            Course = includeCourse && enrollment.Course is not null ? ToCourseSummary(enrollment.Course) : null,
            EnrollDate = business.EnrollDate,
            Status = business.Status
        };
    }

    private static SemesterSummaryResponse ToSemesterSummary(Semester semester)
    {
        SemesterBusinessModel business = ToSemesterBusiness(semester);

        return new SemesterSummaryResponse
        {
            SemesterId = business.SemesterId,
            SemesterName = business.SemesterName
        };
    }

    private static SubjectSummaryResponse ToSubjectSummary(Subject subject)
    {
        SubjectBusinessModel business = ToSubjectBusiness(subject);

        return new SubjectSummaryResponse
        {
            SubjectId = business.SubjectId,
            SubjectCode = business.SubjectCode,
            SubjectName = business.SubjectName
        };
    }

    private static CourseSummaryResponse ToCourseSummary(Course course)
    {
        CourseBusinessModel business = ToCourseBusiness(course);

        return new CourseSummaryResponse
        {
            CourseId = business.CourseId,
            CourseName = business.CourseName,
            Semester = course.Semester is not null ? ToSemesterSummary(course.Semester) : null,
            Subject = course.Subject is not null ? ToSubjectSummary(course.Subject) : null
        };
    }

    private static StudentSummaryResponse ToStudentSummary(Student student)
    {
        StudentBusinessModel business = ToStudentBusiness(student);

        return new StudentSummaryResponse
        {
            StudentId = business.StudentId,
            FullName = business.FullName,
            Email = business.Email
        };
    }

    private static EnrollmentSummaryResponse ToEnrollmentSummary(Enrollment enrollment, bool includeCourse)
    {
        EnrollmentBusinessModel business = ToEnrollmentBusiness(enrollment);

        return new EnrollmentSummaryResponse
        {
            EnrollmentId = business.EnrollmentId,
            EnrollDate = business.EnrollDate,
            Status = business.Status,
            Course = includeCourse && enrollment.Course is not null ? ToCourseSummary(enrollment.Course) : null
        };
    }

    private static SemesterBusinessModel ToSemesterBusiness(Semester semester)
    {
        return new SemesterBusinessModel
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate
        };
    }

    private static SubjectBusinessModel ToSubjectBusiness(Subject subject)
    {
        return new SubjectBusinessModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit
        };
    }

    private static CourseBusinessModel ToCourseBusiness(Course course)
    {
        return new CourseBusinessModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            SubjectId = course.SubjectId
        };
    }

    private static StudentBusinessModel ToStudentBusiness(Student student)
    {
        return new StudentBusinessModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth
        };
    }

    private static EnrollmentBusinessModel ToEnrollmentBusiness(Enrollment enrollment)
    {
        return new EnrollmentBusinessModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status
        };
    }
}
