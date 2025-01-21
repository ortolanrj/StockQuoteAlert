public class EmailOptions
{
    public Sender Sender { get; init; }
    public Receiver Receiver { get; init; }

}

public class Sender
{
    public string Name { get; init; }
    public string Address { get; init; }
}

public class Receiver
{
    public string Name { get; init; }
    public string Address { get; init; }
}