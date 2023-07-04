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
