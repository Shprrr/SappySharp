using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace SappySharp.Classes;

/// <summary>
/// Holds Events to be procesed
/// </summary>
public class SappyEventQueue : IEnumerable<SappyEvent>
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

    public SappyEvent Add(int Ticks, byte CommandByte, byte Param1, byte Param2, byte Param3, string sKey = null)
    {
        // create a new object
        SappyEvent objNewMember = new()
        {
            // set the properties passed into the method
            Key = sKey,
            Ticks = Ticks,
            CommandByte = CommandByte,
            Param1 = Param1,
            Param2 = Param2,
            Param3 = Param3
        };
        // For i = 1 To (mCol.Count)
        //    If mCol(i).Ticks > objNewMember.Ticks Then
        //    i = i - 1
        //    Exit For
        //    End If
        // Next i
        // // If Len(sKey) = 0 Then
        // If i = 0 Then
        //     mCol.Add objNewMember, , 1
        // Else
        //     If mCol.Count < 1 Then
        mCol.Add(objNewMember);
        //     Else
        //     mCol.Add objNewMember, , , i - 1
        //     End If
        // End If
        // Else
        //     mCol.Add objNewMember, sKey
        // End If

        // return the object created
        return objNewMember;
    }

    public SappyEvent this[int Index] => (SappyEvent)mCol[Index];
    public SappyEvent this[string Key] => (SappyEvent)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        // used when removing an element from the collection
        // vntIndexKey contains either the Index or Key, which is why
        // it is declared as a Variant
        // Syntax: x.Remove(xyz)

        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<SappyEvent> GetEnumerator() => mCol.Cast<SappyEvent>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
