using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WumpusWorld
{
  internal static class KnowledgeBase
  {
    private static readonly Random random = new Random();

    private static List<Knowledge> CurrentKnowledge;
    private static HashSet<Index> Visited;
    private static Dictionary<Index, Percept> Percepts;
    private static List<KnowledgeRule> Rules;
    private static DijkstraNode CurrentNode;

    public static IEnumerable<Index> Rooms
    {
      get
      {
        return Percepts.Keys
          .Concat(CurrentKnowledge.Select(k => k.Room))
          .Distinct()
          .ToArray();
      }
    }

    public static void Reset()
    {
      // clear the debug
      Debug.WriteLine("#######################################################################");
      Debug.WriteLine("#                    Wumpus World Reset                               #");

      // clear the knowledge
      if (CurrentKnowledge != null)
      {
        CurrentKnowledge.Clear();
        CurrentKnowledge = null;
      }

      // initialize the knowledge
      CurrentKnowledge = new List<Knowledge>();

      // clear the visited
      if (Visited != null)
      {
        Visited.Clear();
        Visited = null;
      }

      // initialize the visited
      Visited = new HashSet<Index>();

      // clear the percepts
      if (Percepts != null)
      {
        Percepts.Clear();
        Percepts = null;
      }

      // initialize the percepts
      Percepts = new Dictionary<Index, Percept>();

      // clear the rules
      if (Rules != null)
      {
        Rules.Clear();
        Rules = null;
      }

      // initialize the rules
      Type knowledgeRuleType = typeof(KnowledgeRule);
      Rules = (from type in knowledgeRuleType.Assembly.GetTypes()
               where type.IsSubclassOf(knowledgeRuleType) && !type.IsAbstract
               select Activator.CreateInstance(type) as KnowledgeRule).ToList();

      // null the path
      CurrentNode = null;
    }

    public static Index[] GetAdjacentRooms(Index room)
    {
      return new Index[]
      {
        new Index(room.Column, room.Row - 1),
        new Index(room.Column, room.Row + 1),
        new Index(room.Column - 1, room.Row),
        new Index(room.Column + 1, room.Row),
      };
    }

    public static Index Forward(Index room, Orientation orientation)
    {
      Index next = room;
      switch (orientation)
      {
        case Orientation.Down: { next.Row++; break; }
        case Orientation.Left: { next.Column--; break; }
        case Orientation.Right: { next.Column++; break; }
        case Orientation.Up: { next.Row--; break; }
      }
      return next;
    }

    public static bool IsValidRoom(Index room)
    {
      Percept p;
      if (!Percepts.TryGetValue(room, out p))
      {
        // we don't know about it!
        return false;
      }

      return !p.HasFlag(Percept.Bump);
    }

    private static bool IsBump(Index index)
    {
      Percept p;
      if (!Percepts.TryGetValue(index, out p))
      {
        return false;
      }
      return (p.HasFlag(Percept.Bump));
    }

    private static void BuildPathToExit(Index room)
    {
      int rows, columns;
      CalculateBoundingBox(out rows, out columns);

      // build the dijkstra grid
      var grid = new DijkstraNode[columns, rows];
      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < columns; ++c)
        {
          Index index = new Index(c, r);
          bool isIllegal = !Visited.Contains(index);
          grid[c, r] = new DijkstraNode(index, isIllegal);
        }
      }

      // solve the grid
      Dijkstra.SolveGrid(grid, grid[0, 0]);

      // get the node corresponding to our current room
      CurrentNode = grid[room.Column, room.Row];

      // get the parent
      CurrentNode = CurrentNode.Parent;
    }

    private static void CalculateBoundingBox(out int rows, out int columns)
    {
      int min_r = int.MaxValue;
      int min_c = int.MaxValue;
      int max_r = int.MinValue;
      int max_c = int.MinValue;

      foreach (var i in Rooms)
      {
        min_r = Math.Min(min_r, i.Row);
        min_c = Math.Min(min_c, i.Column);

        max_r = Math.Max(max_r, i.Row);
        max_c = Math.Max(max_c, i.Column);
      }

      rows = (max_r - min_r);
      columns = (max_c - min_c);
    }

    private static bool TurnToFace(Agent agent, Index roomToFace, out Action turnAction)
    {
      // we need to turn the agent to face a room. First, get the room that the agent is currently
      // in.
      Index room = agent.Index;

      // next, we need to determine the orientation
      Orientation orientation = agent.Orientation;

      // get the orientation
      try { orientation = DetermineOrientation(room, roomToFace); }
      catch { orientation = agent.Orientation; }

      // if the orientation is different than our own, then we need to turn, otherwise, return false
      if (agent.Orientation == orientation)
      {
        turnAction = Action.MoveForward;
        return false;
      }

      // we need to turn
      turnAction = LeastPath(agent.Orientation, orientation);
      return true;
    }

    private static Action LeastPath(Orientation current, Orientation target)
    {
      if (current == Orientation.Down)
      {
        switch (target)
        {
          case Orientation.Left: { return Action.TurnRight; }
          case Orientation.Right: { return Action.TurnLeft; }
          case Orientation.Up: { return Action.TurnLeft; }
        }
      }
      else if (current == Orientation.Left)
      {
        switch (target)
        {
          case Orientation.Down: { return Action.TurnLeft; }
          case Orientation.Right: { return Action.TurnLeft; }
          case Orientation.Up: { return Action.TurnRight; }
        }
      }
      else if (current == Orientation.Right)
      {
        switch (target)
        {
          case Orientation.Down: { return Action.TurnRight; }
          case Orientation.Left: { return Action.TurnLeft; }
          case Orientation.Up: { return Action.TurnLeft; }
        }
      }
      else if (current == Orientation.Up)
      {
        switch (target)
        {
          case Orientation.Down: { return Action.TurnLeft; }
          case Orientation.Left: { return Action.TurnLeft; }
          case Orientation.Right: { return Action.TurnRight; }
        }
      }

      // if all else fails, turn left
      return Action.TurnLeft;
    }

    private static Orientation DetermineOrientation(Index position, Index lookAt)
    {
      if (position.Column < lookAt.Column) return Orientation.Right;
      if (position.Column > lookAt.Column) return Orientation.Left;

      if (position.Row < lookAt.Row) return Orientation.Down;
      if (position.Row > lookAt.Row) return Orientation.Up;

      throw new InvalidOperationException("The indices are equal");
    }

    private static Action MoveAgentToRoom(Agent agent, Index room)
    {
      // turn to face if needed
      Action turnAction;
      if (TurnToFace(agent, room, out turnAction))
      {
        // the turn to face returns true if we needed to turn
        return turnAction;
      }

      // move forward
      return Action.MoveForward;
    }

    private static bool IsGoodIdeaToMoveToRoom(Index room)
    {
      // get everything we know about this room
      var knowledgeForRoom = from k in CurrentKnowledge
                             where k.Room.Equals(room)
                             select k;

      // the only time we wouldn't move to a room is if it IS a pit, or
      // if it IS a wumpus, or it might be.
      int countDeadlySituations = (from k in knowledgeForRoom
                                   where IsNonBeneficialType(k.Type)
                                   select k).Count();

      // if the deadly situations count is greater than zero, then return false
      return countDeadlySituations == 0;
    }

    private static bool IsNonBeneficialType(KnowledgeType knowledgeType)
    {
      return
        knowledgeType == KnowledgeType.IsPit ||
        knowledgeType == KnowledgeType.IsWumpus ||
        knowledgeType == KnowledgeType.MightBePit ||
        knowledgeType == KnowledgeType.MightBeWumpus ||
        knowledgeType == KnowledgeType.Wall;
    }

    private static bool IsCyclic(Index room)
    {
      return true;
    }

    private static Action DetermineInvalidTurnAction(Index room, Orientation orientation)
    {
      int rows, columns;
      CalculateBoundingBox(out rows, out columns);

      // we're either going to turn left or turn right
      switch (orientation)
      {
        case Orientation.Down:
          {
            if (room.Column == 0) return Action.TurnLeft;
            if (room.Column == columns - 1) return Action.TurnRight;
            break;
          }
        case Orientation.Left:
          {
            if (room.Row == 0) return Action.TurnLeft;
            if (room.Row == rows - 1) return Action.TurnRight;
            break;
          }
        case Orientation.Right:
          {
            if (room.Row == 0) return Action.TurnRight;
            if (room.Row == rows - 1) return Action.TurnLeft;
            break;
          }
        case Orientation.Up:
          {
            if (room.Column == 0) return Action.TurnRight;
            if (room.Column == columns - 1) return Action.TurnLeft;
            break;
          }
      }

      // if all else fails, just turn left
      return Action.TurnLeft;
    }

    private static void RelaxKnowledge()
    {
      int rows, columns;
      CalculateBoundingBox(out rows, out columns);

      // we need to remove any knowledge items with invalid indices UNLESS they're a Wall.
      CurrentKnowledge.RemoveAll(knowledge =>
      {
        bool valid = knowledge.Room.IsValid(rows, columns);
        return !valid && knowledge.Type != KnowledgeType.Wall;
      });

      // we need to run all of the rules on the knowledge
      foreach (KnowledgeRule rule in Rules)
      {
        CurrentKnowledge = rule.Relax(CurrentKnowledge, Percepts);
      }

      // remove repeated knowledge
      var distinct = CurrentKnowledge.Distinct().ToList();
      CurrentKnowledge.Clear();
      CurrentKnowledge.AddRange(distinct);
    }

    private static IEnumerable<Knowledge> InferBreezeKnowledge(Index room, bool breeze)
    {
      // create a list to hold the knowledge
      Knowledge[] retval = new Knowledge[5];

      // create an index for the knowledge array return value
      int k = 0;

      // regardless of whether there is a stench or not, we can safely say that
      // there is no wumpus in this room.
      retval[k++] = new Knowledge { Room = room, Type = KnowledgeType.NoPit };

      // retrieve the adjacent rooms
      Index[] adjacentRooms = GetAdjacentRooms(room);

      // if there was a stench in this room. It means that there might be a wumpus
      // in the rooms adjacent to this. If there was NO stench, it means that there
      // is no wumpus in the adjacent rooms
      KnowledgeType type = breeze ? KnowledgeType.MightBePit : KnowledgeType.NoPit;

      // add the remaining knowledge
      foreach (Index adjacentRoom in adjacentRooms)
      {
        retval[k++] = new Knowledge { Room = adjacentRoom, Type = type };
      }

      // return the knowledge
      return retval;
    }

    private static IEnumerable<Knowledge> InferStenchKnowledge(Index room, bool stench)
    {
      // create a list to hold the knowledge
      Knowledge[] retval = new Knowledge[5];

      // create an index for the knowledge array return value
      int k = 0;

      // regardless of whether there is a stench or not, we can safely say that
      // there is no wumpus in this room.
      retval[k++] = new Knowledge { Room = room, Type = KnowledgeType.NoWumpus };

      // retrieve the adjacent rooms
      Index[] adjacentRooms = GetAdjacentRooms(room);

      // if there was a stench in this room. It means that there might be a wumpus
      // in the rooms adjacent to this. If there was NO stench, it means that there
      // is no wumpus in the adjacent rooms
      KnowledgeType type = stench ? KnowledgeType.MightBeWumpus : KnowledgeType.NoWumpus;

      // add the remaining knowledge
      foreach (Index adjacentRoom in adjacentRooms)
      {
        retval[k++] = new Knowledge { Room = adjacentRoom, Type = type };
      }

      // return the knowledge
      return retval;
    }

    public static void Tell(PerceptSentence perceptSentence)
    {
      // Make-percept-sentence takes a percept and a time and returns a sentence representing 
      // the fact that the agent perceived the percept at time t

      // Let's say this is the agent at position [X,Y] and time t, perceived a percept. We need to
      // store this

      // figure out which room the agent is currently in
      Index room = perceptSentence.Agent.Index;

      // store the percept.
      Percepts[room] = perceptSentence.Percept;

      // we visited this room
      if (Visited.Count == 0 || !(Visited.Count > 0 && Visited.Last().Equals(room)))
      {
        Visited.Add(room);
      }

      // if the agent perceived a bump, then it overrides everything else
      if ((perceptSentence.Percept & Percept.Bump) == Percept.Bump)
      {
        CurrentKnowledge.Add(new Knowledge { Room = room, Type = KnowledgeType.Wall });
      }
      else
      {
        // what did the agent perceive in the room
        bool stench = (perceptSentence.Percept & Percept.Stench) == Percept.Stench;
        bool breeze = (perceptSentence.Percept & Percept.Breeze) == Percept.Breeze;

        // make a set of inferences from this
        CurrentKnowledge.AddRange(InferStenchKnowledge(room, stench));
        CurrentKnowledge.AddRange(InferBreezeKnowledge(room, breeze));

        // if the gold was perceived then just add that the gold is in this room
        bool glitter = (perceptSentence.Percept & Percept.Glitter) == Percept.Glitter;
        if (glitter)
        {
          CurrentKnowledge.Add(new Knowledge { Room = room, Type = KnowledgeType.Gold });
        }

        // if there was nothing perceived, then the adjacent squares are ok
        bool none = (perceptSentence.Percept == Percept.None);
        if (none)
        {
          Index[] adjacentRooms = GetAdjacentRooms(room);
          foreach (Index adjacentRoom in adjacentRooms)
          {
            CurrentKnowledge.Add(new Knowledge { Room = adjacentRoom, Type = KnowledgeType.NoWumpus });
            CurrentKnowledge.Add(new Knowledge { Room = adjacentRoom, Type = KnowledgeType.NoPit });
          }
        }

        // if there was a scream, it means there is no wumpus in any of the rooms
        bool scream = (perceptSentence.Percept & Percept.Scream) == Percept.Scream;
        if (scream)
        {
          CurrentKnowledge.RemoveAll(k => k.Type == KnowledgeType.MightBeWumpus || k.Type == KnowledgeType.IsWumpus);
          foreach (Index index in Rooms)
          {
            CurrentKnowledge.Add(new Knowledge { Room = index, Type = KnowledgeType.NoWumpus });
          }
        }
      }

      // relax the knowledge we've gained
      RelaxKnowledge();
    }

    public static Action Ask(ActionQuery actionQuery)
    {
      int rows, columns;
      CalculateBoundingBox(out rows, out columns);

      // get the agents current orientation
      Orientation orientation = actionQuery.Agent.Orientation;

      // Our default action is to move forward. Is that a wise idea?
      Index room = actionQuery.Agent.Index;
      Index next = Forward(room, orientation);

      // do we know where the wumpus is?
      var wumpusLocation = (from k in CurrentKnowledge
                            where k.Type == KnowledgeType.IsWumpus
                            select k).Distinct();
      if (wumpusLocation.Count() > 0 && actionQuery.Agent.HasArrow)
      {
        // get the location of the wumpus
        Index wumpus = wumpusLocation.First().Room;
        if (wumpus.Column == room.Column || wumpus.Row == room.Row)
        {
          // we know exactly where the wumpus is, so we need to kill it
          Action turnAction;
          if (TurnToFace(actionQuery.Agent, wumpusLocation.First().Room, out turnAction))
          {
            // the turn to face returns tree if we needed to turn
            return turnAction;
          }

          // this means we didn't need to turn, so SHOOT!
          return Action.Shoot;
        }
      }

      // do we have the gold?
      if (actionQuery.Agent.HasGold)
      {
        // has a path been made to the exit yet?
        if (CurrentNode == null)
        {
          BuildPathToExit(room);
        }

        // if we're already at the end, then we need to climb out
        if (CurrentNode == null)
        {
          return Action.Climb;
        }

        // move to the next location in the path
        Index path = CurrentNode.Index;
        Action nextAction = MoveAgentToRoom(actionQuery.Agent, path);

        // if we're moving forward
        if (nextAction == Action.MoveForward)
        {
          // update the path
          CurrentNode = CurrentNode.Parent;
        }

        // return the next action
        return nextAction;
      }

      // are we over the gold?
      var goldLocation = from k in CurrentKnowledge
                         where k.Type == KnowledgeType.Gold
                         select k;
      if (goldLocation.Count() == 1 && goldLocation.First().Room.Equals(room))
      {
        return Action.Grab;
      }

      // if the next movement is invalid, we need to turn
      while (IsBump(next))
      {
        orientation = (Orientation)((((int)orientation) + 1) % 4);
        next = Forward(room, orientation);
        Debug.WriteLine("Next move invalid, finding another based on " + orientation + " orientation.");
      }

      // if it's a good idea to move to the next room, then do so
      if (IsGoodIdeaToMoveToRoom(next) && !Visited.Contains(next))
      {
        Debug.WriteLine("Moving to next room since it's a good idea to.");
        return MoveAgentToRoom(actionQuery.Agent, next);
      }

      // get the valid adjacent rooms to our current room
      var adjacentRooms = (from adj in GetAdjacentRooms(room) where IsValidRoom(adj) && !adj.Equals(next) select adj).ToList();

      // go through the adjacent rooms and see which one would be better to go to
      foreach (Index adjacentRoom in adjacentRooms)
      {
        // if it's a good idea to move to this room
        if (IsGoodIdeaToMoveToRoom(adjacentRoom) && !Visited.Contains(adjacentRoom))
        {
          Debug.WriteLine("Moving to adjacent room because it's a good idea to.");

          // awesome! what would it take to move to this room?
          return MoveAgentToRoom(actionQuery.Agent, adjacentRoom);
        }
      }

      // room to visit
      Index roomToVisit = next;

      // if we get here, see if it's a good idea to move to any of the adjacent rooms even if we've visited them
      adjacentRooms = (from adj in GetAdjacentRooms(room) where IsValidRoom(adj) select adj).Reverse().ToList();
      foreach (Index adjacentRoom in adjacentRooms)
      {
        if (IsGoodIdeaToMoveToRoom(adjacentRoom) && !IsCyclic(adjacentRoom))
        {
          Debug.WriteLine("Moving backwards through adjacent rooms because it's a good idea to.");
          return MoveAgentToRoom(actionQuery.Agent, adjacentRoom);
        }
      }

      // this means that none of the adjacent rooms are the best to visit.
      Debug.WriteLine("Moving to a random room because we're unsure what to do.");
      roomToVisit = adjacentRooms[random.Next(adjacentRooms.Count)];
      while (CurrentKnowledge.Any(k => k.Room.Equals(roomToVisit) && (k.Type == KnowledgeType.IsPit || k.Type == KnowledgeType.IsWumpus)))
      {
        roomToVisit = adjacentRooms[random.Next(adjacentRooms.Count)];
      }

      // if we get here, it means we don't know what to do. We need to choose the first action
      return MoveAgentToRoom(actionQuery.Agent, roomToVisit);
    }
  }
}
