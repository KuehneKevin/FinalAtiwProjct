using System;

namespace Zeiterfassung;

public class Entry
{
    public Entry(Guid id, Guid userId, DateTime? start, DateTime? end)
    {
        Id = id;
        UserId = userId;
        StartTimeStamp = GetCorrectTime((DateTime)start);
        EndTimeStamp = GetCorrectTime((DateTime)end);
        TimeSpan = (EndTimeStamp - StartTimeStamp).ToString(@"hh\:mm\:ss");
    }
    public Entry(Guid id, Guid userId, DateTime? start, DateTime? end, string timeSpan)
    {
        Id = id;
        UserId = userId;
        StartTimeStamp = GetCorrectTime((DateTime)start);
        EndTimeStamp = GetCorrectTime((DateTime)end);
        TimeSpan = timeSpan;
    }

    public Entry(Entry entry)
    {
        Id = entry.Id;
        UserId = entry.UserId;
        StartTimeStamp = GetCorrectTime(entry.StartTimeStamp);
        EndTimeStamp = GetCorrectTime(entry.EndTimeStamp);
        TimeSpan = entry.TimeSpan;
    }

    public static DateTime GetCorrectTime(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
    }

    private Guid Id { get; }
    private Guid UserId { get; }
    public DateTime StartTimeStamp { get; set; }
    public DateTime EndTimeStamp { get; set; }
    public string TimeSpan { get; set; }

    public Guid GetId()
    {
        return Id;
    }
    public Guid GetUserId()
    {
        return UserId;
    }
}
