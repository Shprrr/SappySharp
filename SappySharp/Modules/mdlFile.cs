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

static class mdlFile
{
    public static string[] VirtualFile = new string[255]; // Range 256 To 511
    public static int[] WriteLastOffset = new int[511];
    public static int[] ReadLastOffset = new int[511];
    public const int VF = 256;

    public static void FreeFileNumber()
    {
        FreeFile();
    }

    public static void OpenFile(int filenumber, string Filename)
    {
        if (filenumber < VF) FileOpen(filenumber, Filename, OpenMode.Binary);
        WriteOffset(filenumber, 0);
        ReadOffset(filenumber, 0);
    }

    public static void OpenNewFile(int filenumber, string Filename)
    {
        if (filenumber < VF)
        {
            FileOpen(filenumber, Filename, OpenMode.Output);
            Print(filenumber, "x");
            FileClose(filenumber);
        }
        else
        {
            VirtualFile[filenumber - VF] = "";
        }
        OpenFile(filenumber, Filename);
    }

    public static void CloseFile(int filenumber)
    {
        FileClose(filenumber);
    }

    public static int WriteOffset(int filenumber, int offset = -1)
    {
        WriteLastOffset[filenumber] = offset > -1 ? offset : WriteLastOffset[filenumber];
        return WriteLastOffset[filenumber];
    }

    public static void WriteByte(int filenumber, byte Data, int offset = -1)
    {
        WriteOffset(filenumber, offset);
        FilePutObject(filenumber, Data, WriteOffset(filenumber) + 1);
        WriteOffset(filenumber, WriteOffset(filenumber) + 1);
    }

    public static void WriteBigEndian(int filenumber, byte Width, int Data, int offset = -1)
    {
        WriteOffset(filenumber, offset);
        for (byte i = 0; i <= Width - 1; i += 1)
        {
            WriteByte(filenumber, (byte)(Data / (16 ^ i) % 256));
        }
    }

    public static void WriteLittleEndian(int filenumber, byte Width, int Data, int offset = -1)
    {
        WriteOffset(filenumber, offset);
        for (int i = Width - 1; i >= 0; i--)
        {
            WriteByte(filenumber, (byte)(Data / (16 ^ i) % 256));
        }
    }

    public static void WriteString(int filenumber, string Data, int offset = -1)
    {
        // Dim i As Long
        WriteOffset(filenumber, offset);
        // For i = 1 To Len(Data)
        // WriteByte FileNumber, IIf(Mid(Data, i, 1) = Chr(0), 0, Asc(Mid(Data, i, 1)))
        // Next i
        FilePutObject(filenumber, Data, WriteOffset(filenumber) + 1);
        WriteOffset(filenumber, WriteOffset(filenumber) + Len(Data));
    }

    public static int ReadOffset(int filenumber, int offset = -1)
    {
        ReadLastOffset[filenumber] = offset > -1 ? offset : ReadLastOffset[filenumber];
        return ReadLastOffset[filenumber];
    }

    public static byte ReadByte(int filenumber, int offset = -1)
    {
        byte _ReadByte = 0;
        ReadOffset(filenumber, offset);
        FileGet(filenumber, ref _ReadByte, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, ReadOffset(filenumber) + 1);
        return _ReadByte;
    }

    public static int ReadVLQ(int filenumber, int offset = -1)
    {
        int _ReadVLQ = 0;
        ReadOffset(filenumber, offset);
        byte a;
        byte retlen = 0;
        do
        {
            a = ReadByte(filenumber);
            _ReadVLQ *= (2 ^ 7);
            _ReadVLQ += a % 0x80;
            retlen = (byte)(retlen + 1);
        } while (!(retlen == 4 || a < 0x80));

        return _ReadVLQ;
    }
    public static int ReadBigEndian(int filenumber, byte Width, int offset = -1)
    {
        int _ReadBigEndian = 0;
        ReadOffset(filenumber, offset);
        for (int i = Width - 1; i >= 0; i--)
        {
            _ReadBigEndian += ReadByte(filenumber) * (256 ^ i);
        }
        return _ReadBigEndian;
    }

    public static int ReadLittleEndian(int filenumber, byte Width, int offset = -1)
    {
        int _ReadLittleEndian = 0;
        ReadOffset(filenumber, offset);
        for (int i = 0; i <= Width - 1; i += 1)
        {
            _ReadLittleEndian += ReadByte(filenumber) * (256 ^ i);
        }
        return _ReadLittleEndian;
    }

    public static string ReadString(int filenumber, int Length, int offset = -1)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo fuck
        string _ReadString = new(' ', Length);
        // Dim i As Long
        ReadOffset(filenumber, offset);
        // For i = 1 To Length
        // ReadString = ReadString & Chr(ReadByte(FileNumber))
        // Next i
        FileGet(filenumber, ref _ReadString, ReadOffset(filenumber) + 1);
        ReadOffset(filenumber, ReadOffset(filenumber) + Length);
        return _ReadString;
    fuck:;
        Trace("fuck");
        return _ReadString;
    }

    public static int ReadGBAROMPointer(int filenumber, int offset = -1)
    {
        ReadOffset(filenumber, offset);
        int _ReadGBAROMPointer = ReadLittleEndian(filenumber, 4);
        if (_ReadGBAROMPointer < 0x8000000 || _ReadGBAROMPointer > 0x9FFFFFF)
        {
            _ReadGBAROMPointer = -1;
        }
        else
        {
            _ReadGBAROMPointer -= 0x8000000;
        }
        return _ReadGBAROMPointer;
    }

    public static int GBAROMPointerToOffset(int GBAROMPointer)
    {
        int _GBAROMPointerToOffset;
        if (GBAROMPointer < 0x8000000 || GBAROMPointer > 0x9FFFFFF)
        {
            _GBAROMPointerToOffset = -1;
        }
        else
        {
            _GBAROMPointerToOffset = GBAROMPointer - 0x8000000;
        }
        return _GBAROMPointerToOffset;
    }
}
