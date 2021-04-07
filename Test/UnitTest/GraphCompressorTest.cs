using GraphCompression.Core.Algorithms;
using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Code;
using GraphCompression.Core.Models;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Test.UnitTest
{
    public class GraphCompressorTest
    {
        [Fact]
        public void ShouldHandleFullyOverlapedNodes()
        {
            var compressParameters = new CompressParameters { MaxReferenceListSize = 10 };
            var similarNodeList = new List<SimilarNode>()
            {
                new SimilarNode
                {
                    Node1 = 1,
                    Neighbors1 = new SortedList<int, int> { { 1, 3 }, { 2, 4 }, { 3, 5 } }
                },
                new SimilarNode
                {
                    Node1 = 2,
                    Node2 = 1,
                    Neighbors1 = new SortedList<int, int> { { 1, 3 }, { 2, 4 }, { 3, 5 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 3 }, { 2, 4 }, { 3, 5 } }
                }
            };

            var expectedResult = new CompressedGraph();
            expectedResult.AddNode(new CompressedNode 
            { 
                Id = 1, 
                ExtraNodes = new SortedSet<int> { 3, 4, 5 } 
            });
            expectedResult.AddNode(new CompressedNode 
            { 
                Id = 2, 
                ExtraNodes = new SortedSet<int>(), 
                ReferenceId = 1, 
                ReferenceList = new BitArray(new bool[] { true, true, true })
            });


            var result = ExecuteTest(similarNodeList, compressParameters);
            CheckExpectedAgainstResult(expectedResult, result);
        }
        
        [Fact]
        public void ShouldHandleNodeWithoutReference()
        {
            var compressParameters = new CompressParameters { MaxReferenceListSize = 10 };
            var similarNodeList = new List<SimilarNode>()
            {
                new SimilarNode
                {
                    Node1 = 1,
                    Neighbors1 = new SortedList<int, int> { { 1, 3 }, { 2, 4 }, { 3, 5 } }
                }
            };

            var expectedResult = new CompressedGraph();
            expectedResult.AddNode(new CompressedNode
            {
                Id = 1,
                ExtraNodes = new SortedSet<int> { 3, 4, 5 }
            });


            var result = ExecuteTest(similarNodeList, compressParameters);
            CheckExpectedAgainstResult(expectedResult, result);
        }

        [Fact]
        public void ShouldHandleMaxReferenceListOverstepWithBoundaries()
        {
            var compressParameters = new CompressParameters { MaxReferenceListSize = 2 };
            var similarNodeList = new List<SimilarNode>()
            {
                new SimilarNode
                {
                    Node1 = 1,
                    Node2 = 2,
                    Neighbors1 = new SortedList<int, int> { { 1, 30 }, { 2, 40 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 30 }, { 2, 40 }, { 3, 50 } }
                },
                new SimilarNode
                {
                    Node1 = 3,
                    Node2 = 4,
                    Neighbors1 = new SortedList<int, int> { { 1, 70 }, { 2, 80 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 70 }, { 2, 80 } }
                },
                new SimilarNode
                {
                    Node1 = 5,
                    Node2 = 6,
                    Neighbors1 = new SortedList<int, int> { { 1, 75 }, { 2, 85 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 75 } }
                },
            };

            var expectedResult = new CompressedGraph();
            expectedResult.AddNode(new CompressedNode
            {
                Id = 1,
                ExtraNodes = new SortedSet<int>() { 30, 40 },
            });
            expectedResult.AddNode(new CompressedNode
            {
                Id = 3,
                ReferenceId = 4,
                ReferenceList = new BitArray(new bool[] { true, true }),
                ExtraNodes = new SortedSet<int>(),
            });

            expectedResult.AddNode(new CompressedNode
            {
                Id = 5,
                ReferenceId = 6,
                ReferenceList = new BitArray(new bool[] { true }),
                ExtraNodes = new SortedSet<int> { 85 },
            });

            var result = ExecuteTest(similarNodeList, compressParameters);
            CheckExpectedAgainstResult(expectedResult, result);
        }

        [Fact]
        public void ShouldHandleNotMappedReferences()
        {
            var compressParameters = new CompressParameters { MaxReferenceListSize = 5 };
            var similarNodeList = new List<SimilarNode>()
            {
                new SimilarNode
                {
                    Node1 = 1,
                    Node2 = 2,
                    Neighbors1 = new SortedList<int, int> { { 1, 30 }, { 2, 40 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 30 } }
                },
                new SimilarNode
                {
                    Node1 = 3,
                    Node2 = 4,
                    Neighbors1 = new SortedList<int, int> { { 1, 70 }, { 2, 80 } },
                    Neighbors2 = new SortedList<int, int> { }
                },
                new SimilarNode
                {
                    Node1 = 5,
                    Node2 = 6,
                    Neighbors1 = new SortedList<int, int> { { 1, 75 }, { 2, 85 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 85 }, { 2, 90 } }
                },
                new SimilarNode
                {
                    Node1 = 7,
                    Node2 = 8,
                    Neighbors1 = new SortedList<int, int> { { 1, 90 }, { 2, 96 } },
                    Neighbors2 = new SortedList<int, int> { { 1, 85 }, { 2, 88 } }
                },
            };

            var expectedResult = new CompressedGraph();
            expectedResult.AddNode(new CompressedNode
            {
                Id = 1,
                ReferenceId = 2,
                ReferenceList = new BitArray(new bool[] { true }),
                ExtraNodes = new SortedSet<int>() { 40 },
            });
            expectedResult.AddNode(new CompressedNode
            {
                Id = 3,
                ReferenceId = 4,
                ReferenceList = new BitArray(0),
                ExtraNodes = new SortedSet<int>() { 70, 80 },
            });

            expectedResult.AddNode(new CompressedNode
            {
                Id = 5,
                ReferenceId = 6,
                ReferenceList = new BitArray(new bool[] { true, false }),
                ExtraNodes = new SortedSet<int> { 75 },
            });

            expectedResult.AddNode(new CompressedNode
            {
                Id = 7,
                ReferenceId = 8,
                ReferenceList = new BitArray(new bool[] { false, false }),
                ExtraNodes = new SortedSet<int> { 90, 96 },
            });

            var result = ExecuteTest(similarNodeList, compressParameters);
            CheckExpectedAgainstResult(expectedResult, result);
        }

        private CompressedGraph ExecuteTest(List<SimilarNode> similarNodeList, CompressParameters compressParameters)
        {
            var sortingAlgorithm = GetMockedSortingAlgorithm().Object;
            var similarNodeProcessor = GetMockedSimilarNodeProcessor(similarNodeList).Object;

            var graphCompressor = new GraphCompressor(sortingAlgorithm, similarNodeProcessor);

            var result = graphCompressor.Compress(new Graph(), compressParameters);

            return result;
        }

        private void CheckExpectedAgainstResult(CompressedGraph expected, CompressedGraph result)
        {
            Assert.NotNull(result);
            Assert.NotNull(result.GraphStructure);
            Assert.Equal(expected.GraphStructure.Count, result.GraphStructure.Count);

            foreach(var resultNode in result.GraphStructure)
            {
                var expectedNode = expected.GraphStructure.Where(x => x.Id == resultNode.Id).FirstOrDefault();
                
                Assert.NotNull(expectedNode);
                Assert.Equal(expectedNode.ReferenceId, resultNode.ReferenceId);
                Assert.Equal(expectedNode.ReferenceList, resultNode.ReferenceList);
                Assert.Equal(expectedNode.ExtraNodes, resultNode.ExtraNodes);
            }
        }

        private Mock<IGraphSortingAlgorithm> GetMockedSortingAlgorithm()
        {
            var mockedGraphSortingAlgortihm = new Mock<IGraphSortingAlgorithm>();
            return mockedGraphSortingAlgortihm;
        }

        private Mock<ISimilarNodeProcessor> GetMockedSimilarNodeProcessor(List<SimilarNode> similarNodeList)
        {
            var mockedSimilarNodeProcessor = new Mock<ISimilarNodeProcessor>();
            mockedSimilarNodeProcessor
                .Setup(x => x.CreateListOfSimilarNodes(It.IsAny<List<KeyValuePair<int, List<int>>>>(), 
                                                       It.IsAny<CompressParameters>()))
                .Returns(similarNodeList);

            return mockedSimilarNodeProcessor;
        }
    }
}
