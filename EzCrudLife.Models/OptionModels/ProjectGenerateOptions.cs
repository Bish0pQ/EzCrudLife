namespace EzCrudeLife.Models.OptionModels;

public class ProjectGenerateOptions
{
    public ProjectGenerateOptions(bool generateApiProject, bool generateModelsProject, bool generateRepositoriesProject, bool generateServicesProject)
    {
        GenerateApiProject = generateApiProject;
        GenerateModelsProject = generateModelsProject;
        GenerateRepositoriesProject = generateRepositoriesProject;
        GenerateServicesProject = generateServicesProject;
    }

    public string Directory { get; set; }
    public string ProjectName { get; set; }
    public bool GenerateApiProject { get; set; }
    public bool GenerateModelsProject { get; set; }
    public bool GenerateRepositoriesProject { get; set; }
    public bool GenerateServicesProject { get; set; }
}