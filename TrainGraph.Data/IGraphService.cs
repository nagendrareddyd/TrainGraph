using TrainGraph.Data.Models;

namespace TrainGraph.Data
{
    public interface IGraphService
    {
        GraphModel PopulateGraphModel(string grahpInfo);
        int CalculateRouteDistance(GraphModel graph, string route);
        int? CalculateAllRoutesWithMaxStops(GraphModel routeGraph, char startNode, char endNode
            , int maxStops);
        int? CalculateAllRoutesWithStops(GraphModel routeGraph, char startNode, char endNode
           , int stops);
    }
}
