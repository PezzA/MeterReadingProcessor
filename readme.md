# MeterReadingProcessor

## Summary
A .NET 9 app that will handle the loading of a CSV file containing meter readings and validating against a preset list of accounts.  The initial entry point for processing of a file will be a REST based API, but the core of the logic shouldn't care about the web.  It should be equally able to load a file off disk via a CLI or maybe off a network share for larger sets of batch processing.

## Assumptions / Notes
- Want to separate parsing the CSV file from the processing of readings.
- Spec contains both static and data/network based validations.  Perform static validations first before data validations?
- From the acceptance criteria notes
	- You should not be able to load the same entry twice
		Given than an account can have more than one read, taking this to mean that you cannot load 2 meter readings for the same account with the same timestamp.   This should be checked before saving a record, but have also added a DB constraint to guard against race conditions.
	- Reading values should be in the format NNNNN.  
	    Have assumed this to mean the raw value on the file should be 5 numeric digits.  So `012345` is valid, but `1234` is not.
- Want to demonstrate SOLID principles so code is designed keeping likely possible future extension in mind.
- 
## Dependencies
I've used [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) as the development environment container here. Mainly as it has a good story for setting up dev environments in local containers. To run the aspire project you'll need to have either Docker Desktop or Podman running locally.  Aspire will then create the test SQL container and run the SQL setup steps, which includes schema and seed test data creation.  It then takes the sql connection string and injects this as an environment variable to the api project.

With more time I might have used Entity Framework and to setup the DB with migration scripts and have that run off the main api projects.  Haven't used EF too much in the past few years, but hear that is in a good place at the moment.  At the moment SQL access is done using [Dapper](https://github.com/DapperLib/Dapper).

I would also have liked to have run some examples of observability factes running in the aspire app, logging, tracing etc...

## Other Notes
There is lots of success outcome handling in this, and mostly fought with nullable reference types which was constantly adding green ink.  A few approaches in the file, return a non nullable type and then use exceptions for non sucessful parsing (even through this is not strictly speaking exceptional), or parse back a result type and handle the null propegation and required type narrowing.  Having used discriminated unions in a few other langugates, I so wanted to have a type that was something like `type ReadingResult = MeterReading | ValidationError`.  Hopefully [one day soon](https://github.com/dotnet/csharplang/blob/main/proposals/TypeUnions.md#standard---union-classes) we can!

## Testing Notes
I've included mainly unit tests around some of the more static/pure functions that take an input and return a particular results.  Even in the production of this demo they have been very useful when refactoring.

The one area I've not been able to get to are the end to end tests.  I've have used this demo to try some code examples with aspire project and is the first project I've setup this way.  If I more time I would try use a combination of [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) and [Xunit.Gherkin.Quick](https://github.com/ttutisani/Xunit.Gherkin.Quick) to produce end to end tests based around acceptance criteria in [Gherkin](https://cucumber.io/docs/gherkin/) syntax.

Looking at the example test process [here](https://learn.microsoft.com/en-us/dotnet/aspire/testing/write-your-first-test?pivots=xunit#explore-the-test-project), you could setup BDD tests along the lines of
```gherkin
GIVEN I am using the /meter-reading-upload api
WHEN I load the sample meter reading file
THEN 4 records should be loaded and 31 should fail to load
```
This can produce higher level functional tests that don't care too much about implementaion and should be a stable set of tests even if the internal implementatio changes wildly.

## SOLID analysis
Some note on SOLID as it applies to this demo projects.

### Single Responsibility
Of all the code here,  `IMeterReadingFileLoader.LoadAsync` is probably the largest code block in the example, but this is an orchestration method, and is conceptually only involved with the logic required to load a file, parsing and per record processing.  This would only change if we needed to change how we load a file (e.g. logging of batch processing, event publishing) so reasonbly happy this is 'doing one thing'.  I'm not totally happy with the validation of the individual lines in here, possible that needed a separate abstraction. 

### Open/Close
There are a few examples of the open/close principle in this code base.  First the fact this this is loading a CSV is abstracted by the `IFileParser` interface.  Implementations could be written for XML or JSON, YAML without changing the existing file loading logic.  Similarly the use of the `Steam` abstract class means that we don't need to load this form the web, it could be from a file, in memory or network call.

Also, additional validations can be added to the file processing by adding more implementations of `IMeterReadingValidator` in the composite root.

### Liskov Substitution Principle
Not much base class inheritance going on here, but in the test class a `Stream` subclass `MemoryClass` is passed in, not my abstraction of course, but the only direct example I have in the code.

### Interface Segregation
The file abstraction methods have been kept small and high level `Load` , `Validate` etc...  they do not require the consumers of this interface to take any dependencies they would not other wise need.   The DB repository classes also do not require any specifics of the DB implementation and could be swapped out with other DB implementations.  Although being repository classes, the abstractions here are weak, and mainly for basic polymorphic behaviour/mocking.

### Dependency Inversion
The code mostly relies on abstractions and interfaces, and does not new up or take concrete implementations as dependencies.  All the dependencies are modelled and setup in the application composition root.
