namespace MassTransit.Shared;

public record MassTransitMessage
{
    public required string Message { get; init; }
}

public record MassTransitMessageWithId : MassTransitMessage
{
    public required string Id { get; set; }
}