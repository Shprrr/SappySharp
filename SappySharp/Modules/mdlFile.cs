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



static class mdlFile {
public static List<string> VirtualFile = new List<string>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: VirtualFile(256 To 511)
public static List<int> WriteLastOffset = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: WriteLastOffset(0 To 511)
public static List<int> ReadLastOffset = new List<int>(); //  TODO: (NOT SUPPORTED) Array ranges not supported: ReadLastOffset(0 To 511)
public const int VF = 256;

  public static void FreeFileNumber() {
 _FreeFileNumber = null;
  _FreeFileNumber = FreeFile(1);
return _FreeFileNumber;
}

  public static void OpenFile(int filenumber, string Filename) {
  if(filenumber < VF)VBOpenFile(Filename, "Binary", filenumber); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Binary As #filenumber
  WriteOffset(filenumber, 0);
  ReadOffset(filenumber, 0);
}

  public static void OpenNewFile(int filenumber, string Filename) {
    if(filenumber < VF) {
    VBOpenFile(Filename, "Output", filenumber); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Open Filename For Output As #filenumber
    VBWriteFile("Print #filenumber, "x""); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Print #filenumber, __S1
    VBCloseFile(filenumber); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #filenumber
    } else {
    VirtualFile[filenumber] = "";
  }
  OpenFile(filenumber, Filename);
}

  public static void CloseFile(int filenumber) {
  VBCloseFile(filenumber); // TODO: (NOT SUPPORTED) VB File Access Suppressed.  Convert manually: Close #filenumber
}

  public static int WriteOffset(int filenumber, int offset = -1) {
int _WriteOffset = 0;
  WriteLastOffset[filenumber] = (offset > -1 ? offset : WriteLastOffset[filenumber]);
  _WriteOffset = WriteLastOffset[filenumber];
return _WriteOffset;
}

  public static void WriteByte(int filenumber, Byte Data, int offset = -1) {
  WriteOffset(filenumber, offset);
  Put(#filenumber, WriteOffset(filenumber) + 1, Data);
  WriteOffset(filenumber, WriteOffset(filenumber) + 1);
}

  public static void WriteBigEndian(int filenumber, Byte Width, int Data, int offset = -1) {
  Byte i = 0;
  WriteOffset(filenumber, offset);
    for (i = 0; i <= Width - 1; i += 1) {
    WriteByte(filenumber, (Data / ((16 ^ CLng(i)))) % 256);
  }
}

  public static void WriteLittleEndian(int filenumber, Byte Width, int Data, int offset = -1) {
  int i = 0;
  WriteOffset(filenumber, offset);
    for (i = Width - 1; i <= -1; i += 0) {
    WriteByte(filenumber, (Data / ((16 ^ CLng(i)))) % 256);
  }
}

  public static void WriteString(int filenumber, string Data, int offset = -1) {
   // Dim i As Long
  WriteOffset(filenumber, offset);
   // For i = 1 To Len(Data)
   // WriteByte FileNumber, IIf(Mid(Data, i, 1) = Chr(0), 0, Asc(Mid(Data, i, 1)))
   // Next i
  Put(#filenumber, WriteOffset(filenumber) + 1, Data);
  WriteOffset(filenumber, WriteOffset(filenumber) + Len(Data));
}

  public static int ReadOffset(int filenumber, int offset = -1) {
int _ReadOffset = 0;
  ReadLastOffset[filenumber] = (offset > -1 ? offset : ReadLastOffset[filenumber]);
  _ReadOffset = ReadLastOffset[filenumber];
return _ReadOffset;
}

  public static Byte ReadByte(int filenumber, int offset = -1) {
Byte _ReadByte = 0;
  ReadOffset(filenumber, offset);
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadByte);
  ReadOffset(filenumber, ReadOffset(filenumber) + 1);
return _ReadByte;
}

  public static int ReadVLQ(int filenumber, int offset = -1) {
int _ReadVLQ = 0;
  Byte i = 0;
  Byte retlen = 0;
  ReadOffset(filenumber, offset);
  Byte a = 0;
  retlen = 0;
    do {
    a = ReadByte(filenumber);
    _ReadVLQ = _ReadVLQ * ((2 ^ 7));
    _ReadVLQ = _ReadVLQ + ((a % &H80));
    retlen = retlen + 1;
  } while(!(retlen == 4 || a < 0x80));
  
return _ReadVLQ;
}
  public static int ReadBigEndian(int filenumber, Byte Width, int offset = -1) {
int _ReadBigEndian = 0;
  Byte i = 0;
  ReadOffset(filenumber, offset);
    for (i = Width - 1; i <= -1; i += 0) {
    _ReadBigEndian = _ReadBigEndian + ReadByte(filenumber) * ((256 ^ CLng(i)));
  }
return _ReadBigEndian;
}

  public static int ReadLittleEndian(int filenumber, Byte Width, int offset = -1) {
int _ReadLittleEndian = 0;
  int i = 0;
  ReadOffset(filenumber, offset);
    for (i = 0; i <= Width - 1; i += 1) {
    _ReadLittleEndian = _ReadLittleEndian + CLng(ReadByte(filenumber)) * ((CLng(256) ^ CLng(i)));
  }
return _ReadLittleEndian;
}

  public static string ReadString(int filenumber, int Length, int offset = -1) {
string _ReadString = "";
  // TODO: (NOT SUPPORTED): On Error GoTo fuck
  _ReadString = String(Length, " ");
   // Dim i As Long
  ReadOffset(filenumber, offset);
   // For i = 1 To Length
   // ReadString = ReadString & Chr(ReadByte(FileNumber))
   // Next i
  Get(#filenumber, ReadOffset(filenumber) + 1, _ReadString);
  ReadOffset(filenumber, ReadOffset(filenumber) + Length);
  return _ReadString;
  fuck:;
  Trace("fuck");
return _ReadString;
}

  public static int ReadGBAROMPointer(int filenumber, int offset = -1) {
int _ReadGBAROMPointer = 0;
  ReadOffset(filenumber, offset);
  _ReadGBAROMPointer = ReadLittleEndian(filenumber, 4);
    if(_ReadGBAROMPointer < 0x8000000 || _ReadGBAROMPointer > 0x9FFFFFF) {
    _ReadGBAROMPointer = -1;
    } else {
    _ReadGBAROMPointer = _ReadGBAROMPointer - 0x8000000;
  }
return _ReadGBAROMPointer;
}

  public static int GBAROMPointerToOffset(int GBAROMPointer) {
int _GBAROMPointerToOffset = 0;
    if(GBAROMPointer < 0x8000000 || GBAROMPointer > 0x9FFFFFF) {
    _GBAROMPointerToOffset = -1;
    } else {
    _GBAROMPointerToOffset = GBAROMPointer - 0x8000000;
  }
return _GBAROMPointerToOffset;
}


}