namespace EzCrudeLife.Models;

public interface IGeneratedModel
{
    public string Output { get; set; }
    public string SaveLocation { get; set; }
    public string ProjectName { get; set; }
    public string ClassSuffix { get; set; }
    
    void Generate();
}