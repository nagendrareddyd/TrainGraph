using System.Collections.Generic;

namespace TrainGraph.Data.Models
{
    public class NodeModel
    {
        public char Label { get; set; }
        public List<EdgeModel> Edges { get; set; }

        public NodeModel()
        {
            Edges = new List<EdgeModel>();
        }
    }
}
