using Microsoft.AspNetCore.Mvc;
using MisguidedLogs.SearchEngine.Models;
using MisguidedLogs.SearchEngine.Services;

namespace MisguidedLogs.SearchEngine.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(Searcher searcher, ILogger<SearchController> logger) : ControllerBase
{
    [HttpGet]
    public IEnumerable<Player> Get(string query)
    {
        var players = searcher.GetPlayersByName(query);
        return players;
    }

}
