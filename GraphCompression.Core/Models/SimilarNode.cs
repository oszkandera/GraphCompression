using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class SimilarNode
    {
        public int Node1 { get; set; }
        public SortedList<int, int> Neighbors1 { get; set; }
        public int? Node2 { get; set; }
        public SortedList<int, int> Neighbors2 { get; set; }
    }
}
