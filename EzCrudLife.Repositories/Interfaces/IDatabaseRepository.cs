using EzCrudeLife.Models.DbModels;

namespace EzCrudLife.Repositories.Interfaces;

public interface IDatabaseRepository
{
    Task<List<DatabaseTable>> GetTables();
    Task<List<DatabaseColumn>> GetColumnsForTable(string table);
}