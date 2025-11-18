using Microsoft.EntityFrameworkCore;
using PTJ.Application.Common;
using PTJ.Application.DTOs.Auth;
using PTJ.Application.Services;
using PTJ.Domain.Entities;
using PTJ.Domain.Interfaces;

namespace PTJ.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Validate password confirmation
        if (dto.Password != dto.ConfirmPassword)
        {
            return Result<AuthResponseDto>.FailureResult("Passwords do not match");
        }

        // Check if email already exists
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Email == dto.Email,
            cancellationToken);

        if (existingUser != null)
        {
            return Result<AuthResponseDto>.FailureResult("Email already exists");
        }

        // Check if role exists
        var role = await _unitOfWork.Roles.GetByIdAsync(dto.RoleId, cancellationToken);
        if (role == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid role");
        }

        // Create user
        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            IsEmailVerified = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Assign role
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = dto.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRoles.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create Profile or Company based on role
        if (role.Name == "STUDENT")
        {
            var profile = new Profile
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Profiles.AddAsync(profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Generate tokens
        var roles = new List<string> { role.Name };
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = "0.0.0.0",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Registration successful");
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, string ipAddress, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(
            u => u.Email == dto.Email,
            cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result<AuthResponseDto>.FailureResult("Account is deactivated");
        }

        // Get user roles
        var userRoles = await _unitOfWork.UserRoles.FindAsync(
            ur => ur.UserId == user.Id,
            cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = new List<string>();

        foreach (var roleId in roleIds)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
            if (role != null)
            {
                roles.Add(role.Name);
            }
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == refreshToken,
            cancellationToken);

        if (token == null || !token.IsActive)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid refresh token");
        }

        // Get user
        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return Result<AuthResponseDto>.FailureResult("User not found or inactive");
        }

        // Get user roles
        var userRoles = await _unitOfWork.UserRoles.FindAsync(
            ur => ur.UserId == user.Id,
            cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = new List<string>();

        foreach (var roleId in roleIds)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
            if (role != null)
            {
                roles.Add(role.Name);
            }
        }

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke old token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReplacedByToken = newRefreshToken;
        _unitOfWork.RefreshTokens.Update(token);

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Token refreshed successfully");
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == refreshToken,
            cancellationToken);

        if (token == null || !token.IsActive)
        {
            return Result.FailureResult("Invalid refresh token");
        }

        // Revoke token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        _unitOfWork.RefreshTokens.Update(token);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Token revoked successfully");
    }
}
