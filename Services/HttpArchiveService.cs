using HTTPArchivePerformanceComparer.Models;
using System;

namespace HTTPArchivePerformanceComparer.Services;

public class HttpArchiveService : IHttpArchiveService
{
    private static Dictionary<string, List<SimplifiedHttpArchive>> _httpArchives = [];

    public async Task<List<SimplifiedHttpArchive>> GetSimplifiedHttpArchivesAsync(HttpArchive input)
    {
        List<Task<SimplifiedHttpArchive>> tasks = [];
        foreach (var entry in input.log.entries)
        {
            tasks.Add(SimplifyEntry(entry));
        }

        return (await Task.WhenAll(tasks)).OrderBy(e => e.Url).ToList();
    }

    public async Task<List<SimplifiedHttpArchive>> AddHttpArchiveAsync(HttpArchive input, string name)
    {
        var simplified = await GetSimplifiedHttpArchivesAsync(input);
        _httpArchives.Add(name, simplified);
        return simplified;
    }

    private async Task<SimplifiedHttpArchive> SimplifyEntry(Entry entry)
    {
        return await Task.FromResult(new SimplifiedHttpArchive
        {
            Url = entry.request.url,
            Method = entry.request.method,
            Status = entry.response.status,
            Time = entry.time
        });
    }

    public async Task<Dictionary<string, List<SimplifiedHttpArchive>>> GetSimplifiedHttpArchivesAsync()
    {
        return await Task.FromResult(_httpArchives);
    }

    public Task<bool> DeleteHttpArchiveAsync(string name)
    {
        if (!_httpArchives.ContainsKey(name))
        {
            return Task.FromResult(false);
        }
        else
        {
            _httpArchives.Remove(name);
            return Task.FromResult(true);
        }
    }

    public async Task<List<HttpArchiveCompareResult>> CompareHttpArchivesAsync()
    {
        if (_httpArchives.Count < 2)
        {
            throw new KeyNotFoundException("There must be at least two archives to compare.");
        }

        List<HttpArchiveCompareResult> result = new();

        // Starting to compare the archives:
        foreach (var archive in _httpArchives)
        {
            var fileName = archive.Key;
            foreach (var entry in archive.Value)
            {
                // Removing query string from url (if exists):
                var entryUrl = new Uri(entry.Url);
                var cleanedEntryUrl = entryUrl.AbsoluteUri;
                if(!string.IsNullOrEmpty(entryUrl.Query))
                {
                    cleanedEntryUrl = cleanedEntryUrl.Replace(entryUrl.Query, string.Empty);
                }

                if (result.SingleOrDefault(e => e.Url == cleanedEntryUrl) is null)
                {
                    // Url doesn't exist in list, add it:
                    result.Add(new() { Url = cleanedEntryUrl });
                }

                // Add to existing item:
                result.Single(e => e.Url == cleanedEntryUrl).Durations.Add(new() { Name = fileName, Time = entry.Time});

            }
        }

        // Calculating average duration:
        foreach (var entry in result)
        {
            entry.AverageDuration = entry.Durations.GroupBy(d => d.Name).Select(g => new Duration { Name = g.Key, Time = g.Average(d => d.Time) }).ToList();
        }

        return await Task.FromResult(result);
    }
}

