#nullable enable

using Orbita.Domain.Calculations.Reference.Tsp;
using Orbita.Domain.Tsp;

namespace Orbita.Persistence.Excel;

/// <summary>Reads a TSP workbook and delegates semantic interpretation to the Domain layer.</summary>
public sealed class TspWorkbookReader
{
    public TspReadModel Read(Stream workbookStream, string fileName, DateOnly today) =>
        TspReadModelMapper.Map(ReadReference(workbookStream, fileName, today));

    public TspLu ReadReference(Stream workbookStream, string fileName, DateOnly today)
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
