using Lucene.Net.Index;
using Lucene.Net.Search;
using MisguidedLogs.SearchEngine.Models;

namespace MisguidedLogs.SearchEngine.Services;

public class Searcher(SearchEngine searchEngine)
{
    public IEnumerable<Player> GetPlayersByName(string name)
    {

        var searcher = searchEngine.GetSearcher();
        var query = new WildcardQuery(new Term("playersearchinfo", $"{name.ToLowerInvariant()}*"));

        var topDocs = searcher.Search(query, 5);
        var result = new List<Player>();
        foreach (var docs in topDocs.ScoreDocs)
        {
            var docHit = searcher.Doc(docs.Doc);
            var player = new Player(
                docHit.Get("id"),
                docHit.Get("playername")
            );
            result.Add(player);
        }
        return result;
    }
}
