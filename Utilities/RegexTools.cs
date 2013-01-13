using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{

  /// <summary>
  /// RegexTools - some useful static methods for handling regex data peculiar to corpus work
  /// </summary>
  /// <remarks>
  /// 
  /// </remarks>
  public static class RegexTools
  {
  /// <summary>
    /// regex_filter_pattern supports the transformation of a pseudo regex pattern used to define a search 
    /// for parts of speech into a complete compiled regex suitable for the actual search.
  /// </summary>
  /// <param name="pattern">The pseudopattern that defines the search.
  /// </param>
  /// <returns>the full pattern string</returns>
    public static string regex_filter_pattern(string pattern)
    {
      var subpattern_list = new List<string>();

      // Match all the internal patterns
      MatchCollection internal_matches = Regex.Matches(pattern, @"{([^}]+)}([^{]+)?");
      foreach (Match match in internal_matches)
      {
        var subpattern = make_sub_pattern(match.Groups[1].Value);
        if (match.Groups[2].Success)
          subpattern += match.Groups[2].Value;
        subpattern_list.Add(subpattern);
      }
      return subpattern_list.Aggregate((a,b)=>a+b);
    }

    public static string make_sub_pattern(string tag_pattern)
    {
      return String.Format("(([^/\\s]+)/({0})\\s)", tag_pattern);
    }


  }
}
