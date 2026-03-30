using Microsoft.EntityFrameworkCore;

public class TokenCleanupJob : ITokenCleanupJob
{
    private readonly AppDbContext _context;

    public TokenCleanupJob(AppDbContext context)
    {
        _context = context;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(-7);

        var tokensToDelete = await _context.RefreshTokens
            .Where(t => t.ExpiresAtUtc <= now || (t.RevokedAtUtc != null && t.RevokedAtUtc <= cutoff))
            .ToListAsync();

        if (tokensToDelete.Count == 0)
            return;

        _context.RefreshTokens.RemoveRange(tokensToDelete);
        await _context.SaveChangesAsync();
    }
}