using GraphProcessor.Core.GraphLoader;
using System;

namespace GraphCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"..\..\..\..\GraphCompression.Core\Data\web-NotreDame.edges";

            var graphLoader = new AdjacencyListGraphLoader();

            var graph = graphLoader.Load(path);
        }
    }
}
