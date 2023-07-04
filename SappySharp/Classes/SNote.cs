namespace SappySharp.Classes;

public class SNote
{
    /// <summary>
    /// Note ID from Channel perspective
    /// </summary>
   public string Key = "";

    // local variable(s) to hold property value(s)
    byte mvarNoteID = 0; // local copy
    // local variable(s) to hold property value(s)

    /// <summary>
    /// Note ID in polyphony list
    /// </summary>
    public byte NoteID
    {
        get => mvarNoteID;
        set => mvarNoteID = value;
    }
}
