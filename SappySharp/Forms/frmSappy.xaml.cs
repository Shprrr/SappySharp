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
public partial class frmSappy : Window {
  private static frmSappy _instance;
  public static frmSappy instance { set { _instance = null; } get { return _instance ?? (_instance = new frmSappy()); }}  public static void Load() { if (_instance == null) { dynamic A = frmSappy.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmSappy() { InitializeComponent(); }


public List<Window> frmSappy { get => VBExtension.controlArray<Window>(this, "frmSappy"); }

public List<Image> picTop { get => VBExtension.controlArray<Image>(this, "picTop"); }

public List<Label> cmdStop { get => VBExtension.controlArray<Label>(this, "cmdStop"); }

public List<Label> cmdPrevSong { get => VBExtension.controlArray<Label>(this, "cmdPrevSong"); }

public List<Label> cmdNextSong { get => VBExtension.controlArray<Label>(this, "cmdNextSong"); }

public List<TextBox> txtSong { get => VBExtension.controlArray<TextBox>(this, "txtSong"); }

public List<Label> cmdPlay { get => VBExtension.controlArray<Label>(this, "cmdPlay"); }

public List<Label> cmdSpeed { get => VBExtension.controlArray<Label>(this, "cmdSpeed"); }

public List<Label> cbxSongs { get => VBExtension.controlArray<Label>(this, "cbxSongs"); }

public List<Label> VolumeSlider1 { get => VBExtension.controlArray<Label>(this, "VolumeSlider1"); }

public List<Label> lblSong { get => VBExtension.controlArray<Label>(this, "lblSong"); }

public List<Label> lblSpeed { get => VBExtension.controlArray<Label>(this, "lblSpeed"); }

public List<Label> Label4 { get => VBExtension.controlArray<Label>(this, "Label4"); }

public List<Label> lblInst { get => VBExtension.controlArray<Label>(this, "lblInst"); }

public List<Label> Label3 { get => VBExtension.controlArray<Label>(this, "Label3"); }

public List<Label> lblLoc { get => VBExtension.controlArray<Label>(this, "lblLoc"); }

public List<Label> Label2 { get => VBExtension.controlArray<Label>(this, "Label2"); }

public List<Label> lblDef { get => VBExtension.controlArray<Label>(this, "lblDef"); }

public List<Label> Label1 { get => VBExtension.controlArray<Label>(this, "Label1"); }

public List<Image> picSkin { get => VBExtension.controlArray<Image>(this, "picSkin"); }

public List<Timer> timPlay { get => VBExtension.controlArray<Timer>(this, "timPlay"); }

public List<Image> picStatusbar { get => VBExtension.controlArray<Image>(this, "picStatusbar"); }

public List<Image> picChannels { get => VBExtension.controlArray<Image>(this, "picChannels"); }

public List<Label> chkMute { get => VBExtension.controlArray<Label>(this, "chkMute"); }

public List<Label> cvwChannel { get => VBExtension.controlArray<Label>(this, "cvwChannel"); }

public List<Line> linProgress { get => VBExtension.controlArray<Line>(this, "linProgress"); }

public List<Label> lblExpand { get => VBExtension.controlArray<Label>(this, "lblExpand"); }

public List<Label> lblSongName { get => VBExtension.controlArray<Label>(this, "lblSongName"); }

public List<Line> Line2 { get => VBExtension.controlArray<Line>(this, "Line2"); }

public List<Line> Line1 { get => VBExtension.controlArray<Line>(this, "Line1"); }

public List<Label> lblPC { get => VBExtension.controlArray<Label>(this, "lblPC"); }

public List<Label> lblDel { get => VBExtension.controlArray<Label>(this, "lblDel"); }

public List<Label> lblNote { get => VBExtension.controlArray<Label>(this, "lblNote"); }

public List<Label> ebr { get => VBExtension.controlArray<Label>(this, "ebr"); }

public List<Image> picScreenshot { get => VBExtension.controlArray<Image>(this, "picScreenshot"); }

public List<Label> cPop { get => VBExtension.controlArray<Label>(this, "cPop"); }

public List<Shape> Shape1 { get => VBExtension.controlArray<Shape>(this, "Shape1"); }

public List<Menu> mnuFile { get => VBExtension.controlArray<Menu>(this, "mnuFile"); }

public List<Menu> mnuFileOpen { get => VBExtension.controlArray<Menu>(this, "mnuFileOpen"); }

public List<Menu> mnuFileSep { get => VBExtension.controlArray<Menu>(this, "mnuFileSep"); }

public List<Menu> mnuFileExit { get => VBExtension.controlArray<Menu>(this, "mnuFileExit"); }

public List<Menu> mnuTasks { get => VBExtension.controlArray<Menu>(this, "mnuTasks"); }

public List<Menu> mnuOptions { get => VBExtension.controlArray<Menu>(this, "mnuOptions"); }

public List<Menu> mnuOutput { get => VBExtension.controlArray<Menu>(this, "mnuOutput"); }

public List<Menu> mnuOptionsSep { get => VBExtension.controlArray<Menu>(this, "mnuOptionsSep"); }

public List<Menu> mnuSeekPlaylist { get => VBExtension.controlArray<Menu>(this, "mnuSeekPlaylist"); }

public List<Menu> mnuAutovance { get => VBExtension.controlArray<Menu>(this, "mnuAutovance"); }

public List<Menu> mnuGBMode { get => VBExtension.controlArray<Menu>(this, "mnuGBMode"); }

public List<Menu> mnuOptionsSep2 { get => VBExtension.controlArray<Menu>(this, "mnuOptionsSep2"); }

public List<Menu> mnuImportLST { get => VBExtension.controlArray<Menu>(this, "mnuImportLST"); }

public List<Menu> mnuSelectMIDI { get => VBExtension.controlArray<Menu>(this, "mnuSelectMIDI"); }

public List<Menu> mnuMidiMap { get => VBExtension.controlArray<Menu>(this, "mnuMidiMap"); }

public List<Menu> mnuOptionsSep3 { get => VBExtension.controlArray<Menu>(this, "mnuOptionsSep3"); }

public List<Menu> mnuSettings { get => VBExtension.controlArray<Menu>(this, "mnuSettings"); }

public List<Menu> mnuHelp { get => VBExtension.controlArray<Menu>(this, "mnuHelp"); }

public List<Menu> mnuHelpHelp { get => VBExtension.controlArray<Menu>(this, "mnuHelpHelp"); }

public List<Menu> mnuHelpAbout { get => VBExtension.controlArray<Menu>(this, "mnuHelpAbout"); }

public List<Menu> mnuHelpOnline { get => VBExtension.controlArray<Menu>(this, "mnuHelpOnline"); }

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
 // |  Main interface  |
 // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
 // | Code 99% by Kyoufu Kawa.                           |
 // | Last update: July 22nd, 2006                       |
 // |____________________________________________________|

 // ###########################################################################################


  class RECT{
  public int left;
  public int tOp;
  public int Right;
  public int Bottom;
}

  class tSongHeader{
  public Byte NumTracks;
  public Byte NumBlocks;
  public Byte Priority;
  public Byte Reverb;
  public int VoiceGroup;
  public int Tracks(32);
}

  class tPlaylist{
  public int NumSongs;
  public string SongName(1024);
  public int SongIcon(1024);
  public int SongNo(1024);
}

public static List<tPlaylist> playlist = new List<tPlaylist>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: playlist(0 To 255)
public static int NumPLs = 0;
public static List<int> MidiMap = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: MidiMap(0 To 127)
public static List<int> MidiMapTrans = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: MidiMapTrans(0 To 127)
public static List<int> DrumMap = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: DrumMap(0 To 127)
public static List<int> BleedingEars = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: BleedingEars(0 To 127)
public static int BECnt = 0;
public IXMLDOMElement MidiMapNode = null;
public IXMLDOMElement MidiMapsDaddy = null;

public static tSongHeader SongHead = null;
public static int SongHeadOrg = 0;

public int SongTbl = 0;
public string gamecode = "";
public string myFile = "";
public string xfile = "";

public static bool DontLoadDude = false;
public static int TaskMenus(16) = 0;

public cVBALImageList imlImages = null;
public cVBALImageList imlStatusbar = null;
public static cNoStatusBar cStatusBar = null;
Attribute(cStatusBar.VB_VarHelpID == -1);
public DOMDocument x = new DOMDocument(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
public IXMLDOMElement rootNode = null;

Implements(ISubclass);
public static EMsgResponse e_mr = null;
public const int WM_SIZING = 0x214;
public static int mywidth = 0;

public const int WM_APPCOMMAND = 0x319;
public const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
public const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
public const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
public const int APPCOMMAND_MEDIA_STOP = 13;
public const int APPCOMMAND_VOLUME_DOWN = 9;
public const int APPCOMMAND_VOLUME_UP = 10;

public const int WM_MOUSEWHEEL = 0x20A;

public clsSappyDecoder SappyDecoder = null;
Attribute(SappyDecoder.VB_VarHelpID == -1);
public List<string> DutyCycleWave = new List<string>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: DutyCycleWave(0 To 4)
public List<int> DutyCycle = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: DutyCycle(0 To 4)
public bool mm = false;

public static bool Playing = false;
public static int TotalMinutes = 0;
public static int TotalSeconds = 0;
public static int loopsToGo = 0;
public static string songinfo = "";
public static string justthesongname = "";
public static int WantToRecord = 0;
public static string WantToRecordTo = "";
public static int FullWidth = 0;
public static int ClassicWidth = 0;

[DllImport("winmm.dll")]
private static extern int midiOutGetNumDevs();
[DllImport("gdi32.dll")]
private static extern int GetPixel(int hdc, int x, int y);

 // Private WithEvents HookedDialog As cCommonDialog
 // Private m_bInIDE As Boolean

  private void cbxSongs_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { cbxSongs_Change(); }
private void cbxSongs_Change() {
  int i = 0;
  if(DontLoadDude == true)return;
  if(cbxSongs.ItemData[cbxSongs.ListIndex].DefaultProperty)return; // don't try to load playlists
  txtSong.Text = cbxSongs.ItemData[cbxSongs.ListIndex].DefaultProperty;
  LoadSong(txtSong.Text);
}

  private void chkMute_Click(object sender, RoutedEventArgs e) { chkMute_Click(); }
private void chkMute_Click() {
  int i = 0;
  if(chkMute.Tag == "^_^")return;
   // If Playing = True Then
   // For i = 0 To SappyDecoder.SappyChannels.count - 1
   // cvwChannel(i).mute = chkMute.value
   // Next i
   // Else
  chkMute.Tag = "O.O";
    for (i = 0; i <= cvwChannel.count - 1; i += 1) {
    cvwChannel[i].DefaultProperty = chkMute.Value;
  }
  chkMute.Tag = "-_-";
   // End If
}

  private void cmdPlay_Click(object sender, RoutedEventArgs e) { cmdPlay_Click(); }
private void cmdPlay_Click() {
  int i = 0;
  string s = "";
  cmdStop_Click();
  MousePointer = 11;
  SappyDecoder.outputtype = (mnuOutput[1].DefaultProperty ? sotWave : sotMIDI);
  SappyDecoder.ClearMidiPatchMap();
    for (i = 0; i <= 127; i += 1) {
     // SappyDecoder.MidiMap(i) = MidiMap(i)
    SappyDecoder.SetMidiPatchMap(i, MidiMap[i], MidiMapTrans[i]);
    SappyDecoder.SetMidiDrumMap(i, DrumMap[i]);
  }
    for (i = 0; i <= BECnt; i += 1) {
    SappyDecoder.AddEarPiercer(BleedingEars[i]);
  }
    if(mnuGBMode.Checked == true) {
      for (i = 0; i <= 126; i += 1) {
       // SappyDecoder.MidiMap(i) = IIf(i Mod 2 = 1, 80, 81) '80
      SappyDecoder.SetMidiPatchMap(i, (i % 2 == 1 ? 80 : 81), 0);
    }
  }

  linProgress.x2 = -1;

  SappyDecoder.GlobalVolume = VolumeSlider1.GetValue * 5.1m;
  SappyDecoder.PlaySong(myFile, txtSong.Text, SongTbl, ((WantToRecord)), WantToRecordTo);

  WantToRecord = 2;

  cStatusBar.PanelText("simple") = "";

    for (i = 0; i <= SappyDecoder.SappyChannels.count - 1; i += 1) {
    SappyDecoder.SappyChannels(i + 1).mute = IIf(cvwChannel(i).mute = 1, False, True);
    cvwChannel[i].DefaultProperty = "";
    cvwChannel[i].DefaultProperty = 0;
    cvwChannel[i].DefaultProperty = 0;
    cvwChannel[i].DefaultProperty = 0;
  }

  lblSpeed.Content = SappyDecoder.Tempo;

  TotalMinutes = 0;
  TotalSeconds = 0;
  timPlay.IsEnabled = true;

  loopsToGo = GetSettingI(ref "Song Repeats");
  Playing = true;
  cmdPlay.Icon = 20;

    if(GetSettingI(ref "mIRC Now Playing")) {
    VBOpenFile(App.Path, "&", "\\sappy.stt" For Output As #43); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open App.Path & __S1 For Output As #43
    VBWriteFile("Print #43, songinfo"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #43, songinfo
    VBCloseFile(43); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #43
  }

    if(GetSettingI(ref "MSN Now Playing")) {
    TellMSN(ref justthesongname + IIf(SappyDecoder.outputtype == sotMIDI, " (midi)", ""), ref "Sappy " + App.Major + "." + App.Minor, ref ebr.Bars["Info"].DefaultProperty); // ebr.Bars(__S9).Items(__S10).Text
  }

  mnuOutput[0].DefaultProperty = false;
  mnuOutput[1].DefaultProperty = false;
  mnuGBMode.IsEnabled = false;
  mnuSelectMIDI.IsEnabled = false;
  mnuMidiMap.IsEnabled = false;
  MousePointer = 0;
}

  private void cmdNextSong_Click(object sender, RoutedEventArgs e) { cmdNextSong_Click(); }
private void cmdNextSong_Click() {
    if(mnuSeekPlaylist.Checked == true) {
    if(cbxSongs.ListCount == 1)return;
      if(cbxSongs.SelectedIndex == cbxSongs.ListCount - 1) {
      cbxSongs.SelectedIndex = 1;
      } else {
      cbxSongs.SelectedIndex = cbxSongs.SelectedIndex + 1;
        do {
          if(cbxSongs.ItemData[cbxSongs.ListIndex].DefaultProperty) {
            if(cbxSongs.SelectedIndex == cbxSongs.ListCount - 1) {
            cbxSongs.SelectedIndex = 1;
            } else {
            cbxSongs.SelectedIndex = cbxSongs.SelectedIndex + 1;
          }
          } else {
          break;
        }
      }
    }
    cbxSongs_Change();
    } else {
    if(txtSong.Text == 1024)return;
    txtSong.Text = txtSong.Text + 1;
    LoadSong(txtSong.Text);
  }
}

  private void cmdPrevSong_Click(object sender, RoutedEventArgs e) { cmdPrevSong_Click(); }
private void cmdPrevSong_Click() {
    if(mnuSeekPlaylist.Checked == true) {
    if(cbxSongs.ListCount == 1)return;
      if(cbxSongs.SelectedIndex == 0) {
      cbxSongs.SelectedIndex = cbxSongs.ListCount - 1;
      } else {
      cbxSongs.SelectedIndex = cbxSongs.SelectedIndex - 1;
        do {
          if(cbxSongs.ItemData[cbxSongs.ListIndex].DefaultProperty) {
            if(cbxSongs.SelectedIndex == 0) {
            cbxSongs.SelectedIndex = cbxSongs.ListCount - 1;
            } else {
            cbxSongs.SelectedIndex = cbxSongs.SelectedIndex - 1;
          }
          } else {
          break;
        }
      }
    }
    cbxSongs_Change();
    } else {
    if(txtSong.Text == 0)return; // 181205 update: Someguy complained about lack of Track Zero in Sonic
    txtSong.Text = txtSong.Text - 1;
    LoadSong(txtSong.Text);
  }
}

  private void cmdSpeed_Click(object sender, RoutedEventArgs e) { cmdSpeed_Click(); }
private void cmdSpeed_Click(ref int Index) {
  if(Index == 0)SappyDecoder.Tempo = Int(SappyDecoder.Tempo / 2);
  if(Index == 1)SappyDecoder.Tempo = Int(SappyDecoder.Tempo * 2);
  lblSpeed.Content = SappyDecoder.Tempo;
}

  private void cmdStop_Click(object sender, RoutedEventArgs e) { cmdStop_Click(); }
private void cmdStop_Click() {
    if(Playing == true) {
    Playing = false;
    SappyDecoder.StopSong();
    linProgress.x2 = -1;
  }
  if(mnuAutovance.Checked == true)cmdPrevSong_Click();

  if(WantToRecord == 2)WantToRecord = 0;

  Playing = false;
  timPlay.IsEnabled = false;
  cmdPlay.Icon = 19;
  int i = 0;
    for (i = 0; i <= cvwChannel.count - 1; i += 1) {
    cvwChannel[i].DefaultProperty = 0;
    cvwChannel[i].DefaultProperty = 0;
  }

  // TODO: (NOT SUPPORTED): On Error Resume Next
    if(GetSetting(ref "mIRC Now Playing")) {
    VBOpenFile(App.Path, "&", "\\sappy.stt" For Output As #44); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open App.Path & __S1 For Output As #44
    VBWriteFile("Print #44, App.Major & "." & App.Minor & "| | | |Not running|""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #44, App.Major & __S1 & App.Minor & __S2
    VBCloseFile(44); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #44
  }

  if(GetSettingI(ref "MSN Now Playing"))ShutMSN();
  // TODO: (NOT SUPPORTED): On Error GoTo 0

  mnuOutput[0].DefaultProperty = true;
  mnuOutput[1].DefaultProperty = true;
  mnuSelectMIDI.IsEnabled = true;
  mnuMidiMap.IsEnabled = true;
  mnuGBMode.IsEnabled = mnuOutput[0].DefaultProperty;
}

  private void cPop_Click(object sender, RoutedEventArgs e) { cPop_Click(); }
private void cPop_Click(ref int ItemNumber) {
  vbalExplorerBarLib6.cExplorerBarItem itm = null;
  int i = 0;
    for (i = 1; i <= 16; i += 1) {
      if(ItemNumber == TaskMenus(i)) {
      itm = ebr.Bars["Tasks"].DefaultProperty;
      ebr_ItemClick(itm);
    }
  }
}

  private void cPop_ItemHighlight(ref int ItemNumber, ref bool bEnabled, ref bool bSeparator) {
  cStatusBar.PanelText("simple") = cPop.HelpText[ItemNumber].DefaultProperty;
   // cStatusBar.SimpleMode = True
   // cStatusBar.SimpleText = cPop.HelpText(ItemNumber)
  picStatusbar.Refresh();
}

  private void cPop_MenuExit() {
  cStatusBar.PanelText("simple") = "";
   // cStatusBar.SimpleMode = False
  picStatusbar.Refresh();
}

  private void cvwChannel_MuteChanged(ref int Index) {
   // If Playing = False Then Exit Sub
  // TODO: (NOT SUPPORTED): On Error Resume Next
  if(SappyDecoder.SappyChannels.count < 1)goto FlickIt; // Exit Sub
  SappyDecoder.SappyChannels(Index + 1).mute = IIf(cvwChannel(Index).mute = 1, False, True);

  FlickIt:;
  if(chkMute.Tag == "O.O")return;
  int i = 0;
int j = 0;
    for (i = 0; i <= SappyDecoder.SappyChannels.count - 1; i += 1) {
    if(cvwChannel[i].DefaultProperty)j = j + 1;
  }
  chkMute.Tag = "^_^";
    if(j == SappyDecoder.SappyChannels.count) {
    chkMute.Value = 1;
    } else if(j == 0) {
    chkMute.Value = 0;
    } else {
    chkMute.Value = 2;
  }
  chkMute.Tag = "-_-";
}

  private void cvwChannel_Resize(ref int Index) {
  int i = 0;
    for (i = 1; i <= cvwChannel.count - 1; i += 1) {
    cvwChannel[i].DefaultProperty = cvwChannel[i - 1].DefaultProperty;
  }
}

  private void ebr_BarClick(ref vbalExplorerBarLib6.cExplorerBar bar) {
  WriteSettingI(ref "Bar " + bar.Index + " state", ref bar.State);
}

  private void ebr_ItemClick(ref vbalExplorerBarLib6.cExplorerBarItem itm) {
  int i = 0;
  string s = "";
    if(itm.Key == "taketrax") {
      for (i = 0; i <= SongHead.NumTracks - 1; i += 1) {
      frmTakeTrax.instance.lstTracks.AddItem("0x" + FixHex(ref SongHead.Tracks(i), ref 6));
    }
    frmTakeTrax.instance.ShowDialog();
  }
    if(itm.Key == "maketrax") {
    frmMakeTrax.txtHeaderOffset.Text = "0x" + FixHex(ref SongHeadOrg, ref 6);
    frmMakeTrax.txtTrack.Text = "0x" + FixHex(ref SongHead.Tracks(i), ref 6);
    frmMakeTrax.MyNumblocks = SongHead.NumBlocks;
    frmMakeTrax.MyPriority = SongHead.Priority;
    frmMakeTrax.MyReverb = SongHead.Reverb;
    frmMakeTrax.txtVoicegroup.Text = "0x" + FixHex(ref SongHead.VoiceGroup, ref 6);
    frmMakeTrax.SongTableEntry = SongTbl + (txtSong.Text * 8);
    frmMakeTrax.instance.ShowDialog();
  }
    if(itm.Key == "takesamp") {
    frmTakeSamp.SingleSong = SongHeadOrg;
    frmTakeSamp.instance.ShowDialog();
  }
    if(itm.Key == "codetrax") {
    frmAssembler.SongTableEntry = SongTbl + (txtSong.Text * 8);
    frmAssembler.txtVoicegroup.Text = "0x" + FixHex(ref SongHead.VoiceGroup, ref 6);
    frmAssembler.instance.ShowDialog();
  }
  if(itm.Key == "makemidi")PrepareRecording();

    if(itm.Key == "Game") {
    s = ebr.Bars["Info"].DefaultProperty;
    s = InputBox(ref LoadResString(210), ref , ref s);
    if(s == "")goto hell;
    ebr.Bars["Info"].DefaultProperty = s;
    SaveNewRomHeader("name", s);
  }
    if(itm.Key == "Creator") {
    s = ebr.Bars["Info"].DefaultProperty;
    if(s == LoadResString(63))s = ""; else s = Mid(s, Len(LoadResString(65)) + 1);
    s = InputBox(ref LoadResString(211), ref , ref s);
    if(s == "")goto hell;
    ebr.Bars["Info"].DefaultProperty = LoadResString(65) + s;
    SaveNewRomHeader("creator", s);
  }
    if(itm.Key == "Tagger") {
    s = ebr.Bars["Info"].DefaultProperty;
    if(s == LoadResString(64))s = ""; else s = Mid(s, Len(LoadResString(66)) + 1);
    s = InputBox(ref LoadResString(212), ref , ref s);
    if(s == "")goto hell;
    ebr.Bars["Info"].DefaultProperty = LoadResString(66) + s;
    SaveNewRomHeader("tagger", s);
  }
  hell:;
  cmdPlay.SetFocus();
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  int i = 0;
int j = 0;
IXMLDOMElement blah = null;
string regset = "";

  Trace("frmSappy/Form_Load()");
  Trace("- Set icon");
  SetIcon(hwnd, "APP", true);

   // Trace __S1
   // Dim y As Integer
   // Dim D As Integer
   // Dim m As Integer
   // Dim thresh As Integer
   // D = Val(Format(Now, __S1))
   // m = Val(Format(Now, __S1))
   // y = Val(Format(Now, __S1))
   // thresh = 2007
   // If (y > thresh) Or (y = thresh And D >= 25 And m = 12) Then
   // Trace __S1
   // ShellExecute 0, vbNullString, __S1, vbNullString, __S2, 1
   // End
   // End If

  Trace("- Set dimensions");
  ScaleMode = 1;
  cbxSongs.Height = 330;
  mywidth = Width / Screen.TwipsPerPixelX; // remember for wmSize subclass

  Trace("- Create duty cycle waves");
  DutyCycleWave[0] = String(14, Chr(0)) + String(2, Chr(255));
  DutyCycleWave[1] = String(12, Chr(0)) + String(4, Chr(255));
  DutyCycleWave[2] = String(16, Chr(0)) + String(16, Chr(255));
  DutyCycleWave[3] = String(4, Chr(0)) + String(12, Chr(255));

  Trace("- Create Sappy engine");
  SappyDecoder = new clsSappyDecoder();

  Trace("- Get settings");
  mnuSeekPlaylist.Checked = (GetSettingI("Seek by Playlist") == 1);
  mnuAutovance.Checked = (GetSettingI("AutoAdvance") == 1);
  regset = GetSetting(ref "Driver");
  if(Trim(regset) == "")regset = "FMOD";
  mnuOutput[0].DefaultProperty = (regset == "MIDI" ? true : false);
  mnuOutput[1].DefaultProperty = (regset == "FMOD" ? true : false);
  mnuGBMode.Checked = (GetSettingI("MIDI in GB Mode") == 1);
  i = GetSettingI(ref "Window Height");
  if(i > 0)Height = i;
  WantedMidiDevice = GetSettingI(ref "MIDI Device");
  i = GetSettingI(ref "FMOD Volume");
  if(i > 0)VolumeSlider1.SetValue(i);

  FullWidth = Width;
    if(LoadResString(10000) == "<NLPLZ>" || LoadResString(10000) == "<SPLZ>" || LoadResString(10000) == "<DPLZ>") {
    FullWidth = FullWidth + (16 * Screen.TwipsPerPixelX);
    ebr.Width = ebr.Width + (16 * Screen.TwipsPerPixelX);
  }
  ClassicWidth = FullWidth - ebr.Width - 10;
  HandleClassicMode();

  xfile = GetSetting(ref "XML File");
  if(Trim(xfile) == "")xfile = "sappy.xml";
  ChDir(AppContext.BaseDirectory);
    if(Dir(AppContext.BaseDirectory + "\\" + xfile) == "") {
    Trace("- Oh shit...");
      if(MsgBox(Replace(LoadResString(204), "$XML", xfile), vbYesNo) == vbYes) {
      VBOpenFile(xfile, "Output", 4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open xfile For Output As #4
      VBWriteFile("Print #4, "<sappy xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://helmetedrodent.kickassgamers.com/sappy.xsd\">""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #4, __S1
      VBWriteFile("Print #4, "</sappy>""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #4, __S1
      VBCloseFile(4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #4
      } else {
      End();
    }
  }
  xfile = AppContext.BaseDirectory + "\\" + xfile;

  Trace("- Localize");
  SetCaptions(ref this);
  Caption = LoadResString(0);
  lblPC.FontSize = 7;
  lblDel.FontSize = 7;
  lblNote.FontSize = 7;
  linProgress.x2 = -1;

  Trace("- Attach messages");
  AttachMessage(this, hwnd, WM_SIZING);
  AttachMessage(this, hwnd, WM_APPCOMMAND);
  AttachMessage(this, hwnd, WM_MOUSEWHEEL);

  Trace("- Load tool pics");
  StdPicture stdPic = new StdPicture(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  imlImages = new cVBALImageList();
  // TODO: (NOT SUPPORTED): With imlImages // vbalImages
  .OwnerHDC = this.hdc;
  .ColourDepth = ILC_COLOR8;
  .IconSizeX = 16;
  .IconSizeY = 16;
  .Create();

  stdPic = LoadResPicture("TOOLICONS", vbResBitmap);
  .AddFromHandle(stdPic.Handle, IMAGE_BITMAP, , 0xFF00FF);
  cmdPrevSong.ImageList = hIml;
  cmdNextSong.ImageList = hIml;
  cmdStop.ImageList = hIml;
  cmdPlay.ImageList = hIml;
  cmdPrevSong.Icon = 16;
  cmdNextSong.Icon = 17;
  cmdStop.Icon = 18;
  cmdPlay.Icon = 19;
   // cmdPrevSong.Picture = .ItemPicture(16)
   // cmdNextSong.Picture = .ItemPicture(17)
   // cmdStop.Picture = .ItemPicture(18)
   // cmdPlay.Picture = .ItemPicture(19)
  // TODO: (NOT SUPPORTED): End With
  Trace("- Load status pics");
  imlStatusbar = new cVBALImageList();
  // TODO: (NOT SUPPORTED): With imlStatusbar // vbalStatusBar
  .OwnerHDC = this.hdc;
  .ColourDepth = ILC_COLOR8;
  .IconSizeX = 16;
  .IconSizeY = 16;
  .Create();

  stdPic = LoadResPicture("STATUSICONS", vbResBitmap);
  .AddFromHandle(stdPic.Handle, IMAGE_BITMAP, , 0xFF00FF);
  // TODO: (NOT SUPPORTED): End With

  Trace("- Create status bar");
  cStatusBar = new cNoStatusBar();
  // TODO: (NOT SUPPORTED): With cStatusBar
  .Create(picStatusbar.Source);
  .ImageList = imlStatusbar.hIml;
  .AllowXPStyles = true;
  .Font = lblSong.Font;
  // TODO: (NOT SUPPORTED): End With
  FixStatusBar();

  Trace("- Create menu");
  // TODO: (NOT SUPPORTED): With cPop
  .SubClassMenu(this);
    if(GetSettingI(ref "Nice Menus") == 0) {
     // .HighlightStyle =
    .OfficeXpStyle = false;
    } else {
    .OfficeXpStyle = true;
  }

  Trace("- Set menu icons and help");
  .ImageList = imlImages.hIml;
  ItemIcon("mnuFileOpen") = 0;
  ItemIcon("mnuOutput(0)") = 2;
  ItemIcon("mnuOutput(1)") = 3;
  ItemIcon("mnuSeekPlaylist") = 4;
   // .ItemIcon(__S1) = 5
  ItemIcon("mnuGBMode") = 6;
  ItemIcon("mnuHelpHelp") = 7;
  ItemIcon("mnuHelpOnline") = 8;
  ItemIcon("mnuImportLST") = 20;
  ItemIcon("mnuSelectMIDI") = 22;
  ItemIcon("mnuSettings") = 24;
  ItemIcon("mnuMidiMap") = 23;
  HelpText("mnuFileOpen") = LoadResString(70);
  HelpText("mnuFileExit") = LoadResString(71);
  HelpText("mnuOutput(0)") = LoadResString(72);
  HelpText("mnuOutput(1)") = LoadResString(73);
  HelpText("mnuHelpAbout") = LoadResString(75);
  HelpText("mnuHelpOnline") = LoadResString(76);
  // TODO: (NOT SUPPORTED): End With

   // Not setting any images for Japanese systems until further notice.
    if(LoadResString(10000) != "<JAPPLZ>") {
    cbxSongs.ImageList = imlImages;
  }

  Trace("- Skinning");
  decimal hue = 0;
decimal sat = 0;
int skinno = 0;
  skinno = GetSettingI(ref "Skin");
   // If regset <> __S1 Then skinno = Val(regset) Else skinno = 0
  picSkin.Picture = LoadResPicture(100 + skinno, vbResBitmap);
  regset = GetSetting(ref "Skin Hue");
  if(regset != "")hue = Val(Replace(regset, ",", ".")); else hue = 3.5m;
  regset = GetSetting(ref "Skin Saturation");
  if(regset != "")sat = Val(Replace(regset, ",", ".")); else sat = 0.4m;
  Colorize(ref picSkin.Source, ref hue, ref sat);

   // Dim woogy As Control
   // For Each woogy In Me.Controls
   // If TypeName(woogy) = __S1 Then
   // woogy.MousePointer = 99
   // woogy.MouseIcon = LoadResPicture(__S1, vbResCursor)
   // End If
   // Next woogy

  Trace("- Set up task bar");
  // TODO: (NOT SUPPORTED): With ebr
  .BackColorStart = picSkin.point[6, 16].Source;
  .BackColorEnd = picSkin.point[6, 32].Source;
  .UseExplorerStyle = (GetSettingI(ref "Force Nice Bar") ? false : true);
  .Bars.Add("Tasks", LoadResString(50));
  .ImageList = imlImages.hIml;
  // TODO: (NOT SUPPORTED): With .Bars("Tasks")
  .CanExpand = false;
  .State = eBarCollapsed;
  Trace("- Add tasks");
  .Items.Add("taketrax", LoadResString(52), 9);
  .Items.Add("maketrax", LoadResString(53), 10);
  .Items.Add("takesamp", LoadResString(54), 11);
  .Items.Add("codetrax", LoadResString(55), 12);
  .Items.Add("makemidi", LoadResString(51), 2);
  Items("taketrax").ToolTipText = LoadResString(81);
  Items("maketrax").ToolTipText = LoadResString(82);
  Items("takesamp").ToolTipText = LoadResString(83);
  Items("codetrax").ToolTipText = LoadResString(84);
  Items("makemidi").ToolTipText = LoadResString(80);
    for (i = 1; i <= Items.count; i += 1) {
    TaskMenus(i) = cPop.MenuIndex["mnuTasks"].DefaultProperty;
  }
  .State = GetSettingI(ref "Bar " + Index + " state");
  // TODO: (NOT SUPPORTED): End With
  Trace("- Set up info bar");
  .Bars.Add("Info", LoadResString(60));
  // TODO: (NOT SUPPORTED): With .Bars("Info")
  .CanExpand = false;
  .State = eBarCollapsed;
  .Items.Add("Game", LoadResString(61), , 0);
  .Items.Add("Code", LoadResString(62), , 1);
  .Items.Add("Creator", LoadResString(63), , 0);
  .Items.Add("Tagger", LoadResString(64), , 0);
  .Items.Add("SongTbl", "0x000000", , 1);
  .Items.Add("Screen", , , 2);
  Items("Game").Bold = true;
  Items("SongTbl").SpacingAfter = 8;
  Items("Screen").Control = picScreenshot.Source;
  Items("Screen").SpacingAfter = 6;
  .State = GetSettingI(ref "Bar " + Index + " state");
  // TODO: (NOT SUPPORTED): End With
  // TODO: (NOT SUPPORTED): End With

  Trace("- Create channel views");
    for (i = 1; i <= 32; i += 1) {
    Load(cvwChannel[i].DefaultProperty);
    cvwChannel[i].DefaultProperty = cvwChannel[i - 1].DefaultProperty;
    cvwChannel[i].DefaultProperty = 0;
    cvwChannel[i].DefaultProperty = 0;
    cvwChannel[i].DefaultProperty = false;
  }

   // Trace __S1
   // Set x = New DOMDocument26
   // x.Load xfile
   // x.preserveWhiteSpace = True
   // If x.parseError.errorcode <> 0 Then
   // MsgBox Replace(Replace(LoadResString(208), __S1, x.parseError.reason), __S2, x.parseError.srcText)
   // End
   // End If
   // For Each blah In x.childNodes
   // If blah.baseName = __S1 Then
   // Set rootNode = blah
   // Exit For
   // End If
   // Next

  Trace("- Finalizing");
   // VolumeSlider1.SetValue 50

    if(midiOutGetNumDevs == 0) { // got no midi
    mnuOutput[0].DefaultProperty = false;
    mnuOutput[1].DefaultProperty = true;
  }

  picScreenshot.Picture = LoadResPicture("NOPIC", vbResBitmap);

  // TODO: (NOT SUPPORTED): On Error Resume Next
  Trace("- Handle startup rom");
  i = GetSettingI(ref "Reload ROM");
  // TODO: (NOT SUPPORTED): On Error GoTo 0
    if(Command != "") {
    myFile = Replace(Command, "\"", "");
    mnuFileOpen.Tag = "BrotherMaynard";
    Trace("- Gonna open (argument)");
    mnuFileOpen_Click();
    return;
  }

    if(i == 1) {
    myFile = Replace(GetSetting(ref "Last ROM"), "\"", "");
    if(Dir(myFile) == "")myFile = "";
      if(myFile != "") {
      mnuFileOpen.Tag = "BrotherMaynard";
      Trace("- Gonna open (reloader)");
      mnuFileOpen_Click();
    }
  }

  Trace("- Done loadin'");
   // frmMidiMapper.Show 1, Me
   // frmSelectMidiOut.Show 1
}

  private void Form_Resize() {
  if(WindowState == 1)return;
  picChannels.Height = ScaleHeight - picChannels.tOp - picStatusbar.Height;
}

  private void Form_Unload(ref int Cancel) {
  int i = 0;
  // TODO: (NOT SUPPORTED): On Error Resume Next
  Trace("- Form_Unload() ENGAGE!");
  Trace("- Calling SappyDecoder.StopSong");
  SappyDecoder.StopSong();
  Trace("- Calling ShutMSN");
  ShutMSN();
  Trace("- Terminating SappyDecoder");
  SappyDecoder = null;
  Trace("- Killing menu subclass");
  cPop.UnsubclassMenu();
  VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99
  Trace("- Saving window height");
  WriteSettingI(ref "Window Height", ref Height);
    if(GetSettingI(ref "mIRC Now Playing")) {
    Trace("- Updating mIRC information");
    VBOpenFile(App.Path, "&", "\\sappy.stt" For Output As #42); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open App.Path & __S1 For Output As #42
    VBWriteFile("Print #42, App.Major & "." & App.Minor & "| | | |Not running|""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #42, App.Major & __S1 & App.Minor & __S2
    VBCloseFile(42); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #42
  }
  Trace("- Detaching messages");
  DetachMessage(this, hwnd, WM_SIZING);
  DetachMessage(this, hwnd, WM_APPCOMMAND);
  DetachMessage(this, hwnd, WM_MOUSEWHEEL);
  // TODO: (NOT SUPPORTED): On Error GoTo 0
  Trace("- Killing forms");
    if(Forms.count > 1) {
    Form Form = null;
      foreach (var iterForm in Forms) {
Form = iterForm;
      Trace("- ..." + Form.name);
      Form.instance.Unload();
    }
  }
  Trace("- Will I dream?");
}

  private SSubTimer6.EMsgResponse ISubclass_MsgResponse{
get {
SSubTimer6.EMsgResponse _ISubclass_MsgResponse = default(SSubTimer6.EMsgResponse);
_ISubclass_MsgResponse = emrPostProcess;
return _ISubclass_MsgResponse;
}
set {
 // This property procedure must exist to properly implement
 // the Subclassing Assistant, even though it does nothing.
}
}
 // the Subclassing Assistant, even though it does nothing.


  private int ISubclass_WindowProc(int hwnd, int iMsg, int wParam, int lParam) {
int _ISubclass_WindowProc = 0;

    if(iMsg == WM_SIZING) {
    RECT myRect = null;
    CopyMemory(myRect, ByVal lParam, LenB(myRect)); // get the Rect pointed to in lParam
    myRect.Right = myRect.left + mywidth; // fix width
    if(myRect.Bottom - myRect.tOp < 280)myRect.Bottom = myRect.tOp + 280; // limit height
    CopyMemory(ByVal lParam, myRect, LenB(myRect)); // put our edited Rect back in lParam
  }

    if(iMsg == WM_APPCOMMAND) {
     // Okay... debug shows that the AppCommand's actual __S1 code is in the first byte.
     // Since our track control buttons don't go over 0xF, we can't go over 0xF0000...
      if(lParam <= 0xF0000) { // ...so first we ensure that part...
          switch(Val("&H" + left(Hex(lParam), 1))) { // ...then cut out and evaluate the first nibble.
          case APPCOMMAND_MEDIA_NEXTTRACK: cmdNextSong.Value = true;
          break;
case APPCOMMAND_MEDIA_PREVIOUSTRACK: cmdPrevSong.Value = true;
          break;
case APPCOMMAND_MEDIA_PLAY_PAUSE: cmdPlay.Value = true;
          break;
case APPCOMMAND_MEDIA_STOP: cmdStop.Value = true;
          break;
case APPCOMMAND_VOLUME_DOWN: VolumeSlider1.SetValue(VolumeSlider1.GetValue() - 5);
          break;
case APPCOMMAND_VOLUME_UP: VolumeSlider1.SetValue(VolumeSlider1.GetValue() + 5);
           // Case Else: Trace __S1 & Hex(lParam)
    break;
}
  }
   // TODO: Figure out how to __S1 the message, so pressing Play won't trigger background players.
}

  if(iMsg == WM_MOUSEWHEEL) {
    if(wParam < 0) {
    VolumeSlider1.SetValue(VolumeSlider1.GetValue() - 5);
    } else if(wParam > 0) {
    VolumeSlider1.SetValue(VolumeSlider1.GetValue() + 5);
  }
}

return _ISubclass_WindowProc;
}

private void lblExpand_Click(object sender, RoutedEventArgs e) { lblExpand_Click(); }
private void lblExpand_Click() {
  if(lblExpand.Content == "6") {
  lblExpand.Content = "5";
  } else {
  lblExpand.Content = "6";
}
int i = 0;
  for (i = 0; i <= cvwChannel.count - 1; i += 1) {
  cvwChannel(i).Expand (lblExpand.Caption = "5");
}
  for (i = 1; i <= cvwChannel.count - 1; i += 1) {
  cvwChannel[i].DefaultProperty = cvwChannel[i - 1].DefaultProperty;
}
}

private void mnuAutovance_Click(object sender, RoutedEventArgs e) { mnuAutovance_Click(); }
private void mnuAutovance_Click() {
mnuAutovance.Checked = ! mnuAutovance.Checked;
WriteSettingI(ref "AutoAdvance", ref mnuAutovance.Checked);
}

private void mnuFileExit_Click(object sender, RoutedEventArgs e) { mnuFileExit_Click(); }
private void mnuFileExit_Click() {
Unload();
}

private void mnuFileOpen_Click(object sender, RoutedEventArgs e) { mnuFileOpen_Click(); }
private void mnuFileOpen_Click() {
gCommonDialog cc = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
int i = 0;
string code = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (4)

if(mnuFileOpen.Tag == "BrotherMaynard")goto skipABit;
if(cc.VBGetOpenFileName(myFile, , , , , , LoadResString(2) + "|*.gba") == false)return;

skipABit:;
VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99

SongTbl = 0;
ebr.Bars["Tasks"].DefaultProperty = false;
 // ebr.Bars(__S1).State = eBarCollapsed
ebr.Bars["Info"].DefaultProperty = false;
 // ebr.Bars(__S1).State = eBarCollapsed
cbxSongs.IsEnabled = false;
cmdPrevSong.IsEnabled = false;
cmdNextSong.IsEnabled = false;
txtSong.IsEnabled = false;
chkMute.IsEnabled = false;
cmdPlay.IsEnabled = false;
cmdStop.IsEnabled = false;
  for (i = 1; i <= 5; i += 1) {
  cPop.IsEnabled[TaskMenus(i)].DefaultProperty = false;
}

VBOpenFile(myFile, "Binary", 99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open myFile For Binary As #99
Get(#99, 0xAC + 1, code);
  if(Asc(Mid(code, 1, 1)) == 0) {
  MsgBox(LoadResString(209));
  VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99
  return;
}
DontLoadDude = true;

string axe = "";
if(Dir(code + ".xml") != "")axe = code + ".xml";
if(Dir(AppContext.BaseDirectory + "\\" + code + ".xml") != "")axe = AppContext.BaseDirectory + "\\" + code + ".xml";
LoadGameFromXML(code, axe);

DontLoadDude = false;
gamecode = UCase(code);
  if(SongTbl == 0) {
    if(left(gamecode, 3) == "AGS" || left(gamecode, 3) == "AGF" || left(gamecode, 3) == "BMG") {
     // Autoscan don't like Golden Sun games :P
    MsgBox(LoadResString(110), vbExclamation);
    VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99
    return;
  }
    if(MsgBox(Replace(LoadResString(205), "$CODE", gamecode), vbOKCancel + vbInformation) == vbCancel) {
    mnuFileOpen.Tag = "";
    VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99
    return;
  }
  cStatusBar.PanelText("simple") = LoadResString(105);
  picStatusbar.Refresh();
  FindMST();
  cStatusBar.PanelText("simple") = "";
  picStatusbar.Refresh();
    if(SongTbl == 0) { // still?
    MsgBox(LoadResString(206));
    VBCloseFile(99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #99
    return;
  }
  MsgBox(Replace(LoadResString(207), "$TBL", Hex(SongTbl)));
  SaveBareBonesGameToXML();
  DontLoadDude = true;
  LoadGameFromXML(code);
  DontLoadDude = false;
}

// TODO: (NOT SUPPORTED): On Error Resume Next // until we get the translations
if(axe != "")cStatusBar.PanelText("simple") = LoadResString(111);
// TODO: (NOT SUPPORTED): On Error GoTo 0

ebr.Bars["Tasks"].DefaultProperty = true;
 // ebr.Bars(__S1).State = eBarExpanded
ebr.Bars["Info"].DefaultProperty = true;
 // ebr.Bars(__S1).State = eBarExpanded
cbxSongs.IsEnabled = true;
cmdPrevSong.IsEnabled = true;
cmdNextSong.IsEnabled = true;
txtSong.IsEnabled = true;
chkMute.IsEnabled = true;
cmdPlay.IsEnabled = true;
cmdStop.IsEnabled = true;
txtSong.Text = playlist[0].SongNo(0);
LoadSong(txtSong.Text);
  for (i = 1; i <= 5; i += 1) {
  cPop.IsEnabled[TaskMenus(i)].DefaultProperty = true;
}

mnuFileOpen.Tag = "";

// TODO: (NOT SUPPORTED): On Error Resume Next
  if(GetSettingI(ref "Reload ROM")) {
  WriteSetting(ref "Last ROM", ref myFile);
}

DontLoadDude = false;
// TODO: (NOT SUPPORTED): On Error GoTo 0
}

public void LoadSong(ref int i) {
int l = 0;
int k = 0;
int m = 0;
string n = "";

// TODO: (NOT SUPPORTED): On Error GoTo hell

Get(#99, SongTbl + (i * 8) + 1, l);
l = l - 0x8000000;
SongHeadOrg = l;
Get(#99, l + 1, SongHead);

  for (k = 0; k <= 32; k += 1) {
  cvwChannel[k].DefaultProperty = false;
}

  for (k = 0; k <= SongHead.NumTracks - 1; k += 1) {
  cvwChannel[k].DefaultProperty = true;
  cvwChannel[k].DefaultProperty = SongHead.Tracks(k) - 0x8000000;
  cvwChannel[k].DefaultProperty = "...";
  cvwChannel[k].DefaultProperty = 0;
  cvwChannel[k].DefaultProperty = 0;
  cvwChannel[k].DefaultProperty = 0;
  cvwChannel[k].DefaultProperty = 0;
  cvwChannel[k].DefaultProperty = 0;
  cvwChannel[k].DefaultProperty = 0;
}

lblDef.Content = "0x" + FixHex(ref SongTbl + (i * 8), ref 6);
lblLoc.Content = "0x" + FixHex(ref SongHeadOrg, ref 6);
lblInst.Content = "0x" + FixHex(ref SongHead.VoiceGroup - 0x8000000, ref 6);

n = "?";
lblSongName.Content = Replace(LoadResString(106), "$INDEX", i);
justthesongname = "Track " + i;
  for (k = 0; k <= NumPLs; k += 1) {
    for (l = 0; l <= playlist[k].NumSongs; l += 1) {
      if(playlist[k].SongNo(l) == i) {
      DontLoadDude = true;
      n = playlist[k].SongName(l);
      justthesongname = n;
      lblSongName.Content = Replace(Replace(LoadResString(107), "$NAME", n), "$INDEX", i);
        for (m = 0; m <= cbxSongs.ListCount - 1; m += 1) {
         // If cbxSongs.List(m) = playlist(k).SongName(l) Then
          if(cbxSongs.ItemData[m].DefaultProperty) {
          cbxSongs.SelectedIndex = m;
          DontLoadDude = false;
          goto ExitForGood;
        }
      }
      DontLoadDude = false;
    }
  }
}
ExitForGood:;

 // Do mIRC string...
gCommonDialog cc = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
songinfo = ebr.Bars["Info"].DefaultProperty;

  if(Playing == true) {
  cmdPlay_Click();
}

hell:;
}

public void LoadGameFromXML(ref string gamecode, string newxfile = "") {
IXMLDOMElement n1 = null;
IXMLDOMElement n2 = null;
IXMLDOMAttribute n3 = null;
IXMLDOMElement n4 = null;
IXMLDOMElement s1 = null;
IXMLDOMElement s2 = null;
IXMLDOMAttribute s3 = null;
IXMLDOMElement s4 = null;
int i = 0;
int j = 0;
int Icon = 0;
int picon = 0;

if(newxfile == "")newxfile = xfile;
Trace("Loading from " + newxfile + "...");
x = new DOMDocument26();
x.Load(newxfile);
x.preserveWhiteSpace = true;
  if(x.parseError.errorcode != 0) {
  MsgBox(Replace(Replace(LoadResString(208), "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
  End();
}
  foreach (var itern1 in x.childNodes) {
n1 = itern1;
    if(n1.baseName == "sappy") {
    rootNode = n1;
    break;
  }
}

// TODO: (NOT SUPPORTED): On Error Resume Next
ebr.Bars["Info"].DefaultProperty = LoadResString(61);
ebr.Bars["Info"].DefaultProperty = LoadResString(62);
ebr.Bars["Info"].DefaultProperty = LoadResString(63);
ebr.Bars["Info"].DefaultProperty = LoadResString(64);
ebr.Bars["Info"].DefaultProperty = "0x000000";
 // Set picScreenshot.Picture = Nothing
picScreenshot.Picture = LoadResPicture("NOPIC", vbResBitmap);
cbxSongs.Clear();
  for (j = 0; j <= 255; j += 1) {
    for (i = 0; i <= 1024; i += 1) {
    playlist[j].SongName(i) = "";
    playlist[j].SongNo(i) = 0;
  }
  playlist[0].NumSongs = 0;
}
playlist[0].NumSongs = 1;
playlist[0].SongName(0) = LoadResString(109);
playlist[0].SongNo(0) = 1;
NumPLs = 1;
  for (i = 0; i <= 127; i += 1) {
  MidiMap[i] = i;
  MidiMapTrans[i] = 0;
  DrumMap[i] = i;
  BleedingEars[i] = -1;
}
BECnt = 0;
MidiMapNode = null;
MidiMapsDaddy = null;
  foreach (var itern1 in rootNode.childNodes) {
n1 = itern1;
    if(n1.baseName == "rom") {
    NumPLs = 0;
      foreach (var itern3 in n1.Attributes) {
n3 = itern3;
        if(n3.baseName == "code") {
          if(LCase(n3.Value) != LCase(gamecode)) {
          goto BrotherMaynard;
        }
        ebr.Bars["Info"].DefaultProperty = "Gamecode " + UCase(n3.Text);
        gamecode = UCase(n3.Text);
      }
        if(n3.baseName == "name") {
        ebr.Bars["Info"].DefaultProperty = n3.Text;
      }
        if(n3.baseName == "creator") {
        ebr.Bars["Info"].DefaultProperty = LoadResString(65) + n3.Text;
      }
        if(n3.baseName == "tagger") {
        ebr.Bars["Info"].DefaultProperty = LoadResString(66) + n3.Text;
      }
        if(n3.baseName == "songtable") {
        SongTbl = Val("&H" + FixHex(ref n3.Text, ref 6));
          if(Val("&H" + FixHex(ref n3.Text, ref 6)) != Val("&H" + FixHex(ref n3.Text, ref 6) + "&")) {
          MsgBox("Song pointer in an unsupported location. " + Hex(Val("&H" + FixHex(ref n3.Text, ref 6) + "&")) + " is read as " + Hex(Val("&H" + FixHex(ref n3.Text, ref 6))) + ".");
          return;
        }
        ebr.Bars["Info"].DefaultProperty = LoadResString(67) + "0x" + Hex(SongTbl);
      }
        if(n3.baseName == "screenshot") {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        picScreenshot.Tag = n3.Value;
        picScreenshot.Picture = LoadPicture(n3.Value);
        // TODO: (NOT SUPPORTED): On Error GoTo 0
      }
    }

    // TODO: (NOT SUPPORTED): On Error GoTo BrotherMaynard
    MidiMapsDaddy = n1;
      foreach (var itern2 in n1.childNodes) {
n2 = itern2;
        if(n2.baseName == "playlist") {
          if(n2.getAttribute("steal") != "") {
          cbxSongs.AddItemAndData(n2.getAttribute("name"), 13, 13, 9999);
          playlist[NumPLs].NumSongs = 0;
            foreach (var iters1 in rootNode.childNodes) {
s1 = iters1;
              if(s1.baseName == "rom" && s1.getAttribute("code") == n2.getAttribute("steal")) {
                foreach (var iters2 in s1.childNodes) {
s2 = iters2;
                  if(s2.baseName == "playlist" && s2.getAttribute("name") == n2.getAttribute("name")) {
                    foreach (var iters4 in s2.childNodes) {
s4 = iters4;
                      if(s4.baseName == "song") {
                      playlist[NumPLs].SongName(playlist[NumPLs].NumSongs) = s4.Text;
                      playlist[NumPLs].SongNo(playlist[NumPLs].NumSongs) = Val("&H" + FixHex(ref s4.getAttribute("track"), ref 4));
                      playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
                      cbxSongs.AddItemAndData(s4.Text, 14, 14, Val("&H" + FixHex(ref s4.getAttribute("track"), ref 4)), 1);
                    } // stealing song
                  } // stealing playlist children
                  goto StolenIt;
                } // stealing playlist
              } // stealing rom children
              MsgBox("Couldn't find playlist \"" + n2.getAttribute("name") + "\" for gamecode \"" + n2.getAttribute("steal") + "\".");
            } // stealing rom
          } // stealing library
          NumPLs = NumPLs + 1;
          } else {
          cbxSongs.AddItemAndData(n2.getAttribute("name"), 13, 13, 9999);
          picon = 14;
          if(n2.getAttribute("icon") == "1")picon = 25;
          playlist[NumPLs].NumSongs = 0;
            foreach (var itern4 in n2.childNodes) {
n4 = itern4;
              if(n4.baseName == "song") {
              Icon = picon;
              if(n4.getAttribute("icon") == "0")Icon = 14;
              if(n4.getAttribute("icon") == "1")Icon = 25;
              playlist[NumPLs].SongName(playlist[NumPLs].NumSongs) = n4.Text;
              playlist[NumPLs].SongNo(playlist[NumPLs].NumSongs) = Val("&H" + FixHex(ref n4.getAttribute("track"), ref 4));
              playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
              cbxSongs.AddItemAndData(n4.Text, Icon, Icon, Val("&H" + FixHex(ref n4.getAttribute("track"), ref 4)), 1);
            } // song
          } // playlist songs
          NumPLs = NumPLs + 1;
        } // stealing check
      } // playlist
      // TODO: (NOT SUPPORTED): On Error Resume Next

      StolenIt:;
       // We could get other tags here, like MidiMap.
        if(n2.baseName == "midimap") {
        MidiMapNode = n2;
          foreach (var itern4 in n2.childNodes) {
n4 = itern4;
            if(n4.baseName == "inst") {
            i = n4.getAttribute("from");
            MidiMap[i] = n4.getAttribute("to");
            // TODO: (NOT SUPPORTED): On Error Resume Next
            MidiMapTrans[i] = n4.getAttribute("transpose");
            // TODO: (NOT SUPPORTED): On Error GoTo 0
          } // inst
        } // midimap children
      } // midimap

        if(n2.baseName == "bleedingears") {
          foreach (var itern4 in n2.childNodes) {
n4 = itern4;
            if(n4.baseName == "inst") {
              foreach (var itern3 in n4.Attributes) {
n3 = itern3;
                if(n3.baseName == "id") {
                BleedingEars[BECnt] = n3.Value;
                BECnt = BECnt + 1;
              }
                if(n3.baseName == "from") {
                  for (i = n3.Value; i <= n4.getAttribute("to"); i += 1) {
                  BleedingEars[BECnt] = i;
                  BECnt = BECnt + 1;
                }
              }
            }
             // i = n4.getAttribute(__S1)
             // BleedingEars(BECnt) = i
             // BECnt = BECnt + 1
          } // inst
        } // bleedingears children
      } // bleedingears

    } // rom children
    // TODO: (NOT SUPPORTED): On Error GoTo 0
  }
  BrotherMaynard:;
}
if(cbxSongs.ListCount == 0)cbxSongs.AddItemAndData("No songs defined", 1, 1, 1);
cbxSongs.SelectedIndex = 0;
}

private void SaveBareBonesGameToXML() {
IXMLDOMElement n1 = null;
IXMLDOMComment n2 = null;
IXMLDOMElement n3 = null;
IXMLDOMAttribute n4 = null;
string gamename = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (12)
Get(#99, 0xA1, gamename);
n1 = x.createElement("rom");

n1.setAttribute("code", gamecode);
n1.setAttribute("name", gamename);
n1.setAttribute("songtable", "0x" + Hex(SongTbl)); // FixHex(SongTbl, 6)

n3 = x.createElement("playlist");
n4 = x.createAttribute("name");
n4.Text = "Main";
n3.Attributes.setNamedItem(n4);
n1.appendChild(n3);

rootNode.insertBefore(n1, Null);
x.save(xfile);
}

private void SaveNewRomHeader(ref string att, ref string nV) {
IXMLDOMElement n1 = null;
string axe = "";

axe = xfile;
if(Dir(gamecode + ".xml") != "")axe = gamecode + ".xml";
if(Dir(AppContext.BaseDirectory + "\\" + gamecode + ".xml") != "")axe = AppContext.BaseDirectory + "\\" + gamecode + ".xml";
Trace("Saving to " + axe + "...");
x = new DOMDocument26();
x.Load(axe);
x.preserveWhiteSpace = true;
  if(x.parseError.errorcode != 0) {
  MsgBox(Replace(Replace(LoadResString(208), "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
  End();
}
  foreach (var itern1 in x.childNodes) {
n1 = itern1;
    if(n1.baseName == "sappy") {
    rootNode = n1;
    break;
  }
}


  foreach (var itern1 in rootNode.childNodes) {
n1 = itern1;
    if(n1.baseName == "rom" && n1.getAttribute("code") == gamecode) {
    n1.setAttribute(att, nV);
  }
}
x.save(axe);
}

private void FindMST() {
 // Thumbcode to find:
 // 400B 4018 8388 5900 C918 8900 8918 0A68 0168 101C 00F0 ---- 01BC 0047 MPlayTBL SongTBL
int anArm = 0;
int aPointer = 0;
int off = 0;
int match = 0;
MousePointer = 11;
Seek(#99, 1);
  do {
    if(Seek(99) % &H10000 == 1) {
    cStatusBar.PanelText("frame") = "0x" + Hex(Seek(99) - 1);
    picStatusbar.Refresh();
  }
  Get(#99, , anArm);
    if(match == 0) {
    if(anArm == 0x18400B40)match = 1; else match = 0;
    } else if(match == 1) {
    if(anArm == 0x598883)match = 2; else match = 0;
    } else if(match == 2) {
    if(anArm == 0x8918C9)match = 3; else match = 0;
    } else if(match == 3) {
    if(anArm == 0x680A1889)match = 4; else match = 0;
    } else if(match == 4) {
    if(anArm == 0x1C106801)match = 5; else match = 0;
    } else if(match == 5) {
     // skip over the jump
    match = 6;
    } else if(match == 6) {
      if(anArm == 0x4700BC01) {
      match = 7;
      off = Seek(99);
      } else {
      match = 0;
    }
  }
    if(match == 7) { // mPlayTBL
    Seek(#99, off);
    Get(#99, , aPointer);
    Get(#99, , aPointer);
    SongTbl = aPointer - 0x8000000;
    MousePointer = 0;
    return;
  }
  DoEvents();
} while(!(EOF(99)));
MousePointer = 0;
}

private void mnuGBMode_Click(object sender, RoutedEventArgs e) { mnuGBMode_Click(); }
private void mnuGBMode_Click() {
mnuGBMode.Checked = ! mnuGBMode.Checked;
WriteSettingI(ref "MIDI In GB Mode", ref Abs(mnuGBMode.Checked));
}

private void mnuHelpAbout_Click(object sender, RoutedEventArgs e) { mnuHelpAbout_Click(); }
private void mnuHelpAbout_Click() {
frmAbout.instance.ShowDialog();
}

private void mnuHelpHelp_Click(object sender, RoutedEventArgs e) { mnuHelpHelp_Click(); }
private void mnuHelpHelp_Click() {
ShellExecute(0, vbNullString, AppContext.BaseDirectory + "\\sappy.chm", vbNullString, "", 1);
}

private void mnuHelpOnline_Click(object sender, RoutedEventArgs e) { mnuHelpOnline_Click(); }
private void mnuHelpOnline_Click() {
ShellExecute(0, vbNullString, "http://helmetedrodent.kickassgamers.com", vbNullString, "", 1);
}

private void mnuImportLST_Click(object sender, RoutedEventArgs e) { mnuImportLST_Click(); }
private void mnuImportLST_Click() {
gCommonDialog cc = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
string myFile = "";
string myDir = "";
string c = "";
string n = "";
string e = "";
string p = "";
string m = "";
string s = "";
string F = "";
string l = "";
string y = "";
IXMLDOMElement myNewRom = null;
IXMLDOMElement myNewList = null;
IXMLDOMElement myNewSong = null;
IXMLDOMElement oldRom = null;
VbMsgBoxResult blah = null;

if(cc.VBGetOpenFileName(myFile, , , , , , "Sappy.LST|sappy.lst") == false)return;
myDir = left(myFile, Len(myFile) - Len(cc.VBGetFileTitle(myFile)));

x.save(left(xfile, Len(xfile) - 3) + "bak");

VBOpenFile(myFile, "Input", 96); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open myFile For Input As #96
  do {
  Input(#96, c); // code
  if(c == "ENDFILE")break;
  Input(#96, n); // name
  Input(#96, e); // engine
  Input(#96, p); // playlist
  Input(#96, m); // map
  Input(#96, s); // songlist
  Input(#96, F); // first
  Input(#96, l); // last
    if(c != "****" && Right(c, 1) != "ÿ") {
      if(e == "sapphire") {
      myNewRom = x.createElement("rom");
      myNewRom.setAttribute("code", c); // 181205 update: OOPS!
      myNewRom.setAttribute("name", n);
      myNewRom.setAttribute("songtable", "0x" + Hex(s) + "");
        if(p == "blank") {
        myNewList = x.createElement("playlist");
        myNewList.setAttribute("name", "No playlist");
        myNewRom.appendChild(myNewList);
        } else {
          if(Dir(myDir + p + ".lst") == "") {
          myNewList = x.createElement("playlist");
          myNewList.setAttribute("name", "404");
          myNewRom.appendChild(myNewList);
          } else {
          VBOpenFile(myDir, "&", p & ".lst" For Input As #95); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open myDir & p & __S1 For Input As #95
            do {
            Line(Input #95, y);
            if(y == "ENDFILE")break;
            myNewList = x.createElement("playlist");
            myNewList.setAttribute("name", y);
              do {
              Line(Input #95, y);
                if(y != "ENDFILE") {
                  if(y != "END") {
                  myNewSong = x.createElement("song");
                  myNewSong.setAttribute("track", Val("&H" + left(y, 4)));
                  myNewSong.Text = Mid(y, 6);
                  myNewList.appendChild(myNewSong);
                }
              }
            } while(!(y == "END"));
            myNewRom.appendChild(myNewList);
          }
          VBCloseFile(95); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #95
        }
      }
      rootNode.appendChild(myNewRom);
    }
  }
  SkipThisShit:;
}
x.save(xfile);
VBCloseFile(96); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #96
IncessantNoises(ref "TaskComplete");

LoadGameFromXML(gamecode);
}

private void mnuMidiMap_Click(object sender, RoutedEventArgs e) { mnuMidiMap_Click(); }
private void mnuMidiMap_Click() {
frmMidiMapper.instance.ShowDialog();
}

private void mnuOutput_Click(object sender, RoutedEventArgs e) { mnuOutput_Click(); }
private void mnuOutput_Click(ref int Index) {
  if(Index == 0) {
  mnuOutput[0].DefaultProperty = true;
  mnuOutput[1].DefaultProperty = false;
  WriteSetting(ref "Driver", ref "MIDI");
  } else {
  mnuOutput[0].DefaultProperty = false;
  mnuOutput[1].DefaultProperty = true;
  WriteSetting(ref "Driver", ref "FMOD");
}
mnuGBMode.IsEnabled = mnuOutput[0].DefaultProperty;
}

private void mnuSeekPlaylist_Click(object sender, RoutedEventArgs e) { mnuSeekPlaylist_Click(); }
private void mnuSeekPlaylist_Click() {
mnuSeekPlaylist.Checked = ! mnuSeekPlaylist.Checked;
WriteSettingI(ref "Seek by Playlist", ref Abs(mnuSeekPlaylist.Checked));
}

private void mnuSelectMIDI_Click(object sender, RoutedEventArgs e) { mnuSelectMIDI_Click(); }
private void mnuSelectMIDI_Click() {
frmSelectMidiOut.instance.ShowDialog();
}

private void mnuSettings_Click(object sender, RoutedEventArgs e) { mnuSettings_Click(); }
private void mnuSettings_Click() {
frmOptions.instance.ShowDialog();
}

private void picChannels_Paint() {
 // StretchBlt picChannels.hdc, 0, 0, picChannels.ScaleWidth, 17, picSkin.hdc, 6, 16, 2, 17, vbSrcCopy
}

private void picScreenshot_DblClick(object sender, RoutedEventArgs e) { picScreenshot_DblClick(); }
private void picScreenshot_DblClick() {
gCommonDialog cc = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
string s = "";
s = picScreenshot.Tag;
  if(cc.VBGetOpenFileName(s, , , , , , LoadResString(1) + "|*.BMP;*.GIF;*.JPG") == true) {
  s = cc.VBGetFileTitle(s);
  picScreenshot.Picture = LoadPicture(s);
  picScreenshot.Tag = s;
  SaveNewRomHeader("screenshot", s);
}
}

private void picStatusbar_DblClick(object sender, RoutedEventArgs e) { picStatusbar_DblClick(); }
private void picStatusbar_DblClick() {
  if(picStatusbar.Tag >= 402 && picStatusbar.Tag <= 436) {
  frmOptions.Tag = "repsplz";
  frmOptions.instance.ShowDialog();
}
}

private void picStatusbar_MouseMove(ref int Button, ref int Shift, ref decimal x, ref decimal y) {
picStatusbar.Tag = x + IIf(ebr.Visibility == false, ebr.Width, 0);
}

private void picStatusBar_Paint() {
cStatusBar.Draw();
}

private void picTop_Paint() {
RedrawSkin();
}

private void SappyDecoder_Beat(int Beats) {
 // If mnuOutput(0).Checked Then ToneOn 9, 37, 64
}

private void SappyDecoder_ChangedTempo(int newtempo) {
lblSpeed.Content = SappyDecoder.Tempo;
}

private void SappyDecoder_Loading(int status) {
cStatusBar.PanelText("simple") = LoadResString(8000 + status);
 // If status = 1 Then cStatusBar.PanelText(__S1) = cStatusBar.PanelText(__S2) & __S3 & progress & __S4 & total & __S5
picStatusbar.Refresh();
}

private void SappyDecoder_SongFinish() {
Playing = false;
timPlay.IsEnabled = false;
cmdPlay.Icon = 19;
mnuOutput[0].DefaultProperty = true;
mnuOutput[1].DefaultProperty = true;
mnuGBMode.IsEnabled = mnuOutput[0].DefaultProperty;
linProgress.x2 = -1;
ShutMSN();
}

private void SappyDecoder_SongLoop() {
if(loopsToGo == 0)return;
loopsToGo = loopsToGo - 1;
if(loopsToGo == 0)cmdStop_Click();
}

private void SappyDecoder_UpdateDisplay() {
 // Some of this from Drew's Sappy 2 interface.
int c = 0;
int ct = 0;
string ns = "";
string it = "";
SNote n = null;

  for (c = 1; c <= SappyDecoder.SappyChannels.count; c += 1) {
  // TODO: (NOT SUPPORTED): With SappyDecoder.SappyChannels(c)
  ct = 0;
  ns = "";
    if(Notes.count > 0) {
      foreach (var itern in Notes) {
n = itern;
        if(SappyDecoder.GetNoteInfo(n.NoteID).Enabled == true && SappyDecoder.GetNoteInfo(n.NoteID).NoteOff == false) {
        ct = ct + (((SappyDecoder.GetNoteInfo(n.NoteID).Velocity / 0x7F))) * (SappyDecoder.GetNoteInfo(n.NoteID).EnvPosition / 0xFF) * 0x7F;
        ns = ns + NoteToName(SappyDecoder.GetNoteInfo(n.NoteID).NoteNumber) + " ";
      }
          switch(SappyDecoder.GetNoteInfo(n.NoteID).outputtype) {
          case notDirect: it = "Direct";
          break;
case notNoise: it = "Noise";
          break;
case notSquare1: it = "Square1";
          break;
case notSquare2: it = "Square2";
          break;
case notWave: it = "Wave";
          break;
default: it = "";
    break;
}
  }
  ct = ct / Notes.count;
}
ct = ((ct / 127) * (MainVolume / 127)) * 255;
cvwChannel(c - 1).Delay = .WaitTicks;
cvwChannel(c - 1).volume = ct;
cvwChannel(c - 1).pan = .Panning - 64;
cvwChannel(c - 1).patch = .PatchNumber;
cvwChannel(c - 1).Location = .TrackPointer + .ProgramCounter;
if(ns != "")cvwChannel(c - 1).Note = ns;
cvwChannel(c - 1).iType = it;

// TODO: (NOT SUPPORTED): End With
}

int totallen = 0;
int totalplayed = 0;
int totalpercent = 0;
for (c = 1; c <= SappyDecoder.SappyChannels.count; c += 1) {
// TODO: (NOT SUPPORTED): With SappyDecoder.SappyChannels(c)
totallen = totallen + TrackLengthInBytes;
totalplayed = totalplayed + ProgramCounter;
// TODO: (NOT SUPPORTED): End With
}
totalpercent = (326 / totallen) * totalplayed;
linProgress.x2 = totalpercent;
 // Caption = totalplayed & __S1 & totallen & __S2 & totalpercent & __S3
 // Dim totalplayed As Long
 // With SappyDecoder.SappyChannels(1)
 // tl = .TrackLengthInBytes - (.ProgramCounter + .TrackPointer)
 // Caption = tl
 // End With

cStatusBar.PanelText("crud") = loopsToGo;
cStatusBar.PanelText("time") = Right("00" + TotalMinutes, 2) + ":" + Right("00" + TotalSeconds, 2) + " (" + SappyDecoder.Beats + ")";
cStatusBar.PanelText("frame") = SappyDecoder.TotalTicks;

 // If SappyDecoder.TotalTicks < 96 Then Debug.Print (96 - SappyDecoder.SappyChannels(2).WaitTicks) & __S1 & SappyDecoder.TotalTicks
picStatusbar.Refresh();
}

private void timPlay_Timer() {
TotalSeconds = TotalSeconds + 1;
if(TotalSeconds == 60) {
TotalMinutes = TotalMinutes + 1;
TotalSeconds = 0;
}
}

private void txtSong_LostFocus(object sender, RoutedEventArgs e) { txtSong_LostFocus(); }
private void txtSong_LostFocus() {
LoadSong(Val(txtSong.Text));
}

private void VolumeSlider1_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { VolumeSlider1_Change(); }
private void VolumeSlider1_Change(ref int NewValue) {
decimal VolumeScalar = 0;
VolumeScalar = 5.1m;
SappyDecoder.GlobalVolume = NewValue * VolumeScalar;
WriteSettingI(ref "FMOD Volume", ref NewValue);
}

public void RedrawSkin() {
RECT panelRect = null;
ScaleMode = 3;
// TODO: (NOT SUPPORTED): With panelRect
.left = 0;
.tOp = 0;
.Right = picTop.Width;
.Bottom = picTop.Height;
BitBlt(picTop.hdc, left, tOp, 2, 2, picSkin.hdc, 6, 0, vbSrcCopy);
StretchBlt(picTop.hdc, left + 2, tOp, Right - 4, 2, picSkin.hdc, 6, 2, 2, 2, vbSrcCopy);
BitBlt(picTop.hdc, left + Right - 2, tOp, 2, 2, picSkin.hdc, 6, 4, vbSrcCopy);
StretchBlt(picTop.hdc, left, tOp + 2, 2, Bottom - 4, picSkin.hdc, 6, 6, 2, 2, vbSrcCopy);
StretchBlt(picTop.hdc, left + 2, tOp + 2, Right - 4, Bottom - 4, picSkin.hdc, 0, 0, 6, 62, vbSrcCopy);
StretchBlt(picTop.hdc, left + Right - 2, tOp + 2, 2, Bottom - 4, picSkin.hdc, 6, 8, 2, 2, vbSrcCopy);
BitBlt(picTop.hdc, left, tOp + Bottom - 2, 2, 2, picSkin.hdc, 6, 10, vbSrcCopy);
StretchBlt(picTop.hdc, left + 2, tOp + Bottom - 2, Right - 4, 2, picSkin.hdc, 6, 12, 2, 2, vbSrcCopy);
BitBlt(picTop.hdc, left + Right - 2, tOp + Bottom - 2, 2, 2, picSkin.hdc, 6, 14, vbSrcCopy);
// TODO: (NOT SUPPORTED): End With
// TODO: (NOT SUPPORTED): On Error Resume Next
VolumeSlider1.BackColor = GetPixel(picSkin.hdc, 5, 42);
}

public void HandleClassicMode() {
if(GetSettingI(ref "Hide Bar")) {
ebr.Visibility = false;
Width = ClassicWidth;
ScaleMode = 3;
mywidth = Width / Screen.TwipsPerPixelX; // remember for wmSize subclass
picTop.Move(0);
picChannels.Move(0);
ScaleMode = 1;
cbxSongs.Height = 330;
} else {
ebr.Visibility = true;
Width = FullWidth;
ScaleMode = 3;
mywidth = Width / Screen.TwipsPerPixelX; // remember for wmSize subclass
picTop.Move(ebr.Width);
picChannels.Move(ebr.Width);
ScaleMode = 1;
cbxSongs.Height = 330;
}
}

public void FixStatusBar() {
// TODO: (NOT SUPPORTED): With cStatusBar
// TODO: (NOT SUPPORTED): On Error Resume Next
.RemovePanel("simple");
.RemovePanel("frame");
.RemovePanel("crud");
.RemovePanel("time");
// TODO: (NOT SUPPORTED): On Error GoTo 0
.AddPanel(estbrNoBorders, "", , , true, , , "simple");
.AddPanel(estbrStandard, "0", , , false, , , "frame");
.AddPanel(estbrStandard, "0", 0, 24, false, , , "crud");
.AddPanel(estbrStandard, "00:00 (0)", 1, 64, false, , , "time");
// TODO: (NOT SUPPORTED): End With
picStatusbar.Refresh();
}

private void PrepareRecording() {
string target = "";
gCommonDialog cc = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
if(cc.VBGetSaveFileName(target, lblSongName.Content + ".mid", , "Type 0 MIDI (*.mid)|*.mid", , , , "mid") == false)return;
WantToRecord = 1;
WantToRecordTo = target;
cmdPlay.Value = true;

 // Set HookedDialog = New cCommonDialog
 // With HookedDialog
 // .CancelError = False
 // .DefaultExt = __S1
 // .DialogTitle = LoadResString(51)
 // .Filter = __S1
 // .Filename = __S1 & txtSong & __S2
 // .flags = EOpenFile.OFN_EXPLORER Or EOpenFile.OFN_NOCHANGEDIR
 // .hwnd = Me.hwnd
 // .HookDialog = True
 // If InIDE() Then
 // .cdLoadLibrary App.Path & __S1
 // Else
 // .hInstance = App.hInstance
 // End If
 // .TemplateName = 42
 // .ShowSave
 // If InIDE() Then .cdFreeLibrary
 // If .Filename = __S1 Then Exit Sub
 // WantToRecordTo = .Filename
 // End With
 // WantToRecord = 1
 // cmdPlay.value = True
}

 // Private Property Get InIDE() As Boolean
 // ' debug.assert doesn't go when compiled:
 // Debug.Assert (InIDESub())
 // ' so this will return false
 // InIDE = m_bInIDE
 // End Property
 // Private Property Get InIDESub() As Boolean
 // m_bInIDE = True
 // InIDESub = True
 // End Property


}
}