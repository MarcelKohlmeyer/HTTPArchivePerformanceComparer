using HTTPArchivePerformanceComparer.Models;

namespace HTTPArchivePerformanceComparer.Services;

public interface IHttpArchiveService
{
    public Task<List<SimplifiedHttpArchive>> GetSimplifiedHttpArchivesAsync(HttpArchive input);
    public Task<List<SimplifiedHttpArchive>> AddHttpArchiveAsync(HttpArchive input, string name);

    public Task<Dictionary<string, List<SimplifiedHttpArchive>>> GetSimplifiedHttpArchivesAsync();
    public Task<bool> DeleteHttpArchiveAsync(string name);
    public Task<List<HttpArchiveCompareResult>> CompareHttpArchivesAsync();

}
