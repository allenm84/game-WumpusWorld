using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public interface IAgentWorld
  {
    bool ClimbOut(Index Index);
    bool GrabGold(Index Index);
    void FireArrow(Index Index, Index dir);
  }
}
