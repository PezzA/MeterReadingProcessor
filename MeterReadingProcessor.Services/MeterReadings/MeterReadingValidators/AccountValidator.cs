using MeterReadingProcessor.Services.Accounts;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;

public class AccountValidator(IAccountService accountService) : IMeterReadingValidator
{
    private readonly IAccountService _accountService = accountService;

    public async Task<(bool, string)> Validate(MeterReading reading)
        => await _accountService.IsAccountValidAsync(reading.AccountId)
            ? (true, string.Empty)
            : (false, "Invalid Account");
}
