using System.Collections.Generic;
using System.Text;
using TrainGraph.Data.Models;

namespace TrainGraph.Data
{   
    public class AllRoutesUtil
    {   
        private GraphModel graph;

        private char startNode;

        private char endNode;

        private int stopNumber;

        private List<string> routes;

        private Stack<char> nodesToVisit;

        /// <summary>
        /// Initializes a new instance of the AllRoutesUtil class with the informations provided.
        /// </summary>
        /// <param name="graph">Graph to executed.</param>
        /// <param name="startNode">Initial node of path.</param>
        /// <param name="endNode">End node of a path.</param>
        /// <param name="stopNumber">Number of stops in a path.</param>
        public AllRoutesUtil(GraphModel graph, char startNode, char endNode, int stopNumber)
        {
            this.graph = graph;
            this.startNode = startNode;
            this.endNode = endNode;
            this.stopNumber = stopNumber;

            routes = new List<string>();

            nodesToVisit = new Stack<char>();
            nodesToVisit.Push(startNode);
        }

        /// <summary>
        /// Computes all routes between two nodes in a graph, based in a stop number count
        /// <param name="maxStopNumber">If <code>true</code> indicates that the algorithm will search for a route with max number of stops,
        /// otherwise, algorothm will search for a route with exactly number of stops</param>
        /// </summary>
        private void ComputeRoutes(bool useStopCount)
        {
            StringBuilder route = new StringBuilder();
            int actualWeight = 0; 

            int count;
            if (useStopCount)
                count = -1;
            else
                count = 0;

            while (nodesToVisit.Count > 0)
            {
                char actualNode = nodesToVisit.Pop();
                NodeModel node = graph.Nodes.Find(n => n.Label.Equals(actualNode));

                // Verify if route still valid
                if (route.Length > 0)
                {
                    char priorNodeToVerify = route[route.Length - 1];
                    NodeModel verNode = graph.Nodes.Find(n => n.Label.Equals(priorNodeToVerify));
                    EdgeModel verEdge = verNode.Edges.Find(n => n.Label.Equals(node.Label));

                    if (verEdge == null)
                        continue;
                    else if (!useStopCount)
                        actualWeight = verEdge.Weight;
                }

                // Add a node to route
                if (route.Length == 0)
                    route.Append(node.Label);
                else
                    route.Append("-" + node.Label);

                if (useStopCount)
                    count++;
                else
                    count += actualWeight;

                // Verify if maximum stops reached
                if ((useStopCount && ((count >= stopNumber) && !(node.Label.Equals(endNode))))
                    || (!useStopCount && ((count >= stopNumber))))
                {
                    route.Remove(route.Length-2, 2);
                    if (useStopCount)
                        count--;
                    else
                        count -= actualWeight;

                    bool continueValidation = true;
                    while (continueValidation)
                    {
                        char nodeToVerify = route[route.Length - 1];
                        char priorNodeToVerify = route[route.Length - 3];

                        NodeModel verNode = graph.Nodes.Find(n => n.Label.Equals(nodeToVerify));
                        NodeModel priorNode = graph.Nodes.Find(n => n.Label.Equals(priorNodeToVerify));
                        
                        if (verNode.Edges.Count <= 1
                            || (priorNode.Edges.Find(n => n.Label.Equals(nodeToVerify)) == null))
                        {
                            route.Remove(route.Length - 2, 2);
                            if (useStopCount)
                                count--;
                            else
                                count -= actualWeight;
                            continueValidation = (route.Length > 2);
                        }
                        else
                            continueValidation = false;
                    }
                    if ((nodesToVisit.Count == 1) && (count >= stopNumber))
                    {
                        count = 0;
                        route.Clear();
                        route.Append(startNode);
                    }
                    continue;
                }

                // Verify if found a path end 
                if (node.Label.Equals(endNode))
                {
                    if (!routes.Contains(route.ToString())
                        && route.ToString().Split('-').Length > 1)
                        routes.Add(route.ToString());

                    if (count >= stopNumber-1)
                    {                        
                        route.Clear();
                        route.Append(startNode);
                        count = 0;
                        continue;
                    }
                }

                foreach (EdgeModel conn in node.Edges)
                {
                    nodesToVisit.Push(conn.Label);
                }
            } 
        }

        /// <summary>
        /// Computes all routes between two nodes in a graph.
        /// <param name="maxStopNumber">If <code>true</code> indicates that the algorithm will search for a route with max number of stops,
        /// otherwise, algorithm will search for a route with exactly number of stops</param>
        /// </summary>
        /// <returns></returns>
        private List<GraphRouteModel> ComputeAllRoutes(bool isMaxStopNumber)
        {
            ComputeRoutes(true);

            List<GraphRouteModel> result = new List<GraphRouteModel>();

            foreach (string route in routes)
            {
                GraphRouteModel path = new GraphRouteModel();
                path.Value = route.Split('-').Length - 1;       // number of stops in path
                path.Path = route;

                if (isMaxStopNumber || (!isMaxStopNumber && path.Value == stopNumber))
                    result.Add(path);
            }

            return result;
        }


        /// <summary>
        /// Computes all routes between two nodes in a graph with a maximum number of stops.
        /// </summary>
        /// <returns></returns>
        public List<GraphRouteModel> ComputeAllRoutesMaxStops()
        {
            return ComputeAllRoutes(true);
        }

        /// <summary>
        /// Computes all routes between two nodes in a graph with a defined number of stops.
        /// </summary>
        /// <returns></returns>
        public List<GraphRouteModel> ComputeAllRoutesNumStops()
        {
            return ComputeAllRoutes(false);
        }

        /// <summary>
        /// Computes the number of routes between two nodes with a maximum distance.
        /// </summary>
        /// <returns></returns>
        public int ComputeAllRoutesMaxDistance()
        {
            ComputeRoutes(false);

            return routes.Count;
        }
    }
}
