using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace FreeNLP.Learning
{
  /// <summary>
  /// implementation of the Punkt unsupervised learning algorithm for sentence detection.
  /// </summary>
  /// <remarks>
  /// The algorithm below is derived from the description in "Unsupervised Multilingual 
  /// Sentence Boundary Detection" (Kiss & Strunk), 2006, Association for Computational Linguistics.
  /// The paper describes a set of heuristics for determining sentence boundaries on untagged data
  /// and then measured results of the heuristic on a number of different (european) languages.
  /// </remarks>
  public class Punkt
  {
    public class Punkt_Classification
    {
      public IEnumerable<Type_Classification> type_classifier_results;
    }

    /// <summary>
    /// stored values from the type classification stage
    /// </summary>
    public class Type_Classification
    {
      public Type_Classifier_Statistics statistics;
      public string classification; // <A>, <E>, <S>
    }

    /// <summary>
    /// Keep track of statistics for a log likelihood in a bigram.
    /// </summary>
    public class Type_Classifier_Statistics
    {
      public string w01; // the bigram
      public double c_w01; // bigram count
      public string w0;  // first word
      public double c_w0;  // count of first word
      public string w1;  // second word
      public double c_w1;  // count of second word
      public double p; // probability for the null hypothesis
      public double p0; // probability of the positive alternative case
      public double p1; // probability of the negative alternative case

      public double llr; // log likelihood ratio

      public double length_penalty; 
      public double internal_periods_penalty;
      public double with_final_period_penalty;

      public double scaled_llr;

    }

    /// <summary>
    /// create a Punkt object capable of determining sentence boundaries in this corpus
    /// </summary>
    /// <param name="corpus">
    /// The unigram representation of the corpus
    /// </param>
    public static Punkt_Classification process(IEnumerable<string> corpus)
    {
      // organize the corpus
      var period_final_bigrams = get_period_final_bigrams(corpus);
      var nonperiod_final_bigrams = get_non_period_final_bigrams(corpus);

      // get frequencies
      var period_final_bigram_frequencies = period_final_bigrams.Freqs();
      var nonperiod_final_bigram_frequencies = nonperiod_final_bigrams.Freqs();

      //// print nonperiod_final_bigram_frequencies
      //foreach (var t in nonperiod_final_bigram_frequencies.Generate())
      //{
      //  if (t.Key.Length > 2 && t.Key.Substring(0,2) == "Mr")
      //    Console.WriteLine(String.Format("{0}=>{1}",t.Key, t.Value));
      //}

      // unigram frequencies
      var unigram_freqs = get_nonperiod_unigrams(corpus).Freqs();

      // add period frequency to unigram freqs
      unigram_freqs.AddCount(".", period_final_bigrams.Count());
      foreach (var bigram in period_final_bigram_frequencies.Generate())
      {
        var term = bigram.Key.Substring(0, bigram.Key.Length - 1);
        unigram_freqs.AddCount(term, bigram.Value);
        //if (term == "Mr")
        //{
        //  Console.WriteLine(String.Format("{0}=>{1}", bigram.Key, bigram.Value));
        //}
      }

      // get period frequencies (from the period final corpus)
      var period_freq = period_final_bigrams.Count();

      var results = new Punkt_Classification();
      results.type_classifier_results = type_based_classification_stage(
                                period_final_bigram_frequencies
                              , nonperiod_final_bigram_frequencies
                              , unigram_freqs
                              , period_freq);

      return results;
    }

    /// <summary>
    /// Step 1: Type based classification of period final bigrams.  In this step, 
    /// we want to guess at a classification of period final bigrams whether they are 
    /// elipses, abbreviations, or sentence endings.  This procedure calculates the 
    /// parameters that remain constant over all the enumerations.
    /// </summary>
    /// <returns>a list of type classifications</returns>
    public static IEnumerable<Type_Classification> type_based_classification_stage(
                    Frequencies<string> period_final_bigram_frequencies
                  , Frequencies<string> nonperiod_final_bigram_frequencies
                  , Frequencies<string> unigram_freqs
                  , int period_freq)
    {
      // count of periods
      double cw1 = Convert.ToDouble(period_freq);
      // number of tokens in the corpus
      double N = unigram_freqs.Generate().Select(a=>a.Value).Sum() + cw1; 

      // Do Collocation Bond
      // The paper identifies special values for probabilities on the null hypothesis 
      // and the alternative (p. 5).
      double p = cw1/N;
      double p0 = 0.99;
      double p1 = 1.0 - p0;

      // The remaining parameters:
      // bigrams count (this is the same as the period count)
      double cw01 = cw1;

      // return the results for each period final bigram
      return enumerate_type_based_classification(period_final_bigram_frequencies
                      , unigram_freqs, N, cw1, p, p0, p1);
    }

    /// <summary>
    /// return a stream of the statistics found when calculating type classification step.
    /// </summary>
    /// <param name="period_final_bigram_frequencies"></param>
    /// <param name="unigram_freqs"></param>
    /// <param name="N"></param>
    /// <param name="cw01"></param>
    /// <param name="cw1"></param>
    /// <param name="p"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    public static IEnumerable<Type_Classification> enumerate_type_based_classification(
                  Frequencies<string> period_final_bigram_frequencies
                  , Frequencies<string> unigram_freqs
                  , double N
                  , double cw1
                  , double p
                  , double p0
                  , double p1
                  )
    {
      foreach (var bigram in period_final_bigram_frequencies.Generate())
      {
        var s = new Type_Classifier_Statistics();
        
        s.c_w1 = cw1;
        s.p = p;
        s.p0 = p0;
        s.p1 = p1;
        s.w1 = ".";
        s.w01 = bigram.Key;

        // split off the ending period
        s.w0 = bigram.Key.Substring(0, bigram.Key.Length-1);
        s.c_w01 = bigram.Value;
        // find the unigram frequency of this term.
        s.c_w0 = unigram_freqs.Get(s.w0);

        s.llr = LikelihoodRatio.logLambda(N, s.c_w01, s.c_w0, s.c_w1, s.p, s.p0, s.p1);

        // Calculate the length penalty
        s.length_penalty = 1/Math.Exp(s.w0.Length - count_internal_periods(s.w0));
        // Calculate the Internal periods penalty
        s.internal_periods_penalty = count_internal_periods(s.w0) + 1;
        // Calculate occurrances penalty without final period
        var len = s.w0.Length - count_internal_periods(s.w0);
        len = (len == 0) ? 1: len;
        s.with_final_period_penalty = 1/Math.Pow(len, (s.c_w0 - s.c_w01));
        //if (s.with_final_period_penalty == 0)
        //  Console.WriteLine(s.w0);
        // scaled log likelihood
        s.scaled_llr = s.llr * s.length_penalty * s.internal_periods_penalty * s.with_final_period_penalty;

        // calculate the final classifier for each sentence final bigram
        var c = new Type_Classification();
        c.statistics = s;
        // annotate with the classification
        if (s.scaled_llr < 0.3)
        {
          c.classification = "<S>";
        }
        else
        {
          if (s.w0.Length - count_internal_periods(s.w0) == 0)
            c.classification = "<E>";
          else 
            c.classification = "<A>";
        }
        yield return c;
      }
    }

    public static int count_internal_periods(string text)
    {
      return Regex.Matches(text, @"\.").Count;
    }

    /// <summary>
    /// we have to strip of final periods here because the 
    /// corpora are split inconsitently.  sometimes final periods are joined 
    ///  to the sentence and sometimes not.
    /// </summary>
    /// <param name="word_list">An enumerable of word tokens</param>
    /// <returns>unigram corpus with final periods stripped off and no punctuation</returns>
    public static IEnumerable<string> get_nonperiod_unigrams(IEnumerable<string> word_list)
    {
      return word_list.Select(a => (a != "") && (a.Last() == '.') ? (a.Substring(a.Count() - 1)) : (a))
                            .Where((x) => !TextTools.is_puctuation(x));
    }

    /// <summary>
    /// collect all the bigrams in the corpus that have a final period.  Some of these may be true bigrams, 
    /// but sometimes the periods are simply the fimal character of the unigram.  This routine handles both
    /// types.
    /// </summary>
    /// <param name="word_list">An enumerable of word tokens</param>
    /// <returns>a list of words with periods at the end.</returns>
    public static IEnumerable<string> get_period_final_bigrams(IEnumerable<string> word_list)
    {
      var t1 = word_list.Where(x => (x.Trim() != "") && (x.Trim() != ".") && x.ToCharArray().Last() == '.');
      var t2 = word_list.Where(x=>x.Trim() != "").NGram(2).Where(a => a.Last().Trim() == "." && a.First().Trim() != "")
        .Select(a => a.Aggregate((x, y) => x.Trim().Append(y).Trim()));
      return t1.Concat(t2);
    }

    /// <summary>
    /// Collect all the bigrams of the corpus where the final entry of the bigram is not a period.
    /// </summary>
    /// <param name="word_list">An enumerable of word tokens</param>
    /// <returns>All bigrams that do not have final periods.</returns>
    public static IEnumerable<string> get_non_period_final_bigrams(IEnumerable<string> word_list)
    {
      return word_list.Where(x => (x != ""))
          .NGram(2).Select(a => a.Aggregate((x, y) => x.Trim().Append(" ").Append(y)).Trim()).Where((t)=> t.Last() != '.');
    }
  }

} 
