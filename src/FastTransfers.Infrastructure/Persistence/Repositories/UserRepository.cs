using FastTransfers.Domain.Entities;
using FastTransfers.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastTransfers.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => _db.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
            => await _db.Users.AddAsync(user, ct);

        public void Update(User user)
            => _db.Users.Update(user);
    }
}
