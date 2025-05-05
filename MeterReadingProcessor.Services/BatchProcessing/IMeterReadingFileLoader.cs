namespace MeterReadingProcessor.Services.BatchProcessing;

public interface IMeterReadingFileLoader
{
    public Task<FileLoadResult> LoadAsync(Stream fileStream);
}
