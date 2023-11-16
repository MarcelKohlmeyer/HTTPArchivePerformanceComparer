namespace HTTPArchivePerformanceComparer.Models;

public class HttpArchiveCompareResult
{
    public string Url { get; set; }
    public List<Duration> Durations { get; set; } = new();
    public List<Duration> AverageDuration { get; set; } = new();
}

public class Duration
{
    public string Name { get; set; }
    public float Time { get; set; }
}
