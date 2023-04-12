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



public class SChannels {
 // local variable to hold collection
Collection mCol = null;
  public void Clear() {
    while(mCol.count > 0) {
    mCol.Remove(1);
  }
  
}
  public SChannel Add(ref string sKey) {
SChannel _Add = null;
   // create a new object
  SChannel objNewMember = null;
  objNewMember = new SChannel();
  
  
   // set the properties passed into the method
  objNewMember.Key = sKey;
  objNewMember.Notes = new SNotes();
  objNewMember.PatchNumber = 0;
  objNewMember.Panning = 0x40;
  objNewMember.MainVolume = 100;
  objNewMember.mute = false;
  objNewMember.Enabled = true;
  objNewMember.PitchBend = 0x40;
  objNewMember.PitchBendRange = 2;
  objNewMember.Sustain = false;
  objNewMember.Transpose = 0;
  objNewMember.Subroutines = new SSubroutines();
  objNewMember.EventQueue = new SappyEventQueue();
  objNewMember.LoopPointer = 0;
  objNewMember.SubCountAtLoop = 1;
  objNewMember.SubroutineCounter = 1;
  objNewMember.ProgramCounter = 1;
  objNewMember.WaitTicks = -1;
  mCol.Add(objNewMember);
  
   // return the object created
  _Add = objNewMember;
  objNewMember = null;
  
  
return _Add;
}

  public SChannel Item{ 
get {
SChannel _Item = default(SChannel);
ttribute(_Item.VB_UserMemId == 0);

 // used when referencing an element in the collection
 // vntIndexKey contains either the Index or Key to the collection,
 // this is why it is declared as a Variant
 // Syntax: Set foo = x.Item(xyz) or Set foo = x.Item(5)
_Item = mCol(vntIndexKey);
return _Item;
}
}




  public int count{ 
get {
int _count = default(int);
 // used when retrieving the number of elements in the
 // collection. Syntax: Trace x.Count
_count = mCol._count;
return _count;
}
}



  public void Remove(ref dynamic vntIndexKey) {
   // used when removing an element from the collection
   // vntIndexKey contains either the Index or Key, which is why
   // it is declared as a Variant
   // Syntax: x.Remove(xyz)
  
  
  mCol.Remove(vntIndexKey);
}


  public IUnknown NewEnum{ 
get {
IUnknown _NewEnum = default(IUnknown);
ttribute(_NewEnum.VB_UserMemId == -4);
Attribute(_NewEnum.VB_MemberFlags == "40");
 // this property allows you to enumerate
 // this collection with the For...Each syntax
_NewEnum = mCol.[_NewEnum];
return _NewEnum;
}
}



  private void Class_Initialize() {
   // creates the collection when this class is created
  mCol = new Collection();
}


  private void Class_Terminate() {
   // destroys collection when this class is terminated
  mCol = null;
}



}