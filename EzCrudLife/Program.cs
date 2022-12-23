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
    Console.Write("ConnectionString: ");
    connectionString = Console.ReadLine();
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
    Console.Write("Generate models?");
    Console.ReadLine();
    
    var tables = await GenerateModels();
    Thread.Sleep(1000);
    await GenerateMigrations(tables);
    //await DisplayTables();
}

// Generate the models based on the SQL database
async Task<List<DatabaseTable>> GenerateModels()
{
    // Get the data from the database
    var data = await _databaseService.GetInfoFromConnection(connectionString);
    
    // Build the models and write to disk
    var dapperModels = new List<DapperModel>();
    foreach (var table in data) dapperModels.Add(new DapperModel(table, Path.Combine(exportLocation, "Models"), projectName));
    foreach (var dm in dapperModels) await dm.WriteToDiskAsync();

    Console.WriteLine($"Created {dapperModels.Count} new classes!");
    return data;
}

// Generate the migration based on the SQL database
async Task GenerateMigrations(List<DatabaseTable> tables)
{
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
        if (exportLocation != null) await sb.WriteToDiskAsync(Path.Combine(exportLocation, "Migrations", fileNameOnly));
        migrationNumber++;
    }
}


StringBuilder GetBaseTemplate(string table)
{
    var sb = new StringBuilder();
    sb.AppendLine("using FluentMigrator;")
        .AppendLine("namespace " + projectName + ";")
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


//
// async Task DisplayTables()
// {
//     var data = await _databaseService.GetInfoFromConnection(connectionString);
//     Console.WriteLine($"Found a total of {data.Count} tables.");
//     foreach (var table in data)
//     {
//         table.Columns ??= new List<DatabaseColumn>();
//         Console.WriteLine($"Table {table.Name} has {table.Columns.Count} columns.");
//     }
//
//     Console.WriteLine("Press ANY key to exit.");
//     Console.ReadLine();
// }

// TODO: Generate repositories
// TODO: Generate services
// TODO: Generate controller endpoints
// TODO: Generate UI
// TODO: Options