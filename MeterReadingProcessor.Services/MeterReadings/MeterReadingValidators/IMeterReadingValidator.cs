using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;

public interface IMeterReadingValidator
{
    Task<(bool, string)> Validate(MeterReading reading);
}
