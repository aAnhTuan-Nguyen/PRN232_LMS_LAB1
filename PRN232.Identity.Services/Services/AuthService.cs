using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN232.Identity.Repositories;

namespace PRN232.Identity.Services;

public sealed class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> options) : IAuthService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.Query()
                       .SingleOrDefaultAsync(
                           x => x.Username.ToLower() == request.Username.Trim().ToLower(),
                           cancellationToken)
                   ?? throw new UnauthorizedException("Invalid username or password.");

        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var refreshToken = NewRefreshToken(user.UserId);
        await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Build(user, refreshToken.Token);
    }

    public async Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var current = await unitOfWork.RefreshTokens.Query()
                          .Include(x => x.User)
                          .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken)
                      ?? throw new UnauthorizedException("Refresh token is invalid.");

        if (!current.IsActive)
        {
            throw new UnauthorizedException("Refresh token is invalid.");
        }

        var replacement = NewRefreshToken(current.UserId);
        current.RevokedAt = DateTime.UtcNow;
        current.ReplacedByToken = replacement.Token;

        unitOfWork.RefreshTokens.Update(current);
        await unitOfWork.RefreshTokens.AddAsync(replacement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Build(current.User, replacement.Token);
    }

    public async Task<IReadOnlyList<UserResponse>> GetUsersAsync(
        CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Users.Query()
            .AsNoTracking()
            .OrderBy(x => x.UserId)
            .Select(x => new UserResponse
            {
                UserId = x.UserId,
                Username = x.Username,
                Role = x.Role
            })
            .ToListAsync(cancellationToken);
    }

    private RefreshToken NewRefreshToken(int userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64)),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)
        };
    }

    private AuthResponse Build(User user, string refreshToken)
    {
        return new AuthResponse
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = refreshToken,
            ExpiresIn = _options.AccessTokenMinutes * 60,
            User = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            }
        };
    }

    private string GenerateAccessToken(User user)
    {
        if (_options.Secret.Length < 32
            || string.IsNullOrWhiteSpace(_options.Issuer)
            || string.IsNullOrWhiteSpace(_options.Audience))
        {
            throw new InvalidOperationException("JWT configuration is invalid.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
