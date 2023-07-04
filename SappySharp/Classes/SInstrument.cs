namespace SappySharp.Classes;

public class SInstrument
{
    public string Key = "";

    SKeyMaps mvarKeyMaps = null; // local copy
    SDirects mvarDirects = null; // local copy

    public SDirects Directs { get => mvarDirects; set => mvarDirects = value; }

    public SKeyMaps KeyMaps
    {
        get => mvarKeyMaps;
        set => mvarKeyMaps = value;
    }
}
