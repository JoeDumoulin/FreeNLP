using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class Treebank3CorpusReader : ICorpusReader
  {
    private string _path;
    private static string _sent_pattern = @"*.pos"; // no extension.  have to parse for these manually.
    private static string _tagged_pattern = @"*.pos";
    private static string _parsed_pattern = @"*.prd";
    private static string _combined_pattern = @"*.mrg";


    public Treebank3CorpusReader(string path)
    {
      _path = path;
    }

    public IEnumerable<string> fileids(string pattern = @"*.*")
    {
      foreach (var f in FilesAndFolders.ListFolderContentsFromBase(_path, pattern))
      {
        yield return f;
      }
    }


    public IEnumerable<string> read_raw(string fileid = "")
    {
      if (fileid != string.Empty)
        foreach (var line in TextTools.get_text_from_file(fileid))
          yield return line;
      else
      {
        foreach (var f in fileids(_sent_pattern))
        {
          foreach (var line in TextTools.get_text_from_file(f))
          {
            if (!TextTools.is_empty(line))
            {
              yield return line;
            }
          }
        }
      }
    }

    public IEnumerable<string> read_sents(string fileid)
    {
      foreach (var line in TextTools.get_text_from_file(fileid))
      {
        if (!TextTools.is_empty(line))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> read_subcorpora(
        Func<string, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var f in fileids(_sent_pattern))
      {
        foreach (var line in CorpusReader.read_atis(f, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_switchboard(f, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_wsj(f, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_brown(f, line_filter, term_filter))
        {
          yield return line;
        }
      }
    }


    public IEnumerable<string> read_sents()
    {
      return read_subcorpora(TextTools.get_tagged_strings_from_file, TextTools.get_term_from_string);
    }

    public IEnumerable<string> read_tagged_sents()
    {
      return read_subcorpora(TextTools.get_tagged_strings_from_file, TextTools.get_tagged_term_from_string);
    }

    public IEnumerable<string> read_tags()
    {
      return read_subcorpora(TextTools.get_tagged_strings_from_file, TextTools.get_tag_from_string);
    }

  }
}
