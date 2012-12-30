using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
  public class Frequencies
  {
    private Dictionary<string, double> _counts;

    public Frequencies()
    {
      _counts = new Dictionary<string,double>();
    }

    public Frequencies(string text)
    {
      _counts = new Dictionary<string, double>();
      var pattern = new Regex(@"(\W+)");
      foreach (var term in pattern.Split(text))
      {
        Add(term);
      }
    }

    public Frequencies(string text, Regex pattern)
    {
      _counts = new Dictionary<string, double>();
      foreach (var term in pattern.Split(text))
      {
        Add(term);
      }
    }

    public Frequencies(IEnumerable<string> data)
    {
      _counts = new Dictionary<string,double>();
      foreach (var term in data)
        Add(term);
    }

    public void Add(string term)
    {
      if (_counts.ContainsKey(term))
        _counts[term]++;
      else
      {
        _counts.Add(term, 1);
      }
    }

    public IEnumerable<string> Terms()
    {
      foreach (var term in _counts.Keys)
      {
        yield return term;
      }
     }

    public double Get(string term)
    {
      return _counts[term];
    }

    public IEnumerable<KeyValuePair<string, double>> Generator()
    {
      foreach (var term in _counts)
      {
        yield return term;
      }
    }
  }
}
