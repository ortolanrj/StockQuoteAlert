public class BrapiAPIResponse
{
    public Result[] Results { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Took { get; set; }
}

public class Result
{
    public decimal RegularMarketPrice { get; set; }
}
