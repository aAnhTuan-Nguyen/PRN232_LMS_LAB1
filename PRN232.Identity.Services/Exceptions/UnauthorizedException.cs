namespace PRN232.Identity.Services;

public sealed class UnauthorizedException(string message) : Exception(message);
