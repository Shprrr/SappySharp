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
public partial class frmOptions : Window {
  private static frmOptions _instance;
  public static frmOptions instance { set { _instance = null; } get { return _instance ?? (_instance = new frmOptions()); }}  public static void Load() { if (_instance == null) { dynamic A = frmOptions.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmOptions() { InitializeComponent(); }


public List<Window> frmOptions { get => VBExtension.controlArray<Window>(this, "frmOptions"); }

public List<Image> picPage { get => VBExtension.controlArray<Image>(this, "picPage"); }

public List<Label> cbxPresets { get => VBExtension.controlArray<Label>(this, "cbxPresets"); }

public List<Image> picSkin { get => VBExtension.controlArray<Image>(this, "picSkin"); }

public List<Image> picSkinLoad { get => VBExtension.controlArray<Image>(this, "picSkinLoad"); }

public List<ScrollBar> HScroll1 { get => VBExtension.controlArray<ScrollBar>(this, "HScroll1"); }

public List<ScrollBar> HScroll2 { get => VBExtension.controlArray<ScrollBar>(this, "HScroll2"); }

public List<Image> Picture5 { get => VBExtension.controlArray<Image>(this, "Picture5"); }

public List<Label> Label4 { get => VBExtension.controlArray<Label>(this, "Label4"); }

public List<Shape> shpSkinSel { get => VBExtension.controlArray<Shape>(this, "shpSkinSel"); }

public List<Label> Label7 { get => VBExtension.controlArray<Label>(this, "Label7"); }

public List<Label> Label8 { get => VBExtension.controlArray<Label>(this, "Label8"); }

public List<Label> Label10 { get => VBExtension.controlArray<Label>(this, "Label10"); }

public List<Label> Label11 { get => VBExtension.controlArray<Label>(this, "Label11"); }

public List<Image> Picture3 { get => VBExtension.controlArray<Image>(this, "Picture3"); }

public List<Image> Image1 { get => VBExtension.controlArray<Image>(this, "Image1"); }

public List<CheckBox> chkHideBar { get => VBExtension.controlArray<CheckBox>(this, "chkHideBar"); }

public List<CheckBox> chkSounds { get => VBExtension.controlArray<CheckBox>(this, "chkSounds"); }

public List<CheckBox> chkNiceBar { get => VBExtension.controlArray<CheckBox>(this, "chkNiceBar"); }

public List<Image> Picture1 { get => VBExtension.controlArray<Image>(this, "Picture1"); }

public List<Image> Picture2 { get => VBExtension.controlArray<Image>(this, "Picture2"); }

public List<CheckBox> chkMIRC { get => VBExtension.controlArray<CheckBox>(this, "chkMIRC"); }

public List<CheckBox> chkMSN { get => VBExtension.controlArray<CheckBox>(this, "chkMSN"); }

public List<TextBox> txtXFile { get => VBExtension.controlArray<TextBox>(this, "txtXFile"); }

public List<CheckBox> chkReload { get => VBExtension.controlArray<CheckBox>(this, "chkReload"); }

public List<TextBox> txtReps { get => VBExtension.controlArray<TextBox>(this, "txtReps"); }

public List<Label> Label1 { get => VBExtension.controlArray<Label>(this, "Label1"); }

public List<Label> Label2 { get => VBExtension.controlArray<Label>(this, "Label2"); }

public List<Label> Label3 { get => VBExtension.controlArray<Label>(this, "Label3"); }

public List<Label> MSNList1 { get => VBExtension.controlArray<Label>(this, "MSNList1"); }

public List<Label> Command1 { get => VBExtension.controlArray<Label>(this, "Command1"); }

public List<Label> cmdOK { get => VBExtension.controlArray<Label>(this, "cmdOK"); }

public List<Line> Line10 { get => VBExtension.controlArray<Line>(this, "Line10"); }

public List<Line> Line9 { get => VBExtension.controlArray<Line>(this, "Line9"); }

public List<Label> lblHeader { get => VBExtension.controlArray<Label>(this, "lblHeader"); }

public List<Line> Line6 { get => VBExtension.controlArray<Line>(this, "Line6"); }

public List<Line> Line5 { get => VBExtension.controlArray<Line>(this, "Line5"); }

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
 // |  Options dialog  |
 // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
 // | Code 100% by Kyoufu Kawa.                          |
 // | Last update: April 1st, 2006                       |
 // |____________________________________________________|

 // ###########################################################################################


Implements(ISubclass);
EMsgResponse e_mr = null;
int WM_MOUSEWHEEL = 0x20A;

  private void cbxPresets_Click(object sender, RoutedEventArgs e) { cbxPresets_Click(); }
private void cbxPresets_Click() {
  int r = 0;
int g = 0;
int b = 0;
decimal H = 0;
decimal l = 0;
decimal s = 0;
  SplitRGB(ref cbxPresets.ItemData[cbxPresets.ListIndex].DefaultProperty, ref r, ref g, ref b);
  RGBToHLS(r, g, b, ref H, ref l, ref s);
  HScroll1.Value = (H * 10); // - 1
  HScroll2.Value = (s * 10); // - 1
}

  private void chkNiceBar_Click(object sender, RoutedEventArgs e) { chkNiceBar_Click(); }
private void chkNiceBar_Click() {
  Image1.left = (chkNiceBar.IsChecked == 1 ? -32 : 0);
}

  private void cmdOK_Click(object sender, RoutedEventArgs e) { cmdOK_Click(); }
private void cmdOK_Click() {
  int i = 0;

  i = Int(txtReps.Text);

  frmSappy.xfile = AppContext.BaseDirectory + "\\" + txtXFile.Text;
  WriteSetting(ref "XML File", ref txtXFile.Text);
  WriteSettingI(ref "Reload ROM", ref chkReload.IsChecked);
  WriteSettingI(ref "mIRC Now Playing", ref chkMIRC.IsChecked);
  WriteSettingI(ref "MSN Now Playing", ref chkMSN.IsChecked);
  WriteSettingI(ref "Incessant Sound Override", ref chkSounds.IsChecked);
  WriteSettingI(ref "Force Nice Bar", ref chkNiceBar.IsChecked);
  WriteSettingI(ref "Hide Bar", ref chkHideBar.IsChecked);
  WriteSettingI(ref "Song Repeats", ref i);

  WriteSetting(ref "Skin", ref picSkin[0].Source);
  WriteSetting(ref "Skin Hue", ref HScroll1.Value / 10);
  WriteSetting(ref "Skin Saturation", ref HScroll2.Value / 10);

  frmSappy.HandleClassicMode();
  frmSappy.FixStatusBar();
  frmSappy.ebr.UseExplorerStyle = (chkNiceBar.IsChecked == 0);
  frmSappy.picSkin.Picture = picSkin[0].Source;
  Colorize(ref frmSappy.instance.picSkin, ref HScroll1.Value / 10, ref HScroll2.Value / 10);
  frmSappy.RedrawSkin();
  frmSappy.ebr.Redraw = false;
  frmSappy.ebr.BackColorStart = frmSappy.picSkin.point[6, 16].Source;
  frmSappy.ebr.BackColorEnd = frmSappy.picSkin.point[6, 32].Source;
  frmSappy.ebr.Redraw = true;

  ClickSound();
  Unload();
}

  private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  ClickSound();
  Unload();
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  int i = 0;
    for (i = 0; i <= picPage.UBound(); i += 1) {
    picPage[i].Source = 0;
    picPage[i].Source = false;
    picPage(i).Move 136, 32;
  }
  MSNList1.AddItem(LoadResString(6011)); // Playback
  MSNList1.AddItem(LoadResString(6005)); // Now Playing
  MSNList1.AddItem(LoadResString(6008)); // Interface
  MSNList1.AddItem(LoadResString(6012)); // Skin
   // MSNList1.ListIndex = 0
  // TODO: (NOT SUPPORTED): On Error Resume Next
  i = GetSettingI(ref "Settings Page");
  if(i > MSNList1.ListCount - 1)i = 0;
  MSNList1.SelectedIndex = i;

  AttachMessage(this, hwnd, WM_MOUSEWHEEL);

  picPage[3].Source = 3;

  SetCaptions(ref this);
  Caption = LoadResString(6000);
  Picture1.Picture = frmSappy.instance.instance.imlStatusbar.ItemPicture(3);
  Picture2.Picture = frmSappy.instance.instance.imlStatusbar.ItemPicture(4);

  txtXFile.Text = GetSetting(ref "XML File");
  if(txtXFile.Text == "")txtXFile.Text = "sappy.xml";
  // TODO: (NOT SUPPORTED): On Error Resume Next
  chkReload.IsChecked = GetSettingI(ref "Reload ROM");
  chkMIRC.IsChecked = GetSettingI(ref "mIRC Now Playing");
  chkMSN.IsChecked = GetSettingI(ref "MSN Now Playing");
  chkSounds.IsChecked = GetSettingI(ref "Incessant Sound Override");
  chkNiceBar.IsChecked = GetSettingI(ref "Force Nice Bar");
  chkHideBar.IsChecked = GetSettingI(ref "Hide Bar");
  txtReps.Text = GetSettingI(ref "Song Repeats");
  if(txtReps.Text == "")txtReps.Text = 2;

  decimal hue = 0;
decimal sat = 0;
string regset = "";
int skinno = 0;
   // skinno = GetSettingI(__S1)
  regset = GetSetting(ref "Skin");
  if(regset != "")skinno = Val(regset); else skinno = 0;
  regset = GetSetting(ref "Skin Hue");
  if(regset != "")hue = Val(Replace(regset, ",", ".")); else hue = 3.4m;
  regset = GetSetting(ref "Skin Saturation");
  if(regset != "")sat = Val(Replace(regset, ",", ".")); else sat = 0.4m;

  // TODO: (NOT SUPPORTED): On Error GoTo 0

  Image1.Picture = LoadResPicture("BARSAMPLES", 0);
  Image1.left = (chkNiceBar.IsChecked == 1 ? -32 : 0);

  HScroll1.Value = hue * 10;
  HScroll2.Value = sat * 10;

  picSkin[0].Source = skinno;
  shpSkinSel.Move(picSkin[skinno].Source);

  int r = 0;
int g = 0;
int b = 0;
  HLSToRGB(3.4m, 0.5m, 0.4m, ref r, ref g, ref b);
  cbxPresets.AddItemAndData("MSN-style", , , RGB(r, g, b)); // &HB47C30 '508CB4
  HLSToRGB(0, 1, 0, ref r, ref g, ref b);
  cbxPresets.AddItemAndData("Grayscale", , , RGB(r, g, b));
  HLSToRGB(0.4m, 1, 0.2m, ref r, ref g, ref b);
  cbxPresets.AddItemAndData("Rusty Nailz", , , RGB(r, g, b));
  HLSToRGB(2.4m, 1, 0.3m, ref r, ref g, ref b);
  cbxPresets.AddItemAndData("Green Dreem", , , RGB(r, g, b));
}

  private void Form_Resize() {
    if(Tag == "repsplz") {
    MSNList1.SelectedIndex = 0;
    txtReps.SetFocus();
    Tag = "";
  }
}

  private void Form_Unload(ref int Cancel) {
  DetachMessage(this, hwnd, WM_MOUSEWHEEL);
}

  private void HScroll1_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { HScroll1_Change(); }
private void HScroll1_Change() {
  HScroll1_Scroll();
}
  private void HScroll2_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { HScroll2_Change(); }
private void HScroll2_Change() {
  HScroll2_Scroll();
}

  private void HScroll1_Scroll() {
  decimal i = 0;
  i = HScroll1.Value / 10;
  Label10.Content = i;
  DrawColorBar();
}
  private void HScroll2_Scroll() {
  decimal i = 0;
  i = HScroll2.Value / 10;
  Label11.Content = i;
  DrawColorBar();
}


  private int ISubclass_WindowProc(int hwnd, int iMsg, int wParam, int lParam) {
int _ISubclass_WindowProc = 0;
    if(iMsg == WM_MOUSEWHEEL) {
      if(wParam > 0) {
      MSNList1.SelectedIndex = (MSNList1.SelectedIndex > 0 ? MSNList1.SelectedIndex - 1 : 0);
      MSNList1.Redraw();
      } else if(wParam < 0) {
      MSNList1.SelectedIndex = (MSNList1.SelectedIndex < MSNList1.ListCount - 1 ? MSNList1.SelectedIndex + 1 : MSNList1.ListCount - 1);
      MSNList1.Redraw();
    }
  }
return _ISubclass_WindowProc;
}

  private void MSNList1_Click(object sender, RoutedEventArgs e) { MSNList1_Click(); }
private void MSNList1_Click() {
  int i = 0;
    for (i = 0; i <= picPage.UBound(); i += 1) {
    picPage[i].Source = false;
  }
  picPage[MSNList1.ListIndex].Source = true;
  lblHeader.Content = MSNList1.List[MSNList1.ListIndex].DefaultProperty;
  WriteSettingI(ref "Settings Page", ref MSNList1.SelectedIndex);
}

  private void picSkin_Click(object sender, RoutedEventArgs e) { picSkin_Click(); }
private void picSkin_Click(ref int Index) {
  shpSkinSel.Move(picSkin[Index].Source);
  picSkin[0].Source = Index;
}

  private void picSkin_Paint(ref int Index) {
  picSkinLoad.Picture = LoadResPicture(100 + Index, 0);
  BitBlt(picSkin[Index].Source, 0, 0, 16, 16, picSkinLoad.hdc, 8, 0, vbSrcCopy);
}

  private void Picture5_Paint() {
  DrawColorBar();
}

  private void txtReps_LostFocus(object sender, RoutedEventArgs e) { txtReps_LostFocus(); }
private void txtReps_LostFocus() {
  txtReps.Text = Val(txtReps.Text);
}

  private void DrawColorBar() {
  decimal i = 0;
  int r = 0;
int g = 0;
int b = 0;
decimal H = 0;
decimal s = 0;
int c = 0;
  Picture5.ScaleWidth = 1;
  Picture5.ScaleHeight = 1;
    for (i = 0; i <= 0.01m; i += 1) {
    HLSToRGB(HScroll1.Value / 10, HScroll2.Value / 10, i, ref r, ref g, ref b);
    Picture5.Line(((i, 0))-(i + 0.01m, 1), RGB(r, g, b), BF);
  }
}


}
}