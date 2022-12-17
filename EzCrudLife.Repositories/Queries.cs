namespace EzCrudLife.Repositories;

public class Queries
{
    public const string GetTableQuery = "SELECT DISTINCT name FROM sys.tables";
    public const string GetColumnInformation = @"SELECT [COLUMN_NAME] AS [Name], IS_NULLABLE AS [NullableText], DATA_TYPE AS [DataType],CHARACTER_MAXIMUM_LENGTH AS [CharMaxLength] 
                                                 FROM INFORMATION_SCHEMA.COLUMNS 
                                                 WHERE TABLE_NAME = @TableName";
}