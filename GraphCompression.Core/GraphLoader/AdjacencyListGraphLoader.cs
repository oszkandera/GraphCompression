using GraphProcessor.Core.Interfaces.GraphLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GraphProcessor.Core.GraphLoader
{
    public class AdjacencyListGraphLoader : IGraphLoader
    {
        public Dictionary<int, HashSet<int>> Load(string filePath)
        {
            var graph = new Dictionary<int, HashSet<int>>();
            
            using (var reader = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("%")) continue;
                    
                    var nodes = line.Split(" ");
                    var nodeX = Convert.ToInt32(nodes[0]);
                    var nodeY = Convert.ToInt32(nodes[1]);

                    if (!graph.ContainsKey(nodeX)) graph.Add(nodeX, new HashSet<int>());
                    
                    graph[nodeX].Add(nodeY);
                }
            }

            return graph;
        }
    }
}
