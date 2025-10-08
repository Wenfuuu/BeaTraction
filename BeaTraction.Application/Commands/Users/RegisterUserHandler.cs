using BeaTraction.Application.DTOs.Users.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Users;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        
        if (emailExists)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            RowVersion = 1
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
