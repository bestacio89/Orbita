#nullable enable

using Orbita.Domain.Calculations.Reference.Tsp;

namespace Orbita.Persistence.Excel;

/// <summary>Reads a TSP workbook and delegates semantic interpretation to the Domain layer.</summary>
public sealed class TspWorkbookReader
{
    public TspLu Read(Stream workbookStream, string fileName, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(workbookStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        using var buffer = new MemoryStream();
        workbookStream.CopyTo(buffer);
        buffer.Position = 0;

        var workbook = XlsxWorkbook.Open(buffer);
        return InterpreteurTsp.Interpreter(workbook, fileName, today);
    }
}
