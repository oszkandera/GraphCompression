using GraphCompression.Core.Factory;
using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Algorithms
{
    public class MultidimensionalGraphCompressor : IGraphCompressor
    {
        private readonly GraphCompresorParameters _parameters;

        public MultidimensionalGraphCompressor() { }
        
        public MultidimensionalGraphCompressor(GraphCompresorParameters graphCompresorParameters)
        {
            _parameters = graphCompresorParameters;
        }

        public CompressedGraph Compress(IGraph originalGraph)
        {
            var sortedGraphStructure = GetSortedGraphStructure(originalGraph.RawGraphStructure);

            var similarNodes = CreateListOfSimilarNodes(sortedGraphStructure);

            var compressedGraph = CreateCompressedGraph(similarNodes);

            return compressedGraph;
        }

        private CompressedGraph CreateCompressedGraph(IEnumerable<SimilarNode> similarNodes)
        {
            var compressedGraph = new CompressedGraph();

            foreach (var similarNode in similarNodes)
            {
                //Pokud uzel neobsahuje referenci a nebo ma reference vetsi pocet sousedu nez maximalni 
                if (!SimilarNodeContainsReference(similarNode) || IsReferenceNeighborCountHigherThanMaxReferenceListSize(similarNode))
                {
                    var compressedNodeWithoutReference = CompressedNodeFactroy.CreateCompressedNodeFromSimilarNodeWithoutReference(similarNode);
                    compressedGraph.AddNode(compressedNodeWithoutReference);
                }
                else 
                {
                    var (referenceList, extraNodes) = GetReferenceListAndExtraNodes(similarNode);

                    var compressedNode = new CompressedNode
                    {
                        Id = similarNode.Node1,
                        ReferenceId = similarNode.Node2,
                        ReferenceList = referenceList,
                        ExtraNodes = extraNodes,
                    };

                    compressedGraph.AddNode(compressedNode);
                }

            }

            return compressedGraph;
        }

        private (BitArray, SortedSet<int>) GetReferenceListAndExtraNodes(SimilarNode similarNode)
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

            return (referenceList, extraNodes);
        }

        private bool IsReferenceNeighborCountHigherThanMaxReferenceListSize(SimilarNode similarNode)
        {
            var numberOfNodesInNeighbor = similarNode.Neighbors2.Count;
            return _parameters.MaxReferenceListSize.HasValue && numberOfNodesInNeighbor > _parameters.MaxReferenceListSize.Value;
        }

        private bool SimilarNodeContainsReference(SimilarNode similarNode)
        {
            return similarNode.Node2.HasValue;
        }

        private IEnumerable<SimilarNode> CreateListOfSimilarNodes(List<KeyValuePair<int, List<int>>> sortedGraphStructure)
        {
            //Omezeni - uzel referencuje pouze uzel se stejnym, nebo vyssim stupnem, serazenou strukturu prochazime jednosmerne abychom
            //zamezili cyklickym referencim = proto si muzeme dovolit stanovit podminku v druhem cykly y = i + 1
            //Pocet pruchodu pres prvni cyklus je snizen o 1 protoze predpokladame, ze alespon 1 uzel nebude nic referencovat (v tomto pripade to 
            //bude uzel posledni)



            //TODO: Dodelat omezeni referencniho retezeni - novy pomocny dictionary, ktery bude obsahuvat jako klic id uzlu a jako hodnotu list uzlu
            //ktere na tento uzel odkazuji. Algoritmus pak bude fungovat tak, ze pri kazdem pokusu o vytvoreni reference na jiny uzel y tohto uzlu bude 
            //nutne ykontrolovat hloubku zanoreni. Pokud bude hloubka zanoreni < !! (je nutne ostre rovnitko), pak referenci nepridavat

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

                    for (int findingNodeNeighborIndex = 0; findingNodeNeighborIndex < findingNodesCount; findingNodeNeighborIndex++)
                    {
                        var findingNeighbor = findingNode.Value[findingNodeNeighborIndex];

                        for (int wantedNodeNeighborIndex = 0; wantedNodeNeighborIndex < wantedNodesCount; wantedNodeNeighborIndex++)
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

                    if (sameNodesCount > maxSameNodeCount)
                    {
                        maxSameNodeCount = sameNodesCount;
                        mostSimilarNode = wantedNode;
                    }
                }

                var similarNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(findingNode, mostSimilarNode);

                similarNodes.Add(similarNode);
            }

            //Pridani posledniho uzlu, ktery byl vynechan v cyklu protoze se nikdy nebude referencovat
            var lastNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(sortedGraphStructure[sortedGraphStructure.Count - 1]);
            similarNodes.Add(lastNode);

            return similarNodes;
        }

        private List<KeyValuePair<int, List<int>>> GetSortedGraphStructure(Dictionary<int, List<int>> originalGraphStructure)
        {
            var sortedGraphStructure = new List<KeyValuePair<int, List<int>>>();

            var sortedByDegree = originalGraphStructure.OrderBy(x => x.Value.Count);

            foreach (var node in sortedByDegree)
            {
                var sortedListOfNeighbors = node.Value.OrderBy(x => x).ToList();

                var nodeNeighborPair = KeyValuePair.Create(node.Key, sortedListOfNeighbors);

                sortedGraphStructure.Add(nodeNeighborPair);
            }

            return sortedGraphStructure;
        }
    }
}
