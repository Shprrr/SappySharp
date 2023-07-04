using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

public class SDrumKits : IEnumerable<SDrumKit>
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

    public SDrumKit Add(string sKey = null)
    {
        // create a new object
        SDrumKit objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
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

    public SDrumKit this[int Index] => (SDrumKit)mCol[Index];
    public SDrumKit this[string Key] => (SDrumKit)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SDrumKit> GetEnumerator() => mCol.Cast<SDrumKit>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
