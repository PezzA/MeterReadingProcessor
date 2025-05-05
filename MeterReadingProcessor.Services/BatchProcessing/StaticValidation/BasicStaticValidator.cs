using System.Text.RegularExpressions;

using MeterReadingProcessor.Services.BatchProcessing;
using MeterReadingProcessor.Services.BatchProcessing.FileParser;
using MeterReadingProcessor.Services.BatchProcessing.StaticValidation;

using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.BatchProcessing.StaticValidation;

public partial class BasicStaticValidator : IStaticValidator
{
    [GeneratedRegex("^\\d{5}$")]
    private static partial Regex ReadingValidator();

    public FileReadResult Validate(RawMeterReading rawReading)
    {
        if (!int.TryParse(rawReading.AccountId, out int accountId))
        {
            return new FileReadResult(false, "Invalid AccountId", null);
        }

        if (!DateTime.TryParse(rawReading.ReadingDate, out DateTime readDateTime))
        {
            return new FileReadResult(false, "Read date could not be parsed", null);
        }

        if (!ReadingValidator().IsMatch(rawReading.ReadingValue) || !int.TryParse(rawReading.ReadingValue, out int reading))
        {
            return new FileReadResult(false, "Reading was not a 5 digit value", null);
        }

        return new FileReadResult(true, string.Empty, new MeterReading(accountId, readDateTime, reading, rawReading.Errata));
    }
}
