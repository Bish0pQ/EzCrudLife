using EzCrudeLife.Models.DbModels;
using EzCrudeLife.Models.Logic;
using EzCrudLife.Services;

SqlDatabaseService _databaseService;
var connectionString = @"";
var projectName = ""; // Namespace
var exportLocation = @"C:\Temp\Testing";

// TODO: Get table information and column informationf
await Main();

async Task Main()
{
    Console.Write("ConnectionString: ");
    connectionString = Console.ReadLine();
    Console.Write("Project name: ");
    projectName = Console.ReadLine();

    // Quick checks, to replace later
    if (string.IsNullOrWhiteSpace(connectionString)) return;
    if (string.IsNullOrWhiteSpace(projectName)) return;
    if (string.IsNullOrWhiteSpace(exportLocation)) return;
    
    _databaseService = new SqlDatabaseService((connectionString));
    await GenerateModels();
    //await DisplayTables();
}

async Task GenerateModels()
{
    // Get the data from the database
    var data = await _databaseService.GetInfoFromConnection(connectionString);
    
    // Build the models and write to disk
    var dapperModels = new List<DapperModel>();
    foreach (var table in data) dapperModels.Add(new DapperModel(table, exportLocation, projectName));
    foreach (var dm in dapperModels) await dm.WriteToDiskAsync();

    Console.WriteLine($"Created {dapperModels.Count} new classes!");
    Console.ReadLine();
}




async Task DisplayTables()
{
    var data = await _databaseService.GetInfoFromConnection(connectionString);
    Console.WriteLine($"Found a total of {data.Count} tables.");
    foreach (var table in data)
    {
        table.Columns ??= new List<DatabaseColumn>();
        Console.WriteLine($"Table {table.Name} has {table.Columns.Count} columns.");
    }

    Console.WriteLine("Press ANY key to exit.");
    Console.ReadLine();
}

// TODO: Generate models
// TODO: Generate repositories
// TODO: Generate services
// TODO: Generate controller endpoints
// TODO: Generate UI
// TODO: Options