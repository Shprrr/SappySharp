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

public class SDirect
{
    public string Key = "";

    string mvarSampleID = ""; // local copy
    byte mvarEnvAttenuation = 0; // local copy
    byte mvarEnvDecay = 0; // local copy
    byte mvarEnvSustain = 0; // local copy
    byte mvarEnvRelease = 0; // local copy
    byte mvarRaw0 = 0; // local copy
    byte mvarRaw1 = 0; // local copy
    byte mvarGB1 = 0; // local copy
    byte mvarGB2 = 0; // local copy
    byte mvarGB3 = 0; // local copy
    byte mvarGB4 = 0; // local copy
    bool mvarReverse = false; // local copy
    bool mvarFixedPitch = false; // local copy
    byte mvarDrumTuneKey = 0; // local copy
    public enum DirectOutputTypes
    {
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
    DirectOutputTypes mvarOutputType; // local copy

    public DirectOutputTypes outputtype { get => mvarOutputType; set => mvarOutputType = value; }

    public byte DrumTuneKey
    {
        get => mvarDrumTuneKey;
        set => mvarDrumTuneKey = value;
    }

    public bool FixedPitch
    {
        get => mvarFixedPitch;
        set => mvarFixedPitch = value;
    }

    public bool Reverse
    {
        get => mvarReverse;
        set => mvarReverse = value;
    }

    public byte GB4
    {
        get => mvarGB4;
        set => mvarGB4 = value;
    }

    public byte GB3
    {
        get => mvarGB3;
        set => mvarGB3 = value;
    }

    public byte GB2
    {
        get => mvarGB2;
        set => mvarGB2 = value;
    }

    public byte GB1
    {
        get => mvarGB1;
        set => mvarGB1 = value;
    }

    public byte Raw1
    {
        get => mvarRaw1;
        set => mvarRaw1 = value;
    }

    public byte Raw0
    {
        get => mvarRaw0;
        set => mvarRaw0 = value;
    }

    public byte EnvRelease { get => mvarEnvRelease; set => mvarEnvRelease = value; }

    public byte EnvSustain { get => mvarEnvSustain; set => mvarEnvSustain = value; }

    public byte EnvDecay { get => mvarEnvDecay; set => mvarEnvDecay = value; }

    public byte EnvAttenuation { get => mvarEnvAttenuation; set => mvarEnvAttenuation = value; }

    public string SampleID { get => mvarSampleID; set => mvarSampleID = value; }
}
