using System.Diagnostics;

namespace EzCrudLife.Services;

public class DotNetService
{
    public DotNetService()
    {
    }

    public async Task<bool> RunDotNetCommand(string arguments)
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