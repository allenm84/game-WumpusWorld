using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  [Flags]
  public enum Percept
  {
    None = 0,
    Stench = 1,
    Breeze = 2,
    Glitter = 4,
    Bump = 8,
    Scream = 16,
  };
}
