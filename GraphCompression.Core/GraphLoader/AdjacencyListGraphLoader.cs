using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using GraphCompression.Core.Utils;
using GraphProcessor.Core.Interfaces.GraphLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphProcessor.Core.GraphLoader
{
    public class AdjacencyListGraphLoader : IGraphLoader
    {
        public (IGraph, Dictionary<string, int>) Load(string filePath)
        {
            var graph = new Graph();

            var adjacencyList = LoadAdjacencyList(filePath);

            var anonymizedNodePairMap = GetMapOfAnonymizedNodes(adjacencyList);

            foreach(var nodePair in adjacencyList)
            {
                var nodeX = anonymizedNodePairMap[nodePair.Item1];
                var nodeY = anonymizedNodePairMap[nodePair.Item2];

                graph.AddBidirectionEdge(nodeX, nodeY);
            }


            return (graph, anonymizedNodePairMap);
        }

        private List<Tuple<string, string>> LoadAdjacencyList(string filePath)
        {
            var adjacencyList = new List<Tuple<string, string>>();

            using (var reader = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("%")) continue;

                    var nodes = line.Split(" ");

                    var nodeX = nodes[0];
                    var nodeY = nodes[1];

                    adjacencyList.Add(new Tuple<string, string>(nodeX, nodeY));
                }
            }

            return adjacencyList;
        }

        private Dictionary<string, int> GetMapOfAnonymizedNodes(List<Tuple<string, string>> adjacencyList)
        {
            var distinctNodes = GetDistinctNodes(adjacencyList);
            var countOfDistinctNodes = distinctNodes.Count;

            var shuffledStack = PrepareShuffledStack(countOfDistinctNodes);

            var anonymizedNodePair = new Dictionary<string, int>();

            foreach(var node in distinctNodes)
            {
                var nodeId = shuffledStack.Pop();
                anonymizedNodePair.Add(node, nodeId);
            }

            return anonymizedNodePair;
        }

        private List<string> GetDistinctNodes(List<Tuple<string, string>> adjacencyList)
        {
            var allXNodes = adjacencyList.Select(x => x.Item1);
            var allYNodes = adjacencyList.Select(x => x.Item2);

            var allNodes = allXNodes.Union(allYNodes);

            var distinctNodes = allNodes.Distinct().ToList();

            return distinctNodes;
        }

        private Stack<int> PrepareShuffledStack(int countOfDistinctNumbers)
        {
            var stack = new Stack<int>();

            for (int i = 0; i < countOfDistinctNumbers; i++)
            {
                stack.Push(i);
            }

            stack.Shuffle();

            return stack;
        }
    }
}
