using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
  public class TextTools
  {
    public static Regex whitespace = new Regex(@"\s+");
    public static Regex not_whitespace = new Regex(@"\S+");
    public static Regex term_pattern = new Regex(@"[^/]+]");
    public static Regex bracketed_phrase = new Regex(@"\[([^\]]+)\]");
    public static Regex tagged_term = new Regex(@"([^/ ]+)/(\S+)");
    public static Regex metadata_term = new Regex(@"@([^\]]+)");

    private static string windows_newline = "\r\n";
    private static string unix_newline = "\n"; 

    public static bool is_comment(string text)
    {
      if (text.First() == '*')
      {
        return true;
      }
      return false;
    }

    public static bool is_group_boundary(string text)
    {
      if (text.First() == '=')
      {
        return true;
      }
      return false;
    }

    public static bool is_metadata_line(string text)
    {
      if (metadata_term.IsMatch(text)) return true;
      return false;
    }

    public static int get_newline_length(string text)
    {
      if (text.EndsWith(windows_newline))
      {
        return windows_newline.Length;
      }
      else if (text.EndsWith(unix_newline))
      {
        return unix_newline.Length;
      }
      return 0;
    }

    public static string strip_new_line(string text, bool replace_with_space=false)
    {
      var newline_len = get_newline_length(text);
      return replace_with_space? 
          text.Substring(0,text.Length-newline_len)+ " " 
        : text.Substring(0,text.Length-newline_len) ;
    }

    public static bool is_empty(string text)
    { // true only if the entire string is whitespace
      Match m = whitespace.Match(text);
      if (m.Length == text.Length)
      {
        return true;
      }
      return false;
    }

    public static bool is_NP(string text)
    {
      if (text.Trim().First() == '[')
      {
        return true;
      }
      return false;
    }

    //public bool is_phrase(string text)
    //{
    //  if (text != null && text.Trim().First() == '[')
    //  {
    //    return false;
    //  }
    //  return true;
    //}

    public static string trim_brackets(string text)
    {
      Match m = bracketed_phrase.Match(text);
      return m.Groups[1].Value;
    }

    public static string get_term_from_tagged_term(string tagged_string)
    {
      return tagged_term.Match(tagged_string).Groups[1].Value;
    }

    public static IEnumerable<string> strip_tags_from_terms(MatchCollection terms)
    {
      foreach (Match m in terms)
      {
        if (m.Success)
        {
          string term = get_term_from_tagged_term(m.Groups[0].Value);
          if (term != null)
          {
            yield return term;
          } 
        }
      }
    }

    public static string get_text_from_line(string line)
    {
      MatchCollection ms = not_whitespace.Matches(line);
      return  strip_tags_from_terms(ms).DefaultIfEmpty("").Aggregate((a1, a2) => a1 + " " + a2);
    }

    public static IEnumerable<string> get_text_from_file(string fileid)
    {
      foreach (var line in FilesAndFolders.FileContentsByLine(fileid))
      {
        // Ignore comment lines
        if (!is_empty(line) && !is_comment(line))
        {
          if (is_NP(line))
          {
            yield return get_text_from_line(trim_brackets(line));
          }
          else
          {
            yield return get_text_from_line(line);
          }
        }
      }
    }

    public static IEnumerable<string> get_tagged_term_from_string(string line)
    {
      foreach (Match m in tagged_term.Matches(line))
      {
        yield return m.Groups[0].Value;
      }
    }

    public static IEnumerable<string> get_term_from_string(string line)
    {
      foreach (Match m in tagged_term.Matches(line))
      {
        yield return m.Groups[1].Value;
      }
    }

    public static IEnumerable<string> get_tag_from_string(string line)
    {
      foreach (Match m in tagged_term.Matches(line))
      {
        yield return m.Groups[2].Value;
      }
    }


    public static IEnumerable<string> get_tagged_strings_from_file(string fileid)
    {
      return get_tagged_strings_from_file(fileid, get_tagged_term_from_string);
    }

    public static IEnumerable<string> get_tagged_strings_from_file(string fileid, Func<string, IEnumerable<string>> tagged_term_filter)
    {
      foreach (var line in FilesAndFolders.FileContentsByLine(fileid))
      {
        if (!is_empty(line) && !is_comment(line))
        {
          if (is_NP(line))
          {
            yield return trim_brackets(line);
          }
          else
          {
            yield return line;
          }
        }
      }
    }
  }
}
