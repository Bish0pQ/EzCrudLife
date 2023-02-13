using System.Text;
using EzCrudeLife.Models;
using EzCrudeLife.Models.DbModels;
using EzCrudeLife.Models.Logic;
using EzCrudeLife.Models.OptionModels;
using EzCrudLife.Services;

SqlDatabaseService _databaseService;
ProjectService _projectService;

var connectionString = @"";
var projectName = ""; // Namespace
var exportLocation = @"C:\Temp\Testing";
var migrationNumber = 1;

// TODO: Get table information and column information
await Main();

async Task Main()
{
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.Write("ConnectionString: ");
        connectionString = Console.ReadLine();    
    }
    
    Console.Write("Project name: ");
    projectName = Console.ReadLine();

    Console.ForegroundColor = ConsoleColor.White;  
    // Quick checks, to replace later
    if (string.IsNullOrWhiteSpace(connectionString)) return;
    if (string.IsNullOrWhiteSpace(projectName)) return;
    if (string.IsNullOrWhiteSpace(exportLocation)) return;

    _databaseService = new SqlDatabaseService((connectionString));
    _projectService = new ProjectService();

    Console.Write("Generating projects...");
    var result = await _projectService.CreateProjects(new ProjectGenerateOptions()
    {
        GenerateApiProject = true,
        GenerateModelsProject = true,
        GenerateRepositoriesProject = true,
        GenerateServicesProject = true,
        Directory = exportLocation,
        ProjectName = projectName
    });
    
    Console.WriteLine("Generating projects... " + (result ? " [OK]" : "[NOK]"));

    Console.WriteLine("Generating models...");
    Thread.Sleep(1000);
    var tables = await GenerateModels();
    Thread.Sleep(1000);
    await GenerateMigrations(tables);
    
    Console.WriteLine("Preparing generating repositories...");
    Thread.Sleep(1000);
    await GenerateRepositories(tables);
    
    //await DisplayTables();
}

// Generate the models based on the SQL database
async Task<List<DatabaseTable>> GenerateModels()
{
    // Get the data from the database
    var data = await _databaseService.GetInfoFromConnection(connectionString);
    
    // Build the models and write to disk
    var dapperModels = new List<DapperModel>();
    foreach (var table in data) dapperModels.Add(new DapperModel(table, Path.Combine(exportLocation, $"{projectName}.Models"), projectName));
    foreach (var dm in dapperModels) await dm.WriteToDiskAsync();

    Console.WriteLine($"Created {dapperModels.Count} new classes!");
    return data;
}

// Generate the migration based on the SQL database
async Task GenerateMigrations(List<DatabaseTable> tables)
{
    // TODO: Make sure Table object exists
    await CreateGlobalTablesClass(tables);
    
    foreach (var table in tables)
    {
        var fileNameOnly = $"Migration{getMigrationVersion()}_Add{table.Name}.cs";
        
        var sb = GetBaseTemplate(table.Name);
        sb.AddDbColumns(table.Columns);
        
        // Finishing file
        sb.AppendLine()
            .AppendLine("\tpublic override void Down()")
            .AppendLine("\t{")
            .AppendLine($"\t\tDelete.Table(Tables.{table.Name});")
            .AppendLine("\t}")
            .AppendLine("}");
        
        // Write the file to disk
        if (exportLocation != null) await sb.WriteToDiskAsync(Path.Combine(exportLocation, $"{projectName}.Migrations", fileNameOnly));
        migrationNumber++;
    }
}

