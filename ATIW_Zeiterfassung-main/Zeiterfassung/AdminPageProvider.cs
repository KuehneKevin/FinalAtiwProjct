using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeiterfassung;

public class AdminPageProvider
{
    private readonly DbHandler dbHandler;

    public AdminPageProvider(DbHandler dbHandler)
    {
        this.dbHandler = dbHandler;
    }

    public List<User> GetUsers()
    {
        return dbHandler.GetUsers().Where(u => !u.IsAdmin).ToList();
    }

    public List<Entry> GetEntries(Guid id)
    {
        return dbHandler.GetEntrys(id);
    }

    public List<string> GetFilterOptions()
    {
        var list = new List<string>();
        var years = GetYears();
        var months = GetMonths();

        foreach (var year in years)
        {
            foreach (var month in months)
            {
                list.Add($"{month.ToString("00")}/{year}");
            }
        }
        return list;
    }
    private List<int> GetYears()
    {
        return dbHandler.GetYears();
    }

    private List<int> GetMonths()
    {
        return new List<int>() { 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
    }
}