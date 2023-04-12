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



static class MidiLib {

DefLng(A-Z);

[DllImport("winmm.dll")]
public static extern int midiOutOpen(ref int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);
[DllImport("winmm.dll")]
public static extern int midiOutClose(int hMidiOut);
[DllImport("winmm.dll")]
public static extern int midiOutShortMsg(int hMidiOut, int dwMsg);


 // #define NoteOnCmd       0x90
 // #define NoteOffCmd      0x80
 // #define PgmChngCmd      0xC0
 // #define ControlCmd      0xB0
 // #define PolyPressCmd    0xA0
 // #define ChanPressCmd    0xD0
 // #define PchWheelCmd     0xE0
 // #define SysExCmd        0xF0

public static int mdh = 0;
public static bool midiOpened = false;

public static int WantedMidiDevice = 0;

  public static void MidiOpen() {
    if(midiOutOpen(mdh, WantedMidiDevice, 0, 0, 0)) {
     // Trace __S1
    // TODO: (NOT SUPPORTED): On Error Resume Next
    VBOpenFile(App.Path, "+", "\\pmidi.tmp" For Input As #4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open App.Path + __S1 For Input As #4
      if(Err().Number == 0) {
      int i = 0;
      string S = "";
      Line(Input #4, S);
      Input(#4, i);
      midiOutClose(i);
      VBCloseFile(4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #4
    }
  }
   // allows for closing midi after a crash
  VBOpenFile(App.Path, "+", "\\pmidi.tmp" For Output As #4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open App.Path + __S1 For Output As #4
  VBWriteFile("Print #4, "Previous midi handle: (used in case it crashed last time)""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #4, __S1
  VBWriteFile("Print #4, mdh"); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #4, mdh
  VBCloseFile(4); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #4
  midiOpened = true;
}

  public static void MidiClose() {
    if(midiOpened) {
     // Trace __S1
    midiOutClose(mdh);
    midiOpened = false;
  }
}

  public static void SelectInstrument(int channel, int patch) {
    if(midiOpened) {
    midiOutShortMsg(mdh, 0xC0 || patch * 256 || channel);
     // Trace __S1 & channel & __S2 & patch
  }
}

  public static void ToneOn(int channel, int tone, int volume) {
    if(midiOpened) {
    if(tone < 0)tone = 0;
    if(tone > 127)tone = 127;
    midiOutShortMsg(mdh, 0x90 || tone * 256 || channel || volume * 65536);
     // Trace __S1 & tone & __S2 & channel & __S3 & volume
  }
}

  public static void ToneOff(int channel, int tone) {
    if(midiOpened) {
    if(tone < 0)tone = 0;
    if(tone > 127)tone = 127;
    midiOutShortMsg(mdh, 0x80 || tone * 256 || channel);
     // Trace __S1 & tone & __S2 & channel
  }
}

  public static void SetChnVolume(int channel, int volume) {
    if(midiOpened) {
    midiOutShortMsg(mdh, 0xD0 || volume * 256 || channel);
     // Trace __S1 & channel & __S2 & volume
  }
}

  public static void SetChnPan(int channel, int pan) {
    if(midiOpened) {
    midiOutShortMsg(mdh, 0xB0 || 0xA * 256 || channel || pan * 65536);
     // midiOutShortMsg mdh, &HB0 Or pan * 256 Or channel Or &HA * 65536
     // Trace __S1 & channel & __S2 & pan
  }
}

  public static void PitchWheel(int channel, var pit) {
    if(midiOpened) {
    midiOutShortMsg(mdh, 0xE0 || pit * 256 || channel);
     // Trace __S1 & tone & __S2 & channel & __S3 & volume
  }
}



}