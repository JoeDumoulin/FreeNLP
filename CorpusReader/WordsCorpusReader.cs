using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class WordsCorpusReader : ICorpusReader
  {
    private string _path;

    public WordsCorpusReader(string path)
    {
      _path = path;
    }

    public IEnumerable<string> fileids(string pattern=@"*.*")
    {
      foreach (var f in FilesAndFolders.ListFolderContentsFromBase(_path, pattern))
      {
        if (Path.GetFileName(f) != "README")
          yield return f;
      }
    }

    public IEnumerable<string> read_raw(string fileid = "")
    {
      if (fileid != string.Empty)
      {
        foreach(var line in FilesAndFolders.FileContentsByLine(fileid))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> read_raw()
    {
      foreach (var f in fileids())
      {
        foreach (var line in read_raw(f))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> words(string fileid = "")
    {
      return read_raw(Path.Combine(_path, fileid));
    }

  }
}
