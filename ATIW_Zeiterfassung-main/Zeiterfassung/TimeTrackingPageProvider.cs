using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeiterfassung;

public class TimeTrackingPageProvider
{
    private readonly DbHandler dbHandler;

    public TimeTrackingPageProvider(DbHandler dbHandler)
    {
        this.dbHandler = dbHandler;
    }

    public User? GetUser(string chipId)
    {
        var users = dbHandler.GetUsers();
        return users.SingleOrDefault(u => u.GetChipId() == chipId);
    }

    public DateTime? CreateEntryOrCheckIn(User user, bool IsCheckedIn, DateTime? checkInTimeStamp, List<Entry> entrys)
    {
        if (IsCheckedIn)
        {
            var entry = new Entry(Guid.NewGuid(), user.GetId(), checkInTimeStamp, DateTime.Now);
            dbHandler.SafeEntryInDatabase(entry);
            entrys.Add(entry);
            return null;
        }
        return DateTime.Now;
    }

    public string GetButtonContent(bool isCheckedIn)
    {
        return isCheckedIn ? "Austempeln" : "Einstempeln";
    }

    public List<Entry> GetEntries(Guid id)
    {
        return dbHandler.GetEntrys(id);
    }
}
