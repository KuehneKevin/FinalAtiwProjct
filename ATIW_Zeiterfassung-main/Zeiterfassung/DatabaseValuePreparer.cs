using System;

namespace Zeiterfassung;

public static class DatabaseValuePreparer
{
    public static string GetOracleDateTimeString(DateTime time)
    {
        return $"{time.Year}/{time.Month}/{time.Day} {time.Hour}:{time.Minute}:{time.Second}";
    }
}
