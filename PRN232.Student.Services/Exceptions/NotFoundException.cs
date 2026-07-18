namespace PRN232.Student.Services;

public sealed class NotFoundException(string message) : Exception(message);
