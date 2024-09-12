namespace ConsluentKafka.Shared;

public record ConfluentMessage
{
    public required string Message { get; init; }
}

public record ConfluentMessageWithId : ConfluentMessage
{
    public required string Id { get; set; }
}