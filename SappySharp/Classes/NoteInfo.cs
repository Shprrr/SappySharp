namespace SappySharp.Classes;

public class NoteInfo
{
    bool mvarEnabled = false;
    int mvarFModChannel = 0;
    byte mvarNoteNumber = 0;
    int mvarFrequency = 0;
    byte mvarVelocity = 0;
    byte mvarPatchNumber = 0;
    int mvarParentChannel = 0;
    string mvarSampleID = "";
    byte mvarUnknownValue = 0;
    public enum NoteOutputTypes
    {
        notDirect = 0
    , notSquare1 = 1
    , notSquare2 = 2
    , notWave = 3
    , notNoise = 4
    , notUnk5 = 5
    , notUnk6 = 6
    , notUnk7 = 7
    }
    public enum NotePhases
    {
        npInitial = 0
    , npAttack = 1
    , npDecay = 2
    , npSustain = 3
    , npRelease = 4
    , npNoteOff = 5
    }
    bool mvarNoteOff = false;
    NotePhases mvarNotePhase;
    NoteOutputTypes mvarOutputType;
    decimal mvarEnvStep = 0;
    decimal mvarEnvDestination = 0;
    decimal mvarEnvPosition = 0;
    byte mvarEnvAttenuation = 0;
    byte mvarEnvDecay = 0;
    byte mvarEnvSustain = 0;
    byte mvarEnvRelease = 0;
    decimal mvarWaitTicks = 0;
    string mvarKey = "";

    public decimal EnvPosition
    {
        get => mvarEnvPosition;
        set => mvarEnvPosition = value;
    }

    public decimal EnvDestination
    {
        get => mvarEnvDestination;
        set => mvarEnvDestination = value;
    }

    public decimal EnvStep
    {
        get => mvarEnvStep;
        set => mvarEnvStep = value;
    }

    public NotePhases Notephase
    {
        get => mvarNotePhase;
        set => mvarNotePhase = value;
    }

    public string Key { get => mvarKey; set => mvarKey = value; }

    public decimal WaitTicks
    {
        get => mvarWaitTicks;
        set => mvarWaitTicks = value;
    }

    public byte EnvRelease
    {
        get => mvarEnvRelease;
        set => mvarEnvRelease = value;
    }

    public byte PatchNumber
    {
        get => mvarPatchNumber;
        set => mvarPatchNumber = value;
    }

    public string SampleID
    {
        //get => mvarEnvRelease; // Bug ?
        get => mvarSampleID;
        set => mvarSampleID = value;
    }

    public byte EnvSustain
    {
        get => mvarEnvSustain;
        set => mvarEnvSustain = value;
    }

    public byte EnvDecay
    {
        get => mvarEnvDecay;
        set => mvarEnvDecay = value;
    }

    public byte EnvAttenuation
    {
        get => mvarEnvAttenuation;
        set => mvarEnvAttenuation = value;
    }

    public NoteOutputTypes outputtype
    {
        get => mvarOutputType;
        set => mvarOutputType = value;
    }

    public byte UnknownValue
    {
        get => mvarUnknownValue;
        set => mvarUnknownValue = value;
    }

    public int ParentChannel
    {
        get => mvarParentChannel;
        set => mvarParentChannel = value;
    }

    public byte Velocity
    {
        get => mvarVelocity;
        set => mvarVelocity = value;
    }

    public int Frequency
    {
        get => mvarFrequency;
        set => mvarFrequency = value;
    }

    public byte NoteNumber
    {
        get => mvarNoteNumber;
        set => mvarNoteNumber = value;
    }

    public int FModChannel
    {
        get => mvarFModChannel;
        set => mvarFModChannel = value;
    }

    public bool Enabled
    {
        get => mvarEnabled;
        set => mvarEnabled = value;
    }

    public bool NoteOff
    {
        get => mvarNoteOff;
        set => mvarNoteOff = value;
    }
}
