using GraphCompression.Core.Algorithms;
using GraphCompression.Core.GraphLoader;
using GraphCompression.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            //var path = @"..\..\..\Data\bio-CE-GN.edges";
            //var path = @"..\..\..\Data\web-indochina-2004.mtx";
            //var path = @"..\..\..\Data\web-EPA.edges";
            //var path = @"..\..\..\Data\web-polblogs.mtx";
            var path = @"..\..\..\Data\web-edu.mtx";
            //var path = @"..\..\..\Data\web-NotreDame-light2.edges";
            //var path = @"..\..\..\Data\web-NotreDame.edges";

            //var graphLoader = new AdjacencyListGraphLoader();
            var graphLoader = new MTXGraphLoader();

            var (graph, nodeMap) = graphLoader.Load(path);

            var p = Enumerable.Sum(graph.RawGraphStructure.Select(x => x.Value.Count));

            //var compressor = new GraphCompressor();
            var compressor = new MultidimensionalGraphCompressor(new GraphCompresorParameters
            {
                MaxReferenceListSize = 1024,
                MaxReferenceChainSize = 10
            });


            var compressedGraph = compressor.Compress(graph);

            var x = Enumerable.Sum(compressedGraph.GraphStructure.Select(x => x.ExtraNodes.Count));
        }
    }
}
