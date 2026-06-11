using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.UnitOfWork;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Services.Options;

namespace PRN232.LMS.Services.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        User user = await unitOfWork.Users.Query()
            .SingleOrDefaultAsync(item => item.Username.ToLower() == request.Username.Trim().ToLower(), cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid username or password.");

        PasswordVerificationResult verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        RefreshToken refreshToken = CreateRefreshToken(user.UserId);
        await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user, refreshToken.Token);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        RefreshToken existingRefreshToken = await unitOfWork.RefreshTokens.Query()
            .Include(refreshToken => refreshToken.User)
            .SingleOrDefaultAsync(refreshToken => refreshToken.Token == request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Refresh token is invalid.");

        if (!existingRefreshToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid.");
        }

        RefreshToken newRefreshToken = CreateRefreshToken(existingRefreshToken.UserId);
        existingRefreshToken.RevokedAt = DateTime.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshToken.Token;

        unitOfWork.RefreshTokens.Update(existingRefreshToken);
        await unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(existingRefreshToken.User, newRefreshToken.Token);
    }

    public async Task<IReadOnlyList<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Users.Query()
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .Select(user => new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            })
            .ToListAsync(cancellationToken);
    }

    private AuthResponse BuildAuthResponse(User user, string refreshToken)
    {
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        string accessToken = GenerateAccessToken(user, expiresAt);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtOptions.AccessTokenMinutes * 60,
            User = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            }
        };
    }

    private string GenerateAccessToken(User user, DateTime expiresAt)
    {
        ValidateJwtOptions();

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken CreateRefreshToken(int userId)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken
        {
            UserId = userId,
            Token = Base64UrlEncoder.Encode(randomBytes),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        };
    }

    private void ValidateJwtOptions()
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Secret) || _jwtOptions.Secret.Length < 32)
        {
            throw new InvalidOperationException("JWT secret must be configured and at least 32 characters long.");
        }

        if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer) || string.IsNullOrWhiteSpace(_jwtOptions.Audience))
        {
            throw new InvalidOperationException("JWT issuer and audience must be configured.");
        }
    }
}
