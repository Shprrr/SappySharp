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

namespace SappySharp.Classes;

public class SChannel
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public string Key = "";

    SNotes mvarNotes = null; // local copy
    public enum ChannelOutputTypes
    {
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
    byte mvarPatchNumber = 0; // local copy
    int mvarPanning = 0; // local copy
    byte mvarMainVolume = 0; // local copy
    byte mvarPitchBend = 0; // local copy
    byte mvarVibratoDepth = 0; // local copy
    byte mvarVibratoRate = 0; // local copy
    int mvarPitchBendRange = 0; // local copy
    bool mvarSustain = false; // local copy
    int mvarTranspose = 0; // local copy
    SSubroutines mvarSubroutines = null; // local copy
    SappyEventQueue mvarEventQueue = null; // local copy
    int mvarTrackPointer = 0; // local copy
    int mvarLoopPointer = 0; // local copy
    int mvarProgramCounter = 0; // local copy
    bool mvarMute = false;
    ChannelOutputTypes mvarOutputType;
    int mvarReturnPointer = 0; // local copy
    bool mvarInSubroutine = false; // localcopy
    int mvarSubroutineCounter = 0; // localcopy
    int mvarSubCountAtLoop = 0; // localcopy
    decimal mvarWaitTicks = 0; // localcopy
    int mvarTrackLengthInBytes = 0;
    // local variable(s) to hold property value(s)

    /// <summary>
    /// Output Device
    /// </summary>
    public ChannelOutputTypes outputtype { get => mvarOutputType; set => mvarOutputType = value; }

    public int TrackPointer
    {
        get => mvarTrackPointer;
        set => mvarTrackPointer = value;
    }

    /// <summary>
    /// Pointer to loop point in the event queue
    /// </summary>
    public int LoopPointer
    {
        get => mvarLoopPointer;
        set => mvarLoopPointer = value;
    }

    public bool Enabled { get => mvarEnabled; set => mvarEnabled = value; }

    public bool mute
    {
        get => mvarMute;
        set => mvarMute = value;
    }

    public decimal WaitTicks { get => mvarWaitTicks; set => mvarWaitTicks = value; }

    public int ReturnPointer
    {
        get => mvarReturnPointer;
        set => mvarReturnPointer = value;
    }

    public bool InSubroutine
    {
        get => mvarInSubroutine;
        set => mvarInSubroutine = value;
    }

    public int SubroutineCounter
    {
        get => mvarSubroutineCounter;
        set => mvarSubroutineCounter = value;
    }

    public int SubCountAtLoop
    {
        get => mvarSubCountAtLoop;
        set => mvarSubCountAtLoop = value;
    }

    public int ProgramCounter
    {
        get => mvarProgramCounter;
        set => mvarProgramCounter = value;
    }

    /// <summary>
    /// Channel's event queue
    /// </summary>
    public SappyEventQueue EventQueue
    {
        get => mvarEventQueue;
        set => mvarEventQueue = value;
    }

    /// <summary>
    /// Collection of the channel's subroutines
    /// </summary>
    public SSubroutines Subroutines
    {
        get => mvarSubroutines;
        set => mvarSubroutines = value;
    }

    public int Transpose
    {
        get => mvarTranspose;
        set => mvarTranspose = value;
    }

    public bool Sustain { get => mvarSustain; set => mvarSustain = value; }

    public int PitchBendRange
    {
        get => mvarPitchBendRange;
        set => mvarPitchBendRange = value;
    }

    public byte PitchBend { get => mvarPitchBend; set => mvarPitchBend = value; }

    public byte VibratoRate
    {
        get => mvarVibratoRate;
        set => mvarVibratoRate = value;
    }

    public byte VibratoDepth
    {
        get => mvarVibratoDepth;
        set => mvarVibratoDepth = value;
    }

    public byte MainVolume
    {
        get => mvarMainVolume;
        set => mvarMainVolume = value;
    }

    public int Panning
    {
        get => mvarPanning;
        set => mvarPanning = value;
    }

    public byte PatchNumber { get => mvarPatchNumber; set => mvarPatchNumber = value; }

    /// <summary>
    /// Collection of Notes
    /// </summary>
    public SNotes Notes
    {
        get => mvarNotes;
        set => mvarNotes = value;
    }

    public int TrackLengthInBytes
    {
        get => mvarTrackLengthInBytes;
        set => mvarTrackLengthInBytes = value;
    }
}
