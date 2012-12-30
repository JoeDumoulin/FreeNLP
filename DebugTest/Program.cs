using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TestFilesAndFolders;
using Utilities;
using CorpusReader;

namespace DebugTest
{
  class Program
  {
    static void Main(string[] args)
    {
      //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3");
      //var treebank = new Treebank3CorpusReader(path);
      //foreach (var content in treebank.words())
      //{
      //  //var x = content;
      //  Console.WriteLine(content);
      //}
      //var freq = new Frequencies(TextExamples.emma());
      //foreach (var term in freq.Generator().OrderBy(p => p.Value))
      //{
      //  Console.WriteLine(String.Format(@"{0}: {1}", term.Key, term.Value));
      //}
      //var treebank = new NLTKTreebankCorpusReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/Treebank"));
      //foreach (var content in treebank.read_raw())
      //{
      //  Console.WriteLine(content);
      //}

      //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/Treebank");
      //var treebank = new NLTKTreebankCorpusReader(path);
      //foreach (var word in treebank.words())
      //{
      //  Console.WriteLine(word);
      //}

      var stops = new StopwordsCorpusReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/stopwords"));
      foreach (var stop in stops.words("english"))
      {
        Console.WriteLine(stop);
      }
      // A little test
      //var r = new Regex(@"([^\s]+)");
      //foreach (var term in r.Split(FileAndFolders.AllFileContents(path)))
      //{
      //  //    if (term.Length > 0 && term.Last() == '.') 
      //  Console.WriteLine(term);
      //}
    }
  }
}
