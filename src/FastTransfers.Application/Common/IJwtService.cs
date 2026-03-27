using FastTransfers.Domain.Entities;

namespace FastTransfers.Application.Common;

public interface IJwtService
{
    string GenerateToken(User user);
}
