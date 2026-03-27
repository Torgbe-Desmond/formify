using FastTransfers.Application.Common;
using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Auth.Commands;

// ── Command ──────────────────────────────────────────────────────
public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

// ── Validator ────────────────────────────────────────────────────
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

// ── Handler ──────────────────────────────────────────────────────
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordService _passwords;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IUserRepository users,
                               IPasswordService passwords,
                               IJwtService jwt)
    {
        _users     = users;
        _passwords = passwords;
        _jwt       = jwt;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct)
            ?? throw new NotFoundException("Invalid email or password.");

        if (!_passwords.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedDomainException("Invalid email or password.");

        var token = _jwt.GenerateToken(user);

        return new AuthResponse(token, new UserDto(user.Id, user.Name, user.Email));
    }
}
