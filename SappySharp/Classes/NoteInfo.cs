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



public class NoteInfo {
bool mvarEnabled = false;
int mvarFModChannel = 0;
Byte mvarNoteNumber = 0;
int mvarFrequency = 0;
Byte mvarVelocity = 0;
Byte mvarPatchNumber = 0;
int mvarParentChannel = 0;
string mvarSampleID = "";
Byte mvarUnknownValue = 0;
  public enum NoteOutputTypes{ 
  notDirect = 0
  , notSquare1 = 1
  , notSquare2 = 2
  , notWave = 3
  , notNoise = 4
  , notUnk5 = 5
  , notUnk6 = 6
  , notUnk7 = 7
}
  public enum NotePhases{ 
  npInitial = 0
  , npAttack = 1
  , npDecay = 2
  , npSustain = 3
  , npRelease = 4
  , npNoteOff = 5
}
bool mvarNoteOff = false;
NotePhases mvarNotePhase = null;
NoteOutputTypes mvarOutputType = null;
decimal mvarEnvStep = 0;
decimal mvarEnvDestination = 0;
decimal mvarEnvPosition = 0;
Byte mvarEnvAttenuation = 0;
Byte mvarEnvDecay = 0;
Byte mvarEnvSustain = 0;
Byte mvarEnvRelease = 0;
decimal mvarWaitTicks = 0;
string mvarKey = "";

  public decimal EnvPosition{ 
get {
decimal _EnvPosition = default(decimal);
_EnvPosition = mvarEnvPosition;
return _EnvPosition;
}
set {
mvarEnvPosition = vData;
}
}

  

  public decimal EnvDestination{ 
get {
decimal _EnvDestination = default(decimal);
_EnvDestination = mvarEnvDestination;
return _EnvDestination;
}
set {
mvarEnvDestination = vData;
}
}

  

  public decimal EnvStep{ 
get {
decimal _EnvStep = default(decimal);
_EnvStep = mvarEnvStep;
return _EnvStep;
}
set {
mvarEnvStep = vData;
}
}

  

  public NotePhases Notephase{ 
get {
NotePhases _Notephase = default(NotePhases);
_Notephase = mvarNotePhase;
return _Notephase;
}
set {
mvarNotePhase = vData;
}
}

  

  
  

  public decimal WaitTicks{ 
get {
decimal _WaitTicks = default(decimal);
_WaitTicks = mvarWaitTicks;
return _WaitTicks;
}
set {
mvarWaitTicks = vData;
}
}

  

  public Byte EnvRelease{ 
get {
Byte _EnvRelease = default(Byte);
_EnvRelease = mvarEnvRelease;
return _EnvRelease;
}
set {
mvarEnvRelease = vData;
}
}

  

  public Byte PatchNumber{ 
get {
Byte _PatchNumber = default(Byte);
_PatchNumber = mvarPatchNumber;
return _PatchNumber;
}
set {
mvarPatchNumber = vData;
}
}

  

  public string SampleID{ 
get {
string _SampleID = default(string);
_SampleID = mvarEnvRelease;
return _SampleID;
}
set {
mvarSampleID = vData;
}
}

  

  public Byte EnvSustain{ 
get {
Byte _EnvSustain = default(Byte);
_EnvSustain = mvarEnvSustain;
return _EnvSustain;
}
set {
mvarEnvSustain = vData;
}
}

  

  public Byte EnvDecay{ 
get {
Byte _EnvDecay = default(Byte);
_EnvDecay = mvarEnvDecay;
return _EnvDecay;
}
set {
mvarEnvDecay = vData;
}
}

  

  public Byte EnvAttenuation{ 
get {
Byte _EnvAttenuation = default(Byte);
_EnvAttenuation = mvarEnvAttenuation;
return _EnvAttenuation;
}
set {
mvarEnvAttenuation = vData;
}
}

  

  public NoteOutputTypes outputtype{ 
get {
NoteOutputTypes _outputtype = default(NoteOutputTypes);
_outputtype = mvarOutputType;
return _outputtype;
}
set {
mvarOutputType = vData;
}
}

  

  public Byte UnknownValue{ 
get {
Byte _UnknownValue = default(Byte);
_UnknownValue = mvarUnknownValue;
return _UnknownValue;
}
set {
mvarUnknownValue = vData;
}
}

  

  public int ParentChannel{ 
get {
int _ParentChannel = default(int);
_ParentChannel = mvarParentChannel;
return _ParentChannel;
}
set {
mvarParentChannel = vData;
}
}

  

  public Byte Velocity{ 
get {
Byte _Velocity = default(Byte);
_Velocity = mvarVelocity;
return _Velocity;
}
set {
mvarVelocity = vData;
}
}

  

  public int Frequency{ 
get {
int _Frequency = default(int);
_Frequency = mvarFrequency;
return _Frequency;
}
set {
mvarFrequency = vData;
}
}

  

  public Byte NoteNumber{ 
get {
Byte _NoteNumber = default(Byte);
_NoteNumber = mvarNoteNumber;
return _NoteNumber;
}
set {
mvarNoteNumber = vData;
}
}

  

  public int FModChannel{ 
get {
int _FModChannel = default(int);
_FModChannel = mvarFModChannel;
return _FModChannel;
}
set {
mvarFModChannel = vData;
}
}

  

  public bool Enabled{ 
get {
bool _Enabled = default(bool);
_Enabled = mvarEnabled;
return _Enabled;
}
set {
mvarEnabled = vData;
}
}

  

  public bool NoteOff{ 
get {
bool _NoteOff = default(bool);
_NoteOff = mvarNoteOff;
return _NoteOff;
}
set {
mvarNoteOff = vData;
}
}

  



}