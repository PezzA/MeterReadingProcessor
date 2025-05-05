var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql", port:59125)
      .WithLifetime(ContainerLifetime.Persistent);

var databaseName = "MeterReading-db";

var creationScript = $$"""
    IF DB_ID('{{databaseName}}') IS NULL
        CREATE DATABASE [{{databaseName}}];
    GO

    -- Use the database
    USE [{{databaseName}}];
    GO


    IF (NOT EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'dbo' 
    AND  TABLE_NAME = 'Accounts'))
    BEGIN
        CREATE TABLE Accounts (
            AccountId INT ,
            FirstName NVARCHAR(100),
            LastName NVARCHAR(100),
        );

        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2344,'Tommy','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2233,'Barry','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (8766,'Sally','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2345,'Jerry','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2346,'Ollie','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2347,'Tara','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2348,'Tammy','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2349,'Simon','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2350,'Colin','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2351,'Gladys','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2352,'Greg','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2353,'Tony','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2355,'Arthur','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (2356,'Craig','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (6776,'Laura','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (4534,'JOSH','TEST')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1234,'Freya','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1239,'Noddy','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1240,'Archie','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1241,'Lara','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1242,'Tim','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1243,'Graham','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1244,'Tony','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1245,'Neville','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1246,'Jo','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1247,'Jim','Test')
        INSERT INTO Accounts (AccountId, FirstName, LastName) VALUES (1248,'Pam','Test')
    END
    

    IF (NOT EXISTS (SELECT * 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'dbo' 
    AND  TABLE_NAME = 'MeterReadings'))
    BEGIN
        CREATE TABLE MeterReadings (
            Id INT IDENTITY(1,1),
            AccountId INT,
            ReadDateTime DATETIME,
            ReadingValue INT,
            Errata VARCHAR(100),
            CONSTRAINT UK_AccountId_ReadDateTime UNIQUE(AccountId, ReadDateTime)   
        );
    END
    GO
    """;

var db = sql.AddDatabase(databaseName)
            .WithCreationScript(creationScript);


var apiservice = builder.AddProject<Projects.MeterReadingProcessor_Api>("apiservice")
    .WithReference(db)
    .WaitFor(db);

apiservice
    .WithUrl($"{apiservice.GetEndpoint("https")}/swagger", "Swagger Docs");

builder.Build().Run();
