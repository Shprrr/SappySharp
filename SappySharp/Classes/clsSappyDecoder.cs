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
    int lasttempo = 0;

    /// <summary>
    /// Collection of channels.
    /// </summary>
    SChannels SappyChannels = new();
    NoteInfos NoteQueue = new();
    NoteInfo[] NoteArray = new NoteInfo[32];

    WithEvents CTimerMM EventProcessor = null;
    WithEvents CTimerMM NoteProcessor = null;

    int TotalTicks = 0; // by Kawa
    int TotalMSecs = 0; // by Kawa
    int Beats = 0; // by Kawa
    delegate void EmptyEventHandler();
    event EmptyEventHandler SongLoop; // by Kawa
    /// <summary>
    /// Raised when the song finishes playing.
    /// </summary>
    event EmptyEventHandler SongFinish;
    /// <summary>
    /// Raised when there's new tracking info to display.
    /// </summary>
    event EmptyEventHandler UpdateDisplay;
    delegate void ChangedTempoEventHandler(int newtempo);
    /// <summary>
    /// Raised when a Tempo command is found in the song data
    /// </summary>
    event ChangedTempoEventHandler ChangedTempo;
    delegate void PlayedANoteEventHandler(byte channelid, byte notenum, byte lenno);
    event PlayedANoteEventHandler PlayedANote;
    delegate void LoadingEventHandler(int status);
    event LoadingEventHandler Loading;
    delegate void BeatEventHandler(int beats);
    event BeatEventHandler Beat;

    SSamples SamplePool = new();
    SDirects Directs = new();
    SDrumKits DrumKits = new();
    SInstruments Instruments = new();

    // local variable(s) to hold property value(s)
    decimal mvarNoteFrameCounter = 0;
    decimal mvarTickCounter = 0;
    decimal mvarLastTick = 0;
    string mvarFileName = "";
    int mvarTranspose = 0;
    int mvarSongListPointer = 0;
    int mvarSongPointer = 0;
    int mvarSongNumber = 0;
    int mvarLayer = 0;
    int mvarInstrumentTablePointer = 0;
    int mvarCurrentSong = 0;
    int incr = 0;
    int incr2 = 0;
    byte mvarGB1Chan = 0;
    byte mvarGB2Chan = 0;
    byte mvarGB3Chan = 0;
    byte mvarGB4Chan = 0;
    bool mvarPlaying = false;
    int mvarTempo = 0;
    int InstrumentTable = 0;

    public enum SongOutputTypes
    {
        sotNull = 0
    , sotWave = 1
    , sotMIDI = 2
    }

    SongOutputTypes mvarOutputType;
    int mvarGlobalVolume = 0; // by Kawa

    int GBWaveBaseFreq = 880;
    int SappyPPQN = 24;

    int[] MidiPatchMap = new int[127]; // by Kawa
    int[] MidiPatchTrans = new int[127];
    int[] MidiDrumMap = new int[127];
    int[] EarPiercers = new int[127];
    int EarPiercerCnt = 0;

    bool PlayingNow = false;

    class tBufferRawMidiEvent
    {
        public int RawDelta;
        public int Ticks;
        public string EventCode;
    }
    // Private Type tBufferTrack
    // Length As Long
    // Events(-1 To 5000) As tBufferRawMidiEvent
    // End Type
    // Private BufferTrack(32) As tBufferTrack
    tBufferRawMidiEvent PreviousEvent = null;
    bool Recording = false;
    int midifile = 0;

    private void BufferEvent(ref string EventCode, int Ticks)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo hell
        if (Recording == false) return;
        if (midifile != 42) return;
        tBufferRawMidiEvent newevent = new();
        // Ticks = Ticks * (mvarTempo / 60) '/ (60000000 / SappyPPQN) 'MATH!
        newevent.Ticks = Ticks;
        newevent.RawDelta = Ticks - PreviousEvent.Ticks;
        newevent.EventCode = EventCode;
        WriteVarLen(midifile, newevent.RawDelta);
        //Put(#midifile, , newevent.EventCode); //TODO: Put
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
        for (int i = 0; i <= 127; i += 1)
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

    public bool NoteBelongsToChannel(byte NoteID, int channelid)
    {
        bool _NoteBelongsToChannel = false;
        _NoteBelongsToChannel = NoteArray[NoteID].ParentChannel == channelid;
        return _NoteBelongsToChannel;
    }

    public void StopSong()
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        Console.WriteLine("StopSong()");
        CloseFile(1);
        CloseFile(2);
        EventProcessor.Enabled = false;
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
            //Put(#midifile, , CByte(10)); //TODO: Put
            //Put(#midifile, , CByte(0xFF));
            //Put(#midifile, , CByte(0x2F));
            //Put(#midifile, , CByte(0));

            int tlen = 0; // ...to here. (Drag)
            tlen = LOF(midifile) - 22; // This line should now give a more accurate track length. (Drag)
            Console.WriteLine("StopSong(): Track length is " + tlen + ", total ticks " + this.TotalTicks);

            // Put #midifile, , CLng(0) // Why are these here? (Drag)
            // Put #midifile, , CLng(0)
            //Put(#midifile, 0x13, FlipLong(tlen)); // FlipLong(&H12345678) //TODO: Put
            Close(midifile);
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
    public void PlaySong(string Filename, int SongNumber, string SongListPointer, bool WantToRecord = false, string RecordTo = "midiout.mid")
    {
        // TODO: (NOT SUPPORTED): if (ErrorChecking == true) On Error GoTo hell

        mvarFileName = Filename;
        mvarSongListPointer = SongListPointer;
        mvarSongNumber = SongNumber;

        if (mvarPlaying == true) StopSong();

        SappyInstrumentHeader inshead = null;
        SappyDrumKitHeader drmhead = null;
        SappyDirectHeader dirhead = null;
        SappySampleHeader smphead = null;
        SappyMultiHeader mulhead = null;
        SappyNoiseHeader gbhead = null;

        SappyChannels.Clear();
        DrumKits.Clear();
        SamplePool.Clear();
        Instruments.Clear();
        Directs.Clear();
        NoteQueue.Clear();
        for (int i = 0; i <= 31; i += 1)
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
        for (int i = 1; i <= b; i += 1)
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
                VBOpenFile(7, "trackseven.txt", "Output");
            }

            int cticks = 0;
            int cEI = 0;
            int lc = 0xBE;
            List<var> lln = new(); //  TODO: (NOT SUPPORTED) Array ranges not supported: lln(0 To 65)
            byte[] llv = new byte[65];
            List<var> lla = new(); //  TODO: (NOT SUPPORTED) Array ranges not supported: lla(0 To 65)
            byte lp = 0;
            int src2 = 1;
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

                if (i == 7) VBWriteFile(7, Right("000000" + Hex(pc), 6) + vbTab + Hex(c));
                if (pc == 0x11BE31)
                {
                    VBWriteFile(7, "Warning!");
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
                    while (g == false)
                    {
                        byte D = ReadByte(1);

                        if (i == 7) VBWriteFile(7, Right("000000" + Hex(pc), 6) + vbTab + "  " + Hex(D));

                        if (D >= 0x80)
                        {
                            if (nc == 0)
                            {
                                int pn = lln[nc] + tR;
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
                            if (i == 7) VBWriteFile(7, Right("000000" + Hex(pc), 6) + vbTab + "    " + Hex(e));
                            byte F;
                            if (e < 0x80)
                            {
                                llv[nc] = e;
                                pc++;
                                F = ReadByte(1);
                                if (i == 7) VBWriteFile(7, Right("000000" + Hex(pc), 6) + vbTab + "      " + Hex(F));
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
                            int pn = D + tR;
                            SappyChannels[i].EventQueue.Add(cticks, c, pn, e, F);
                        }

                        // TODO: (NOT SUPPORTED): On Error Resume Next

                        if (PatchExists(lp) == false)
                        {
                            inshead = ReadInstrumentHead(1, (int)(mvarInstrumentTablePointer + CLng(lp) * CLng(12)));

                            if ((inshead.bChannel & 0x80) == 0x80)
                            {
                                drmhead = ReadDrumKitHead(1);
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)) + 2);
                                DrumKits.Add(Str(lp));
                                DrumKits(Str(lp)).Directs.Add Str(pn);
                                SetStuff(DrumKits(Str(lp)).Directs(Str(pn)), inshead, dirhead, gbhead);
                                if (Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotDirect || Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotWave) GetSample(DrumKits(Str(lp)).Directs(Str(pn)), dirhead, smphead, false);

                            }
                            else if ((inshead.bChannel & 0x40) == 0x40)
                            { // multi
                                mulhead = ReadMultiHead(1);
                                Instruments.Add(Str(lp));
                                Instruments(Str(lp)).KeyMaps.Add 0, Str(pn);
                                Instruments(Str(lp)).KeyMaps(Str(pn)).AssignDirect = ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn);
                                cdr = Instruments(Str(lp)).KeyMaps(Str(pn)).AssignDirect;
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)) + 2);
                                Instruments(Str(lp)).Directs.Add Str(cdr);
                                SetStuff(Instruments(Str(lp)).Directs(Str(cdr)), inshead, dirhead, gbhead);
                                if (Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotDirect || Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotWave) GetSample(Instruments(Str(lp)).Directs(Str(cdr)), dirhead, smphead, false);

                            }
                            else
                            { // direct or gb sample

                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, mvarInstrumentTablePointer + CLng(lp) * CLng(12) + 2);
                                Directs.Add(Str(lp));
                                SetStuff(Directs(Str(lp)), inshead, dirhead, gbhead);
                                if (Directs(Str(lp)).outputtype == dotDirect || Directs(Str(lp)).outputtype == dotWave) GetSample(Directs(Str(lp)), dirhead, smphead, true);

                            }

                        }
                        else
                        { // patch already exists

                            inshead = ReadInstrumentHead(1, mvarInstrumentTablePointer + CLng(lp) * CLng(12));
                            if ((inshead.bChannel && 0x80) == 0x80)
                            {
                                drmhead = ReadDrumKitHead(1);
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)) + 2);
                                if (DirectExists(DrumKits(Str(lp)).Directs, pn) == false)
                                {
                                    DrumKits(Str(lp)).Directs.Add Str(pn);
                                    SetStuff(DrumKits(Str(lp)).Directs(Str(pn)), inshead, dirhead, gbhead);
                                    if (DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotDirect || DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotWave) GetSampleWithMulti(DrumKits(Str(lp)).Directs(Str(pn)), dirhead, smphead, false);
                                }

                            }
                            else if ((inshead.bChannel && 0x40) == 0x40)
                            { // multi
                                mulhead = ReadMultiHead(1);
                                if (KeyMapExists(Instruments(Str(lp)).KeyMaps, pn) == false) Instruments(Str(lp)).KeyMaps.Add ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn);
                                cdr = Instruments(Str(lp)).KeyMaps(Str(pn)).AssignDirect;
                                inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)));
                                dirhead = ReadDirectHead(1);
                                gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)) + 2);
                                if (DirectExists(Instruments(Str(lp)).Directs, cdr) == false)
                                {
                                    Instruments(Str(lp)).Directs.Add Str(cdr);
                                    SetStuff(Instruments(Str(lp)).Directs(Str(cdr)), inshead, dirhead, gbhead);
                                    if (Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotDirect || Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotWave) GetSampleWithMulti(Instruments(Str(lp)).Directs(Str(cdr)), dirhead, smphead, false);
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
                        SappyChannels(i).EventQueue.Add cticks, lc, c, 0, 0;
                        pc++;
                    }
                    else
                    {
                        c = lc;
                        ReadOffset(1, pc);
                        g = false;
                        nc = 0;
                        while (g == false)
                        {
                            D = ReadByte(1);
                            if (D >= 0x80)
                            {
                                if (nc == 0)
                                {
                                    pn = lln[nc] + tR;
                                    SappyChannels(i).EventQueue.Add cticks, c, pn, llv(nc), lla(nc);
                                }
                                g = true;
                            }
                            else
                            {
                                lln[nc] = D;
                                pc++;
                                e = ReadByte(1);
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
                                        nc = nc + 1;
                                    }
                                }
                                else
                                {
                                    e = llv[nc];
                                    F = lla[nc];
                                    g = true;
                                }
                                pn = D + tR;
                                SappyChannels(i).EventQueue.Add cticks, c, pn, e, F;
                            }

                            // TODO: (NOT SUPPORTED): On Error Resume Next

                            if (PatchExists(lp) == false)
                            {
                                inshead = ReadInstrumentHead(1, mvarInstrumentTablePointer + CLng(lp) * CLng(12));
                                if ((inshead.bChannel && 0x80) == 0x80)
                                {
                                    drmhead = ReadDrumKitHead(1);
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)) + 2);
                                    DrumKits.Add(Str(lp));
                                    DrumKits(Str(lp)).Directs.Add Str(pn);
                                    SetStuff(DrumKits(Str(lp)).Directs(Str(pn)), inshead, dirhead, gbhead);
                                    if (DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotDirect || DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotWave) GetSample(DrumKits(Str(lp)).Directs(Str(pn)), dirhead, smphead, true);

                                }
                                else if ((inshead.bChannel && 0x40) == 0x40)
                                { // multi
                                    mulhead = ReadMultiHead(1);
                                    Instruments.Add(Str(lp));
                                    Instruments(Str(lp)).KeyMaps.Add ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn);
                                    cdr = Instruments(Str(lp)).KeyMaps(Str(pn)).AssignDirect;
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)) + 2);
                                    Instruments(Str(lp)).Directs.Add Str(cdr);
                                    SetStuff(Instruments(Str(lp)).Directs(Str(cdr)), inshead, dirhead, gbhead);
                                    if (Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotDirect || Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotWave) GetSampleWithMulti(Instruments(Str(lp)).Directs(Str(cdr)), dirhead, smphead, false);

                                }
                                else
                                { // direct or gb sample
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, mvarInstrumentTablePointer + CLng(lp) * CLng(12) + 2);
                                    Directs.Add(Str(lp));
                                    SetStuff(Directs(Str(lp)), inshead, dirhead, gbhead);
                                    if (Directs(Str(lp)).outputtype == dotDirect || Directs(Str(lp)).outputtype == dotWave) GetSampleWithMulti(Directs(Str(lp)), dirhead, smphead, false);
                                }

                            }
                            else
                            { // patch already exists
                                inshead = ReadInstrumentHead(1, mvarInstrumentTablePointer + CLng(lp) * CLng(12));
                                if ((inshead.bChannel && 0x80) == 0x80)
                                {
                                    drmhead = ReadDrumKitHead(1);
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(drmhead.pDirectTable + CLng(pn) * CLng(12)) + 2);
                                    if (DirectExists(DrumKits(Str(lp)).Directs, pn) == false)
                                    {
                                        DrumKits(Str(lp)).Directs.Add Str(pn);
                                        SetStuff(DrumKits(Str(lp)).Directs(Str(pn)), inshead, dirhead, gbhead);
                                        if (DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotDirect || DrumKits(Str(lp)).Directs(Str(pn)).outputtype == dotWave) GetSampleWithMulti(DrumKits(Str(lp)).Directs(Str(pn)), dirhead, smphead, false);
                                    }

                                }
                                else if ((inshead.bChannel && 0x40) == 0x40)
                                { // multi
                                    mulhead = ReadMultiHead(1);
                                    if (KeyMapExists(Instruments(Str(lp)).KeyMaps, pn) == false) Instruments(Str(lp)).KeyMaps.Add ReadByte(1, GBAROMPointerToOffset(mulhead.pKeyMap) + pn), Str(pn);
                                    cdr = Instruments(Str(lp)).KeyMaps(Str(pn)).AssignDirect;
                                    inshead = ReadInstrumentHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)));
                                    dirhead = ReadDirectHead(1);
                                    gbhead = ReadNoiseHead(1, GBAROMPointerToOffset(mulhead.pDirectTable + CLng(cdr) * CLng(12)) + 2);
                                    if (DirectExists(Instruments(Str(lp)).Directs, cdr) == false)
                                    {
                                        Instruments(Str(lp)).Directs.Add Str(cdr);
                                        SetStuff(Instruments(Str(lp)).Directs(Str(cdr)), inshead, dirhead, gbhead);
                                        if (Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotDirect || Instruments(Str(lp)).Directs(Str(cdr)).outputtype == dotWave) GetSampleWithMulti(Instruments(Str(lp)).Directs(Str(cdr)), dirhead, smphead, false);
                                    }
                                }
                            }

                            // TODO: (NOT SUPPORTED): On Error GoTo 0

                        }
                    }
                }
                else if (c >= 0x80 && c <= 0xB0)
                {
                    SappyChannels(i).EventQueue.Add cticks, c, 0, 0, 0;
                    cticks += SLen2Ticks(c - 0x80);
                    pc++;
                }
            } while (!(c == 0xB1 || c == 0xB2));

            SappyChannels(i).EventQueue.Add cticks, c, 0, 0, 0;
        }

        FSOUND_Init(44100, 64, 0);
        FSOUND_SetSFXMasterVolume(mvarGlobalVolume);
        MidiOpen();
        if (mvarOutputType == sotMIDI) goto SkipThatWholeInstrumentGarbish;

        Trace("===================");
        Trace("Filling sample pool");
        Trace("===================");
        int quark = 0;
        foreach (var iterItem in SamplePool)
        {
            Item = iterItem;
            RaiseEvent(Loading(1));
            // OpenFile 2, Item.Key & __S1
            // If Item.GBWave = True Then
            // WriteString 2, Item.SampleData
            // Else
            // For i = 0 To Item.Size
            // WriteByte 2, ReadByte(1, Item.SampleData + i)
            // Next i
            // End If
            // CloseFile 2
            // Item.SampleData = __S1
            // If Item.GBWave = True Then
            // Item.FModSample = FSOUND_Sample_Load(FSOUND_FREE, Item.Key & __S1, FSOUND_8BITS + FSOUND_LOADRAW + FSOUND_LOOP_NORMAL + FSOUND_MONO + FSOUND_UNSIGNED, 0, 0)
            // Call FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31)
            // Else
            // Item.FModSample = FSOUND_Sample_Load(FSOUND_FREE, Item.Key & __S1, FSOUND_8BITS + FSOUND_LOADRAW + IIf(Item.LoopEnable = True, FSOUND_LOOP_NORMAL, 0) + FSOUND_MONO + FSOUND_SIGNED, 0, 0)
            // Call FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1)
            // End If
            // DeleteFile Item.Key & __S1
            quark++;
            // On Error Resume Next
            Trace("#" + quark + " - " + Item.GBWave + " - " + Item.SampleData);
            if (Item.GBWave == true)
            {
                if (Val(Item.SampleData) == 0)
                {
                    OpenFile(2, "temp.raw");
                    WriteString(2, Item.SampleData);
                    CloseFile(2);
                    Item.FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "temp.raw", FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + FSOUND_MODES.FSOUND_LOOP_NORMAL + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
                    Call(FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31));
                    DeleteFile("temp.raw");
                }
                else
                {
                    Item.FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, Filename, FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + FSOUND_MODES.FSOUND_LOOP_NORMAL + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_UNSIGNED, Item.SampleData, Item.Size);
                    Call(FSOUND_Sample_SetLoopPoints(Item.FModSample, 0, 31));
                }
            }
            else
            {
                if (Val(Item.SampleData) == 0)
                {
                    // TODO: (NOT SUPPORTED): On Error Resume Next
                    OpenFile(2, "temp.raw");
                    // For i = 0 To Item.Size
                    // WriteByte 2, ReadByte(1, Item.SampleData + i)
                    // Next i
                    WriteString(2, Item.SampleData);
                    CloseFile(2);
                    Item.FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "temp.raw", FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + IIf(Item.LoopEnable == true, FSOUND_MODES.FSOUND_LOOP_NORMAL, 0) + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_SIGNED, 0, 0);
                    Call(FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1));
                    DeleteFile("temp.raw");
                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                }
                else
                {
                    Item.FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, Filename, FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + IIf(Item.LoopEnable == true, FSOUND_MODES.FSOUND_LOOP_NORMAL, 0) + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_SIGNED, Item.SampleData, Item.Size);
                    Call(FSOUND_Sample_SetLoopPoints(Item.FModSample, Item.loopstart, Item.Size - 1));
                }
            }
            // TODO: (NOT SUPPORTED): On Error GoTo 0
        }

        for (i = 0; i <= 9; i += 1)
        {
            SamplePool.Add("noise0" + i);
            // TODO: (NOT SUPPORTED): With SamplePool("noise0" & i)
            Randomize(Timer);
    .SampleData = NoiseWaves(0, i);
    .Frequency = 7040;
    .Size = 16384;
            OpenFile(2, "noise0" + i + ".raw");
            WriteString(2, SampleData);
            CloseFile(2);
    .SampleData = "";
    .FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "noise0" + i + ".raw", FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + FSOUND_MODES.FSOUND_LOOP_NORMAL + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            Call(FSOUND_Sample_SetLoopPoints(FModSample, 0, 16383));
            DeleteFile("noise0" + i + ".raw");
            // TODO: (NOT SUPPORTED): End With
            SamplePool.Add("noise1" + i);

            // TODO: (NOT SUPPORTED): With SamplePool("noise1" & i)
            Randomize(Timer);
    .SampleData = NoiseWaves(1, i);
    .Frequency = 7040;
    .Size = 256;
            OpenFile(2, "noise1" + i + ".raw");
            WriteString(2, SampleData);
            CloseFile(2);
    .SampleData = "";
    .FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "noise1" + i + ".raw", FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + FSOUND_MODES.FSOUND_LOOP_NORMAL + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            Call(FSOUND_Sample_SetLoopPoints(FModSample, 0, 255));
            DeleteFile("noise1" + i + ".raw");
            // TODO: (NOT SUPPORTED): End With
        }

        for (mx2 = 0; mx2 <= 3; mx2 += 1)
        {
            SamplePool.Add("square" + mx2);
            // TODO: (NOT SUPPORTED): With SamplePool("square" & mx2)
            switch (mx2)
            {
                case 0: 
        .SampleData = String(4, Chr(Int(0x80 + 0x7F * GBSquareMulti))) + String(28, Chr(Int(0x80 - 0x7F * GBSquareMulti)));
                    break;
                case 1: 
        .SampleData = String(8, Chr(Int(0x80 + 0x7F * GBSquareMulti))) + String(24, Chr(Int(0x80 - 0x7F * GBSquareMulti)));
                    break;
                case 2: 
        .SampleData = String(16, Chr(Int(0x80 + 0x7F * GBSquareMulti))) + String(16, Chr(Int(0x80 - 0x7F * GBSquareMulti)));
                    break;
                case 3: 
        .SampleData = String(24, Chr(Int(0x80 + 0x7F * GBSquareMulti))) + String(8, Chr(Int(0x80 - 0x7F * GBSquareMulti)));
                    break;
            }
  .Frequency = 7040;
  .Size = 32;
            OpenFile(2, "square" + mx2 + ".raw");
            WriteString(2, SampleData);
            CloseFile(2);
  .SampleData = "";
  .FModSample = FSOUND_Sample_Load(FSOUND_CHANNELSAMPLEMODE.FSOUND_FREE, "square" + mx2 + ".raw", FSOUND_MODES.FSOUND_8BITS + FSOUND_MODES.FSOUND_LOADRAW + FSOUND_MODES.FSOUND_LOOP_NORMAL + FSOUND_MODES.FSOUND_MONO + FSOUND_MODES.FSOUND_UNSIGNED, 0, 0);
            Call(FSOUND_Sample_SetLoopPoints(FModSample, 0, 31));
            DeleteFile("square" + mx2 + ".raw");
            // TODO: (NOT SUPPORTED): End With
        }
        mvarGB1Chan = 255;
        mvarGB2Chan = 255;
        mvarGB3Chan = 255;
        mvarGB4Chan = 255;

    SkipThatWholeInstrumentGarbish:;
        RaiseEvent(Loading(2));

        EventProcessor = new CTimerMM();
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
            string H = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (4)
            if (Dir(RecordTo) != "") Kill(RecordTo);
            Open(RecordTo For Binary As midifile);
            H = "MThd";
            //Put(#midifile, , H); //TODO: Put
            //Put(#midifile, , FlipLong(6));
            //Put(#midifile, , FlipInt(0));
            //Put(#midifile, , FlipInt(1)); // FlipInt(SappyChannels.count)
            //Put(#midifile, , FlipInt(24)); // 48
            H = "MTrk";
            //Put(#midifile, , H);
            //Put(#midifile, , CLng(0));
            string msg = "";
            msg = frmSappy.instance.lblSongName;
            msg = msg + " --- dumped by " + App.Title + " version " + App.Major + "." + App.Minor;

            if (Len(msg) > 120) msg = left(msg, 120);
            BufferEvent(Chr$(0xFF) + Chr$(2) + Chr$(Len(msg)) + msg, 0);
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
    private void Class_Initialize()
    {
        int ts = 0;
        int te = 0;
        int sz = 0;
        Trace(DateTime.Now + vbTab + "- Yo, this be clsSappyDecoder, Class_Initialize()");
        Randomize(Timer);
        sz = GetSettingI(ref "Noise Length");
        if (sz == 0) sz = 2047;
        ts = GetTickCount;
        for (i = 0; i <= 9; i += 1)
        {
            Trace(DateTime.Now + vbTab + "- Creating NoiseWaves(0," + i + ")");
            for (j = 0; j <= sz; j += 1)
            { // 2047 '16383
                NoiseWaves(0, i) = NoiseWaves(0, i) & Chr(Int(Rnd * 153)); // (255 * 0.6)))
            }
            Trace(DateTime.Now + vbTab + "- Creating NoiseWaves(1," + i + ")");
            for (j = 0; j <= 255; j += 1)
            {
                NoiseWaves(1, i) = NoiseWaves(1, i) & Chr(Int(Rnd * 153)); // (255 * 0.6)))
            }
        }
        te = GetTickCount;
        Trace(DateTime.Now + vbTab + "- Took " + (te - ts) + ".");
        mvarGlobalVolume = 255;
        Trace(DateTime.Now + vbTab + "- Done. Back to the studio...");
    }

    private void EventProcessor_Timer(int lMilliseconds)
    {
        if (ErrorChecking == true)// TODO: (NOT SUPPORTED): On Error GoTo hell

            int ep = 0;
        bool mutethis = false;

        TotalMSecs += lMilliseconds;
        if (mvarTickCounter > 0)
        {
            for (i = 0; i <= 31; i += 1)
            {
                // TODO: (NOT SUPPORTED): With NoteArray(i)
                if (Enabled == true && WaitTicks > 0)
                {
      .WaitTicks = WaitTicks - (mvarTickCounter - mvarLastTick);
                }
                if (WaitTicks <= 0 && Enabled == true && NoteOff == false)
                {
                    if (SappyChannels(ParentChannel).Sustain == false)
                    {
        .NoteOff = true;
                    }
                }
                // TODO: (NOT SUPPORTED): End With
            }
            for (i = 1; i <= SappyChannels.count; i += 1)
            {

                while (SappyChannels(i).Enabled == false)
                {
                    i = i + 1;
                    if (i > SappyChannels.count) break;
                }

                // TODO: (NOT SUPPORTED): With SappyChannels(i)
                // mutethis = False
                for (ep = 0; ep <= EarPiercerCnt; ep += 1)
                {
                    if (EarPiercers(ep) == PatchNumber)
                    {
        .mute = true;
                        // .Enabled = False
                        break;
                    }
                }

                if (WaitTicks > 0).WaitTicks = WaitTicks - (mvarTickCounter - mvarLastTick);
                oo = false;
                OP = false;
                while (WaitTicks <= 0)
                {
                    // DoEvents
                    switch (EventQueue(ProgramCounter).CommandByte)
                    {

                        case 0xB1: 
          .Enabled = false;
                            break;

                            break;
                        case 0xB9: 
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xBB:
                            mvarTempo = EventQueue(ProgramCounter).Param1 * 2;
                            RaiseEvent(ChangedTempo(mvarTempo));
                            if (Recording) BufferEvent(Chr$(0xFF) + Chr$(0x51), this.TotalTicks);
                            //if (Recording) Put(#midifile, , FlipLong(((60000000 / mvarTempo) && 0xFFFFFF) || 0x3000000)); //TODO: Put
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xBC: 
          .Transpose = SignedByteToInteger(EventQueue(ProgramCounter).Param1);
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xBD: 
          .PatchNumber = EventQueue(ProgramCounter).Param1;
                            if (DirectExists(Directs, PatchNumber))
                            {
            .outputtype = Directs(Str(PatchNumber)).outputtype;
                            }
                            else if (InstrumentExists(PatchNumber))
                            {
            .outputtype = cotMultiSample;
                            }
                            else if (DrumKitExists(PatchNumber))
                            {
            .outputtype = cotDrumKit;
                            }
                            else
                            {
            .outputtype = cotNull;
                            }
          .ProgramCounter = ProgramCounter + 1;
                            SelectInstrument(i, MidiPatchMap(PatchNumber));
                            BufferEvent(Chr$(0xC0 + i) + Chr$(MidiPatchMap(PatchNumber)), this.TotalTicks);

                            break;
                        case 0xBE: 
           // Do Set Volume
          .MainVolume = EventQueue(ProgramCounter).Param1;
                            foreach (var iterItem in Notes)
                            {
                                Item = iterItem;
                                if (NoteArray(Item.NoteID).Enabled == true && NoteArray(Item.NoteID).ParentChannel == i)
                                {

                                    dav = CSng(CSng(NoteArray(Item.NoteID).Velocity) / CSng(0x7F) * (CSng(MainVolume) / CSng(0x7F)) * (CSng(Int(NoteArray(Item.NoteID).EnvPosition)) / CSng(0xFF)) * 255);
                                    if (mutethis) dav = 0;
                                    if (mvarOutputType == sotWave)
                                    {
                                        FSOUND_SetVolume(NoteArray(Item.NoteID).FModChannel, dav * IIf(mute, 0, 1));
                                    }
                                    else
                                    {
                                        // MIDISETVOL
                                        SetChnVolume(NoteArray(Item.NoteID).FModChannel, dav * IIf(mute, 0, 2));
                                    }
                                    // TODO: (NOT SUPPORTED): On Error Resume Next
                                    if (Recording) BufferEvent(Chr$(0xD0 + NoteArray(Item.NoteID).FModChannel) + Chr$(dav), this.TotalTicks);
                                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                                }
                            }
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xBF: 
           // Do Set Panning
          .Panning = EventQueue(ProgramCounter).Param1;
                            foreach (var iterItem in Notes)
                            {
                                Item = iterItem;
                                if (NoteArray(Item.NoteID).Enabled == true && NoteArray(Item.NoteID).ParentChannel == i)
                                {
                                    if (mvarOutputType == sotWave)
                                    {
                                        FSOUND_SetPan(NoteArray(Item.NoteID).FModChannel, Panning * 2);
                                    }
                                    else
                                    {
                                        // MIDISETPAN
                                        SetChnPan(NoteArray(Item.NoteID).FModChannel, Panning * 2);
                                    }
                                    // TODO: (NOT SUPPORTED): On Error Resume Next
                                    if (Recording) BufferEvent(Chr$(0xB0 + NoteArray(Item.NoteID).FModChannel) + Chr$(0xA) + Chr$(Panning * 2), this.TotalTicks);
                                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                                }
                            }
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xC0: 
           // Do Set Pitch Bend
          .PitchBend = EventQueue(ProgramCounter).Param1;
          .ProgramCounter = ProgramCounter + 1;
                            foreach (var iterItem in Notes)
                            {
                                Item = iterItem;
                                if (NoteArray(Item.NoteID).Enabled == true && NoteArray(Item.NoteID).ParentChannel == i)
                                {
                                    if (mvarOutputType == sotWave)
                                    {
                                        FSOUND_SetFrequency(NoteArray(Item.NoteID).FModChannel, NoteArray(Item.NoteID).Frequency * (2 ^ (1 / 12)) ^ (CSng(PitchBend - 0x40) / CSng(0x40) * CSng(PitchBendRange)));
                                    }
                                    else
                                    {
                                        // MIDIPITCHBEND
                                        // PitchWheel NoteArray(Item.NoteID).FModChannel, NoteArray(Item.NoteID).Frequency * (2 ^ (1 / 12)) ^ ((CSng(.PitchBend - &H40)) / CSng(&H40) * CSng(.PitchBendRange))
                                    }
                                }
                            }

                            break;
                        case 0xC1: 
           // Do Set Pitch Bend Range
          .PitchBendRange = SignedByteToInteger(EventQueue(ProgramCounter).Param1);
          .ProgramCounter = ProgramCounter + 1;
                            foreach (var iterItem in Notes)
                            {
                                Item = iterItem;
                                if (NoteArray(Item.NoteID).Enabled == true && NoteArray(Item.NoteID).ParentChannel == i)
                                {
                                    if (mvarOutputType == sotWave)
                                    {
                                        FSOUND_SetFrequency(NoteArray(Item.NoteID).FModChannel, NoteArray(Item.NoteID).Frequency * (2 ^ (1 / 12)) ^ (CSng(PitchBend - 0x40) / CSng(0x40) * CSng(PitchBendRange)));
                                    }
                                    else
                                    {
                                        // MIDIPITCHBENDRANGE
                                        // PitchWheel NoteArray(Item.NoteID).FModChannel, NoteArray(Item.NoteID).Frequency * (2 ^ (1 / 12)) ^ ((CSng(.PitchBend - &H40)) / CSng(&H40) * CSng(.PitchBendRange))
                                    }
                                }
                            }

                            break;
                        case 0xC2: 
           // Do Set Vibrato Depth
          .VibratoDepth = EventQueue(ProgramCounter).Param1;
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xC4: 
           // Do Set Vibrato Rate
          .VibratoRate = EventQueue(ProgramCounter).Param1;
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xCE: 
           // Do Set Sustain Off
          .Sustain = false;
                            foreach (var iterItem in Notes)
                            {
                                Item = iterItem;
                                if (NoteArray(Item.NoteID).Enabled == true && NoteArray(Item.NoteID).NoteOff == false)
                                { // And NoteArray(Item.NoteID).WaitTicks < 1 Then
                                    NoteArray(Item.NoteID).NoteOff = true;
                                }
                            }
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        case 0xB3: 
          .SubroutineCounter = SubroutineCounter + 1;
          .ReturnPointer = ProgramCounter + 1;
          .ProgramCounter = Subroutines(SubroutineCounter).EventQueuePointer;
          .InSubroutine = true;

                            break;
                        case 0xB4:
                            if (InSubroutine == true)
                            {
            .ProgramCounter = ReturnPointer;
            .InSubroutine = false;
                            }
                            else
                            {
            .ProgramCounter = ProgramCounter + 1;
                            }

                            break;
                        case 0xB2:
                            justlooped = true;
          .InSubroutine = false;
          .ProgramCounter = LoopPointer;

                            break;
                        default: /* TODO: Cannot Convert Expression Case: Is >= &HCF */
                            ll = SLen2Ticks(EventQueue(ProgramCounter).CommandByte - 0xCF) + 1;
                            if (EventQueue(ProgramCounter).CommandByte == 0xCF)
                            {
            .Sustain = true;
                                ll = 0;
                            }
                            nn = EventQueue(ProgramCounter).Param1;
                            vv = EventQueue(ProgramCounter).Param2;
                            uu = EventQueue(ProgramCounter).Param3;
                            NoteQueue.Add(true, 0, nn, 0, vv, i, uu, 0, 0, 0, 0, 0, ll, PatchNumber);
          .ProgramCounter = ProgramCounter + 1;

                            break;
                        default: /* TODO: Cannot Convert Expression Case: Is <= &HB0 */
                            if (justlooped == true)
                            {
                                justlooped = false;
                                if (i == 1) RaiseEvent(SongLoop);
            .WaitTicks = 0;
                            }
                            else
                            {
            .ProgramCounter = ProgramCounter + 1;
                                if (ProgramCounter > 1)
                                {
              .WaitTicks = EventQueue(ProgramCounter).Ticks - EventQueue(ProgramCounter - 1).Ticks;
                                }
                                else
                                {
              .WaitTicks = EventQueue(ProgramCounter).Ticks;
                                }
                            }

                            break;
                        default: 
          .ProgramCounter = ProgramCounter + 1;

                            break;
                    }

                    oo = true;
                }

                // TODO: (NOT SUPPORTED): End With
            }
            mmx = -1;

            if (SappyChannels.count > 0)
            {
                List<bool> clearedchannel = new();
                // TODO: (NOT SUPPORTED): ReDim clearedchannel(1 To SappyChannels.count)
                RaiseEvent(UpdateDisplay);
                foreach (var iterItem in NoteQueue)
                {
                    Item = iterItem;
                    x = FreeNote;
                    if (x < 32)
                    {
                        NoteArray(x) = Item;
                        // TODO: (NOT SUPPORTED): With SappyChannels(Item.ParentChannel)
                        if (clearedchannel[Item.ParentChannel] == false)
                        {
                            clearedchannel[Item.ParentChannel] = true;
                            foreach (var iteritem2 in Notes)
                            {
                                item2 = iteritem2;
                                if (NoteArray(item2.NoteID).Enabled == true && NoteArray(item2.NoteID).NoteOff == false)
                                {
                                    NoteArray(item2.NoteID).NoteOff = true;
                                }
                            }
                        }
      
      .Notes.Add(CByte(x), Str(x));
                        pat = Item.PatchNumber;
                        nn = Item.NoteNumber;

                        if (DirectExists(Directs, pat))
                        {
                            NoteArray(x).outputtype = Directs(Str(pat)).outputtype;
                            NoteArray(x).EnvAttenuation = Directs(Str(pat)).EnvAttenuation;
                            NoteArray(x).EnvDecay = Directs(Str(pat)).EnvDecay;
                            NoteArray(x).EnvSustain = Directs(Str(pat)).EnvSustain;
                            NoteArray(x).EnvRelease = Directs(Str(pat)).EnvRelease;
                            if (Directs(Str(pat)).outputtype == dotDirect || Directs(Str(pat)).outputtype == dotWave)
                            {
                                das = Str(Directs(Str(pat)).SampleID);
                                daf = NoteToFreq(nn + (60 - Directs(Str(pat)).DrumTuneKey), IIf(SamplePool(das).GBWave, -1, SamplePool(das).Frequency));
                                if (SamplePool(das).GBWave == true) daf = daf / 2;
                            }
                            else if (Directs(Str(pat)).outputtype == dotSquare1 || Directs(Str(pat)).outputtype == dotSquare2)
                            {
                                das = "square" + ((Directs(Str(pat)).GB1 % 4));
                                daf = NoteToFreq(nn + (60 - Directs(Str(pat)).DrumTuneKey));
                            }
                            else if (Directs(Str(pat)).outputtype == dotNoise)
                            {
                                das = "noise" + ((Directs(Str(pat)).GB1 % 2)) + Int(Rnd() * 3);
                                daf = NoteToFreq(nn + (60 - Directs(Str(pat)).DrumTuneKey));
                            }
                            else
                            {
                                das = "";
                            }

                        }
                        else if (InstrumentExists(pat))
                        {
                            NoteArray(x).outputtype = Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).outputtype;
                            NoteArray(x).EnvAttenuation = Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).EnvAttenuation;
                            NoteArray(x).EnvDecay = Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).EnvDecay;
                            NoteArray(x).EnvSustain = Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).EnvSustain;
                            NoteArray(x).EnvRelease = Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).EnvRelease;
                            if (Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).outputtype == dotDirect || Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).outputtype == dotWave)
                            {
                                das = Str(Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).SampleID);
                                if (Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).FixedPitch == true)
                                {
                                    daf = SamplePool(das).Frequency;
                                }
                                else
                                {
                                    daf = NoteToFreq(nn, IIf(SamplePool(das).GBWave, -2, SamplePool(das).Frequency));
                                }
                            }
                            else if (Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).outputtype == dotSquare1 || Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).outputtype == dotSquare2)
                            {
                                das = "square" + ((Instruments(Str(pat)).Directs(Str(Instruments(Str(pat)).KeyMaps(Str(nn)).AssignDirect)).GB1 % 4));
                                daf = NoteToFreq(nn);
                            }
                            else
                            {
                                das = "";
                            }

                        }
                        else if (DrumKitExists(pat))
                        {
                            NoteArray(x).outputtype = DrumKits(Str(pat)).Directs(Str(nn)).outputtype;
                            NoteArray(x).EnvAttenuation = DrumKits(Str(pat)).Directs(Str(nn)).EnvAttenuation;
                            NoteArray(x).EnvDecay = DrumKits(Str(pat)).Directs(Str(nn)).EnvDecay;
                            NoteArray(x).EnvSustain = DrumKits(Str(pat)).Directs(Str(nn)).EnvSustain;
                            NoteArray(x).EnvRelease = DrumKits(Str(pat)).Directs(Str(nn)).EnvRelease;
                            if (DrumKits(Str(pat)).Directs(Str(nn)).outputtype == dotDirect || DrumKits(Str(pat)).Directs(Str(nn)).outputtype == dotWave)
                            {
                                das = Str(DrumKits(Str(pat)).Directs(Str(nn)).SampleID);
                                if (DrumKits(Str(pat)).Directs(Str(nn)).FixedPitch == true && SamplePool(das).GBWave == false)
                                {
                                    daf = SamplePool(das).Frequency;
                                }
                                else
                                {
                                    daf = NoteToFreq(DrumKits(Str(pat)).Directs(Str(nn)).DrumTuneKey, IIf(SamplePool(das).GBWave, -2, SamplePool(das).Frequency));
                                }
                            }
                            else if (DrumKits(Str(pat)).Directs(Str(nn)).outputtype == dotSquare1 || DrumKits(Str(pat)).Directs(Str(nn)).outputtype == dotSquare2)
                            {
                                das = "square" + ((DrumKits(Str(pat)).Directs(Str(nn)).GB1 % 4));
                                daf = NoteToFreq(DrumKits(Str(pat)).Directs(Str(nn)).DrumTuneKey);
                            }
                            else if (DrumKits(Str(pat)).Directs(Str(nn)).outputtype == dotNoise)
                            {
                                das = "noise" + ((DrumKits(Str(pat)).Directs(Str(nn)).GB1 % 2)) + Int(Rnd() * 3);
                                daf = NoteToFreq(((DrumKits(Str(pat)).Directs(Str(nn)).DrumTuneKey)));
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
                            daf = daf * ((2 ^ (1 / 12)) ^ Me.Transpose);
                            dav = CSng(CSng(Item.Velocity) / CSng(0x7F) * (CSng(MainVolume) / CSng(0x7F)) * 255);
                            if (mutethis) dav = 0;

                            switch (NoteArray(x).outputtype)
                            {
                                case notSquare1:
                                    if (mvarGB1Chan < 32)
                                    {
                                        // TODO: (NOT SUPPORTED): With NoteArray(mvarGB1Chan)
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber));
                                            // PitchWheel NoteArray(Item.NoteID).FModChannel, 0
                                        }
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks);
              .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(mvarGB1Chan);
              // TODO: (NOT SUPPORTED): On Error GoTo 0
              .Enabled = false;
                                        // TODO: (NOT SUPPORTED): End With
                                    }
                                    mvarGB1Chan = x;
                                    break;
                                case notSquare2:
                                    if (mvarGB2Chan < 32)
                                    {
                                        // TODO: (NOT SUPPORTED): With NoteArray(mvarGB2Chan)
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber));
                                        }
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks);
              .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(mvarGB2Chan);
              // TODO: (NOT SUPPORTED): On Error GoTo 0
              .Enabled = false;
                                        // TODO: (NOT SUPPORTED): End With
                                    }
                                    mvarGB2Chan = x;
                                    break;
                                case notWave:
                                    if (mvarGB3Chan < 32)
                                    {
                                        // TODO: (NOT SUPPORTED): With NoteArray(mvarGB3Chan)
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(Item.ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber)); // I'm removing the Item. parts here... (Drag)
                                        }
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks); // ...and here (Drag)
              .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(mvarGB3Chan);
              // TODO: (NOT SUPPORTED): On Error GoTo 0
              .Enabled = false;
                                        // TODO: (NOT SUPPORTED): End With
                                    }
                                    mvarGB3Chan = x;
                                    break;
                                case notNoise:
                                    if (mvarGB4Chan < 32)
                                    {
                                        // TODO: (NOT SUPPORTED): With NoteArray(mvarGB4Chan)
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber));
                                        }
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks);
              .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(mvarGB4Chan);
              // TODO: (NOT SUPPORTED): On Error GoTo 0
              .Enabled = false;
                                        // TODO: (NOT SUPPORTED): End With
                                    }
                                    mvarGB4Chan = x;
                                    break;
                            }

                            if (mvarOutputType == sotWave)
                            {
                                if (mutethis == false)
                                {
                                    NoteArray(x).FModChannel = FSOUND_PlaySound(x + 1, SamplePool(das).FModSample);
                                }
                                else
                                {
                                    x = x;
                                }
                            }
                            else
                            {
                                NoteArray(x).FModChannel = Item.ParentChannel;
                            }
                            NoteArray(x).Frequency = daf;
                            NoteArray(x).Notephase = npInitial;
                            if (mvarOutputType == sotWave)
                            {
                                FSOUND_SetFrequency(NoteArray(x).FModChannel, CSng(daf) * (2 ^ (1 / 12)) ^ (CSng(PitchBend - 0x40) / CSng(0x40) * CSng(PitchBendRange)));
                                FSOUND_SetVolume(NoteArray(x).FModChannel, dav * IIf(mute, 0, 1));
                                FSOUND_SetPan(NoteArray(x).FModChannel, Panning * 2);
                            }
                            else
                            {
                                // MIDIPLAYSOUND
                                if (mute == false)
                                {
                                    // If Item.PatchNumber = 127 Then 'easy way
                                    if (DrumKitExists(Item.PatchNumber))
                                    { // better way
                                        ToneOn(9, MidiDrumMap(Item.NoteNumber), Item.Velocity);
                                    }
                                    else
                                    {
                                        ToneOn(Item.ParentChannel, Item.NoteNumber + MidiPatchTrans(Item.PatchNumber), Item.Velocity);
                                    }
                                }
                            }
                            if (Recording)
                            {
                                if (DrumKitExists(Item.PatchNumber))
                                { // better way
                                    BufferEvent(Chr$(0x99) + Chr$(MidiDrumMap(Item.NoteNumber)) + Chr$(Item.Velocity), this.TotalTicks);
                                }
                                else
                                {
                                    BufferEvent(Chr$(0x90 + Item.ParentChannel) + Chr$(Item.NoteNumber + MidiPatchTrans(Item.PatchNumber)) + Chr$(Item.Velocity), this.TotalTicks);
                                }
                            }
                            RaiseEvent(PlayedANote(Item.ParentChannel, Item.NoteNumber, Item.Velocity));
                        }
                        // TODO: (NOT SUPPORTED): End With
                    }
                }
            }
            NoteQueue.Clear();

            if (mvarNoteFrameCounter > 0)
            {
                for (i = 0; i <= 31; i += 1)
                {
                    if (NoteArray(i).Enabled == true)
                    {
                        // TODO: (NOT SUPPORTED): With NoteArray(i)
                        if (outputtype == notDirect)
                        {
                            if (NoteOff == true && Notephase < npRelease)
                            {
        .EnvStep = 0;
        .Notephase = npRelease;
                            }
                            if (EnvStep == 0 || EnvPosition == EnvDestination || EnvStep == 0 && EnvPosition <= EnvDestination || EnvStep >= 0 && EnvPosition >= EnvDestination)
                            {
                                switch (Notephase)
                                {
                                    case npInitial: 
            .Notephase = npAttack;
            .EnvPosition = 0;
            .EnvDestination = 255;
            .EnvStep = EnvAttenuation;
                                        break;
                                    case npAttack: 
            .Notephase = npDecay;
            .EnvDestination = EnvSustain;
            .EnvStep = (EnvDecay - 0x100) / 2;
                                        break;
                                    case npDecay: 
            .Notephase = npSustain;
            .EnvStep = 0;
                                        break;
                                    case npSustain: 
            .Notephase = npSustain;
            .EnvStep = 0;
                                        break;
                                    case npRelease: 
            .Notephase = npNoteOff;
            .EnvDestination = 0;
            .EnvStep = EnvRelease - 0x100;
                                        break;
                                    case npNoteOff:
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber));
                                            // Watch this: enabling this makes the file unreadable
                                            // BufferEvent Chr$(&H80 + .ParentChannel) & Chr$(.NoteNumber), Me.TotalTicks, .ParentChannel
                                        }
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks);
            .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(i);
            // TODO: (NOT SUPPORTED): On Error GoTo 0
            .Enabled = false;
                                        break;
                                }
                            }
                            // .EnvStep = .EnvStep * 1
                            nex = EnvPosition + EnvStep;
                            if (nex > EnvDestination && EnvStep > 0) nex = EnvDestination;
                            if (nex < EnvDestination && EnvStep < 0) nex = EnvDestination;
    .EnvPosition = nex;
                            dav = CSng(CSng(Velocity) / CSng(0x7F) * (CSng(SappyChannels(ParentChannel).MainVolume) / CSng(0x7F)) * (CSng(Int(EnvPosition)) / CSng(0xFF)) * 255);
                            if (mutethis) dav = 0;

                            if (mvarOutputType == sotWave)
                            {
                                FSOUND_SetVolume(FModChannel, dav * IIf(SappyChannels(ParentChannel).mute, 0, 1));
                            }
                            else
                            {
                                // MIDISETVOL
                                SetChnVolume(FModChannel, dav * IIf(SappyChannels(ParentChannel).mute, 0, 1));
                            }
                            // TODO: (NOT SUPPORTED): On Error Resume Next
                            if (Recording) BufferEvent(Chr$(0xD0 + FModChannel) + Chr$(dav), this.TotalTicks);
                            // TODO: (NOT SUPPORTED): On Error GoTo 0
                        }
                        else
                        {

                            // GB Envelope
                            if (NoteOff == true && Notephase < npRelease)
                            {
      .EnvStep = 0;
      .Notephase = npRelease;
                            }
                            if (EnvStep == 0 || EnvPosition == EnvDestination || EnvStep == 0 && EnvPosition <= EnvDestination || EnvStep >= 0 && EnvPosition >= EnvDestination)
                            {
                                switch (Notephase)
                                {
                                    case npInitial: 
          .Notephase = npAttack;
          .EnvPosition = 0;
          .EnvDestination = 255;
          .EnvStep = 0x100 - EnvAttenuation * 8;
                                        break;
                                    case npAttack: 
          .Notephase = npDecay;
          .EnvDestination = EnvSustain;
          .EnvStep = -EnvDecay * 2;
                                        break;
                                    case npDecay: 
          .Notephase = npSustain;
          .EnvStep = 0;
                                        break;
                                    case npSustain: 
          .Notephase = npSustain;
          .EnvStep = 0;
                                        break;
                                    case npRelease: 
          .Notephase = npNoteOff;
          .EnvDestination = 0;
          .EnvStep = (0x8 - EnvRelease) * 2;
                                        break;
                                    case npNoteOff:
                                        switch (outputtype)
                                        {
                                            case notSquare1:
                                                mvarGB1Chan = 255;
                                                break;
                                            case notSquare2:
                                                mvarGB2Chan = 255;
                                                break;
                                            case notWave:
                                                mvarGB3Chan = 255;
                                                break;
                                            case notNoise:
                                                mvarGB4Chan = 255;
                                                break;
                                        }
                                        if (mvarOutputType == sotWave)
                                        {
                                            FSOUND_StopSound(FModChannel);
                                        }
                                        else
                                        {
                                            // MIDISTOPSOUND
                                            ToneOff(ParentChannel, NoteNumber + MidiPatchTrans(PatchNumber));
                                        }
                                        // Watch this...
                                        if (Recording) BufferEvent(Chr$(0x80 + ParentChannel) + Chr$(NoteNumber) + Chr$(0), this.TotalTicks);
        .FModChannel = 0;
                                        // TODO: (NOT SUPPORTED): On Error Resume Next
                                        SappyChannels(.ParentChannel).Notes.Remove Str(i);
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        .Enabled = false;
                                        break;
                                }
                            }
                            // .EnvStep = .EnvStep * 1
                            nex = EnvPosition + EnvStep;
                            if (nex > EnvDestination && EnvStep > 0) nex = EnvDestination;
                            if (nex < EnvDestination && EnvStep < 0) nex = EnvDestination;
.EnvPosition = nex;

                            dav = CSng(CSng(Velocity) / CSng(0x7F) * (CSng(SappyChannels(ParentChannel).MainVolume) / CSng(0x7F)) * (CSng(Int(EnvPosition)) / CSng(0xFF)) * 255);
                            if (mutethis) dav = 0;
                            if (mvarOutputType == sotWave)
                            {
                                FSOUND_SetVolume(FModChannel, dav * IIf(SappyChannels(ParentChannel).mute, 0, 1));
                            }
                            else
                            {
                                // MIDISETVOL
                                SetChnVolume(FModChannel, dav * IIf(SappyChannels(ParentChannel).mute, 0, 1));
                            }
                            // TODO: (NOT SUPPORTED): On Error Resume Next
                            if (Recording) BufferEvent(Chr$(0xD0 + FModChannel) + Chr$(dav), this.TotalTicks);
                            // TODO: (NOT SUPPORTED): On Error GoTo 0
                        }
                        // TODO: (NOT SUPPORTED): End With
                    }
                }
            }

            xmmm = false;
            for (i = 1; i <= SappyChannels.count; i += 1)
            {
                if (SappyChannels(i).Enabled) xmmm = true;
            }
            if (xmmm == false || mvarTempo == 0)
            {
                StopSong();
                RaiseEvent(SongFinish);
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
                RaiseEvent(Beat(Beats));
            }
            incr = 0;
        }

        mvarNoteFrameCounter = 1; // (8 / (60000 / (mvarTempo * SappyPPQN)))

        if (mvarTempo != lasttempo)
        {
            lasttempo = mvarTempo;
            EventProcessor.Enabled = false;
            EventProcessor.EventType = etPeriodic;
            EventProcessor.Interval = 1;
            EventProcessor.Resolution = 1;
            EventProcessor.Enabled = true;
        }
        return;
    hell:;
        MsgBox("RTE " + Err().Number + " - " + Err().Description, vbCritical);
        StopSong();
    }

    private void NoteProcessor_Timer(int lMilliseconds)
    {
        EventProcessor.Enabled = false;
        EventProcessor.EventType = etPeriodic;
        EventProcessor.Interval = 60000 / (mvarTempo * SappyPPQN); // * dticks
        EventProcessor.Resolution = 1;
        EventProcessor.Enabled = true;
        return;
    hell:;
        MsgBox("runtime error: " + Err().Number + " / " + Err().Description, vbCritical);
        StopSong();
    }

    public bool DirectExists(ref SDirects DirectsCollection, byte DirectID)
    {
        bool _DirectExists = false;
        foreach (var iterItem in DirectsCollection)
        {
            Item = iterItem;
            if (Val(Item.Key) == DirectID)
            {
                _DirectExists = true;
                return _DirectExists;
            }
        }
        return _DirectExists;
    }
    public bool KeyMapExists(ref SKeyMaps KeyMapCollection, byte KeyMapID)
    {
        bool _KeyMapExists = false;
        foreach (var iterItem in KeyMapCollection)
        {
            Item = iterItem;
            if (Val(Item.Key) == KeyMapID)
            {
                _KeyMapExists = true;
                return _KeyMapExists;
            }
        }
        return _KeyMapExists;
    }
    public bool PatchExists(byte patch)
    {
        bool _PatchExists = false;
        foreach (var iterItem in Directs)
        {
            Item = iterItem;
            if (Val(Item.Key) == patch)
            {
                _PatchExists = true;
                return _PatchExists;
            }
        }
        foreach (var iterItem in Instruments)
        {
            Item = iterItem;
            if (Val(Item.Key) == patch)
            {
                _PatchExists = true;
                return _PatchExists;
            }
        }
        foreach (var iterItem in DrumKits)
        {
            Item = iterItem;
            if (Val(Item.Key) == patch)
            {
                _PatchExists = true;
                return _PatchExists;
            }
        }
        _PatchExists = false;
        return _PatchExists;
    }
    public bool InstrumentExists(byte patch)
    {
        bool _InstrumentExists = false;
        foreach (var iterItem in Instruments)
        {
            Item = iterItem;
            if (Val(Item.Key) == patch)
            {
                _InstrumentExists = true;
                return _InstrumentExists;
            }
        }
        _InstrumentExists = false;
        return _InstrumentExists;
    }
    public bool DrumKitExists(byte patch)
    {
        bool _DrumKitExists = false;
        foreach (var iterItem in DrumKits)
        {
            Item = iterItem;
            if (Val(Item.Key) == patch)
            {
                _DrumKitExists = true;
                return _DrumKitExists;
            }
        }
        _DrumKitExists = false;
        return _DrumKitExists;
    }
    public bool SampleExists(int SampleID)
    {
        bool _SampleExists = false;
        foreach (var iterItem in SamplePool)
        {
            Item = iterItem;
            if (Val(Item.Key) == SampleID)
            {
                _SampleExists = true;
                return _SampleExists;
            }
        }
        _SampleExists = false;
        return _SampleExists;
    }

    public byte FreeNote()
    {
        byte _FreeNote = 0;
        for (i = 0; i <= 31; i += 1)
        {
            if (NoteArray(i).Enabled == false)
            {
                _FreeNote = i;
                return _FreeNote;
            }
        }
        _FreeNote = 255;
        return _FreeNote;
    }

    private void WriteVarLen(int ch, int Value)
    {
        int buffer = 0;
        // This sets the most significant bits of the value wrong, so
        // I need to fix this. (Drag)
        // buffer = Value And &H7F
        // While Value \ 128 > 0
        // Value = Value \ 128
        // buffer = buffer * 256
        // buffer = buffer Or ((Value And &H7F) Or &H80)
        // Wend
        // The following is my code. (Drag)
        buffer = Value && 0x7F;
        while (Value / 128 > 0)
        {
            Value /= 128;
            buffer = buffer || 0x80;
            buffer = buffer * 256 || Value && 0x7F;
        }

        do
        {
            Put(#ch, , CByte(buffer && 255)); // : Pos = Pos + 1
if (buffer && 0x80)
            {
                buffer /= 256;
            }
            else
            {
                break;
            }
        }
}

    public int FlipLong(int Value)
    {
        int _FlipLong = 0;
        string s1 = "";
        string s2 = "";
        string b(4) = "";
        s1 = Right("00000000" + Hex(Value), 8);
        b(0) = Mid(s1, 1, 2);
        b(1) = Mid(s1, 3, 2);
        b(2) = Mid(s1, 5, 2);
        b(3) = Mid(s1, 7, 2);
        s2 = b(3) + b(2) + b(1) + b(0);
        _FlipLong = Val("&H" + s2);
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
        return _FlipLong;
    }

    public int FlipInt(int Value)
    {
        int _FlipInt = 0;
        byte b1 = 0;
        byte b2 = 0;
        b1 = Value % &H100;
        Value /= 0x100;
        b2 = Value % &H100;

        Value = b1;
        Value *= 0x100;
        Value += b2;

        _FlipInt = Value;
        return _FlipInt;
    }

    private void GetSample(ref SDirect D, ref SappyDirectHeader dirhead, ref SappySampleHeader smphead, ref bool UseReadString)
    {
        // TODO: (NOT SUPPORTED): With D
        Console.WriteLine("GetSample -> 0x" + Hex(dirhead.pSampleHeader) + " (" + IIf(UseReadString, "readstring", "seek") + ")");
.SampleID = dirhead.pSampleHeader;
        sid = SampleID;
        if (SampleExists(sid) == false)
        {
            SamplePool.Add(Str(sid));
            if (outputtype == dotDirect)
            {
                smphead = ReadSampleHead(1, GBAROMPointerToOffset(sid));
// TODO: (NOT SUPPORTED): With SamplePool(Str(sid))
.Size = smphead.wSize;
.Frequency = smphead.wFreq * 64;
.loopstart = smphead.wLoop;
.LoopEnable = smphead.flags > 0;
.GBWave = false;
                if (UseReadString)
                {
.SampleData = ReadString(1, Size);
                }
                else
                {
.SampleData = Seek(1);
                }
                // TODO: (NOT SUPPORTED): End With
            }
            else
            {
// TODO: (NOT SUPPORTED): With SamplePool(Str(sid))
.Size = 32;
.Frequency = GBWaveBaseFreq;
.loopstart = 0;
.LoopEnable = true;
.GBWave = true;
                tsi = ReadString(1, 16, GBAROMPointerToOffset(sid));
.SampleData = "";
                for (ai = 0; ai <= 31; ai += 1)
                {
                    bi = ai % 2;
.SampleData = SampleData + ChrB(Int(IIf(Mid(tsi, ai / 2 + 1, 1) == "", 0, Asc(Mid(tsi, ai / 2 + 1, 1))) / (16 ^ bi) % 16 * (GBWaveMulti * 16)));
                }
                // TODO: (NOT SUPPORTED): End With
            }
        }
        // TODO: (NOT SUPPORTED): End With
    }

    private void GetSampleWithMulti(ref SDirect D, ref SappyDirectHeader dirhead, ref SappySampleHeader smphead, ref bool UseReadString)
    {
        // TODO: (NOT SUPPORTED): With D
        Console.WriteLine("GetSample -> 0x" + Hex(dirhead.pSampleHeader) + " (" + IIf(UseReadString, "readstring", "seek") + ", multi)");
.SampleID = dirhead.pSampleHeader;
        sid = SampleID;
        if (SampleExists(sid) == false)
        {
            SamplePool.Add(Str(sid));
            if (outputtype == dotDirect)
            {
                smphead = ReadSampleHead(1, GBAROMPointerToOffset(sid));
// TODO: (NOT SUPPORTED): With SamplePool(Str(sid))
.Size = smphead.wSize;
.Frequency = smphead.wFreq * CLng(64);
.loopstart = smphead.wLoop;
.LoopEnable = smphead.flags > 0;
.GBWave = false;
                if (UseReadString)
                {
.SampleData = ReadString(1, Size);
                }
                else
                {
.SampleData = Seek(1);
                }
                // TODO: (NOT SUPPORTED): End With
            }
            else
            {
// TODO: (NOT SUPPORTED): With SamplePool(Str(sid))
.Size = 32;
.Frequency = GBWaveBaseFreq;
.loopstart = 0;
.LoopEnable = true;
.GBWave = true;
                tsi = ReadString(1, 16, GBAROMPointerToOffset(sid));
.SampleData = "";
                for (ai = 0; ai <= 31; ai += 1)
                {
                    bi = ai % 2;
.SampleData = SampleData + ChrB(Int(IIf(Mid(tsi, ai / 2 + 1, 1) == "", 0, Asc(Mid(tsi, ai / 2 + 1, 1))) / (16 ^ bi) % 16 * (GBWaveMulti * 16)));
                }
                // TODO: (NOT SUPPORTED): End With
            }
        }
        // TODO: (NOT SUPPORTED): End With
    }

    private void SetStuff(ref SDirect foo, ref SappyInstrumentHeader inshead, ref SappyDirectHeader dirhead, ref SappyNoiseHeader gbhead)
    {
// TODO: (NOT SUPPORTED): With foo
.DrumTuneKey = inshead.bDrumPitch;
.outputtype = inshead.bChannel && 7;
.EnvAttenuation = dirhead.bAttack;
.EnvDecay = dirhead.bHold;
.EnvSustain = dirhead.bSustain;
.EnvRelease = dirhead.bRelease;
.Raw0 = dirhead.b0;
.Raw1 = dirhead.b1;
.GB1 = gbhead.b2;
.GB2 = gbhead.b3;
.GB3 = gbhead.b4;
.GB4 = gbhead.b5;
.FixedPitch = (inshead.bChannel && 0x8) == 0x8 ? true : false;
.Reverse = (inshead.bChannel && 0x10) == 0x10 ? true : false;
        // TODO: (NOT SUPPORTED): End With
    }


}
