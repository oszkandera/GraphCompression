using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class CompressedGraph
    {
        public List<CompressedNode> GraphStructure { get; set; }

        public void AddNode(CompressedNode node)
        {
            GraphStructure.Add(node);
        }
    }
}
