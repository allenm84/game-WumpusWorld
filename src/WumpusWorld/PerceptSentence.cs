using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public class PerceptSentence
  {
    public Agent Agent { get; set; }
    public Percept Percept { get; set; }
    public int Time { get; set; }
  }
}
