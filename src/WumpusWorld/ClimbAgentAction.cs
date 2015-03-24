using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The agent can Climb out of the cave if at the Start square.
  /// </summary>
  public class ClimbAgentAction : BaseAgentAction
  {
    public override AgentState Do(AgentState state)
    {
      if (state.AtStartingPosition())
      {
        state.Escaped = true;
      }
      return state;
    }
  }
}
