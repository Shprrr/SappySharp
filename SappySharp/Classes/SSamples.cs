using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

public class SSamples : IEnumerable<SSample>
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

    public SSample Add(string sKey = null)
    {
        // create a new object
        SSample objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            //SampleData = SampleData,
            //loopstart = loopstart,
            //Size = Size,
            //Frequency = Frequency,
            //FModSample = FModSample
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

    public SSample this[int Index] => (SSample)mCol[Index];
    public SSample this[string Key] => (SSample)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SSample> GetEnumerator() => mCol.Cast<SSample>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
