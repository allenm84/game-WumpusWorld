using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class Arrow
  {
    public Index Position { get; set; }
    public int Steps { get; set; }
    public Index Direction { get; set; }
    public bool Alive { get; set; }
  }
}
