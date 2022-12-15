namespace EzCrudeLife.Models.DbModels;

public class DatabaseTable
{
    public string Name { get; set; }
    public List<DatabaseColumn> Columns { get; set; }
}