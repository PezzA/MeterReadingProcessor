namespace MeterReadingProcessor.Services.BatchProcessing.FileParser;

public record FileParseResult(bool Success, string ValidationError, IList<FileEntryParseResult> RawReadings);