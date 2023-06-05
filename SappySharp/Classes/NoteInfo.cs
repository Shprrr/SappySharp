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

public class NoteInfo
{
    bool mvarEnabled = false;
    int mvarFModChannel = 0;
    byte mvarNoteNumber = 0;
    int mvarFrequency = 0;
    byte mvarVelocity = 0;
    byte mvarPatchNumber = 0;
    int mvarParentChannel = 0;
    string mvarSampleID = "";
    byte mvarUnknownValue = 0;
    public enum NoteOutputTypes
    {
        notDirect = 0
    , notSquare1 = 1
    , notSquare2 = 2
    , notWave = 3
    , notNoise = 4
    , notUnk5 = 5
    , notUnk6 = 6
    , notUnk7 = 7
    }
    public enum NotePhases
    {
        npInitial = 0
    , npAttack = 1
    , npDecay = 2
    , npSustain = 3
    , npRelease = 4
    , npNoteOff = 5
    }
    bool mvarNoteOff = false;
    NotePhases mvarNotePhase;
    NoteOutputTypes mvarOutputType;
    decimal mvarEnvStep = 0;
    decimal mvarEnvDestination = 0;
    decimal mvarEnvPosition = 0;
    byte mvarEnvAttenuation = 0;
    byte mvarEnvDecay = 0;
    byte mvarEnvSustain = 0;
    byte mvarEnvRelease = 0;
    decimal mvarWaitTicks = 0;
    string mvarKey = "";

    public decimal EnvPosition
    {
        get => mvarEnvPosition;
        set => mvarEnvPosition = value;
    }

    public decimal EnvDestination
    {
        get => mvarEnvDestination;
        set => mvarEnvDestination = value;
    }

    public decimal EnvStep
    {
        get => mvarEnvStep;
        set => mvarEnvStep = value;
    }

    public NotePhases Notephase
    {
        get => mvarNotePhase;
        set => mvarNotePhase = value;
    }

    public string Key { get => mvarKey; set => mvarKey = value; }

    public decimal WaitTicks
    {
        get => mvarWaitTicks;
        set => mvarWaitTicks = value;
    }

    public byte EnvRelease
    {
        get => mvarEnvRelease;
        set => mvarEnvRelease = value;
    }

    public byte PatchNumber
    {
        get => mvarPatchNumber;
        set => mvarPatchNumber = value;
    }

    public string SampleID
    {
        //get => mvarEnvRelease; // Bug ?
        get => mvarSampleID;
        set => mvarSampleID = value;
    }

    public byte EnvSustain
    {
        get => mvarEnvSustain;
        set => mvarEnvSustain = value;
    }

    public byte EnvDecay
    {
        get => mvarEnvDecay;
        set => mvarEnvDecay = value;
    }

    public byte EnvAttenuation
    {
        get => mvarEnvAttenuation;
        set => mvarEnvAttenuation = value;
    }

    public NoteOutputTypes outputtype
    {
        get => mvarOutputType;
        set => mvarOutputType = value;
    }

    public byte UnknownValue
    {
        get => mvarUnknownValue;
        set => mvarUnknownValue = value;
    }

    public int ParentChannel
    {
        get => mvarParentChannel;
        set => mvarParentChannel = value;
    }

    public byte Velocity
    {
        get => mvarVelocity;
        set => mvarVelocity = value;
    }

    public int Frequency
    {
        get => mvarFrequency;
        set => mvarFrequency = value;
    }

    public byte NoteNumber
    {
        get => mvarNoteNumber;
        set => mvarNoteNumber = value;
    }

    public int FModChannel
    {
        get => mvarFModChannel;
        set => mvarFModChannel = value;
    }

    public bool Enabled
    {
        get => mvarEnabled;
        set => mvarEnabled = value;
    }

    public bool NoteOff
    {
        get => mvarNoteOff;
        set => mvarNoteOff = value;
    }
}
