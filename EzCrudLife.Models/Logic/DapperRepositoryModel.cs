using System.Text;
using EzCrudeLife.Models.DbModels;
using EzCrudeLife.Models.Enums;

namespace EzCrudeLife.Models.Logic;

public class DapperRepositoryModel : IGeneratedModel
{
    private readonly DatabaseTable _table;
    
    public string Output { get; set; }
    public string SaveLocation { get; set; }
    public string ProjectName { get; set; }
    public string ClassSuffix { get; set; }

    public DapperRepositoryModel(){}
    
    public DapperRepositoryModel(DatabaseTable table, string saveDirectory, string projectName)
    {
        _table = table;
        ProjectName = projectName;
        Generate();
        SaveLocation = Path.Combine(saveDirectory, table.Name + ClassSuffix + "Repository.cs");
    }

    public void Generate()
    {
        if (_table.Columns.Count < 1) return;
        
        // Generate base template
        var baseTemplate = Templates.GetDapperRepositoryModelTemplate(ProjectName);

        // Build the actual repository methods
        var sb = new StringBuilder();
        
        // Declare variables
        sb.SetVariable("_logger", "ILogger", "private", "readonly")
            .SetVariable("_sqlProvider", "DatabaseProvider", "private", "readonly")
            .AppendLine();
        
        // Create constructors
        sb.InsertConstructor(_table.Name + "Repository", new Dictionary<string, string>()
        {
            {
                "ILogger", "logger"
            },
            {
                "DatabaseProvider", "sqlProvider"
            }
            
        });

        sb.InsertConstructor(_table.Name + "Repository", new Dictionary<string, string>());
        
        // Add dapper methods
        sb.InsertDapperMethod(_table.Name + "Dto", DapperMethodType.Insert);
        sb.InsertDapperMethod(_table.Name + "Dto", DapperMethodType.Update);
        sb.InsertDapperMethod(_table.Name + "Dto", DapperMethodType.Delete);
        sb.InsertDapperMethod(_table.Name + "Dto", DapperMethodType.Get);

        Output = baseTemplate
            .Replace("%dt", _table.Name)
            .Replace("%t", _table.Name + "Repository" + ClassSuffix)
            .Replace("%d", sb.ToString());
    }
}