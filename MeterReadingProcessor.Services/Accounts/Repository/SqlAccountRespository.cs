using Dapper;

using MeterReadingProcessor.Services.Accounts.Model;

using Microsoft.Data.SqlClient;

namespace MeterReadingProcessor.Services.Accounts.Repository;

public class SqlAccountRespository(string connectionString) : IAccountRepository
{
    private readonly string _connectionString = connectionString;

    public async Task<Account?> GetAsync(int accountId)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<Account>(@"
    SELECT
        AccountId,
        FirstName,
        LastName
    FROM
        Accounts
    WHERE
        AccountId = @accountId", new { accountId });
    }
}
