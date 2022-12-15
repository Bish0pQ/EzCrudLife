using System.Collections.Immutable;
using EzCrudeLife.Models.DbModels;
using EzCrudLife.Repositories;
using EzCrudLife.Services.Interfaces;

namespace EzCrudLife.Services;

public class SqlDatabaseService : IDatabaseService
{
    private readonly SqlDatabaseRepository _sqlDatabaseRepository;

    public SqlDatabaseService(string connectionString)
    {
        _sqlDatabaseRepository = new SqlDatabaseRepository(connectionString);
    }

    /// <summary>
    /// Returns a list with all tables
    /// </summary>
    /// <param name="connectionString">The connection string you want to generate the code from.</param>
    /// <returns>List of database tables with their columns.</returns>
    public async Task<List<DatabaseTable>> GetInfoFromConnection(string connectionString)
    {
        // Get a list of all the tables
        var tables = await _sqlDatabaseRepository.GetTables();

        // Get a list of columns that belong to the tables
        foreach (var table in tables)
            table.Columns = await _sqlDatabaseRepository.GetColumnsForTable(table.Name);

        return tables;
    }
}