using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
  /// <summary>
  /// a class to support log likelihood calculations on binomially distributed data 
  /// (like text in a corpus).
  /// </summary>
  /// <remarks>
  /// These calculations are explained very well in "Foundations of Statistical 
  /// Natural Language Processing", C Manning and H. Schutze, MIT Press 1999.
  /// around pages 172-175.
  /// </remarks>
  public static class LikelihoodRatio
  {
    /// <summary>
    /// A partial log likelihood calculation (without the binomial coefficient)
    /// suitable for likelihood ratio calcs.
    /// </summary>
    /// <param name="k">count of the test item in the training set</param>
    /// <param name="n">count of all compared items in the training set</param>
    /// <param name="p">ML probability of the item in k given all the items counted in n</param>
    /// <returns></returns>
    public static double logL(double k, double n, double p)
    {
      return k*Math.Log(p) + (n-k)*Math.Log(1-p);
    }

    /// <summary>
    /// Given unigram frequencies and conditional bigram frequencies, calculate the log lambda 
    /// for each bigram in the corpus.
    /// </summary>
    /// <param name="unigram_frequencies">frequency of each individual term</param>
    /// <param name="bigram_frequencies">frequency of each bigram</param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, double>> log_likelihood_ratio(Frequencies<string> unigram_frequencies
                                          , Frequencies<string> bigram_frequencies)
    {
      // calculate N = the number of terms in the corpus
      double N = unigram_frequencies.Count();

      // now for each bigram, 
      foreach (var bigram_count in bigram_frequencies.Generate())
      {
        // calculate c_0, c_1, and c_01 and return the lambda
        var c_01 = bigram_count.Value;

        var term_freqs = bigram_count.Key.Split().Select((t) => unigram_frequencies.Get(t));
        var c = (double[])term_freqs.ToArray();

        // Can we even calculate the value?
        if (c[1] != c_01 && c[0] != c_01)
        {
          // now calculate p, p_1 and p_2
          var p = c[1]/N;
          var p_1 = c_01/c[0];
          var p_2 = (c[1] - c_01)/(N - c[0]);

          // now we can get log lambda
          var ll = logL(c_01, c[0], p) + logL(c[1] - c_01, N - c[0], p)
                  - logL(c_01, c[0], p_1) - logL(c[1] - c_01, N - c[0], p_2);
          yield return new KeyValuePair<string, double> (bigram_count.Key, ll);
        }
      }
    }

    /// <summary>
    /// holds the data needed for reporting the likelihood ratio statistics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LikelihoodRatioData<T, S>
    {
      public LikelihoodRatioData(double N, S bigram
          , double bigram_count, T[] terms, Frequencies<T> term_freqs)
      {
        feature = bigram;
        c_01 = bigram_count;
        t = terms;
        c = terms.Select((term) => term_freqs.Get(term)).ToArray();

        p = c[1] / N;
        p_1 = c_01 / c[0];
        p_2 = (c[1] - c_01) / (N - c[0]);
        log_lambda = -2*lambda(c_01, N);
      }
      public S feature { get; set;}
      public T[] t { get; set; }
      public double[] c { get; set; }
      public double c_01 { get; set; }
      public double p { get; set; }
      public double p_1 { get; set; }
      public double p_2 { get; set; }
      public double log_lambda { get; set; } 

      public double lambda(double c_01, double N)
      {
        return logL(c_01, c[0], p) + logL(c[1] - c_01, N - c[0], p)
             - logL(c_01, c[0], p_1) - logL(c[1] - c_01, N - c[0], p_2);
      }
    }
  }
}
