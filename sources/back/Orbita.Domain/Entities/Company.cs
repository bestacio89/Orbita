using Orbita.Domain.Common;

namespace Orbita.Domain.Entities;

/// <summary>Organisation boundary for company-owned Orbita business data.</summary>
public sealed class Company : DomainEntity
{
    private Company()
    {
    }

    private Company(string name, string key, DateTimeOffset createdAt)
    {
        Rename(name);
        Key = NormalizeKey(key);
        CreatedAt = createdAt;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string Key { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Company Create(string name, string key, DateTimeOffset createdAt) =>
        new(name, key, createdAt);

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("A company name is required.");
        }

        Name = name.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new BusinessRuleException("A company key is required.");
        }

        var normalized = key.Trim().ToUpperInvariant();
        if (normalized.Length > 64)
        {
            throw new BusinessRuleException("A company key cannot exceed 64 characters.");
        }

        return normalized;
    }
}
