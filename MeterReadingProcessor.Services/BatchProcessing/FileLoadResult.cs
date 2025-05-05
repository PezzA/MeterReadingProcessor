namespace MeterReadingProcessor.Services.BatchProcessing;

public record FileLoadResult(bool Success, string Narative, IList<FileEntryResult> FileEntries);
