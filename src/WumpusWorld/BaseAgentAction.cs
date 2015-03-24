using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  /// <summary>
  /// The Agent can perform a single action
  /// </summary>
  public abstract class BaseAgentAction
  {
    public abstract AgentState Do(AgentState state);
  }
}
