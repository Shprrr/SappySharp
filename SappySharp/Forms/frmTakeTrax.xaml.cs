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
public partial class frmTakeTrax : Window {
  private static frmTakeTrax _instance;
  public static frmTakeTrax instance { set { _instance = null; } get { return _instance ?? (_instance = new frmTakeTrax()); }}  public static void Load() { if (_instance == null) { dynamic A = frmTakeTrax.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmTakeTrax() { InitializeComponent(); }


public List<Window> frmTakeTrax { get => VBExtension.controlArray<Window>(this, "frmTakeTrax"); }

public List<TextBox> txtLog { get => VBExtension.controlArray<TextBox>(this, "txtLog"); }

public List<Image> Picture1 { get => VBExtension.controlArray<Image>(this, "Picture1"); }

public List<Label> Label9 { get => VBExtension.controlArray<Label>(this, "Label9"); }

public List<Label> Label10 { get => VBExtension.controlArray<Label>(this, "Label10"); }

public List<Label> Command2 { get => VBExtension.controlArray<Label>(this, "Command2"); }

public List<Label> Command1 { get => VBExtension.controlArray<Label>(this, "Command1"); }

public List<TextBox> txtFilename { get => VBExtension.controlArray<TextBox>(this, "txtFilename"); }

public List<ListBox> lstTracks { get => VBExtension.controlArray<ListBox>(this, "lstTracks"); }

public List<Line> Line3 { get => VBExtension.controlArray<Line>(this, "Line3"); }

public List<Line> Line2 { get => VBExtension.controlArray<Line>(this, "Line2"); }

public List<Line> Line4 { get => VBExtension.controlArray<Line>(this, "Line4"); }

public List<Label> Label4 { get => VBExtension.controlArray<Label>(this, "Label4"); }

public List<Label> Label3 { get => VBExtension.controlArray<Label>(this, "Label3"); }

public List<Line> Line1 { get => VBExtension.controlArray<Line>(this, "Line1"); }

public List<Label> Label2 { get => VBExtension.controlArray<Label>(this, "Label2"); }

public List<Label> Label1 { get => VBExtension.controlArray<Label>(this, "Label1"); }

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
 // __________________
 // |  Track exporter  |
 // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
 // | Code 100% by Kyoufu Kawa, based on Juicer.         |
 // | Last update: February 1st, 2005                    |
 // |____________________________________________________|

 // ###########################################################################################


  private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  ClickSound();
  Unload();
}

  private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
private void Command2_Click() {
  int i = 0;
  string t = "";
    if(lstTracks.SelCount == 0) {
    MsgBox(LoadResString(3005));
    IncessantNoises(ref "TaskFail"); // Bee-owee-owee-oweeeeeohh....
    return;
  }
    if(txtFilename.Text == "") {
    MsgBox(LoadResString(3006));
    IncessantNoises(ref "TaskFail"); // Bee-owee-owee-oweeeeeohh....
    return;
  }

  txtFilename.Text = Replace(txtFilename.Text, "$t", "$T");
  txtFilename.Text = Replace(txtFilename.Text, "$p", "$P");
  i = 2;
  if(InStr(txtFilename.Text, "$T"))i = i - 1;
  if(InStr(txtFilename.Text, "$P"))i = i - 1;
    if(i == 2) {
    MsgBox(LoadResString(3011));
    IncessantNoises(ref "TaskFail"); // Bee-owee-owee-oweeeeeohh....
    return;
  }

  ClickSound();

  txtLog.Top = 8;
  txtLog.Visibility = true;
  Scribe(LoadResString(3007));
  Scribe(String(Len(LoadResString(3007)), "¯"));
    for (i = 0; i <= lstTracks.Items.Count - 1; i += 1) {
      if(lstTracks.Selected[i].DefaultProperty) {
      Scribe(Replace(LoadResString(3008), "$TRACK", i));
      t = txtFilename.Text;
      t = Replace(t, "$T", i);
      t = lstTracks.List[i].DefaultProperty;
      Scribe(Replace(LoadResString(3009), "$FILE", t));
      DumpTrack(lstTracks.List[i].DefaultProperty, t);
      } else {
      Scribe(Replace(LoadResString(3010), "$TRACK", i));
    }
  }
  Scribe(LoadResString(7));
  Command2.IsEnabled = false;
   // Command1.FontBold = False
  Command1.Caption = LoadResString(6);
   // Command1.FontBold = True
  Command1.IsDefault = true;
  IncessantNoises(ref "TaskComplete");
   // Unload Me
}

  private void Scribe(ref string t) {
  txtLog.Text = txtLog.Text + t + vbCrLf;
  txtLog.SelectionStart = Len(txtLog.Text);
}

  private void DumpTrack(ref int o, ref string t) {
  Byte b = 0;
  int p = 0;
  VBOpenFile(t, "Binary", 98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open t For Binary As #98
  Seek(#99, o + 1);
    do {
    Get(#99, , b);
    Put(#98, , b);
      if(b == 0xB1) { // fine
       // Scribe LoadResString(3012)
      break;
    }
      if(b == 0xB2 || b == 0xB3 || b == 0xB5) { // goto/patt/rept
        if(b == 0xB5) {
        Get(#99, , b); // num reps
        Put(#98, , b);
      }
      Get(#99, , p);
      p = p - 0x8000000 - o;
      Put(#98, , p);
    }
    DoEvents();
  } while(!(EOF(98)));
  VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  SetCaptions(ref this);
  Caption = LoadResString(3000);
}

  private void Picture1_Paint() {
  DrawSkin(ref Picture1.Source);
}


}
}