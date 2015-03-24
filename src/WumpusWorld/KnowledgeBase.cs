using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class KnowledgeBase
  {
    private readonly Dictionary<Position, Percept> knowledgeBase;
    private readonly List<KnowledgeActionHistory> history;
    private readonly List<BaseKnowledgeRule> rules;

    private bool wumpusIsDead = false;
    private Position? wumpusLocation = null;

    public KnowledgeBase()
    {
      knowledgeBase = new Dictionary<Position, Percept>();
      history = new List<KnowledgeActionHistory>();
      rules = new List<BaseKnowledgeRule>();
    }

    public void Tell(AgentState state, Percept percept)
    {
      Percept p;
      if (!knowledgeBase.TryGetValue(state.Position, out p))
      {
        p = Percept.None;
      }

      p &= ~percept;
      p |= percept;

      knowledgeBase[state.Position] = p;

      if (percept.HasFlag(Percept.Scream))
      {
        wumpusIsDead = true;
      }
    }

    public void Tell(AgentState state, BaseAgentAction action)
    {
      history.Add(new KnowledgeActionHistory(state, action));
    }

    public void Relax()
    {
      if (wumpusLocation == null)
      {
        InferWumpusLocation();
      }
    }

    private void InferWumpusLocation()
    {
      var checkedStenches = new HashSet<Position>();
      foreach (var kvp in knowledgeBase.Where(k => k.Value.HasFlag(Percept.Stench)))
      {
        if (!checkedStenches.Add(kvp.Key))
        {
          continue;
        }

        // any of the neighbors of the stench location could be a wumpus
        var potentialWumpus = GetNeighbors(kvp.Key);
        foreach (var w in potentialWumpus)
        {
          var stenches = GetNeighbors(w);
          var all = true;

          foreach (var stench in stenches)
          {
            checkedStenches.Add(stench);

            Percept p;
            if (!knowledgeBase.TryGetValue(stench, out p))
            {
              p = Percept.None;
            }

            all |= (p.HasFlag(Percept.Stench));
            if (!all)
            {
              break;
            }
          }

          // if all the neighbors of this square stink, then this is the wumpus
          if (all)
          {
            wumpusLocation = w;
            return;
          }
        }
      }
    }

    private IEnumerable<Position> GetNeighbors(Position l)
    {
      yield return new Position(l.Row + 1, l.Column);
      yield return new Position(l.Row - 1, l.Column);
      yield return new Position(l.Row, l.Column + 1);
      yield return new Position(l.Row, l.Column - 1);
    }

    private bool IsFacing(AgentState state, Position location)
    {
      int dc = Math.Sign(state.Position.Column - location.Column);
      int dr = Math.Sign(state.Position.Row - location.Row);

      if (dc != 0 && dr == 0)
      {
        if (dc == 1)
        {
          return (state.Direction == AgentDirection.West);
        }
        else
        {
          return (state.Direction == AgentDirection.East);
        }
      }
      else if (dr != 0 && dc == 0)
      {
        if (dr == 1)
        {
          return (state.Direction == AgentDirection.North);
        }
        else
        {
          return (state.Direction == AgentDirection.South);
        }
      }

      return false;
    }

    private bool CanFace(AgentState state, Position location)
    {
      return (state.Position.Column == location.Column) || (state.Position.Row == location.Row);
    }

    public BaseAgentAction Ask(AgentState state)
    {
      // based on the current state, and our current knowledge
      Percept p;
      if (knowledgeBase.TryGetValue(state.Position, out p))
      {
        if (p.HasFlag(Percept.Glitter))
        {
          return new GrabAgentAction();
        }

        if (state.AtStartingPosition() && state.HoldingObject)
        {
          return new ClimbAgentAction();
        }

        if (p.HasFlag(Percept.Stench) && !wumpusIsDead)
        {
          // the square we're in is stinky. At the very least, we do not want to go forward, we want
          // to turn. BUT, if we know where the wumpus is, let's kill it. If we're not facing the
          // right direction, we'll need to turn to face
          if (wumpusLocation.HasValue)
          {
            var wumpus = wumpusLocation.Value;
            if (IsFacing(state, wumpus))
            {
              return new ShootAgentAction();
            }
            else if (CanFace(state, wumpus))
            {
              return new TurnRightAgentAction();
            }
            else
            {
              // we know where the wumpus is, but we aren't facing it, and we can't face it.
              // let's just keep moving
              var forward = LocationForward(state);
              if (!forward.Value.HasFlag(Percept.Bump))
              {
                return new MoveForwardAgentAction();
              }
              else
              {
                return new TurnRightAgentAction();
              }
            }
          }
          else
          {
            return new TurnRightAgentAction();
          }
        }

        if (p.HasFlag(Percept.Breeze))
        {
          return new TurnRightAgentAction();
        }
      }

      return new MoveForwardAgentAction();
    }

    private KeyValuePair<Position, Percept> LocationForward(AgentState state)
    {
      throw new NotImplementedException();
    }
  }
}
