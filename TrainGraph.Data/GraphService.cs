using System;
using TrainGraph.Data.Models;

namespace TrainGraph.Data
{
    public class GraphService : IGraphService
    {
        /// <summary>
        /// Calculate the total distance of any route in a graph.
        /// </summary>
        /// <param name="routeGraph">Graph Info</param>
        /// <param name="route">Route in format : A-B-C</param>
        /// <returns></returns>
        public int CalculateRouteDistance(GraphModel routeGraph, string route)
        {
            int routeDistance = 0;            
            route = route.Replace(" ", "").ToUpper().Trim();
            char[] routeNodes = route.Replace("-","").Trim().ToCharArray();
                        
            if (routeNodes.Length < 2)
                return -1;             
            
            for (int i = 0; i < (routeNodes.Length-1); i++)
            {
                char nodeName = routeNodes[i];
                NodeModel nodeElement  = routeGraph.Nodes.Find(s => s.Label.Equals(nodeName));
                if (nodeElement != null)
                {
                    char edgeName = Convert.ToChar(routeNodes[i + 1]);
                    EdgeModel edge = nodeElement.Edges.Find(s => s.Label.Equals(edgeName));
                    if (edge != null)
                    {
                        routeDistance += edge.Weight;
                    }
                    else
                        return -1;
                }
                else
                    return -1;
            }
            return routeDistance;
        }

        /// <summary>
        /// Loads the graph model
        /// </summary>
        /// <param name="grahpInfo">Info about the graphn as text</param>
        /// <returns>Graph Model</returns>
        public GraphModel PopulateGraphModel(string grahpInfo)
        {
            GraphModel routeGraph = new GraphModel();
            grahpInfo = grahpInfo.Replace(" ", "").Trim();
            string[] nodeElements = grahpInfo.Split(',');

            foreach(var nodeElement in nodeElements)
            {
                if(nodeElement.Length == 3)
                {                    
                    char nodeName = nodeElement[0];
                    EdgeModel edge = new EdgeModel
                    {
                        Label = nodeElement[1],
                        Weight = int.Parse(nodeElement[2].ToString())
                    };

                    NodeModel node =  routeGraph.Nodes.Find(n => n.Label.Equals(nodeName));

                    // Add a new node or new edge in a existing node
                    if (node != null)
                    {
                        if (node.Edges.Find(e => e.Label.Equals(node.Label)) == null)
                            node.Edges.Add(edge);
                        else
                            throw new Exception(string.Format("Node: {0}, Weight: {1}", node.Label, edge.Weight));
                    }
                    else
                    {
                        node = new NodeModel
                        {
                            Label = nodeName
                        };
                        node.Edges.Add(edge);
                        routeGraph.Nodes.Add(node);
                    }
                }
                else
                    throw new Exception(string.Format("The node information '{0}' is incorrect.", nodeElement));
            }
            return routeGraph;
        }

        /// <summary>
        /// Calculate all routes between two nodes with a maximum number of stops.
        /// </summary>
        /// <param name="routeGraph">Graph info</param>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="maxStops"></param>
        /// <returns></returns>
        public int? CalculateAllRoutesWithMaxStops(GraphModel routeGraph, char startNode, char endNode
            , int maxStops)
        {
            AllRoutesUtil allRoutesUtil = new AllRoutesUtil(routeGraph, startNode, endNode, maxStops);

            return allRoutesUtil.ComputeAllRoutesMaxStops()?.Count;
        }

        public int? CalculateAllRoutesWithStops(GraphModel routeGraph, char startNode, char endNode
            , int stops)
        {
            AllRoutesUtil allRoutesUtil = new AllRoutesUtil(routeGraph, startNode, endNode, stops);

            return allRoutesUtil.ComputeAllRoutesNumStops()?.Count;
        }
    }
}
