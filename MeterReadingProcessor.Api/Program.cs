using MeterReadingProcessor.Services.Accounts;
using MeterReadingProcessor.Services.Accounts.Repository;
using MeterReadingProcessor.Services.BatchProcessing;
using MeterReadingProcessor.Services.BatchProcessing.FileParser;
using MeterReadingProcessor.Services.BatchProcessing.StaticValidation;
using MeterReadingProcessor.Services.MeterReadings;
using MeterReadingProcessor.Services.MeterReadings.MeterReadingValidators;

using MeterReadingProcessor.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IAccountService, AccountService>();
builder.Services.AddSingleton<IFileParser, CSVFileParser>();
builder.Services.AddSingleton<IMeterReadingFileLoader, MeterReadingFileLoader>();

builder.Services.AddSingleton<IStaticValidator, BasicStaticValidator>();

// this validations will fire in the order added to the DI container.
builder.Services.AddSingleton<IMeterReadingValidator, AccountValidator>();
builder.Services.AddSingleton<IMeterReadingValidator, DuplicateReadingValidator>();
builder.Services.AddSingleton<IMeterReadingValidator, MeterConsistencyValidator>();

var dbConnectionString = builder.Configuration.GetConnectionString("MeterReading-db") 
    ?? throw new Exception("No meter reading db specifed");

builder.Services.AddSingleton<IAccountRepository>(new SqlAccountRespository(dbConnectionString));
builder.Services.AddSingleton<IMeterReadingRepository>(new SqlMeterReadingRepository(dbConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.MapPost("/meter-reading-uploads", async (HttpRequest request, Stream body, IMeterReadingFileLoader meterReadingFileLoader) =>
{
    // Hardcoded value to limit to request of 1MB, this could be better off on the edge, WAF etc..
    // but is provided as an example of validation that might specifically be applied to web, others could be auth etc..
    if (request.ContentLength != null && request.ContentLength > 1_048_576)
    {
        return Results.BadRequest("Request too large");
    }

    FileLoadResult loadedFile = await meterReadingFileLoader.LoadAsync(body);

    if (!loadedFile.Success)
    { 
        return Results.BadRequest(loadedFile.Narative);
    }

    // O(2n), but just for example
    var results = new LoadResults(loadedFile.FileEntries.Count(e => e.Success), loadedFile.FileEntries.Count(e => !e.Success));

    return Results.Ok(loadedFile);
});

app.UseHttpsRedirection();


app.Run();
