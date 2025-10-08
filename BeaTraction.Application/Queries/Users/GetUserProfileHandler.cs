using BeaTraction.Application.DTOs.Users.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Users;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

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
