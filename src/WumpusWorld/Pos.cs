using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public struct Pos : IEquatable<Pos>
  {
    public int x, y;

    public Pos(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((y << 16) | x);
      }
    }

    public override bool Equals(object obj)
    {
      var p = obj as Pos?;
      if (p == null) return false;
      return Equals(p.Value);
    }

    public bool Equals(Pos other)
    {
      return (x == other.x) && (y == other.y);
    }
  }
}
