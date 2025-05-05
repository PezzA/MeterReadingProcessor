using MeterReadingProcessor.Services.Accounts.Model;

namespace MeterReadingProcessor.Services.Accounts.Repository;

public interface IAccountRepository
{
    Task<Account?> GetAsync(int accountId);
}
