using System;
using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace Zeiterfassung;

public class DbHandler
{
    private static string connectionString = "Data Source=db-server.s-atiw.de:1521/atiwora;User ID=FS224_kkuehne;Password=kevin";

    public void SafeEntryInDatabase(Entry entry)
    {
        var startDateString = DatabaseValuePreparer.GetOracleDateTimeString(entry.StartTimeStamp);
        var endDateString = DatabaseValuePreparer.GetOracleDateTimeString(entry.EndTimeStamp);
        var query = string.Format(SqlCommands.CreateEntryCommand, entry.GetId(), startDateString, endDateString, entry.TimeSpan, entry.GetUserId());
        ExecuteCommand(query);
    }

    public void RemoveEntry(Entry entry)
    {
        var query = string.Format(SqlCommands.DeleteEntryCommand, entry.GetId());
        ExecuteCommand(query);
    }

    public void UpdateExistingEntry(Entry entry)
    {
        RemoveEntry(entry);
        SafeEntryInDatabase(entry);
    }

    public bool TryCreateUser(User user)
    {
        var query1 = string.Format(SqlCommands.GetChipIdCount, user.GetChipId());
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        var command = new OracleCommand(query1, connection);
        var value = command.ExecuteScalar();
        if ((decimal)value > 0)
            return false;
        var isAdminValue = user.IsAdmin ? 1 : 0;
        var query2 = string.Format(SqlCommands.CreateUserCommand, user.GetId(), user.Username, user.GetChipId(), isAdminValue);
        return ExecuteCommand(query2);
    }

    public bool ExecuteCommand(string query)
    {
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        var command = new OracleCommand(query, connection);
        command.ExecuteNonQuery();
        connection.Close();
        return true;
    }

    public List<User> GetUsers()
    {
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        var command = new OracleCommand(SqlCommands.GetUsersCommand, connection);
        var reader = command.ExecuteReader();
        var users = new List<User>();
        while (reader.Read())
        {
            var idString = reader.GetString(0);
            var userName = reader.GetString(1);
            var chipId = reader.GetString(2);
            var isAdmin = reader.GetInt32(3) == 1;
            var user = new User(new Guid(idString), userName, isAdmin, chipId);
            users.Add(user);
        }
        reader.Close();
        connection.Close();
        return users;
    }

    public List<Entry> GetEntrys(Guid id)
    {
        var query = string.Format(SqlCommands.GetEntriesCommand, id);
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        var command = new OracleCommand(query, connection);
        var reader = command.ExecuteReader();
        var entries = new List<Entry>();
        while (reader.Read())
        {
            var entryIdString = reader.GetString(0);
            var startTime = reader.GetDateTime(1);
            var endTime = reader.GetDateTime(2);
            var duration = reader.GetString(3);
            var userIdString = reader.GetString(4);
            var entry = new Entry(new Guid(entryIdString), new Guid(userIdString), startTime, endTime, duration);
            entries.Add(entry);
        }
        reader.Close();
        connection.Close();
        return entries.OrderByDescending(e=> e.StartTimeStamp).ToList();
    }

    public List<int> GetYears()
    {
        using var connection = new OracleConnection(connectionString);
        connection.Open();
        var command = new OracleCommand(SqlCommands.GetYearsCommand, connection);
        var reader = command.ExecuteReader();
        var years = new List<int>();
        while (reader.Read())
        {
            var date = DateTime.Parse(reader.GetString(0));
            years.Add(date.Year);
        }
        reader.Close();
        connection.Close();
        return years.Distinct().ToList();
    }
}
