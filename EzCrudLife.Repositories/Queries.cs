namespace EzCrudLife.Repositories;

public class Queries
{
    public const string GetTableQuery = "SELECT DISTINCT name FROM sys.tables";
    public const string GetColumnInformation = @"SELECT [COLUMN_NAME], IS_NULLABLE, DATA_TYPE,CHARACTER_MAXIMUM_LENGTH 
                                                 FROM INFORMATION_SCHEMA.COLUMNS 
                                                 WHERE TABLE_NAME = @TableName";
}