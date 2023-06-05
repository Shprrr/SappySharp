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
using SappySharp.Classes;
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
using System.Windows.Threading;

namespace SappySharp.Forms;

// ______________
// |  SAPPY 2006  |
// |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
// | Interface code © 2006 by Kyoufu Kawa               |
// | Player code © 200X by DJ ßouché                    |
// | In-program graphics by Kyoufu Kawa                 |
// | Thanks to SomeGuy, Majin Bluedragon and SlimeSmile |
// |                                                    |
// | This code is NOT in the Public Domain or whatever. |
// | At least until Kyoufu Kawa releases it in the PD   |
// | himself.  Until then, you're not supposed to even  |
// | HAVE this code unless given to you by Kawa or any  |
// | other Helmeted Rodent member.                      |
// |____________________________________________________|
// ________________
// |  About dialog  |
// |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
// | Code 100% by Kyoufu Kawa.                          |
// | Last update: September 25th, 2006                  |
// |____________________________________________________|

// ###########################################################################################
public partial class frmAbout : Window
{
    private static frmAbout _instance;
    public static frmAbout instance { set { _instance = null; } get { return _instance ??= new frmAbout(); } }
    public static void Load() { if (_instance == null) { dynamic A = frmAbout.instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmAbout()
    {
        InitializeComponent();
        timScroll.Interval = new TimeSpan(0, 0, 0, 0, 50);
        timScroll.Tick += timScroll_Timer;
    }

    public DispatcherTimer timScroll { get; set; } = new DispatcherTimer();

    private string[] lines = new string[128]; // Refactored credits.
    public int y = 0; // scroller Y position

    [DllImport("user32.dll", EntryPoint = "DrawTextA")]
    private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
    class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }

    [DllImport("gdi32.dll", EntryPoint = "CreateFontA")]
    private static extern int CreateFont(int H, int W, int E, int o, int W2, int i, int u, int S, int C, int OP, int CP, int Q, int PAF, string F);
    [LibraryImport("gdi32.dll")]
    private static partial int SelectObject(int hdc, int hObject);
    [LibraryImport("gdi32.dll")]
    private static partial int DeleteObject(int hObject);
    [LibraryImport("gdi32.dll")]
    private static partial int SetTextColor(int hdc, int crColor);
    [LibraryImport("gdi32.dll")]
    private static partial int SetBkColor(int hdc, int crColor);

    pcMemDC myDC = new();

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        ClickSound();
        myDC = null;
        Unload();
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        SetCaptions(this);
        Title = Properties.Resources._1002;

        myDC.Width = (int)picScroller.Width;
        myDC.Height = (int)picScroller.Height;

        picGroup.Source = ConvertBitmap(Properties.Resources.BANNER);
        picScroller.Cursor = Cursors.Hand; //ConvertCursor(Properties.Resources.HAND);
        picFont.Source = ConvertBitmap(Properties.Resources.CREDITSFONT);
        picLogos.Source = ConvertBitmap(Properties.Resources.CREDITSLOGOS);
        //picScroller.BackColor = Properties.Resources.CREDITSFONT.GetPixel(1, 1); //TODO: To review
        y = (int)picScroller.Height;

        for (int i = 0; i <= 128; i += 1)
        {
            lines[i] = "";
        }
        string b = Properties.Resources._1001;
        int C = 0;
        for (int i = 1; i <= Len(b); i += 1)
        {
            lines[C] = lines[C] + Mid(b, i, 1);
            if (Asc(Mid(b, i, 1)) == 10)
            {
                C++;
            }
        }
    }

    private void Form_Paint(object sender, EventArgs e)
    {
        DrawSkin(this);
    }

    private void picScroller_MouseDown(object sender, MouseButtonEventArgs e) => CallMouseButton(e, this, picScroller_MouseDown);
    private void picScroller_MouseDown(int Button, int Shift, double x, double y)
    {
        timScroll.Interval = new TimeSpan(0, 0, 0, 0, 1);

        int i = (int)(Int(y / 15) - Int(this.y / 15));
        if (y > 0)
        {
            if (Left(lines[i], 7) == "http://")
            {
                ShellExecute((int)this.hWnd(), "", lines[i], "", "", 0);
            }
        }
    }

    private void picScroller_MouseMove(object sender, MouseEventArgs e) => CallMouseMove(e, this, picScroller_MouseMove);
    private void picScroller_MouseMove(int Button, int Shift, double x, double y)
    {
        int i = (int)(Int(y / 15) - Int(this.y / 15));
        if (y > 0)
        {
            if (Left(lines[i], 7) == "http://")
            {
                picScroller.ForceCursor = true;
            }
            else
            {
                picScroller.ForceCursor = false;
            }
        }
    }

    private void picScroller_MouseUp(object sender, MouseButtonEventArgs e) => CallMouseButton(e, this, picScroller_MouseUp);
    private void picScroller_MouseUp(int Button, int Shift, double x, double y)
    {
        timScroll.Interval = new TimeSpan(0, 0, 0, 0, 50);
    }

    private void timScroll_Timer(object sender, EventArgs e)
    {
        for (int r = 0; r <= lines.Length; r += 1)
        {
            int x = (int)(picScroller.Width / 2 - Len(lines[r]) * 8 / 2 - 4);
            if (Trim(lines[r]) == "<logos>")
            {
                BitBlt(myDC.hDC, (int)(picScroller.Width / 2 - picLogos.Width / 2), y + r * 15, (int)picLogos.Width, (int)picLogos.Height, (int)picLogos.hWnd(), 0, 0, vbSrcCopy);
            }
            else
            {
                for (int i = 1; i <= Len(lines[r]); i += 1)
                {
                    if (Asc(Mid(lines[r], i, 1)) == Asc("\\"))
                    {
                        BitBlt(myDC.hDC, x + i * 8, y + r * 15, 8, 16, (int)picFont.hWnd(), (int)(968 + Abs((y + r) / 1.5m % 6) * 8), 0, vbSrcCopy);
                    }
                    else if (Asc(Mid(lines[r], i, 1)) == Asc("ß"))
                    {
                        BitBlt(myDC.hDC, x + i * 8, y + r * 15, 8, 16, (int)picFont.hWnd(), 864, 0, vbSrcCopy);
                    }
                    else if (Asc(Mid(lines[r], i, 1)) >= Asc("à"))
                    {
                        BitBlt(myDC.hDC, x + i * 8, y + r * 15, 8, 16, (int)picFont.hWnd(), (Asc(Mid(lines[r], i, 1)) - 132) * 8, 0, vbSrcCopy);
                    }
                    else
                    {
                        BitBlt(myDC.hDC, x + i * 8, y + r * 15, 8, 16, (int)picFont.hWnd(), (Asc(Mid(lines[r], i, 1)) - 32) * 8, 0, vbSrcCopy);
                    }
                }

            }
        }

        StretchBlt(myDC.hDC, 0, 31, (int)picScroller.Width, -32, (int)frmSappy.instance.picSkin.hWnd(), 6, 16, 2, 17, vbSrcAnd);
        StretchBlt(myDC.hDC, 0, (int)picScroller.Height - 32, (int)picScroller.Width, 32, (int)frmSappy.instance.picSkin.hWnd(), 6, 16, 2, 17, vbSrcAnd);

        myDC.Draw((int)picScroller.hWnd());

        y--;
        if (y < -800) y = (int)picScroller.Height;
    }
}
