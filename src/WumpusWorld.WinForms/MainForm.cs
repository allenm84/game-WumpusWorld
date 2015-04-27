using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WumpusWorld.WinForms.Properties;

namespace WumpusWorld.WinForms
{
  public partial class MainForm : Form
  {
    const int Count = 16;

    private StringFormat center;
    private Font font;
    private Cave cave;

    private Image agentImage;
    private Image stenchImage;
    private Image breezeImage;

    private Dictionary<Orientation, Rectangle> agentSourceRects;
    private Dictionary<CaveCellType, Image> cellImages;
    private Index lastValidLocation;
    private bool forceReveal = false;
    private bool isRunning = false;
    private bool resetNeeded = false;

    public MainForm()
    {
      InitializeComponent();
      ClientSize = new Size(400, 300);

      cellImages = new Dictionary<CaveCellType, Image>();
      cellImages[CaveCellType.DeadWumpus] = Resources.DeadWumpus;
      cellImages[CaveCellType.Gold] = Resources.Gold;
      cellImages[CaveCellType.Pit] = Resources.Pit;
      cellImages[CaveCellType.Wumpus] = Resources.Wumpus;

      agentImage = Resources.Agent;
      stenchImage = Resources.Stench;
      breezeImage = Resources.Breeze;

      Size size = agentImage.Size;
      int width = size.Width / 4;

      agentSourceRects = new Dictionary<Orientation, Rectangle>();
      agentSourceRects[Orientation.Down] = new Rectangle(width * 0, 0, width, size.Height);
      agentSourceRects[Orientation.Left] = new Rectangle(width * 1, 0, width, size.Height);
      agentSourceRects[Orientation.Up] = new Rectangle(width * 2, 0, width, size.Height);
      agentSourceRects[Orientation.Right] = new Rectangle(width * 3, 0, width, size.Height);

      center = new StringFormat
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
      };
      font = new System.Drawing.Font("Tahoma", 18f, FontStyle.Bold);

      cave = new Cave(Count);
    }

    private void RenderScene(Graphics g)
    {
      forceReveal = Keyboard.IsKeyDown(Keys.Space);

      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      RectangleF screen = pnlScreen.ClientRectangle;
      screen.Width--;
      screen.Height--;

      RectangleF rect = new RectangleF(0, 0, screen.Width / Count, screen.Height / Count);
      var arrows = new HashSet<Index>(cave.Arrows);

      Index agent = cave.AgentPosition;
      if (agent.IsValid(cave.Rows, cave.Columns))
      {
        lastValidLocation = agent;
      }

      Color tint = Color.WhiteSmoke;
      float intensity = 0.25f;
      ImageAttributes attributes = new ImageAttributes();
      ColorMatrix m = new ColorMatrix(new float[][]
      {
        new float[] {1, 0, 0, 0, 0},
        new float[] {0, 1, 0, 0, 0},
        new float[] {0, 0, 1, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {tint.R / 255f * intensity, tint.G / 255f * intensity, tint.B / 255f * intensity, 0, 1}
      });
      attributes.SetColorMatrix(m);

      int r, c;
      for (r = 0; r < Count; ++r)
      {
        rect.X = 0;
        for (c = 0; c < Count; ++c)
        {
          var cell = cave[r, c];

          var inflated = rect;
          inflated.Inflate(-2, -2);

          Color fill = cell.Revealed ? Color.SkyBlue : Color.LightGray;
          FillSquare(g, fill, rect);

          Image image;
          if (cellImages.TryGetValue(cell.Type, out image))
          {
            if(cell.Revealed)
            {
              g.DrawImage(image, rect);
            }
            else if (forceReveal)
            {
              g.DrawImage(image, rect, attributes);
            }
          }

          if (cell.ContainsStench)
          {
            if (cell.Revealed)
            {
              g.DrawImage(stenchImage, inflated);
            }
            else if (forceReveal)
            {
              g.DrawImage(stenchImage, inflated, attributes);
            }
          }

          if (cell.ContainsBreeze)
          {
            if (cell.Revealed)
            {
              g.DrawImage(breezeImage, inflated);
            }
            else if (forceReveal)
            {
              g.DrawImage(breezeImage, inflated, attributes);
            }
          }

          if (lastValidLocation.Equals(r, c))
          {
            g.DrawImage(agentImage, inflated, agentSourceRects[cave.AgentOrientation], GraphicsUnit.Pixel);
          }

          Index index = new Index(c, r);
          if (arrows.Contains(index))
          {
            g.DrawString(">", font, Brushes.Goldenrod, rect, center);
          }

          rect.X += rect.Width;
        }
        rect.Y += rect.Height;
      }
    }

    private void FillSquare(Graphics g, Color color, RectangleF rect)
    {
      using (var b = new SolidBrush(color))
      {
        g.FillRectangle(b, rect);
      }
      using (var p = new Pen(Color.Black, 2f))
      {
        g.DrawRectangle(p, rect);
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Space)
      {
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Space)
      {
        e.SuppressKeyPress = true;
        e.Handled = true;
      }
      base.OnKeyDown(e);
    }

    private void pnlScreen_Paint(object sender, PaintEventArgs e)
    {
      RenderScene(e.Graphics);
    }

    private void timerUpdate_Tick(object sender, EventArgs e)
    {
      if (isRunning)
      {
        cave.Update();
        if (!cave.AgentAlive || cave.AgentEscaped)
        {
          isRunning = false;
          resetNeeded = true;
          btnUpdate.Enabled = true;
        }
      }
      pnlScreen.Invalidate();
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      if (resetNeeded)
      {
        cave.Reset();
      }

      isRunning = true;
      btnUpdate.Enabled = false;
    }
  }
}
