namespace BatchProcessing.Abstractions.Configuration
{
    public class EngineConfig
    {
        public int WorkerCount { get; set; } = 10;

        public int MaxActiveEngine { get; set; } = 5;
    }
}
