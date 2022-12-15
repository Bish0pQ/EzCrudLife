using EzCrudeLife.Models.DbModels;
using EzCrudLife.Services;

SqlDatabaseService _databaseService;
var connectionString = "";

// TODO: Get table information and column informationf
await Main();

async Task Main()
{
    Console.Write("ConnectionString: ");
    connectionString = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(connectionString)) return;
    _databaseService = new SqlDatabaseService((connectionString));

    await DisplayTables();
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