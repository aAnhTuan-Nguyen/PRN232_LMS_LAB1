namespace PRN232.Student.Services;

public sealed class BadRequestException(string message) : Exception(message);
