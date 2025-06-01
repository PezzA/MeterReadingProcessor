using MeterReadingProcessor.Services.BatchProcessing.StaticValidation;

using MeterReadingProcessor.Services.BatchProcessing.FileParser;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Tests;
public class StaticValidationTests
{
    [Theory]
    // last field is not a 5 digit number
    [InlineData("2345", "22/04/2019 12:25", "5522", false)]
    [InlineData("2345xx", "22/04/2019 12:25", "55222", false)]
    [InlineData("2345", "22/04/201934 12:25", "55222", false)]
    [InlineData("2345", "22/04/2019 12:25", "55223", true)]

    public void BasicFormatValidator_ValidatesCorrectly(string accountNumber, string readDateTime, string readValue, bool expectedresult)
    {
        IStaticValidator validator = new BasicStaticValidator();

        var validationResult = validator.Validate(new RawMeterReading(accountNumber, readDateTime, readValue, string.Empty));


        Assert.Equal(expectedresult, validationResult.Success);
    }

    [Fact]
    public void BasicFormatValidator_ParsesCorrectly()
    {
        var testData = new RawMeterReading("6776", "10/05/2019 09:24", "23566", "X");

        IStaticValidator validator = new BasicStaticValidator();

        var result = validator.Validate(testData);

        Assert.True(result.Success);
        Assert.Equal(new MeterReading(6776, new DateTime(2019, 05, 10, 09, 24, 0), 23566, "X"), result.MeterReading);
    }
}
