using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using DatabaseBenchmarking.Benchmarking.Helpers;
using DatabaseBenchmarking.Benchmarking.Models;
using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Entities.MongoDB;
using DatabaseBenchmarking.DBConnection.Extensions;
using DatabaseBenchmarking.DBConnection.Helpers;
using DatabaseBenchmarking.DBConnection.Repository;
using DatabaseBenchmarking.DBConnection.Repository.Interfaces;
using DatabaseBenchmarking.DBConnection.Services.Factories;
using Microsoft.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using Npgsql;

namespace DatabaseBenchmarking.Benchmarking;

[Config(typeof(CustomBenchmarkingConfig))]
[SimpleJob(iterationCount: Constants.BenchmarkingSettings.IterationCount, warmupCount: Constants.BenchmarkingSettings.WarmupCount, launchCount: Constants.BenchmarkingSettings.LaunchCount, runtimeMoniker: RuntimeMoniker.Net60)]
//[SimpleJob(iterationCount: Constants.BenchmarkingSettings.IterationCount, warmupCount: Constants.BenchmarkingSettings.WarmupCount, launchCount: Constants.BenchmarkingSettings.LaunchCount, runtimeMoniker: RuntimeMoniker.Net70)]
[SimpleJob(iterationCount: Constants.BenchmarkingSettings.IterationCount, warmupCount: Constants.BenchmarkingSettings.WarmupCount, launchCount: Constants.BenchmarkingSettings.LaunchCount, runtimeMoniker: RuntimeMoniker.Net80)]
public class StudentRepositoryBenchmark
{
    #region Private Fields

