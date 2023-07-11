using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;

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
            FileOpen(4, AppContext.BaseDirectory + "\\pmidi.tmp", OpenMode.Input);
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
        FileOpen(4, AppContext.BaseDirectory + "\\pmidi.tmp", OpenMode.Output);
        PrintLine(4, "Previous midi handle: (used in case it crashed last time)");
        PrintLine(4, mdh);
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
