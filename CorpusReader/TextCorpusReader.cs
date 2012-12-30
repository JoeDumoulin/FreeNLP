using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class TextCorpusReader : ICorpusReader
  {
    private static Regex _splitter = new Regex(@"(\W+)");
    private string _path;

    public TextCorpusReader(string path)
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

    public IEnumerable<string> read_raw(string fileid = "")
    {
      if (fileid != string.Empty)
      {
        foreach (var line in FilesAndFolders.FileContentsByLine(fileid))
        {
          yield return line;
        }
      }
      else
      {
        foreach (var line in read_raw())
          yield return line;
      }
    }

    public IEnumerable<string> read_raw()
    {
      foreach (var f in fileids())
      {
        Console.WriteLine(f);
        foreach (var line in read_raw(f))
        {
          yield return line;
        }
      }
    }

    public IEnumerable<string> words(string fileid = "")
    {
      var path = "";
      if (fileid != String.Empty)
        path  = Path.Combine(_path, fileid);
      else
      {
        foreach (var f in fileids())
        {
          path  = Path.Combine(_path, f);
          foreach (var line in read_raw(path))
          {
            foreach (var token in _splitter.Split(line))
            {
              if (token != String.Empty && token != " ")
                yield return token;
            }
          }
        }
      }
    }

  }
}
