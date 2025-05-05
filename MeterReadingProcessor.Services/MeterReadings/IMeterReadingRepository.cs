using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings;
public interface IMeterReadingRepository
{
    Task<int> SaveReadingAsync(MeterReading meterReading);
    Task<MeterReading?> GetLastReadingForAccountAsync(int accountId);
    Task<bool> DoesReadingExistAsync(MeterReading meterReading);
}
