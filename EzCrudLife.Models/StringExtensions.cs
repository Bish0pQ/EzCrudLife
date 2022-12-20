﻿namespace EzCrudeLife.Models;

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
}