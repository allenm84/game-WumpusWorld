using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public struct Index : IEquatable<Index>
  {
    public static readonly Index Empty = new Index();

    static bool eq(Index a, Index b)
    {
      return (a.col == b.col) && (a.row == b.row);
    }

    private int col;
    private int row;

    public Index(int column, int row)
    {
      this.col = column;
      this.row = row;
    }

    public bool IsEmpty
    {
      get { return ((this.col == 0) && (this.row == 0)); }
    }

    public int Column
    {
      get { return this.col; }
      set { this.col = value; }
    }

    public int Row
    {
      get { return this.row; }
      set { this.row = value; }
    }

    public static bool operator ==(Index left, Index right)
    {
      return eq(left, right);
    }

    public static bool operator !=(Index left, Index right)
    {
      return !(left == right);
    }

    public bool IsValid(int rows, int cols)
    {
      return (-1 < Row && Row < rows)
        && (-1 < Column && Column < cols);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Index))
      {
        return false;
      }

      Index index = (Index)obj;
      return Equals(index);
    }

    public bool Equals(Index other)
    {
      return eq(this, other);
    }

    public bool Equals(int row, int col)
    {
      return eq(this, new Index(col, row));
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((row << 16) | col);
      }
    }

    public override string ToString()
    {
      return String.Format("{{Column={0}, Row={1}}}", col, row);
    }
  }
}
