public class EmailOptions
{
    public required Sender Sender { get; init; }
    public required Receiver Receiver { get; init; }

}

public class Sender
{
    public required string Name { get; init; }
    public required string Address { get; init; }
}

public class Receiver
{
    public required string Name { get; init; }
    public required string Address { get; init; }
}