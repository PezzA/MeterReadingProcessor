namespace MeterReadingProcessor.Services.BatchProcessing.FileParser;
public record FileEntryParseResult(bool Success, string Narrative, RawMeterReading? RawMeterReading);