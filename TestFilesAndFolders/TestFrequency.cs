using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Utilities;

namespace TestFilesAndFolders
{
  [TestFixture]
  public class TestFrequency
  {
    [Test]
    public void test_add_to_frequencies()
    {
      var freq = new Frequencies();
      freq.Add("a");
      Assert.AreEqual(1, freq.Get("a"));
    }

    [Test]
    public void test_terms()
    {
    // add some values to the freq
      var freq = new Frequencies();
      freq.Add("a");
      freq.Add("b");

      // check for equality
      IEnumerable<string> check = new List<string>() { "a", "b" };

      Assert.AreEqual(check, freq.Terms());
    }

    [Test]
    public void test_create_frequency_object_from_text()
    {
      var text = TextExamples.emma();
      //var freq = new Frequencies(text);
    }
  }
}
