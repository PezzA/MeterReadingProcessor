namespace MeterReadingProcessor.Services.BatchProcessing.FileParser;

public class CSVFileParser : IFileParser
{
    public const string Header = "AccountId,MeterReadingDateTime,MeterReadValue,";

    // Can go obviously go deeper on the checks here.  But for the purpse of this exercise assuming if that we have
    // the header line "AccountId,MeterReadingDateTime,MeterReadValue," the file is valid.
    public async Task<FileParseResult> Parse(Stream inputData)
    {
        using StreamReader reader = new(inputData);

        var rawText = await reader.ReadToEndAsync();

        var lines = rawText.Split(Environment.NewLine);

        if (lines.Length == 1)
        {
            lines = rawText.Split("\n");
        }

        if (lines[0] != Header) return new FileParseResult(false, "Invalid Header Row", []);

        if (lines.Length <= 1) return new FileParseResult(false, "No Data", []);

        var results = lines
            .Skip(1)
            .Select(GetRawReading)
            .ToList();

        return new FileParseResult(true, string.Empty, results);
    }

    private FileEntryParseResult GetRawReading(string line)
    {
        var fields = line.Split(',');

        if (fields.Length != 4)
        {
            return new FileEntryParseResult(false, "Invalid Column Count", null);
        }

        return new FileEntryParseResult(true, string.Empty, new RawMeterReading(fields[0], fields[1], fields[2], fields[3]));
    }
}
