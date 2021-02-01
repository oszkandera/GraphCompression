using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using GraphCompression.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphCompression.Core.Algorithms
{
    public class GraphCompressor : IGraphCompressor
    {
        private readonly object _syncObject = new object();

        public CompressedGraph Compress(IGraph originalGraph)
        {
            var sortedGraphStructure = GetSortedGrahStructure(originalGraph.RawGraphStructure);

            var mostSimilarNodes = GetMostSimilarNodes(sortedGraphStructure);

            //RemoveCycles(mostSimilarNodes);

            var compressedGraph = CreateCompressedGraph(mostSimilarNodes);

            return compressedGraph;
        }

        public CompressedGraph CreateCompressedGraph(List<SimilarNode> mostSimilarNodes)
        {
            var compressedGraph = new CompressedGraph();
            
            foreach (var similarNode in mostSimilarNodes)
            {
                var nodeId = similarNode.Node1;
                var referenceTo = similarNode.Node2;


                compressedGraph.AddNode(new CompressedNode
                {
                    Id = nodeId,
                    ReferenceId = referenceTo,
                });
            }

            return compressedGraph;
        }

        public void RemoveCycles(List<SimilarNode> mostSimilarNodes)
        {
            var usedNodes = new HashSet<int>();

            var nodeToProcess = mostSimilarNodes.FirstOrDefault();
            while(nodeToProcess != null)
            {
                usedNodes.Add(nodeToProcess.Node1);

                if (nodeToProcess.Node2.HasValue && usedNodes.Contains(nodeToProcess.Node2.Value)) //Detect cycle
                {
                    nodeToProcess.Node2 = null;
                    nodeToProcess.Neighbors2 = new SortedList<int, int>();

                    nodeToProcess = mostSimilarNodes.FirstOrDefault(x => !usedNodes.Contains(x.Node1));
                }
                else if (nodeToProcess.Node2.HasValue)
                {
                    nodeToProcess = mostSimilarNodes.FirstOrDefault(x => x.Node1 == nodeToProcess.Node2);
                }
                else
                {
                    nodeToProcess = mostSimilarNodes.FirstOrDefault(x => !usedNodes.Contains(x.Node1));
                }

            }
        }

        private Dictionary<int, SortedList<int, int>> GetSortedGrahStructure(Dictionary<int, List<int>> graphStructure)
        {
            var sortedGraphStructure = new Dictionary<int, SortedList<int, int>>();
            for (int key = 0; key < graphStructure.Count; key++)
            {
                var listOfNeighbors = graphStructure[key];
                var sortedList = new SortedList<int, int>(listOfNeighbors.ToDictionary(s => s));

                sortedGraphStructure.Add(key, sortedList);
            }

            return sortedGraphStructure;
        }

        private List<SimilarNode> GetMostSimilarNodes(Dictionary<int, SortedList<int, int>> originalGraph)
        {
            var mostSimilarNodes = new List<SimilarNode>();

            //var graphCopy = Clone.Create(originalGraph);

            Parallel.For(0, originalGraph.Count, i =>
            {
                int? theMostSimilarNode = null;
                var maxNumberOfSimilarNodes = 0;

                for (int y = 0; y < originalGraph.Count; y++)
                {
                    if (AreSameNodes(i, y)) continue;



                    var primaryNodeNeighbors = originalGraph[i];
                    var secondaryNodeNeighbors = originalGraph[y];

                    var minNeighborCount = GetLowerCountOfNeighors(primaryNodeNeighbors, secondaryNodeNeighbors);

                    var numberOfSimilarNodes = 0;

                    for (int z = 0; z < minNeighborCount; z++)
                    {
                        var neighbor1 = primaryNodeNeighbors.Values[z];
                        var neighbor2 = secondaryNodeNeighbors.Values[z];

                        if (AreSameNodes(neighbor1, neighbor2)) numberOfSimilarNodes++;
                    }

                    if (numberOfSimilarNodes > maxNumberOfSimilarNodes)
                    {
                        maxNumberOfSimilarNodes = numberOfSimilarNodes;
                        theMostSimilarNode = y;
                    }
                }
                lock (_syncObject)
                {
                    mostSimilarNodes.Add(new SimilarNode()
                    {
                        Node1 = i,
                        Node2 = theMostSimilarNode,
                        Neighbors1 = originalGraph[i],
                        Neighbors2 = theMostSimilarNode.HasValue ? originalGraph[theMostSimilarNode.Value] : new SortedList<int, int>()
                    });
                }
            });

            return mostSimilarNodes;
        }

        private int GetLowerCountOfNeighors(SortedList<int, int> neighborsList1, SortedList<int, int> neighborsList2)
        {
            return neighborsList1.Count > neighborsList2.Count ? neighborsList2.Count : neighborsList1.Count;
        }

        private bool AreSameNodes(int node1, int node2)
        {
            return node1 == node2;
        }
    }
}
