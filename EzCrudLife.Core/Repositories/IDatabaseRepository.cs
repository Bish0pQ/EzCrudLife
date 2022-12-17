using EzCrudeLife.Models.DbModels;

namespace EzCrudLife.Core.Repositories;

public interface IDatabaseRepository
{
    Task<List<DatabaseTable>> GetTables();
    Task<List<DatabaseColumn>> GetColumnsForTable(string table);
}