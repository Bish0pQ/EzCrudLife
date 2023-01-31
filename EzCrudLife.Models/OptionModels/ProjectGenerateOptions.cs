namespace EzCrudeLife.Models.OptionModels;

public class ProjectGenerateOptions
{
    public ProjectGenerateOptions()
    {
    }

    public ProjectGenerateOptions(bool generateApiProject, bool generateModelsProject, bool generateRepositoriesProject, bool generateServicesProject,
        bool generateMigrationsProject)
    {
        GenerateApiProject = generateApiProject;
        GenerateModelsProject = generateModelsProject;
        GenerateRepositoriesProject = generateRepositoriesProject;
        GenerateServicesProject = generateServicesProject;
        GenerateMigrationsProject = generateMigrationsProject;
    }

    public string Directory { get; set; }
    public string ProjectName { get; set; }
    public bool GenerateApiProject { get; set; } = false;
    public bool GenerateModelsProject { get; set; } = true;
    public bool GenerateRepositoriesProject { get; set; } = false;
    public bool GenerateServicesProject { get; set; } = false;
    public bool GenerateMigrationsProject { get; set; } = true;
}