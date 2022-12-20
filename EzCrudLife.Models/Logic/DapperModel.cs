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
        var idColumns = _table.Columns.Where(x => x.Name.ToLower().Contains("id")).ToList();

        // Always put all of the ID's above (for easily reading what it is connected to
        foreach (var idc in idColumns)
        {
            // It's a key when it's called "ID" or when it has the name of the table with the suffix ID
            if (idc.Name.ToLower() == "id" || idc.Name.ToLower() == $"{_table.Name.ToLower()}id") 
                sb.AppendLine("\t[Key]"); // To replace by actually checking if key exists
            
            var type = idc.DataType.ToCSharpType();
            if (string.IsNullOrWhiteSpace(type)) continue; // Skip because type not known 

            sb.Append($"\tpublic {type} {idc.Name}").AppendLine(" { get; set; }");
        }
        
        // Loop over the regular columns which aren't id's
        foreach (var dc in _table.Columns.Where(x => !x.Name.ToLower().Contains("id")).OrderBy(x => x.Name))
        {
            var type = dc.DataType.ToCSharpType();
            if (string.IsNullOrWhiteSpace(type)) continue; // Skip because type not known 

            sb.Append($"\tpublic {type} {dc.Name}").AppendLine(" { get; set; }");
        }

        Output = Templates.GetDapperModelTemplate(ProjectName)
            .Replace("%dt", _table.Name)
            .Replace("%t", _table.Name + ClassSuffix)
            .Replace("%d", sb.ToString());
    }
}