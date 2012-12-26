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

    protected static IEnumerable<string> read_tagged_lines(Regex corpus, string fileid)
    {
      return read_tagged_lines(corpus, fileid
        , TextTools.get_tagged_strings_from_file
        , TextTools.get_tagged_term_from_string);
    }

    protected static IEnumerable<string> read_tagged_lines(Regex corpus, string fileid
        , Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      if (corpus.IsMatch(fileid))
      {
        var term_list = new List<string>();
        foreach (var line in line_filter(fileid, term_filter))
        {
          //Console.WriteLine(line);
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

    public static IEnumerable<string> read_atis(string fileid
        , Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)

    {
      return read_tagged_lines(CorpusReader.atis, fileid, line_filter, term_filter);
    }

    public static IEnumerable<string> read_switchboard(string fileid
        , Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_tagged_lines(CorpusReader.switchboard, fileid, line_filter, term_filter);
    }

    public static IEnumerable<string> read_wsj(string fileid
        , Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_tagged_lines(CorpusReader.wsj, fileid, line_filter, term_filter);
    }

    public static IEnumerable<string> read_brown(string fileid
        , Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      return read_tagged_lines(CorpusReader.brown, fileid, line_filter, term_filter);
    }


  }
}
