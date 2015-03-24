using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public enum Percept
  {
    /// <summary>
    /// There is no spoon
    /// </summary>
    None = 0,

    /// <summary>
    /// Stench is perceived at a square iff the wumpus is at this square or in its neighborhood.
    /// </summary>
    Stench = 1 << 0,

    /// <summary>
    /// Breeze is perceived at a square iff a pit is in the neighborhood of this square.
    /// </summary>
    Breeze = 1 << 1,

    /// <summary>
    /// Glitter is perceived at a square iff gold is in this square
    /// </summary>
    Glitter = 1 << 2,

    /// <summary>
    /// Bump is perceived at a square iff the agent goes Forward into a wall
    /// </summary>
    Bump = 1 << 3,

    /// <summary>
    /// Scream is perceived at a square iff the wumpus is killed anywhere in the cave
    /// </summary>
    Scream = 1 << 4,
  }
}
