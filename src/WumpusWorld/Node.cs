using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class Node : IEquatable<Node>
  {
    private Pos pos;
    public int x { get { return pos.x; } }
    public int y { get { return pos.y; } }

    public Node(Pos p)
    {
      pos = p;
    }

    public Percept percept;
    public double wumpusPc;
    public double pitPc;

    public float g_score;
    public float f_score;

    public override int GetHashCode()
    {
      return pos.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Node n = obj as Node;
      if (n == null) return false;
      return Equals(n);
    }

    public bool Equals(Node other)
    {
      return pos.Equals(other.pos);
    }
  }
}
