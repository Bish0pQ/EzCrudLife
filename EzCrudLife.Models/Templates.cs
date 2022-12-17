using System.Text;

namespace EzCrudeLife.Models;

public abstract class Templates
{
    public static string GetDapperModelTemplate()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Dapper.Contrib.Extensions;\r\n")
            .AppendLine($"[Dapper.Contrib.Extensions.Table(\"%t\")]\r\n")
            .AppendLine("public class %t\r\n{\r\n%d\r\n}");
        return sb.ToString();
    }
}