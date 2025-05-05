namespace MeterReadingProcessor.Services.MeterReadings.Model;

public record MeterReading(int AccountId, DateTime ReadDateTime, int ReadingValue, string Errata);
