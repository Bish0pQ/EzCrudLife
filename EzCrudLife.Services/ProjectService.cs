using System.Diagnostics;
using System.Security.AccessControl;
using EzCrudeLife.Models.Enums;
using EzCrudeLife.Models.OptionModels;

namespace EzCrudLife.Services;

public class ProjectService
{
    private const string DefaultClassName = "Class1.cs";
    private const string DefaultControllerName = "WeatherForecastController.cs";
    private const string DefaultControllerModelName = "WeatherForecast.cs";
    public async Task<bool> CreateProjects(ProjectGenerateOptions options)
    {
        try
        {
            var projectName = options.ProjectName;
        
            // Create the solution
            var result = await RunDotNetCommand(projectName, $"dotnet new sln --name {projectName} --output {options.Directory}");
            if (!result) return result;
        
            // Generate the core
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, projectName)} --name {projectName}");
        
            // Generate the API
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.WebApi.ToString().ToLower()} --output {Path.Combine(options.Directory, $"{projectName}.API")} --name {projectName}.API");
            if (!result) return result;
        
            // Generate the models
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, $"{projectName}.Models")} --name {projectName}.Models");
            if (!result) return result;
        
            // Generate the repositories
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, $"{projectName}.Repositories")} --name {projectName}.Repositories");
            if (!result) return result;
        
            // Generate the services
            if (!result) return result;
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, $"{projectName}.Services")} --name {projectName}.Services");   
            
            // Generate the migrations
            if (!result) return result;
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, $"{projectName}.Migrations")} --name {projectName}.Migrations");

            // Clean-up any redundant files
            CleanUpProjects(options.Directory);
         
            // Add the projects to the solution file
            await AddProjects(options.Directory, projectName);
            
            return result;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
            return false;
        }
    }

    public async Task AddProjects(string directory, string projectName)
    {
        // Find the SLN file
        var slnFile = Directory.GetFiles(directory, searchPattern: "*.sln", SearchOption.TopDirectoryOnly)[0];
        Console.WriteLine($"Solution file found on path {slnFile}");
        
        var allProjectFiles = Directory.GetFiles(directory, searchPattern: "*.csproj", SearchOption.AllDirectories);
        foreach (var projectFile in allProjectFiles)
        {
            Console.WriteLine($"Adding project {Path.GetFileName(projectFile)} to solution...");
            await RunDotNetCommand(projectName, $"dotnet sln {slnFile} add {projectFile}");
        }
    }

    public void CleanUpProjects(string directory)
    {
        var defaultClassNames = new List<string>()
        {
            "class1.cs",
            "weatherforecastcontroller.cs",
            "weatherforecast.cs"
        };

        // Find the redundant files
        var allFiles = Directory.GetFiles(directory, searchPattern: "*.cs", SearchOption.AllDirectories);
        var filesToDelete = allFiles
            .Where(file => defaultClassNames
                .Contains(Path.GetFileName(file).ToLower())).ToList();
        
        Console.Write($"A total of {filesToDelete.Count} redundant files have been found. Deleting...");
        
        // Delete all the redundant files
        Array.ForEach(filesToDelete.ToArray(), File.Delete);
        Console.WriteLine(" OK");
    }

    public async Task<bool> RunDotNetCommand(string projectName, string arguments)
    {
        try
        {
            // Set the path to the dotnet executable
            var dotnetPath = @"C:\Program Files\dotnet\dotnet.exe";
            
            // Create a new process and start it
            var process = new Process();
            process.StartInfo.FileName = dotnetPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
        
            process.StartInfo.Arguments = arguments;
            process.Start();

            // Read the standard output and error streams of the process
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
        
            Console.WriteLine("Process output:" + output);

            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error output: " + error);
                Console.ForegroundColor = ConsoleColor.White;
            }
            
            await process.WaitForExitAsync();
            process.Close();
        
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return false;
        }
    }
}