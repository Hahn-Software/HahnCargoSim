using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahnCargoSim.Model
{
  public class Connection
  {
    public int Id { get; set; }
    public int EdgeId { get; set; }
    public int FirstNodeId { get; set; }
    public int SecondNodeId { get; set; }

  }
}
