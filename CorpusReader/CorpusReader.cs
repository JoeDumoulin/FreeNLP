using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace CorpusReader
{

  public interface ICorpusReader
  {
    // pure virtual methods
    IEnumerable<string> fileids(string pattern = @"*.*");

    IEnumerable<string> read_raw(string fileid = "");
  }

  public class CorpusReader
  {
    private static Regex atis = new Regex("atis");
    private static Regex switchboard = new Regex("swbd");
    private static Regex wsj = new Regex("wsj");
    private static Regex brown = new Regex("brown");

    public static bool subcorpus_pred(Regex corpus, string match_string)
    {
      return corpus.IsMatch(match_string);
    }

    // reads multiple lines and puts them together beteen section boundaries.
    protected static IEnumerable<string> read_lines(
          Func<bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      if (corpus_pred())
      {
        var term_list = new List<string>();
        foreach (var line in line_filter(line_generator))
        {
          if (line != String.Empty && !TextTools.is_metadata_line(line))
          {
            if (!TextTools.is_group_boundary(line))
            {
              term_list.Add(term_filter(line).DefaultIfEmpty("").Aggregate((a1, a2) => a1 + " " + a2));
            }
            if (TextTools.is_group_boundary(line))
            {
              string new_line = term_list.DefaultIfEmpty("").Aggregate((x, y) => x + " " + y);
              if (new_line != "")
                yield return new_line;
              term_list.Clear();
            }
          }
        }
      }
    }

    public static IEnumerable<string> read_atis(Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)

    {
      return read_lines(() => corpus_pred(CorpusReader.atis), line_generator, line_filter, term_filter);
    }

    public static IEnumerable<string> read_switchboard(Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_lines(() => corpus_pred(CorpusReader.switchboard), line_generator, line_filter, term_filter);
    }

    public static IEnumerable<string> read_wsj(Func<Regex, bool> corpus_pred, Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_lines(() => corpus_pred(CorpusReader.wsj), line_generator, line_filter, term_filter);
    }

    public static IEnumerable<string> read_brown(Func<Regex, bool> corpus_pred, Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_lines(() => corpus_pred(CorpusReader.brown), line_generator, line_filter, term_filter);
    }


  }
}
