namespace Zeiterfassung;

public static class SqlCommands
{
    public static readonly string GetUsersCommand = "SELECT * FROM User_Table";
    public static readonly string GetEntriesCommand = "SELECT * FROM Entry_Table WHERE UserId = '{0}'";
    public static readonly string DeleteEntryCommand = "DELETE FROM Entry_Table WHERE ID = '{0}'";
    public static readonly string CreateEntryCommand = "INSERT INTO Entry_Table VALUES ('{0}',(TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss')),(TO_DATE('{2}','yyyy/mm/dd hh24:mi:ss')),'{3}','{4}')";
    public static readonly string GetYearsCommand = "SELECT et.StartTimeStamp FROM Entry_Table et";
    public static readonly string GetChipIdCount = "SELECT COUNT(*) FROM User_Table WHERE ChipId = '{0}'";
    public static readonly string CreateUserCommand = "INSERT INTO User_Table VALUES ('{0}','{1}','{2}',{3})";
}
