using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public class Cave : IAgentWorld
  {
    private Random random = new Random();
    private CaveCell[,] cells;
    private Agent agent;
    private List<Arrow> arrows = new List<Arrow>();
    private bool pendingScream = false;

    public bool AgentEscaped { get { return agent.Escaped; } }
    public bool AgentAlive { get { return agent.Alive; } }
    public Index AgentPosition { get { return agent.Index; } }
    public Orientation AgentOrientation { get { return agent.Orientation; } }

    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public CaveCell this[Index index]
    {
      get { return cells[index.Row, index.Column]; }
    }

    public CaveCell this[int r, int c]
    {
      get { return cells[r, c]; }
    }

    public IEnumerable<Index> Arrows
    {
      get { return arrows.Select(a => a.Position); }
    }

    public Cave(int size)
      : this(size, size)
    {

    }

    public Cave(int rows, int columns)
    {
      Rows = rows;
      Columns = columns;
      agent = new Agent(this);
      Reset();
    }

    public void Reset()
    {
      agent.Reset();
      agent.Index = new Index(0, 0);
      InitializeGrid();
    }

    private void InitializeGrid()
    {
      cells = new CaveCell[Rows, Columns];
      int r, c;
      for (r = 0; r < Rows; ++r)
      {
        for (c = 0; c < Columns; ++c)
        {
          cells[r, c] = new CaveCell(r, c);
        }
      }

      double total = Rows * Columns;
      int maxPits = (int)Math.Floor(total * 0.06);
      int pits = random.Next(-maxPits / 2, maxPits + 1);
      for (; pits > 0; --pits)
      {
        RandomCell(out c, out r);
        cells[r, c].Type = CaveCellType.Pit;
        PerformActionOnNeighbors(r, c, (n) => n.ContainsBreeze = true);
      }

      do
      {
        RandomCell(out c, out r);
      }
      while (cells[r, c].Type != CaveCellType.None);
      cells[r, c].Type = CaveCellType.Gold;

      do
      {
        RandomCell(out c, out r);
      }
      while (cells[r, c].Type != CaveCellType.None);
      cells[r, c].Type = CaveCellType.Wumpus;
      PerformActionOnNeighbors(r, c, (n) => n.ContainsStench = true);
    }

    private void RandomCell(out int c, out int r)
    {
      r = random.Next(1, Rows - 1);
      c = random.Next(1, Columns - 1);
    }

    private IEnumerable<Index> GetNeighbors(int r, int c)
    {
      yield return new Index(c + 1, r);
      yield return new Index(c - 1, r);
      yield return new Index(c, r + 1);
      yield return new Index(c, r - 1);
    }

    private void PerformActionOnNeighbors(int r, int c, Action<CaveCell> action)
    {
      foreach (var index in GetNeighbors(r, c))
      {
        if (ValidIndex(index))
        {
          var cell = this[index];
          action(cell);
        }
      }
    }

    private bool ValidIndex(Index index)
    {
      return index.IsValid(Rows, Columns);
    }

    public bool IsAgent(int c, int r)
    {
      return (c == agent.Column) && (r == agent.Row);
    }

    public void Update()
    {
      if (!UpdateArrows())
      {
        var percept = Percept.None;
        if (ValidIndex(agent.Index))
        {
          var cell = this[agent.Index];
          cell.Revealed = true;

          if (cell.Type == CaveCellType.Gold)
          {
            percept = Percept.Glitter;
          }
          else
          {
            if (cell.ContainsBreeze)
            {
              percept |= Percept.Breeze;
            }
            if (cell.ContainsStench)
            {
              percept |= Percept.Stench;
            }
          }
        }
        else
        {
          percept = Percept.Bump;
        }

        if (pendingScream)
        {
          pendingScream = false;
          percept |= Percept.Scream;
        }

        agent.Perceive(percept);
      }
    }

    private bool UpdateArrows()
    {
      arrows.RemoveAll(a => !a.Alive);
      if (arrows.Count > 0)
      {
        // update the arrows
        foreach (var a in arrows)
        {
          Index i = a.Position;

          if (ValidIndex(i))
          {
            var cell = this[i];
            if (cell.Type == CaveCellType.Wumpus)
            {
              pendingScream = true;
              cell.Type = CaveCellType.DeadWumpus;
              a.Alive = false;
            }
            else
            {
              i.Row += a.Direction.Row;
              i.Column += a.Direction.Column;
            }

            a.Steps++;
            a.Position = i;
            if (a.Steps > 5)
            {
              a.Alive = false;
            }
          }
          else
          {
            a.Alive = false;
          }
        }

        // don't update anything else
        return true;
      }
      return false;
    }

    bool IAgentWorld.ClimbOut(Index Index)
    {
      return (Index.Row == 0) && (Index.Column == 0);
    }

    bool IAgentWorld.GrabGold(Index Index)
    {
      if (ValidIndex(Index))
      {
        var cell = this[Index];
        if (cell.Type == CaveCellType.Gold)
        {
          cell.Type = CaveCellType.None;
          return true;
        }
      }
      return false;
    }

    void IAgentWorld.FireArrow(Index Index, Index dir)
    {
      arrows.Add(new Arrow
      {
        Alive = true,
        Direction = dir,
        Position = Index,
        Steps = 0,
      });
    }
  }
}
