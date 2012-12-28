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
        foreach (var line in TextTools.get_tagged_strings_from_file(
                 () => FilesAndFolders.FileContentsByLine(fileid), TextTools.get_term_from_string))
          yield return line;
      else
      {
        foreach (var f in fileids(_sent_pattern))
        {
          foreach (var line in TextTools.get_tagged_strings_from_file(
                   () => FilesAndFolders.FileContentsByLine(fileid), TextTools.get_term_from_string))
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
      foreach (var line in TextTools.get_tagged_strings_from_file(
               () => FilesAndFolders.FileContentsByLine(fileid), TextTools.get_term_from_string))
      {
        if (!TextTools.is_empty(line))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> read_subcorpora(
        Func<Func<IEnumerable<string>>, Func<string, IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var f in fileids(_sent_pattern))
      {
        Func<IEnumerable<string>> lines = () => FilesAndFolders.FileContentsByLine(f);
        foreach (var line in CorpusReader.read_atis(f, lines, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_switchboard(f, lines, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_wsj(f, lines, line_filter, term_filter))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_brown(f, lines, line_filter, term_filter))
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

    //TODO: read_NP_Chunks
    public IEnumerable<string> read_NPs()
    {
      return read_subcorpora(TextTools.get_NP_strings_from_file, TextTools.get_any_term_from_string);
    }

    public IEnumerable<string> read_NP_terms()
    {
      return read_subcorpora(TextTools.get_NP_terms_from_file, TextTools.get_any_term_from_string);
    }


    // TODO: read_parses
  }
}
