using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

public class SKeyMaps : IEnumerable<SKeyMap>
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

    public SKeyMap Add(int AssignDirect, string sKey = null)
    {
        // create a new object
        SKeyMap objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            AssignDirect = AssignDirect
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

    public SKeyMap this[int Index] => (SKeyMap)mCol[Index];
    public SKeyMap this[string Key] => (SKeyMap)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SKeyMap> GetEnumerator() => mCol.Cast<SKeyMap>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
