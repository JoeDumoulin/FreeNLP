using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
  public static class EnumerableExtension
  {
    // split the enumerable object into enumerable chunks.
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (size < 0) throw new ArgumentOutOfRangeException("size", 
          @"the ValueType of 'size' must be positive");

      // Get the first level enumerator
      using (IEnumerator<T> iterator = source.GetEnumerator())
      {
        for (int i = 0; i < source.Count() - size; i++)
        {
          yield return ChunkSequence(iterator, size);
        }
      }
    }

    private static IEnumerable<T> ChunkSequence<T>(IEnumerator<T> iterator, int size)
    {
      // get a new iterator for the ngram
      for (int i = size; i > 0; i--)
      {
        if (!iterator.MoveNext())
          yield break;
        yield return iterator.Current;
      }
    }

    public static IEnumerable<IEnumerable<T>> NGram<T>(this IEnumerable<T> source, int size)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (size < 0) throw new ArgumentOutOfRangeException("size", 
          @"the ValueType of 'size' must be positive");

      using (IEnumerator<T> iterator = source.GetEnumerator())
      {
        var seq = new Queue<T>(ChunkSequence(iterator, size));
        yield return seq;
        for (;;)
        {
          if (!iterator.MoveNext())
            yield break;
          
          seq.Dequeue();
          seq.Enqueue(iterator.Current);
          yield return seq;
        }
      }
    }

    public static Frequencies<T> Freqs<T>(this IEnumerable<T> source)
    {
      return new Frequencies<T>(source);
    }

    // this is actually just Where<>((t)=>filter_predicate(t))
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> filter_predicate)
    {
      foreach (var value in source)
      {
        if (filter_predicate(value))
        {
          yield return value;
        }
      }
    }
    // this is actually just Select((t)=>filter_func(t))
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, T> filter_func)
    {
      foreach (var value in source)
      {
        var filtered_value = filter_func(value);
        if (filtered_value != null)
        {
          yield return filtered_value;
        }
      }
    }

    /// <summary>
    /// return a container that is in a bounded range of the source container
    /// </summary>
    /// <typeparam name="T">the container type</typeparam>
    /// <param name="source">The source container</param>
    /// <param name="start">the start of the range</param>
    /// <param name="distance">the maximum length of the container to return.</param>
    /// <returns>a container of maximum length 'distance'.  If the source 
    /// container size is less than start then IndexOutOfRangeException is thrown.
    /// if container size is less than start+distance, then size - start elements
    /// are returned.</returns>
    public static IEnumerable<T> Range<T>(this IEnumerable<T> source, int start, int distance)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (start < 0) throw new ArgumentOutOfRangeException("start", 
          @"'start' must be positive");
      if (distance < 0) throw new ArgumentOutOfRangeException("distance", 
          @"'distance' must be positive");

      int there = 0;
      using (IEnumerator<T> iterator = source.GetEnumerator())
      {
        while (there != start)
        {
          if (!iterator.MoveNext())
            throw new IndexOutOfRangeException("index not found in source container");
          there += 1;
        }
        there = 0;
        while (there != distance)
        {
          if (!iterator.MoveNext())
            yield break;
          yield return iterator.Current;
          there += 1;
        }
      }
    }

    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int size)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (size < 0) throw new ArgumentOutOfRangeException("size",
          @"'size' must be positive");
      int there = 0;
      foreach (var item in source)
      {
        if (there % size == 0)
          yield return item;
        there += 1;
      }
    }
  }
}
