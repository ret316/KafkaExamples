namespace Rebus.Shared;

public record RebusMessage
{
    public required string Message { get; set; }
}

public record RebusMessageWithId : RebusMessage
{
    public required string Id { get; set; }
}