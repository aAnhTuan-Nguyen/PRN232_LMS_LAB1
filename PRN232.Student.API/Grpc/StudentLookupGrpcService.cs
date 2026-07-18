using Grpc.Core;
using PRN232.LMS.Contracts;
using PRN232.Student.Services;

namespace PRN232.Student.API;

public sealed class StudentLookupGrpcService(IStudentService service) : StudentLookup.StudentLookupBase
{
    public override async Task<StudentLookupReply> GetStudent(
        StudentIdRequest request,
        ServerCallContext context)
    {
        try
        {
            var item = await service.GetByIdAsync(request.StudentId, null, context.CancellationToken);

            return new StudentLookupReply
            {
                Exists = true,
                Student = new StudentSummary
                {
                    StudentId = item.StudentId,
                    FullName = item.FullName,
                    Email = item.Email
                }
            };
        }
        catch (NotFoundException)
        {
            return new StudentLookupReply { Exists = false };
        }
    }

    public override async Task<StudentsReply> GetStudents(
        StudentIdsRequest request,
        ServerCallContext context)
    {
        var response = new StudentsReply();

        foreach (var id in request.StudentIds.Distinct())
        {
            var item = await GetStudent(new StudentIdRequest { StudentId = id }, context);

            if (item.Exists)
            {
                response.Students.Add(item.Student);
            }
        }

        return response;
    }
}
