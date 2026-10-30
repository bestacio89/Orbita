namespace Orbita.Domain.Common;

/// <summary>Stable matching key for a person name while no account/profile link exists.</summary>
public static class PersonIdentity
{
    public static string CreateKey(string? name)
    {
        var words = TextNormalizer.Normalize(name)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Array.Sort(words, StringComparer.Ordinal);
        return string.Join(' ', words);
    }
}
