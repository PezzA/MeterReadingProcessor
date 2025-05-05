namespace MeterReadingProcessor.Services.BatchProcessing.FileParser;

/// <summary>
///  General abstraction, could be in memory, from a file, or over the network, could be CSV, Json, Xml etc...
///  It's purpose is to understand the file format and return an intermediary result that can be further processed
/// </summary>
public interface IFileParser
{
    Task<FileParseResult> Parse(Stream inputData);
}
