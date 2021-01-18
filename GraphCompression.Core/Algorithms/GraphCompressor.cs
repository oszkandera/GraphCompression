using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using System.Collections.Generic;

namespace GraphCompression.Core.Algorithms
{
    public class GraphCompressor : IGraphCompressor
    {
        public CompressedGraph Compress(IGraph originalGraph)
        {
            var compressedGraph = new CompressedGraph();

            var mostSimilarNodes = GetMostSimilarNodes(originalGraph);
            
            return compressedGraph;
        }

        private Dictionary<Node, Node> GetMostSimilarNodes(IGraph originalGraph)
        {
            var mostSimilarNodes = new Dictionary<Node, Node>();

            foreach(var node1 in originalGraph)
            {
                foreach(var node2 in originalGraph)
                {
                    var node1Neighbors = node1.Neighbors;
                    var node2Neighbors = node2.Neighbors;
                    var minNumberOfNeigboardOfNodes = node1Neighbors.Count < node2Neighbors.Count ? node1Neighbors.Count : node2Neighbors.Count;

                    var isSimilar = true;

                    for(int i = 0; i < minNumberOfNeigboardOfNodes; i++)
                    {
                        //TODO: Doresit genericitu
                        //if(node1Neighbors[i] != node2Neighbors[i])
                        //{
                        //    isSimilar = false;
                        //    break;
                        //}
                    }
                }
            }

            return mostSimilarNodes;
        }
    }
}
