public interface ITokenCleanupJob
{
    Task CleanupExpiredTokensAsync();
}