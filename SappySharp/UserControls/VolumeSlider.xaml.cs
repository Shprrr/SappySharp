using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;

namespace SappySharp.UserControls;

public partial class VolumeSlider : UserControl
{
    public VolumeSlider()
    {
        Background = Brushes.White;
        Cursor = Cursors.Hand;
        InitializeComponent();
        GotFocus += UserControl_GotFocus;
        KeyDown += UserControl_KeyDown;
        LostFocus += UserControl_LostFocus;
        MouseDown += UserControl_MouseDown;
        MouseMove += UserControl_MouseMove;
        MouseUp += UserControl_MouseUp;
        SizeChanged += UserControl_Resize;
    }

    [DllImport("user32.dll")]
    private static extern int DrawEdge(int hdc, RECT qrc, int edge, int grfFlags);
    [DllImport("user32.dll")]
    private static extern int FillRect(int hdc, RECT lpRect, int hBrush);
    [DllImport("user32.dll")]
    private static extern int DrawFocusRect(int hdc, RECT lpRect);

    [LibraryImport("gdi32.dll")]
    private static partial int CreateSolidBrush(int crColor);
    [LibraryImport("gdi32.dll")]
    private static partial int SelectObject(int hdc, int hObject);
    [LibraryImport("gdi32.dll")]
    private static partial int DeleteObject(int hObject);

    private class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }

    private const int BDR_RAISEDINNER = 0x4;
    private const int BDR_RAISEDOUTER = 0x1;
    private const int BDR_SUNKENINNER = 0x8;
    private const int BDR_SUNKENOUTER = 0x2;
    private const int BF_BOTTOM = 0x8;
    private const int BF_RIGHT = 0x4;
    private const int BF_DIAGONAL = 0x10;
    private const int BF_TOP = 0x2;
    private const int BF_LEFT = 0x1;
    private const int BF_TOPLEFT = BF_TOP + BF_LEFT;
    private const int BF_BOTTOMRIGHT = BF_BOTTOM + BF_RIGHT;
    private const int BF_DIAGONAL_ENDTOPRIGHT = BF_DIAGONAL + BF_TOP + BF_RIGHT;

    private const int COLOR_BTNFACE = 16;

    [LibraryImport("gdi32.dll")]
    private static partial int LineTo(int hdc, int x, int y);
    [DllImport("gdi32.dll")]
    private static extern int MoveToEx(int hdc, int x, int y, POINTAPI lpPoint);
    [LibraryImport("gdi32.dll")]
    private static partial int CreatePen(int nPenStyle, int nWidth, int crColor);
    [LibraryImport("gdi32.dll")]
    private static partial int FloodFill(int hdc, int x, int y, int crColor);
    private class POINTAPI
    {
        int x;
        int y;
    }

    private int mValue;
    private bool Dragging;
    private int DragOrigin;
    private bool HasFocus;

    public delegate void ChangeEventHandler(int newValue);
    public event ChangeEventHandler Change;

    public int Value
    {
        get => mValue;
        set
        {
            if (value < 0) value = 0;
            if (value > 50) value = 50;
            mValue = value;
            UserControl_Paint();
            Change?.Invoke(mValue);
        }
    }

    private void UserControl_GotFocus(object sender, RoutedEventArgs e)
    {
        HasFocus = true;
        UserControl_Paint();
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e) => UserControl_KeyDown((int)e.Key, 0);
    private void UserControl_KeyDown(int KeyCode, int Shift)
    {
        if (KeyCode == 37) //left
        {
            if (mValue == 0) return;
            mValue--;
            UserControl_Paint();
            Change?.Invoke(mValue);
        }
        if (KeyCode == 39) //right
        {
            if (mValue == 50) return;
            mValue++;
            UserControl_Paint();
            Change?.Invoke(mValue);
        }
        if (KeyCode == 38) //up
        {
            mValue += 10;
            if (mValue > 50) mValue = 50;
            UserControl_Paint();
            Change?.Invoke(mValue);
        }
        if (KeyCode == 40) //down
        {
            mValue -= 10;
            if (mValue < 0) mValue = 0;
            UserControl_Paint();
            Change?.Invoke(mValue);
        }
    }

    private void UserControl_LostFocus(object sender, RoutedEventArgs e)
    {
        HasFocus = false;
        UserControl_Paint();
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
        UserControl_Paint();
    }

    private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) => VBExtension.CallMouseButton(e, this, UserControl_MouseUp);
    private void UserControl_MouseUp(int Button, int Shift, double x, double y)
    {
        Dragging = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        UserControl_Paint();
    }

    private void UserControl_Paint()
    {
        RECT myRect;
        int MyColor;
        int OldColor;
        //int ThreedHilite;
        //int ThreedShadow;
        //int oldPen;
        //POINTAPI foo;

        //ThreedHilite = CreatePen(0, 0, ChangeBrightness(BackColor, 0.4))
        //ThreedShadow = CreatePen(0, 0, ChangeBrightness(BackColor, -0.4))

        //Cls();

        MyColor = CreateSolidBrush(mColorUtils.TranslateColor(BackColor));
        OldColor = SelectObject((int)this.hWnd(), MyColor);

        myRect = new RECT { left = 0, tOp = 0, Bottom = 22, Right = 60 };
        FillRect((int)this.hWnd(), myRect, MyColor);

        myRect.left = 5;
        myRect.tOp = 2;
        myRect.Bottom = 18;
        myRect.Right = 55;
        //  oldPen = SelectObject(hdc, ThreedShadow)
        //  MoveToEx hdc, myRect.Left, myRect.Bottom, foo
        //  LineTo hdc, myRect.Right, myRect.Top
        //  SelectObject hdc, ThreedHilite
        //  LineTo hdc, myRect.Right, myRect.Bottom
        //  LineTo hdc, myRect.Right, myRect.Top
        DrawEdge((int)this.hWnd(), myRect, BDR_SUNKENINNER + BDR_SUNKENINNER, BF_BOTTOMRIGHT);
        DrawEdge((int)this.hWnd(), myRect, BDR_SUNKENINNER, BF_DIAGONAL_ENDTOPRIGHT);

        myRect.left = mValue;
        myRect.Right = mValue + 10;
        myRect.tOp = 0;
        myRect.Bottom = 20;
        FillRect((int)this.hWnd(), myRect, MyColor);//COLOR_BTNFACE
        DrawEdge((int)this.hWnd(), myRect, BDR_RAISEDOUTER + BDR_RAISEDINNER, BF_TOPLEFT + BF_BOTTOMRIGHT);

        SelectObject((int)this.hWnd(), OldColor);
        DeleteObject(MyColor);

        if (HasFocus)
        {
            myRect.left = mValue + 3;
            myRect.tOp = 3;
            myRect.Bottom = 17;
            myRect.Right = mValue + 7;
            //DrawFocusRect((int)this.hWnd(), myRect);
        }

        //DeleteObject(ThreedHilite);
        //DeleteObject(ThreedShadow);
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
