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



public class NoteInfos {
Collection mCol = null;

  public void Clear() {
    while(mCol.count > 0) {
    mCol.Remove(1);
  }
}

  public NoteInfo Add(bool Enabled, int FModChannel, Byte NoteNumber, int Frequency, Byte Velocity, int ParentChannel, Byte UnknownValue, NoteOutputTypes outputtype, Byte EnvAttenuation, Byte EnvDecay, Byte EnvSustain, Byte EnvRelease, int WaitTicks, Byte PatchNumber, string sKey) {
NoteInfo _Add = null;
  NoteInfo objNewMember = null;
  objNewMember = new NoteInfo();
  
  objNewMember.Key = sKey;
  objNewMember.NoteOff = false;
  objNewMember.Enabled = Enabled;
  objNewMember.FModChannel = FModChannel;
  objNewMember.NoteNumber = NoteNumber;
  objNewMember.Frequency = Frequency;
  objNewMember.Velocity = Velocity;
  objNewMember.PatchNumber = PatchNumber;
  objNewMember.ParentChannel = ParentChannel;
  objNewMember.SampleID = SampleID;
  objNewMember.UnknownValue = UnknownValue;
  objNewMember.outputtype = outputtype;
  objNewMember.Notephase = npInitial;
  objNewMember.EnvDestination = 0;
  objNewMember.EnvStep = 0;
  objNewMember.EnvPosition = 0;
  objNewMember.EnvAttenuation = EnvAttenuation;
  objNewMember.EnvDecay = EnvDecay;
  objNewMember.EnvSustain = EnvSustain;
  objNewMember.EnvRelease = EnvRelease;
  objNewMember.WaitTicks = WaitTicks;
    if(Len(sKey) == 0) {
    mCol.Add(objNewMember);
    } else {
    mCol.Add(objNewMember, sKey);
  }
  
  _Add = objNewMember;
  objNewMember = null;
return _Add;
}

  

  

  public void Remove(ref dynamic vntIndexKey) {
  mCol.Remove(vntIndexKey);
}

  

  private void Class_Initialize() {
  mCol = new Collection();
}

  private void Class_Terminate() {
  mCol = null;
}



}