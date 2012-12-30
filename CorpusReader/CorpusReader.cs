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

    // reads multiple lines and puts them together between section boundaries.
    protected static IEnumerable<string> read_lines(Func<string, bool> group_boundary_pred
        , Func<bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter
        , bool preserve_boundary)
    {
      if (corpus_pred())
      {
        var term_list = new List<string>();
        foreach (var line in line_filter(line_generator))
        {
          if (line != String.Empty && !TextTools.is_metadata_line(line))
          {
            if (!group_boundary_pred(line))
            {
              term_list.Add(term_filter(line).DefaultIfEmpty("").Aggregate((a1, a2) => a1 + " " + a2));
            }
            if (group_boundary_pred(line))
            {
              if (preserve_boundary)
                term_list.Add(term_filter(line).DefaultIfEmpty("").Aggregate((a1, a2) => a1 + " " + a2));
              string new_line = term_list.DefaultIfEmpty("").Aggregate((x, y) => x + " " + y);
              if (new_line != "")
                yield return new_line;
              term_list.Clear();
            }
          }
        }
      }
    }

    public static IEnumerable<string> read_atis(Func<string, bool> group_boundary_pred
        , Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter
        , bool preserve_boundary)

    {
      Func<bool> atis_pred = () => corpus_pred(CorpusReader.atis);
      return read_lines(group_boundary_pred, atis_pred, line_generator, line_filter, term_filter, preserve_boundary);
    }

    public static IEnumerable<string> read_switchboard(Func<string, bool> group_boundary_pred
        , Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter
        , bool preserve_boundary)
    {
      Func<bool> switchboard_pred = () => corpus_pred(CorpusReader.switchboard);
      return read_lines(group_boundary_pred, switchboard_pred, line_generator, line_filter, term_filter, preserve_boundary);
    }

    public static IEnumerable<string> read_wsj(Func<string, bool> group_boundary_pred
        , Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter
        , bool preserve_boundary)
    {
      Func<bool> wsj_pred = () => corpus_pred(CorpusReader.wsj);
      return read_lines(group_boundary_pred, wsj_pred, line_generator, line_filter, term_filter, preserve_boundary);
    }

    public static IEnumerable<string> read_brown(Func<string, bool> group_boundary_pred
        , Func<Regex, bool> corpus_pred
        , Func<IEnumerable<string>> line_generator
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter
        , bool preserve_boundary)
    {
      Func<bool> brown_pred = () => corpus_pred(CorpusReader.brown);
      return read_lines(group_boundary_pred, brown_pred, line_generator, line_filter, term_filter, preserve_boundary);
    }


  }
}
