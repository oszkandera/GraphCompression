using System.Collections.Generic;
using System.Linq;

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

        public Node GetNodeById(int id)
        {
            var nodeBase = GraphStructure.FirstOrDefault(x => x.Id == id);

            if (nodeBase == null)
            {
                return null;
            }

            var nodes = GetNodes(nodeBase);

            var node = new Node()
            {
                NodeIdentifier = nodeBase.Id,
                Neighbors = nodes.ToList()
            };

            return node;
        }

        private SortedSet<int> GetNodes(CompressedNode node)
        {
            if (!node.ReferenceId.HasValue)
            {
                return node.ExtraNodes;
            }

            var referenceNode = GraphStructure.FirstOrDefault(x => x.Id == node.ReferenceId.Value);

            var allExtraNodes = GetNodes(referenceNode);

            var filteredExtraNodes = new SortedSet<int>();

            for(int i = 0; i < node.ReferenceList.Count; i++)
            {
                if (node.ReferenceList[i])
                {
                    filteredExtraNodes.Add(allExtraNodes.ElementAt(i));
                }
            }

            foreach (var extraNode in node.ExtraNodes)
            {
                filteredExtraNodes.Add(extraNode);
            }

            return filteredExtraNodes;
        
        }
    }
}
