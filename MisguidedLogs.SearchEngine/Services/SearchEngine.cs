using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using MisguidedLogs.SearchEngine.Repositories;
using MisguidedLogs.SearchEngine.Repositories.Bunnycdn;
using MisguidedLogs.SearchEngine.Services.Documents;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace MisguidedLogs.SearchEngine.Services;

public class SearchEngine
{
    private const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
    private readonly BunnyCdnStorageLoader loader;
    private readonly ProbabilityService probabilityService;
    private readonly IndexWriter indexWriter;
    private readonly LuceneDirectory indexDir;
    private static bool IsUpdating;

    public SearchEngine(BunnyCdnStorageLoader loader, ProbabilityService probabilityService)
    {
        indexDir = new RAMDirectory();
      

        // Create an analyzer to process the text 
        var standardAnalyzer = new MultiLanguageAnalyzer(luceneVersion);

        //Create an index writer
        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };

        indexWriter = new IndexWriter(indexDir, indexConfig);
        this.loader = loader;
        this.probabilityService = probabilityService;
        reader = indexWriter.GetReader(applyAllDeletes: true);
    }

    public async Task UpdateAllDocuments()
    {
        if (IsUpdating)
        {
            return;
        }

        IsUpdating = true;
        var probability = await probabilityService.GetProbabilityValuesAsync(2018); // set in config to begin with, can't support all zones yet
        var playersInfo = await loader.GetStorageObject<HashSet<PlayerSearchIndex>>("misguided-logs-warcraftlogs/gold/searchValues.json.gz");
        if (playersInfo is null)
        {
            return;
        }
        foreach (var playerInfo in playersInfo)
        {
            var term = new Term("id", playerInfo.Id);
            indexWriter.UpdateDocument(term, playerInfo.GetDocument(probability));
        }
        indexWriter.Commit();

        IsUpdating = false;
    }

    private DirectoryReader reader;
    public IndexSearcher GetSearcher()
    {
        var newReader = DirectoryReader.OpenIfChanged(reader);

        if (newReader != null)
        {
            reader = newReader;
        }

        return new IndexSearcher(reader);
    }
}