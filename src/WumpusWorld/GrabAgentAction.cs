using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The agent can Grab a portable object at the current square or it can Release an object that it is holding.
  /// </summary>
  public class GrabAgentAction : BaseAgentAction
  {
    public override AgentState Do(AgentState state)
    {
      state.HoldingObject = !state.HoldingObject;
      return state;
    }
  }
}
