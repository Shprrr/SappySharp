using System;
using static mdlFile;
using static Microsoft.VisualBasic.FileSystem;

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
        Array sampleData = new byte[tsize];
        mvarSampleDataL = tsize;
        int filenumber = 0;
        FileGet(fn, ref sampleData, ReadOffset(filenumber) + 1);
        mvarSampleDataB = (byte[])sampleData;
    }
    public void SaveSampleDataToFile(int fn)
    {
        int filenumber = 0;
        FilePut(fn, mvarSampleDataB, WriteOffset(filenumber) + 1);
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
