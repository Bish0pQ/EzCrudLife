using System.Text;
using EzCrudeLife.Models.DbModels;

namespace EzCrudeLife.Models.Logic;

public class DapperModel : IGeneratedModel
{
    public string Output { get; set; }
    public string SaveLocation { get; set; }
    public string ProjectName { get; set; }
    public string ClassSuffix { get; set; } = "Dto";
    private readonly DatabaseTable _table;

    public DapperModel(DatabaseTable table, string saveDirectory, string projectName)
    {
        _table = table;
        ProjectName = projectName;
        Generate();
        SaveLocation = Path.Combine(saveDirectory, table.Name + ClassSuffix + ".cs");
    }

    public void Generate()
    {
        if (_table.Columns.Count < 1) return; // No point in generating the class

        var sb = new StringBuilder();
        foreach (var dc in _table.Columns.OrderBy(x => x.Name))
        {
            if (dc.Name.ToLower() == "id") sb.AppendLine("\t[Key]"); // To replace by actually checking if key exists
            var type = dc.DataType.ToCSharpType();
            if (string.IsNullOrWhiteSpace(type)) continue; // Skip because type not known 

            sb.Append($"\tpublic {type} {dc.Name}").AppendLine("{ get; set; }");
        }

        Output = Templates.GetDapperModelTemplate(ProjectName)
            .Replace("%dt", _table.Name)
            .Replace("%t", _table.Name + ClassSuffix)
            .Replace("%d", sb.ToString());
    }
}