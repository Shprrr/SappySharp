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

static class SapPlayer
{
    public static string[,] NoiseWaves = new string[2, 10];
    public static bool Details = false;
    public class SappyMasterTableEntry
    {
        public int pSong;
        public int wPriority1;
        public int wPriority2;
    }
    public class SappySongHeader
    {
        public byte wTracks; // Integer
        public byte wBlocks;
        // w1 As Integer
        public byte wPriority;
        public byte wReverb;
        public int pInstrumentBank;
    }
    public class SappyInstrumentHeader
    {
        public byte bChannel;
        public byte bDrumPitch;
    }
    public class SappyDirectHeader
    {
        public byte b0;
        public byte b1;
        public int pSampleHeader;
        public byte bAttack;
        public byte bHold;
        public byte bSustain;
        public byte bRelease;
    }
    public class SappySquare1Header
    {
        public byte bRaw1;
        public byte bRaw2;
        public byte bDutyCycle;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte bAttack;
        public byte bDecay;
        public byte bSustain;
        public byte bRelease;
    }
    public class SappySquare2Header
    {
        public byte b0;
        public byte b1;
        public byte bDutyCycle;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte bAttack;
        public byte bDecay;
        public byte bSustain;
        public byte bRelease;
    }
    public class SappyWaveHeader
    {
        public byte b0;
        public byte b1;
        public int pSample;
        public byte bAttack;
        public byte bDecay;
        public byte bSustain;
        public byte bRelease;
    }
    public class SappyNoiseHeader
    {
        public byte b0;
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte bAttack;
        public byte bDecay;
        public byte bSustain;
        public byte bRelease;
    }
    public class SappyInvalidHeader
    {
        public byte b0;
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte b6;
        public byte b7;
        public byte b8;
        public byte b9;
    }
    public class SappyDrumKitHeader
    {
        public byte b0;
        public byte b1;
        public int pDirectTable;
        public byte b6;
        public byte b7;
        public byte b8;
        public byte b9;
    }
    public class SappyMultiHeader
    {
        public byte b0;
        public byte b1;
        public int pDirectTable;
        public int pKeyMap;
    }
    public class SappySampleHeader
    {
        public int flags;
        public byte b4;
        public byte FineTune;
        public int wFreq;
        public int wLoop;
        public int wSize;
    }


    // Channel Byte

    // Special Instrument Bits (Override all others)
    // 6 - Multi-sample Instrument
    // 7 - Drum Kit

    // Bits 0-2 assign a channel:
    // 0 - Direct
    // 1 - Square 1
    // 2 - Square 2
    // 3 - Wave
    // 4 - Noise
    // 5-7 - None

    // Direct Channel Bits:
    // 3 - Fixed Pitch (Plays Sample at Middle C frequency)
    // 4 - Reverse (Reverses Sample)
    // 5 - ??


