using GraphCompression.Core.Factory;
using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Algorithms
{
    /// <summary>
    /// Class contains algorithm for similar node search
    /// </summary>
    public class SimilarNodeProcessor : ISimilarNodeProcessor
    {
        /// <summary>
        /// Method creates list of similar nodes of original graph
        /// </summary>
        /// <param name="sortedGraphStructure">Sorted original graph structure</param>
        /// <param name="compressParameters">Parameters of compression</param>
        /// <returns>List of <c>SimilarNode</c>.</returns>
        public IEnumerable<SimilarNode> CreateListOfSimilarNodes(List<KeyValuePair<int, List<int>>> sortedGraphStructure, CompressParameters compressParameters)
        {
            //Omezeni - uzel referencuje pouze uzel se stejnym, nebo vyssim stupnem, serazenou strukturu prochazime jednosmerne abychom
            //zamezili cyklickym referencim = proto si muzeme dovolit stanovit podminku v druhem cykly y = i + 1
            //Pocet pruchodu pres prvni cyklus je snizen o 1 protoze predpokladame, ze alespon 1 uzel nebude nic referencovat (v tomto pripade to 
            //bude uzel posledni)

            var similarNodes = new List<SimilarNode>();
            var referenceChain = new Dictionary<int, HashSet<int>>();

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

                    for (int findingNodeNeighborIndex = 0; findingNodeNeighborIndex < findingNodesCount; findingNodeNeighborIndex++)
                    {
                        var findingNeighbor = findingNode.Value[findingNodeNeighborIndex];

                        for (int wantedNodeNeighborIndex = 0; wantedNodeNeighborIndex < wantedNodesCount; wantedNodeNeighborIndex++)
                        {
                            var wantedNeighbor = wantedNode.Value[wantedNodeNeighborIndex];

                            //Možnost časové optimalizace
                            if (compressParameters.WithTimeOptimalization && findingNeighbor > wantedNeighbor) break;

                            if (findingNeighbor == wantedNeighbor) sameNodesCount++;
                        }

                        if (sameNodesCount == findingNodesCount) // Našli jsme uzel, ktery pojme všechny sousedy hledaneho uzlu
                        {
                            var canCreateReference = CheckReferenceImmersion(referenceChain, findingNode.Key, compressParameters.MaxReferenceChainSize);
                            if (canCreateReference)
                            {
                                mostSimilarNode = wantedNode;
                                mostSimilarNodeFound = true;
                                break;
                            }
                        }
                    }

                    if (mostSimilarNodeFound)
                    {
                        break;
                    }

                    if (sameNodesCount > maxSameNodeCount)
                    {
                        maxSameNodeCount = sameNodesCount;
                        mostSimilarNode = wantedNode;
                    }
                }

                //TODO: Refactoring
                //Kontroluje, zda je mozne vytvorit referenci z hlediska zanoreni
                if (mostSimilarNode.HasValue && CheckReferenceImmersion(referenceChain, findingNode.Key, compressParameters.MaxReferenceChainSize))
                {
                    if (referenceChain.ContainsKey(mostSimilarNode.Value.Key))
                    {
                        referenceChain[mostSimilarNode.Value.Key].Add(findingNode.Key);
                    }
                    else
                    {
                        referenceChain.Add(mostSimilarNode.Value.Key, new HashSet<int> { findingNode.Key });
                    }
                }
                else
                {
                    mostSimilarNode = null;
                }

                var similarNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(findingNode, mostSimilarNode);

                similarNodes.Add(similarNode);
            }

            //Pridani posledniho uzlu, ktery byl vynechan v cyklu protoze se nikdy nebude referencovat
            var lastNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(sortedGraphStructure[sortedGraphStructure.Count - 1]);
            similarNodes.Add(lastNode);

            return similarNodes;
        }

        private bool CheckReferenceImmersion(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, int? maxReferenceChainSize)
        {
            if (!maxReferenceChainSize.HasValue) return true;

            var referenceChainDepth = Depth(referenceChain, referencingItem, 0);

            return referenceChainDepth < maxReferenceChainSize.Value;
        }

        private int Depth(Dictionary<int, HashSet<int>> referenceChain, int referencingItem, int depth)
        {
            if (!referenceChain.ContainsKey(referencingItem) || !referenceChain[referencingItem].Any()) return 0;

            int result = depth + 1;

            foreach (var referencingNode in referenceChain[referencingItem])
                result = Math.Max(result, Depth(referenceChain, referencingNode, depth + 1));

            return result;
        }
    }
}
