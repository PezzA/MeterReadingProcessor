using MeterReadingProcessor.Services.MeterReadings;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;

public class MeterConsistencyValidator(IMeterReadingRepository meterReadingRepository) : IMeterReadingValidator
{
    private readonly IMeterReadingRepository _meterReadingRepository = meterReadingRepository;

    public async Task<(bool, string)> Validate(MeterReading reading)
    {
        var lastReading = await _meterReadingRepository.GetLastReadingForAccountAsync(reading.AccountId);

        return lastReading == null || reading.ReadDateTime > lastReading.ReadDateTime
            ? (true, string.Empty)
            : (false, "Last reading on account is newer than current reading");
    }
}
