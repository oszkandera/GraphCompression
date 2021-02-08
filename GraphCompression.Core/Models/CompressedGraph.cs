using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class CompressedGraph
    {
        private List<CompressedNode> _graphStructure;
        public List<CompressedNode> GraphStructure 
        {
            get
            {
                return _graphStructure != null ? _graphStructure : _graphStructure = new List<CompressedNode>(); 
            } 
        }

        public void AddNode(CompressedNode node)
        {
            GraphStructure.Add(node);
        }
    }
}
