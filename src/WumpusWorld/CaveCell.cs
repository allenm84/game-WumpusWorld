using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class CaveCell
  {
    public int Row { get; private set; }
    public int Column { get; private set; }

    public CaveCellType Type { get; set; }
    public bool Revealed { get; set; }
    public bool ContainsStench { get; set; }
    public bool ContainsBreeze { get; set; }

    public CaveCell(int r, int c)
    {
      Row = r;
      Column = c;
    }
  }
}
