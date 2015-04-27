using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public class WumpusRule : KnowledgeRule
  {
    public override List<Knowledge> Relax(List<Knowledge> knowledge, Dictionary<Index, Percept> percepts)
    {
      // create a return value
      List<Knowledge> retval = new List<Knowledge>(knowledge);

      // create booleans to determine if we keep going
      bool foundWumpus = retval.Count(k => k.Type == KnowledgeType.IsWumpus) > 0;
      bool killedWumpus = percepts.Count(kvp => (kvp.Value & Percept.Scream) == Percept.Scream) > 0;

      // if we already know where the wumpus is OR we've killed the wumpus, then we don't need to do this
      if (!foundWumpus && !killedWumpus)
      {
        // create a variable to store the location of the wumpus
        Index? wumpus = null;

        // there is only one wumpus. In order for a wumpus to be in a room, there has to be at least two
        // squares with a stench in them. The first objective is to get all the rooms which have a stench in them
        var rooms = (from kvp in percepts where (kvp.Value & Percept.Stench) == Percept.Stench select kvp.Key).ToList();

        // if there are 3 or more rooms with a stench in them, then we know where the wumpus is
        if (rooms.Count >= 3)
        {
          // retrieve the common room for the rooms
          var commonRooms = GetCommonRooms(rooms);

          // let's just make sure that there is only ONE common room
          if (commonRooms.Count() == 1)
          {
            // now that we have the common room, we can say that this room IS the wumpus.
            wumpus = commonRooms.First();
          }
        }
        else if (rooms.Count == 2)
        {
          // if there are two rooms with a stench, then there are two possibilities. One is that one room is
          // above the other, or next to the other (meaning only the x or only the y changes). The other is
          // that the the rooms are diagonal from each other.
          Index room1 = rooms[0];
          Index room2 = rooms[1];

          int dc = Math.Abs(room1.Column - room2.Column);
          int dr = Math.Abs(room1.Row - room2.Row);

          if ((dc > 0 && dr == 0) || (dc == 0 && dr > 0))
          {
            // only one of the values has changed, so, just get the common room
            var commonRooms = GetCommonRooms(rooms);

            // let's just make sure that there is only ONE common room
            if (commonRooms.Count() == 1)
            {
              wumpus = commonRooms.First();
            }
          }
          else if (dc == 1 && dr == 1)
          {
            // get the other two adjacent rooms
            Index adj1 = new Index(room1.Column, room2.Row);
            Index adj2 = new Index(room2.Column, room1.Row);

            // as long as one of these rooms is known NOT to be a wumpus, then we know where the wumpus is
            if (retval.Count(k => k.Room.Equals(adj1) && k.Type == KnowledgeType.NoWumpus) > 0)
            {
              // this means that adj1 is known not to be a wumpus. So, adj2 must be the wumpus
              wumpus = adj2;
            }
            else if (retval.Count(k => k.Room.Equals(adj2) && k.Type == KnowledgeType.NoWumpus) > 0)
            {
              // this means that adj2 is known not to be a wumpus. So, adj1 must be the wumpus
              wumpus = adj1;
            }
          }
        }
        else if (rooms.Count == 1)
        {
          // if there is only ONE cell that contains a stench, we might still be able to find out where the wumpus
          // is. If there is only one square, but we have knowledge about the other squares and they check out as okay,
          // then the remaining adjacent square must be the wumpus
          Index room = rooms[0];
          var adjacentRooms = KnowledgeBase.GetAdjacentRooms(room).ToList();
          adjacentRooms.RemoveAll(adj =>
            {
              bool invalid = !KnowledgeBase.IsValidRoom(adj);
              bool isOK = retval.Count(k => k.Room.Equals(adj) && k.Type == KnowledgeType.NoWumpus) > 0;
              return invalid || isOK;
            });
          if (adjacentRooms.Count == 1)
          {
            wumpus = adjacentRooms[0];
          }
        }

        // if we found the wumpus
        if (wumpus.HasValue)
        {
          // we need to remove all knowledge about the wumpus
          retval.RemoveAll(k => IsWumpusType(k.Type));

          // next, we need to say that NONE of the squares are a wumpus
          retval.AddRange(from room in KnowledgeBase.Rooms
                          where !room.Equals(wumpus.Value)
                          select new Knowledge { Room = room, Type = KnowledgeType.NoWumpus });

          // finally, we say that this room IS the wumpus
          retval.Add(new Knowledge { Room = wumpus.Value, Type = KnowledgeType.IsWumpus });
          retval.Add(new Knowledge { Room = wumpus.Value, Type = KnowledgeType.NoPit });
        }
        else
        {
          // this means we didn't find the wumpus. We might still be able to relax the knowledge though. If
          // any of the rooms say that they might be a wumpus, but we determined that there is no wumpus in
          // the room, then we can make the room safe
          var noWumpusRooms = from k in retval where k.Type == KnowledgeType.NoWumpus select k.Room;
          retval.RemoveAll(k => noWumpusRooms.Contains(k.Room) && k.Type == KnowledgeType.MightBeWumpus);
        }
      }

      // return the retval
      return retval;
    }

    private IEnumerable<Index> GetCommonRooms(List<Index> rooms)
    {
      // the wumpus will be in the square that all the rooms are adjacent too. Let's get all of the rooms adjacent
      // to each of the rooms, then we'll say that the room that is common between them will be the room we want
      List<IEnumerable<Index>> adjacentRoomsForEachRoom = new List<IEnumerable<Index>>();
      foreach (var room in rooms)
      {
        var adjacent = from adj in KnowledgeBase.GetAdjacentRooms(room) where KnowledgeBase.IsValidRoom(adj) select adj;
        adjacentRoomsForEachRoom.Add(adjacent);
      }

      // create a variable to store the common room
      int i = 0;
      var commonRooms = adjacentRoomsForEachRoom[i++];
      for (; i < adjacentRoomsForEachRoom.Count; ++i)
      {
        commonRooms = commonRooms.Intersect(adjacentRoomsForEachRoom[i]);
      }

      // return the common rooms
      return commonRooms;
    }

    private bool IsWumpusType(KnowledgeType knowledgeType)
    {
      return
        knowledgeType == KnowledgeType.IsWumpus ||
        knowledgeType == KnowledgeType.MightBeWumpus ||
        knowledgeType == KnowledgeType.NoWumpus;
    }
  }
}
