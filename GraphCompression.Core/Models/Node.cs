using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class Node
    {
        public Node()
        {
            Neighbors = new SortedList<int, int>();
        }

        public int NodeIdentifier { get; set; }
        public SortedList<int, int> Neighbors { get; set; }
    }
}
