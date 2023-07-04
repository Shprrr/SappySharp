namespace SappySharp.Classes;

public class SSubroutine
{
    /// <summary>
    /// Subroutine ID
    /// </summary>
    public string Key = "";

    int mvarEventQueuePointer = 0; // local copy
    /// <summary>
    /// Pointer to the subroutine in the event queue
    /// </summary>
    public int EventQueuePointer
    {
        get => mvarEventQueuePointer;
        set => mvarEventQueuePointer = value;
    }
}
