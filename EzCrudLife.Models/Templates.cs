using System.Text;

namespace EzCrudeLife.Models;

public abstract class Templates
{
    public static string GetDapperModelTemplate(string nameSpace)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Dapper.Contrib.Extensions;\r\n")
            .AppendLine($"namespace {nameSpace}.Models;\r\n")
            .AppendLine($"[Dapper.Contrib.Extensions.Table(\"%dt\")]")
            .AppendLine("public class %t\r\n{\r\n%d}");
        return sb.ToString();
    }
}