using System;
using System.Collections.Generic;
using System.IO;

namespace GraphCompression.Core.GraphLoader
{
    public class AdjacencyListGraphLoader : GraphLoaderBase
    {
        protected override List<Tuple<string, string>> ProcessRawData(string filePath)
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
    }
}
