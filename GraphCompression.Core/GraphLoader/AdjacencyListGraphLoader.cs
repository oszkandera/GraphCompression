using GraphProcessor.Core.Interfaces.GraphLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphProcessor.Core.GraphLoader
{
    public class AdjacencyListGraphLoader : IGraphLoader
    {
        public Dictionary<int, List<int>> Load(string filePath)
        {
            var graph = new Dictionary<int, List<int>>();
            
            using (var reader = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("%")) continue;
                    
                    var nodes = line.Split(" ");
                    var nodeX = Convert.ToInt32(nodes[0]);
                    var nodeY = Convert.ToInt32(nodes[1]);

                    AddBidirectEdge(graph, nodeX, nodeY);
                }
            }

            RemoveDupliciteNeigbors(graph);
            return graph;
        }

        private void RemoveDupliciteNeigbors(Dictionary<int, List<int>> graph)
        {
            foreach(var node in graph.Keys.Reverse())
            {
                graph[node] = graph[node].Distinct().ToList();
            }
        }

        private void AddBidirectEdge(Dictionary<int, List<int>> graph, int nodeX, int nodeY)
        {
            if (!graph.ContainsKey(nodeX)) graph.Add(nodeX, new List<int>());
            if (!graph.ContainsKey(nodeY)) graph.Add(nodeY, new List<int>());

            graph[nodeX].Add(nodeY);
            graph[nodeY].Add(nodeX);
        }
    }
}
