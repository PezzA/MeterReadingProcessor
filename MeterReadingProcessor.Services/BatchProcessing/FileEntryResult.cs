using MeterReadingProcessor.Services.BatchProcessing.FileParser;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.BatchProcessing;

public record FileEntryResult(bool Success, string Narrative, MeterReading? Reading, RawMeterReading? RawReading);
