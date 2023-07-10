using System;
using static mdlFile;
using static Microsoft.VisualBasic.FileSystem;

static class SapPlayer
{
    public static string[,] NoiseWaves = new string[2, 10];
    public static bool Details = false;
    public struct SappyInstrumentHeader
    {
        public byte bChannel;
        public byte bDrumPitch;
    }
    public struct SappyDirectHeader
    {
        public byte b0;
        public byte b1;
        public int pSampleHeader;
        public byte bAttack;
        public byte bHold;
        public byte bSustain;
        public byte bRelease;
    }
    public struct SappyNoiseHeader
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
    public struct SappyDrumKitHeader
    {
        public byte b0;
        public byte b1;
        public int pDirectTable;
        public byte b6;
        public byte b7;
        public byte b8;
        public byte b9;
    }
    public struct SappyMultiHeader
    {
        public byte b0;
        public byte b1;
        public int pDirectTable;
        public int pKeyMap;
    }
    public struct SappySampleHeader
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


    public static SappyInstrumentHeader ReadInstrumentHead(int filenumber, int offset = -1)
    {
        ValueType _ReadInstrumentHead = new SappyInstrumentHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadInstrumentHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyInstrumentHeader)_ReadInstrumentHead;
    }
    public static SappyDirectHeader ReadDirectHead(int filenumber, int offset = -1)
    {
        ValueType _ReadDirectHead = new SappyDirectHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadDirectHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyDirectHeader)_ReadDirectHead;
    }
    public static SappyDrumKitHeader ReadDrumKitHead(int filenumber, int offset = -1)
    {
        ValueType _ReadDrumKitHead = new SappyDrumKitHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadDrumKitHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyDrumKitHeader)_ReadDrumKitHead;
    }
    public static SappyMultiHeader ReadMultiHead(int filenumber, int offset = -1)
    {
        ValueType _ReadMultiHead = new SappyMultiHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadMultiHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyMultiHeader)_ReadMultiHead;
    }
    public static SappyNoiseHeader ReadNoiseHead(int filenumber, int offset = -1)
    {
        ValueType _ReadNoiseHead = new SappyNoiseHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadNoiseHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappyNoiseHeader)_ReadNoiseHead;
    }
    public static SappySampleHeader ReadSampleHead(int filenumber, int offset = -1)
    {
        ValueType _ReadSampleHead = new SappySampleHeader();
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadSampleHead, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, (int)(Seek(filenumber) - 1));
        return (SappySampleHeader)_ReadSampleHead;
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
        double magic = Math.Pow(2, 1d / 12);
        int X = MIDINote - 0x3C;
        double c;
        if (MidCFreq == -1)
        {
            int a = 7040;
            c = a * Math.Pow(magic, 3);
        }
        else if (MidCFreq == -2)
        {
            int a = 7040 / 2;
            c = a * Math.Pow(magic, 3);
        }
        else
        {
            c = MidCFreq;
        }
        return (int)(c * Math.Pow(magic, X));
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
