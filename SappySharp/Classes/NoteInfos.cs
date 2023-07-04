using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Strings;
using static SappySharp.Classes.NoteInfo;

namespace SappySharp.Classes;

public class NoteInfos : IEnumerable<NoteInfo>
{
    private readonly Collection mCol = new();

    public void Clear()
    {
        while (mCol.Count > 0)
        {
            mCol.Remove(1);
        }
    }

    public NoteInfo Add(bool Enabled, int FModChannel, byte NoteNumber, int Frequency, byte Velocity, int ParentChannel, byte UnknownValue, NoteOutputTypes outputtype, byte EnvAttenuation, byte EnvDecay, byte EnvSustain, byte EnvRelease, int WaitTicks, byte PatchNumber, string sKey = null)
    {
        NoteInfo objNewMember = new()
        {
            Key = sKey,
            NoteOff = false,
            Enabled = Enabled,
            FModChannel = FModChannel,
            NoteNumber = NoteNumber,
            Frequency = Frequency,
            Velocity = Velocity,
            PatchNumber = PatchNumber,
            ParentChannel = ParentChannel,
            //SampleID = SampleID,
            UnknownValue = UnknownValue,
            outputtype = outputtype,
            Notephase = NotePhases.npInitial,
            EnvDestination = 0,
            EnvStep = 0,
            EnvPosition = 0,
            EnvAttenuation = EnvAttenuation,
            EnvDecay = EnvDecay,
            EnvSustain = EnvSustain,
            EnvRelease = EnvRelease,
            WaitTicks = WaitTicks
        };
        if (Len(sKey) == 0)
        {
            mCol.Add(objNewMember);
        }
        else
        {
            mCol.Add(objNewMember, sKey);
        }

        return objNewMember;
    }

    public NoteInfo this[int Index] => (NoteInfo)mCol[Index];
    public NoteInfo this[string Key] => (NoteInfo)mCol[Key];

    public int count => mCol.Count;

    public void Remove(dynamic vntIndexKey)
    {
        mCol.Remove(vntIndexKey);
    }

    public IEnumerator<NoteInfo> GetEnumerator() => mCol.Cast<NoteInfo>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => mCol.GetEnumerator();
}
