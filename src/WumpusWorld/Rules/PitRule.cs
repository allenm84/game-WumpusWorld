using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public class PitRule : KnowledgeRule
  {
    public override List<Knowledge> Relax(List<Knowledge> knowledge, Dictionary<Index, Percept> percepts)
    {
      // create a return value
      List<Knowledge> retval = new List<Knowledge>(knowledge);

      // do a pre-clean of the knowledge
      var noPitRooms = (from k in retval where k.Type == KnowledgeType.NoPit select k.Room).ToList();
      retval.RemoveAll(k => noPitRooms.Contains(k.Room) && k.Type == KnowledgeType.MightBePit);

      // get all of the rooms which might be pits
      var mightBePits = (from kvp in retval where kvp.Type == KnowledgeType.MightBePit select kvp.Room).Distinct().ToList();
      foreach (var mightBePit in mightBePits)
      {
        // get all of the squares adjacent to this room (which might be a pit)
        var adjacent = from adj in KnowledgeBase.GetAdjacentRooms(mightBePit)
                       where KnowledgeBase.IsValidRoom(adj)
                       select adj;

        // do we have percepts for each of these rooms
        bool anyDontHavePercepts = adjacent.Any(adj => !percepts.ContainsKey(adj));
        if (!anyDontHavePercepts)
        {
          // this means that we have received a percept about all of the adjacent rooms. Are any
          // of the percepts NOT breezes
          bool anyNotBreezes = adjacent.Any(adj => (percepts[adj] & Percept.Breeze) != Percept.Breeze);
          if (anyNotBreezes)
          {
            // one or more of the squares adjacent to this "might be a pit" rooms isn't a breeze. That means
            // that the room isn't a pit!
            retval.Add(new Knowledge { Room = mightBePit, Type = KnowledgeType.NoPit });
          }
          else
          {
            // this means that all of the adjacent rooms are a breeze. Let's check the adjacent rooms of EACH
            // of the breezes
            foreach (var breeze in adjacent)
            {
              // we now have the valid rooms that are adjacent to the breeze for this "might be a pit" room
              var adjacentToBreeze = from adj in KnowledgeBase.GetAdjacentRooms(breeze)
                                     where KnowledgeBase.IsValidRoom(adj) && !adj.Equals(mightBePit)
                                     select adj;

              // let's make sure that we have percepts for each of these
              bool anyAdjacentDoesntHavePercepts = adjacentToBreeze.Any(adj => !percepts.ContainsKey(adj));
              if (anyAdjacentDoesntHavePercepts) continue;

              // if we get here, that means that each of these rooms adjacent to the breeze room has knowledge
              // about it. If we have knowledge about the adjacent rooms to the breeze, it means that none of them
              // are pits. WHICH means that this "might be a pit" room HAS to be a pit. We don't need to do anything
              // about it though since the agent will still avoid rooms that might be a pit.
              retval.Add(new Knowledge { Room = mightBePit, Type = KnowledgeType.IsPit });
              retval.Add(new Knowledge { Room = mightBePit, Type = KnowledgeType.NoWumpus });
            }
          }
        }
      }

      // cleanup the knowledge again
      noPitRooms = (from k in retval where k.Type == KnowledgeType.NoPit select k.Room).ToList();
      retval.RemoveAll(k => noPitRooms.Contains(k.Room) && k.Type == KnowledgeType.MightBePit);

      // return the knowledge
      return retval;
    }

    private bool IsPitType(KnowledgeType knowledgeType)
    {
      return
        knowledgeType == KnowledgeType.IsPit ||
        knowledgeType == KnowledgeType.MightBePit ||
        knowledgeType == KnowledgeType.NoPit;
    }
  }
}
