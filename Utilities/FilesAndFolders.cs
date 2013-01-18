using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utilities
{
  public class UtilityException : Exception
  {
    public UtilityException()
      : base()
    { }
    public UtilityException(string message)
      : base(message)
    { }
    public UtilityException(string message, Exception inner)
      : base(message, inner)
    { }
  }

  public class FilesAndFolders
  {
    private static DirectoryInfo GetFolderInfo(string folderPath)
    {
      try
      {
        if (Directory.Exists(folderPath))
          return new DirectoryInfo(folderPath);
        else throw new UtilityException(String.Format("problem accessing folder: {0}", folderPath));
      }
      catch (Exception ex)
      {
        throw new UtilityException("problem accessing folder", ex);
      }
    }

    public static IEnumerable<string> ListFolderFiles(string folder, string pattern = "*.*")
    {
      foreach (FileInfo f in GetFolderInfo(folder).EnumerateFiles(pattern))
      {
        yield return f.FullName;
      }
    }

    public static IEnumerable<string> ListFolderFolders(string folder)
    {
      foreach (DirectoryInfo d in GetFolderInfo(folder).EnumerateDirectories())
      {
        yield return d.FullName;
      }
    }

    // list the base folder's files and then recursively list files in subfolders.
    // subfolders are not listed if they are empty.
    public static IEnumerable<string> ListFolderContentsFromBase(string basePath, string pattern = "*.*")
    {
      foreach (string f in ListFolderFiles(basePath, pattern))
      {
        yield return f;
      }
      foreach (string d in ListFolderFolders(basePath))
      {
        foreach (string f in ListFolderContentsFromBase(d, pattern))
        {
          yield return f;
        }
      }
    }

    public static string AllFileContents(string filePath)
    {
      using (var instream = new FileStream(filePath, FileMode.Open))
      using (var reader = new StreamReader(instream, Encoding.UTF8))
      {
        return reader.ReadToEnd();
      }
    }

    public static IEnumerable<string> FileContentsByLine(string filePath)
    {
      return FileContentsByLine(filePath, Encoding.UTF8);
    }

    public static IEnumerable<string> FileContentsByLine(string filePath, Encoding encoding)
    {
      using (var instream = new FileStream(filePath, FileMode.Open))
      using (var reader = new StreamReader(instream, encoding))
      {
        while (!reader.EndOfStream)
          yield return reader.ReadLine();
      }
    }
  }
}
