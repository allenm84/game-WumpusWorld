using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class KnowledgeActionHistory
  {
    public AgentState State { get; private set; }
    public BaseAgentAction Action { get; private set; }

    public KnowledgeActionHistory(AgentState state, BaseAgentAction action)
    {
      State = state;
      Action = action;
    }
  }
}
