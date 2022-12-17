using System.Text;
using EzCrudeLife.Models.DbModels;

namespace EzCrudeLife.Models.Logic;

public class DapperModel : IGeneratedModel
{
    public string Output { get; set; }
    public string SaveLocation { get; set; }
    private readonly DatabaseTable _table;

    public DapperModel(DatabaseTable table, string saveDirectory)
    {
        _table = table;
        Generate();
        SaveLocation = Path.Combine(saveDirectory, table.Name + ".cs");
    }

    public void Generate()
    {
        if (_table.Columns.Count < 1) return; // No point in generating the class

        var sb = new StringBuilder();
        foreach (var dc in _table.Columns)
        {
            if (dc.Name.ToLower().Contains("id")) sb.AppendLine("[Key]"); // To replace by actually checking if key exists
            var type = dc.DataType.ToCSharpType();
            if (string.IsNullOrWhiteSpace(type)) continue; // Skip because type not known 

            sb.Append($"public {type} {dc.Name}").AppendLine("{ get; set; }");
        }

        Output = Templates.GetDapperModelTemplate()
            .Replace("%t", _table.Name)
            .Replace("%d", sb.ToString());
    }
}