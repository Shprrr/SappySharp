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
public partial class frmAssembler : Window {
  private static frmAssembler _instance;
  public static frmAssembler instance { set { _instance = null; } get { return _instance ?? (_instance = new frmAssembler()); }}  public static void Load() { if (_instance == null) { dynamic A = frmAssembler.instance; } }  public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }  public frmAssembler() { InitializeComponent(); }


public List<Window> frmAssembler { get => VBExtension.controlArray<Window>(this, "frmAssembler"); }

public List<TextBox> Text1 { get => VBExtension.controlArray<TextBox>(this, "Text1"); }

public List<Button> Command4 { get => VBExtension.controlArray<Button>(this, "Command4"); }

public List<TextBox> txtROM { get => VBExtension.controlArray<TextBox>(this, "txtROM"); }

public List<Button> Command3 { get => VBExtension.controlArray<Button>(this, "Command3"); }

public List<Button> Command2 { get => VBExtension.controlArray<Button>(this, "Command2"); }

public List<Button> Command1 { get => VBExtension.controlArray<Button>(this, "Command1"); }

public List<TextBox> txtFile { get => VBExtension.controlArray<TextBox>(this, "txtFile"); }

public List<TextBox> txtVoicegroup { get => VBExtension.controlArray<TextBox>(this, "txtVoicegroup"); }

public List<TextBox> txtOffset { get => VBExtension.controlArray<TextBox>(this, "txtOffset"); }

public List<Label> Label6 { get => VBExtension.controlArray<Label>(this, "Label6"); }

public List<Label> Label5 { get => VBExtension.controlArray<Label>(this, "Label5"); }

public List<Label> Label2 { get => VBExtension.controlArray<Label>(this, "Label2"); }

public List<Line> Line2 { get => VBExtension.controlArray<Line>(this, "Line2"); }

public List<Line> Line1 { get => VBExtension.controlArray<Line>(this, "Line1"); }

public List<Label> Label1 { get => VBExtension.controlArray<Label>(this, "Label1"); }

public List<Label> Label4 { get => VBExtension.controlArray<Label>(this, "Label4"); }

public List<Label> Label3 { get => VBExtension.controlArray<Label>(this, "Label3"); }


