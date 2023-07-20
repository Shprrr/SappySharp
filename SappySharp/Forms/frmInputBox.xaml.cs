using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using static modSappy;
using static VBConstants;
using static VBExtension;

namespace SappySharp.Forms;

public partial class frmInputBox : Window
{
    public frmInputBox() { InitializeComponent(); }

    [LibraryImport("user32.dll")]
    private static partial int ReleaseCapture();
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 2;

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        Text1.Text = "";
        Hide();
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        Hide();
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        SetCaptions(this);
    }

    private void Form_MouseDown(object sender, MouseButtonEventArgs e) => CallMouseButton(e, this, Form_MouseDown);
    private void Form_MouseDown(int Button, int Shift, double x, double y)
    {
        Form_MouseMove(Button, Shift, x, y);
    }

    private void Form_MouseMove(object sender, MouseEventArgs e) => CallMouseMove(e, this, Form_MouseMove);
    private void Form_MouseMove(int Button, int Shift, double x, double y)
    {
        if (Button == vbLeftButton)
        {
            ReleaseCapture();
            SendMessageLong((int)this.hWnd(), WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
    }

    private void Form_Paint(object sender, EventArgs e)
    {
        DrawSkin(this);
    }

    private void Label1_MouseDown(object sender, MouseButtonEventArgs e) => CallMouseButton(e, this, Label1_MouseDown);
    private void Label1_MouseDown(int Button, int Shift, double x, double y)
    {
        Form_MouseMove(Button, Shift, x, y);
    }

    private void Label1_MouseMove(object sender, MouseEventArgs e) => CallMouseMove(e, this, Label1_MouseMove);
    private void Label1_MouseMove(int Button, int Shift, double x, double y)
    {
        Form_MouseMove(Button, Shift, x, y);
    }
}
