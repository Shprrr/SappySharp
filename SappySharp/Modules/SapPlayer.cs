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



static class SapPlayer {
public static var NoiseWaves(0 To 1 = null;
public static string 0 To 9) = "";
public static bool Details = false;
  public class SappyMasterTableEntry{ 
  public int pSong;
  public int wPriority1;
  public int wPriority2;
}
  public class SappySongHeader{ 
  public Byte wTracks; // Integer
  public Byte wBlocks;
   // w1 As Integer
  public Byte wPriority;
  public Byte wReverb;
  public int pInstrumentBank;
}
  public class SappyInstrumentHeader{ 
  public Byte bChannel;
  public Byte bDrumPitch;
}
  public class SappyDirectHeader{ 
  public Byte b0;
  public Byte b1;
  public int pSampleHeader;
  public Byte bAttack;
  public Byte bHold;
  public Byte bSustain;
  public Byte bRelease;
}
  public class SappySquare1Header{ 
  public Byte bRaw1;
  public Byte bRaw2;
  public Byte bDutyCycle;
  public Byte b3;
  public Byte b4;
  public Byte b5;
  public Byte bAttack;
  public Byte bDecay;
  public Byte bSustain;
  public Byte bRelease;
}
  public class SappySquare2Header{ 
  public Byte b0;
  public Byte b1;
  public Byte bDutyCycle;
  public Byte b3;
  public Byte b4;
  public Byte b5;
  public Byte bAttack;
  public Byte bDecay;
  public Byte bSustain;
  public Byte bRelease;
}
  public class SappyWaveHeader{ 
  public Byte b0;
  public Byte b1;
  public int pSample;
  public Byte bAttack;
  public Byte bDecay;
  public Byte bSustain;
  public Byte bRelease;
}
  public class SappyNoiseHeader{ 
  public Byte b0;
  public Byte b1;
  public Byte b2;
  public Byte b3;
  public Byte b4;
  public Byte b5;
  public Byte bAttack;
  public Byte bDecay;
  public Byte bSustain;
  public Byte bRelease;
}
  public class SappyInvalidHeader{ 
  public Byte b0;
  public Byte b1;
  public Byte b2;
  public Byte b3;
  public Byte b4;
  public Byte b5;
  public Byte b6;
  public Byte b7;
  public Byte b8;
  public Byte b9;
}
  public class SappyDrumKitHeader{ 
  public Byte b0;
  public Byte b1;
  public int pDirectTable;
  public Byte b6;
  public Byte b7;
  public Byte b8;
  public Byte b9;
}
  public class SappyMultiHeader{ 
  public Byte b0;
  public Byte b1;
  public int pDirectTable;
  public int pKeyMap;
}
  public class SappySampleHeader{ 
  public int flags;
  public Byte b4;
  public Byte FineTune;
  public int wFreq;
  public int wLoop;
  public int wSize;
}


 // Channel Byte

 // Special Instrument Bits (Override all others)
 // 6 - Multi-sample Instrument
 // 7 - Drum Kit

 // Bits 0-2 assign a channel:
 // 0 - Direct
 // 1 - Square 1
 // 2 - Square 2
 // 3 - Wave
 // 4 - Noise
 // 5-7 - None

 // Direct Channel Bits:
 // 3 - Fixed Pitch (Plays Sample at Middle C frequency)
 // 4 - Reverse (Reverses Sample)
 // 5 - ??


  public static SappySongHeader ReadSongHead(int filenumber, int offset = -1) {
SappySongHeader _ReadSongHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadSongHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadSongHead;
}
  public static SappyInstrumentHeader ReadInstrumentHead(int filenumber, int offset = -1) {
SappyInstrumentHeader _ReadInstrumentHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadInstrumentHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadInstrumentHead;
}
  public static SappyDirectHeader ReadDirectHead(int filenumber, int offset = -1) {
SappyDirectHeader _ReadDirectHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadDirectHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadDirectHead;
}
  public static SappyDrumKitHeader ReadDrumKitHead(int filenumber, int offset = -1) {
SappyDrumKitHeader _ReadDrumKitHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadDrumKitHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadDrumKitHead;
}
  public static SappyMultiHeader ReadMultiHead(int filenumber, int offset = -1) {
SappyMultiHeader _ReadMultiHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadMultiHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadMultiHead;
}
  public static SappySquare1Header ReadSquare1Head(int filenumber, int offset = -1) {
SappySquare1Header _ReadSquare1Head = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadSquare1Head);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadSquare1Head;
}
  public static SappySquare1Header ReadSquare2Head(int filenumber, int offset = -1) {
SappySquare1Header _ReadSquare2Head = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadSquare2Head);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadSquare2Head;
}
  public static SappyWaveHeader ReadWaveHead(int filenumber, int offset = -1) {
SappyWaveHeader _ReadWaveHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadWaveHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadWaveHead;
}
  public static SappyNoiseHeader ReadNoiseHead(int filenumber, int offset = -1) {
SappyNoiseHeader _ReadNoiseHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadNoiseHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadNoiseHead;
}
  public static SappyInvalidHeader ReadInvalidHead(int filenumber, int offset = -1) {
SappyInvalidHeader _ReadInvalidHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadInvalidHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadInvalidHead;
}
  public static SappySampleHeader ReadSampleHead(int filenumber, int offset = -1) {
SappySampleHeader _ReadSampleHead = null;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadSampleHead);
  ReadOffset(filenumber, Seek(filenumber) - 1);
