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
        KeyDown += UserControl_KeyDown;
        MouseDown += UserControl_MouseDown;
        MouseMove += UserControl_MouseMove;
        MouseUp += UserControl_MouseUp;
        SizeChanged += UserControl_Resize;
    }

    private int mValue;
    private bool Dragging;
    private int DragOrigin;

    public delegate void ChangeEventHandler(int newValue);
    public event ChangeEventHandler Change;
    public event PropertyChangedEventHandler PropertyChanged;

    public int Value
    {
        get => mValue;
        set
        {
            if (value < 0) value = 0;
            if (value > 50) value = 50;
            mValue = value;
            Change?.Invoke(mValue);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e) => UserControl_KeyDown((int)e.Key, 0);
    private void UserControl_KeyDown(int KeyCode, int Shift)
    {
        if (KeyCode == 37) //left
        {
            if (mValue == 0) return;
            mValue--;
            Change?.Invoke(mValue);
        }
        if (KeyCode == 39) //right
        {
            if (mValue == 50) return;
            mValue++;
            Change?.Invoke(mValue);
        }
        if (KeyCode == 38) //up
        {
            mValue += 10;
            if (mValue > 50) mValue = 50;
            Change?.Invoke(mValue);
        }
        if (KeyCode == 40) //down
        {
            mValue -= 10;
            if (mValue < 0) mValue = 0;
            Change?.Invoke(mValue);
        }
    }

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => VBExtension.CallMouseButton(e, this, UserControl_MouseDown);
    private void UserControl_MouseDown(int Button, int Shift, double x, double y)
    {
        mTrace.Trace("VolSlider_MouseDown -> X: " + x + ", mValue: " + mValue);
        if (x < mValue || x > mValue) mValue = (int)(x - 5);
        Dragging = true;
        DragOrigin = (int)(x - mValue);
        UserControl_MouseMove(Button, Shift, x, y);
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e) => VBExtension.CallMouseMove(e, this, UserControl_MouseMove);
    private void UserControl_MouseMove(int Button, int Shift, double x, double y)
    {
        ForceCursor = x >= mValue && x <= mValue + 10;

        if (!Dragging) return;
        ForceCursor = true;
        mValue = (int)(x - 5);
        if (mValue > 50) mValue = 50;
        if (mValue < 0) mValue = 0;
        Change?.Invoke(mValue);
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) => VBExtension.CallMouseButton(e, this, UserControl_MouseUp);
    private void UserControl_MouseUp(int Button, int Shift, double x, double y)
    {
        Dragging = false;
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
