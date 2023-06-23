using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahnCargoSim.Model
{
  public class Grid
  {
    public List<Node>? Nodes { get; set; }
    public List<Edge>? Edges { get; set; }
    public List<Connection>? Connections { get; set; }
  }
}
