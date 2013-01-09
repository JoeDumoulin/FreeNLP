using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
      var freq = new Frequencies<string>();
      freq.Add("a");
      Assert.AreEqual(1, freq.Get("a"));
    }

    [Test]
    public void test_terms()
    {
    // add some values to the freq
      var freq = new Frequencies<string>();
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
      var freq = new Frequencies<string>();
      foreach (var token in Regex.Split(text, @"(\W+)"))
      {
        freq.Add(token);
      }
      Assert.AreEqual(freq.Count(), 2227);
      Assert.AreEqual(freq.Get("and"), 47.0);
      Assert.AreEqual(freq.Terms().Count(), 479);
    }

    [Test]
    public void test_create_frequency_object_from_ngrams()
    {
      var text = TextExamples.emma();
      var freq = new Frequencies<string>();
      foreach (var token in Regex.Split(text, @"(\W+)").NGram(2))
      {
        freq.Add(token.Aggregate((a,b)=>a+" "+b));
      }

    }
  }
}
