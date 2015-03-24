using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public sealed class PositionEqualityComparer : IEqualityComparer<Position>, IComparer<Position>
  {
    static Lazy<PositionEqualityComparer> sInstance = new Lazy<PositionEqualityComparer>(true);
    public static PositionEqualityComparer Instance { get { return sInstance.Value; } }
    private PositionEqualityComparer() { }

    public bool Equals(Position x, Position y)
    {
      return x.Equals(y);
    }

    public int GetHashCode(Position obj)
    {
      return obj.GetHashCode();
    }

    public int Compare(Position x, Position y)
    {
      return x.CompareTo(y);
    }
  }
}
