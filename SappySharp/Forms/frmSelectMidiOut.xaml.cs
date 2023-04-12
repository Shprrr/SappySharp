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
public partial class frmSelectMidiOut : Window {
  private static frmSelectMidiOut _instance;
  public static frmSelectMidiOut instance { set { _instance = null; } get { return _instance ?? (_instance = new frmSelectMidiOut()); }}  public static void Load() { if (_instance == null) { dynamic A = frmSelectMidiOut.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmSelectMidiOut() { InitializeComponent(); }


public List<Window> frmSelectMidiOut { get => VBExtension.controlArray<Window>(this, "frmSelectMidiOut"); }

public List<Label> Command1 { get => VBExtension.controlArray<Label>(this, "Command1"); }

public List<ListBox> List1 { get => VBExtension.controlArray<ListBox>(this, "List1"); }

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
 // |  MidiOUT dialog  |
 // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
 // | Code 100% by Kyoufu Kawa.                          |
 // | Last update: July 20th, 2006                       |
 // |____________________________________________________|

 // ###########################################################################################


[DllImport("winmm.dll")]
private static extern int midiOutGetNumDevs();
[DllImport("winmm.dll", EntryPoint="midiOutGetDevCapsA")]
private static extern int midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS lpCaps, int uSize);

  class MIDIOUTCAPS{
  public int wMid;
  public int wPid;
  public int vDriverVersion;
  public string szPname; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (32)
  public int wTechnology;
  public int wVoices;
  public int wNotes;
  public int wChannelMask;
  public int dwSupport;
}

  private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  ClickSound();
  WantedMidiDevice = List1.SelectedIndex;
  WriteSettingI(ref "MIDI Device", ref WantedMidiDevice);
  Unload();
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
    if(midiOutGetNumDevs == 0) {
    List1.AddItem("No devices.");
    return;
  }

  int i = 0;
MIDIOUTCAPS myCaps = null;
    for (i = 1; i <= midiOutGetNumDevs; i += 1) {
    midiOutGetDevCaps(i - 1, myCaps, 52); // LenB(myCaps)
    List1.AddItem(myCaps.szPname); // Trim(myCaps.szPname)
  }

  List1.SelectedIndex = WantedMidiDevice;

  SetCaptions(ref this);
  Caption = LoadResString(9000);
}

  private void Form_Paint() {
  DrawSkin(ref this);
}

  private void List1_Click(object sender, RoutedEventArgs e) { List1_Click(); }
private void List1_Click() {
  if(midiOutGetNumDevs == 0)Label1.Content = "";

  MIDIOUTCAPS myCaps = null;
  midiOutGetDevCaps(List1.SelectedIndex, myCaps, 52);
  Label1.Content = LoadResString(9000 + myCaps.wTechnology);
      switch(myCaps.wTechnology) {
      case 1: Label1.Content = Label1.Content + vbCrLf + "W00t!";
      break;
case 3: Label1.Content = Label1.Content + vbCrLf + "Oooh...";
      break;
case 4: Label1.Content = Label1.Content + vbCrLf + "Lame...";
      break;
case 6: Label1.Content = Label1.Content + vbCrLf + "Sweet!";
      break;
case 7: Label1.Content = Label1.Content + vbCrLf + "Cool!";
break;
}
}

 // Private Const MOD_MIDIPORT As Long = 1
 // Private Const MOD_SYNTH As Long = 2
 // Private Const MOD_SQSYNTH As Long = 3
 // Private Const MOD_FMSYNTH As Long = 4
 // Private Const MOD_MAPPER As Long = 5
 // Private Const MOD_WAVETABLE As Long = 6
 // Private Const MOD_SWSYNTH As Long = 7


}
}