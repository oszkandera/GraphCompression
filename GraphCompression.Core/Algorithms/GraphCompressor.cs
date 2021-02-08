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
        //private readonly object _syncObject = new object();

        public CompressedGraph Compress(IGraph originalGraph)
        {
            var sortedGraphStructure = GetSortedGrahStructure(originalGraph.RawGraphStructure);

            //Node 1 referencuje Node2
            var mostSimilarNodes = GetMostSimilarNodes(sortedGraphStructure);


            var compressedGraph = CreateCompressedGraph(mostSimilarNodes);


            var originalCount = Enumerable.Sum(originalGraph.Select(x => x.Neighbors.Count));
            var cCount = Enumerable.Sum(compressedGraph.GraphStructure.Select(x => x.ExtraNodes.Count));

            return compressedGraph;
        }

        public CompressedGraph CreateCompressedGraph(List<SimilarNode> mostSimilarNodes)
        {
            var compressedGraph = new CompressedGraph();
            
            foreach (var similarNode in mostSimilarNodes)
            {
                var nodeId = similarNode.Node1;
                var referenceTo = similarNode.Node2;


                var referenceList = new bool[similarNode.Neighbors2.Count];
                int tempIndex = 0;
                foreach(var neighbor in similarNode.Neighbors2)
                {
                    if (similarNode.Neighbors1.Contains(neighbor))
                    {
                        referenceList[tempIndex] = true;
                    }
                    else
                    {
                        referenceList[tempIndex] = false;
                    }

                    tempIndex++;
                }

                var extraNodes = new SortedSet<int>();
                foreach(var neighbor in similarNode.Neighbors1)
                {
                    if (!similarNode.Neighbors2.Contains(neighbor))
                    {
                        extraNodes.Add(neighbor.Value);
                    }
                }

                compressedGraph.AddNode(new CompressedNode
                {
                    Id = nodeId,
                    ReferenceId = referenceTo,
                    ReferenceList = referenceList,
                    ExtraNodes = extraNodes
                });
            }

            return compressedGraph;
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

            //Uzel na který je referencovano nemuze referencovat jiné uzly
            var usedReferences = new HashSet<int>();
            //Uzly, které referencuji jiné uzly nemohou byt referencovany jinymi uzly
            var alreadyReferencingNodes = new HashSet<int>();

            for(int i = 0; i < originalGraph.Count; i++)
            {
                int? theMostSimilarNode = null;
                var maxNumberOfSimilarNodes = 0;

                if (usedReferences.Contains(i))
                {
                    mostSimilarNodes.Add(new SimilarNode()
                    {
                        Node1 = i,
                        Neighbors1 = originalGraph[i],
                        Neighbors2 = new SortedList<int, int>()
                    });
                    continue;
                }

                for (int y = 0; y < originalGraph.Count; y++)
                {
                    if (AreSameNodes(i, y) || alreadyReferencingNodes.Contains(y)) continue;

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

                if (theMostSimilarNode.HasValue)
                {
                    usedReferences.Add(theMostSimilarNode.Value);
                    alreadyReferencingNodes.Add(i);
                }

                mostSimilarNodes.Add(new SimilarNode()
                {
                    Node1 = i,
                    Node2 = theMostSimilarNode,
                    Neighbors1 = originalGraph[i],
                    Neighbors2 = theMostSimilarNode.HasValue ? originalGraph[theMostSimilarNode.Value] : new SortedList<int, int>()
                });
            }

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
