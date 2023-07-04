using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

public class SInstruments : IEnumerable<SInstrument>
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

    public SInstrument Add(string sKey = null)
    {
        // create a new object
        SInstrument objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            KeyMaps = new SKeyMaps(),
            Directs = new SDirects()
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

    public SInstrument this[int Index] => (SInstrument)mCol[Index];
    public SInstrument this[string Key] => (SInstrument)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SInstrument> GetEnumerator() => mCol.Cast<SInstrument>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
