using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SappySharp.UserControls;

public partial class RenderingControl : UserControl
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
