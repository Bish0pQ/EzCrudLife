namespace EzCrudeLife.Models.DbModels;

public class DatabaseColumn
{
    public string Name { get; set; }
    public bool Nullable { get; set; }
    public string DataType { get; set; }
    public int CharMaxLength { get; set; }
}