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
