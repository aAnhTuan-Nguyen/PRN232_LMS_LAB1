using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.Services.Services;

internal static class LmsMapping
{
    public static SemesterBusinessModel ToSemesterBusinessModel(Semester semester, bool includeCourses)
    {
        return new SemesterBusinessModel
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Courses = includeCourses
                ? semester.Courses
                    .Select(course => ToCourseBusinessModel(
                        course,
                        includeSemester: false,
                        includeSubject: course.Subject is not null,
                        includeEnrollments: false))
                    .ToList()
                : null
        };
    }

    public static SubjectBusinessModel ToSubjectBusinessModel(Subject subject, bool includeCourses)
    {
        return new SubjectBusinessModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit,
            Courses = includeCourses
                ? subject.Courses
                    .Select(course => ToCourseBusinessModel(
                        course,
                        includeSemester: course.Semester is not null,
                        includeSubject: false,
                        includeEnrollments: false))
                    .ToList()
                : null
        };
    }

    public static CourseBusinessModel ToCourseBusinessModel(
        Course course,
        bool includeSemester,
        bool includeSubject,
        bool includeEnrollments)
    {
        return new CourseBusinessModel
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            SemesterId = course.SemesterId,
            Semester = includeSemester && course.Semester is not null
                ? ToSemesterBusinessModel(course.Semester, includeCourses: false)
                : null,
            SubjectId = course.SubjectId,
            Subject = includeSubject && course.Subject is not null
                ? ToSubjectBusinessModel(course.Subject, includeCourses: false)
                : null,
            Enrollments = includeEnrollments
                ? course.Enrollments
                    .Select(enrollment => ToEnrollmentBusinessModel(
                        enrollment,
                        includeStudent: false,
                        includeCourse: false))
                    .ToList()
                : null
        };
    }

    public static StudentBusinessModel ToStudentBusinessModel(Student student, bool includeEnrollments)
    {
        return new StudentBusinessModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = includeEnrollments
                ? student.Enrollments
                    .Select(enrollment => ToEnrollmentBusinessModel(
                        enrollment,
                        includeStudent: false,
                        includeCourse: enrollment.Course is not null))
                    .ToList()
                : null
        };
    }

    public static EnrollmentBusinessModel ToEnrollmentBusinessModel(
        Enrollment enrollment,
        bool includeStudent,
        bool includeCourse)
    {
        return new EnrollmentBusinessModel
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            Student = includeStudent && enrollment.Student is not null
                ? ToStudentBusinessModel(enrollment.Student, includeEnrollments: false)
                : null,
            CourseId = enrollment.CourseId,
            Course = includeCourse && enrollment.Course is not null
                ? ToCourseBusinessModel(
                    enrollment.Course,
                    includeSemester: enrollment.Course.Semester is not null,
                    includeSubject: enrollment.Course.Subject is not null,
                    includeEnrollments: false)
                : null,
            EnrollDate = enrollment.EnrollDate,
            Status = enrollment.Status
        };
    }

    public static SemesterResponse ToSemesterResponse(SemesterBusinessModel semester, bool includeCourses)
    {
        return new SemesterResponse
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            Courses = includeCourses
                ? semester.Courses?.Select(ToCourseSummary).ToList() ?? []
                : null
        };
    }

    public static SubjectResponse ToSubjectResponse(SubjectBusinessModel subject, bool includeCourses)
    {
        return new SubjectResponse
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Credit = subject.Credit,
            Courses = includeCourses
                ? subject.Courses?.Select(ToCourseSummary).ToList() ?? []
                : null
        };
    }

    public static CourseResponse ToCourseResponse(
        CourseBusinessModel course,
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
                ? course.Enrollments?.Select(enrollment => ToEnrollmentSummary(enrollment, includeCourse: false)).ToList() ?? []
                : null
        };
    }

    public static StudentResponse ToStudentResponse(StudentBusinessModel student, bool includeEnrollments)
    {
        return new StudentResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = student.DateOfBirth,
            Enrollments = includeEnrollments
                ? student.Enrollments?.Select(enrollment => ToEnrollmentSummary(enrollment, includeCourse: true)).ToList() ?? []
                : null
        };
    }

    public static EnrollmentResponse ToEnrollmentResponse(
        EnrollmentBusinessModel enrollment,
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

    private static SemesterSummaryResponse ToSemesterSummary(SemesterBusinessModel semester)
    {
        return new SemesterSummaryResponse
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName
        };
    }

    private static SubjectSummaryResponse ToSubjectSummary(SubjectBusinessModel subject)
    {
        return new SubjectSummaryResponse
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName
        };
    }

    private static CourseSummaryResponse ToCourseSummary(CourseBusinessModel course)
    {
        return new CourseSummaryResponse
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            Semester = course.Semester is not null ? ToSemesterSummary(course.Semester) : null,
            Subject = course.Subject is not null ? ToSubjectSummary(course.Subject) : null
        };
    }

    private static StudentSummaryResponse ToStudentSummary(StudentBusinessModel student)
    {
        return new StudentSummaryResponse
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email
        };
    }

    private static EnrollmentSummaryResponse ToEnrollmentSummary(EnrollmentBusinessModel enrollment, bool includeCourse)
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
