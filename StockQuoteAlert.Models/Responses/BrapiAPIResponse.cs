public class BrapiAPIResponse
{
    public required Result[] Results { get; set; }
    public required DateTime RequestedAt { get; set; }
    public required string Took { get; set; }
}

public class Result
{
    public required decimal RegularMarketPrice { get; set; }
}
