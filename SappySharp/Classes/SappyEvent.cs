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
using System.Text;
using System.Threading.Tasks;
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

namespace SappySharp.Classes;

public class SappyEvent
{
    /// <summary>
    /// Event ID
    /// </summary>
    public string Key = "";

    // local variable(s) to hold property value(s)
    int mvarTicks = 0; // local copy
    byte mvarCommandByte = 0; // local copy
    byte mvarParam1 = 0; // local copy
    byte mvarParam2 = 0; // local copy
    byte mvarParam3 = 0; // local copy
    /// <summary>
    /// Command Paramater 1
    /// </summary>
    public byte Param3
    {
        get => mvarParam3;
        set => mvarParam3 = value;
    }

    /// <summary>
    /// Command Paramater 2
    /// </summary>
    public byte Param2
    {
        get => mvarParam2;
        set => mvarParam2 = value;
    }

    /// <summary>
    /// Command Paramater 1
    /// </summary>
    public byte Param1
    {
        get => mvarParam1;
        set => mvarParam1 = value;
    }

    /// <summary>
    /// Sappy command byte:<br/>
    /// B1 - End Track <br/>
    /// B2 - Jump to position in event queue (Params: ticks)<br/>
    /// B3 - Jump to subroutine in event queue (Params: ticks)<br/>
    /// B4 - End of subroutine <br/>
    /// BB - Tempo (Params: BPM/2)<br/>
    /// BC - Transpose (Params: Signed Semitone)<br/>
    /// BD - Set Patch (Params: GM)<br/>
    /// BE - Set Volume (Params: GM)<br/>
    /// BF - Set Panning (Params: Signed GM)<br/>
    /// C0 - Set Pitch Bend (Params: Signed GM)<br/>
    /// C1 - Set Pitch Bend Range (Params: Semitones)<br/>
    /// C2 - Set vibrato depth (Params: Number)<br/>
    /// C4 - Set vibrato rate (Params: Number)<br/>
    /// CE - Note sustain off<br/>
    /// CF - Note sustain for long notes<br/>
    /// D1-FF - Play a note (Params: Note number, Velocity, ???)
    /// </summary>
    public byte CommandByte
    {
        get => mvarCommandByte;
        set => mvarCommandByte = value;
    }

    public int Ticks { get => mvarTicks; set => mvarTicks = value; }
}
