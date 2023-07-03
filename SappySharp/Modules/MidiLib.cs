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

static partial class MidiLib
{
    [LibraryImport("winmm.dll")]
    public static partial int midiOutOpen(ref int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);
    [LibraryImport("winmm.dll")]
    public static partial int midiOutClose(int hMidiOut);
    [LibraryImport("winmm.dll")]
    public static partial int midiOutShortMsg(int hMidiOut, int dwMsg);


    // #define NoteOnCmd       0x90
    // #define NoteOffCmd      0x80
    // #define PgmChngCmd      0xC0
    // #define ControlCmd      0xB0
    // #define PolyPressCmd    0xA0
    // #define ChanPressCmd    0xD0
    // #define PchWheelCmd     0xE0
    // #define SysExCmd        0xF0

    public static int mdh = 0;
    public static bool midiOpened = false;

    public static int WantedMidiDevice = 0;

    public static void MidiOpen()
    {
        if (midiOutOpen(ref mdh, WantedMidiDevice, 0, 0, 0) != 0)
        {
            //Trace("Opened MIDI port");
            // TODO: (NOT SUPPORTED): On Error Resume Next
            FileOpen(4, System.Reflection.Assembly.GetExecutingAssembly().Location + "\\pmidi.tmp", OpenMode.Input);
            if (Err().Number == 0)
            {
                int i = 0;
                LineInput(4);
                Input(4, ref i);
                midiOutClose(i);
                FileClose(4);
            }
        }
        // allows for closing midi after a crash
        FileOpen(4, System.Reflection.Assembly.GetExecutingAssembly().Location + "\\pmidi.tmp", OpenMode.Output);
        Print(4, "Previous midi handle: (used in case it crashed last time)");
        Print(4, mdh);
        FileClose(4);
        midiOpened = true;
    }

    public static void MidiClose()
    {
        if (midiOpened)
        {
            //Trace("Closing MIDI");
            midiOutClose(mdh);
            midiOpened = false;
        }
    }

    public static void SelectInstrument(int channel, int patch)
    {
        if (midiOpened)
        {
            midiOutShortMsg(mdh, 0xC0 | patch * 256 | channel);
            //Trace("Set Patch for " + channel + " to " + patch);
        }
    }

    public static void ToneOn(int channel, int tone, int volume)
    {
        if (midiOpened)
        {
            if (tone < 0) tone = 0;
            if (tone > 127) tone = 127;
            midiOutShortMsg(mdh, 0x90 | tone * 256 | channel | volume * 65536);
            //Trace("Tone on: " + tone + " on " + channel + ", vol " + volume);
        }
    }

    public static void ToneOff(int channel, int tone)
    {
        if (midiOpened)
        {
            if (tone < 0) tone = 0;
            if (tone > 127) tone = 127;
            midiOutShortMsg(mdh, 0x80 | tone * 256 | channel);
            //Trace("Tone off: " + tone + " on " + channel);
        }
    }

    public static void SetChnVolume(int channel, int volume)
    {
        if (midiOpened)
        {
            midiOutShortMsg(mdh, 0xD0 | volume * 256 | channel);
            //Trace("volume for " + channel + " to " + volume);
        }
    }

    public static void SetChnPan(int channel, int pan)
    {
        if (midiOpened)
        {
            midiOutShortMsg(mdh, 0xB0 | 0xA * 256 | channel | pan * 65536);
            //midiOutShortMsg(mdh, 0xB0 | pan * 256 | channel | 0xA * 65536);
            //Trace("pan for " + channel + " to " + pan);
        }
    }

    public static void PitchWheel(int channel, int pit)
    {
        if (midiOpened)
        {
            midiOutShortMsg(mdh, 0xE0 | pit * 256 | channel);
            //Trace("Tone on: " + tone + " on " + channel + ", vol " + volume);
        }
    }
}
