namespace EzCrudeLife.Models.DbModels;

public class DatabaseColumn
{
    public string Name { get; set; }
    public string NullableText { get; set; }
    public bool Nullable => NullableText.ToLower() == "yes";

    public string DataType { get; set; }
    public int CharMaxLength { get; set; }
}