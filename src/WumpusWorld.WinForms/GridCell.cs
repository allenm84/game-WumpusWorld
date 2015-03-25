using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld.WinForms
{
  public class GridCell
  {
    public int Row { get; private set; }
    public int Column { get; private set; }

    public bool Pit { get; set; }
    public bool Wumpus { get; set; }
    public bool Gold { get; set; }

    public GridCell(int r, int c)
    {
      Row = r;
      Column = c;
    }
  }
}
