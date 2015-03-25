using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class Agent
  {
    static Orientation[] values = new Orientation[]
    {
      Orientation.up,
      Orientation.right,
      Orientation.down,
      Orientation.left,
    };

    private Pos pos = new Pos(0, 0);
    private Orientation orient = Orientation.right;
    private Stack<Action> path = new Stack<Action>();
    private Dictionary<Pos, Node> nodes = new Dictionary<Pos, Node>();
    private object inventory;

    public int X { get { return pos.x; } }
    public int Y { get { return pos.y; } }
    public Orientation Orientation { get { return orient; } }

    private Action act_turn(Orientation d)
    {
      return () => orient = d;
    }

    private Action act_forward()
    {
      return () => pos = move(orient, pos);
    }

    private Action act_grab()
    {
      return () =>
      {
        if (inventory == null)
        {
          inventory = new object();
        }
      };
    }

    private Pos move(Orientation o, Pos p)
    {
      switch (o)
      {
        case Orientation.down: p.y++; break;
        case Orientation.left: p.x--; break;
        case Orientation.right: p.x++; break;
        case Orientation.up: p.y--; break;
      }

      return p;
    }

    private IEnumerable<Pos> neighbors()
    {
      return neighbors(pos);
    }

    private IEnumerable<Pos> neighbors(Node n)
    {
      return neighbors(new Pos(n.x, n.y));
    }

    private IEnumerable<Pos> neighbors(Pos p)
    {
      int x = p.x, y = p.y;
      yield return new Pos(x + 1, y);
      yield return new Pos(x - 1, y);
      yield return new Pos(x, y + 1);
      yield return new Pos(x, y - 1);
    }

    private Node node()
    {
      return node(pos);
    }

    private Node node(Pos p)
    {
      Node n;
      if (!nodes.TryGetValue(p, out n))
      {
        n = new Node(p);
        nodes[p] = n;
      }
      return n;
    }

    private Orientation rotate(Orientation o)
    {
      int index = values.IndexOf(o);
      return values[(index + 1) % 4];
    }

    private void tell(Percept percept)
    {
      var n = node();
      n.percept = percept;
    }

    private void relax()
    {
      foreach (var n in nodes.Values)
      {
        reduce(n, Percept.stench, (x, y) => x.wumpusPc = y);
        reduce(n, Percept.breeze, (x, y) => x.pitPc = y);
      }
    }

    private void reduce(Node n, Percept percept, Action<Node, double> setPc)
    {
      if (n.percept.HasFlag(percept))
      {
        setPc(n, 0);

        // now, we check the neighbors
        foreach (var c in neighbors(n))
        {
          if (isUnknown(c))
          {
            continue;
          }
          var w = node(c);

          double count = 0;
          double target = 0;

          foreach (var d in neighbors(c))
          {
            if (isUnknown(d))
            {
              continue;
            }
            ++count;

            var t = node(d);
            if (t.percept.HasFlag(percept))
            {
              ++target;
            }
          }

          setPc(w, count / target);
        }
      }
    }

    private Stack<Action> turnaround()
    {
      // turn to the next orientation, then move forward
      var stack = new Stack<Action>();
      stack.Push(act_forward());
      stack.Push(act_turn(rotate(orient)));
      return stack;
    }

    private bool ask(out Pos target)
    {
      target = new Pos(-1, -1);
      foreach (var ij in neighbors())
      {
        var n = node(ij);

        bool question1 = !definitePit(n) && !definiteWumpus(n);
        bool question2 = maybePit(n) || maybeWumpus(n);

        if (question1 == true || question2 == false)
        {
          target = ij;
          return true;
        }
      }
      return false;
    }

    private bool maybeWumpus(Node n)
    {
      return n.wumpusPc > 0.25;
    }

    private bool maybePit(Node n)
    {
      return n.pitPc > 0.25;
    }

    private bool definiteWumpus(Node n)
    {
      return n.wumpusPc >= 1.0;
    }

    private bool definitePit(Node n)
    {
      return n.pitPc >= 1.0;
    }

    private void reset()
    {
      foreach (var kvp in nodes)
      {
        var n = kvp.Value;
        n.f_score = 0;
        n.g_score = 0;
      }
    }

    private float cost(Node x, Node y)
    {
      return Math.Abs((x.x - y.x) + (x.y - y.y));
    }

    private Node popMin(List<Node> lst)
    {
      var n = lst[0];
      int index = 0;

      for (int i = 1; i < lst.Count; ++i)
      {
        var v = lst[i];
        if (v.f_score < n.f_score)
        {
          n = v;
          index = i;
        }
      }

      lst.RemoveAt(index);
      return n;
    }

    private bool facing(Node src, Node dst, Orientation dir)
    {
      var other = direction(src, dst);
      if (other == null) return false;
      return other.Value == dir;
    }

    private Orientation? direction(Node src, Node dst)
    {
      int dx = Math.Sign(src.x - dst.x);
      int dy = Math.Sign(src.y - dst.y);

      if (dx == -1 && dy == 0)
        return Orientation.right;
      if (dx == 1 && dy == 0)
        return Orientation.left;

      if (dx == 0 && dy == -1)
        return Orientation.down;
      if (dx == 0 && dy == 1)
        return Orientation.up;

      return null;
    }

    private Stack<Action> reconstruct(Node current, Dictionary<Node, Node> cameFrom)
    {
      var path = new List<Node> { current };
      while (cameFrom.ContainsKey(current))
      {
        current = cameFrom[current];
        path.Insert(0, current);
      }

      var actions = new List<Action>();

      var src = path[0];
      var dir = orient;

      for (int i = 1; i < path.Count; ++i)
      {
        var dst = path[i];
        while (!facing(src, dst, dir))
        {
          dir = rotate(dir);
          actions.Add(act_turn(dir));
        }
        actions.Add(act_forward());
      }

      actions.Reverse();
      return new Stack<Action>(actions);
    }

    private Action face(Node src, Node dst, ref Orientation dir)
    {
      var d = direction(src, dst).Value;
      dir = d;
      return act_turn(d);
    }

    private bool isUnknown(Pos n)
    {
      return !nodes.ContainsKey(n);
    }

    private bool isWall(Node n)
    {
      return n.percept.HasFlag(Percept.bump);
    }

    private bool isDangerous(Node n)
    {
      return maybePit(n)
        || maybeWumpus(n)
        || definitePit(n)
        || definiteWumpus(n);
    }

    private Stack<Action> plan(Pos g)
    {
      reset();

      var start = node(pos);
      var goal = node(g);
      
      var closedset = new HashSet<Node>();
      var openset = new List<Node> { start };
      var cameFrom = new Dictionary<Node, Node>();

      start.g_score = 0;
      start.f_score = start.g_score + cost(start, goal);

      while (openset.Count > 0)
      {
        var current = popMin(openset);
        if (current == goal)
        {
          return reconstruct(goal, cameFrom);
        }

        closedset.Add(current);
        foreach (var n in neighbors(current))
        {
          if (isUnknown(n))
          {
            continue;
          }

          var neighbor = node(n);
          if (isWall(neighbor) || closedset.Contains(neighbor) || isDangerous(neighbor))
          {
            continue;
          }

          float score = current.g_score + cost(current, neighbor);
          if (!openset.Contains(neighbor) || score < neighbor.g_score)
          {
            cameFrom[neighbor] = current;
            neighbor.g_score = score;
            neighbor.f_score = neighbor.g_score + cost(neighbor, goal);
            if (!openset.Contains(neighbor))
            {
              openset.Add(neighbor);
            }
          }
        }
      }

      return null;
    }

    private Action random()
    {
      return act_forward();
    }

    public Action update(Percept percept)
    {
      Action action = null;
      tell(percept);
      relax();

      Pos p;

      if (percept.HasFlag(Percept.glitter))
      {
        action = act_grab();
      }
      else if (percept.HasFlag(Percept.bump))
      {
        path = turnaround();
      }
      else if (path.Count > 0)
      {
        action = path.Pop();
      }
      else if (ask(out p))
      {
        path = plan(p);
        action = path.Pop();
      }
      else
      {
        action = random();
      }

      return action;
    }
  }
}
