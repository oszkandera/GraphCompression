using GraphCompression.Core.Model;
using GraphProcessor.Core.Interfaces.GraphLoader;
using System;
using System.IO;

namespace GraphProcessor.Core.GraphLoader
{
    public class AdjacencyListGraphLoader : IGraphLoader
    {
        public Graph<int> Load(string filePath)
        {
            var graph = new Graph<int>();
            
            using (var reader = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("%")) continue;
                    
                    var nodes = line.Split(" ");
                    var nodeX = Convert.ToInt32(nodes[0]);
                    var nodeY = Convert.ToInt32(nodes[1]);

                    graph.AddEdge(nodeX, nodeY);
                }
            }
            return graph;
        }
    }
}
