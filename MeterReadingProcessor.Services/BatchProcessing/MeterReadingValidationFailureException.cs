namespace MeterReadingProcessor.Services.BatchProcessing;

public partial class MeterReadingFileLoader
{
    private class MeterReadingValidationFailureException : Exception
    {
        public MeterReadingValidationFailureException()
        {
        }

        public MeterReadingValidationFailureException(string? message) : base(message)
        {
        }

        public MeterReadingValidationFailureException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
