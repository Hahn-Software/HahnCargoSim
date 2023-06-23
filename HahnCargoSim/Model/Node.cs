using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahnCargoSim.Model
{
  public class Node
  {
    public int Id { get; set; }
    public string Name { get; set; }

    public Node Clone()
    {
      return new Node
      {
        Id = this.Id,
        Name = this.Name
      };
    }

  }
}
