using System;

namespace Zeiterfassung;

public class User
{
    public User(Guid id, string username, bool isAdmin, string? chipId)
    {
        Id = id;
        Username = username;
        IsAdmin = isAdmin;
        ChipId = chipId;
    }

    private Guid Id { get; }
    public string Username { get; }
    public bool IsAdmin { get; }
    private string? ChipId { get; }

    public Guid GetId() { return Id; }
    public string? GetChipId() { return ChipId; }
}
