namespace SappySharp.Classes;

public class SappyEvent
{
    /// <summary>
    /// Event ID
    /// </summary>
    public string Key = "";

    // local variable(s) to hold property value(s)
    int mvarTicks = 0; // local copy
    byte mvarCommandByte = 0; // local copy
    byte mvarParam1 = 0; // local copy
    byte mvarParam2 = 0; // local copy
    byte mvarParam3 = 0; // local copy
    /// <summary>
    /// Command Paramater 1
    /// </summary>
    public byte Param3
    {
        get => mvarParam3;
        set => mvarParam3 = value;
    }

    /// <summary>
    /// Command Paramater 2
    /// </summary>
    public byte Param2
    {
        get => mvarParam2;
        set => mvarParam2 = value;
    }

    /// <summary>
    /// Command Paramater 1
    /// </summary>
    public byte Param1
    {
        get => mvarParam1;
        set => mvarParam1 = value;
    }

    /// <summary>
    /// Sappy command byte:<br/>
    /// B1 - End Track <br/>
    /// B2 - Jump to position in event queue (Params: ticks)<br/>
    /// B3 - Jump to subroutine in event queue (Params: ticks)<br/>
    /// B4 - End of subroutine <br/>
    /// BB - Tempo (Params: BPM/2)<br/>
    /// BC - Transpose (Params: Signed Semitone)<br/>
    /// BD - Set Patch (Params: GM)<br/>
    /// BE - Set Volume (Params: GM)<br/>
    /// BF - Set Panning (Params: Signed GM)<br/>
    /// C0 - Set Pitch Bend (Params: Signed GM)<br/>
    /// C1 - Set Pitch Bend Range (Params: Semitones)<br/>
    /// C2 - Set vibrato depth (Params: Number)<br/>
    /// C4 - Set vibrato rate (Params: Number)<br/>
    /// CE - Note sustain off<br/>
    /// CF - Note sustain for long notes<br/>
    /// D1-FF - Play a note (Params: Note number, Velocity, ???)
    /// </summary>
    public byte CommandByte
    {
        get => mvarCommandByte;
        set => mvarCommandByte = value;
    }

    public int Ticks { get => mvarTicks; set => mvarTicks = value; }
}
