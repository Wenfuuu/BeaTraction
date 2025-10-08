namespace BeaTraction.Application.DTOs.Users.Response;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}
