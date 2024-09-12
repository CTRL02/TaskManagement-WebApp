namespace alot.Data
{
    public class MyRateLimitOptions
    {
        public const string MyRateLimit = "MyRateLimit";
        public int PermitLimit { get; set; }
        public int Window { get; set; }
        public int SegmentsPerWindow { get; set; }
        public int QueueLimit { get; set; }
    }
}