async Task<bool> CreateGlobalTablesClass(List<DatabaseTable> tables)
{
    Console.Write("Creating global tables class...");
    var sb = new StringBuilder()
        .InNamespace(projectName + ".Migrations")
        .InsertClass("Tables", isAbstract: true);

    var template = sb.ToString();

    sb.Clear(); // Data
    Array.ForEach(tables.ToArray(), table =>
    {
        sb.Indent(1).InsertConstantVariable(typeof(string), table.Name, table.Name);
    });

    template = template.Replace("%d", sb.ToString());
    sb.Clear();
    sb.Append(template);
    
    var success = await sb.WriteToDiskAsync(Path.Combine(exportLocation, $"{projectName}.Migrations", "Tables.cs"));
    Console.WriteLine(" OK");
    return success;
}

async Task<bool> GenerateRepositories(List<DatabaseTable> tables)
{
    // Create the database provider
    await CreateDatabaseProvider();
    
    // Build the models and write to disk
    var dapperModels = new List<DapperRepositoryModel>();
    foreach (var table in tables) dapperModels.Add(new DapperRepositoryModel(table, Path.Combine(exportLocation, $"{projectName}.Repositories"), projectName));
    foreach (var dm in dapperModels) await dm.WriteToDiskAsync();

    Console.WriteLine($"Created {dapperModels.Count} new repositories!");

    return true;
}

async Task CreateDatabaseProviderInterface()
{
    projectName ??= "test"; // TODO: To replace
    var sb = new StringBuilder();
    sb.InNamespace(projectName + ".Repositories.Interfaces")
        .UseImport("System.Data")
        .AppendLine("public interface IDatabaseProvider")
        .Indent(0, startingCharacter: "{")
        .NewLine().Indent(1).AppendLine("public string GetConnectionString();")
        .Indent(0, startingCharacter: "}");

    var interfacePath = Path.Combine(exportLocation, $"{projectName}.Repositories", "Interfaces");
    if (!Directory.Exists(interfacePath))
    {
        Console.Write("Interface directory is missing, creating... ");
        Directory.CreateDirectory(interfacePath);
        Console.WriteLine(" OK");
    }

    await sb.WriteToDiskAsync(Path.Combine(interfacePath, "IDatabaseProvider.cs"));
}

async Task CreateDatabaseProvider()
{
    await CreateDatabaseProviderInterface();
    
    var sb = new StringBuilder();
    sb.InNamespace(projectName + ".Repositories")
        .UseImport("Microsoft.Extensions.Configuration")
        .UseImport($"{projectName}.Repositories.Interfaces")
        .InsertClass("DatabaseProvider : IDatabaseProvider"); // TODO: Add proper inherits lates

    var template = sb.ToString();
    sb.Clear();

    sb.SetVariable("_config", "IConfiguration", "private", "readonly")
        .InsertConstructor("DatabaseProvider", new Dictionary<string, string>()
        {
            { "IConfiguration", "config" }
        })
        .InsertShortHandFunction("GetConnectionString", " _config[\"ConnectionStrings:DbConnectionString\"];");

    template = template.Replace("%d", sb.ToString());
    sb.Clear();

    sb.AppendLine(template);
    await sb.WriteToDiskAsync(Path.Combine(exportLocation, $"{projectName}.Repositories", "DatabaseProvider.cs"));
}


StringBuilder GetBaseTemplate(string table)
{
    var sb = new StringBuilder();
    sb.AppendLine("using FluentMigrator;")
        .AppendLine("namespace " + projectName + ".Migrations;")
        .AppendLine($"\r\n[Migration({getMigrationVersion()})]")
        .AppendLine($"public class Migration{getMigrationVersion()}_Add{table} : Migration")
        .AppendLine("{")
        .AppendLine("\tpublic override void Up()")
        .AppendLine("\t{")
        .AppendLine($"\t\tCreate.Table(Tables.{table})")
        .AppendLine($"\t\t\t.WithDescription(\"\")");

    return sb;
}

string getMigrationVersion() => migrationNumber.ToString().Length == 1 ? "00000000000" + migrationNumber : "0000000000" + migrationNumber;

// TODO: Generate repositories
// TODO: Generate services
// TODO: Generate controller endpoints
// TODO: Generate UI
// TODO: Options