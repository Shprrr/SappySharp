using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using System.Linq;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SappySharp.Forms;
using static modSappy;
using static FMod;
using static mdlFile;
using static SapPlayer;
using static MidiLib;
using static mColorUtils;
using static mTrace;
using static SappySharp.Forms.frmSappy;
using static SappySharp.Forms.frmTakeTrax;
using static SappySharp.Forms.frmMakeTrax;
using static SappySharp.Forms.frmAbout;
using static SappySharp.Forms.frmTakeSamp;
using static SappySharp.Forms.frmAssembler;
using static SappySharp.Forms.frmOptions;
using static SappySharp.Forms.frmMidiMapper;
using static SappySharp.Forms.frmSelectMidiOut;
using static SappySharp.Forms.frmInputBox;
using static SappySharp.Classes.SChannels;
using static SappySharp.Classes.SNotes;
using static SappySharp.Classes.NoteInfo;
using static SappySharp.Classes.SChannel;
using static SappySharp.Classes.SNote;
using static SappySharp.Classes.SSubroutines;
using static SappySharp.Classes.SSubroutine;
using static SappySharp.Classes.SappyEventQueue;
using static SappySharp.Classes.SappyEvent;
using static SappySharp.Classes.NoteInfos;
using static SappySharp.Classes.SSamples;
using static SappySharp.Classes.SSample;
using static SappySharp.Classes.SDirects;
using static SappySharp.Classes.SDirect;
using static SappySharp.Classes.SDrumKit;
using static SappySharp.Classes.SDrumKits;
using static SappySharp.Classes.SInstruments;
using static SappySharp.Classes.SInstrument;
using static SappySharp.Classes.SKeyMaps;
using static SappySharp.Classes.SKeyMap;
using static SappySharp.Classes.clsSappyDecoder;
using static SappySharp.Classes.gCommonDialog;
using static SappySharp.Classes.pcMemDC;
using static SappySharp.Classes.cVBALImageList;
using static SappySharp.Classes.cRegistry;
using System.Collections;

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
