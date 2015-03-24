using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The agent can go Forward in the direction it is currently facing. Going Forward into a wall will generate a Bump percept.
  /// </summary>
  public class MoveForwardAgentAction : BaseAgentAction
  {
    public override AgentState Do(AgentState state)
    {
      var direction = state.Direction;

      switch (direction)
      {
        case AgentDirection.East:
          {
            state.Position.Column++;
            break;
          }
        case AgentDirection.North:
          {
            state.Position.Row--;
            break;
          }
        case AgentDirection.South:
          {
            state.Position.Row++;
            break;
          }
        case AgentDirection.West:
          {
            state.Position.Column--;
            break;
          }
      }

      return state;
    }
  }
}
