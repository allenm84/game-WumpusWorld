using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public enum Percept
  {
    none = 0,
    stench = 1 << 0,
    breeze = 1 << 1,
    glitter = 1 << 2,
    bump = 1 << 3,
  }
}
