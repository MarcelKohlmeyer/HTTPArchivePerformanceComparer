namespace HTTPArchivePerformanceComparer.Models;

public class SimplifiedHttpArchive
{
    public string Url { get; set; }
    public string Method { get; set; }
    public int Status { get; set; }
    public float Time { get; set; }
}
