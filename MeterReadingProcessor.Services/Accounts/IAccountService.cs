namespace MeterReadingProcessor.Services.Accounts;

public interface IAccountService
{
    Task<bool> IsAccountValidAsync(int accountId);
}
