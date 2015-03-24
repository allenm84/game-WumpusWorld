using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public abstract class TurnAgentAction : BaseAgentAction
  {
    private readonly Lazy<AgentDirection[]> lzDirectionOrder;

    public TurnAgentAction()
    {
      lzDirectionOrder = new Lazy<AgentDirection[]>(InternalCreateDirectionOrder, true);
    }

    protected abstract AgentDirection[] InternalCreateDirectionOrder();

    public override AgentState Do(AgentState state)
    {
      var directionOrder = lzDirectionOrder.Value;
      int index = directionOrder.IndexOf(state.Direction);
      index = (index + 1) % 4;
      state.Direction = directionOrder[index];
      return state;
    }
  }
}
