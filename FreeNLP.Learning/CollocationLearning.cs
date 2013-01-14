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
          .Filter((x) => Regex.Match(x, collocated_words_pattern).Groups[0].Value)
          .Filter((x) => TextTools.get_term_from_string(x).DefaultIfEmpty("").Aggregate((a, b) => a + " " + b))
          .Freqs().Generate().OrderBy((x) => x.Value))
      {
        yield return content;
      }
    }


  }
}

