using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WumpusWorld.WinForms
{
  public partial class MainForm : Form
  {
    const int Count = 4;

    private GridCell[,] cells;
    private Agent agent;
    private StringFormat center;
    private Font font;

    public MainForm()
    {
      InitializeComponent();
      ClientSize = new Size(400, 300);
      agent = new Agent();
      center = new StringFormat
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
      };
      font = new System.Drawing.Font("Tahoma", 18f, FontStyle.Bold);
      InitializeGrid();
    }

    private void InitializeGrid()
    {
      var random = new Random();
      cells = new GridCell[Count, Count];

      int r, c;
      for (r = 0; r < Count; ++r)
      {
        for (c = 0; c < Count; ++c)
        {
          var cell = new GridCell(r, c);
          cells[r, c] = cell;
        }
      }

      int maxPits = (int)Math.Floor(((double)Count * Count) * 0.1);
      int pits = random.Next(-maxPits / 2, maxPits + 1);
      for (; pits > 0; --pits)
      {
        r = random.Next(1, Count);
        c = random.Next(1, Count);
        cells[r, c].Pit = true;
      }

      do
      {
        r = random.Next(1, Count);
        c = random.Next(1, Count);
      }
      while (cells[r, c].Pit);
      cells[r, c].Gold = true;

      do
      {
        r = random.Next(1, Count);
        c = random.Next(1, Count);
      }
      while (cells[r, c].Pit || cells[r, c].Gold);
      cells[r, c].Wumpus = true;
    }

    private bool IsValidIndex(int r, int c)
    {
      return (-1 < r && r < Count) && (-1 < c && c < Count);
    }

    private IEnumerable<GridCell> GetNeighbors(GridCell cell)
    {
      int r, c;
      r = cell.Row;
      c = cell.Column;

      var cands = new[]
      {
        new { r = r + 1, c = c },
        new { r = r - 1, c = c },
        new { r = r, c = c + 1 },
        new { r = r, c = c - 1 },
      };

      return cands
        .Where(b => IsValidIndex(b.r, b.c))
        .Select(b => cells[b.r, b.c]);
    }

    private void RenderScene(Graphics g)
    {
      float width = pnlScreen.ClientSize.Width - 1;
      float height = pnlScreen.ClientSize.Height - 1;

      float cellWidth = width / Count;
      float cellHeight = height / Count;

      int r, c;
      float x, y;

      y = 0;
      for (r = 0; r < Count; ++r)
      {
        x = 0;
        for (c = 0; c < Count; ++c)
        {
          var rect = new RectangleF(x, y, cellWidth, cellHeight);
          g.FillRectangle(Brushes.WhiteSmoke, rect);
          g.DrawRectangle(Pens.Black, rect);

          var cell = cells[r, c];
          if (cell.Gold)
          {
            g.DrawString("G", font, Brushes.Gold, rect, center);
          }
          if (cell.Pit)
          {
            g.DrawString("P", font, Brushes.Black, rect, center);
          }
          if (cell.Wumpus)
          {
            g.DrawString("W", font, Brushes.Red, rect, center);
          }

          if (c == agent.X && r == agent.Y)
          {
            PointF p1 = new PointF(x + cellWidth * 0.5f, y + cellHeight * 0.5f);
            PointF p2 = p1;
            switch (agent.Orientation)
            {
              case Orientation.down: p2.Y = (y + cellHeight * 0.85f); break;
              case Orientation.left: p2.X = x * 1.85f; break;
              case Orientation.right: p2.X = (x + cellWidth * 0.85f); break;
              case Orientation.up: p2.Y = y * 1.85f; break;
            }

            using (var pen = new Pen(Color.SlateBlue, 8f))
            {
              pen.SetLineCap(LineCap.Round, LineCap.ArrowAnchor, DashCap.Flat);
              g.DrawLine(pen, p1, p2);
            }
          }

          x += cellWidth;
        }
        y += cellHeight;
      }
    }

    private void pnlScreen_Paint(object sender, PaintEventArgs e)
    {
      RenderScene(e.Graphics);
    }

    private void timerUpdate_Tick(object sender, EventArgs e)
    {
      pnlScreen.Invalidate();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int r, c;
      r = agent.Y;
      c = agent.X;

      if (IsValidIndex(r, c))
      {
        Percept p = Percept.none;
        var cell = cells[r, c];
        if (cell.Gold)
          p |= Percept.glitter;

        var neighbors = GetNeighbors(cell);
        if (neighbors.Any(n => n.Pit))
          p |= Percept.breeze;
        if (neighbors.Any(n => n.Wumpus))
          p |= Percept.stench;

        var act = agent.update(p);
        act();
      }
      else
      {
        agent.update(Percept.bump);
      }

      pnlScreen.Invalidate();
    }
  }
}
