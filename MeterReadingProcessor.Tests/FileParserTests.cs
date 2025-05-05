using System.Text;

using MeterReadingProcessor.Services.BatchProcessing.FileParser;

namespace MeterReadingProcessor.Tests;

public class FileParserTests
{
    private const string IncorrectHeader = "xxxxx";

    private const string DataBody = @"
2344,22/04/2019 09:24,1002,
2233,22/04/2019 12:25,323,
";

    [Theory]
    [InlineData(IncorrectHeader, DataBody, false)]
    [InlineData(CSVFileParser.Header, DataBody, true)]
    [InlineData(CSVFileParser.Header, "", false)]
    public async Task CSVFileParser_DetectsCSV(string header, string data, bool expectedResult)
    {
        IFileParser parser = new CSVFileParser();

        var testFileData = Encoding.ASCII.GetBytes(header + data);
        var testStream = new MemoryStream(testFileData);

        var result = await parser.Parse(testStream);

        Assert.Equal(expectedResult, result.Success);
    }
}
