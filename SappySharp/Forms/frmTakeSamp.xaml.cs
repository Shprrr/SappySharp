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
public partial class frmTakeSamp : Window {
  private static frmTakeSamp _instance;
  public static frmTakeSamp instance { set { _instance = null; } get { return _instance ?? (_instance = new frmTakeSamp()); }}  public static void Load() { if (_instance == null) { dynamic A = frmTakeSamp.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmTakeSamp() { InitializeComponent(); }


public List<Window> frmTakeSamp { get => VBExtension.controlArray<Window>(this, "frmTakeSamp"); }

public List<TextBox> txtLog { get => VBExtension.controlArray<TextBox>(this, "txtLog"); }

public List<Image> Picture2 { get => VBExtension.controlArray<Image>(this, "Picture2"); }

public List<Label> Label3 { get => VBExtension.controlArray<Label>(this, "Label3"); }

public List<Label> Label2 { get => VBExtension.controlArray<Label>(this, "Label2"); }

public List<Image> Picture1 { get => VBExtension.controlArray<Image>(this, "Picture1"); }

public List<Label> Label9 { get => VBExtension.controlArray<Label>(this, "Label9"); }

public List<Label> lblFileDesc { get => VBExtension.controlArray<Label>(this, "lblFileDesc"); }

public List<Label> Command2 { get => VBExtension.controlArray<Label>(this, "Command2"); }

public List<ComboBox> cboSaveAs { get => VBExtension.controlArray<ComboBox>(this, "cboSaveAs"); }

public List<TextBox> txtNamePat { get => VBExtension.controlArray<TextBox>(this, "txtNamePat"); }

public List<Label> Command1 { get => VBExtension.controlArray<Label>(this, "Command1"); }

public List<Line> Line2 { get => VBExtension.controlArray<Line>(this, "Line2"); }

public List<Line> Line1 { get => VBExtension.controlArray<Line>(this, "Line1"); }

public List<Label> Label12 { get => VBExtension.controlArray<Label>(this, "Label12"); }

public List<Label> Label13 { get => VBExtension.controlArray<Label>(this, "Label13"); }

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
 // ___________________
 // |  Sample exporter  |
 // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
 // | Code 100% by Kyoufu Kawa, based on Maple.          |
 // | Last update: February 1st, 2005                    |
 // |____________________________________________________|

 // ###########################################################################################

int SingleSong = 0;

  class tInst{
  public Byte SndType;
  public Byte Shit1;
  public Byte Shit2;
  public Byte Shit3;
  public int WavePtr;
  public int MoreShit;
}

Byte DidWeAlreadyDumpThisOne(&HFFFFFF) = 0;
 // Private MyCdl As New cCommonDialog
gCommonDialog MyCdl = new gCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages

  private int ConFreq(int freq) {
int _ConFreq = 0;
  int k = 0;
    for (k = 1; k <= 10; k += 1) {
    freq = freq / 2;
  }
  _ConFreq = freq;
return _ConFreq;
}

  private void SaveSampleRAW(ref string Filename, ref int hdr1, ref int hdr2, ref int freq, ref int loopstart, ref int Length) {
  // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
  List<Byte> theStuff = new List<Byte>();
  // TODO: (NOT SUPPORTED): ReDim theStuff(Length)
  VBOpenFile(Filename, "Binary", 98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Binary As #98
  Get(#99, , theStuff);
  Put(#98, , theStuff);
  VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
  return;
  Fucksocks:;
    if(Err().Number == 75) {
    MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
    VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
    return;
  }
  MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
  // TODO: (NOT SUPPORTED): Resume Next
}

  private void SaveSampleWAV(ref string Filename, ref int hdr1, ref int hdr2, ref int freq, ref int loopstart, ref int Length) {
  // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
  List<Byte> theStuff = new List<Byte>();
  int k = 0;
  // TODO: (NOT SUPPORTED): ReDim theStuff(Length)
  VBOpenFile(Filename, "Binary", 98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Binary As #98
  Put(#98, , CByte(0x52));
  Put(#98, , CByte(0x49));
  Put(#98, , CByte(0x46));
  Put(#98, , CByte(0x46));
  Put(#98, , CByte(0x3E));
  Put(#98, , CByte(0x2B));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x57));
  Put(#98, , CByte(0x41));
  Put(#98, , CByte(0x56));
  Put(#98, , CByte(0x45));
  Put(#98, , CByte(0x66));
  Put(#98, , CByte(0x6D));
  Put(#98, , CByte(0x74));
  Put(#98, , CByte(0x20));
  Put(#98, , CByte(0x10));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x1));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x1));
  Put(#98, , CByte(0x0));
  Put(#98, , CLng(ConFreq(freq)));
  Put(#98, , CLng(ConFreq(freq)));
  Put(#98, , CByte(0x1));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x8));
  Put(#98, , CByte(0x0));
  Put(#98, , CByte(0x64));
  Put(#98, , CByte(0x61));
  Put(#98, , CByte(0x74));
  Put(#98, , CByte(0x61));
  Put(#98, , Length + 1);
  Get(#99, , theStuff);
    for (k = 0; k <= Length; k += 1) {
    theStuff[k] = theStuff[k] ^^ 128;
  }
  Put(#98, , theStuff);
  VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
  return;
  Fucksocks:;
    if(Err().Number == 75) {
    MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
    VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
    return;
  }
  MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
  // TODO: (NOT SUPPORTED): Resume Next
}

  private void SaveSampleITS(ref string Filename, ref int hdr1, ref int hdr2, ref int freq, ref int loopstart, ref int Length) {
  // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
  List<Byte> theStuff = new List<Byte>();
  string IMPS = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (4)
  IMPS = "IMPS";
  string DOSName = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (12)
  string SampName = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (26)

  // TODO: (NOT SUPPORTED): ReDim theStuff(Length)
  VBOpenFile(Filename, "Binary", 98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Binary As #98
  Get(#99, , theStuff);

  Put(#98, , IMPS);
  Put(#98, , DOSName);
  Put(#98, , CByte(0));
  Put(#98, , CByte(64)); // GvL
    if(loopstart) {
    Put(#98, , CByte(16));
    } else {
    Put(#98, , CByte(0));
  }
  Put(#98, , CByte(64)); // Vol
  Put(#98, , SampName);
  Put(#98, , CByte(1)); // Cvt
  Put(#98, , CByte(0)); // DfP
  Put(#98, , CLng(Length)); // CHECK!
  Put(#98, , CLng(loopstart)); // CHECK!
    if(loopstart) {
    Put(#98, , CLng(Length)); // CHECK!
    } else {
    Put(#98, , CLng(0)); // CHECK!
  }
  Put(#98, , CLng(ConFreq(freq))); // C5Speed
  Put(#98, , CLng(0)); // Sustain start
  Put(#98, , CLng(0)); // Sustain end
  Put(#98, , CLng(0x50));
  Put(#98, , CLng(0)); // Vibe settings

  Put(#98, , theStuff);
  VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
  return;
  Fucksocks:;
    if(Err().Number == 75) {
    MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
    VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
    return;
  }
  MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
  // TODO: (NOT SUPPORTED): Resume Next
}

  private void SaveSampleASM(ref string Filename, ref int hdr1, ref int hdr2, ref int freq, ref int loopstart, ref int Length) {
  // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
  List<Byte> theStuff = new List<Byte>();
  int j = 0;
int k = 0;
int l = 0;
string aByteStr = "";
  // TODO: (NOT SUPPORTED): ReDim theStuff(Length)
  Get(#99, , theStuff);
  VBOpenFile(Filename, "Output", 98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Output As #98
  VBWriteFile("Print #98, LoadResString(7030)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, LoadResString(7030)
  VBWriteFile("Print #98, "#TONE NAME     : "; Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1; Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2)
  VBWriteFile("Print #98, "#FREQUENCY     :"; freq"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1; freq
  VBWriteFile("Print #98, "#BASE NOTE#    : 60""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, "#START ADRESS  : 000000""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, "#LOOP ADDRESS  : " & Right("000000" & loopstart, 6)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1 & Right(__S2 & loopstart, 6)
  VBWriteFile("Print #98, "#END ADDRESS   : " & Right("000000" & Length, 6)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1 & Right(__S2 & Length, 6)
    if(hdr2 == 0x0) {
    aByteStr = "1Shot";
    } else if(hdr2 == 0x4000) {
    aByteStr = "Fwd";
    } else {
    aByteStr = "Maple chokes on 0x" + Right("0000" + Hex(hdr2), 4);
  }
  VBWriteFile("Print #98, "#LOOP MODE     : " & aByteStr"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1 & aByteStr
  VBWriteFile("Print #98, "#FINE TUNE     : 0""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, "#WAVE EXP/COMP : 1""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, "#VOL EXP/COMP  : 1""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, """); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, vbTab & ".section .rodata""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1
  VBWriteFile("Print #98, vbTab & ".Global" & vbTab & Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2)
  VBWriteFile("Print #98, vbTab & ".Align" & vbTab & "2""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & __S2
  VBWriteFile("Print #98, """); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2) & ":""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, Left(MyCdl.VBGetFileTitle(Filename), Len(MyCdl.VBGetFileTitle(Filename)) - 2) & __S1
  VBWriteFile("Print #98, vbTab & ".short" & vbTab & "0x" & Right("0000" & Hex(hdr1), 4)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & __S2 & Right(__S3 & Hex(hdr1), 4)
  VBWriteFile("Print #98, vbTab & ".short" & vbTab & "0x" & Right("0000" & Hex(hdr2), 4)"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & __S2 & Right(__S3 & Hex(hdr2), 4)
  VBWriteFile("Print #98, vbTab & ".Int" & vbTab & freq"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & freq
  VBWriteFile("Print #98, vbTab & ".Int" & vbTab & loopstart"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & loopstart
  VBWriteFile("Print #98, vbTab & ".Int" & vbTab & Length"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1 & vbTab & Length
  VBWriteFile("Print #98, """); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
    for (j = 0; j <= 8; j += Length) {
    aByteStr = vbTab + ".byte ";
    // TODO: (NOT SUPPORTED): On Error Resume Next
      for (k = 0; k <= 7; k += 1) {
      aByteStr = aByteStr + "0x" + Right("00" + Hex(theStuff[j + k]), 2) + ",";
    }
    // TODO: (NOT SUPPORTED): On Error GoTo 0
    aByteStr = Left(aByteStr, Len(aByteStr) - 1); // chop off trailing ,
    VBWriteFile("Print #98, aByteStr"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, aByteStr
  }
  VBWriteFile("Print #98, """); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, __S1
  VBWriteFile("Print #98, vbTab & ".end""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #98, vbTab & __S1
  VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
  return;
  Fucksocks:;
    if(Err().Number == 75) {
    MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
    VBCloseFile(98); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #98
    return;
  }
  MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
  // TODO: (NOT SUPPORTED): Resume Next
}

  private void DumpVoiceGroup(ref int anInstrumentLong, ref int numsamples = 256) {
  int i = 0;
int j = 0;
  int k = 0;
  int aFillerLong = 0;
  tInst anInstrument = null;
  Byte aByte = 0;
  string Blargh = "";
  int hdr1 = 0;
  int hdr2 = 0;
  int hdr3 = 0;
  int hdr4 = 0;
  int hdr5 = 0;

    for (j = 0; j <= numsamples; j += 1) {
    Get(#99, (anInstrumentLong + 1) + (12 * j), anInstrument);
      if(anInstrument.SndType == 0) {
        if(anInstrument.WavePtr > 0x8000000) {
          if(DidWeAlreadyDumpThisOne(j) == false) {
          DidWeAlreadyDumpThisOne(j) = true;
          Transcribe("Wave instrument found! #" + j);
          Seek(#99, anInstrument.WavePtr - 0x8000000 + 1);
          Get(#99, , hdr1);
          Get(#99, , hdr2);
          Get(#99, , hdr3);
          Get(#99, , hdr4); // loop
          Get(#99, , hdr5); // length
            if(hdr5 > 0x10000) {
            hdr5 = 0x10000;
            Transcribe("Warning: sample cut off.");
          }
          Blargh = txtNamePat.Text;
          Blargh = Replace(Blargh, "$I", Trim(Str(j)));
          Blargh = Replace(Blargh, "$P", Hex(anInstrument.WavePtr - 0x8000000 + 16));
          Transcribe("Saving as " + Blargh + "...");
          if(cboSaveAs.SelectedIndex == 0)SaveSampleRAW(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
          if(cboSaveAs.SelectedIndex == 1)SaveSampleWAV(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
          if(cboSaveAs.SelectedIndex == 2)SaveSampleITS(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
          if(cboSaveAs.SelectedIndex == 3)SaveSampleASM(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
          DoEvents();
        }
      }
       // ElseIf anInstrument.SndType = &H40 Then 'Key Split
       // If anInstrument.WavePtr > &H8000000 Then
       // Transcribe __S1
       // Transcribe __S1
       // Transcribe __S1
       // DumpVoiceGroup anInstrument.WavePtr - &H8000000, 4
       // End If
    }
  }
}

  private void cboSaveAs_Click(object sender, RoutedEventArgs e) { cboSaveAs_Click(); }
private void cboSaveAs_Click() {
  int dot = 0;
  dot = InStr(txtNamePat.Text, ".");
  if(dot > 0)txtNamePat.Text = Left(txtNamePat.Text, dot - 1);
  if(cboSaveAs.SelectedIndex == 0)txtNamePat.Text = txtNamePat.Text + ".raw";
  if(cboSaveAs.SelectedIndex == 1)txtNamePat.Text = txtNamePat.Text + ".wav";
  if(cboSaveAs.SelectedIndex == 2)txtNamePat.Text = txtNamePat.Text + ".its";
  if(cboSaveAs.SelectedIndex == 3)txtNamePat.Text = txtNamePat.Text + ".s";
  lblFileDesc.Content = LoadResString(cboSaveAs.SelectedIndex + 7020);
}

  private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
  int i = 0;
int j = 0;
  int k = 0;
  int anInstrumentLong = 0;
  int aFillerLong = 0;
  tInst anInstrument = null;
  Byte aByte = 0;
  string Blargh = "";

  txtNamePat.Text = Replace(txtNamePat.Text, "$p", "$P");
  txtNamePat.Text = Replace(txtNamePat.Text, "$i", "$I");
  i = 2;
  if(InStr(txtNamePat.Text, "$I"))i = i - 1;
  if(InStr(txtNamePat.Text, "$P"))i = i - 1;
    if(i == 2) {
    MsgBox(LoadResString(3011));
    IncessantNoises(ref "TaskFail"); // Bee-owee-owee-oweeeeeohh....
    return;
  }

  ClickSound();

  txtLog.Move(8, 8);
  txtLog.Visibility = true;
  MousePointer = 11;

    for (k = 0; k <= 0xFFFFFF; k += 1) {
    DidWeAlreadyDumpThisOne(k) = 0;
  }
  Seek(#99, SingleSong + 1);
  Get(#99, , aFillerLong);
  Get(#99, , anInstrumentLong);
  // TODO: (NOT SUPPORTED): On Error GoTo SillyPointers
  anInstrumentLong = anInstrumentLong - 0x8000000;
  // TODO: (NOT SUPPORTED): On Error GoTo 0
  Transcribe("Song's instrument pointer is " + Hex(anInstrumentLong));
  if(anInstrumentLong > 0)DumpVoiceGroup(anInstrumentLong);
  Blargh:;

  MousePointer = 0;
  Command1.IsEnabled = false;
   // Command1.FontBold = False
  Command2.Caption = "Exit";
   // Command2.FontBold = True
  Command2.IsDefault = true;
  IncessantNoises(ref "TaskComplete");
  return;

  SillyPointers:;
  Transcribe("Silly pointer!");
  goto Blargh;
}

  private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
private void Command2_Click() {
  ClickSound();
  Unload();
}

  private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
private void Form_Load() {
  SetCaptions(ref this);
  Caption = LoadResString(7000);
  cboSaveAs.AddItem(LoadResString(7010));
  cboSaveAs.AddItem(LoadResString(7011));
  cboSaveAs.AddItem(LoadResString(7012));
  cboSaveAs.AddItem(LoadResString(7013));

  cboSaveAs.SelectedIndex = 0;

    if(LoadResString(10000) == "<NLPLZ>" || LoadResString(10000) == "<SPLZ>" || LoadResString(10000) == "<DPLZ>") {
    Label9.Visibility = false;
    lblFileDesc.Top = Label9.Top;
    lblFileDesc.Height = lblFileDesc.Height + 16;
  }
}

  private void Transcribe(ref var t$) {
  if(Len(txtLog.Text) > 32000)txtLog.Text = "Resetting log." + vbCrLf;
  txtLog.Text = txtLog.Text + t$ + vbCrLf;
  txtLog.SelectionStart = Len(txtLog.Text);
}

  private void Picture1_Paint() {
  DrawSkin(ref Picture1.Source);
}
  private void Picture2_Paint() {
  DrawSkin(ref Picture2.Source);
}


}
}