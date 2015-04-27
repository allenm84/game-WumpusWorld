using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public class Knowledge : IEquatable<Knowledge>
  {
    public Index Room { get; set; }
    public KnowledgeType Type { get; set; }

    public override int GetHashCode()
    {
      unchecked
      {
        long l = ((int)Type) << 32 | Room.GetHashCode();
        return l.GetHashCode();
      }
    }

    public override bool Equals(object obj)
    {
      Knowledge other = obj as Knowledge;
      if (other == null) return false;
      return Equals(other);
    }

    public bool Equals(Knowledge other)
    {
      return other.Room.Equals(Room) &&
        other.Type == Type;
    }

    public override string ToString()
    {
      return string.Format("{0} in Room ({1},{2})", Type, Room.Column, Room.Row);
    }
  }
}
