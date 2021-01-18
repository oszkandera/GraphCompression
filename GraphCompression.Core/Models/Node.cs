using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class Node
    {
        public Node()
        {
            Neighbors = new List<int>();
        }

        public int NodeIdentifier { get; set; }
        public List<int> Neighbors { get; set; }
    }
}
