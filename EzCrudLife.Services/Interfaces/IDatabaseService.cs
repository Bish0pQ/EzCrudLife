using EzCrudeLife.Models.DbModels;

namespace EzCrudLife.Services.Interfaces;

public interface IDatabaseService
{
  public Task<List<DatabaseTable>> GetInfoFromConnection(string connectionString);   
}