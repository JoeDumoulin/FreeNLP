using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
  /// <summary>
  /// Tools for iterating concrete sequential containers.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Iterator<T>
  {
    private Queue<T>  _container { get; set; }
    private IEnumerator<T> _enumerator { get; set; }
    public Iterator(IEnumerable<T> container, int location)
    {
      
    }
  }
}
