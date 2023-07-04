namespace SappySharp.Classes;

public class SKeyMap
{
    public string Key = "";

    int mvarAssignDirect = 0; // local copy
    public int AssignDirect
    {
        get => mvarAssignDirect;
        set => mvarAssignDirect = value;
    }
}
