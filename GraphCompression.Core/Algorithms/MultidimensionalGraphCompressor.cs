﻿using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Algorithms
{
    public class MultidimensionalGraphCompressor : IGraphCompressor
    {
        //private readonly int MaxRerefenceListSize = 64; NWM
        //private readonly int MaxReferenceChainSize = 5;

        public CompressedGraph Compress(IGraph originalGraph)
        {
            var similarNodes = CreateListOfSimilarNodes(originalGraph);

            var compressedGraph = CreateCompressedGraph(similarNodes);

            return compressedGraph;
        }

        private CompressedGraph CreateCompressedGraph(List<SimilarNode> similarNodes)
        {

            var compressedGraph = new CompressedGraph();

            foreach (var similarNode in similarNodes)
            {
                CompressedNode compressedNode = null;

                if (similarNode.Node2.HasValue) // Uzel obsahuje nějaký odkaz na jiný uzel
                {
                    var referenceList = new BitArray(similarNode.Neighbors2.Count);

                    var pairedReferencingNodeNeighbors = new HashSet<int>();

                    for (int referencedNodeNeighborIndex = 0; referencedNodeNeighborIndex < similarNode.Neighbors2.Count; referencedNodeNeighborIndex++)
                    {
                        var referencesNodeNeighborKey = similarNode.Neighbors2.Keys[referencedNodeNeighborIndex];
                        var referencedNodeNeighbor = similarNode.Neighbors2[referencesNodeNeighborKey];

                        var isNodeInReferencingNode = false;

                        for (int referencingNodeNeighorIndex = 0; referencingNodeNeighorIndex < similarNode.Neighbors1.Count; referencingNodeNeighorIndex++)
                        {
                            var referencingNodeNeighborKey = similarNode.Neighbors1.Keys[referencingNodeNeighorIndex];
                            var referencingNodeNeighbor = similarNode.Neighbors1[referencingNodeNeighborKey];

                            if (referencingNodeNeighbor == referencedNodeNeighbor)
                            {
                                isNodeInReferencingNode = true;
                                pairedReferencingNodeNeighbors.Add(referencingNodeNeighbor);
                                break;
                            }

                        }

                        referenceList.Set(referencedNodeNeighborIndex, isNodeInReferencingNode);
                    }

                    var extraNodeList = similarNode.Neighbors1.Where(x => !pairedReferencingNodeNeighbors.Contains(x.Value)).Select(x => x.Value);
                    var extraNodes = new SortedSet<int>(extraNodeList);

                    compressedNode = new CompressedNode
                    {
                        Id = similarNode.Node1,
                        ReferenceId = similarNode.Node2,
                        ReferenceList = referenceList,
                        ExtraNodes = extraNodes,
                    };
                }
                else
                {
                    var extraNodes = similarNode.Neighbors1.Select(x => x.Value);

                    compressedNode = new CompressedNode
                    {
                        Id = similarNode.Node1,
                        ExtraNodes = new SortedSet<int>(extraNodes)
                    };
                }


                compressedGraph.AddNode(compressedNode);
            }

            return compressedGraph;
        }

        private List<SimilarNode> CreateListOfSimilarNodes(IGraph originalGraph)
        {
            var sortedGraphStructure = GetSortedGraphStructure(originalGraph.RawGraphStructure);

            //Omezeni - uzel referencuje pouze uzel se stejnym, nebo vyssim stupnem, serazenou strukturu prochazime jednosmerne abychom
            //zamezili cyklickym referencim = proto si muzeme dovolit stanovit podminku v druhem cykly y = i + 1
            //Pocet pruchodu pres prvni cyklus je snizen o 1 protoze predpokladame, ze alespon 1 uzel nebude nic referencovat (v tomto pripade to 
            //bude uzel posledni)

            var similarNodes = new List<SimilarNode>();

            for (int findingNodeIndex = 0; findingNodeIndex < sortedGraphStructure.Count - 1; findingNodeIndex++)
            {
                var findingNode = sortedGraphStructure[findingNodeIndex];
                
                int maxSameNodeCount = 0;
                KeyValuePair<int, List<int>>? mostSimilarNode = null;

                for (int wantedNodeIndex = findingNodeIndex + 1; wantedNodeIndex < sortedGraphStructure.Count; wantedNodeIndex++)
                {
                    var wantedNode = sortedGraphStructure[wantedNodeIndex];

                    var findingNodesCount = findingNode.Value.Count;
                    var wantedNodesCount = wantedNode.Value.Count;

                    var sameNodesCount = 0;
                    var mostSimilarNodeFound = false;

                    for(int findingNodeNeighborIndex = 0; findingNodeNeighborIndex < findingNodesCount; findingNodeNeighborIndex++)
                    {
                        var findingNeighbor = findingNode.Value[findingNodeNeighborIndex];

                        for(int wantedNodeNeighborIndex = 0; wantedNodeNeighborIndex < wantedNodesCount; wantedNodeNeighborIndex++)
                        {
                            var wantedNeighbor = wantedNode.Value[wantedNodeNeighborIndex];

                            if (findingNeighbor > wantedNeighbor) break; // Uzly jsou seřazeny - pokud najdeme vetší uzel, je jisté, že menší nenajdeme

                            if (findingNeighbor == wantedNeighbor) sameNodesCount++;
                        }

                        if (sameNodesCount == findingNodesCount) // Našli jsme uzel, ktery pojme všechny sousedy hledaneho uzlu
                        {
                            mostSimilarNode = wantedNode;
                            mostSimilarNodeFound = true;
                            break; 
                        }
                    }

                    if (mostSimilarNodeFound)
                    {
                        break;
                    }

                    if(sameNodesCount > maxSameNodeCount)
                    {
                        maxSameNodeCount = sameNodesCount;
                        mostSimilarNode = wantedNode;
                    }
                }

                if(mostSimilarNode.HasValue)
                {
                    var similarNode = new SimilarNode
                    {
                        Node1 = findingNode.Key,
                        Neighbors1 = new SortedList<int, int>(findingNode.Value.ToDictionary(x => x)),
                        Node2 = mostSimilarNode.Value.Key,
                        Neighbors2 = new SortedList<int, int>(mostSimilarNode.Value.Value.ToDictionary(x => x))
                    };
                    similarNodes.Add(similarNode);
                }
                else
                {
                    var similarNode = new SimilarNode
                    {
                        Node1 = findingNode.Key,
                        Neighbors1 = new SortedList<int, int>(findingNode.Value.ToDictionary(x => x)),
                        Node2 = null,
                        Neighbors2 = new SortedList<int, int>()
                    };

                    similarNodes.Add(similarNode);
                }
            }

            //Pridani posledniho uzlu, ktery byl vynechan v cyklu protoze se nikdy nebude referencovat
            similarNodes.Add(new SimilarNode
            {
                Node1 = sortedGraphStructure[sortedGraphStructure.Count - 1].Key,
                Neighbors1 = new SortedList<int, int>(sortedGraphStructure[sortedGraphStructure.Count - 1].Value.ToDictionary(x => x))
            });

            return similarNodes;
        }

        private List<KeyValuePair<int, List<int>>> GetSortedGraphStructure(Dictionary<int, List<int>> originalGraphStructure)
        {
            var sortedGraphStructure = new List<KeyValuePair<int, List<int>>>();

            var sortedByDegree = originalGraphStructure.OrderBy(x => x.Value.Count);

            foreach(var node in sortedByDegree)
            {
                var sortedListOfNeighbors = node.Value.OrderBy(x => x).ToList();

                var nodeNeighborPair = KeyValuePair.Create(node.Key, sortedListOfNeighbors);

                sortedGraphStructure.Add(nodeNeighborPair);
            }

            return sortedGraphStructure;
        }
    }
}
