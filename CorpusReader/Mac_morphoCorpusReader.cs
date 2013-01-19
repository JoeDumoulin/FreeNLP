using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class Mac_morphoCorpusReader : ICorpusReaderBase
  {
    private string _path;
    public Mac_morphoCorpusReader(string path)
    {
      _path = path;
    }

    public IEnumerable<string> fileids(string pattern = @"*.*")
    {
      foreach (var f in FilesAndFolders.ListFolderContentsFromBase(_path, pattern))
      {
        if (Path.GetFileName(f) != "README")
          yield return f;
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

    public IEnumerable<string> read_raw(string fileid = "")
    {
      if (fileid != "")
      {
        foreach (var line in FilesAndFolders.FileContentsByLine(fileid, Encoding.Default))
        {
          yield return line;
        }
      }

    }

    public IEnumerable<string> words(string fileid = "")
    {
      if (fileid != "")
      {
        var path = Path.Combine(_path, fileid);
        foreach (var line in read_raw(path))
        {
          yield return line.Split('_').First();
        }
      }
      else
      {
        foreach(var line in read_raw())
        {
          yield return line.Split('_').First();
        }
      }
    }
  }
}
