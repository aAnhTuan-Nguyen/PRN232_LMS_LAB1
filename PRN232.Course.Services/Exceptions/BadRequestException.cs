namespace PRN232.Course.Services;

public sealed class BadRequestException(string message) : Exception(message);
