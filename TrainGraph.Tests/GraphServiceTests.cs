using System;
using TrainGraph.Data.Models;
using Moq;
using TrainGraph.Data;
using NUnit.Framework;

namespace TrainGraph.Tests
{
    [TestFixture]
    public class GraphServiceTests
    {
        protected IGraphService GraphService;
        protected string GraphInfo;

        [SetUp]
        public void Setup()
        {
            //GraphService = Mock.Of<IGraphService>();
            GraphService = new GraphService();
            GraphInfo = "AB6, AE4, BA6, BC2, BD4, CB3, CD1, CE7, DB8, EB5, ED7";
        }

        [Test]
        public void CreateGraphTest()
        {
            GraphModel graph = GraphService.PopulateGraphModel(GraphInfo);
                     
            Assert.IsTrue(graph.Nodes.Count == 5);
            Assert.IsTrue(graph.Nodes.Find(n => n.Label.Equals('A')).Edges.Count == 2);
            Assert.IsTrue(graph.Nodes.Find(n => n.Label.Equals('B')).Edges.Count == 3);            
        }

        [Test]
        public void CreateGraphNodeInfoIncorrectExceptionTest()
        {            
            string graphInfo = "AB6, AB, A4, BA6, B2, BD4, CB3, CD1, CE7, DB8, EB5, ED7";

            bool correctExcetion = false;
            try
            {
                GraphModel graph = GraphService.PopulateGraphModel(graphInfo);
            }
            catch (Exception ex)
            {
                correctExcetion = true;
            }
            
            Assert.IsTrue(correctExcetion);
        }

        [Test]
        public void CreateGraphDuplicatedNodeConnectionExceptionTest()
        {            
            string graphInfo = "AB6, AB6, A4, BA6, B2, BD4, CB3, CD1, CE7, DB8, EB5, ED7";
                        
            bool correctExcetion = false;
            try
            {
                GraphModel graph = GraphService.PopulateGraphModel(graphInfo);
            }
            catch (Exception ex)
            {
                correctExcetion = true;
            }
            
            Assert.IsTrue(correctExcetion);
        }

        [Test]
        public void RouteDistanceTest()
        {            
            string graphInfo = "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7";
            
            GraphModel graph = GraphService.PopulateGraphModel(graphInfo);
            
            Assert.AreEqual(9, GraphService.CalculateRouteDistance(graph, "A-B-C"));
            Assert.AreEqual(5, GraphService.CalculateRouteDistance(graph, "A-D"));
            Assert.AreEqual(13, GraphService.CalculateRouteDistance(graph, "A-D-C"));
            Assert.AreEqual(22, GraphService.CalculateRouteDistance(graph, "A-E-B-C-D"));
            Assert.AreEqual(-1, GraphService.CalculateRouteDistance(graph, "A-E-D"));
        }

        [Test]
        public void CalculateAllRoutesWithMaxStopsTest()
        {            
            string graphInfo = "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7";
                                
            GraphModel graph = GraphService.PopulateGraphModel(graphInfo);
            
            var result = GraphService.CalculateAllRoutesWithMaxStops(graph, 'C', 'C', 3);
            Assert.AreEqual(2, result);                        
        }
    }
}
