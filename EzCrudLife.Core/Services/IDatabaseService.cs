using EzCrudeLife.Models.DbModels;

namespace EzCrudLife.Core.Services;

public interface IDatabaseService
{
  public Task<List<DatabaseTable>> GetInfoFromConnection(string connectionString);   
}