    private readonly IEntityFrameworkRepository<Student> entityFrameworkSqlServerRepository = 
        new EntityFrameworkRepository<Student>(new SqlServerDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<Student> entityFrameworkPostgreSqlRepository = 
        new EntityFrameworkRepository<Student>(new PostgreSqlDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<Student> entityFrameworkMySqlRepository =
        new EntityFrameworkRepository<Student>(new MySqlDatabaseContextFactory());

    private readonly IDapperRepository<Student> dapperSqlServerRepository = 
        new DapperRepository<Student>(new SqlConnection(ConnectionStringService.SqlServerConnectionString));

    private readonly IDapperRepository<Student> dapperPostgreSqlRepository = 
        new DapperRepository<Student>(new NpgsqlConnection(ConnectionStringService.PostgreSqlConnectionString));

    private readonly IDapperRepository<Student> dapperMySqlRepository =
        new DapperRepository<Student>(new MySqlConnection(ConnectionStringService.MySqlConnectionString));

    private readonly IEntityFrameworkRepository<StudentInserted> entityFrameworkSqlServerInsertedRepository =
        new EntityFrameworkRepository<StudentInserted>(new SqlServerDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<StudentInserted> entityFrameworkPostgreSqlInsertedRepository =
        new EntityFrameworkRepository<StudentInserted>(new PostgreSqlDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<StudentInserted> entityFrameworkMySqlInsertedRepository =
        new EntityFrameworkRepository<StudentInserted>(new MySqlDatabaseContextFactory());

    private readonly IDapperRepository<StudentInserted> dapperSqlServerInsertedRepository =
        new DapperRepository<StudentInserted>(new SqlConnection(ConnectionStringService.SqlServerConnectionString));

    private readonly IDapperRepository<StudentInserted> dapperPostgreSqlInsertedRepository =
        new DapperRepository<StudentInserted>(new NpgsqlConnection(ConnectionStringService.PostgreSqlConnectionString));

    private readonly IDapperRepository<StudentInserted> dapperMySqlInsertedRepository =
        new DapperRepository<StudentInserted>(new MySqlConnection(ConnectionStringService.MySqlConnectionString));

    private readonly IEntityFrameworkRepository<Theme> entityFrameworkSqlServerThemesRepository =
    new EntityFrameworkRepository<Theme>(new SqlServerDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<Theme> entityFrameworkPostgreSqlThemesRepository =
        new EntityFrameworkRepository<Theme>(new PostgreSqlDatabaseContextFactory());

    private readonly IEntityFrameworkRepository<Theme> entityFrameworkMySqlThemesRepository =
        new EntityFrameworkRepository<Theme>(new MySqlDatabaseContextFactory());

    private readonly IDapperRepository<Theme> dapperSqlServerThemesRepository =
        new DapperRepository<Theme>(new SqlConnection(ConnectionStringService.SqlServerConnectionString));

    private readonly IDapperRepository<Theme> dapperPostgreSqlThemesRepository =
        new DapperRepository<Theme>(new NpgsqlConnection(ConnectionStringService.PostgreSqlConnectionString));

    private readonly IDapperRepository<Theme> dapperMySqlThemesRepository =
        new DapperRepository<Theme>(new MySqlConnection(ConnectionStringService.MySqlConnectionString));

    private readonly MongoDbRepository<MongoDbStudent> mongoDbStudentRepository = new MongoDbRepository<MongoDbStudent>();
    private readonly MongoDbRepository<MongoDbStudentInserted> mongoDbStudentInsertedRepository = new MongoDbRepository<MongoDbStudentInserted>();
    private readonly MongoDbRepository<MongoDbTheme> mongoDbThemeRepository = new MongoDbRepository<MongoDbTheme>();

    private MongoDbStudentInserted mongoDbStudentInserted = new MongoDbStudentInserted ();

    private readonly Consumer consumer = new Consumer();
    private readonly Random random = new Random();

    private string? personEmail;
    private string? personPassword;
    private string? personNumber;
    private bool? isNotifiable;

    #endregion Private Fields

    #region Global Methods

    [GlobalSetup]
    public async Task SetUp()
    {
        await mongoDbStudentInsertedRepository.CreateCollectionIfNotExistsAsync();
        await mongoDbStudentRepository.CreateCollectionIfNotExistsAsync();
        await mongoDbThemeRepository.CreateCollectionIfNotExistsAsync();

        long mongoDbStudentsInsertedCount = await mongoDbStudentInsertedRepository.CountAsync();
        long mongoDbThemesCount = await mongoDbThemeRepository.CountAsync();

        await SeedMongoDbStudentsInserted(mongoDbStudentsInsertedCount);
        mongoDbStudentInserted = await mongoDbStudentInsertedRepository.SelectAsync(Builders<MongoDbStudentInserted>.Filter.Empty);
        await SeedMongoDbThemes(mongoDbThemesCount);

        int sqlServerStudentsInsertedCount = await entityFrameworkSqlServerInsertedRepository.CountAsync(x => true);
        int postgreSqlStudentsInsertedCount = await entityFrameworkPostgreSqlInsertedRepository.CountAsync(x => true);
        int mySqlStudentsInsertedCount = await entityFrameworkMySqlInsertedRepository.CountAsync(x => true);
        await SeedSqlStudentsInserted(entityFrameworkSqlServerInsertedRepository, sqlServerStudentsInsertedCount);
        await SeedSqlStudentsInserted(entityFrameworkPostgreSqlInsertedRepository, postgreSqlStudentsInsertedCount);
        await SeedSqlStudentsInserted(entityFrameworkMySqlInsertedRepository, mySqlStudentsInsertedCount);

        StudentInserted? sqlServerStudentInserted = await entityFrameworkSqlServerInsertedRepository.SelectAsync(x => true);
        StudentInserted? postgreSqlStudentInserted = await entityFrameworkPostgreSqlInsertedRepository.SelectAsync(x => true);
        StudentInserted? mySqlStudentInserted = await entityFrameworkMySqlInsertedRepository.SelectAsync(x => true);

        int sqlServerThemesCount = await entityFrameworkSqlServerThemesRepository.CountAsync(x => true);
        int postgreSqlThemesCount = await entityFrameworkPostgreSqlThemesRepository.CountAsync(x => true);
        int mySqlThemesCount = await entityFrameworkMySqlThemesRepository.CountAsync(x => true);
        await SeedSqlThemes(entityFrameworkSqlServerThemesRepository, sqlServerStudentInserted!, sqlServerThemesCount);
        await SeedSqlThemes(entityFrameworkPostgreSqlThemesRepository, postgreSqlStudentInserted!, postgreSqlThemesCount);
        await SeedSqlThemes(entityFrameworkMySqlThemesRepository, mySqlStudentInserted!, mySqlThemesCount);

    }

    [GlobalCleanup]
    public async Task CleanUp()
    {
        await entityFrameworkSqlServerRepository.DeleteAllAsync();
        await dapperPostgreSqlRepository.DeleteAllAsync();
        await dapperMySqlRepository.DeleteAllAsync();

        await mongoDbStudentRepository.DeleteAllAsync();
        await mongoDbStudentInsertedRepository.DeleteAllAsync();
    }

    #endregion Global Methods

    #region Sql Server

    #region Entity Framework

    #region Insert

    [Benchmark]
    public async Task Insert_10_EfCore_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_EfCore_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_EfCore_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_EfCore_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_EfCore_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkSqlServerRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_EfCore_SqlServer()
    {
        (await entityFrameworkSqlServerInsertedRepository.SelectAllAsync(x => true)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_EfCore_SqlServer()
    {
        (await entityFrameworkSqlServerInsertedRepository.SelectAllAsync(x => x.IsNotifiable)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_EfCore_SqlServer()
    {
        (await entityFrameworkSqlServerThemesRepository.SelectAllAsync(x => x.FontSize >= 100000)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_EfCore_SqlServer()
    {
        (await entityFrameworkSqlServerThemesRepository.SelectAllAsync(x => true, t => t.Student!)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_EfCore_SqlServer()
    {
        (await entityFrameworkSqlServerThemesRepository.SelectAllAsync(x => x.Student.IsNotifiable == true, t => t.Student!)).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_EfCore_SqlServer()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkSqlServerInsertedRepository.UpdateAsync(x => true, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    [Benchmark]
    public async Task UpdateWhere_200000_EfCore_SqlServer()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkSqlServerInsertedRepository.UpdateAsync(x => x.IsNotifiable, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    #endregion Update

    #endregion Entity Framework

    #region Dapper

    #region Insert

    [Benchmark]
    public async Task Insert_10_Dapper_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_Dapper_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_Dapper_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_Dapper_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperSqlServerRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_Dapper_SqlServer()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperSqlServerRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_Dapper_SqlServer()
    {
        (await dapperSqlServerInsertedRepository.SelectAllAsync()).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_Dapper_SqlServer()
    {
        (await dapperSqlServerInsertedRepository.SelectAllAsync("isnotifiable = 1")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_Dapper_SqlServer()
    {
        (await dapperSqlServerThemesRepository.SelectAllAsync("fontsize >= 100000")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_Dapper_SqlServer()
    {
        (await dapperSqlServerThemesRepository.SelectAllAsync(where: null, join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_Dapper_SqlServer()
    {
        (await dapperSqlServerThemesRepository.SelectAllAsync(where: "isnotifiable = 1", join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_Dapper_SqlServer()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperSqlServerInsertedRepository.UpdateAsync(new StudentInserted()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress"
        );
    }

    [Benchmark]
    public async Task UpdateWhere_200000_Dapper_SqlServer()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperSqlServerInsertedRepository.UpdateAsync(new StudentInserted()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress",
        "isnotifiable = 1"
        );
    }

    #endregion Update

    #endregion Dapper

    #endregion Sql Server

    #region PostgreSQL

    #region Entity Framework

    #region Insert

    [Benchmark]
    public async Task Insert_10_EfCore_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_EfCore_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_EfCore_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_EfCore_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_EfCore_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkPostgreSqlRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_EfCore_PostgreSQL()
    {
        (await entityFrameworkPostgreSqlInsertedRepository.SelectAllAsync(x => true)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_EfCore_PostgreSQL()
    {
        (await entityFrameworkPostgreSqlInsertedRepository.SelectAllAsync(x => x.IsNotifiable)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_EfCore_PostgreSQL()
    {
        (await entityFrameworkPostgreSqlThemesRepository.SelectAllAsync(x => x.FontSize >= 100000)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_EfCore_PostgreSQL()
    {
        (await entityFrameworkPostgreSqlThemesRepository.SelectAllAsync(x => true, t => t.Student!)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_EfCore_PostgreSQL()
    {
        (await entityFrameworkPostgreSqlThemesRepository.SelectAllAsync(x => x.Student.IsNotifiable == true, t => t.Student!)).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_EfCore_PostgreSQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkPostgreSqlInsertedRepository.UpdateAsync(x => true, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    [Benchmark]
    public async Task UpdateWhere_200000_EfCore_PostgreSQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkPostgreSqlInsertedRepository.UpdateAsync(x => x.IsNotifiable, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    #endregion Update

    #endregion Entity Framework

    #region Dapper

    #region Insert

    [Benchmark]
    public async Task Insert_10_Dapper_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_Dapper_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_Dapper_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_Dapper_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperPostgreSqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_Dapper_PostgreSQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperPostgreSqlRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_Dapper_PostgreSQL()
    {
        (await dapperPostgreSqlInsertedRepository.SelectAllAsync()).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_Dapper_PostgreSQL()
    {
        (await dapperPostgreSqlInsertedRepository.SelectAllAsync("isnotifiable = True")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_Dapper_PostgreSQL()
    {
        (await dapperPostgreSqlThemesRepository.SelectAllAsync("fontsize >= 100000")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_Dapper_PostgreSQL()
    {
        (await dapperPostgreSqlThemesRepository.SelectAllAsync(where: null, join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_Dapper_PostgreSQL()
    {
        (await dapperPostgreSqlThemesRepository.SelectAllAsync(where: "isnotifiable = True", join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_Dapper_PostgreSQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperPostgreSqlInsertedRepository.UpdateAsync(new StudentInserted()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress"
        );
    }

    [Benchmark]
    public async Task UpdateWhere_200000_Dapper_PostgreSQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperPostgreSqlInsertedRepository.UpdateAsync(new StudentInserted()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress",
        "isnotifiable = True"
        );
    }

    #endregion Update

    #endregion Dapper

    #endregion PostgreSQL

    #region MySQL

    #region Entity Framework

    #region Insert

    [Benchmark]
    public async Task Insert_10_EfCore_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_EfCore_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_EfCore_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_EfCore_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_EfCore_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await entityFrameworkMySqlRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_EfCore_MySQL()
    {
        (await entityFrameworkMySqlInsertedRepository.SelectAllAsync(x => true)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_EfCore_MySQL()
    {
        (await entityFrameworkMySqlInsertedRepository.SelectAllAsync(x => x.IsNotifiable)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_EfCore_MySQL()
    {
        (await entityFrameworkMySqlThemesRepository.SelectAllAsync(x => x.FontSize >= 100000)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_EfCore_MySQL()
    {
        (await entityFrameworkMySqlThemesRepository.SelectAllAsync(x => true, t => t.Student!)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_EfCore_MySQL()
    {
        (await entityFrameworkMySqlThemesRepository.SelectAllAsync(x => x.Student.IsNotifiable == true, t => t.Student!)).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_EfCore_MySQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkMySqlInsertedRepository.UpdateAsync(x => true, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    [Benchmark]
    public async Task UpdateWhere_200000_EfCore_MySQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await entityFrameworkMySqlInsertedRepository.UpdateAsync(x => x.IsNotifiable, x => new StudentInserted() { EmailAddress = defaultEmail });
    }

    #endregion Update

    #endregion Entity Framework

    #region Dapper

    #region Insert

    [Benchmark]
    public async Task Insert_10_Dapper_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_Dapper_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 100)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_Dapper_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 1000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_Dapper_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 10000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperMySqlRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_Dapper_MySQL()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<Student> students = Enumerable.Range(0, 50000)
            .Select(i => new Student()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await dapperMySqlRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_Dapper_MySQL()
    {
        (await dapperMySqlInsertedRepository.SelectAllAsync()).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_Dapper_MySQL()
    {
        (await dapperMySqlInsertedRepository.SelectAllAsync("isnotifiable = 1")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_Dapper_MySQL()
    {
        (await dapperMySqlThemesRepository.SelectAllAsync("fontsize >= 100000")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_Dapper_MySQL()
    {
        (await dapperMySqlThemesRepository.SelectAllAsync(where: null, join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_Dapper_MySQL()
    {
        (await dapperMySqlThemesRepository.SelectAllAsync(where: "isnotifiable = 1", join: "AS t1 INNER JOIN students_inserted AS t2 ON t2.Id = t1.studentid")).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_Dapper_MySQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperMySqlInsertedRepository.UpdateAsync(new StudentInserted ()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress"
        );
    }

    [Benchmark]
    public async Task UpdateWhere_200000_Dapper_MySQL()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();
        await dapperMySqlInsertedRepository.UpdateAsync(new StudentInserted()
        {
            EmailAddress = defaultEmail
        },
        "emailaddress",
        "isnotifiable = 1"
        );
    }

    #endregion Update

    #endregion Dapper

    #endregion MySQL

    #region MongoDb

    #region Insert

    [Benchmark]
    public async Task Insert_10_MongoDbDriver_MongoDb()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<MongoDbStudent> students = Enumerable.Range(0, 10)
            .Select(i => new MongoDbStudent()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });

        await mongoDbStudentRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_100_MongoDbDriver_MongoDb()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<MongoDbStudent> students = Enumerable.Range(0, 100)
            .Select(i => new MongoDbStudent()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        await mongoDbStudentRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_1000_MongoDbDriver_MongoDb()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<MongoDbStudent> students = Enumerable.Range(0, 1000)
            .Select(i => new MongoDbStudent()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        await mongoDbStudentRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_10000_MongoDbDriver_MongoDb()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<MongoDbStudent> students = Enumerable.Range(0, 10000)
            .Select(i => new MongoDbStudent()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        await mongoDbStudentRepository.InsertBulkAsync(students);
    }

    [Benchmark]
    public async Task Insert_50000_MongoDbDriver_MongoDb()
    {
        personEmail = BenchmarkingHelper.GenerateRandomString(10);
        personPassword = BenchmarkingHelper.GenerateRandomString(10);
        personNumber = BenchmarkingHelper.GenerateRandomString(10);
        isNotifiable = random.NextDouble() > 0.5;

        IEnumerable<MongoDbStudent> students = Enumerable.Range(0, 50000)
            .Select(i => new MongoDbStudent()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        await mongoDbStudentRepository.InsertBulkAsync(students);
    }

    #endregion Insert

    #region Select

    [Benchmark]
    public async Task Select_200000_MongoDbDriver_MongoDb()
    {
        (await mongoDbStudentInsertedRepository.SelectAllAsync(Builders<MongoDbStudentInserted>.Filter.Empty)).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhere_200000_MongoDbDriver_MongoDb()
    {
        (await mongoDbStudentInsertedRepository.SelectAllAsync(Builders<MongoDbStudentInserted>.Filter.Eq(x => x.IsNotifiable, true))).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectWhereInt_200000_MongoDbDriver_MongoDb()
    {
        (await mongoDbThemeRepository.SelectAllAsync(Builders<MongoDbTheme>.Filter.Gt(x => x.FontSize, 100000))).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoin_200000_MongoDbDriver_MongoDb()
    {
        (await mongoDbThemeRepository
            .SelectAllLookupAsync(
                Builders<BsonDocument>.Filter.Empty,
                typeof(MongoDbStudentInserted).GetDescription(),
                "studentId",
                "_id",
                "Students"
            )).Consume(consumer);
    }

    [Benchmark]
    public async Task SelectJoinWhere_200000_MongoDbDriver_MongoDb()
    {
        (await mongoDbThemeRepository
            .SelectAllLookupAsync(
                Builders<BsonDocument>.Filter.ElemMatch("Students", Builders<BsonDocument>.Filter.Eq("IsNotifiable", true)),
                typeof(MongoDbStudentInserted).GetDescription(),
                "studentId",
                "_id",
                "Students"
            )).Consume(consumer);
    }

    #endregion Select

    #region Update

    [Benchmark]
    public async Task Update_200000_MongoDbDriver_MongoDb()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();

        FilterDefinition<MongoDbStudentInserted> filter = Builders<MongoDbStudentInserted>.Filter.Empty;

        UpdateDefinition<MongoDbStudentInserted> update = Builders<MongoDbStudentInserted>.Update
            .Set(student => student.EmailAddress, defaultEmail);

        await mongoDbStudentInsertedRepository.UpdateManyAsync(filter, update);
    }

    [Benchmark]
    public async Task UpdateWhere_200000_MongoDbDriver_MongoDb()
    {
        string defaultEmail = BenchmarkingHelper.GetEmail();

        FilterDefinition<MongoDbStudentInserted> filter = Builders<MongoDbStudentInserted>.Filter
            .Eq(x => x.IsNotifiable, true);

        UpdateDefinition<MongoDbStudentInserted> update = Builders<MongoDbStudentInserted>.Update
            .Set(student => student.EmailAddress, defaultEmail);

        await mongoDbStudentInsertedRepository.UpdateManyAsync(filter, update);
    }

    #endregion Update

    #endregion

    #region Private Methods

    private async Task SeedMongoDbStudentsInserted(long studentsInsertedCount)
    {
        Random random = new Random();
        List<MongoDbStudentInserted> seedStudents = new List<MongoDbStudentInserted>();
        for (int i = 0; i < 200000 - studentsInsertedCount; ++i)
        {
            string personEmail = BenchmarkingHelper.GetEmail();
            string personPassword = BenchmarkingHelper.GenerateRandomString(10);
            string personNumber = BenchmarkingHelper.GenerateRandomString(10);
            bool? isNotifiable = random.NextDouble() > 0.5;
            seedStudents.Add(new MongoDbStudentInserted()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        }

        if (seedStudents.Count == 0)
        {
            return;
        }

        await mongoDbStudentInsertedRepository.InsertBulkAsync(seedStudents);
    }

    private async Task SeedMongoDbThemes(long themesCount)
    {
        List<MongoDbTheme> seedThemes = new List<MongoDbTheme>();

        for (int i = 0; i < 200000 - themesCount; ++i)
        {
            string themeTitle = BenchmarkingHelper.GenerateRandomString(10);
            string themeForegroundColor = BenchmarkingHelper.GenerateRandomString(10);
            string themeBackgroundColor = BenchmarkingHelper.GenerateRandomString(10);
            string themeFontName = BenchmarkingHelper.GenerateRandomString(10);
            seedThemes.Add(new MongoDbTheme()
            {
                Title = themeTitle!,
                ForegoundColor = themeForegroundColor!,
                BackgroundColor = themeBackgroundColor!,
                FontName = themeFontName!,
                FontSize = i,
                StudentId = mongoDbStudentInserted.Id
            });
        }

        if (seedThemes.Count == 0)
        {
            return;
        }

        await mongoDbThemeRepository.InsertBulkAsync(seedThemes);
    }

    private async Task SeedSqlStudentsInserted(IEntityFrameworkRepository<StudentInserted> repository, long studentsInsertedCount)
    {
        Random random = new Random();
        List<StudentInserted> seedStudents = new List<StudentInserted>();
        for (int i = 0; i < 200000 - studentsInsertedCount; ++i)
        {
            string personEmail = BenchmarkingHelper.GetEmail();
            string personPassword = BenchmarkingHelper.GenerateRandomString(10);
            string personNumber = BenchmarkingHelper.GenerateRandomString(10);
            bool? isNotifiable = random.NextDouble() > 0.5;
            seedStudents.Add(new StudentInserted()
            {
                EmailAddress = personEmail!,
                Password = personPassword!,
                IsNotifiable = isNotifiable!.Value,
                PhoneNumber = personNumber!,
            });
        }

        if (seedStudents.Count == 0)
        {
            return;
        }

        await repository.InsertBulkAsync(seedStudents);
        await repository.InsertBulkAsync(seedStudents);
        await repository.InsertBulkAsync(seedStudents);
    }

    private async Task SeedSqlThemes(IEntityFrameworkRepository<Theme> repository, StudentInserted studentInserted, long themesCount)
    {
        List<Theme> seedThemes = new List<Theme>();

        for (int i = 0; i < 200000 - themesCount; ++i)
        {
            string themeTitle = BenchmarkingHelper.GenerateRandomString(10);
            string themeForegroundColor = BenchmarkingHelper.GenerateRandomString(6);
            string themeBackgroundColor = BenchmarkingHelper.GenerateRandomString(6);
            string themeFontName = BenchmarkingHelper.GenerateRandomString(10);
            seedThemes.Add(new Theme()
            {
                Title = themeTitle!,
                ForegoundColor = themeForegroundColor!,
                BackgroundColor = themeBackgroundColor!,
                FontName = themeFontName!,
                FontSize = i,
                StudentId = studentInserted.Id
            });
        }

        if (seedThemes.Count == 0)
        {
            return;
        }

        await repository.InsertBulkAsync(seedThemes);
    }

    #endregion
}