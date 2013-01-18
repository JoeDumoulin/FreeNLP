using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Utilities;

namespace CorpusReader
{
  public class FlorestaCorpusReader : ICorpusReaderBase
  {
    private string _path;
    public FlorestaCorpusReader(string path)
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
        foreach (var line in FilesAndFolders.FileContentsByLine(fileid, Encoding.Default))
        {
          short test;
          if (line != null && line.Length > 2 && line.First() == '#'  && short.TryParse(line.Substring(1, 1), out test))
            yield return line;
        }
      }
    }

    public IEnumerable<string> read_raw()
    {
      foreach (var f in fileids())
      {
        Console.WriteLine(f);
        foreach (var line in read_raw(f))
        {
          yield return trim_raw_line(line);
        }
      }
    }

    private string trim_raw_line(string raw_line)
    {
      return raw_line.Split().Skip(2).Aggregate((a,b)=>a+" "+b);
    }

    public IEnumerable<string> words(string fileid = "")
    {
      foreach (var line in read_raw())
      {
        foreach (var word in line.Split())
        {
          yield return word;
        }
      }
    }
  }
}
