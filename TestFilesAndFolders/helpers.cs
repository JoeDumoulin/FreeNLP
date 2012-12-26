using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Utilities;

namespace TestFilesAndFolders
{
  public static class helpers
  {
    public static string CreateFolderWithBasePath(string basePath, string folder)
    {
      string path = Path.Combine(basePath, folder);
      Directory.CreateDirectory(path);
      return path;
    }

    public static string CreateFolderInUserTemp(string folder)
    {
      return CreateFolderWithBasePath(Path.GetTempPath(), folder);
    }

    public static void CreateFileInFolder(string folder, string name)
    {
      File.Create(Path.Combine(folder, name));
    }

    public static IEnumerable<string>  create_test_folder_with_two_files_and_subfolder_with_one_file(string folderPath)
    {
      // create the test folder with two file and a sub-folder with one file
      string theTestFolder = helpers.CreateFolderWithBasePath(folderPath, @"theTestFolder");

      var testList = new List<string>();
      testList.Add(Path.Combine(folderPath, @"1.tmp"));
      testList.Add(Path.Combine(folderPath, @"2.tmp"));
      testList.Add(Path.Combine(theTestFolder, @"3.tmp"));

      // create files from full paths
      foreach (string f in testList)
      {
        File.Create(f);
      }
      return testList;
    }

    public static void try_to_open_folder()
    {
      string folderPath = CreateFolderInUserTemp(@"test_listing_folder_folders");
      foreach (string d in FilesAndFolders.ListFolderFolders(Path.Combine(folderPath, @"abc")))
      {
        Assert.AreEqual(d, d);
      }
    }

    public static Tuple<string, string> create_a_file_with_known_contents(string pathName)
    {
      string folderPath = CreateFolderInUserTemp(pathName);
      var theFilePath = Path.Combine(folderPath, @"emma.txt");
      string theText = TextExamples.emma();

      using (var f = new StreamWriter(theFilePath))
      {
        f.Write(theText);
      }
      return Tuple.Create(theFilePath, theText);
    }

  }
}
