using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace MisguidedLogs.SearchEngine.Services;

public class MultiLanguageAnalyzer(LuceneVersion version) : Analyzer
{
    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
    {
        if (fieldName is "id")
        {
            return new TokenStreamComponents(new KeywordTokenizer(reader));
        }
        var tokenizer = new WhitespaceTokenizer(version, reader);
        TokenStream tokenFilter = new StandardFilter(version, tokenizer);
        tokenFilter = new EdgeNGramTokenFilter(version, tokenFilter, 1, 15);
        return new TokenStreamComponents(tokenizer, tokenFilter);
    }
}
