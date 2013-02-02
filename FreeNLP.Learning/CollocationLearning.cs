using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CorpusReader;
using Utilities;

namespace FreeNLP.Learning
{
  // Learn Collocation pairs and triples according to patterns
  // the patterns are from:
  // Justeson, John S., and Slava M. Katz. 1995. Technical terminology: some linguistic properties and 
  //  an algorithm for identification in text. Natural Language Engineering 1:9-27.
  /// <summary>
  /// Collocation Learning and Storage
  /// </summary>
  public static class CollocationLearning
  {
    /// <summary>
    /// Learn Collocation pairs and triples according to patterns in Treebank-3 tagged files
    /// </summary>
    /// <remarks>
    /// the patterns are from:
    /// Justeson, John S., and Slava M. Katz. 1995. Technical terminology: some linguistic properties and 
    ///  an algorithm for identification in text. Natural Language Engineering 1:9-27.
    /// </remarks>
    /// <returns>IEnumerable of sorted frequencies (KeyValuePairs of collocated terms 
    /// and the number of occurrrences of that string) of each collocation found in the corpus.
    /// </returns>
    public static IEnumerable<KeyValuePair<string, double>> collocated_terms_in_Treebank3()
    {
      var collocated_words_pattern = RegexTools.regex_filter_pattern("<J\\S+|N\\S+><J\\S+|N\\S+|IN\\S|TO\\S>*<N\\S+>");
      //Console.WriteLine(collocated_words_pattern);

      var treebank3 = new Treebank3CorpusReader(Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3"));
      foreach (var content in treebank3.read_tagged_sents()
          .Select((x) => Regex.Match(x, collocated_words_pattern).Groups[0].Value)
          .Select((x) => TextTools.get_term_from_string(x).DefaultIfEmpty("").Aggregate((a, b) => a + " " + b))
          .Freqs().Generate().OrderBy((x) => x.Value))
      {
        yield return content;
      }
    }

    public static IEnumerable<KeyValuePair<string, LikelihoodRatio.LikelihoodRatioData<string, string>>> 
        collocations_from_likelihood_ratio_Treebank3()
    {
      var treebank3 = new Treebank3CorpusReader(Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3"));

      var bigram_freqs = treebank3.words()
                .Where((x) => x != ", " && !TextTools.is_puctuation(x) && TextTools.not_whitespace.IsMatch(x))
                .NGram(2).Select((a) => a.Aggregate((x, y) => x + " " + y))
                .Freqs();

      var unigram_freqs = treebank3.words()
          .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Freqs();
      var N = bigram_freqs.Count();

      foreach (var bigram_llr in bigram_freqs.Generate())
      {
        var llr = new LikelihoodRatio.LikelihoodRatioData<string, string>(N
                  , bigram_llr.Key, bigram_llr.Value, bigram_llr.Key.Split().ToArray(), unigram_freqs);
        {
          yield return new KeyValuePair<string, LikelihoodRatio.LikelihoodRatioData<string, string>>(bigram_llr.Key, llr);
        }
      }
    }
  }
}

