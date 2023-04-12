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



public class SappyEvent {
string Key = "";
Attribute(Key.VB_VarDescription == "Event ID");

 // local variable(s) to hold property value(s)
int mvarTicks = 0; // local copy
Byte mvarCommandByte = 0; // local copy
Byte mvarParam1 = 0; // local copy
Byte mvarParam2 = 0; // local copy
Byte mvarParam3 = 0; // local copy
  public Byte Param3{ 
get {
Byte _Param3 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Param3
_Param3 = mvarParam3;
return _Param3;
}
set {
ttribute(_Param3.VB_Description == "Command Paramater 1");
 // used when assigning a value to the property, on the left side of an assignment.
 // Syntax: X.Param3 = 5
mvarParam3 = vData;
}
}



  



  public Byte Param2{ 
get {
Byte _Param2 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Param2
_Param2 = mvarParam2;
return _Param2;
}
set {
ttribute(_Param2.VB_Description == "Command Paramater 2");
 // used when assigning a value to the property, on the left side of an assignment.
 // Syntax: X.Param2 = 5
mvarParam2 = vData;
}
}



  



  public Byte Param1{ 
get {
Byte _Param1 = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.Param1
_Param1 = mvarParam1;
return _Param1;
}
set {
ttribute(_Param1.VB_Description == "Command Paramater 1");
 // used when assigning a value to the property, on the left side of an assignment.
 // Syntax: X.Param1 = 5
mvarParam1 = vData;
}
}



  



  public Byte CommandByte{ 
get {
Byte _CommandByte = default(Byte);
used(when retrieving value of a property, on the right side of an assignment.);
 // Syntax: Trace X.CommandByte
_CommandByte = mvarCommandByte;
return _CommandByte;
}
set {
ttribute(_CommandByte.VB_Description == "Sappy command byte:\\r\\nB1 - End Track \\r\\nB2 - Jump to position in event queue (Params: ticks)\\r\\nB3 - Jump to subroutine in event queue (Params: ticks)\\r\\nB4 - End of subroutine \\r\\nBB - Tempo (Params: BPM/2)\\r\\nBC - Transpose (Params: Signed Semitone)\\r\\nBD - Set Patch (Params: GM)\\r\\nBE - Set Volume (Params: GM)\\r\\nBF - Set Panning (Params: Signed GM)\\r\\nC0 - Set Pitch Bend (Params: Signed GM)\\r\\nC1 - Set Pitch Bend Range (Params: Semitones)\\r\\nC2 - Set vibrato depth (Params: Number)\\r\\nC4 - Set vibrato rate (Params: Number)\\r\\nCE - Note sustain off\\r\\nCF - Note sustain for long notes\\r\\nD1-FF - Play a note (Params: Note number, Velocity, ???)");
 // used when assigning a value to the property, on the left side of an assignment.
 // Syntax: X.CommandByte = 5
mvarCommandByte = vData;
}
}



  



  


  





}