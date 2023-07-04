namespace SappySharp.Classes;

public class SDrumKit
{
    public string Key = "";

    SDirects mvarDirects = null; // local copy
    public SDirects Directs
    {
        get => mvarDirects;
        set => mvarDirects = value;
    }
}
