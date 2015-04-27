using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusWorld
{
  public static class GraphicsExtensions
  {
    public static void DrawImage(this Graphics graphics, Image image, RectangleF dest, ImageAttributes attributes)
    {
      var destPts = new PointF[3]
      { 
        new PointF(dest.Left, dest.Top),
        new PointF(dest.Right, dest.Top),
        new PointF(dest.Left, dest.Bottom)
      };

      graphics.DrawImage(image, destPts, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel, attributes);
    }

    public static void DrawRectangle(this Graphics graphics, Pen pen, RectangleF rect)
    {
      graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
    }
  }
}
