using System.Text.RegularExpressions;

namespace Orbita.Domain.Common;

/// <summary>
/// Produces a stable uniqueness key for a commercial project.
/// Opportunity number has precedence, followed by normalized proposal reference,
/// then normalized client and project name.
/// </summary>
public static partial class ProjectIdentity
{
    public static string Create(string? reference, string? client, string? projectName)
    {
        if (TryGetOpportunityNumber(reference) is { } opportunityNumber)
        {
            return $"OP:{opportunityNumber}";
        }

        var normalizedReference = Simplify(ProposalVersion().Replace(reference ?? string.Empty, string.Empty));
        if (normalizedReference.Length > 0)
        {
            return Truncate($"REF:{normalizedReference}");
        }

        return Truncate($"NAME:{Simplify(client)}|{Simplify(projectName)}");
    }

    public static string? TryGetOpportunityNumber(string? reference)
    {
        var match = OpportunityNumber().Match(reference ?? string.Empty);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static string? TryGetProposalVersion(string? reference)
    {
        var match = ProposalVersion().Match(reference ?? string.Empty);
        return match.Success ? $"V{match.Groups[1].Value}" : null;
    }

    public static string Simplify(string? value) =>
        Whitespace().Replace(NonAlphanumeric().Replace(TextNormalizer.Normalize(value), " "), " ")
            .Trim()
            .ToUpperInvariant();

    private static string Truncate(string value) => value.Length <= 200 ? value : value[..200];

    [GeneratedRegex(@"(?<![\p{L}\p{N}])OP\s*[-_ ]?\s*(\d{3,})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex OpportunityNumber();

    [GeneratedRegex(@"[\s\-_]*\bV\s*(\d+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ProposalVersion();

    [GeneratedRegex(@"[^\p{L}\p{N}]+", RegexOptions.CultureInvariant)]
    private static partial Regex NonAlphanumeric();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex Whitespace();
}
