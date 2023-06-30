using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;

namespace SappySharp.UserControls;

public partial class VolumeSlider : UserControl, INotifyPropertyChanged
{
    public VolumeSlider()
    {
        Background = Brushes.White;
        Cursor = Cursors.Hand;
        InitializeComponent();
        PreviewKeyDown += UserControl_KeyDown;
        PreviewMouseDown += UserControl_MouseDown;
        PreviewMouseMove += UserControl_MouseMove;
        PreviewMouseUp += UserControl_MouseUp;
        SizeChanged += UserControl_Resize;
    }

    private int _value;
    private bool _dragging;

    public delegate void ChangeEventHandler(int newValue);
    public event ChangeEventHandler Change;
    public event PropertyChangedEventHandler PropertyChanged;

    public int Value
    {
        get => _value;
        set
        {
            if (value < 0) value = 0;
            if (value > 50) value = 50;
            _value = value;
            Change?.Invoke(_value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)
        {
            if (Value == 0) return;
            Value--;
            Change?.Invoke(Value);
            e.Handled = true;
        }
        if (e.Key == Key.Right)
        {
            if (Value == 50) return;
            Value++;
            Change?.Invoke(Value);
            e.Handled = true;
        }
        if (e.Key == Key.Up)
        {
            Value += 10;
            if (Value > 50) Value = 50;
            Change?.Invoke(Value);
            e.Handled = true;
        }
        if (e.Key == Key.Down)
        {
            Value -= 10;
            if (Value < 0) Value = 0;
            Change?.Invoke(Value);
            e.Handled = true;
        }
    }

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => VBExtension.CallMouseButton(e, this, UserControl_MouseDown);
    private void UserControl_MouseDown(int Button, int Shift, double x, double y)
    {
        mTrace.Trace("VolSlider_MouseDown -> X: " + x + ", mValue: " + Value);
        if (x < Value || x > Value) Value = (int)(x - 5);
        _dragging = true;
        UserControl_MouseMove(Button, Shift, x, y);
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e) => VBExtension.CallMouseMove(e, this, UserControl_MouseMove);
    private void UserControl_MouseMove(int Button, int Shift, double x, double y)
    {
        ForceCursor = x >= Value && x <= Value + 10;

        if (!_dragging) return;
        ForceCursor = true;
        Value = (int)(x - 5);
        if (Value > 50) Value = 50;
        if (Value < 0) Value = 0;
        Change?.Invoke(Value);
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) => VBExtension.CallMouseButton(e, this, UserControl_MouseUp);
    private void UserControl_MouseUp(int Button, int Shift, double x, double y)
    {
        _dragging = false;
    }

    private void UserControl_Resize(object sender, SizeChangedEventArgs e)
    {
        Width = 975 / VBExtension.Screen.TwipsPerPixelX;
        Height = 375 / VBExtension.Screen.TwipsPerPixelY;
    }

    /// <summary>
    /// Returns/sets the background color used to display text and graphics in an object.
    /// </summary>
    public int BackColor
    {
        get
        {
            Color c = ((SolidColorBrush)Background).Color;
            return Information.RGB(c.R, c.G, c.B);
        }
        set
        {
            int r = 0, g = 0, b = 0;
            mColorUtils.SplitRGB(value, ref r, ref g, ref b);
            Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }
    }
}
