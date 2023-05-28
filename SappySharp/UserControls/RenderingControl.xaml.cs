using System.Windows.Controls;
using System.Windows.Media;

namespace SappySharp.UserControls;

public partial class RenderingControl : ItemsControl
{
    public delegate void DrawingEventHandler(object sender, DrawingContext e);
    public event DrawingEventHandler Rendering;

    public RenderingControl()
    {
        InitializeComponent();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Rendering?.Invoke(this, drawingContext);
        base.OnRender(drawingContext);
    }
}
