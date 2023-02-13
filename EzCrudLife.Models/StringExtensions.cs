using System.Text;
using EzCrudeLife.Models.DbModels;
using EzCrudeLife.Models.Enums;

namespace EzCrudeLife.Models;

public static class StringExtensions
{
    public static string ToCSharpType(this string sqlType)
    {
        switch (sqlType)
        {
            case "varchar" or "nvarchar" or "nchar" or "text":
                return "string";
            case "int" or "numeric":
                return "int";
            case "datetime" or "datetime2" or "time":
                return "DateTime";
            case "float" or "decimal":
                return "double";
            case "bit":
                return "bool";
            case "bigint":
                return "long";
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
            case "smallint" or "short":
                return ".AsInt16()";
        }

        Console.WriteLine($"[Error] Following SQL type has no mapping for fluent migrator: {type}");
        return "";
    }

    #region String builder extensions

    public static StringBuilder UseImport(this StringBuilder builder, string name) => builder.Append($"using {name};\r\n");
    public static StringBuilder InNamespace(this StringBuilder builder, string nameSpace) => builder.Append($"namespace {nameSpace};\r\n");
    public static StringBuilder InsertAttribute(this StringBuilder builder, string attribute, string attributeValue) =>
        builder.Append($"[{attribute}({attributeValue})]");
    public static StringBuilder InsertClass(this StringBuilder builder, string className, bool isAbstract = false,
        bool isStatic = false) => builder.Append($"\r\npublic {(isAbstract ? "abstract " : "")}{(isStatic ? "static " : "")}class " +
        $"{className}\r\n{{\r\n%d}}");

    public static StringBuilder InsertConstructor(this StringBuilder sb, string className, Dictionary<string, string> parameters)
    {
        // Generate the constructor
        sb.AppendLine("\tpublic " + className + "(" + string.Join(", ", parameters.Select(x => x.Key + " " + x.Value)) +
                      ") ")
            .Indent(1).AppendLine("{");
        foreach (KeyValuePair<string, string> parameter in parameters)
            sb.AppendLine($"\t\t_{parameter.Value.ToLower()} = {parameter.Value};");
        sb.AppendLine("\t}");

        return sb;
    }
    
    public static StringBuilder InsertConstantVariable(this StringBuilder builder, Type t, string name, string value) =>
        builder.AppendLine($"public const {t.Name.ToLower()} {name} = \"{value}\";"); 
    
    public static StringBuilder InsertDapperMethod(this StringBuilder builder, string itemName, DapperMethodType methodType)
    {
        var returnType = methodType switch
        {
            DapperMethodType.Insert => "int",
            DapperMethodType.Delete => "bool",
            DapperMethodType.Update => "bool",
            DapperMethodType.Get => $"IEnumerable<{itemName}>",
            _ => ""
        };

        var indentLevel = 2;
        var tableName = itemName.Replace("Dto", "");
        builder.NewLine().Indent().AppendLine(
                $"public async Task<{returnType}> {methodType.ToString()}{tableName}Async({itemName} {itemName.FirstCharToLower()})")
            .Indent(1, "{")
            .NewLine().Indent(indentLevel).Append("try")
            .NewLine().Indent(indentLevel, startingCharacter: "{");
            
            indentLevel = 3;
            builder.NewLine().Indent(indentLevel)
                .AppendLine("using IDbConnection connection = new SqlConnection(_sqlProvider.GetConnectionString());")
                .Indent(indentLevel);

            if (methodType != DapperMethodType.Get)
                builder.AppendLine(
                    $"return await connection.{methodType.ToString()}Async({itemName.FirstCharToLower()});");
            else builder.AppendLine($"return await connection.QueryAsync<{itemName}>({GetBaseSelectQuery(tableName)});");

            builder.Indent(2, "}")
                .NewLine().Indent(2).AppendLine("catch (Exception exception)")
                .Indent(2, "{").NewLine()
                .Indent(indentLevel)
                .AppendLine(
                    $"_logger.Error(exception, \"Unable to {methodType.ToString().ToLower()} entity of type {{Type}}\", typeof(" +
                    $"{itemName}));")
                .Indent(indentLevel).AppendLine(methodType == DapperMethodType.Get ? "return null;" :
                    methodType == DapperMethodType.Insert ? "return -1;" : "return false;")
                .Indent(2).AppendLine("}")
                .Indent().AppendLine("}");

            return builder;
    }

    public static string GetBaseSelectQuery(string item) => $"\"SELECT * FROM {item}\"";

    public static StringBuilder Indent(this StringBuilder sb, int count = 1, string startingCharacter = "") => sb.Append(new string('\t', count) + startingCharacter);

    public static StringBuilder NewLine(this StringBuilder sb,  string startingCharacter = "") => sb.Append("\r\n" + startingCharacter);
    public static StringBuilder AppendWithSpace(this StringBuilder sb, string toAppend) => sb.Append(toAppend + " ");

    public static StringBuilder SetVariable(this StringBuilder sb, string name, string type, string access = "public",
        string attributes = "") =>
        sb.Indent().AppendWithSpace(access)
            .AppendWithSpace(attributes).AppendWithSpace(type).Append(name).AppendLine(";");

    #endregion
    
    
    public static string FirstCharToLower(this string s) => string.IsNullOrWhiteSpace(s) ? s : s[0].ToString().ToLower() + s.Substring(1, s.Length - 1);

}