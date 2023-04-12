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



public class SDirect {
string Key = "";

string mvarSampleID = ""; // local copy
Byte mvarEnvAttenuation = 0; // local copy
Byte mvarEnvDecay = 0; // local copy
Byte mvarEnvSustain = 0; // local copy
Byte mvarEnvRelease = 0; // local copy
Byte mvarRaw0 = 0; // local copy
Byte mvarRaw1 = 0; // local copy
Byte mvarGB1 = 0; // local copy
Byte mvarGB2 = 0; // local copy
Byte mvarGB3 = 0; // local copy
Byte mvarGB4 = 0; // local copy
bool mvarReverse = false; // local copy
bool mvarFixedPitch = false; // local copy
Byte mvarDrumTuneKey = 0; // local copy
  public enum DirectOutputTypes{ 
  dotDirect = 0
  , dotSquare1 = 1
  , dotSquare2 = 2
  , dotWave = 3
  , dotNoise = 4
  , dotUnk5 = 5
  , dotUnk6 = 6
  , dotUnk7 = 7
}
 // local variable(s) to hold property value(s)
DirectOutputTypes mvarOutputType = null; // local copy
  


  




  public Byte DrumTuneKey{ 
get {
Byte _DrumTuneKey = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.DrumTuneKey
_DrumTuneKey = mvarDrumTuneKey;
return _DrumTuneKey;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.DrumTuneKey = 5
mvarDrumTuneKey = vData;
}
}



  



  public bool FixedPitch{ 
get {
bool _FixedPitch = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.FixedPitch
_FixedPitch = mvarFixedPitch;
return _FixedPitch;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.FixedPitch = 5
mvarFixedPitch = vData;
}
}



  



  public bool Reverse{ 
get {
bool _Reverse = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Reverse
_Reverse = mvarReverse;
return _Reverse;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.Reverse = 5
mvarReverse = vData;
}
}



  



  public Byte GB4{ 
get {
Byte _GB4 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GB4
_GB4 = mvarGB4;
return _GB4;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GB4 = 5
mvarGB4 = vData;
}
}



  



  public Byte GB3{ 
get {
Byte _GB3 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GB3
_GB3 = mvarGB3;
return _GB3;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GB3 = 5
mvarGB3 = vData;
}
}



  



  public Byte GB2{ 
get {
Byte _GB2 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GB2
_GB2 = mvarGB2;
return _GB2;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GB2 = 5
mvarGB2 = vData;
}
}



  



  public Byte GB1{ 
get {
Byte _GB1 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GB1
_GB1 = mvarGB1;
return _GB1;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GB1 = 5
mvarGB1 = vData;
}
}



  



  public Byte Raw1{ 
get {
Byte _Raw1 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Raw1
_Raw1 = mvarRaw1;
return _Raw1;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.Raw1 = 5
mvarRaw1 = vData;
}
}



  



  public Byte Raw0{ 
get {
Byte _Raw0 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Raw0
_Raw0 = mvarRaw0;
return _Raw0;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.Raw0 = 5
mvarRaw0 = vData;
}
}



  



  


  



  


  



  


  



  


  



  


  





}