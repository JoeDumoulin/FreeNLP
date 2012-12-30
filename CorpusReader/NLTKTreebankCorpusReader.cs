using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class NLTKTreebankCorpusReader : ICorpusReader
  {
    private string _path;
    private static string _sent_pattern = @""; // no extension.  have to parse for these manually.
    private static string _tagged_pattern = @"*.pos";
    private static string _parsed_pattern = @"*.prd";
    private static string _combined_pattern = @"*.mrg";

    public NLTKTreebankCorpusReader(string path)
    {
      _path = path;
    }

    public IEnumerable<string> fileids(string pattern=@"*.*")
    {
      foreach (var f in FilesAndFolders.ListFolderContentsFromBase(_path, pattern))
      {
        yield return f;
      }

    }

    public IEnumerable<string> read_raw(string fileid = "")
    {
      if (fileid != string.Empty)
        yield return FilesAndFolders.AllFileContents(fileid);
      else
      {
        foreach (var f in fileids())
        {
          if (Path.GetExtension(f) == _sent_pattern)
          { // raw sentence files in Treebank have no extension
            yield return FilesAndFolders.AllFileContents(f);
          }
        }
      }
    }

    public IEnumerable<string> read_tagged_sents(string fileid = "")
    {
      return read_sents(_tagged_pattern, fileid);
    }

    public IEnumerable<string> read_parsed_sents(string fileid = "")
    {
      return read_sents(_parsed_pattern, fileid);
    }

    public IEnumerable<string> read_combined_sents(string fileid = "")
    {
      return read_sents(_combined_pattern, fileid);
    }

    private IEnumerable<string> read_sents(string pattern, string fileid = "")
    {
      if (fileid != string.Empty)
        yield return FilesAndFolders.AllFileContents(fileid);
      else
      {
        foreach (var f in fileids(pattern))
        {
          yield return FilesAndFolders.AllFileContents(f);
        }
      }
    }

    public IEnumerable<string> words(string fileid = "")
    {
      foreach (var sent in read_raw(fileid))
      {
        foreach (var word in sent.Split(' '))
        {
          yield return word;
        }
      }
    }
  }
}
