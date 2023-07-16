using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using SappySharp.Forms;
using TimerMM;
using static FMod;
using static mdlFile;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.VBMath;
using static MidiLib;
using static modSappy;
using static mTrace;
using static SapPlayer;
using static SappySharp.Classes.NoteInfo;
using static SappySharp.Classes.SChannel;
using static SappySharp.Classes.SDirect;
using static VBExtension;

namespace SappySharp.Classes;

/// <summary>
/// All routines for the processing of Sappy code
/// </summary>
public partial class clsSappyDecoder
{
    // #Const XDebug = True

    [DllImport("kernel32", EntryPoint = "DeleteFileA")]
    private static extern int DeleteFile(string lpFileName);
    [LibraryImport("kernel32.dll")]
    private static partial int GetTickCount();

    private const bool ErrorChecking = false;
    private const double GBSquareMulti = 0.5;
    private const double GBWaveMulti = 0.5;
    private const double GBNoiseMulti = 0.5;
    private int lasttempo = 0;

    /// <summary>
    /// Collection of channels.
    /// </summary>
    public SChannels SappyChannels = new();
    public NoteInfos NoteQueue = new();
    private NoteInfo[] NoteArray = new NoteInfo[32];

    public CTimerMM EventProcessor = null;

    public int TotalTicks = 0; // by Kawa
    public int TotalMSecs = 0; // by Kawa
    public int Beats = 0; // by Kawa
    public delegate void EmptyEventHandler();
    public event EmptyEventHandler SongLoop; // by Kawa
    /// <summary>
    /// Raised when the song finishes playing.
    /// </summary>
    public event EmptyEventHandler SongFinish;
    /// <summary>
    /// Raised when there's new tracking info to display.
    /// </summary>
    public event EmptyEventHandler UpdateDisplay;
    public delegate void ChangedTempoEventHandler(int newtempo);
    /// <summary>
    /// Raised when a Tempo command is found in the song data
    /// </summary>
    public event ChangedTempoEventHandler ChangedTempo;
    public delegate void PlayedANoteEventHandler(byte channelid, byte notenum, byte lenno);
    public event PlayedANoteEventHandler PlayedANote;
    public delegate void LoadingEventHandler(int status);
    public event LoadingEventHandler Loading;
    public delegate void BeatEventHandler(int beats);
    public event BeatEventHandler Beat;

    public SSamples SamplePool = new();
    public SDirects Directs = new();
    public SDrumKits DrumKits = new();
    public SInstruments Instruments = new();

    // local variable(s) to hold property value(s)
    private decimal mvarNoteFrameCounter = 0;
    private decimal mvarTickCounter = 0;
    private decimal mvarLastTick = 0;
    private string mvarFileName = "";
    private int mvarTranspose = 0;
    private int mvarSongListPointer = 0;
    private int mvarSongPointer = 0;
    private int mvarSongNumber = 0;
    private int mvarLayer = 0;
    private int mvarInstrumentTablePointer = 0;
    private int mvarCurrentSong = 0;
    private int incr = 0;
    private byte mvarGB1Chan = 0;
    private byte mvarGB2Chan = 0;
    private byte mvarGB3Chan = 0;
    private byte mvarGB4Chan = 0;
    private bool mvarPlaying = false;
    private int mvarTempo = 0;

    public enum SongOutputTypes
    {
        sotNull = 0
    , sotWave = 1
    , sotMIDI = 2
    }

    private SongOutputTypes mvarOutputType;
    private int mvarGlobalVolume = 0; // by Kawa

    private const int GBWaveBaseFreq = 880;
    private const int SappyPPQN = 24;

    private int[] MidiPatchMap = new int[128]; // by Kawa
    private int[] MidiPatchTrans = new int[128];
    private int[] MidiDrumMap = new int[128];
    private int[] EarPiercers = new int[128];
    private int EarPiercerCnt = 0;

    private class tBufferRawMidiEvent
    {
        public int RawDelta;
        public int Ticks;
        public string EventCode;
    }
    // Private Type tBufferTrack
    //   Length As Long
    //   Events(-1 To 5000) As tBufferRawMidiEvent
    // End Type
    // Private BufferTrack(32) As tBufferTrack
    private tBufferRawMidiEvent PreviousEvent = null;
    private bool Recording = false;
    private int midifile = 0;

