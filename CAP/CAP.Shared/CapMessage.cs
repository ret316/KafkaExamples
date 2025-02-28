namespace CAP.Shared;

public record CapMessage
{
    public required string Message { get; init; }
}

public record CapMessageWithId : CapMessage
{
    public required string Id { get; set; }
}