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

    // term predicates
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
      if (text.Trim().First() == '=')
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

    public static string trim_brackets(string text)
    {
      Match m = bracketed_phrase.Match(text);
      return m.Groups[1].Value;
    }

    public static string get_term_from_tagged_term(string tagged_string)
    {
      return tagged_term.Match(tagged_string).Groups[1].Value;
    }

    // term filters
    public static IEnumerable<string> get_any_term_from_string(string line)
    {
      foreach (Match m in not_whitespace.Matches(line))
      {
        yield return m.Groups[0].Value;
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

    // line filters
    public static IEnumerable<string> get_tagged_strings_from_file(
          Func<IEnumerable<string>> line_generator
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var line in line_generator())
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


    public static IEnumerable<string> get_NP_strings_from_file(
          Func<IEnumerable<string>> line_generator
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var line in line_generator())
      {
        if (!is_empty(line) && !is_comment(line))
        {
          if (is_NP(line))
          {
            yield return "[";
            foreach (var term in term_filter(trim_brackets(line)))
            {
              yield return term;
            }
            yield return "]";
          }
          else if (is_group_boundary(line))
            yield return line;
        }
      }
    }

    public static IEnumerable<string> get_parsed_strings_from_file(
          Func<IEnumerable<string>> line_generator
        , Func<string, IEnumerable<string>> term_filter)
    {
      foreach (var line in line_generator())
      {
        if (!is_empty(line) && !is_comment(line))
        {
          yield return line;
        }
      }
    }

  }
}
