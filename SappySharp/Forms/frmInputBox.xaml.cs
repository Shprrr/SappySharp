using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using System.Linq;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SappySharp.Forms;
using static modSappy;
using static FMod;
using static mdlFile;
using static SapPlayer;
using static MidiLib;
using static mColorUtils;
using static mTrace;
using static SappySharp.Forms.frmSappy;
using static SappySharp.Forms.frmTakeTrax;
using static SappySharp.Forms.frmMakeTrax;
using static SappySharp.Forms.frmAbout;
using static SappySharp.Forms.frmTakeSamp;
using static SappySharp.Forms.frmAssembler;
using static SappySharp.Forms.frmOptions;
using static SappySharp.Forms.frmMidiMapper;
using static SappySharp.Forms.frmSelectMidiOut;
using static SappySharp.Forms.frmInputBox;
using static SappySharp.Classes.cNoStatusBar;
using static SappySharp.Classes.SChannels;
using static SappySharp.Classes.SNotes;
using static SappySharp.Classes.NoteInfo;
using static SappySharp.Classes.SChannel;
using static SappySharp.Classes.SNote;
using static SappySharp.Classes.SSubroutines;
using static SappySharp.Classes.SSubroutine;
using static SappySharp.Classes.SappyEventQueue;
using static SappySharp.Classes.SappyEvent;
using static SappySharp.Classes.NoteInfos;
using static SappySharp.Classes.SSamples;
using static SappySharp.Classes.SSample;
using static SappySharp.Classes.SDirects;
using static SappySharp.Classes.SDirect;
using static SappySharp.Classes.SDrumKit;
using static SappySharp.Classes.SDrumKits;
using static SappySharp.Classes.SInstruments;
using static SappySharp.Classes.SInstrument;
using static SappySharp.Classes.SKeyMaps;
using static SappySharp.Classes.SKeyMap;
using static SappySharp.Classes.clsSappyDecoder;
using static SappySharp.Classes.gCommonDialog;
using static SappySharp.Classes.pcMemDC;
using static SappySharp.Classes.cVBALImageList;
using static SappySharp.Classes.cRegistry;

namespace SappySharp.Forms;

public partial class frmInputBox : Window
{
    private static frmInputBox _instance;
    public static frmInputBox instance { set { _instance = null; } get { return _instance ??= new frmInputBox(); } }
    public static void Load() { if (_instance == null) { dynamic A = frmInputBox.instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmInputBox() { InitializeComponent(); }

    [LibraryImport("user32.dll")]
    private static partial int ReleaseCapture();
    [DllImport("user32.dll", EntryPoint = "SendMessageA")]
    private static extern int SendMessage(int hwnd, int wMsg, int wParam, ref dynamic lParam);
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
