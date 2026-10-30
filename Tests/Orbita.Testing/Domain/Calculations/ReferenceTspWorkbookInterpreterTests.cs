using Orbita.Domain.Calculations.Reference.Tsp;
using Orbita.Domain.Calculations.Reference.Common;
using Xunit;

namespace Orbita.Testing.Domain.Calculations;

public sealed class ReferenceTspWorkbookInterpreterTests
{
    [Fact]
    public void Workbook_without_required_sheets_is_rejected_with_a_business_error()
    {
        var workbook = new InMemoryWorkbook([]);

        var exception = Assert.Throws<InvalidTspException>(() =>
            InterpreteurTsp.Interpreter(workbook, "missing-sheets.xlsx", new DateOnly(2026, 10, 30)));

        Assert.Contains("WORKLOAD", exception.Message);
        Assert.Contains("PROFIL", exception.Message);
    }

    private sealed class InMemoryWorkbook(IReadOnlyList<string> sheetNames) : IClasseur
    {
        public IReadOnlyList<string> NomsFeuilles { get; } = sheetNames;

        public IFeuille? Feuille(string nom) => null;
    }
}
