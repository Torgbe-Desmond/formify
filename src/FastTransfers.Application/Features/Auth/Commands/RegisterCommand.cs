using FastTransfers.Application.Common;
using FastTransfers.Application.DTOs;
using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Exceptions;
using FastTransfers.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FastTransfers.Application.Features.Auth.Commands;

// ── Command ──────────────────────────────────────────────────────
public record RegisterCommand(string Name, string Email, string Password) : IRequest<AuthResponse>;

// ── Validator ────────────────────────────────────────────────────
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

// ── Handler ──────────────────────────────────────────────────────
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _passwords;
    private readonly IJwtService _jwt;

    public RegisterCommandHandler(IUserRepository users,
                                  IUnitOfWork uow,
                                  IPasswordService passwords,
                                  IJwtService jwt)
    {
        _users     = users;
        _uow       = uow;
        _passwords = passwords;
        _jwt       = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(request.Email, ct))
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var hash = _passwords.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash);

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwt.GenerateToken(user);

        return new AuthResponse(token, new UserDto(user.Id, user.Name, user.Email));
    }
}
