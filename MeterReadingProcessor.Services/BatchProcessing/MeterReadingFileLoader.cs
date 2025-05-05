using System.ComponentModel.DataAnnotations;

using MeterReadingProcessor.Services.BatchProcessing.FileParser;
using MeterReadingProcessor.Services.BatchProcessing.StaticValidation;
using MeterReadingProcessor.Services.MeterReadings;
using MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.BatchProcessing;

public partial class MeterReadingFileLoader(
    IFileParser fileParser,
    IStaticValidator staticValidator,
    IEnumerable<IMeterReadingValidator> meterReadingValidators,
    IMeterReadingRepository meterReadingRepository) : IMeterReadingFileLoader
{
    private readonly IFileParser _fileParser = fileParser;
    private readonly IStaticValidator _staticValidator = staticValidator;
    private readonly IEnumerable<IMeterReadingValidator> _meterReadingValidators = meterReadingValidators;
    private readonly IMeterReadingRepository _meterReadingRepository = meterReadingRepository;

    public async Task<FileLoadResult> LoadAsync(Stream fileStream)
    {
        FileParseResult parsedFile;

        try
        {
            parsedFile = await _fileParser.Parse(fileStream);

            if (!parsedFile.Success)
            {
                return new FileLoadResult(false, parsedFile.ValidationError, []);
            }
        }
        catch (Exception exception)
        {
            // TODO - Log exception
            throw;
        }

        var entries = new List<FileEntryResult>();


        foreach (FileEntryParseResult entryParseResult in parsedFile.RawReadings)
        {
            if (entryParseResult?.RawMeterReading == null)
            {
                entries.Add(new FileEntryResult(false,entryParseResult?.Narrative ?? "Unknown parsing error", null, null));
                continue;
            }

            try
            {
                var meterReading = await ValidateRawMeterReading(entryParseResult.RawMeterReading);
                var readingId = await _meterReadingRepository.SaveReadingAsync(meterReading);
                entries.Add(new FileEntryResult(true, $"Reading Saved with ID - {readingId}", meterReading, entryParseResult.RawMeterReading));
            }
            catch (MeterReadingValidationFailureException validationException)
            {
                entries.Add(new FileEntryResult(false, $"Problem validating reading: {validationException.Message}" , null, entryParseResult.RawMeterReading));
                continue;
            }
            catch (Exception exception)
            {
                // TODO - Log Exception
                entries.Add(new FileEntryResult(false, $"Unknown problem occured: {exception.Message}", null, entryParseResult.RawMeterReading));
                continue;
            }
        }

        return new FileLoadResult(true, "File Loaded Successfully", entries);
    }


    // Using exception handling here to either return a non nun
    private async Task<MeterReading> ValidateRawMeterReading(RawMeterReading rawMeterReading)
    { 
        var checkedReading = _staticValidator.Validate(rawMeterReading);

        if (!checkedReading.Success)
        {
            throw new MeterReadingValidationFailureException($"Static Validation Failure: {checkedReading.ValidationError}");
        }

        var meterReading = checkedReading.MeterReading ?? throw new MeterReadingValidationFailureException($"Null meter reading from static validator");

        // Could run these concurrently
        foreach (IMeterReadingValidator validator in _meterReadingValidators)
        {
            (bool success, string narrative) = await validator.Validate(meterReading);

            if (!success)
            {
                throw new MeterReadingValidationFailureException($"{typeof(Validator)} Validation Failure: {narrative}");
            }
        }

        return meterReading;
    }
}
