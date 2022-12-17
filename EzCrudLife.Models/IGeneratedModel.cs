namespace EzCrudeLife.Models;

public interface IGeneratedModel
{
    public string Output { get; set; }
    public string SaveLocation { get; set; }
    void Generate();
}