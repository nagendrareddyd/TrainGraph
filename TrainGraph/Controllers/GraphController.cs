using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TrainGraph.Data;
using TrainGraph.Data.Models;

namespace TrainGraph.Controllers
{
    public class GraphController : Controller
    {
        private IGraphService graphService;

        public GraphController(IGraphService _GraphService)
        {
            graphService = _GraphService;
        }

        // GET: Graph
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Upload graph information text file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFile()
        {
            try
            {
                string graphInfo = string.Empty;

                if (HttpContext.Request.Files.Count != 0)
                {
                    HttpPostedFileBase file = HttpContext.Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        graphInfo = new StreamReader(file.InputStream).ReadToEnd();
                    }
                }               

                GraphModel graph = graphService.PopulateGraphModel(graphInfo);

                return Json(new GraphResult{ Graph = graph, GraphInfo = graphInfo },JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Calculate the total distance of any route in a graph.
        /// </summary>
        /// <param name="routePath">Route path format : A-B-C</param>
        /// <returns>Total distance if exists, -1 if doesn't exists a route</returns>
        [HttpPost]
        public ActionResult GetRouteDistance(GraphModel graph,string routePath)
        {
            var response = graphService.CalculateRouteDistance(graph, routePath);

            if (response == -1)
                return Json("No SUCH ROUTE");

            return Json(response);
        }
        [HttpPost]
        public ActionResult GetRoutesWithStops(GraphModel graph,char startNode,char endNode,int stops,bool isMaxStops)
        {
            var result = isMaxStops ? graphService.CalculateAllRoutesWithMaxStops(graph, startNode, endNode, stops) : graphService.CalculateAllRoutesWithStops(graph, startNode, endNode, stops);

            if (result == -1)
                return Json("No SUCH ROUTE");

            return Json(result);
        }
    }
}