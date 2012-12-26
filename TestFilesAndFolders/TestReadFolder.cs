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
  [TestFixture]
  public class TestReadFolder
  {
    [Test]
    public void test_listing_folder_files()
    {
      // Create a folder
      string folderPath = helpers.CreateFolderInUserTemp(@"test_listing_folder_files");
      // Add some files
      helpers.CreateFileInFolder(folderPath, @"1.tmp");
      helpers.CreateFileInFolder(folderPath, @"2.tmp");

      // check the files
      IEnumerable<string> filePaths = FilesAndFolders.ListFolderFiles(folderPath).OrderBy(f => f);
      Assert.AreEqual(Path.GetFileName(filePaths.First()), @"1.tmp");
      Assert.AreEqual(Path.GetFileName(filePaths.Last()), @"2.tmp");
    }

    [Test]
    public void test_listing_folder_folders()
    {
      string folderPath = helpers.CreateFolderInUserTemp(@"test_listing_folder_folders");
      string theTestFolder = helpers.CreateFolderWithBasePath(folderPath, @"theTestFolder");

      IEnumerable<string> folderPaths = FilesAndFolders.ListFolderFolders(folderPath).OrderBy(f => f);
      Assert.AreEqual(folderPaths.First(), theTestFolder);
    }

    [Test]
    public void test_listing_folder_contents()
    {
      string folderPath = helpers.CreateFolderInUserTemp(@"test_listing_folder_contents");
      var testList = helpers.create_test_folder_with_two_files_and_subfolder_with_one_file(folderPath);

      Assert.AreEqual(FilesAndFolders.ListFolderContentsFromBase(folderPath).OrderBy(f => f), testList);
    }

    [Test]
    // this test does not run until the output is enumerated due to deferred execution of the yield return in callee.
    public void no_folder_to_open_throws_exception()
    {
      Assert.Throws<UtilityException>(() => helpers.try_to_open_folder());
    }

    [Test]
    public void test_file_contents_match_expected()
    {
      var theData = helpers.create_a_file_with_known_contents(@"test_file_contents_match_expected");
      var path = theData.Item1;
      var text = theData.Item2;

      Assert.AreEqual(text, FilesAndFolders.AllFileContents(path));
    }

    [Test]
    public void test_file_contents_line_by_line()
    {
      var theData = helpers.create_a_file_with_known_contents(@"test_file_contents_line_by_line");
      var path = theData.Item1;

      IEnumerable<string> text = Regex.Split(theData.Item2, Environment.NewLine);
      var lineList = FilesAndFolders.FileContentsByLine(path);

      Assert.AreEqual(text, lineList);
    }
  }
}
