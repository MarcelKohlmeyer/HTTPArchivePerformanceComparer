using HTTPArchivePerformanceComparer.Models;
using HTTPArchivePerformanceComparer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HTTPArchivePerformanceComparer.Controllers;

[Route("httpArchives")]
[ApiController]
public class HttpArchivesController : ControllerBase
{
    private readonly IHttpArchiveService _httpArchiveService;
    public HttpArchivesController(IHttpArchiveService httpArchiveService)
    {
        _httpArchiveService = httpArchiveService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Dictionary<string, List<SimplifiedHttpArchive>>>> Get()
    {
        return Ok(await _httpArchiveService.GetSimplifiedHttpArchivesAsync());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<SimplifiedHttpArchive>>> AddHttpArchive(IFormFile file)
    {
        try
        {
            var json = JsonSerializer.Deserialize<HttpArchive>(file.OpenReadStream());
            var dictKey = file.FileName.Replace(' ', '_');
            var simplified = await _httpArchiveService.AddHttpArchiveAsync(json, dictKey);
            return Created(dictKey, simplified);
        } catch (Exception e)
        {
            return BadRequest($"The file does not seem to be a valid .HAR file: >{e.Message}<");
        }
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string name)
    {
        if (await _httpArchiveService.DeleteHttpArchiveAsync(name))
        {
            return Ok();
        }
        return NotFound();
    }

    [HttpGet("compare")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HttpArchiveCompareResult>> Compare()
    {
        try
        {
            return Ok(await _httpArchiveService.CompareHttpArchivesAsync());
        } 
        catch (KeyNotFoundException e)
        {
          return NotFound ($"Could not find more than one HttpArchive.");
        } 
        catch (Exception e)
        {
            return BadRequest($"Something went wrong: >{e.Message}<");
        }
    }
}

