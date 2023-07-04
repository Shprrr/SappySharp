using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace SappySharp.Classes;

public class SDirects : IEnumerable<SDirect>
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

    public SDirect Add(string sKey)
    {
        // create a new object
        SDirect objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            SampleID = "",
            EnvAttenuation = 0,
            EnvDecay = 0,
            EnvSustain = 0,
            EnvRelease = 0,
            Raw0 = 0,
            Raw1 = 0,
            GB1 = 0,
            GB2 = 0,
            GB3 = 0,
            GB4 = 0,
            Reverse = false,
            FixedPitch = false,
            DrumTuneKey = 0x3C
        };
        mCol.Add(objNewMember, sKey);

        // return the object created
        return objNewMember;
    }

    public SDirect this[int Index] => (SDirect)mCol[Index];
    public SDirect this[string Key] => (SDirect)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SDirect> GetEnumerator() => mCol.Cast<SDirect>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
