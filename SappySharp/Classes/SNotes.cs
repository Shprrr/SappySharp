using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

/// <summary>
/// Holds a collection of notes
/// </summary>
public class SNotes : IEnumerable<SNote>
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

    public SNote Add(byte NoteID, string sKey = null)
    {
        // create a new object
        SNote objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            NoteID = NoteID
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

    public SNote this[int Index] => (SNote)mCol[Index];
    public SNote this[string Key] => (SNote)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SNote> GetEnumerator() => mCol.Cast<SNote>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
