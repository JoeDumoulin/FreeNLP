using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TestFilesAndFolders;
using Utilities;
using CorpusReader;
using FreeNLP.Learning;

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

    private static void test_log_likelihood_collocation()
    {
      var treebank3 = new Treebank3CorpusReader(Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3"));

      var bigram_freqs = treebank3.words()
                .Where((x) => x != ", " && !TextTools.is_puctuation(x) && TextTools.not_whitespace.IsMatch(x))
                .NGram(2).Select((a) => a.Aggregate((x, y) => x + " " + y))
                .Freqs();

      var unigram_freqs = treebank3.words()
          .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Freqs();

      foreach (var bigram_llr in LikelihoodRatio.log_likelihood_ratio(unigram_freqs, bigram_freqs).OrderBy((t)=>t.Value).Take(2000))
      {
        Console.WriteLine(String.Format(@"{0}: {1}", bigram_llr.Key, bigram_llr.Value));
      }
    }

    private static void test_log_likelihood_collocation2()
    {
      foreach (var bigram in CollocationLearning.collocations_from_likelihood_ratio_Treebank3().OrderBy((x)=>x.Value.log_lambda))
      {
        Console.WriteLine("{0}: {1}", bigram.Key, bigram.Value.log_lambda);
      }
    }

    private static void test_range()
    {
      var text = TextExamples.emma();
      foreach (var token in Regex.Split(text, @"(\W+)")
              .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Take(10))
      {
        Console.WriteLine(token);
      }
      foreach (var token in Regex.Split(text, @"(\W+)")
              .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Range(2,8))
      {
        Console.WriteLine(token);
      }
    }

    private static void test_slice()
    {
      var text = TextExamples.emma();
      foreach (var token in Regex.Split(text, @"(\W+)")
              .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Take(10))
      {
        Console.WriteLine(token);
      }
      foreach (var token in Regex.Split(text, @"(\W+)")
              .Where((x) => x != ", " && TextTools.not_whitespace.IsMatch(x)).Slice(2).Take(10))
      {
        Console.WriteLine(token);
      }
    }

    private static void read_treebank3_period_bigrams()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3");
      var treebank = new Treebank3CorpusReader(path);
      var t1 = treebank.words().Where(x=> (x != "") && x.ToCharArray().Last() == '.');
      var t2 = treebank.words().NGram(2).Where(a=>a.Last()==".").Select(a=>a.Aggregate((x,y)=>String.Concat(x,y)));
      var t_no_periods = treebank.words().Where(x=> (x != "") && x.ToCharArray().Last() != '.')
          .Where((x)=>!TextTools.is_puctuation(x)).NGram(2).Where(a=>a.Last()!=".")
          .Select(a=>a.Aggregate((x,y)=>String.Concat(x,y)));
      foreach (var content in t1.Union(t2))
      {
        //var x = content;
        Console.WriteLine(content);
      }
      Console.WriteLine(@"ending period: {0}", t1.Concat(t2).Count());
      Console.WriteLine(@" no ending period: {0}", t_no_periods.Count());
      Console.WriteLine(@" total: {0}", t_no_periods.Concat(t1.Concat(t2)).Count());
    }

    private static void get_punkt_statistics_from_treebank3()
    {
      //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Treebank-3");
      //var treebank = new Treebank3CorpusReader(path);
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Austen");
      var treebank = new TextCorpusReader(path);
      var stats = Punkt.process(treebank.words().Where(a=> a.Length <=7 || a.Substring(0,7)!="Speaker"));

      using (var writer = new StreamWriter("statistics.log"))
      {
        foreach (var stat in stats.type_classifier_results
                .OrderBy(a => a.statistics.scaled_llr)
                )
        {// TODO: output statistics to a log for review.
          writer.WriteLine(@"{0}, {1}, {2}, {3} {4} {5} {6} {7}", stat.statistics.scaled_llr
              , stat.statistics.c_w0, stat.statistics.c_w1, stat.statistics.c_w01, stat.statistics.length_penalty
              , stat.statistics.with_final_period_penalty, stat.statistics.w0, stat.classification);
//          Console.WriteLine(@"{0}, c(w,.)={1},  c(w,~.)={2} -> {3}", stat.statistics.scaled_llr, stat.statistics.c_w01, stat.statistics.c_w0 - stat.statistics.c_w01, stat.statistics.w01);
        }
      }
    }

    private static void get_instances_of_Mr_from_austen()
    {
      var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Data\Austen");
      var austen = new TextCorpusReader(path);
      var freq = new Frequencies<string>();
      var i = 0;
      foreach (var w in austen.words().NGram(2))
      {
        var term = w.First().Trim().Append(w.Last()).Trim();
        //if (term.Length > 1 && term.Substring(0, 2) == "Mr" && term.Substring(2, 1) != "." && term.Substring(2, 1) != "s")
        if (term.Length > 1 && term.Substring(0, 2) == "Mr")
        {
          freq.Add(term);
        }
      }
      foreach (var t in freq.Generate())
      {
        Console.WriteLine("{0} => {1}", t.Key, t.Value);
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
      //portuguese_word_frequency();

      //test_log_likelihood_collocation2();
      //test_range();
      //test_slice();
      //read_treebank3_period_bigrams();
      get_punkt_statistics_from_treebank3();
      //get_instances_of_Mr_from_austen();
    }
  }
} 
