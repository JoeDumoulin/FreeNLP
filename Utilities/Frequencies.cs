using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
  public class Frequencies<T>
  {
    private Dictionary<T, double> _counts;

    public Frequencies()
    {
      _counts = new Dictionary<T,double>();
    }

    public Frequencies(IEnumerable<T> data)
    {
      _counts = new Dictionary<T,double>();
      foreach (var term in data)
        Add(term);
    }

    public void Add(T term)
    {
      if (_counts.ContainsKey(term))
        _counts[term]++;
      else
      {
        _counts.Add(term, 1);
      }
    }

    public void AddCount(T term, double count)
    {
      if (_counts.ContainsKey(term))
        _counts[term] += count;
      else
      {
        _counts.Add(term, count);
      }
    }

    public IEnumerable<T> Terms()
    {
      foreach (var term in _counts.Keys)
      {
        yield return term;
      }
     }

    public double Get(T term)
    {
      return _counts[term];
    }

    public IEnumerable<KeyValuePair<T, double>> Generate()
    {
      foreach (var term in _counts)
      {
        yield return term;
      }
    }

    public double Count()
    {
      return _counts.Values.Sum();
    }
  }

  public class ConditionalFrequencies
  {
    Dictionary<string, Frequencies<string>> _counts;

    public ConditionalFrequencies()
    {
      _counts = new Dictionary<string, Frequencies<string>>();
    }

    public void Add(IEnumerable<string> termlist)
    {
      var data = condition_and_event_from_list(termlist);
      if (!_counts.ContainsKey(data.Key))
      {
        _counts.Add(data.Key, new Frequencies<string>());
      }

      _counts[data.Key].Add(data.Value);
    }

    public IEnumerable<string> Conditions()
    {
      foreach (var term in _counts)
      {
        yield return term.Key;
      }
    }

    public IEnumerable<Frequencies<string>> Values()
    {
      foreach (var terms in _counts)
      {
        yield return terms.Value;
      }
    }

    public IEnumerable<KeyValuePair<string, Frequencies<string>>> Generate()
    {
      foreach (var terms in _counts)
      {
        yield return terms;
      }
    }

    // The following code is used to create a string key.  
    // TODO: This can be replaced by implementing hashing on the key list.
    private KeyValuePair<string, string> condition_and_event_from_list(IEnumerable<string> termlist)
    {
      if (termlist.Count() < 2) throw new ArgumentException("termlist has < 2 terms.");

      using(var iterator = termlist.Reverse().GetEnumerator())
      {
        if (iterator.MoveNext())
        {
          var evt = iterator.Current;
          if (!iterator.MoveNext()) throw new ArgumentException("can't parse termlist");

          return new KeyValuePair<string, string>(
              make_base_list(iterator).Reverse().Aggregate((a,b) => a + " " + b), evt);
        }
        else throw new ArgumentException("can't parse termlist");
      }
    }

    private IEnumerable<string> make_base_list(IEnumerator<string> iterator)
    {
      do{
        yield return iterator.Current;
      } while (iterator.MoveNext());
    }
  }
}
