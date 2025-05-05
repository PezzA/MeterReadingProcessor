using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.BatchProcessing;

public record FileReadResult(bool Success, string ValidationError, MeterReading? MeterReading);
