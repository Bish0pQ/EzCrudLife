using EzCrudeLife.Models.DbModels;

namespace EzCrudeLife.Models.Logic;

public class FluentMigratorModel
{
    private readonly DatabaseTable _table;
    private readonly string _saveDirectory;
    
    
    public FluentMigratorModel(DatabaseTable table, string saveDirectory, string projectName)
    {
        _table = table;
        //_saveDirecotory = Path.Combine(saveDirectory, table.Name + ClassSuffix + ".cs"); // TODO: To determine name
        Generate();
    }

    public void Generate()
    {
        
    }
}