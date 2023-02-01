using System.Text;

namespace EzCrudeLife.Models;

public abstract class Templates
{
    public static string GetDapperModelTemplate(string nameSpace)
    {
        var sb = new StringBuilder();
        sb.UseImport("Dapper.Contrib.Extensions")
            .InNamespace(nameSpace + ".Models")
            .InsertAttribute("Dapper.Contrib.Extensions.Table", "\"%dt\"")
            .InsertClass();
        
        return sb.ToString();
    }

    public static string GetDapperRepositoryModelTemplate(string nameSpace)
    {
        var sb = new StringBuilder();
        sb.UseImport("Dapper.Contrib.Extensions")
            .UseImport("Microsoft.Data.SqlClient")
            .UseImport("Dapper")
            .UseImport("Serilog")
            .UseImport("System.Data")
            .UseImport(nameSpace + ".Models")
            .InNamespace(nameSpace + ".Repositories")
            .InsertClass();

        return sb.ToString();
    }
}