namespace NServiceBus.Shared;

public record NServiceBusMessage
{
    public required string Message { get; init; }
}
