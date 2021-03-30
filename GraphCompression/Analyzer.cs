using GraphCompression.Core.Algorithms;
using GraphCompression.Core.Code;
using GraphCompression.Core.GraphLoader;
using GraphCompression.Core.Interfaces.GraphLoader;
using GraphCompression.Core.Models;
using GraphCompression.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GraphCompression
{
    public class Analyzer
    {
        private List<AnalyzeData> _data;
        private List<CompressParameters> _testCompressParameters;

        private IAnonymizableGraphLoader _mtxGrahLoader;
        private IAnonymizableGraphLoader _adjacencyListGraphLoader;
        private SortingAlgorithm _sortingAlgorithm;
        private SimilarNodeProcessor _similarNodeProcessor;
        
        private Analyzer()
        {
            _mtxGrahLoader = new MTXGraphLoader();
            _adjacencyListGraphLoader = new AdjacencyListGraphLoader();
            _sortingAlgorithm = new SortingAlgorithm();
            _similarNodeProcessor = new SimilarNodeProcessor();


            _testCompressParameters = new List<CompressParameters>
            {
                //new CompressParameters(){ MaxReferenceListSize = 64, MaxReferenceChainSize = null, WithTimeOptimalization = true},
                //new CompressParameters(){ MaxReferenceListSize = 64, MaxReferenceChainSize = 5 },
                //new CompressParameters(){ MaxReferenceListSize = 64, MaxReferenceChainSize = 10 },
                //new CompressParameters(){ MaxReferenceListSize = 64, MaxReferenceChainSize = 20 },
                //new CompressParameters(){ MaxReferenceListSize = 64, MaxReferenceChainSize = 50 },

                //new CompressParameters(){ MaxReferenceListSize = 256, MaxReferenceChainSize = null},
                //new CompressParameters(){ MaxReferenceListSize = 256, MaxReferenceChainSize = 5 },
                //new CompressParameters(){ MaxReferenceListSize = 256, MaxReferenceChainSize = 10 },
                //new CompressParameters(){ MaxReferenceListSize = 256, MaxReferenceChainSize = 20 },
                //new CompressParameters(){ MaxReferenceListSize = 256, MaxReferenceChainSize = 50 },

                //new CompressParameters(){ MaxReferenceListSize = 512, MaxReferenceChainSize = null},
                //new CompressParameters(){ MaxReferenceListSize = 512, MaxReferenceChainSize = 5 },
                //new CompressParameters(){ MaxReferenceListSize = 512, MaxReferenceChainSize = 10 },
                //new CompressParameters(){ MaxReferenceListSize = 512, MaxReferenceChainSize = 20 },
                //new CompressParameters(){ MaxReferenceListSize = 512, MaxReferenceChainSize = 50 },

                new CompressParameters(){ MaxReferenceListSize = 1024, MaxReferenceChainSize = null},
                new CompressParameters(){ MaxReferenceListSize = 1024, MaxReferenceChainSize = 5 },
                //new CompressParameters(){ MaxReferenceListSize = 1024, MaxReferenceChainSize = 10 },
                //new CompressParameters(){ MaxReferenceListSize = 1024, MaxReferenceChainSize = 20 },
                new CompressParameters(){ MaxReferenceListSize = 1024, MaxReferenceChainSize = 50 },
            };
        }

        public Analyzer(List<AnalyzeData> data) : this()
        {
            _data = data;
        }

        public Analyzer(string pathToCsvFileWithAnalyzeData) : this()
        {
            _data = LoadDataForAnalysis(pathToCsvFileWithAnalyzeData);
        }

        public List<AnalyzeResult> Analyze()
        {
            var analyzeOutput = new List<AnalyzeResult>();

            var stopwatch = new Stopwatch();

            foreach(var testInstance in _data)
            {
                var sourceFileType = Path.GetExtension(testInstance.FilePath);
                var graphLoader = sourceFileType == "edge" ? _adjacencyListGraphLoader : _mtxGrahLoader;
                
                var (graph, map) = graphLoader.Load(testInstance.FilePath);
                var originalSize = Enumerable.Sum(graph.RawGraphStructure.Select(x => x.Value.Count));
                
                foreach(var compressParameters in _testCompressParameters)
                {
                    var compressor = new GraphCompressor(compressParameters);
                    
                    stopwatch.Start();
                    var compressedGraph = compressor.Compress(graph, _sortingAlgorithm, _similarNodeProcessor);
                    stopwatch.Stop();

                    var compressedSize = Enumerable.Sum(compressedGraph.GraphStructure.Select(x => x.ExtraNodes.Count));


                    analyzeOutput.Add(new AnalyzeResult
                    {
                        AnalyzeData = testInstance,
                        CompressedSize = compressedSize,
                        CompressTime = stopwatch.Elapsed,
                        MaxReferenceChainSize = compressParameters.MaxReferenceChainSize,
                        MaxReferenceListSize = compressParameters.MaxReferenceListSize,
                        OriginalSize = originalSize,
                        TimeOptimalizationUsed = compressParameters.WithTimeOptimalization
                    });

                    stopwatch.Restart();
                }
            }

            return analyzeOutput;
        }

        private List<AnalyzeData> LoadDataForAnalysis(string pathToCsvFileWithAnalyzeData)
        {
            var analyzeDataList = new List<AnalyzeData>();

            using(var stream = new FileStream(pathToCsvFileWithAnalyzeData, FileMode.Open))
            {
                using(var reader = new StreamReader(stream))
                {
                    var lineNumber = 0;
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        lineNumber++;
                        if(lineNumber == 1) continue;
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var data = line.Split(";");
                        analyzeDataList.Add(new AnalyzeData
                        {
                            Name = data[0],
                            NodeCount = int.Parse(data[1]),
                            EdgeCount = int.Parse(data[2]),
                            MaxDegree = int.Parse(data[3]),
                            AverageDegree = int.Parse(data[4]),
                            FilePath = data[5]
                        });
                    }
                }
            }

            return analyzeDataList;
        }
    }
}
