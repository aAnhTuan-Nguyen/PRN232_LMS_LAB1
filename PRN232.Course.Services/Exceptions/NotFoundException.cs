namespace PRN232.Course.Services;

public sealed class NotFoundException(string message) : Exception(message);
