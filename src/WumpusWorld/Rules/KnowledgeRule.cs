using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public abstract class KnowledgeRule
  {
    public abstract List<Knowledge> Relax(List<Knowledge> knowledge, Dictionary<Index, Percept> percepts);
  }
}
