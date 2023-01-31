using System.Text;
using EzCrudeLife.Models.DbModels;

namespace EzCrudeLife.Models;

public static class StringExtensions
{
    public static string ToCSharpType(this string sqlType)
    {
        switch (sqlType)
        {
            case "varchar" or "nvarchar":
                return "string";
            case "int":
                return "int";
            case "numeric":
                return "int";
            case "text":
                return "string";
            case "datetime":
                return "DateTime";
            case "bit":
                return "bool";
            case "bigint":
                return "long";
            case "time":
                return "DateTime";
            case "nchar":
                return "string";
            case "float":
                return "double";
            case "decimal":
                return "double";
            case "varbinary":
                return "byte[]";
            case "smallint":
                return "short";
            case "char":
                return "char";
            default:
                Console.WriteLine($"Unknown CSharp type for SQL type {sqlType}");
                return "";
        }
    }
    
    public static StringBuilder AddDbColumns(this StringBuilder sb, List<DatabaseColumn> columns)
    {
        foreach (var column in columns)
        {
            sb.Append("\t\t\t"); // Formatting
            sb.Append($".WithColumn(\"{column.Name}\")");
            if (!string.IsNullOrWhiteSpace(column.DataType))
                sb.Append(GetFmType(column.DataType, column.CharMaxLength));
            
            sb.AppendLine(column.Nullable ? ".Nullable()" : ".NotNullable()");
        }

        if (!sb.ToString().EndsWith(";")) sb.Append(';');
        sb.AppendLine("\t}");
        return sb;
    }

    private static string GetFmType(string type, int? stringMaxSize)
    {
        switch (type)
        {
            case "int":
                return ".AsInt32()";
            case "nvarchar":
                if (stringMaxSize == null || stringMaxSize < 1)
                    return $".AsString(int.MaxValue)";
                return $".AsString({stringMaxSize})";
            case "datetime":
                return ".AsDateTime()";
            case "datetime2":
                return ".AsDateTime2()";
            case "bit":
                return ".AsBinary()";
            case "bigint":
                return ".AsInt64()";
        }

        return "";
    }
}