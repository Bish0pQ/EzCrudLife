using System.Text;
using EzCrudeLife.Models;

namespace EzCrudLife.Services;

public static class OutputService
{
    public static async Task<IGeneratedModel> WriteToDiskAsync(this IGeneratedModel model)
    {
        //if (!Directory.Exists(model.SaveLocation)) throw new DirectoryNotFoundException("The directory where you want to save the file does not exist. Check again.");
        Console.Write($"Attempting to write {Encoding.UTF8.GetBytes(model.Output).Length} bytes to {model.SaveLocation}...");
        await File.WriteAllTextAsync(model.SaveLocation, model.Output, Encoding.UTF8);
        Console.WriteLine(" OK");
        return model;
    }
}