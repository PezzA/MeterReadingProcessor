using MeterReadingProcessor.Services.BatchProcessing;
using MeterReadingProcessor.Services.BatchProcessing.FileParser;

namespace MeterReadingProcessor.Services.BatchProcessing.StaticValidation;

/// <summary>
/// Static reading validator will perform static checks ont he input data.
/// Right number of fields, valid formats etc... the sort of checks that can be performed quickly
/// and before commiting more intensive compute
/// </summary>
public interface IStaticValidator
{
    FileReadResult Validate(RawMeterReading reading);
}
