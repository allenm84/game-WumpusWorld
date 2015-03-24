using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class Agent
  {
    private readonly KnowledgeBase knowledge;
    private readonly IWumpusCave cave;

    private AgentState state;

    public Agent(IWumpusCave cave)
    {
      this.knowledge = new KnowledgeBase();
      this.cave = cave;

      state = new AgentState();
      state.Arrows = 1;
      state.Direction = AgentDirection.East;
      state.Position = new Position(1, 1);
      state.HoldingObject = false;
      state.Escaped = false;
    }

    public void Run()
    {
      var percept = cave.Percept(state.Position);
      knowledge.Tell(state, percept);
      knowledge.Relax();

      var action = knowledge.Ask(state);
      if (Peform(action))
      {
        knowledge.Tell(state, action);
      }
    }

    private bool Peform(BaseAgentAction action)
    {
      throw new NotImplementedException();
    }
  }
}
