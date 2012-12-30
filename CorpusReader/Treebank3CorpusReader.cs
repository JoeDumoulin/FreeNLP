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

    public IEnumerable<string> words(string fileid = "")
    {
      foreach (var sent in read_sents(fileid))
      {
        foreach (var word in sent.Split(' '))
        {
          yield return word;
        }
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

    public IEnumerable<string> read_sents(string fileid = "")
    {
      if (fileid=="")
      {
        foreach (var line in read_sents())
        {
          yield return line;
        }
      }
      else
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

    public IEnumerable<string> read_subcorpora(string file_pattern
        , Func<string, bool> group_boundary_pred
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter, bool preserve_boundary)
    {
      foreach (var f in fileids(file_pattern))
      {
        Func<Regex, bool> corpus_pred = (regex) => CorpusReader.subcorpus_pred(regex, f);
        Func<IEnumerable<string>> line_generator = () => FilesAndFolders.FileContentsByLine(f);

        foreach (var line in CorpusReader.read_atis(group_boundary_pred, corpus_pred, line_generator, line_filter, term_filter, preserve_boundary))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_switchboard(group_boundary_pred, corpus_pred, line_generator, line_filter, term_filter, preserve_boundary))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_wsj(group_boundary_pred, corpus_pred, line_generator, line_filter, term_filter, preserve_boundary))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_brown(group_boundary_pred, corpus_pred, line_generator, line_filter, term_filter, preserve_boundary))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> read_parsed_subcorpora(string file_pattern
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var f in fileids(file_pattern))
      {
        Func<Regex, bool> corpus_pred = (regex) => CorpusReader.subcorpus_pred(regex, f);
        Func<IEnumerable<string>> line_generator = () => FilesAndFolders.FileContentsByLine(f);

        foreach (var line in CorpusReader.read_atis((x) => x.Trim() == "(  END_OF_TEXT_UNIT)" ? true : false, corpus_pred, line_generator, line_filter, term_filter, false))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_switchboard((x) => x.Trim() == "E_S))", corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_wsj((x) => x.Trim() == ".))", corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_brown((x) => x.Trim() == ".))", corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> read_tagged_parsed_subcorpora(string file_pattern
        , Func<Func<IEnumerable<string>>, IEnumerable<string>> line_filter
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var f in fileids(file_pattern))
      {
        Func<Regex, bool> corpus_pred = (regex) => CorpusReader.subcorpus_pred(regex, f);
        Func<IEnumerable<string>> line_generator = () => FilesAndFolders.FileContentsByLine(f);

        foreach (var line in CorpusReader.read_atis((x) => x.Trim() == "( END_OF_TEXT_UNIT)" ? true : false, corpus_pred, line_generator, line_filter, term_filter, false))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_switchboard((x) => Regex.IsMatch(x, @"E_S"), corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_wsj((x) => x.Trim() == "(. .) ))", corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
        foreach (var line in CorpusReader.read_brown((x) => x.Trim() == "(. .) ))", corpus_pred, line_generator, line_filter, term_filter, true))
        {
          yield return line;
        }
      }
    }

    // raw (tagged) sections
    public IEnumerable<string> read_sents()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_tagged_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_term_from_string, false);
    }

    public IEnumerable<string> read_tagged_sents()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_tagged_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_tagged_term_from_string, false);
    }

    public IEnumerable<string> read_tags()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_tagged_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_tag_from_string, false);
    }

    // read_NP_Chunks
    public IEnumerable<string> read_NPs()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_NP_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_any_term_from_string, false);
    }

    public IEnumerable<string> read_NP_terms()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_NP_strings_from_file(lg, TextTools.get_term_from_string)
          , TextTools.get_any_term_from_string, false);
    }

    public IEnumerable<string> read_NP_tags()
    {
      return read_subcorpora(_sent_pattern, TextTools.is_group_boundary
          , (lg) => TextTools.get_NP_strings_from_file(lg, TextTools.get_tag_from_string)
          , TextTools.get_any_term_from_string, false);
    }


    // read_parses
    public IEnumerable<string> read_parse_trees()
    {
      return read_parsed_subcorpora(_parsed_pattern
          , (lg) => TextTools.get_parsed_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_any_term_from_string);
    }

    public IEnumerable<string> read_tagged_parse_trees()
    {
      return read_tagged_parsed_subcorpora(_combined_pattern
          , (lg) => TextTools.get_parsed_strings_from_file(lg, TextTools.get_any_term_from_string)
          , TextTools.get_any_term_from_string);
    }
  }
}
