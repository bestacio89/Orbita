using Orbita.Domain.Common;
using Orbita.Domain.Entities;

namespace Orbita.Domain.Services;

/// <summary>Enforces the one-person-day-per-calendar-day rule, except for purchase and assignment entities.</summary>
public static class DailyEffortPolicy
{
    public static void EnsureWithinDailyLimit(ProjectProfile profile, decimal totalForDay)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (totalForDay < 0)
        {
            throw new BusinessRuleException("Recorded effort cannot be negative.");
        }

        if (profile.LegalEntity.StartsWith("ACHAT", StringComparison.OrdinalIgnoreCase) ||
            profile.LegalEntity.StartsWith("CESSION", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (totalForDay > 1m)
        {
            throw new BusinessRuleException(
                $"{profile.Name} cannot exceed one person-day on the same day across all tasks ({totalForDay:0.##} recorded).");
        }
    }
}
