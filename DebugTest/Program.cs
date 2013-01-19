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
    private static void read_treebank3()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3");
      var treebank = new Treebank3CorpusReader(path);
      foreach (var content in treebank.words())
      {
        //var x = content;
        Console.WriteLine(content);
      }
    }

    private static void freqs_from_emma_sample()
    {
      var freq = new Frequencies<string>(TextExamples.emma().Split());
      foreach (var term in freq.Generate().OrderBy(p => p.Value))
      {
        Console.WriteLine(String.Format(@"{0}: {1}", term.Key, term.Value));
      }
    }

    private static void raw_text_from_nltk_treebank()
    {
      var treebank = new NLTKTreebankCorpusReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/Treebank"));
      foreach (var content in treebank.read_raw())
      {
        Console.WriteLine(content);
      }
    }

    private static void words_from_nltk_treebank()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/Treebank");
      var treebank = new NLTKTreebankCorpusReader(path);
      foreach (var word in treebank.words())
      {
        Console.WriteLine(word);
      }
    }

    private static void stopwords_reader()
    {
      var stops = new WordsCorpusReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/stopwords"));
      foreach (var stop in stops.words("english"))
      {
        Console.WriteLine(stop);
      }
    }

    private static void read_raw_text_from_washingtons_first_inagural_address()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\inaugural");
      var inaugural = new TextCorpusReader(path);
      foreach (var address in inaugural.read_raw(Path.Combine(path, "1789-Washington.txt")))
      {
        Console.WriteLine(address);
      }
    }

    private static void read_trigram_frequencies_from_inaugural_addresses()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\inaugural");
      var inaugural = new TextCorpusReader(path);
      var f = new Frequencies<string>();
      foreach (var address in inaugural.words().Where((x) => x != ", ").NGram(3))
      {
        f.Add(address.DefaultIfEmpty("").Aggregate((a, b) => a + " " + b));
      }
      foreach (var term in f.Generate().OrderBy(p => p.Value).Reverse().Take(10))
      {
        Console.WriteLine(String.Format(@"{0}: {1}", term.Key, term.Value));
      }
    }

    private static void conditional_frequency_basic_test()
    {
      var text1 = new List<string>(){"a", "b"};
      var cf = new ConditionalFrequencies();
      cf.Add(text1);
      cf.Add(text1);
      foreach (var term in cf.Conditions())
      {
        Console.WriteLine(term);
      }
      foreach (var term in cf.Values())
      {
        foreach (var t in term.Terms())
          Console.WriteLine("{0} => {1}", t, term.Get(t));
      }
    }

    private static void read_file_contents_one_word_at_a_time()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\inaugural\1789-Washington.txt");
      var r = new Regex(@"([^\s]+)");
      foreach (var term in r.Split(FilesAndFolders.AllFileContents(path)))
      {
        Console.WriteLine(term);
      }
    }

    private static void inaugural_ngram_conditional_frequencies()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\inaugural");
      var inaugural = new TextCorpusReader(path);
      var cf = new ConditionalFrequencies();

      foreach (var term in inaugural.words().Where((x) => x != ", ").NGram(3))
      {
        cf.Add(term);
      }
      foreach (var term in cf.Generate())
      {
        if (term.Value.Count() > 1)
        {
          Console.WriteLine(term.Key);
          foreach (var evt in term.Value.Generate())
          {
            Console.WriteLine("\t{0} -> {1}", evt.Key, evt.Value);
          }
        }
      }
    }

    private static void frequencies_of_ngrams_in_emma_sample()
    {
      var text = TextExamples.emma();
      var freq = new Frequencies<string>();
      foreach (var token in Regex.Split(text, @"(\W+)").Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).NGram(3))
      {
        freq.Add(token.Aggregate((a, b) => a + " " + b));
      }
      foreach (var term in freq.Generate().OrderBy(p => p.Value).Reverse().Take(10))
      {
        Console.WriteLine(String.Format(@"{0}: {1}", term.Key, term.Value));
      }
      Console.WriteLine(freq.Count());
      Console.WriteLine(freq.Terms().Count());
    }

    private static void frequencies_of_ngrams_in_emma_sample2()
    {
      var text = TextExamples.emma();
      foreach (var token in Regex.Split(text, @"(\W+)")
                .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x))
                .NGram(3).Select((a)=>a.Aggregate((x, y) => x + " " + y))
                .Freqs().Generate().OrderBy(p => p.Value).Reverse().Take(10)
                )
      {
        Console.WriteLine(String.Format(@"{0}: {1}", token.Key, token.Value));
      }
    }

    private static void test_regex_filter_for_noun_phrases_from_wsj()
    {
      var pattern = RegexTools.regex_filter_pattern("{J\\S+|N\\S+}{J\\S+|N\\S+|IN\\S|TO\\S}*{N\\S+}");
      Console.WriteLine(pattern);
      var treebank = new NLTKTreebankCorpusReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/Treebank"));
      foreach (var content in treebank.read_tagged_sents().Filter((x)=>Regex.Match(x, pattern).Groups[0].Value))
      {
        Console.WriteLine(content);
      }
    }

    private static void most_common_collocated_NPs_in_Treebank3()
    {
      var collocated_words_pattern = RegexTools.regex_filter_pattern("<J\\S+|N\\S+><J\\S+|N\\S+|IN\\S|TO\\S>*<N\\S+>");
      Console.WriteLine(collocated_words_pattern);

      var treebank3 = new Treebank3CorpusReader(Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3"));
      foreach (var content in treebank3.read_tagged_sents()
          .Select((x)=>Regex.Match(x, collocated_words_pattern).Groups[0].Value)
          .Select((x) => TextTools.get_term_from_string(x).DefaultIfEmpty("").Aggregate((a, b) => a + " " + b))
          .Freqs().Generate().OrderBy((x)=>x.Value).Reverse().Take(1000))
      {
        Console.WriteLine("{0} : {1}", content.Key, content.Value);
      }
    }

    private static void test_floresta_reader()
    {
      var floresta = new FlorestaCorpusReader(Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/floresta"));
      foreach ( var word in floresta.words().Select((x)=>TextTools.trim_punctuation(x)).Freqs().Generate().OrderBy((x)=>x.Value))
      {
        Console.WriteLine("{0}: {1}", word.Key, word.Value);
      }
    }

    private static void test_mac_morpho_reader()
    {
      var mac_morpho = new Mac_morphoCorpusReader(Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/mac_morpho"));

      foreach (var word in mac_morpho.words().Where((x)=>!TextTools.is_puctuation(x)).Freqs().Generate().OrderBy((x)=>x.Value))
      {
        Console.WriteLine("{0}: {1}", word.Key, word.Value);
      }
    }

    private static void portuguese_word_frequency()
    {
      var floresta = new FlorestaCorpusReader(Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/floresta"));
      var mac_morpho = new Mac_morphoCorpusReader(Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data/mac_morpho"));

      using (StreamWriter outfile = new StreamWriter(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "portuguese_words.txt"), false, Encoding.UTF8))
      {
        foreach (var word in mac_morpho.words().Where((x) => !TextTools.is_puctuation(x))
              .Concat(floresta.words().Select((x) => TextTools.trim_punctuation(x)))
              .Freqs().Generate().OrderBy((x) => x.Value).Reverse())
        {
          outfile.WriteLine("{0}: {1}", word.Key, word.Value);
        }
      }

    }

    static void Main(string[] args)
    {
      //read_treebank3();
      //freqs_from_emma_sample();
      //raw_text_from_nltk_treebank();
      //words_from_nltk_treebank();
      //stopwords_reader();
      //read_raw_text_from_washingtons_first_inagural_address();
      //read_trigram_frequencies_from_inaugural_addresses();
      //conditional_frequency_basic_test();
      //read_file_contents_one_word_at_a_time();

      //inaugural_ngram_conditional_frequencies();
      //frequencies_of_ngrams_in_emma_sample();
      //frequencies_of_ngrams_in_emma_sample2();

      //test_regex_filter_for_noun_phrases_from_wsj();
      //most_common_collocated_NPs_in_Treebank3();

      //test_floresta_reader();
      //test_mac_morpho_reader();
      portuguese_word_frequency();
    }
  }
}
