namespace ECommerce.Modules.Identity.Domain.Entities;

public sealed class RefreshToken
{
    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, string? createdByIp, Guid familyId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        CreatedByIp = createdByIp;
        FamilyId = familyId;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public string? RevokedByIp { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    public ApplicationUser User { get; private set; } = null!;
    public byte[] RowVersion { get; private set; } = null!;

    public Guid FamilyId { get; private set; }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsActive(DateTime utcNow) => !IsRevoked && !IsExpired(utcNow);

    public bool WasReplaced => ReplacedByTokenId.HasValue;

    public void Revoke(DateTime utcNow, string? revokedByIp)
    {
        if (IsRevoked)
            return;

        RevokedAt = utcNow;
        RevokedByIp = revokedByIp;
    }

    public void Replace(Guid replacementTokenId, DateTime utcNow, string? revokedByIp)
    {
        ReplacedByTokenId = replacementTokenId;

        Revoke(utcNow, revokedByIp);
    }
}