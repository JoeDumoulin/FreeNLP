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
  }
}
