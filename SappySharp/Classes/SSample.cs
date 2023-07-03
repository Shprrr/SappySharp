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

namespace SappySharp.Classes;

public class SSample
{
    public string Key = "";

    // local variable(s) to hold property value(s)
    string mvarSampleData = ""; // local copy
    int mvarLoopStart = 0; // local copy
    bool mvarLoopEnable = false; // local copy
    int mvarSize = 0; // local copy
    int mvarFrequency = 0; // local copy
    int mvarFModSample = 0; // local copy
    // local variable(s) to hold property value(s)
    bool mvarGBWave = false; // local copy

    byte[] mvarSampleDataB = Array.Empty<byte>();
    int mvarSampleDataL = 0;
    public byte[] SampleDataB => mvarSampleDataB;


    public void ReadSampleDataFromFile(int fn, int tsize)
    {
        Array sampleData =new byte[tsize];
        mvarSampleDataL = tsize;
        int filenumber = 0;
        FileGet(fn, ref sampleData, ReadOffset(filenumber) + 1);
        mvarSampleDataB = (byte[])sampleData;
    }
    public void SaveSampleDataToFile(int fn)
    {
        int filenumber = 0;
        FilePutObject(fn, mvarSampleDataB, WriteOffset(filenumber) + 1);
    }

    public bool GBWave
    {
        get => mvarGBWave;
        set => mvarGBWave = value;
    }

    public bool LoopEnable
    {
        get => mvarLoopEnable;
        set => mvarLoopEnable = value;
    }

    public int FModSample
    {
        get => mvarFModSample;
        set => mvarFModSample = value;
    }

    public int Frequency { get => mvarFrequency; set => mvarFrequency = value; }

    public int Size { get => mvarSize; set => mvarSize = value; }

    public int loopstart
    {
        get => mvarLoopStart;
        set => mvarLoopStart = value;
    }

    public string SampleData { get => mvarSampleData; set => mvarSampleData = value; }
}
