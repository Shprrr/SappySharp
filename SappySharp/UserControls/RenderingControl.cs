using System.Windows.Controls;
using System.Windows.Media;

namespace SappySharp.UserControls;

public class RenderingControl : ItemsControl
{
    public delegate void DrawingEventHandler(object sender, DrawingContext e);
    public event DrawingEventHandler Rendering;

    protected override void OnRender(DrawingContext drawingContext)
    {
        Rendering?.Invoke(this, drawingContext);
        base.OnRender(drawingContext);
    }
}
