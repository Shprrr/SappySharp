using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace SappySharp.Classes;

/// <summary>
/// Holds collection of channels
/// </summary>
public class SChannels : IEnumerable<SChannel>
{
    // local variable to hold collection
    private readonly Collection mCol = new();
    public void Clear()
    {
        while (mCol.Count > 0)
        {
            mCol.Remove(1);
        }
    }

    public SChannel Add(string sKey = null)
    {
        // create a new object
        SChannel objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            Notes = new SNotes(),
            PatchNumber = 0,
            Panning = 0x40,
            MainVolume = 100,
            mute = false,
            Enabled = true,
            PitchBend = 0x40,
            PitchBendRange = 2,
            Sustain = false,
            Transpose = 0,
            Subroutines = new SSubroutines(),
            EventQueue = new SappyEventQueue(),
            LoopPointer = 0,
            SubCountAtLoop = 1,
            SubroutineCounter = 1,
            ProgramCounter = 1,
            WaitTicks = -1
        };
        mCol.Add(objNewMember);

        // return the object created
        return objNewMember;
    }

    public SChannel this[int Index] => (SChannel)mCol[Index];
    public SChannel this[string Key] => (SChannel)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SChannel> GetEnumerator() => mCol.Cast<SChannel>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
