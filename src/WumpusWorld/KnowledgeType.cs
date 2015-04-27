using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public enum KnowledgeType
  {
    Unknown = 0,
    MightBeWumpus,
    NoWumpus,
    IsWumpus,
    MightBePit,
    NoPit,
    IsPit,
    Wall,
    Gold,
  }
}
