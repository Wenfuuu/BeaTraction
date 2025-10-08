namespace BeaTraction.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string email, string role);
    string? ValidateToken(string token);
}
