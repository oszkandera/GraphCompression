using GraphCompression.Core.Algorithms;
using GraphProcessor.Core.GraphLoader;
using System;

namespace GraphCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"..\..\..\..\GraphCompression.Core\Data\web-NotreDame-light.edges";
            //var path = @"..\..\..\..\GraphCompression.Core\Data\web-NotreDame.edges";

            var graphLoader = new AdjacencyListGraphLoader();

            var (graph, nodeMap) = graphLoader.Load(path);

            //Test - GetNode
            var node = graph.GetNode(254);


            var compressor = new GraphCompressor();
            var compressedGraph = compressor.Compress(graph);
        }
    }
}
