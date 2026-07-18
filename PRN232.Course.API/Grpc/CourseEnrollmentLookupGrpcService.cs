using Grpc.Core;
using PRN232.Course.Services;
using PRN232.LMS.Contracts;

namespace PRN232.Course.API;

public sealed class CourseEnrollmentLookupGrpcService(ICourseLmsService service)
    : CourseEnrollmentLookup.CourseEnrollmentLookupBase
{
    public override async Task<ExistsReply> HasEnrollments(
        StudentIdRequest request,
        ServerCallContext context)
    {
        var exists = await service.HasEnrollmentsAsync(request.StudentId, context.CancellationToken);
        return new ExistsReply { Exists = exists };
    }

    public override async Task<StudentEnrollmentsReply> GetEnrollments(
        StudentIdsRequest request,
        ServerCallContext context)
    {
        var response = new StudentEnrollmentsReply();
        var enrollments = await service.GetEnrollmentsForStudentsAsync(
            request.StudentIds.Distinct().ToArray(),
            context.CancellationToken);

        foreach (var enrollment in enrollments)
        {
            response.Enrollments.Add(new EnrollmentSummary
            {
                StudentId = enrollment.StudentId,
                EnrollmentId = enrollment.EnrollmentId,
                CourseId = enrollment.CourseId,
                CourseName = enrollment.Course.CourseName,
                EnrollDate = enrollment.EnrollDate.ToString("O"),
                Status = enrollment.Status
            });
        }

        return response;
    }
}
