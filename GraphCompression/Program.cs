﻿using GraphCompression.Core.Algorithms;
using GraphProcessor.Core.GraphLoader;
using System.Linq;

namespace GraphCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"..\..\..\..\GraphCompression.Core\Data\web-NotreDame-light2.edges";
            //var path = @"..\..\..\..\GraphCompression.Core\Data\web-NotreDame.edges";

            var graphLoader = new AdjacencyListGraphLoader();

            var (graph, nodeMap) = graphLoader.Load(path);

            //Test - GetNode
            var node = graph.GetNode(254);


            //var compressor = new GraphCompressor();
            var compressor = new MultidimensionalGraphCompressor(64);
            var compressedGraph = compressor.Compress(graph);

            var x = compressedGraph.GraphStructure.Where(x => x.ReferenceId.HasValue).Count();

        }
    }
}
