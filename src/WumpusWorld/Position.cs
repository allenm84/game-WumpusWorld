using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public struct Position : IEquatable<Position>, IComparable<Position>
  {
    public int Row, Column;
    
    public Position(int both)
    {
      Row = both;
      Column = both;
    }

    public Position(int row, int column)
    {
      Row = row;
      Column = column;
    }

    static bool equals(Position x, Position y)
    {
      return (x.Row == y.Row) && (x.Column == y.Column);
    }

    public override bool Equals(object obj)
    {
      Position? other = obj as Position?;
      if (!other.HasValue) return false;
      return equals(this, other.Value);
    }

    public bool Equals(Position other)
    {
      return equals(this, other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Row << 16) | Column);
      }
    }

    public int CompareTo(Position other)
    {
      int comparison = Row.CompareTo(other.Row);
      if (comparison == 0)
      {
        comparison = Column.CompareTo(other.Column);
      }
      return comparison;
    }
  }
}