    public static SappySongHeader ReadSongHead(int filenumber, int offset = -1)
    {
        Array _ReadSongHead = new SappySongHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadSongHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappySongHeader)_ReadSongHead.GetValue(0);
    }
    public static SappyInstrumentHeader ReadInstrumentHead(int filenumber, int offset = -1)
    {
        Array _ReadInstrumentHead = new SappyInstrumentHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadInstrumentHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyInstrumentHeader)_ReadInstrumentHead.GetValue(0);
    }
    public static SappyDirectHeader ReadDirectHead(int filenumber, int offset = -1)
    {
        Array _ReadDirectHead = new SappyDirectHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadDirectHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyDirectHeader)_ReadDirectHead.GetValue(0);
    }
    public static SappyDrumKitHeader ReadDrumKitHead(int filenumber, int offset = -1)
    {
        Array _ReadDrumKitHead = new SappyDrumKitHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadDrumKitHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyDrumKitHeader)_ReadDrumKitHead.GetValue(0);
    }
    public static SappyMultiHeader ReadMultiHead(int filenumber, int offset = -1)
    {
        Array _ReadMultiHead = new SappyMultiHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadMultiHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyMultiHeader)_ReadMultiHead.GetValue(0);
    }
    public static SappySquare1Header ReadSquare1Head(int filenumber, int offset = -1)
    {
        Array _ReadSquare1Head = new SappySquare1Header[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadSquare1Head, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappySquare1Header)_ReadSquare1Head.GetValue(0);
    }
    public static SappySquare1Header ReadSquare2Head(int filenumber, int offset = -1)
    {
        Array _ReadSquare2Head = new SappySquare1Header[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadSquare2Head, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappySquare1Header)_ReadSquare2Head.GetValue(0);
    }
    public static SappyWaveHeader ReadWaveHead(int filenumber, int offset = -1)
    {
        Array _ReadWaveHead = new SappyWaveHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadWaveHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyWaveHeader)_ReadWaveHead.GetValue(0);
    }
    public static SappyNoiseHeader ReadNoiseHead(int filenumber, int offset = -1)
    {
        Array _ReadNoiseHead = new SappyNoiseHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadNoiseHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyNoiseHeader)_ReadNoiseHead.GetValue(0);
    }
    public static SappyInvalidHeader ReadInvalidHead(int filenumber, int offset = -1)
    {
        Array _ReadInvalidHead = new SappyInvalidHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadInvalidHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyInvalidHeader)_ReadInvalidHead.GetValue(0);
    }
    public static SappySampleHeader ReadSampleHead(int filenumber, int offset = -1)
    {
        Array _ReadSampleHead = new SappySampleHeader[1];
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadSampleHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappySampleHeader)_ReadSampleHead.GetValue(0);
    }

    public static int SignedByteToInteger(byte SignedByte) => SignedByte > 0x7F ? SignedByte - 0x100 : SignedByte;

    public static string NoteToName(byte MIDINote)
    {
        string _NoteToName = "";
        int X = MIDINote % 12;
        int o = MIDINote / 12;
        switch (X)
        {
            case 0:
                _NoteToName = "C";
                break;
            case 1:
                _NoteToName = "C#";
                break;
            case 2:
                _NoteToName = "D";
                break;
            case 3:
                _NoteToName = "D#";
                break;
            case 4:
                _NoteToName = "E";
                break;
            case 5:
                _NoteToName = "F";
                break;
            case 6:
                _NoteToName = "F#";
                break;
            case 7:
                _NoteToName = "G";
                break;
            case 8:
                _NoteToName = "G#";
                break;
            case 9:
                _NoteToName = "A";
                break;
            case 10:
                _NoteToName = "A#";
                break;
            case 11:
                _NoteToName = "B";
                break;
        }
        _NoteToName += o;
        return _NoteToName;
    }
    public static int NoteToFreq(int MIDINote, int MidCFreq = -1)
    {
        int magic = 2 ^ (1 / 12);
        int X = MIDINote - 0x3C;
        int c;
        if (MidCFreq == -1)
        {
            int a = 7040;
            c = a * (magic ^ 3);
        }
        else if (MidCFreq == -2)
        {
            int a = 7040 / 2;
            c = a * (magic ^ 3);
        }
        else
        {
            c = MidCFreq;
        }
        return c * (magic ^ X);
    }
    public static int SLen2Ticks(byte ShortLen)
    {
        int _SLen2Ticks = 0;
        switch (ShortLen)
        {
            case 0x0:
                _SLen2Ticks = 0x0;
                break;
            case 0x1:
                _SLen2Ticks = 0x1;
                break;
            case 0x2:
                _SLen2Ticks = 0x2;
                break;
            case 0x3:
                _SLen2Ticks = 0x3;
                break;
            case 0x4:
                _SLen2Ticks = 0x4;
                break;
            case 0x5:
                _SLen2Ticks = 0x5;
                break;
            case 0x6:
                _SLen2Ticks = 0x6;
                break;
            case 0x7:
                _SLen2Ticks = 0x7;
                break;
            case 0x8:
                _SLen2Ticks = 0x8;
                break;
            case 0x9:
                _SLen2Ticks = 0x9;
                break;
            case 0xA:
                _SLen2Ticks = 0xA;
                break;
            case 0xB:
                _SLen2Ticks = 0xB;
                break;
            case 0xC:
                _SLen2Ticks = 0xC;
                break;
            case 0xD:
                _SLen2Ticks = 0xD;
                break;
            case 0xE:
                _SLen2Ticks = 0xE;
                break;
            case 0xF:
                _SLen2Ticks = 0xF;
                break;
            case 0x10:
                _SLen2Ticks = 0x10;
                break;
            case 0x11:
                _SLen2Ticks = 0x11;
                break;
            case 0x12:
                _SLen2Ticks = 0x12;
                break;
            case 0x13:
                _SLen2Ticks = 0x13;
                break;
            case 0x14:
                _SLen2Ticks = 0x14;
                break;
            case 0x15:
                _SLen2Ticks = 0x15;
                break;
            case 0x16:
                _SLen2Ticks = 0x16;
                break;
            case 0x17:
                _SLen2Ticks = 0x17;
                break;
            case 0x18:
                _SLen2Ticks = 0x18;
                break;
            case 0x19:
                _SLen2Ticks = 0x1C;
                break;
            case 0x1A:
                _SLen2Ticks = 0x1E;
                break;
            case 0x1B:
                _SLen2Ticks = 0x20;
                break;
            case 0x1C:
                _SLen2Ticks = 0x24;
                break;
            case 0x1D:
                _SLen2Ticks = 0x28;
                break;
            case 0x1E:
                _SLen2Ticks = 0x2A;
                break;
            case 0x1F:
                _SLen2Ticks = 0x2C;
                break;
            case 0x20:
                _SLen2Ticks = 0x30;
                break;
            case 0x21:
                _SLen2Ticks = 0x34;
                break;
            case 0x22:
                _SLen2Ticks = 0x36;
                break;
            case 0x23:
                _SLen2Ticks = 0x38;
                break;
            case 0x24:
                _SLen2Ticks = 0x3C;
                break;
            case 0x25:
                _SLen2Ticks = 0x40;
                break;
            case 0x26:
                _SLen2Ticks = 0x42;
                break;
            case 0x27:
                _SLen2Ticks = 0x44;
                break;
            case 0x28:
                _SLen2Ticks = 0x48;
                break;
            case 0x29:
                _SLen2Ticks = 0x4C;
                break;
            case 0x2A:
                _SLen2Ticks = 0x4E;
                break;
            case 0x2B:
                _SLen2Ticks = 0x50;
                break;
            case 0x2C:
                _SLen2Ticks = 0x54;
                break;
            case 0x2D:
                _SLen2Ticks = 0x58;
                break;
            case 0x2E:
                _SLen2Ticks = 0x5A;
                break;
            case 0x2F:
                _SLen2Ticks = 0x5C;
                break;
            case 0x30:
                _SLen2Ticks = 0x60;
                break;
        }
        return _SLen2Ticks;
    }
}