    private void BufferEvent(string EventCode, int Ticks)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo hell
        if (Recording == false) return;
        if (midifile != 42) return;
        tBufferRawMidiEvent newevent = new()
        {
            // Ticks = Ticks * (mvarTempo / 60) // / (60000000 / SappyPPQN) // MATH!
            Ticks = Ticks,
            RawDelta = Ticks - PreviousEvent.Ticks,
            EventCode = EventCode
        };
        WriteVarLen(midifile, newevent.RawDelta);
        FilePutObject(midifile, newevent.EventCode);
        PreviousEvent = newevent;
        return;
    hell:;
        Console.WriteLine("fucked up: eventcode: " + EventCode + ", ticks: " + Ticks);
    }

    public int GlobalVolume // by Kawa
    {
        get => mvarGlobalVolume;
        set
        {
            mvarGlobalVolume = value;
            // If PlayingNow = True Then
            FSOUND_SetSFXMasterVolume(value);
        }
    }

    // Public Property Get MidiMap(ByVal o As Integer) As Integer  // by Kawa
    // MidiMap = MidiPatchMap(o)
    // End Property
    // Public Property Let MidiMap(ByVal o As Integer, ByVal n As Integer) // by Kawa
    // MidiPatchMap(o) = n
    // End Property
    public void SetMidiPatchMap(int Index, int instrument, int Transpose)
    {
        MidiPatchMap[Index] = instrument;
        MidiPatchTrans[Index] = Transpose;
    }
    public void SetMidiDrumMap(int Index, int NewDrum)
    {
        MidiDrumMap[Index] = NewDrum;
    }
    public void AddEarPiercer(int InstrumentID)
    {
        EarPiercers[EarPiercerCnt] = InstrumentID;
        EarPiercerCnt++;
    }
    public void ClearMidiPatchMap() // by Kawa
    {
        for (int i = 0; i < 128; i++)
        {
            MidiPatchMap[i] = i;
            MidiDrumMap[i] = i;
            EarPiercers[i] = -1;
        }
        EarPiercerCnt = 0;
    }

    // //------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Returns information about a given note in play.
    /// </summary>
    public NoteInfo[] NoteInfo
    {
        get => NoteArray;
        set => NoteArray = value;
    }

    public SongOutputTypes outputtype { get => mvarOutputType; set => mvarOutputType = value; }

    public bool Playing
    {
        get => mvarPlaying;
        set => mvarPlaying = value;
    }

    public int CurrentSong
    {
        get => mvarCurrentSong;
        set => mvarCurrentSong = value;
    }

    public decimal TickCounter
    {
        get => mvarTickCounter;
        set => mvarTickCounter = value;
    }

    public int Transpose
    {
        get => mvarTranspose;
        set => mvarTranspose = value;
    }

    public int Tempo
    {
        get => mvarTempo;
        set => mvarTempo = value;
    }

    public int Layer
    {
        get => mvarLayer;
        set => mvarLayer = value;
    }

    public int SongNumber
    {
        get => mvarSongNumber;
        set => mvarSongNumber = value;
    }

    public int SongPointer
    {
        get => mvarSongPointer;
        set => mvarSongPointer = value;
    }

    public int SongListPointer
    {
        get => mvarSongListPointer;
        set => mvarSongListPointer = value;
    }

    public int InstrumentTablePointer
    {
        get => mvarInstrumentTablePointer;
        set => mvarInstrumentTablePointer = value;
    }

    public string Filename
    {
        get => mvarFileName;
        set => mvarFileName = value;
    }

    public bool NoteBelongsToChannel(byte NoteID, int channelid) => NoteArray[NoteID].ParentChannel == channelid;

    public void StopSong()
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        Console.WriteLine("StopSong()");
        CloseFile(1);
        CloseFile(2);
        if (EventProcessor != null) EventProcessor.Enabled = false;
        FSOUND_Close();
        MidiClose();

        if (Recording)
        {
            Console.WriteLine("StopSong(): Recording stops...");
            Recording = false;
            midifile = 42;
            // Dim tlen As Long  // I'm relocating these three lines. (Drag)
            // tlen = LOF(midifile) - 12
            // Debug.Print "StopSong(): Track length is " & tlen & ", total ticks " & Me.TotalTicks
            // //BufferEvent Chr$(&HFF) & Chr$(&H2F) & Chr(0), Me.TotalTicks
            FilePutObject(midifile, (byte)10);
            FilePutObject(midifile, (byte)0xFF);
            FilePutObject(midifile, (byte)0x2F);
            FilePutObject(midifile, (byte)0);

            // ...to here. (Drag)
            int tlen = (int)(LOF(midifile) - 22); // This line should now give a more accurate track length. (Drag)
            Console.WriteLine("StopSong(): Track length is " + tlen + ", total ticks " + TotalTicks);

            // Put #midifile, , CLng(0) // Why are these here? (Drag)
            // Put #midifile, , CLng(0)
            FilePutObject(midifile, FlipLong(tlen), 0x13); // FlipLong(&H12345678)
            FileClose(midifile);
        }

        SongFinish.Invoke();
    }

    /// <summary>
    /// Loads, decodes and plays a song.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="SongNumber"></param>
    /// <param name="SongListPointer"></param>
    /// <param name="WantToRecord"></param>
    /// <param name="RecordTo"></param>
    public void PlaySong(string Filename, int SongNumber, int SongListPointer, bool WantToRecord = false, string RecordTo = "midiout.mid")
    {
        // TODO: (NOT SUPPORTED): if (ErrorChecking == true) On Error GoTo hell

        mvarFileName = Filename;
        mvarSongListPointer = SongListPointer;
        mvarSongNumber = SongNumber;

        if (mvarPlaying) StopSong();

        SappyInstrumentHeader inshead = new();
        SappyDrumKitHeader drmhead = new();
        SappyDirectHeader dirhead = new();
        SappySampleHeader smphead = new();
        SappyMultiHeader mulhead = new();
        SappyNoiseHeader gbhead = new();

        SappyChannels.Clear();
        DrumKits.Clear();
        SamplePool.Clear();
        Instruments.Clear();
        Directs.Clear();
        NoteQueue.Clear();
        for (int i = 0; i < 32; i++)
        {
            NoteArray[i].Enabled = false;
        }

        OpenFile(1, mvarFileName);
        int a = ReadGBAROMPointer(1, mvarSongListPointer + SongNumber * 8);
        mvarSongPointer = a;
        mvarLayer = ReadLittleEndian(1, 4);
        byte b = ReadByte(1, a);
        mvarInstrumentTablePointer = ReadGBAROMPointer(1, a + 4);

        Loading.Invoke(0);
        SSubroutines xta = new();
        for (int i = 1; i <= b; i++)
        {
            int loopoffset = -1;
            SappyChannels.Add();
            int pc = ReadGBAROMPointer(1, a + 4 + i * 4);
            SappyChannels[i].TrackPointer = pc;
            xta.Clear();
            byte c;
            do
            {
                ReadOffset(1, pc);
                c = ReadByte(1);
                if (c >= 0 && c <= 0xB0 || c == 0xCE || c == 0xCF || c == 0xB4)
                {
                    pc++;
                }
                else if (c == 0xB9)
                {
                    pc += 4;
                }
                else if (c >= 0xB5 && c <= 0xCD)
                {
                    pc += 2;
                }
                else if (c == 0xB2)
                {
                    loopoffset = ReadGBAROMPointer(1);
                    pc += 5;
                    break;
                }
                else if (c == 0xB3)
                {
                    xta.Add(ReadGBAROMPointer(1));
                    pc += 5;
                }
                else if (c >= 0xD0 && c <= 0xFF)
                {
                    pc++;
                    while (ReadByte(1) < 0x80)
                    {
                        pc++;
                    }
                }
            } while (!(c == 0xB1));
            SappyChannels[i].TrackLengthInBytes = pc - SappyChannels[i].TrackPointer;
            pc = ReadGBAROMPointer(1, a + 4 + i * 4);


            if (i == 7)
            {
                i = 7;
                FileClose(7);
                FileOpen(7, "trackseven.txt", OpenMode.Output);
            }

            int cticks = 0;
            byte lc = 0xBE;
            byte[] lln = new byte[65];
            byte[] llv = new byte[65];
            byte[] lla = new byte[65];
            byte lp = 0;
            int insub = 0;
            int rpc = 0;
            int tR = 0;
            SappyChannels[i].LoopPointer = -1;
            do
            {
                ReadOffset(1, pc);
                if (pc >= loopoffset && SappyChannels[i].LoopPointer == -1 && loopoffset != -1)
                {
                    SappyChannels[i].LoopPointer = SappyChannels[i].EventQueue.count + 1;
                }
                c = ReadByte(1);

                if (i == 7) PrintLine(7, Strings.Right("000000" + Hex(pc), 6) + vbTab + Hex(c));
                if (pc == 0x11BE31)
                {
                    PrintLine(7, "Warning!");
                }

                if (c != 0xB9 && c >= 0xB5 && c < 0xC5 || c == 0xCD)
                {
                    byte D = ReadByte(1);
                    if (c == 0xBC) tR = SignedByteToInteger(D);
                    if (c == 0xBD) lp = D;
                    if (c == 0xBE || c == 0xBF || c == 0xC0 || c == 0xC4 || c == 0xCD) lc = c;
                    SappyChannels[i].EventQueue.Add(cticks, c, D, 0, 0);
                    pc += 2;
                }
                else if (c > 0xC4 && c < 0xCF)
                {
                    SappyChannels[i].EventQueue.Add(cticks, c, 0, 0, 0);
                    pc++;
                }
                else if (c == 0xB9)
                {
                    byte D = ReadByte(1);
                    byte e = ReadByte(1);
                    byte F = ReadByte(1);
                    SappyChannels[i].EventQueue.Add(cticks, c, D, e, F);
                    pc += 4;
                }
                else if (c == 0xB4)
                {
                    if (insub == 1)
                    {
                        pc = rpc;
                        insub = 0;
                    }
                    else
                    {
                        pc++;
                    }
                }
                else if (c == 0xB3)
                {
                    rpc = pc + 5;
                    insub = 1;
                    pc = ReadGBAROMPointer(1);
                }
                else if (c >= 0xCF && c <= 0xFF)
                {
                    pc++;
                    lc = c;
                    bool g = false;
                    int nc = 0;
                    int cdr = 0;
                    while (g == false)
                    {
                        byte D = ReadByte(1);
                        byte pn = 0;

                        if (i == 7) PrintLine(7, Strings.Right("000000" + Hex(pc), 6) + vbTab + "  " + Hex(D));

                        if (D >= 0x80)
                        {
                            if (nc == 0)
                            {
                                pn = (byte)(lln[nc] + tR);
                                SappyChannels[i].EventQueue.Add(cticks, c, pn, llv[nc], lla[nc]);
                            }
                            g = true;
                        }
                        else
                        {
                            // TODO: (NOT SUPPORTED): On Error GoTo hell
                            lln[nc] = D;
                            pc++;
                            byte e = ReadByte(1);
                            if (i == 7) PrintLine(7, Strings.Right("000000" + Hex(pc), 6) + vbTab + "    " + Hex(e));
                            byte F;
                            if (e < 0x80)
                            {
                                llv[nc] = e;
                                pc++;
                                F = ReadByte(1);
                                if (i == 7) PrintLine(7, Strings.Right("000000" + Hex(pc), 6) + vbTab + "      " + Hex(F));
                                if (F >= 0x80)
                                {
                                    F = lla[nc];
                                    g = true;
                                }
                                else
                                {
                                    lla[nc] = F;
                                    pc++;
                                    nc++;
                                }
                            }
                            else
                            {
                                e = llv[nc];
                                F = lla[nc];
                                g = true;
                            }
                            pn = (byte)(D + tR);
                            SappyChannels[i].EventQueue.Add(cticks, c, pn, e, F);
                        }

                        // TODO: (NOT SUPPORTED): On Error Resume Next

                        if (!PatchExists(lp))
                        {
                            inshead = ReadInstrumentHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12)));

                            if ((inshead.bChannel & 0x80) == 0x80)
                            {
                                drmhead = ReadDrumKitHead(1);
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))) + 2);
                                DrumKits.Add(Str(lp));
                                DrumKits[Str(lp)].Directs.Add(Str(pn));
                                SetStuff(DrumKits[Str(lp)].Directs[Str(pn)], inshead, dirhead, gbhead);
                                try
                                {
                                    if (Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotWave) GetSample(DrumKits[Str(lp)].Directs[Str(pn)], dirhead, ref smphead, false);
                                }
                                catch (Exception) { }
                            }
                            else if ((inshead.bChannel & 0x40) == 0x40) // multi
                            {
                                mulhead = ReadMultiHead(1);
                                Instruments.Add(Str(lp));
                                Instruments[Str(lp)].KeyMaps.Add(0, Str(pn));
                                Instruments[Str(lp)].KeyMaps[Str(pn)].AssignDirect = ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn);
                                cdr = Instruments[Str(lp)].KeyMaps[Str(pn)].AssignDirect;
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))) + 2);
                                Instruments[Str(lp)].Directs.Add(Str(cdr));
                                SetStuff(Instruments[Str(lp)].Directs[Str(cdr)], inshead, dirhead, gbhead);
                                if (Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotWave) GetSample(Instruments[Str(lp)].Directs[Str(cdr)], dirhead, ref smphead, false);

                            }
                            else // direct or gb sample
                            {
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12) + 2));
                                Directs.Add(Str(lp));
                                SetStuff(Directs[Str(lp)], inshead, dirhead, gbhead);
                                if (Directs[Str(lp)].outputtype == DirectOutputTypes.dotDirect || Directs[Str(lp)].outputtype == DirectOutputTypes.dotWave) GetSample(Directs[Str(lp)], dirhead, ref smphead, true);

                            }

                        }
                        else // patch already exists
                        {

                            inshead = ReadInstrumentHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12)));
                            if ((inshead.bChannel & 0x80) == 0x80)
                            {
                                drmhead = ReadDrumKitHead(1);
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))) + 2);
                                if (!DirectExists(DrumKits[Str(lp)].Directs, pn))
                                {
                                    DrumKits[Str(lp)].Directs.Add(Str(pn));
                                    SetStuff(DrumKits[Str(lp)].Directs[Str(pn)], inshead, dirhead, gbhead);
                                    if (DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotDirect || DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(DrumKits[Str(lp)].Directs[Str(pn)], dirhead, ref smphead, false);
                                }

                            }
                            else if ((inshead.bChannel & 0x40) == 0x40) // multi
                            {
                                mulhead = ReadMultiHead(1);
                                if (!KeyMapExists(Instruments[Str(lp)].KeyMaps, pn)) Instruments[Str(lp)].KeyMaps.Add(ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn));
                                cdr = Instruments[Str(lp)].KeyMaps[Str(pn)].AssignDirect;
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))) + 2);
                                if (!DirectExists(Instruments[Str(lp)].Directs, (byte)cdr))
                                {
                                    Instruments[Str(lp)].Directs.Add(Str(cdr));
                                    SetStuff(Instruments[Str(lp)].Directs[Str(cdr)], inshead, dirhead, gbhead);
                                    if (Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(Instruments[Str(lp)].Directs[Str(cdr)], dirhead, ref smphead, false);
                                }
                            }
                        }

                        // TODO: (NOT SUPPORTED): On Error GoTo 0

                    }

                }
                else if (c >= 0x0 && c < 0x80)
                {

                    // TODO: (NOT SUPPORTED): On Error Resume Next

                    if (lc < 0xCF)
                    {
                        SappyChannels[i].EventQueue.Add(cticks, lc, c, 0, 0);
                        pc++;
                    }
                    else
                    {
                        c = lc;
                        ReadOffset(1, pc);
                        bool g = false;
                        int nc = 0;
                        int cdr = 0;
                        while (g == false)
                        {
                            byte D = ReadByte(1);
                            byte pn = 0;
                            if (D >= 0x80)
                            {
                                if (nc == 0)
                                {
                                    pn = (byte)(lln[nc] + tR);
                                    SappyChannels[i].EventQueue.Add(cticks, c, pn, llv[nc], lla[nc]);
                                }
                                g = true;
                            }
                            else
                            {
                                lln[nc] = D;
                                pc++;
                                byte e = ReadByte(1);
                                byte F;
                                if (e < 0x80)
                                {
                                    llv[nc] = e;
                                    pc++;
                                    F = ReadByte(1);
                                    if (F >= 0x80)
                                    {
                                        F = lla[nc];
                                        g = true;
                                    }
                                    else
                                    {
                                        lla[nc] = F;
                                        pc++;
                                        nc++;
                                    }
                                }
                                else
                                {
                                    e = llv[nc];
                                    F = lla[nc];
                                    g = true;
                                }
                                pn = (byte)(D + tR);
                                SappyChannels[i].EventQueue.Add(cticks, c, pn, e, F);
                            }

                            // TODO: (NOT SUPPORTED): On Error Resume Next

                            if (!PatchExists(lp))
                            {
                                inshead = ReadInstrumentHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12)));
                                if ((inshead.bChannel & 0x80) == 0x80)
                                {
                                    drmhead = ReadDrumKitHead(1);
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))) + 2);
                                    DrumKits.Add(Str(lp));
                                    DrumKits[Str(lp)].Directs.Add(Str(pn));
                                    SetStuff(DrumKits[Str(lp)].Directs[Str(pn)], inshead, dirhead, gbhead);
                                    if (DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotDirect || DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotWave) GetSample(DrumKits[Str(lp)].Directs[Str(pn)], dirhead, ref smphead, true);

                                }
                                else if ((inshead.bChannel & 0x40) == 0x40) // multi
                                {
                                    mulhead = ReadMultiHead(1);
                                    Instruments.Add(Str(lp));
                                    Instruments[Str(lp)].KeyMaps.Add(ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn));
                                    cdr = Instruments[Str(lp)].KeyMaps[Str(pn)].AssignDirect;
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))) + 2);
                                    Instruments[Str(lp)].Directs.Add(Str(cdr));
                                    SetStuff(Instruments[Str(lp)].Directs[Str(cdr)], inshead, dirhead, gbhead);
                                    if (Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(Instruments[Str(lp)].Directs[Str(cdr)], dirhead, ref smphead, false);

                                }
                                else // direct or gb sample
                                {
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12) + 2));
                                    Directs.Add(Str(lp));
                                    SetStuff(Directs[Str(lp)], inshead, dirhead, gbhead);
                                    if (Directs[Str(lp)].outputtype == DirectOutputTypes.dotDirect || Directs[Str(lp)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(Directs[Str(lp)], dirhead, ref smphead, false);
                                }

                            }
                            else // patch already exists
                            {
                                inshead = ReadInstrumentHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12)));
                                if ((inshead.bChannel & 0x80) == 0x80)
                                {
                                    drmhead = ReadDrumKitHead(1);
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(drmhead.pDirectTable + CLng(pn) * CLng(12))) + 2);
                                    if (!DirectExists(DrumKits[Str(lp)].Directs, pn))
                                    {
                                        DrumKits[Str(lp)].Directs.Add(Str(pn));
                                        SetStuff(DrumKits[Str(lp)].Directs[Str(pn)], inshead, dirhead, gbhead);
                                        if (DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotDirect || DrumKits[Str(lp)].Directs[Str(pn)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(DrumKits[Str(lp)].Directs[Str(pn)], dirhead, ref smphead, false);
                                    }

                                }
                                else if ((inshead.bChannel & 0x40) == 0x40) // multi
                                {
                                    mulhead = ReadMultiHead(1);
                                    if (!KeyMapExists(Instruments[Str(lp)].KeyMaps, pn)) Instruments[Str(lp)].KeyMaps.Add(ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn));
                                    cdr = Instruments[Str(lp)].KeyMaps[Str(pn)].AssignDirect;
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset((int)(mulhead.pDirectTable + CLng(cdr) * CLng(12))) + 2);
                                    if (!DirectExists(Instruments[Str(lp)].Directs, (byte)cdr))
                                    {
                                        Instruments[Str(lp)].Directs.Add(Str(cdr));
                                        SetStuff(Instruments[Str(lp)].Directs[Str(cdr)], inshead, dirhead, gbhead);
                                        if (Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(lp)].Directs[Str(cdr)].outputtype == DirectOutputTypes.dotWave) GetSampleWithMulti(Instruments[Str(lp)].Directs[Str(cdr)], dirhead, ref smphead, false);
                                    }
                                }
                            }

                            // TODO: (NOT SUPPORTED): On Error GoTo 0

                        }
                    }
                }
                else if (c >= 0x80 && c <= 0xB0)
                {
                    SappyChannels[i].EventQueue.Add(cticks, c, 0, 0, 0);
                    cticks += SLen2Ticks((byte)(c - 0x80));
                    pc++;
                }
            } while (!(c == 0xB1 || c == 0xB2));

            SappyChannels[i].EventQueue.Add(cticks, c, 0, 0, 0);
        }

        FSOUND_Init(44100, 64, 0);
        FSOUND_SetSFXMasterVolume(mvarGlobalVolume);
        MidiOpen();
        if (mvarOutputType == SongOutputTypes.sotMIDI) goto SkipThatWholeInstrumentGarbish;

        Trace("===================");
        Trace("Filling sample pool");
        Trace("===================");
        int quark = 0;
        foreach (var Item in SamplePool)
        {
            Loading.Invoke(1);
            // OpenFile 2, Item.Key & ".raw"
            // If Item.GBWave = True Then
            //   WriteString 2, Item.SampleData
            // Else
            //   For i = 0 To Item.Size
            //     WriteByte 2, ReadByte(1, Item.SampleData + i)
            //   Next i
            // End If
            // CloseFile 2
            // Item.SampleData = ""
            // If Item.GBWave = True Then
            //   Item.FModSample = FSOUND_Sample_Load(FSOUND_FREE, Item.Key & ".raw", FSOUND_8BITS + FSOUND_LOADRAW + FSOUND_LOOP_NORMAL + FSOUND_MONO + FSOUND_UNSIGNED, 0, 0)
            //   Call FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31)
            // Else
            //   Item.FModSample = FSOUND_Sample_Load(FSOUND_FREE, Item.Key & ".raw", FSOUND_8BITS + FSOUND_LOADRAW + IIf(Item.LoopEnable = True, FSOUND_LOOP_NORMAL, 0) + FSOUND_MONO + FSOUND_SIGNED, 0, 0)
            //   Call FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1)
            // End If
            // DeleteFile Item.Key & ".raw"
            quark++;
            // On Error Resume Next
            Trace("#" + quark + " - " + Item.GBWave + " - " + Item.SampleData);
            if (Item.GBWave)
            {
                if (Val(Item.SampleData) == 0)
                {
                    OpenFile(2, "temp.raw");
                    WriteString(2, Item.SampleData);
                    CloseFile(2);
                    Item.FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "temp.raw", FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | FSOUND_MODES.FSOUND_LOOP_NORMAL | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
                    FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31);
                    DeleteFile("temp.raw");
                }
                else
                {
                    Item.FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, Filename, FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | FSOUND_MODES.FSOUND_LOOP_NORMAL | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_UNSIGNED, int.Parse(Item.SampleData), Item.Size);
                    FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31);
                }
            }
            else
            {
                if (Val(Item.SampleData) == 0)
                {
                    // TODO: (NOT SUPPORTED): On Error Resume Next
                    OpenFile(2, "temp.raw");
                    // For i = 0 To Item.Size
                    //   WriteByte 2, ReadByte(1, Item.SampleData + i)
                    // Next i
                    WriteString(2, Item.SampleData);
                    CloseFile(2);
                    Item.FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "temp.raw", FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | (Item.LoopEnable ? FSOUND_MODES.FSOUND_LOOP_NORMAL : 0) | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_SIGNED, 0, 0);
                    FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1);
                    DeleteFile("temp.raw");
                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                }
                else
                {
                    Item.FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, Filename, FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | (Item.LoopEnable ? FSOUND_MODES.FSOUND_LOOP_NORMAL : 0) | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_SIGNED, int.Parse(Item.SampleData), Item.Size);
                    FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1);
                }
            }
            // TODO: (NOT SUPPORTED): On Error GoTo 0
        }

        for (int i = 0; i <= 9; i++)
        {
            SamplePool.Add("noise0" + i);
            Randomize(DateAndTime.Timer);
            SamplePool["noise0" + i].SampleData = NoiseWaves[0, i];
            SamplePool["noise0" + i].Frequency = 7040;
            SamplePool["noise0" + i].Size = 16384;
            OpenFile(2, "noise0" + i + ".raw");
            WriteString(2, SamplePool["noise0" + i].SampleData);
            CloseFile(2);
            SamplePool["noise0" + i].SampleData = "";
            SamplePool["noise0" + i].FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "noise0" + i + ".raw", FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | FSOUND_MODES.FSOUND_LOOP_NORMAL | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            FSOUND_Sample_SetLoopPoints(SamplePool["noise0" + i].FModSample, 0, 16383);
            DeleteFile("noise0" + i + ".raw");
            SamplePool.Add("noise1" + i);

            Randomize(DateAndTime.Timer);
            SamplePool["noise1" + i].SampleData = NoiseWaves[1, i];
            SamplePool["noise1" + i].Frequency = 7040;
            SamplePool["noise1" + i].Size = 256;
            OpenFile(2, "noise1" + i + ".raw");
            WriteString(2, SamplePool["noise1" + i].SampleData);
            CloseFile(2);
            SamplePool["noise1" + i].SampleData = "";
            SamplePool["noise1" + i].FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "noise1" + i + ".raw", FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | FSOUND_MODES.FSOUND_LOOP_NORMAL | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            FSOUND_Sample_SetLoopPoints(SamplePool["noise1" + i].FModSample, 0, 255);
            DeleteFile("noise1" + i + ".raw");
        }

        for (int mx2 = 0; mx2 <= 3; mx2 += 1)
        {
            SamplePool.Add("square" + mx2);
            switch (mx2)
            {
                case 0:
                    SamplePool["square" + mx2].SampleData = new string(Chr((int)Int(0x80 + 0x7F * GBSquareMulti)), 4) + new string(Chr((int)Int(0x80 - 0x7F * GBSquareMulti)), 28);
                    break;
                case 1:
                    SamplePool["square" + mx2].SampleData = new string(Chr((int)Int(0x80 + 0x7F * GBSquareMulti)), 8) + new string(Chr((int)Int(0x80 - 0x7F * GBSquareMulti)), 24);
                    break;
                case 2:
                    SamplePool["square" + mx2].SampleData = new string(Chr((int)Int(0x80 + 0x7F * GBSquareMulti)), 16) + new string(Chr((int)Int(0x80 - 0x7F * GBSquareMulti)), 16);
                    break;
                case 3:
                    SamplePool["square" + mx2].SampleData = new string(Chr((int)Int(0x80 + 0x7F * GBSquareMulti)), 24) + new string(Chr((int)Int(0x80 - 0x7F * GBSquareMulti)), 8);
                    break;
            }
            SamplePool["square" + mx2].Frequency = 7040;
            SamplePool["square" + mx2].Size = 32;
            OpenFile(2, "square" + mx2 + ".raw");
            WriteString(2, SamplePool["square" + mx2].SampleData);
            CloseFile(2);
            SamplePool["square" + mx2].SampleData = "";
            SamplePool["square" + mx2].FModSample = FSOUND_Sample_Load((int)FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "square" + mx2 + ".raw", FSOUND_MODES.FSOUND_8BITS | FSOUND_MODES.FSOUND_LOADRAW | FSOUND_MODES.FSOUND_LOOP_NORMAL | FSOUND_MODES.FSOUND_MONO | FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            FSOUND_Sample_SetLoopPoints(SamplePool["square" + mx2].FModSample, 0, 31);
            DeleteFile("square" + mx2 + ".raw");
        }
        mvarGB1Chan = 255;
        mvarGB2Chan = 255;
        mvarGB3Chan = 255;
        mvarGB4Chan = 255;

    SkipThatWholeInstrumentGarbish:;
        Loading.Invoke(2);

        EventProcessor = new CTimerMM();
        EventProcessor.Timer += EventProcessor_Timer;
        mvarTempo = 120;
        lasttempo = -1;
        incr = 0;
        EventProcessor_Timer(0);

        CloseFile(1);

        TotalTicks = 0;
        TotalMSecs = 0;
        Beats = 0;
        if (WantToRecord)
        {
            Recording = true;
            midifile = 42;
            if (Dir(RecordTo) != "") Kill(RecordTo);
            FileOpen(midifile, RecordTo, OpenMode.Binary);
            string H = "MThd";
            FilePutObject(midifile, H);
            FilePutObject(midifile, FlipLong(6));
            FilePutObject(midifile, FlipInt(0));
            FilePutObject(midifile, FlipInt(1)); // FlipInt(SappyChannels.count)
            FilePutObject(midifile, FlipInt(24)); // 48
            H = "MTrk";
            FilePutObject(midifile, H);
            FilePutObject(midifile, CLng(0));
            string msg = frmSappy.instance.lblSongName.Text;
            System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            msg = msg + " --- dumped by " + assemblyName.Name + " version " + assemblyName.Version.Major + "." + assemblyName.Version.Minor;

            if (Strings.Len(msg) > 120) msg = Strings.Left(msg, 120);
            BufferEvent(Chr(0xFF) + Chr(2) + Chr(Strings.Len(msg)) + msg, 0);
        }

        return;
    hell:;
        if (Err().Number == 6)
        {
            MsgBox("Invalid song number. Probably went out of the table.");
            StopSong();
            return;
        }
        MsgBox("runtime error: " + Err().Number + " / " + Err().Description, vbCritical);
        StopSong();
    }

    // Added by Kawa in DLL conversion
    public clsSappyDecoder()
    {
        for (int i = 0; i < NoteArray.Length; i++)
        {
            NoteArray[i] = new();
        }

        Trace(DateTime.Now + vbTab + "- Yo, this be clsSappyDecoder, Class_Initialize()");
        Randomize(DateAndTime.Timer);
        int sz = GetSettingI("Noise Length");
        if (sz == 0) sz = 2047;
        int ts = GetTickCount();
        for (int i = 0; i <= 9; i++)
        {
            Trace(DateTime.Now + vbTab + "- Creating NoiseWaves(0," + i + ")");
            for (int j = 0; j <= sz; j += 1) // 2047 // 16383
            {
                NoiseWaves[0, i] = NoiseWaves[0, i] + Chr((int)Int(Rnd() * 153)); // (255 * 0.6)))
            }
            Trace(DateTime.Now + vbTab + "- Creating NoiseWaves(1," + i + ")");
            for (int j = 0; j <= 255; j += 1)
            {
                NoiseWaves[1, i] = NoiseWaves[1, i] + Chr((int)Int(Rnd() * 153)); // (255 * 0.6)))
            }
        }
        int te = GetTickCount();
        Trace(DateTime.Now + vbTab + "- Took " + (te - ts) + ".");
        mvarGlobalVolume = 255;
        Trace(DateTime.Now + vbTab + "- Done. Back to the studio...");
    }

    private void EventProcessor_Timer(int lMilliseconds)
    {
        // TODO: (NOT SUPPORTED): if (ErrorChecking == true) On Error GoTo hell

        int ep = 0;
        bool mutethis = false;

        TotalMSecs += lMilliseconds;
        if (mvarTickCounter > 0)
        {
            for (int i = 0; i <= 31; i++)
            {
                if (NoteArray[i].Enabled && NoteArray[i].WaitTicks > 0)
                {
                    NoteArray[i].WaitTicks = NoteArray[i].WaitTicks - (mvarTickCounter - mvarLastTick);
                }
                if (NoteArray[i].WaitTicks <= 0 && NoteArray[i].Enabled && !NoteArray[i].NoteOff)
                {
                    if (!SappyChannels[NoteArray[i].ParentChannel].Sustain)
                    {
                        NoteArray[i].NoteOff = true;
                    }
                }
            }
            for (int i = 1; i <= SappyChannels.count; i++)
            {
                while (!SappyChannels[i].Enabled)
                {
                    i++;
                    if (i > SappyChannels.count) break;
                }

                // mutethis = False
                for (ep = 0; ep <= EarPiercerCnt; ep += 1)
                {
                    if (EarPiercers[ep] == SappyChannels[i].PatchNumber)
                    {
                        SappyChannels[i].mute = true;
                        // SappyChannels[i].Enabled = False
                        break;
                    }
                }

                if (SappyChannels[i].WaitTicks > 0) SappyChannels[i].WaitTicks = SappyChannels[i].WaitTicks - (mvarTickCounter - mvarLastTick);
                bool justlooped = false;
                while (SappyChannels[i].WaitTicks <= 0)
                {
                    // DoEvents
                    switch (SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].CommandByte)
                    {

                        case 0xB1:
                            SappyChannels[i].Enabled = false;
                            goto ExitFor;

                        case 0xB9:
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xBB:
                            mvarTempo = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1 * 2;
                            ChangedTempo.Invoke(mvarTempo);
                            if (Recording) BufferEvent($"{Chr(0xFF)}{Chr(0x51)}", TotalTicks);
                            if (Recording) FilePutObject(midifile, FlipLong(((60000000 / mvarTempo) & 0xFFFFFF) | 0x3000000));
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xBC:
                            SappyChannels[i].Transpose = SignedByteToInteger(SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1);
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xBD:
                            SappyChannels[i].PatchNumber = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            if (DirectExists(Directs, SappyChannels[i].PatchNumber))
                            {
                                SappyChannels[i].outputtype = (ChannelOutputTypes)Directs[Str(SappyChannels[i].PatchNumber)].outputtype;
                            }
                            else if (InstrumentExists(SappyChannels[i].PatchNumber))
                            {
                                SappyChannels[i].outputtype = ChannelOutputTypes.cotMultiSample;
                            }
                            else if (DrumKitExists(SappyChannels[i].PatchNumber))
                            {
                                SappyChannels[i].outputtype = ChannelOutputTypes.cotDrumKit;
                            }
                            else
                            {
                                SappyChannels[i].outputtype = ChannelOutputTypes.cotNull;
                            }
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            SelectInstrument(i, MidiPatchMap[SappyChannels[i].PatchNumber]);
                            BufferEvent($"{Chr(0xC0 + i)}{Chr(MidiPatchMap[SappyChannels[i].PatchNumber])}", TotalTicks);
                            break;

                        case 0xBE:
                            // Do Set Volume
                            SappyChannels[i].MainVolume = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            foreach (var Item in SappyChannels[i].Notes)
                            {
                                if (NoteArray[Item.NoteID].Enabled && NoteArray[Item.NoteID].ParentChannel == i)
                                {
                                    int dav = CInt(CInt(NoteArray[Item.NoteID].Velocity) / CInt(0x7F) * (CInt(SappyChannels[i].MainVolume) / CInt(0x7F)) * (CInt(Int(NoteArray[Item.NoteID].EnvPosition)) / CInt(0xFF)) * 255);
                                    if (mutethis) dav = 0;
                                    if (mvarOutputType == SongOutputTypes.sotWave)
                                    {
                                        FSOUND_SetVolume(NoteArray[Item.NoteID].FModChannel, dav * (SappyChannels[i].mute ? 0 : 1));
                                    }
                                    else
                                    {
                                        // MIDISETVOL
                                        SetChnVolume(NoteArray[Item.NoteID].FModChannel, dav * (SappyChannels[i].mute ? 0 : 2));
                                    }
                                    // TODO: (NOT SUPPORTED): On Error Resume Next
                                    if (Recording) BufferEvent($"{Chr(0xD0 + NoteArray[Item.NoteID].FModChannel)}{Chr(dav)}", TotalTicks);
                                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                                }
                            }
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xBF:
                            // Do Set Panning
                            SappyChannels[i].Panning = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            foreach (var Item in SappyChannels[i].Notes)
                            {
                                if (NoteArray[Item.NoteID].Enabled && NoteArray[Item.NoteID].ParentChannel == i)
                                {
                                    if (mvarOutputType == SongOutputTypes.sotWave)
                                    {
                                        FSOUND_SetPan(NoteArray[Item.NoteID].FModChannel, SappyChannels[i].Panning * 2);
                                    }
                                    else
                                    {
                                        // MIDISETPAN
                                        SetChnPan(NoteArray[Item.NoteID].FModChannel, SappyChannels[i].Panning * 2);
                                    }
                                    // TODO: (NOT SUPPORTED): On Error Resume Next
                                    if (Recording) BufferEvent($"{Chr(0xB0 + NoteArray[Item.NoteID].FModChannel)}{Chr(0xA)}{Chr(SappyChannels[i].Panning * 2)}", TotalTicks);
                                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                                }
                            }
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xC0:
                            // Do Set Pitch Bend
                            SappyChannels[i].PitchBend = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            foreach (var Item in SappyChannels[i].Notes)
                            {
                                if (NoteArray[Item.NoteID].Enabled && NoteArray[Item.NoteID].ParentChannel == i)
                                {
                                    if (mvarOutputType == SongOutputTypes.sotWave)
                                    {
                                        FSOUND_SetFrequency(NoteArray[Item.NoteID].FModChannel, NoteArray[Item.NoteID].Frequency * (int)Math.Pow(Math.Pow(2, 1d / 12), CInt(SappyChannels[i].PitchBend - 0x40) / CInt(0x40) * CInt(SappyChannels[i].PitchBendRange)));
                                    }
                                    else
                                    {
                                        // MIDIPITCHBEND
                                        // PitchWheel(NoteArray[Item.NoteID].FModChannel, NoteArray[Item.NoteID].Frequency * (int)Math.Pow(Math.Pow(2, 1d / 12), CInt(SappyChannels[i].PitchBend - 0x40) / CInt(0x40) * CInt(SappyChannels[i].PitchBendRange)));
                                    }
                                }
                            }
                            break;

                        case 0xC1:
                            // Do Set Pitch Bend Range
                            SappyChannels[i].PitchBendRange = SignedByteToInteger(SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1);
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            foreach (var Item in SappyChannels[i].Notes)
                            {
                                if (NoteArray[Item.NoteID].Enabled && NoteArray[Item.NoteID].ParentChannel == i)
                                {
                                    if (mvarOutputType == SongOutputTypes.sotWave)
                                    {
                                        FSOUND_SetFrequency(NoteArray[Item.NoteID].FModChannel, NoteArray[Item.NoteID].Frequency * (int)Math.Pow(Math.Pow(2, 1d / 12), CInt(SappyChannels[i].PitchBend - 0x40) / CInt(0x40) * CInt(SappyChannels[i].PitchBendRange)));
                                    }
                                    else
                                    {
                                        // MIDIPITCHBENDRANGE
                                        // PitchWheel(NoteArray[Item.NoteID].FModChannel, NoteArray[Item.NoteID].Frequency * (int)Math.Pow(Math.Pow(2, 1d / 12), CInt(SappyChannels[i].PitchBend - 0x40) / CInt(0x40) * CInt(SappyChannels[i].PitchBendRange)));
                                    }
                                }
                            }
                            break;

                        case 0xC2:
                            // Do Set Vibrato Depth
                            SappyChannels[i].VibratoDepth = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xC4:
                            // Do Set Vibrato Rate
                            SappyChannels[i].VibratoRate = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xCE:
                            // Do Set Sustain Off
                            SappyChannels[i].Sustain = false;
                            foreach (var Item in SappyChannels[i].Notes)
                            {
                                if (NoteArray[Item.NoteID].Enabled && !NoteArray[Item.NoteID].NoteOff) // And NoteArray[Item.NoteID].WaitTicks < 1 Then
                                {
                                    NoteArray[Item.NoteID].NoteOff = true;
                                }
                            }
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case 0xB3:
                            SappyChannels[i].SubroutineCounter = SappyChannels[i].SubroutineCounter + 1;
                            SappyChannels[i].ReturnPointer = SappyChannels[i].ProgramCounter + 1;
                            SappyChannels[i].ProgramCounter = SappyChannels[i].Subroutines[SappyChannels[i].SubroutineCounter].EventQueuePointer;
                            SappyChannels[i].InSubroutine = true;
                            break;

                        case 0xB4:
                            if (SappyChannels[i].InSubroutine)
                            {
                                SappyChannels[i].ProgramCounter = SappyChannels[i].ReturnPointer;
                                SappyChannels[i].InSubroutine = false;
                            }
                            else
                            {
                                SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            }
                            break;

                        case 0xB2:
                            justlooped = true;
                            SappyChannels[i].InSubroutine = false;
                            SappyChannels[i].ProgramCounter = SappyChannels[i].LoopPointer;
                            break;

                        case >= 0xCF:
                            int ll = SLen2Ticks((byte)(SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].CommandByte - 0xCF)) + 1;
                            if (SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].CommandByte == 0xCF)
                            {
                                SappyChannels[i].Sustain = true;
                                ll = 0;
                            }
                            byte nn = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param1;
                            byte vv = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param2;
                            byte uu = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Param3;
                            NoteQueue.Add(true, 0, nn, 0, vv, i, uu, 0, 0, 0, 0, 0, ll, SappyChannels[i].PatchNumber);
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;

                        case <= 0xB0:
                            if (justlooped)
                            {
                                justlooped = false;
                                if (i == 1) SongLoop.Invoke();
                                SappyChannels[i].WaitTicks = 0;
                            }
                            else
                            {
                                SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                                if (SappyChannels[i].ProgramCounter > 1)
                                {
                                    SappyChannels[i].WaitTicks = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Ticks - SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter - 1].Ticks;
                                }
                                else
                                {
                                    SappyChannels[i].WaitTicks = SappyChannels[i].EventQueue[SappyChannels[i].ProgramCounter].Ticks;
                                }
                            }
                            break;

                        default:
                            SappyChannels[i].ProgramCounter = SappyChannels[i].ProgramCounter + 1;
                            break;
                    }
                }

            ExitFor:;
            }

            if (SappyChannels.count > 0)
            {
                bool[] clearedchannel = new bool[SappyChannels.count];

                UpdateDisplay.Invoke();
                foreach (var Item in NoteQueue)
                {
                    byte x = FreeNote();
                    if (x < 32)
                    {
                        NoteArray[x] = Item;
                        if (!clearedchannel[Item.ParentChannel - 1])
                        {
                            clearedchannel[Item.ParentChannel - 1] = true;
                            foreach (var item2 in SappyChannels[Item.ParentChannel].Notes)
                            {
                                if (NoteArray[item2.NoteID].Enabled && !NoteArray[item2.NoteID].NoteOff)
                                {
                                    NoteArray[item2.NoteID].NoteOff = true;
                                }
                            }
                        }

                        SappyChannels[Item.ParentChannel].Notes.Add(x, Str(x));
                        byte pat = Item.PatchNumber;
                        byte nn = Item.NoteNumber;
                        string das;
                        int daf = 0;
                        if (DirectExists(Directs, pat))
                        {
                            NoteArray[x].outputtype = (NoteOutputTypes)Directs[Str(pat)].outputtype;
                            NoteArray[x].EnvAttenuation = Directs[Str(pat)].EnvAttenuation;
                            NoteArray[x].EnvDecay = Directs[Str(pat)].EnvDecay;
                            NoteArray[x].EnvSustain = Directs[Str(pat)].EnvSustain;
                            NoteArray[x].EnvRelease = Directs[Str(pat)].EnvRelease;
                            if (Directs[Str(pat)].outputtype == DirectOutputTypes.dotDirect || Directs[Str(pat)].outputtype == DirectOutputTypes.dotWave)
                            {
                                das = Str(Directs[Str(pat)].SampleID);
                                daf = NoteToFreq(nn + (60 - Directs[Str(pat)].DrumTuneKey), SamplePool[das].GBWave ? -1 : SamplePool[das].Frequency);
                                if (SamplePool[das].GBWave) daf /= 2;
                            }
                            else if (Directs[Str(pat)].outputtype == DirectOutputTypes.dotSquare1 || Directs[Str(pat)].outputtype == DirectOutputTypes.dotSquare2)
                            {
                                das = "square" + Directs[Str(pat)].GB1 % 4;
                                daf = NoteToFreq(nn + (60 - Directs[Str(pat)].DrumTuneKey));
                            }
                            else if (Directs[Str(pat)].outputtype == DirectOutputTypes.dotNoise)
                            {
                                das = "noise" + Directs[Str(pat)].GB1 % 2 + Int(Rnd() * 3);
                                daf = NoteToFreq(nn + (60 - Directs[Str(pat)].DrumTuneKey));
                            }
                            else
                            {
                                das = "";
                            }
                        }
                        else if (InstrumentExists(pat))
                        {
                            NoteArray[x].outputtype = (NoteOutputTypes)Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].outputtype;
                            NoteArray[x].EnvAttenuation = Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].EnvAttenuation;
                            NoteArray[x].EnvDecay = Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].EnvDecay;
                            NoteArray[x].EnvSustain = Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].EnvSustain;
                            NoteArray[x].EnvRelease = Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].EnvRelease;
                            if (Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].outputtype == DirectOutputTypes.dotDirect || Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].outputtype == DirectOutputTypes.dotWave)
                            {
                                das = Str(Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].SampleID);
                                if (Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].FixedPitch)
                                {
                                    daf = SamplePool[das].Frequency;
                                }
                                else
                                {
                                    daf = NoteToFreq(nn, SamplePool[das].GBWave ? -2 : SamplePool[das].Frequency);
                                }
                            }
                            else if (Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].outputtype == DirectOutputTypes.dotSquare1 || Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].outputtype == DirectOutputTypes.dotSquare2)
                            {
                                das = "square" + Instruments[Str(pat)].Directs[Str(Instruments[Str(pat)].KeyMaps[Str(nn)].AssignDirect)].GB1 % 4;
                                daf = NoteToFreq(nn);
                            }
                            else
                            {
                                das = "";
                            }
                        }
                        else if (DrumKitExists(pat))
                        {
                            NoteArray[x].outputtype = (NoteOutputTypes)DrumKits[Str(pat)].Directs[Str(nn)].outputtype;
                            NoteArray[x].EnvAttenuation = DrumKits[Str(pat)].Directs[Str(nn)].EnvAttenuation;
                            NoteArray[x].EnvDecay = DrumKits[Str(pat)].Directs[Str(nn)].EnvDecay;
                            NoteArray[x].EnvSustain = DrumKits[Str(pat)].Directs[Str(nn)].EnvSustain;
                            NoteArray[x].EnvRelease = DrumKits[Str(pat)].Directs[Str(nn)].EnvRelease;
                            if (DrumKits[Str(pat)].Directs[Str(nn)].outputtype == DirectOutputTypes.dotDirect || DrumKits[Str(pat)].Directs[Str(nn)].outputtype == DirectOutputTypes.dotWave)
                            {
                                try
                                {
                                    das = Str(DrumKits[Str(pat)].Directs[Str(nn)].SampleID);
                                    if (DrumKits[Str(pat)].Directs[Str(nn)].FixedPitch && !SamplePool[das].GBWave)
                                    {
                                        daf = SamplePool[das].Frequency;
                                    }
                                    else
                                    {
                                        daf = NoteToFreq(DrumKits[Str(pat)].Directs[Str(nn)].DrumTuneKey, SamplePool[das].GBWave ? -2 : SamplePool[das].Frequency);
                                    }
                                }
                                catch (Exception) { das = ""; }
                            }
                            else if (DrumKits[Str(pat)].Directs[Str(nn)].outputtype == DirectOutputTypes.dotSquare1 || DrumKits[Str(pat)].Directs[Str(nn)].outputtype == DirectOutputTypes.dotSquare2)
                            {
                                das = "square" + DrumKits[Str(pat)].Directs[Str(nn)].GB1 % 4;
                                daf = NoteToFreq(DrumKits[Str(pat)].Directs[Str(nn)].DrumTuneKey);
                            }
                            else if (DrumKits[Str(pat)].Directs[Str(nn)].outputtype == DirectOutputTypes.dotNoise)
                            {
                                das = "noise" + DrumKits[Str(pat)].Directs[Str(nn)].GB1 % 2 + Int(Rnd() * 3);
                                daf = NoteToFreq(DrumKits[Str(pat)].Directs[Str(nn)].DrumTuneKey);
                            }
                            else
                            {
                                das = "";
                            }
                        }
                        else
                        {
                            das = "";
                        }

                        if (das != "")
                        {
                            daf *= (int)Math.Pow(Math.Pow(2, 1d / 12), Transpose);
                            int dav = CInt(CInt(Item.Velocity) / CInt(0x7F) * (CInt(SappyChannels[Item.ParentChannel].MainVolume) / CInt(0x7F)) * 255);
                            if (mutethis) dav = 0;

                            switch (NoteArray[x].outputtype)
                            {
                                case NoteOutputTypes.notSquare1:
                                    if (mvarGB1Chan < 32)
                                    {
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[mvarGB1Chan].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(NoteArray[mvarGB1Chan].ParentChannel, NoteArray[mvarGB1Chan].NoteNumber + MidiPatchTrans[NoteArray[mvarGB1Chan].PatchNumber]);
                                            // PitchWheel(NoteArray[Item.NoteID].FModChannel, 0);
                                        }
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[mvarGB1Chan].ParentChannel)}{Chr(NoteArray[mvarGB1Chan].NoteNumber)}{Chr(0)}", TotalTicks);
                                        NoteArray[mvarGB1Chan].FModChannel = 0;
                                        try { SappyChannels[NoteArray[mvarGB1Chan].ParentChannel].Notes.Remove(Str(mvarGB1Chan)); } catch (Exception) { }
                                        NoteArray[mvarGB1Chan].Enabled = false;
                                    }
                                    mvarGB1Chan = x;
                                    break;

                                case NoteOutputTypes.notSquare2:
                                    if (mvarGB2Chan < 32)
                                    {
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[mvarGB2Chan].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(NoteArray[mvarGB2Chan].ParentChannel, NoteArray[mvarGB2Chan].NoteNumber + MidiPatchTrans[NoteArray[mvarGB2Chan].PatchNumber]);
                                        }
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[mvarGB2Chan].ParentChannel)}{Chr(NoteArray[mvarGB2Chan].NoteNumber)}{Chr(0)}", TotalTicks);
                                        NoteArray[mvarGB2Chan].FModChannel = 0;
                                        try { SappyChannels[NoteArray[mvarGB2Chan].ParentChannel].Notes.Remove(Str(mvarGB2Chan)); } catch (Exception) { }
                                        NoteArray[mvarGB2Chan].Enabled = false;
                                    }
                                    mvarGB2Chan = x;
                                    break;

                                case NoteOutputTypes.notWave:
                                    if (mvarGB3Chan < 32)
                                    {
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[mvarGB3Chan].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(Item.ParentChannel, NoteArray[mvarGB3Chan].NoteNumber + MidiPatchTrans[NoteArray[mvarGB3Chan].PatchNumber]); // I'm removing the Item. parts here... (Drag)
                                        }
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[mvarGB3Chan].ParentChannel)}{Chr(NoteArray[mvarGB3Chan].NoteNumber)}{Chr(0)}", TotalTicks); // ...and here (Drag)
                                        NoteArray[mvarGB3Chan].FModChannel = 0;
                                        try { SappyChannels[NoteArray[mvarGB3Chan].ParentChannel].Notes.Remove(Str(mvarGB3Chan)); } catch (Exception) { }
                                        NoteArray[mvarGB3Chan].Enabled = false;
                                    }
                                    mvarGB3Chan = x;
                                    break;

                                case NoteOutputTypes.notNoise:
                                    if (mvarGB4Chan < 32)
                                    {
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[mvarGB4Chan].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(NoteArray[mvarGB4Chan].ParentChannel, NoteArray[mvarGB4Chan].NoteNumber + MidiPatchTrans[NoteArray[mvarGB4Chan].PatchNumber]);
                                        }
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[mvarGB4Chan].ParentChannel)}{Chr(NoteArray[mvarGB4Chan].NoteNumber)}{Chr(0)}", TotalTicks);
                                        NoteArray[mvarGB4Chan].FModChannel = 0;
                                        try { SappyChannels[NoteArray[mvarGB4Chan].ParentChannel].Notes.Remove(Str(mvarGB4Chan)); } catch (Exception) { }
                                        NoteArray[mvarGB4Chan].Enabled = false;
                                    }
                                    mvarGB4Chan = x;
                                    break;
                            }

                            if (mvarOutputType == SongOutputTypes.sotWave)
                            {
                                if (!mutethis)
                                {
                                    NoteArray[x].FModChannel = FSOUND_PlaySound(x + 1, SamplePool[das].FModSample);
                                }
                            }
                            else
                            {
                                NoteArray[x].FModChannel = Item.ParentChannel;
                            }
                            NoteArray[x].Frequency = daf;
                            NoteArray[x].Notephase = NotePhases.npInitial;
                            if (mvarOutputType == SongOutputTypes.sotWave)
                            {
                                FSOUND_SetFrequency(NoteArray[x].FModChannel, CInt(daf) * (int)Math.Pow(Math.Pow(2, 1d / 12), CInt(SappyChannels[Item.ParentChannel].PitchBend - 0x40) / CInt(0x40) * CInt(SappyChannels[Item.ParentChannel].PitchBendRange)));
                                FSOUND_SetVolume(NoteArray[x].FModChannel, dav * (SappyChannels[Item.ParentChannel].mute ? 0 : 1));
                                FSOUND_SetPan(NoteArray[x].FModChannel, SappyChannels[Item.ParentChannel].Panning * 2);
                            }
                            else
                            {
                                // MIDIPLAYSOUND
                                if (!SappyChannels[Item.ParentChannel].mute)
                                {
                                    // If Item.PatchNumber = 127 Then 'easy way
                                    if (DrumKitExists(Item.PatchNumber)) // better way
                                    {
                                        ToneOn(9, MidiDrumMap[Item.NoteNumber], Item.Velocity);
                                    }
                                    else
                                    {
                                        ToneOn(Item.ParentChannel, Item.NoteNumber + MidiPatchTrans[Item.PatchNumber], Item.Velocity);
                                    }
                                }
                            }
                            if (Recording)
                            {
                                if (DrumKitExists(Item.PatchNumber)) // better way
                                {
                                    BufferEvent($"{Chr(0x99)}{Chr(MidiDrumMap[Item.NoteNumber])}{Chr(Item.Velocity)}", TotalTicks);
                                }
                                else
                                {
                                    BufferEvent($"{Chr(0x90 + Item.ParentChannel)}{Chr(Item.NoteNumber + MidiPatchTrans[Item.PatchNumber])}{Chr(Item.Velocity)}", TotalTicks);
                                }
                            }
                            PlayedANote?.Invoke((byte)Item.ParentChannel, Item.NoteNumber, Item.Velocity);
                        }
                    }
                }
            }
            NoteQueue.Clear();

            if (mvarNoteFrameCounter > 0)
            {
                for (int i = 0; i <= 31; i++)
                {
                    if (NoteArray[i].Enabled)
                    {
                        if (NoteArray[i].outputtype == NoteOutputTypes.notDirect)
                        {
                            if (NoteArray[i].NoteOff && NoteArray[i].Notephase < NotePhases.npRelease)
                            {
                                NoteArray[i].EnvStep = 0;
                                NoteArray[i].Notephase = NotePhases.npRelease;
                            }
                            if (NoteArray[i].EnvStep == 0 || NoteArray[i].EnvPosition == NoteArray[i].EnvDestination || NoteArray[i].EnvStep == 0 && NoteArray[i].EnvPosition <= NoteArray[i].EnvDestination || NoteArray[i].EnvStep >= 0 && NoteArray[i].EnvPosition >= NoteArray[i].EnvDestination)
                            {
                                switch (NoteArray[i].Notephase)
                                {
                                    case NotePhases.npInitial:
                                        NoteArray[i].Notephase = NotePhases.npAttack;
                                        NoteArray[i].EnvPosition = 0;
                                        NoteArray[i].EnvDestination = 255;
                                        NoteArray[i].EnvStep = NoteArray[i].EnvAttenuation;
                                        break;
                                    case NotePhases.npAttack:
                                        NoteArray[i].Notephase = NotePhases.npDecay;
                                        NoteArray[i].EnvDestination = NoteArray[i].EnvSustain;
                                        NoteArray[i].EnvStep = (NoteArray[i].EnvDecay - 0x100) / 2;
                                        break;
                                    case NotePhases.npDecay:
                                        NoteArray[i].Notephase = NotePhases.npSustain;
                                        NoteArray[i].EnvStep = 0;
                                        break;
                                    case NotePhases.npSustain:
                                        NoteArray[i].Notephase = NotePhases.npSustain;
                                        NoteArray[i].EnvStep = 0;
                                        break;
                                    case NotePhases.npRelease:
                                        NoteArray[i].Notephase = NotePhases.npNoteOff;
                                        NoteArray[i].EnvDestination = 0;
                                        NoteArray[i].EnvStep = NoteArray[i].EnvRelease - 0x100;
                                        break;
                                    case NotePhases.npNoteOff:
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[i].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(NoteArray[i].ParentChannel, NoteArray[i].NoteNumber + MidiPatchTrans[NoteArray[i].PatchNumber]);
                                            // Watch this: enabling this makes the file unreadable
                                            // BufferEvent(Chr(&H80 + NoteArray[i].ParentChannel) & Chr(NoteArray[i].NoteNumber), this.TotalTicks, NoteArray[i].ParentChannel)
                                        }
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[i].ParentChannel)}{Chr(NoteArray[i].NoteNumber)}{Chr(0)}", TotalTicks);
                                        NoteArray[i].FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels[NoteArray[i].ParentChannel].Notes.Remove(Str(i));
                                        // TODO: (NOT SUPPORTED): On Error GoTo 0
                                        NoteArray[i].Enabled = false;
                                        break;
                                }
                            }
                            // NoteArray[i].EnvStep = NoteArray[i].EnvStep * 1
                            decimal nex = NoteArray[i].EnvPosition + NoteArray[i].EnvStep;
                            if (nex > NoteArray[i].EnvDestination && NoteArray[i].EnvStep > 0) nex = NoteArray[i].EnvDestination;
                            if (nex < NoteArray[i].EnvDestination && NoteArray[i].EnvStep < 0) nex = NoteArray[i].EnvDestination;
                            NoteArray[i].EnvPosition = nex;
                            int dav = CInt(CInt(NoteArray[i].Velocity) / CInt(0x7F) * (CInt(SappyChannels[NoteArray[i].ParentChannel].MainVolume) / CInt(0x7F)) * (CInt(Int(NoteArray[i].EnvPosition)) / CInt(0xFF)) * 255);
                            if (mutethis) dav = 0;

                            if (mvarOutputType == SongOutputTypes.sotWave)
                            {
                                FSOUND_SetVolume(NoteArray[i].FModChannel, dav * (SappyChannels[NoteArray[i].ParentChannel].mute ? 0 : 1));
                            }
                            else
                            {
                                // MIDISETVOL
                                SetChnVolume(NoteArray[i].FModChannel, dav * (SappyChannels[NoteArray[i].ParentChannel].mute ? 0 : 1));
                            }
                            // TODO: (NOT SUPPORTED): On Error Resume Next
                            if (Recording) BufferEvent($"{Chr(0xD0 + NoteArray[i].FModChannel)}{Chr(dav)}", TotalTicks);
                            // TODO: (NOT SUPPORTED): On Error GoTo 0
                        }
                        else
                        {
                            // GB Envelope
                            if (NoteArray[i].NoteOff && NoteArray[i].Notephase < NotePhases.npRelease)
                            {
                                NoteArray[i].EnvStep = 0;
                                NoteArray[i].Notephase = NotePhases.npRelease;
                            }
                            if (NoteArray[i].EnvStep == 0 || NoteArray[i].EnvPosition == NoteArray[i].EnvDestination || NoteArray[i].EnvStep == 0 && NoteArray[i].EnvPosition <= NoteArray[i].EnvDestination || NoteArray[i].EnvStep >= 0 && NoteArray[i].EnvPosition >= NoteArray[i].EnvDestination)
                            {
                                switch (NoteArray[i].Notephase)
                                {
                                    case NotePhases.npInitial:
                                        NoteArray[i].Notephase = NotePhases.npAttack;
                                        NoteArray[i].EnvPosition = 0;
                                        NoteArray[i].EnvDestination = 255;
                                        NoteArray[i].EnvStep = 0x100 - NoteArray[i].EnvAttenuation * 8;
                                        break;
                                    case NotePhases.npAttack:
                                        NoteArray[i].Notephase = NotePhases.npDecay;
                                        NoteArray[i].EnvDestination = NoteArray[i].EnvSustain;
                                        NoteArray[i].EnvStep = -NoteArray[i].EnvDecay * 2;
                                        break;
                                    case NotePhases.npDecay:
                                        NoteArray[i].Notephase = NotePhases.npSustain;
                                        NoteArray[i].EnvStep = 0;
                                        break;
                                    case NotePhases.npSustain:
                                        NoteArray[i].Notephase = NotePhases.npSustain;
                                        NoteArray[i].EnvStep = 0;
                                        break;
                                    case NotePhases.npRelease:
                                        NoteArray[i].Notephase = NotePhases.npNoteOff;
                                        NoteArray[i].EnvDestination = 0;
                                        NoteArray[i].EnvStep = (0x8 - NoteArray[i].EnvRelease) * 2;
                                        break;
                                    case NotePhases.npNoteOff:
                                        switch (NoteArray[i].outputtype)
                                        {
                                            case NoteOutputTypes.notSquare1:
                                                mvarGB1Chan = 255;
                                                break;
                                            case NoteOutputTypes.notSquare2:
                                                mvarGB2Chan = 255;
                                                break;
                                            case NoteOutputTypes.notWave:
                                                mvarGB3Chan = 255;
                                                break;
                                            case NoteOutputTypes.notNoise:
                                                mvarGB4Chan = 255;
                                                break;
                                        }
                                        if (mvarOutputType == SongOutputTypes.sotWave)
                                        {
                                            FSOUND_StopSound(NoteArray[i].FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(NoteArray[i].ParentChannel, NoteArray[i].NoteNumber + MidiPatchTrans[NoteArray[i].PatchNumber]);
                                        }
                                        // Watch this...
                                        if (Recording) BufferEvent($"{Chr(0x80 + NoteArray[i].ParentChannel)}{Chr(NoteArray[i].NoteNumber)}{Chr(0)}", TotalTicks);
                                        NoteArray[i].FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels[NoteArray[i].ParentChannel].Notes.Remove(Str(i));
                                        // TODO: (NOT SUPPORTED): On Error GoTo 0
                                        NoteArray[i].Enabled = false;
                                        break;
                                }
                            }
                            // NoteArray[i].EnvStep = NoteArray[i].EnvStep * 1
                            decimal nex = NoteArray[i].EnvPosition + NoteArray[i].EnvStep;
                            if (nex > NoteArray[i].EnvDestination && NoteArray[i].EnvStep > 0) nex = NoteArray[i].EnvDestination;
                            if (nex < NoteArray[i].EnvDestination && NoteArray[i].EnvStep < 0) nex = NoteArray[i].EnvDestination;
                            NoteArray[i].EnvPosition = nex;

                            int dav = CInt(CInt(NoteArray[i].Velocity) / CInt(0x7F) * (CInt(SappyChannels[NoteArray[i].ParentChannel].MainVolume) / CInt(0x7F)) * (CInt(Int(NoteArray[i].EnvPosition)) / CInt(0xFF)) * 255);
                            if (mutethis) dav = 0;
                            if (mvarOutputType == SongOutputTypes.sotWave)
                            {
                                FSOUND_SetVolume(NoteArray[i].FModChannel, dav * (SappyChannels[NoteArray[i].ParentChannel].mute ? 0 : 1));
                            }
                            else
                            {
                                // MIDISETVOL
                                SetChnVolume(NoteArray[i].FModChannel, dav * (SappyChannels[NoteArray[i].ParentChannel].mute ? 0 : 1));
                            }
                            // TODO: (NOT SUPPORTED): On Error Resume Next
                            if (Recording) BufferEvent($"{Chr(0xD0 + NoteArray[i].FModChannel)}{Chr(dav)}", TotalTicks);
                            // TODO: (NOT SUPPORTED): On Error GoTo 0
                        }
                    }
                }
            }

            bool xmmm = false;
            for (int i = 1; i <= SappyChannels.count; i++)
            {
                if (SappyChannels[i].Enabled) xmmm = true;
            }
            if (xmmm == false || mvarTempo == 0)
            {
                StopSong();
                SongFinish.Invoke();
                return;
            }
        }
        mvarLastTick = 0;
        mvarTickCounter = 0;
        incr++;
        if (incr >= Int(60000 / (mvarTempo * SappyPPQN)))
        {
            mvarTickCounter = 1; // (8 / (60000 / (mvarTempo * SappyPPQN)))
            TotalTicks++;
            if (TotalTicks % 48 == 0)
            {
                Beats++;
                Beat.Invoke(Beats);
            }
            incr = 0;
        }

        mvarNoteFrameCounter = 1; // (8 / (60000 / (mvarTempo * SappyPPQN)))

        if (mvarTempo != lasttempo)
        {
            lasttempo = mvarTempo;
            EventProcessor.Enabled = false;
            EventProcessor.EventType = EEventTypes.etPeriodic;
            EventProcessor.Interval = 1;
            EventProcessor.Resolution = 1;
            EventProcessor.Enabled = true;
        }
        return;
    hell:;
        MsgBox("RTE " + Err().Number + " - " + Err().Description, vbCritical);
        StopSong();
    }

    public static bool DirectExists(SDirects DirectsCollection, byte DirectID)
    {
        foreach (var Item in DirectsCollection)
        {
            if (Val(Item.Key) == DirectID)
            {
                return true;
            }
        }
        return false;
    }
    public static bool KeyMapExists(SKeyMaps KeyMapCollection, byte KeyMapID)
    {
        foreach (var Item in KeyMapCollection)
        {
            if (Val(Item.Key) == KeyMapID)
            {
                return true;
            }
        }
        return false;
    }
    public bool PatchExists(byte patch)
    {
        foreach (var Item in Directs)
        {
            if (Val(Item.Key) == patch)
            {
                return true;
            }
        }
        foreach (var Item in Instruments)
        {
            if (Val(Item.Key) == patch)
            {
                return true;
            }
        }
        foreach (var Item in DrumKits)
        {
            if (Val(Item.Key) == patch)
            {
                return true;
            }
        }
        return false;
    }
    public bool InstrumentExists(byte patch)
    {
        foreach (var Item in Instruments)
        {
            if (Val(Item.Key) == patch)
            {
                return true;
            }
        }
        return false;
    }
    public bool DrumKitExists(byte patch)
    {
        foreach (var Item in DrumKits)
        {
            if (Val(Item.Key) == patch)
            {
                return true;
            }
        }
        return false;
    }
    public bool SampleExists(int SampleID)
    {
        foreach (var Item in SamplePool)
        {
            if (Val(Item.Key) == SampleID)
            {
                return true;
            }
        }
        return false;
    }

    public byte FreeNote()
    {
        for (byte i = 0; i <= 31; i++)
        {
            if (!NoteArray[i].Enabled)
            {
                return i;
            }
        }
        return 255;
    }

    private void WriteVarLen(int ch, int Value)
    {
        // This sets the most significant bits of the value wrong, so
        // I need to fix this. (Drag)
        // buffer = Value And &H7F
        // While Value \ 128 > 0
        // Value = Value \ 128
        // buffer = buffer * 256
        // buffer = buffer Or ((Value And &H7F) Or &H80)
        // Wend
        // The following is my code. (Drag)
        int buffer = Value & 0x7F;
        while (Value / 128 > 0)
        {
            Value /= 128;
            buffer |= 0x80;
            buffer = (buffer * 256) | (Value & 0x7F);
        }

        while (true)
        {
            FilePutObject(ch, (byte)(buffer & 255)); // : Pos = Pos + 1
            if ((buffer & 0x80) != 0)
            {
                buffer /= 256;
            }
            else
            {
                break;
            }
        }
    }

    public static int FlipLong(int Value)
    {
        string[] b = new string[4];
        string s1 = Strings.Right("00000000" + Hex(Value), 8);
        b[0] = Strings.Mid(s1, 1, 2);
        b[1] = Strings.Mid(s1, 3, 2);
        b[2] = Strings.Mid(s1, 5, 2);
        b[3] = Strings.Mid(s1, 7, 2);
        string s2 = b[3] + b[2] + b[1] + b[0];
        return (int)Val("&H" + s2);
        // Dim b1 As Byte, b2 As Byte, b3 As Byte, b4 As Byte
        // On Error Resume Next
        // b1 = value Mod &H100
        // value = value \ &H100
        // b2 = value Mod &H100
        // value = value \ &H100
        // b3 = value Mod &H100
        // value = value \ &H100
        // b4 = value Mod &H100

        // value = b1
        // value = value * &H100
        // value = value + b2
        // value = value * &H100
        // value = value + b3
        // value = value * &H100
        // value = value + b4

        // FlipLong = value
    }

    public static int FlipInt(int Value)
    {
        byte b1 = (byte)(Value % 0x100);
        Value /= 0x100;
        byte b2 = (byte)(Value % 0x100);

        Value = b1;
        Value *= 0x100;
        Value += b2;

        return Value;
    }

    private void GetSample(SDirect D, SappyDirectHeader dirhead, ref SappySampleHeader smphead, bool UseReadString)
    {
        Console.WriteLine("GetSample -> 0x" + Hex(dirhead.pSampleHeader) + " (" + (UseReadString ? "readstring" : "seek") + ")");
        D.SampleID = dirhead.pSampleHeader.ToString();
        int sid = int.Parse(D.SampleID);
        if (!SampleExists(sid))
        {
            SamplePool.Add(Str(sid));
            if (D.outputtype == DirectOutputTypes.dotDirect)
            {
                smphead = ReadSampleHead(1, GBAROMPointerToOffset(sid));
                SamplePool[Str(sid)].Size = smphead.wSize;
                SamplePool[Str(sid)].Frequency = smphead.wFreq * 64;
                SamplePool[Str(sid)].loopstart = smphead.wLoop;
                SamplePool[Str(sid)].LoopEnable = smphead.flags > 0;
                SamplePool[Str(sid)].GBWave = false;
                if (UseReadString)
                {
                    SamplePool[Str(sid)].SampleData = ReadString(1, SamplePool[Str(sid)].Size);
                }
                else
                {
                    SamplePool[Str(sid)].SampleData = Seek(1).ToString();
                }
            }
            else
            {
                SamplePool[Str(sid)].Size = 32;
                SamplePool[Str(sid)].Frequency = GBWaveBaseFreq;
                SamplePool[Str(sid)].loopstart = 0;
                SamplePool[Str(sid)].LoopEnable = true;
                SamplePool[Str(sid)].GBWave = true;
                string tsi = ReadString(1, 16, GBAROMPointerToOffset(sid));
                SamplePool[Str(sid)].SampleData = "";
                for (int ai = 0; ai <= 31; ai += 1)
                {
                    int bi = ai % 2;
                    SamplePool[Str(sid)].SampleData = SamplePool[Str(sid)].SampleData + Chr((int)Int((Strings.Mid(tsi, ai / 2 + 1, 1) == "" ? 0 : Strings.Mid(tsi, ai / 2 + 1, 1)[0]) / (int)Math.Pow(16, bi) % 16 * (GBWaveMulti * 16)));
                }
            }
        }
    }

    private void GetSampleWithMulti(SDirect D, SappyDirectHeader dirhead, ref SappySampleHeader smphead, bool UseReadString)
    {
        Console.WriteLine("GetSample -> 0x" + Hex(dirhead.pSampleHeader) + " (" + (UseReadString ? "readstring" : "seek") + ", multi)");
        D.SampleID = dirhead.pSampleHeader.ToString();
        int sid = int.Parse(D.SampleID);
        if (!SampleExists(sid))
        {
            SamplePool.Add(Str(sid));
            if (D.outputtype == DirectOutputTypes.dotDirect)
            {
                smphead = ReadSampleHead(1, GBAROMPointerToOffset(sid));
                SamplePool[Str(sid)].Size = smphead.wSize;
                SamplePool[Str(sid)].Frequency = (int)(smphead.wFreq * CLng(64));
                SamplePool[Str(sid)].loopstart = smphead.wLoop;
                SamplePool[Str(sid)].LoopEnable = smphead.flags > 0;
                SamplePool[Str(sid)].GBWave = false;
                if (UseReadString)
                {
                    SamplePool[Str(sid)].SampleData = ReadString(1, SamplePool[Str(sid)].Size);
                }
                else
                {
                    SamplePool[Str(sid)].SampleData = Seek(1).ToString();
                }
            }
            else
            {
                SamplePool[Str(sid)].Size = 32;
                SamplePool[Str(sid)].Frequency = GBWaveBaseFreq;
                SamplePool[Str(sid)].loopstart = 0;
                SamplePool[Str(sid)].LoopEnable = true;
                SamplePool[Str(sid)].GBWave = true;
                string tsi = ReadString(1, 16, GBAROMPointerToOffset(sid));
                SamplePool[Str(sid)].SampleData = "";
                for (int ai = 0; ai <= 31; ai += 1)
                {
                    int bi = ai % 2;
                    SamplePool[Str(sid)].SampleData = SamplePool[Str(sid)].SampleData + Chr((int)Int((Strings.Mid(tsi, ai / 2 + 1, 1) == "" ? 0 : Strings.Mid(tsi, ai / 2 + 1, 1)[0]) / (int)Math.Pow(16, bi) % 16 * (GBWaveMulti * 16)));
                }
            }
        }
    }

    private void SetStuff(SDirect foo, SappyInstrumentHeader inshead, SappyDirectHeader dirhead, SappyNoiseHeader gbhead)
    {
        foo.DrumTuneKey = inshead.bDrumPitch;
        foo.outputtype = (DirectOutputTypes)(inshead.bChannel & 7);
        foo.EnvAttenuation = dirhead.bAttack;
        foo.EnvDecay = dirhead.bHold;
        foo.EnvSustain = dirhead.bSustain;
        foo.EnvRelease = dirhead.bRelease;
        foo.Raw0 = dirhead.b0;
        foo.Raw1 = dirhead.b1;
        foo.GB1 = gbhead.b2;
        foo.GB2 = gbhead.b3;
        foo.GB3 = gbhead.b4;
        foo.GB4 = gbhead.b5;
        foo.FixedPitch = (inshead.bChannel & 0x8) == 0x8;
        foo.Reverse = (inshead.bChannel & 0x10) == 0x10;
    }
}
