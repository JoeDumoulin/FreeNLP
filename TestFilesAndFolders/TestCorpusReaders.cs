using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CorpusReader;
using Utilities;
using NUnit.Framework;


namespace TestFilesAndFolders
{
  public class TestCorpusReader : CorpusReader.CorpusReader
  {
    private string _path;
    private string _pattern;
    public TestCorpusReader(string path, string pattern = @"*.*")
    {
      _path = path;
      _pattern = pattern;
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
        foreach (var f in FilesAndFolders.ListFolderContentsFromBase(_path, _pattern))
        {
          yield return FilesAndFolders.AllFileContents(f);
        }
      }
    }

  }

  [TestFixture]
  public class TestCorpusReaders
  {
    [Test]
    public void test_corpus_reader_on_known_fileids()
    {
      string folderPath = helpers.CreateFolderInUserTemp(@"test_corpus_reader_on_known_fileids");
      var testList = helpers.create_test_folder_with_two_files_and_subfolder_with_one_file(folderPath);

      var corpus = new TestCorpusReader(folderPath);
      Assert.AreEqual(corpus.fileids(), testList);
    }

    [Test]
    public void test_corpus_reader_fileContents()
    {
      var theData = helpers.create_a_file_with_known_contents(@"test_file_contents_match_expected");
      var path = Path.GetDirectoryName(theData.Item1);
      var filePath = theData.Item1;
      var text = theData.Item2;

      Assert.AreEqual(text, (new TestCorpusReader(path)).read_raw().First());
      Assert.AreEqual(text, (new TestCorpusReader(path)).read_raw(filePath).First());
    }
  }


}
