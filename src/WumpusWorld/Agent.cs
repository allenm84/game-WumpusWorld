using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public class Agent
  {
    private readonly IAgentWorld world;
    private int time;

    public Index Index { get; set; }
    public int Column { get { return Index.Column; } }
    public int Row { get { return Index.Row; } }

    public bool HasArrow { get; private set; }
    public Orientation Orientation { get; private set; }
    public bool Alive { get; private set; }
    public bool Escaped { get; private set; }
    public bool HasGold { get; private set; }

    private Index? previousLocation = null;

    public Agent(IAgentWorld world)
    {
      this.world = world;
      Reset();
    }

    public void Reset()
    {
      // we start out facing right
      Orientation = Orientation.Right;

      // we can fire an arrow
      HasArrow = true;

      // reset the time to 0
      time = 0;

      // we're still alive
      Alive = true;

      // we don't have the gold
      HasGold = false;

      // reset the knowledge base
      KnowledgeBase.Reset();
    }

    private void Rollback()
    {
      if (previousLocation != null)
      {
        Index = previousLocation.Value;
        previousLocation = null;
      }
    }

    private void Perform(Action action)
    {
      switch (action)
      {
        case Action.Climb:
          {
            Escaped = world.ClimbOut(Index);
            break;
          }
        case Action.Grab:
          {
            HasGold = world.GrabGold(Index);
            break;
          }
        case Action.MoveForward:
          {
            Index next = KnowledgeBase.Forward(Index, Orientation);
            previousLocation = Index;
            Index = next;
            break;
          }
        case Action.Shoot:
          {
            if (HasArrow)
            {
              HasArrow = false;
              Index dir = KnowledgeBase.Forward(Index, Orientation);
              dir.Column = dir.Column - Index.Column;
              dir.Row = dir.Row - Index.Row;
              world.FireArrow(Index, dir);
            }
            break;
          }
        case Action.TurnLeft:
          {
            Orientation newOrientation = Orientation;
            switch (Orientation)
            {
              case Orientation.Down: newOrientation = Orientation.Right; break;
              case Orientation.Left: newOrientation = Orientation.Down; break;
              case Orientation.Right: newOrientation = Orientation.Up; break;
              case Orientation.Up: newOrientation = Orientation.Left; break;
            }
            Orientation = newOrientation;
            break;
          }
        case Action.TurnRight:
          {
            Orientation newOrientation = Orientation;
            switch (Orientation)
            {
              case Orientation.Down: newOrientation = Orientation.Left; break;
              case Orientation.Left: newOrientation = Orientation.Up; break;
              case Orientation.Right: newOrientation = Orientation.Down; break;
              case Orientation.Up: newOrientation = Orientation.Right; break;
            }
            Orientation = newOrientation;
            break;
          }
      }
    }

    public void Perceive(Percept percept)
    {
      KnowledgeBase.Tell(MakePerceptSentence(percept, time));
      if (percept == Percept.Bump)
      {
        Rollback();
      }

      Action action = KnowledgeBase.Ask(MakeActionQuery(time));
      ++time;
      Perform(action);
    }

    private ActionQuery MakeActionQuery(int time)
    {
      return new ActionQuery
      {
        Agent = this,
        Time = time,
      };
    }

    private PerceptSentence MakePerceptSentence(Percept percept, int time)
    {
      return new PerceptSentence
      {
        Agent = this,
        Percept = percept,
        Time = time,
      };
    }
  }
}
