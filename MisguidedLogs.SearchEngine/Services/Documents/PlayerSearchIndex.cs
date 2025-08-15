using Lucene.Net.Documents;
using MisguidedLogs.SearchEngine.Models;
using MisguidedLogs.SearchEngine.Repositories;

namespace MisguidedLogs.SearchEngine.Services.Documents;

public record PlayerSearchIndex(string Id, string Name, Class Class, HashSet<PlayedCombinations> Combinations)
{
    public Document GetDocument(ProbabilityValues probability)
    {
        var doc = new Document
        {
            new TextField("playername", Name, Field.Store.YES),
            new TextField("id", Id, Field.Store.YES),
            new TextField("playersearchinfo", Name.ToLowerInvariant().Replace("-",""), Field.Store.NO)
        };

        return doc;
    }
}

public record PlayedCombinations(short BossId, Role Role, TalentSpec Spec);