cCommonDialog MyCC = new cCommonDialog(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages

ScriptControl MyScripter = null; // to do Eval with
var Defs(1 = null;
string 2048) = "";
int DefC = 0;
var Labels(1 = null;
string 2048) = "";
int LabelC = 0;

string FileTit = ""; // just the name.ext part
string FileDir = ""; // just the dir part

string LabelToWatchFor = "";
int WatchedLabelOffset = 0;
int LabelOffset = 0;
int VoiceGroup = 0;

int SongTableEntry = 0;

bool YouFailedIt = false;

  private void Compile(ref string file) {
  List<string> MyKeywords = new List<string>();
  string MyLine = "";
  string D = "";
  string Eval = "";
  int ff = 0;
  int i = 0;

   // Get file title and dir if not already known
   // WARNING: might fail!
   // If FileDir = __S1 Then
   // FileTit = MyCC.VBGetFileTitle(txtFile.Text)
   // FileDir = Left(txtFile, Len(txtFile) - Len(FileTit))
   // End If

  Seek(#99, Replace(txtOffset.Text, "0x", "&H") + 1);

  ff = FreeFile; // find next file # to use, can't use hardcoded #s here
  Open(file For Input As ff);
   // On Error GoTo hell
    do {
    Line(Input #ff, MyLine);
    MyLine = Replace(MyLine, vbTab, " "); // fold in the tabs
      if(InStr(1, MyLine, "@")) { // strip away comments
      MyLine = Left(MyLine, InStr(1, MyLine, "@") - 1);
    }
    MyLine = Trim(MyLine); // strip away spaces
    MyLine = Replace(MyLine, "0x", "&H"); // VB-ify all hex numbers
    MyLine = Replace(MyLine, ",", ", "); // do some more folding
    MyLine = Replace(MyLine, "  ", " ");
    MyLine = Replace(MyLine, "  ", " ");
    MyLine = Replace(MyLine, ",", "");
      for (i = DefC; i <= -1; i += 0) { // fold out all equs
      MyLine = Replace(MyLine, Defs(0, i), Defs(1, i));
    }
    if(Len(MyLine) == 0)goto Bleh; // did we end up with an empty line? then skip it.

    MyKeywords = new List<string>(Split(MyLine)); // split up the line into seperate keywords

      if(Right(MyKeywords[0], 1) == ":") { // is this a label?
      Labels(0, LabelC) = Left(MyKeywords(0), Len(MyKeywords(0)) - 1); // take out the label name
      Labels(1, LabelC) = Seek(99) - 1 + &H8000000; // find and store the current target file position
        if(Labels(0, LabelC) == LabelToWatchFor) { // is this the song header's label?
        WatchedLabelOffset = Seek(99); // store header offset
      }
      LabelC = LabelC + 1;
      goto Bleh; // stop compiling. we can get away with this thanks to the well-formed MID2AGB output.
    }

        switch(MyKeywords[0]) {
        case ".include":
          if(Dir(Mid(MyKeywords[1], 2, Len(MyKeywords[1]) - 2)) == "") {
          MsgBox("Can't find file " + MyKeywords[1] + " for inclusion. Assembly halted.");
          YouFailedIt = true; // FUCK!
          return;
        }
        Compile(Mid(MyKeywords[1], 2, Len(MyKeywords[1]) - 2)); // fork out a new compiler

        break;
case ".global":
        LabelToWatchFor = MyKeywords[1]; // there's only one global: the header's label!

        break;
case ".equ":
        Defs(0, DefC) = MyKeywords(1); // simply store the keyword and value
        Defs(1, DefC) = MyKeywords(2);
        DefC = DefC + 1;

        break;
case ".byte":
          for (i = 1; i <= MyKeywords.Count; i += 1) {
            if(MyKeywords[i] != "") {
            Put(#99, , CByte(Val(MyScripter.Eval(MyKeywords[i])))); // ooooooooooh yeah!
          }
        }

        break;
case ".word":
          for (i = 0; i <= LabelC; i += 1) {
            if(MyKeywords[1] == Labels(0, i)) {
             // Trace __S1 & Labels(0, i) & __S2 & Hex(CLng(Val(Labels(1, i))))
            Put(#99, , CLng(Val(Labels(1, i)))); // words in Sappy songs are always label names.
            break;
          }
        }

        break;
case ".end":
        break;
  break;
}

  Bleh:;
  DoEvents();
} while(!(EOF(ff)));
VBCloseFile(ff); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #ff
return;
hell:;
 // If Err.Number <> 62 Then
MsgBox("Error #" + Err().Number + ", \"" + Err().Description + "\"" + vbCrLf + vbCrLf + "On line: \"" + MyLine + "\".");
YouFailedIt = true;
return;
 // End If
}

private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
private void Command1_Click() {
MousePointer = 11;
LabelOffset = Val(Replace(txtOffset.Text, "0x", "&H"));
SongTableEntry = Val(Replace(Text1.Text, "0x", "&H"));
VoiceGroup = Val(Replace(txtVoicegroup.Text, "0x", "&H"));
Command1.IsEnabled = false;

  if(txtROM.Text == "") {
  MsgBox("You must specify both file names.");
  MousePointer = 0;
  Command1.IsEnabled = true;
  return;
}
  if(txtROM.Text == "") {
  MsgBox("You must specify both file names.");
  MousePointer = 0;
  Command1.IsEnabled = true;
  return;
}
  if(LabelOffset == 0) {
  MsgBox("You must specify a base offset.");
  MousePointer = 0;
  Command1.IsEnabled = true;
  return;
}
  if(VoiceGroup == 0) {
  MsgBox("You must specify a voicegroup offset.");
  MousePointer = 0;
  Command1.IsEnabled = true;
  return;
}
  if(SongTableEntry == 0) {
  MsgBox("You must specify a song table offset.");
  MousePointer = 0;
  Command1.IsEnabled = true;
  return;
}

VBOpenFile(txtROM, "Binary", 99); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open txtROM For Binary As #99

MyScripter = new ScriptControl(); // instantiate and set a new VBScript interpreter
MyScripter.Language = "vbScript";
Labels(0, 0) = "voicegroup000"; // auto-def a value for __S2
Labels(1, 0) = VoiceGroup + &H8000000; // that's VERY important!
LabelC = 1;

FileDir = Left(txtFile.Text, Len(txtFile.Text) - Len(MyCC.VBGetFileTitle(txtFile.Text)));
ChDir(FileDir);
Compile(MyCC.VBGetFileTitle(txtFile.Text));

  if(YouFailedIt == true) {
  MousePointer = 0;
  return;
}

Command2.Caption = "Close";

  if(MsgBox("Done. Do you want to set the proper entry in the Song Table?", vbYesNo) == vbYes) {
  Put(#99, SongTableEntry + 1, WatchedLabelOffset - 1 + 0x8000000);
}

MousePointer = 0;
Command1.IsEnabled = false;
}

private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
private void Command2_Click() {
Unload();
}

private void Command3_Click(object sender, RoutedEventArgs e) { Command3_Click(); }
private void Command3_Click() {
if(MyCC.VBGetOpenFileName(FileTit, , , , , , "MID2AGB output (*.s)|*.s") == false)return;
txtFile.Text = FileTit;
 // FileTit = MyCC.VBGetFileTitle(txtFile.Text)
 // FileDir = Left(txtFile, Len(txtFile) - Len(FileTit))
}

private void Command4_Click(object sender, RoutedEventArgs e) { Command4_Click(); }
private void Command4_Click() {
string foo = "";
if(MyCC.VBGetOpenFileName(foo, , , , , , "GBA ROM files (*.gba)|*.gba") == false)return;
txtROM.Text = foo;
}


}
}