return _ReadSampleHead;
}

  public static int SignedByteToInteger(Byte SignedByte) {
int _SignedByteToInteger = 0;
  _SignedByteToInteger = (SignedByte > 0x7F ? SignedByte - 0x100 : SignedByte);
return _SignedByteToInteger;
}

  public static string NoteToName(Byte MIDINote) {
string _NoteToName = "";
  X = MIDINote % 12;
  o = MIDINote / 12;
      switch(X) {
      case 0: _NoteToName = "C";
      break;
case 1: _NoteToName = "C#";
      break;
case 2: _NoteToName = "D";
      break;
case 3: _NoteToName = "D#";
      break;
case 4: _NoteToName = "E";
      break;
case 5: _NoteToName = "F";
      break;
case 6: _NoteToName = "F#";
      break;
case 7: _NoteToName = "G";
      break;
case 8: _NoteToName = "G#";
      break;
case 9: _NoteToName = "A";
      break;
case 10: _NoteToName = "A#";
      break;
case 11: _NoteToName = "B";
break;
}
_NoteToName = _NoteToName + o;
return _NoteToName;
}
public static int NoteToFreq(int MIDINote, int MidCFreq = -1) {
int _NoteToFreq = 0;
magic = 2 ^ (1 / 12);
X = MIDINote - 0x3C;
  if(MidCFreq == -1) {
  a = 7040;
  c = a * ((magic ^ 3));
  } else if(MidCFreq == -2) {
  a = 7040 / 2;
  c = a * ((magic ^ 3));
  
  } else {
  c = MidCFreq;
}
_NoteToFreq = c * ((magic ^ X));
return _NoteToFreq;
}
public static int SLen2Ticks(Byte ShortLen) {
int _SLen2Ticks = 0;
    switch(ShortLen) {
    case 0x0: _SLen2Ticks = 0x0;
    break;
case 0x1: _SLen2Ticks = 0x1;
    break;
case 0x2: _SLen2Ticks = 0x2;
    break;
case 0x3: _SLen2Ticks = 0x3;
    break;
case 0x4: _SLen2Ticks = 0x4;
    break;
case 0x5: _SLen2Ticks = 0x5;
    break;
case 0x6: _SLen2Ticks = 0x6;
    break;
case 0x7: _SLen2Ticks = 0x7;
    break;
case 0x8: _SLen2Ticks = 0x8;
    break;
case 0x9: _SLen2Ticks = 0x9;
    break;
case 0xA: _SLen2Ticks = 0xA;
    break;
case 0xB: _SLen2Ticks = 0xB;
    break;
case 0xC: _SLen2Ticks = 0xC;
    break;
case 0xD: _SLen2Ticks = 0xD;
    break;
case 0xE: _SLen2Ticks = 0xE;
    break;
case 0xF: _SLen2Ticks = 0xF;
    break;
case 0x10: _SLen2Ticks = 0x10;
    break;
case 0x11: _SLen2Ticks = 0x11;
    break;
case 0x12: _SLen2Ticks = 0x12;
    break;
case 0x13: _SLen2Ticks = 0x13;
    break;
case 0x14: _SLen2Ticks = 0x14;
    break;
case 0x15: _SLen2Ticks = 0x15;
    break;
case 0x16: _SLen2Ticks = 0x16;
    break;
case 0x17: _SLen2Ticks = 0x17;
    break;
case 0x18: _SLen2Ticks = 0x18;
    break;
case 0x19: _SLen2Ticks = 0x1C;
    break;
case 0x1A: _SLen2Ticks = 0x1E;
    break;
case 0x1B: _SLen2Ticks = 0x20;
    break;
case 0x1C: _SLen2Ticks = 0x24;
    break;
case 0x1D: _SLen2Ticks = 0x28;
    break;
case 0x1E: _SLen2Ticks = 0x2A;
    break;
case 0x1F: _SLen2Ticks = 0x2C;
    break;
case 0x20: _SLen2Ticks = 0x30;
    break;
case 0x21: _SLen2Ticks = 0x34;
    break;
case 0x22: _SLen2Ticks = 0x36;
    break;
case 0x23: _SLen2Ticks = 0x38;
    break;
case 0x24: _SLen2Ticks = 0x3C;
    break;
case 0x25: _SLen2Ticks = 0x40;
    break;
case 0x26: _SLen2Ticks = 0x42;
    break;
case 0x27: _SLen2Ticks = 0x44;
    break;
case 0x28: _SLen2Ticks = 0x48;
    break;
case 0x29: _SLen2Ticks = 0x4C;
    break;
case 0x2A: _SLen2Ticks = 0x4E;
    break;
case 0x2B: _SLen2Ticks = 0x50;
    break;
case 0x2C: _SLen2Ticks = 0x54;
    break;
case 0x2D: _SLen2Ticks = 0x58;
    break;
case 0x2E: _SLen2Ticks = 0x5A;
    break;
case 0x2F: _SLen2Ticks = 0x5C;
    break;
case 0x30: _SLen2Ticks = 0x60;
break;
}
return _SLen2Ticks;
}



}