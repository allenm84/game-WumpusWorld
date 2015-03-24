using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The agent will turn right
  /// </summary>
  public class TurnRightAgentAction : TurnAgentAction
  {
    protected override AgentDirection[] InternalCreateDirectionOrder()
    {
      return new AgentDirection[]
      {
        AgentDirection.East,
        AgentDirection.South,
        AgentDirection.West,
        AgentDirection.North,
      };
    }
  }
}
