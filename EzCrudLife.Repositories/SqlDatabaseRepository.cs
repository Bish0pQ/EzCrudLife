using System.Data;
using Dapper;
using EzCrudeLife.Models.DbModels;
using EzCrudLife.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EzCrudLife.Repositories;

public class SqlDatabaseRepository : IDatabaseRepository
{
    private readonly string _connectionString;

    public SqlDatabaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Get all the tables in a specific connection string
    /// </summary>
    /// <returns>List of all the tables without their columns.</returns>
    public async Task<List<DatabaseTable>> GetTables()
    {
        try
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<DatabaseTable>(Queries.GetTableQuery);
            return result.ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    /// <summary>
    /// Get all the columns for a specific table with their types as well
    /// </summary>
    /// <param name="table">The table where you want to find the information of the columns in.</param>
    /// <returns>A list of database columns or an empty list in case of nothing found or on exception.</returns>
    public async Task<List<DatabaseColumn>> GetColumnsForTable(string table)
    {  
        try
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<DatabaseColumn>(Queries.GetColumnInformation, new
            {
                TableName = table
            });
            
            return result.ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return new List<DatabaseColumn>();
        }
    }
}