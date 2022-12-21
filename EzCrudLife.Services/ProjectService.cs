using System.Diagnostics;
using EzCrudeLife.Models.Enums;
using EzCrudeLife.Models.OptionModels;

namespace EzCrudLife.Services;

public class ProjectService
{
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
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, projectName)} --name {projectName} --lang C#");
        
            // Generate the API
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.WebApi.ToString().ToLower()} --output {Path.Combine(options.Directory, projectName, ".API")} --name {projectName} --lang C#");
            if (!result) return result;
        
            // Generate the models
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, projectName, ".Models")} --name {projectName} --lang C#");
            if (!result) return result;
        
            // Generate the repositories
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {Path.Combine(options.Directory, projectName, ".Repositories")} --name {projectName} --lang C#");
            if (!result) return result;
        
            // Generate the services
            if (!result) return result;
            result = await RunDotNetCommand(projectName,
                $"dotnet new {DotNetType.ClassLib.ToString().ToLower()} --output {options.Directory} --name {Path.Combine(options.Directory, projectName, ".Services")} --lang C#");

            return result;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
            return false;
        }
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