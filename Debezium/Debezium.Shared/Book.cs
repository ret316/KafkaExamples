namespace Debezium.Shared;

public class Book
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public DateOnly Date { get; set; }
}
