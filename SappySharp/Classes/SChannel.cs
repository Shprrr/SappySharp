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



public class SChannel {
string Key = "";
Attribute(Key.VB_VarDescription == "Channel ID");

SNotes mvarNotes = null; // local copy
  public enum ChannelOutputTypes{ 
  cotDirect = 0
  , cotSquare1 = 1
  , cotSquare2 = 2
  , cotWave = 3
  , cotNoise = 4
  , cotUnk5 = 5
  , cotUnk6 = 6
  , cotUnk7 = 7
  , cotMultiSample = 8
  , cotDrumKit = 9
  , cotNull = 255
}
bool mvarEnabled = false;
Byte mvarPatchNumber = 0; // local copy
int mvarPanning = 0; // local copy
Byte mvarMainVolume = 0; // local copy
Byte mvarPitchBend = 0; // local copy
Byte mvarVibratoDepth = 0; // local copy
Byte mvarVibratoRate = 0; // local copy
int mvarPitchBendRange = 0; // local copy
bool mvarSustain = false; // local copy
int mvarTranspose = 0; // local copy
SSubroutines mvarSubroutines = null; // local copy
SappyEventQueue mvarEventQueue = null; // local copy
int mvarTrackPointer = 0; // local copy
int mvarLoopPointer = 0; // local copy
int mvarProgramCounter = 0; // local copy
bool mvarMute = false;
ChannelOutputTypes mvarOutputType = null;
int mvarReturnPointer = 0; // local copy
bool mvarInSubroutine = false; // localcopy
int mvarSubroutineCounter = 0; // localcopy
int mvarSubCountAtLoop = 0; // localcopy
decimal mvarWaitTicks = 0; // localcopy
int mvarTrackLengthInBytes = 0;
 // local variable(s) to hold property value(s)

  


  


  public int TrackPointer{ 
get {
int _TrackPointer = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.TrackPointer
_TrackPointer = mvarTrackPointer;
return _TrackPointer;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.TrackPointer = 5
mvarTrackPointer = vData;
}
}



  

  public int LoopPointer{ 
get {
int _LoopPointer = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_LoopPointer = mvarLoopPointer;
return _LoopPointer;
}
set {
ttribute(_LoopPointer.VB_Description == "Pointer to loop point in the event queue");
 // used when assigning a value to the property, on the left side of an assignment.
 // Syntax: X.LoopPointer = 5
mvarLoopPointer = vData;
}
}



  

  

  

  public bool mute{ 
get {
bool _mute = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_mute = mvarMute;
return _mute;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarMute = vData;
}
}


  



  

  

  public int ReturnPointer{ 
get {
int _ReturnPointer = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_ReturnPointer = mvarReturnPointer;
return _ReturnPointer;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarReturnPointer = vData;
}
}



  




  public bool InSubroutine{ 
get {
bool _InSubroutine = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_InSubroutine = mvarInSubroutine;
return _InSubroutine;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarInSubroutine = vData;
}
}



  

  public int SubroutineCounter{ 
get {
int _SubroutineCounter = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_SubroutineCounter = mvarSubroutineCounter;
return _SubroutineCounter;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarSubroutineCounter = vData;
}
}



  


  public int SubCountAtLoop{ 
get {
int _SubCountAtLoop = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_SubCountAtLoop = mvarSubCountAtLoop;
return _SubCountAtLoop;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarSubCountAtLoop = vData;
}
}



  



  public int ProgramCounter{ 
get {
int _ProgramCounter = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopPointer
_ProgramCounter = mvarProgramCounter;
return _ProgramCounter;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopPointer = 5
mvarProgramCounter = vData;
}
}



  



  public SappyEventQueue EventQueue{ 
get {
SappyEventQueue _EventQueue = default(SappyEventQueue);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.EventQueue
_EventQueue = mvarEventQueue;
return _EventQueue;
}
set {
ttribute(_EventQueue.VB_Description == "Channel's event queue");
 // used when assigning an Object to the property, on the left side of a Set statement.
 // Syntax: Set x.EventQueue = Form1
mvarEventQueue = vData;
}
}



  



  public SSubroutines Subroutines{ 
get {
SSubroutines _Subroutines = default(SSubroutines);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Subroutines
_Subroutines = mvarSubroutines;
return _Subroutines;
}
set {
ttribute(_Subroutines.VB_Description == "Collection of the channel's subroutines");
 // used when assigning an Object to the property, on the left side of a Set statement.
 // Syntax: Set x.Subroutines = Form1
mvarSubroutines = vData;
}
}



  



  public int Transpose{ 
get {
int _Transpose = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Transpose
_Transpose = mvarTranspose;
return _Transpose;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.Transpose = 5
mvarTranspose = vData;
}
}



  



  


  



  public int PitchBendRange{ 
get {
int _PitchBendRange = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.PitchBendRange
_PitchBendRange = mvarPitchBendRange;
return _PitchBendRange;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.PitchBendRange = 5
mvarPitchBendRange = vData;
}
}



  



  


  

  public Byte VibratoRate{ 
get {
Byte _VibratoRate = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.VibratoRate
_VibratoRate = mvarVibratoRate;
return _VibratoRate;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.VibratoRate = 5
mvarVibratoRate = vData;
}
}



  
  public Byte VibratoDepth{ 
get {
Byte _VibratoDepth = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.VibratoDepth
_VibratoDepth = mvarVibratoDepth;
return _VibratoDepth;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.VibratoDepth = 5
mvarVibratoDepth = vData;
}
}



  
  public Byte MainVolume{ 
get {
Byte _MainVolume = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.MainVolume
_MainVolume = mvarMainVolume;
return _MainVolume;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.MainVolume = 5
mvarMainVolume = vData;
}
}



  



  public int Panning{ 
get {
int _Panning = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Panning
_Panning = mvarPanning;
return _Panning;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.Panning = 5
mvarPanning = vData;
}
}



  



  


  




  public SNotes Notes{ 
get {
SNotes _Notes = default(SNotes);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Notes
_Notes = mvarNotes;
return _Notes;
}
set {
ttribute(_Notes.VB_Description == "Collection of Notes");
 // used when assigning an Object to the property, on the left side of a Set statement.
 // Syntax: Set x.Notes = Form1
mvarNotes = vData;
}
}



  




  public int TrackLengthInBytes{ 
get {
int _TrackLengthInBytes = default(int);
_TrackLengthInBytes = mvarTrackLengthInBytes;
return _TrackLengthInBytes;
}
set {
mvarTrackLengthInBytes = vData;
}
}



  



}