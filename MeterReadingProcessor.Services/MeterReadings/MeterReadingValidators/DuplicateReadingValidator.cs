using MeterReadingProcessor.Services.MeterReadings;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;
public class DuplicateReadingValidator(IMeterReadingRepository meterReadingRepository) : IMeterReadingValidator
{
    private readonly IMeterReadingRepository _meterReadingRepository = meterReadingRepository;

    public async Task<(bool, string)> Validate(MeterReading reading)
    {
        return !await _meterReadingRepository.DoesReadingExistAsync(reading)
            ? (true, string.Empty)
            : (false, "Meter reading already exists");
    }
}
