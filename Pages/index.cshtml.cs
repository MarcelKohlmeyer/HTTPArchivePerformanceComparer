using HTTPArchivePerformanceComparer.Models;
using HTTPArchivePerformanceComparer.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HTTPArchivePerformanceComparer.View;

public class HttpArchiveCompareResultViewWrapper: PageModel
{
    public List<HttpArchiveCompareResult> Data { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;

    private readonly IHttpArchiveService _httpArchiveService;

    public HttpArchiveCompareResultViewWrapper(IHttpArchiveService httpArchiveService)
    {
        _httpArchiveService = httpArchiveService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Data = await _httpArchiveService.CompareHttpArchivesAsync();
        } 
        catch (KeyNotFoundException ex)
        {
            // not enough files:
            ErrorMessage = "This only works if there are at least two HAR-Files!";
        } 
        catch(Exception ex)
        {
            // Something else
            ErrorMessage = ex.Message;
        }
    }
}
