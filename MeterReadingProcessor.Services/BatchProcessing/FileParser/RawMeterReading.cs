namespace MeterReadingProcessor.Services.BatchProcessing.FileParser;

public record RawMeterReading(string AccountId, string ReadingDate, string ReadingValue, string Errata);

