using System.Collections.Generic;

namespace GraphCompression.Core.Model
{
    public class Node<T>
    {
        public T NodeIdentifier { get; set; }
        public SortedList<T, T> Neighbors { get; set; }
    }
}
