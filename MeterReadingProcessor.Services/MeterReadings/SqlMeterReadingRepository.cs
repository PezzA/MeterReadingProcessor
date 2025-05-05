using Microsoft.Data.SqlClient;
using Dapper;
using MeterReadingProcessor.Services.MeterReadings.Model;

namespace MeterReadingProcessor.Services.MeterReadings;

public class SqlMeterReadingRepository(string connectionString) : IMeterReadingRepository
{
    private readonly string _connectionString = connectionString;

    public async Task<bool> DoesReadingExistAsync(MeterReading meterReading)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        return await connection.ExecuteScalarAsync<bool>(@"
        SELECT 
            COUNT(AccountId) 
        FROM 
            MeterReadings 
        WHERE 
            AccountId = @accountId 
        AND 
            ReadDateTime = @readDateTime", new { meterReading.AccountId, meterReading.ReadDateTime });
    }

    public async Task<MeterReading?> GetLastReadingForAccountAsync(int accountId)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<MeterReading>(@"
    SELECT
        TOP 1
        AccountId,
        ReadDateTime,
        ReadingValue,
        Errata
    FROM
        MeterReadings
    WHERE
        AccountId = @accountId
    ORDER BY
        ReadDateTime DESC", new { accountId });

    }

    public async Task<int> SaveReadingAsync(MeterReading meterReading)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var readingId = await connection.ExecuteScalarAsync<int>(@"
        INSERT INTO MeterReadings
            (AccountId, ReadDateTime, ReadingValue, Errata)
        VALUES
            (@accountId, @readDateTime, @readingValue, @errata)

        SELECT SCOPE_IDENTITY();", meterReading);

        return readingId;
    }
}
