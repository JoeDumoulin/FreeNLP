using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;
using NUnit.Framework;

namespace TestFilesAndFolders
{
  [TestFixture]
  public class TestTextTools
  {
    [Test]
    public void test_is_comment()
    {
      string test1 = @"****************";
      string test2 = @"*another comment";
      string test3 = @"not a comment";

      Assert.IsTrue(TextTools.is_comment(test1));
      Assert.IsTrue(TextTools.is_comment(test2));
      Assert.IsFalse(TextTools.is_comment(test3));
    }

    [Test]
    public void test_is_sentence_boundary()
    {
      string test1 = @"=============";
      string test2 = @"not a sentence boundary";

      Assert.IsTrue(TextTools.is_group_boundary(test1));
      Assert.IsFalse(TextTools.is_group_boundary(test2));
    }

        [Test]
    public void test__new_line_length()
    {
      string test0 = @"this is a test";
      string test1 = test0 + "\n";
      string test2 = test0 + "\r\n";

      Assert.AreEqual(TextTools.get_newline_length(test0), 0);
      Assert.AreEqual(TextTools.get_newline_length(test1), 1);
      Assert.AreEqual(TextTools.get_newline_length(test2), 2);
    }

    [Test]
    public void test_strip_new_line()
    {
      string matchstring1 = @"this is a test";
      string matchstring2 = @"this is a test ";
      string test1 = matchstring1 + "\r\n";
      string test2 = matchstring1 + "\n";

      Assert.AreEqual(TextTools.strip_new_line(test1), matchstring1);
      Assert.AreEqual(TextTools.strip_new_line(test2), matchstring1);

      Assert.AreEqual(TextTools.strip_new_line(test1, replace_with_space: true), matchstring2);
      Assert.AreEqual(TextTools.strip_new_line(test2, replace_with_space: true), matchstring2);
    }

    [Test]
    public void test_is_empty()
    {
      string test1 = "";
      string test2 = "\n";
      string test3 = "\r\n";

      Assert.IsTrue(TextTools.is_empty(test1));
      Assert.IsTrue(TextTools.is_empty(test2));
      Assert.IsTrue(TextTools.is_empty(test3));
    }

    [Test]
    public void test_is_NP()
    {
      string test1 = "[ this is a NP ]";
      string test2 = "this is not an NP\n";

      Assert.IsTrue(TextTools.is_NP(test1));
      Assert.IsFalse(TextTools.is_NP(test2));
    }

    [Test]
    public void test_trim_brackets()
    {
      string matchstring1 = " this is a NP ";
      string matchstring2 = "this is not an NP";
      string test1 = "[" + matchstring1 + "]";
      string test2 = matchstring2 + "\n";
      
      Assert.AreEqual(TextTools.trim_brackets(test1), matchstring1);
      Assert.AreEqual(TextTools.trim_brackets(test2), string.Empty);
    }

    [Test]
    public void test_get_term_from_tagged_term()
    {
      string matchstring1 = "Santa";
      string matchstring2 = "Clause";
      string test1 = matchstring1 + "/NNP";
      string test2 = matchstring2 + "/NNP";

      Assert.AreEqual(TextTools.get_term_from_tagged_term(test1), matchstring1);
      Assert.AreEqual(TextTools.get_term_from_tagged_term(test2), matchstring2);
      Assert.AreEqual(TextTools.get_term_from_tagged_term(matchstring1), "");
    }

    [Test]
    public void test_strip_tags_from_terms()
    {
      string matchstring1 = "Santa";
      string matchstring2 = "Clause";
      string matchstring = matchstring1 + " " + matchstring2;

      string test = matchstring1 + "/NP" + " " + matchstring2 + "/NP";

      MatchCollection ms = (new Regex(@"\S+")).Matches(test);
      Assert.AreEqual(TextTools.strip_tags_from_terms(ms).Aggregate((a1, a2) => a1 + " " + a2), matchstring);
    }

    [Test]
    public void test_get_text_from_line()
    {
      string matchstring1 = "Santa";
      string matchstring2 = "Clause";
      string matchstring = matchstring1 + " " + matchstring2;

      string test = matchstring1 + "/NP" + " " + matchstring2 + "/NP";

      Assert.AreEqual(TextTools.get_text_from_line(test), matchstring);
    }
  }
}
