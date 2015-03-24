using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public struct AgentState
  {
    public AgentDirection Direction;
    public Position Position;
    public int Arrows;
    public bool HoldingObject;
    public bool Escaped;

    public bool AtStartingPosition()
    {
      return Position.Row == 1 && Position.Column == 1;
    }
  }
}
