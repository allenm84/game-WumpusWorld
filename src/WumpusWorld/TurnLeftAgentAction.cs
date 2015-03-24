using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The agent will turn left
  /// </summary>
  public class TurnLeftAgentAction : TurnAgentAction
  {
    protected override AgentDirection[] InternalCreateDirectionOrder()
    {
      return new AgentDirection[]
      {
        AgentDirection.East,
        AgentDirection.North,
        AgentDirection.West,
        AgentDirection.South,
      };
    }
  }
}
