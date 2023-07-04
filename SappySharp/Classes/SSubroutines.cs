using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

/// <summary>
/// Collection of subroutine information
/// </summary>
public class SSubroutines : IEnumerable<SSubroutine>
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

    public SSubroutine Add(int EventQueuePointer, string sKey = null)
    {
        // create a new object
        SSubroutine objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            EventQueuePointer = EventQueuePointer
        };
        if (Len(sKey) == 0)
        {
            mCol.Add(objNewMember);
        }
        else
        {
            mCol.Add(objNewMember, sKey);
        }

        // return the object created
        return objNewMember;
    }

    public SSubroutine this[int Index] => (SSubroutine)mCol[Index];
    public SSubroutine this[string Key] => (SSubroutine)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SSubroutine> GetEnumerator() => mCol.Cast<SSubroutine>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
