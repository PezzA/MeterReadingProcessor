using MeterReadingProcessor.Services.Accounts.Repository;

namespace MeterReadingProcessor.Services.Accounts;

public class AccountService(IAccountRepository accountRepository) : IAccountService
{
    private readonly IAccountRepository _accountRepository = accountRepository;

    // Something of a ghost method at the moment, but account validation might involve other checks
    // as well.
    public async Task<bool> IsAccountValidAsync(int accountId)
        => await _accountRepository.GetAsync(accountId) is not null;
}
