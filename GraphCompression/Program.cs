using GraphCompression.Core.Algorithms;
using GraphCompression.Core.Code;
using GraphCompression.Core.GraphLoader;
using GraphCompression.Core.Models;
using System.IO;
using System.Linq;
using System.Text;

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
            //var path = @"..\..\..\Data\web-edu.mtx";
            //var path = @"..\..\..\Data\web-NotreDame-light2.edges";
            //var path = @"..\..\..\Data\web-NotreDame.edges";


            var pathToFileWithAnalyzeData = @"..\..\..\Data\AnalyzeInput.csv";
            var pathToAnalyzeOutput = @"..\..\..\Data\AnalyzeOutput.csv";
            var analyzer = new Analyzer(pathToFileWithAnalyzeData);

            var outputList = analyzer.Analyze();


            using(var stream = new FileStream(pathToAnalyzeOutput, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream)) 
                {
                    var outputBuilder = new StringBuilder();
                    
                    foreach(var output in outputList)
                    {
                        var compressionRation = output.OriginalSize / (double)output.CompressedSize;
                        var numberOfSavedNodes = output.OriginalSize - output.CompressedSize;
                        var savings = 1 - 1 / (double)compressionRation;

                        outputBuilder.Append($"{output.AnalyzeData.Name};{output.OriginalSize};{output.CompressedSize};" +
                            $"{output.CompressTime};{numberOfSavedNodes};" +
                            $"{compressionRation};{savings};{savings * 100};{output.MaxReferenceChainSize};{output.MaxReferenceListSize}");
                        outputBuilder.AppendLine();
                    }

                    writer.Write(outputBuilder.ToString());
                }
            }








            //var graphLoader = new AdjacencyListGraphLoader();
            //var graphLoader = new MTXGraphLoader();

            //var (graph, nodeMap) = graphLoader.Load(path);

            //var d = graph.RawGraphStructure.Where(x => x.Value.Count > 10).FirstOrDefault();

            //var n1 = graph.RawGraphStructure.FirstOrDefault(x => x.Key == d.Key);

            //var p = Enumerable.Sum(graph.RawGraphStructure.Select(x => x.Value.Count));

            //var sortingAlgorithm = new SortingAlgorithm();
            //var similarNodeProcessor = new SimilarNodeProcessor();

            //var compressor = new GraphCompressor(new CompressParameters
            //{
            //    MaxReferenceListSize = 1024,
            //    MaxReferenceChainSize = 10
            //});

            //var compressedGraph = compressor.Compress(graph, sortingAlgorithm, similarNodeProcessor);

            //var n2 = compressedGraph.GetNodeById(d.Key);

            //if(n2.Neighbors.Count != n1.Value.Count)
            //{
            //    System.Console.WriteLine();
            //}

            //foreach(var l in n1.Value)
            //{
            //    bool found = false;
            //    foreach(var l2 in n2.Neighbors)
            //    {
            //        if(l2 == l)
            //        {
            //            found = true;
            //            break;
            //        }
            //    }

            //    if (!found)
            //    {
            //        System.Console.WriteLine();
            //    }
            //}

            //var x = Enumerable.Sum(compressedGraph.GraphStructure.Select(x => x.ExtraNodes.Count));
        }
    }
}
