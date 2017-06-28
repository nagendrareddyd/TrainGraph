using System.Collections.Generic;

namespace TrainGraph.Data.Models
{
   public class GraphModel
    {
        public List<NodeModel> Nodes { get; set; }
        public GraphModel()
        {
            Nodes = new List<NodeModel>();
        }
    }
}
