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



public class SSample {
string Key = "";

 // local variable(s) to hold property value(s)
string mvarSampleData = ""; // local copy
int mvarLoopStart = 0; // local copy
bool mvarLoopEnable = false; // local copy
int mvarSize = 0; // local copy
int mvarFrequency = 0; // local copy
int mvarFModSample = 0; // local copy
 // local variable(s) to hold property value(s)
bool mvarGBWave = false; // local copy

List<Byte> mvarSampleDataB = new List<Byte>();
int mvarSampleDataL = 0;
  public int SampleDataB{ 
get {
int _SampleDataB = default(int);
_SampleDataB = mvarSampleDataB[Index];
return _SampleDataB;
}
set {
  if(Index > mvarSampleDataL) {
  // TODO: (NOT SUPPORTED): ReDim Preserve mvarSampleDataB(mvarSampleDataL) As Byte
}
if(mvarSampleDataL == 0)// TODO: (NOT SUPPORTED): ReDim Preserve mvarSampleDataB(0) As Byte

mvarSampleDataB[Index] = newval;
}
}


public void ReadSampleDataFromFile(int fn, ref int tsize) {
// TODO: (NOT SUPPORTED): ReDim Preserve mvarSampleDataB(tsize) As Byte
mvarSampleDataL = tsize;
Get(#fn, ReadOffset(filenumber) + 1, mvarSampleDataB());
}
public void SaveSampleDataToFile(int fn) {
Put(#fn, WriteOffset(filenumber) + 1, mvarSampleDataB());
}


public bool GBWave{ 
get {
bool _GBWave = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GBWave
_GBWave = mvarGBWave;
return _GBWave;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GBWave = 5
mvarGBWave = vData;
}
}






public bool LoopEnable{ 
get {
bool _LoopEnable = default(bool);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.GBWave
_LoopEnable = mvarLoopEnable;
return _LoopEnable;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.GBWave = 5
mvarLoopEnable = vData;
}
}







public int FModSample{ 
get {
int _FModSample = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.FModSample
_FModSample = mvarFModSample;
return _FModSample;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.FModSample = 5
mvarFModSample = vData;
}
}





















public int loopstart{ 
get {
int _loopstart = default(int);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.LoopStart
_loopstart = mvarLoopStart;
return _loopstart;
}
set {
used(when assigning a value to the property, on the left side of an assignment.);
 // Syntax: X.LoopStart = 5
mvarLoopStart = vData;
}
}
















}