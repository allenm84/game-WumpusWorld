using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class ShootAgentAction : BaseAgentAction
  {
    public override AgentState Do(AgentState state)
    {
      if (state.Arrows > 0)
      {
        --state.Arrows;
      }
      return state;
    }
  }
}
