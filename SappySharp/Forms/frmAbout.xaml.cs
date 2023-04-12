using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
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
using static Microsoft.VisualBasic.Globals;
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
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
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


namespace SappySharp.Forms
{
public partial class frmAbout : Window {
  private static frmAbout _instance;
  public static frmAbout instance { set { _instance = null; } get { return _instance ?? (_instance = new frmAbout()); }}  public static void Load() { if (_instance == null) { dynamic A = frmAbout.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmAbout() { InitializeComponent(); }


public List<Window> frmAbout { get => VBExtension.controlArray<Window>(this, "frmAbout"); }

public List<Image> picLogos { get => VBExtension.controlArray<Image>(this, "picLogos"); }

public List<Image> picFont { get => VBExtension.controlArray<Image>(this, "picFont"); }

public List<Timer> timScroll { get => VBExtension.controlArray<Timer>(this, "timScroll"); }

public List<Image> picGroup { get => VBExtension.controlArray<Image>(this, "picGroup"); }

public List<Label> Command1 { get => VBExtension.controlArray<Label>(this, "Command1"); }

public List<Image> picScroller { get => VBExtension.controlArray<Image>(this, "picScroller"); }

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


string lines(128) = ""; // Refactored credits.
int y = 0; // scroller Y position

[DllImport("user32.dll", EntryPoint="DrawTextA")]
private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
  class RECT{
  public int left;
  public int tOp;
  public int Right;
  public int Bottom;
}
int DT_CENTER = 0x1;

[DllImport("gdi32.dll", EntryPoint="CreateFontA")]
private static extern int CreateFont(int H, int W, int E, int o, int W, int i, int u, int S, int C, int OP, int CP, int Q, int PAF, string F);
[DllImport("gdi32.dll")]
private static extern int SelectObject(int hdc, int hObject);
[DllImport("gdi32.dll")]
private static extern int DeleteObject(int hObject);
[DllImport("gdi32.dll")]
private static extern int SetTextColor(int hdc, int crColor);
[DllImport("gdi32.dll")]
private static extern int SetBkColor(int hdc, int crColor);

pcMemDC myDC = new pcMemDC(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages

  private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  ClickSound();
  myDC = null;
  Unload();
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  SetCaptions(ref this);
  Caption = LoadResString(1002);

  myDC.Width = picScroller.ScaleWidth;
  myDC.Height = picScroller.ScaleHeight;

  picGroup.Picture = LoadResPicture("BANNER", 0);
  picScroller.MouseIcon = LoadResPicture("HAND", 2);
  picFont.Picture = LoadResPicture("CREDITSFONT", 0);
  picLogos.Picture = LoadResPicture("CREDITSLOGOS", 0);
  picScroller.BackColor = picFont.point[1, 1].Source;
  y = picScroller.ScaleHeight;

  string b = "";
int C = 0;
int i = 0;
    for (i = 0; i <= 128; i += 1) {
    lines(i) = "";
  }
  b = LoadResString(1001);
  C = 0;
    for (i = 1; i <= Len(b); i += 1) {
    lines(C) = lines(C) + Mid(b, i, 1);
      if(Asc(Mid(b, i, 1)) == 10) {
      C = C + 1;
    }
  }

}

  private void Form_Paint() {
  DrawSkin(ref this);
}

  private void picScroller_MouseDown(ref int Button, ref int Shift, ref decimal x, ref decimal y) {
  timScroll.Interval = 1;

  int i = 0;
  y = (Int(y / 15) - Int(this.y / 15));
    if(y > 0) {
      if(left(lines(y), 7) == "http://") {
      ShellExecute(this.hwnd, "", lines(y), "", "", 0);
    }
  }
}

  private void picScroller_MouseMove(ref int Button, ref int Shift, ref decimal x, ref decimal y) {
  int i = 0;
  y = (Int(y / 15) - Int(this.y / 15));
    if(y > 0) {
      if(left(lines(y), 7) == "http://") {
      picScroller.MousePointer = 99;
      } else {
      picScroller.MousePointer = 0;
    }
  }
}

  private void picScroller_MouseUp(ref int Button, ref int Shift, ref decimal x, ref decimal y) {
  timScroll.Interval = 50;
}

  private void timScroll_Timer() {
  int F = 0;
int of = 0;
RECT rc = null;
int r = 0;
int x = 0;
int i = 0;

    for (r = 0; r <= lines.Count; r += 1) {
    x = (picScroller.ScaleWidth / 2) - ((Len(lines(r)) * 8) / 2) - 4;
      if(Trim(lines(r)) == "<logos>") {
      BitBlt(myDC.hdc, (picScroller.ScaleWidth / 2) - (picLogos.ScaleWidth / 2), y + (r * 15), picLogos.Width, picLogos.Height, picLogos.hdc, 0, 0, vbSrcCopy);
      } else {
        for (i = 1; i <= Len(lines(r)); i += 1) {
          if(Asc(Mid(lines(r), i, 1)) == Asc("\\")) {
          BitBlt(myDC.hdc, x + (i * 8), y + (r * 15), 8, 16, picFont.hdc, 968 + (Abs(((y + r) / 1.5m) % 6) * 8), 0, vbSrcCopy);
          } else if(Asc(Mid(lines(r), i, 1)) == Asc("ß")) {
          BitBlt(myDC.hdc, x + (i * 8), y + (r * 15), 8, 16, picFont.hdc, 864, 0, vbSrcCopy);
          } else if(Asc(Mid(lines(r), i, 1)) >= Asc("à")) {
          BitBlt(myDC.hdc, x + (i * 8), y + (r * 15), 8, 16, picFont.hdc, (Asc(Mid(lines(r), i, 1)) - 132) * 8, 0, vbSrcCopy);
          } else {
          BitBlt(myDC.hdc, x + (i * 8), y + (r * 15), 8, 16, picFont.hdc, (Asc(Mid(lines(r), i, 1)) - 32) * 8, 0, vbSrcCopy);
        }
      }

    }
  }

  StretchBlt(myDC.hdc, 0, 31, picScroller.ScaleWidth, -32, frmSappy.instance.picSkin.hdc, 6, 16, 2, 17, vbSrcAnd);
  StretchBlt(myDC.hdc, 0, picScroller.ScaleHeight - 32, picScroller.ScaleWidth, 32, frmSappy.instance.picSkin.hdc, 6, 16, 2, 17, vbSrcAnd);

  myDC.Draw(picScroller.hdc);

  y = y - 1;
  if(y < -800)y = picScroller.ScaleHeight;
}


}
}