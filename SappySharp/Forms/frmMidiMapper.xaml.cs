using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.VBMath;
using static MidiLib;
using static modSappy;
using static SapPlayer;
using static VBExtension;

namespace SappySharp.Forms;

public partial class frmMidiMapper : Window
{
    public frmMidiMapper()
    {
        InitializeComponent();
        Timer1.IsEnabled = false;
        Timer1.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        Timer1.Tick += Timer1_Timer;
    }

    public List<RadioButton> Option1 { get => VBExtension.controlArray<RadioButton>(this, "Option1"); }

    public DispatcherTimer Timer1 { get; set; } = new DispatcherTimer();

    public List<Grid> Picture1 { get => VBExtension.controlArray<Grid>(this, "Picture1"); }

    // ______________
    // |  SAPPY 2006  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Interface code © 2006 by Kyoufu Kawa               |
    // | Player code © 200X by DJ ßouché                    |
    // | In-program graphics by Kyoufu Kawa                 |
    // | Thanks to SomeGuy, Majin Bluedragon and SlimeSmile |
    // |                                                    |
    // | This code is NOT in the Public Domain or whatever. |
    // | At least until Kyoufu Kawa releases it in the PD   |
    // | himself.  Until then, you're not supposed to even  |
    // | HAVE this code unless given to you by Kawa or any  |
    // | other Helmeted Rodent member.                      |
    // |____________________________________________________|
    // _______________
    // |  MIDI Mapper  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 100% by Kyoufu Kawa.                          |
    // | Last update: July 21st, 2006                       |
    // |____________________________________________________|

    // ###########################################################################################


    int[] MidiMap = new int[127];
    int[] MidiTrans = new int[127];
    int[] DrumMap = new int[127];

    string[] InstNames = new string[128];
    string[] Drums = new string[128];

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        Command1.IsEnabled = false;
        if (Option1[0].IsChecked.GetValueOrDefault(true))
        {
            SelectInstrument(4, lstRemapTo.SelectedIndex);
            Command1.Tag = 50 + Rnd() * 4;
            ToneOn(4, CInt(Command1.Tag), 127);
            Timer1.IsEnabled = true;
        }
        else
        {
            Command1.Tag = lstDrumR.SelectedIndex + 35;
            ToneOn(9, CInt(Command1.Tag), 127);
            Timer1.IsEnabled = true;
        }
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        MidiClose();

        bool need = false;

        XmlElement NewMap = frmSappy.instance.x.CreateElement("midimap");

        for (int i = 0; i <= 127; i += 1)
        {
            if (MidiMap[i] != i) // it's remapped
            {
                need = true;
                XmlElement NewInst = frmSappy.instance.x.CreateElement("inst");
                XmlAttribute NewAtt = frmSappy.instance.x.CreateAttribute("from");
                NewAtt.Value = i.ToString();
                NewInst.Attributes.SetNamedItem(NewAtt);
                NewAtt = frmSappy.instance.x.CreateAttribute("to");
                NewAtt.Value = MidiMap[i].ToString();
                NewInst.Attributes.SetNamedItem(NewAtt);
                if (MidiTrans[i] != 0)
                {
                    NewAtt = frmSappy.instance.x.CreateAttribute("transpose");
                    NewAtt.Value = MidiTrans[i].ToString();
                    NewInst.Attributes.SetNamedItem(NewAtt);
                }
                NewMap.AppendChild(NewInst);
            }
        }

        for (int i = 0; i <= 127; i += 1)
        {
            if (DrumMap[i] != i) // it's remapped
            {
                need = true;
                XmlElement NewInst = frmSappy.instance.x.CreateElement("drum");
                XmlAttribute NewAtt = frmSappy.instance.x.CreateAttribute("from");
                NewAtt.Value = i.ToString();
                NewInst.Attributes.SetNamedItem(NewAtt);
                NewAtt = frmSappy.instance.x.CreateAttribute("to");
                NewAtt.Value = DrumMap[i].ToString();
                NewInst.Attributes.SetNamedItem(NewAtt);
                NewMap.AppendChild(NewInst);
            }
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next
        frmSappy.instance.MidiMapsDaddy.RemoveChild(frmSappy.instance.MidiMapNode);
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        if (need)
        {
            frmSappy.instance.MidiMapsDaddy.AppendChild(NewMap);
        }

        frmSappy.instance.x.Save(frmSappy.instance.xfile);
        frmSappy.instance.LoadGameFromXML(ref frmSappy.instance.gamecode);
        Close();
    }

    private void Command3_Click(object sender, RoutedEventArgs e) { Command3_Click(); }
    private void Command3_Click()
    {
        MidiClose();
        Close();
    }

    private void Command4_Click(object sender, RoutedEventArgs e) { Command4_Click(); }
    private void Command4_Click()
    {
        for (int i = 0; i <= 127; i += 1)
        {
            MidiMap[i] = i;
            MidiTrans[i] = 0;
            DrumMap[i] = i;
        }

        lstInsts.Clear();
        for (int i = 0; i <= 127; i += 1)
        {
            lstInsts.AddItem(i + " - " + InstNames[MidiMap[i]]);
        }

        lstDrums.Clear();
        for (int i = 35; i <= 81; i += 1)
        {
            lstDrums.AddItem(i + " - " + Drums[DrumMap[i]]);
        }

        lstInsts.SelectedIndex = 0;
        lstDrums.SelectedIndex = 0;
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        InstNames[0] = "Acoustic Grand Piano";
        InstNames[1] = "Bright Acoustic Piano";
        InstNames[2] = "Electric Grand Piano";
        InstNames[3] = "Honky-tonk Piano";
        InstNames[4] = "Rhodes Piano";
        InstNames[5] = "Chorus Piano";
        InstNames[6] = "Harpsichord";
        InstNames[7] = "Clavinet";
        InstNames[8] = "Celesta";
        InstNames[9] = "Glockenspiel";
        InstNames[10] = "Music Box";
        InstNames[11] = "Vibraphone";
        InstNames[12] = "Marimba";
        InstNames[13] = "Xylophone";
        InstNames[14] = "Tubular Bells";
        InstNames[15] = "Dulcimer";
        InstNames[16] = "Hammond Organ";
        InstNames[17] = "Percuss. Organ";
        InstNames[18] = "Rock Organ";
        InstNames[19] = "Church Organ";
        InstNames[20] = "Reed Organ";
        InstNames[21] = "Accordion";
        InstNames[22] = "Harmonica";
        InstNames[23] = "Tango Accordion";
        InstNames[24] = "Acoustic Guitar (nylon)";
        InstNames[25] = "Acoustic Guitar (steel)";
        InstNames[26] = "Electric Guitar (jazz)";
        InstNames[27] = "Electric Guitar (clean)";
        InstNames[28] = "Electric Guitar (muted)";
        InstNames[29] = "Overdriven Guitar";
        InstNames[30] = "Distortion Guitar";
        InstNames[31] = "Guitar Harmonics";
        InstNames[32] = "Acoustic Bass";
        InstNames[33] = "Electric Bass (finger)";
        InstNames[34] = "Electric Bass (pick)";
        InstNames[35] = "Fretless Bass";
        InstNames[36] = "Slap Bass 1";
        InstNames[37] = "Slap Bass 2";
        InstNames[38] = "Synth Bass 1";
        InstNames[39] = "Synth Bass 2";
        InstNames[40] = "Violin";
        InstNames[41] = "Viola";
        InstNames[42] = "Cello";
        InstNames[43] = "Contra Bass";
        InstNames[44] = "Tremolo Strings";
        InstNames[45] = "Pizzicato Strings";
        InstNames[46] = "Orchestral Harp";
        InstNames[47] = "Timpani";
        InstNames[48] = "String Ensemble 1";
        InstNames[49] = "String Ensemble 2";
        InstNames[50] = "Synth Strings 1";
        InstNames[51] = "Synth Strings 2";
        InstNames[52] = "Choir Aahs";
        InstNames[53] = "Voice Oohs";
        InstNames[54] = "Synth Voice";
        InstNames[55] = "Orchestra Hit";
        InstNames[56] = "Trumpet";
        InstNames[57] = "Trombone ";
        InstNames[58] = "Tuba";
        InstNames[59] = "Muted Trumpet";
        InstNames[60] = "French Horn ";
        InstNames[61] = "Brass Section";
        InstNames[62] = "Synth Brass 1";
        InstNames[63] = "Synth Brass 2";
        InstNames[64] = "Soprano Sax";
        InstNames[65] = "Alto Sax";
        InstNames[66] = "Tenor Sax";
        InstNames[67] = "Baritone Sax";
        InstNames[68] = "Oboe";
        InstNames[69] = "English Horn";
        InstNames[70] = "Bassoon";
        InstNames[71] = "Clarinet";
        InstNames[72] = "Piccolo";
        InstNames[73] = "Flute";
        InstNames[74] = "Recorder";
        InstNames[75] = "Pan Flute";
        InstNames[76] = "Bottle Blow";
        InstNames[77] = "Shaku";
        InstNames[78] = "Whistle";
        InstNames[79] = "Ocarina";
        InstNames[80] = "Lead 1 (square)";
        InstNames[81] = "Lead 2 (saw tooth)";
        InstNames[82] = "Lead 3 (calliope lead)";
        InstNames[83] = "Lead 4 (chiff lead)";
        InstNames[84] = "Lead 5 (charang)";
        InstNames[85] = "Lead 6 (voice)";
        InstNames[86] = "Lead 7 (fifths)";
        InstNames[87] = "Lead 8 (bass + lead)";
        InstNames[88] = "Pad 1 (new age)";
        InstNames[89] = "Pad 2 (warm)";
        InstNames[90] = "Pad 3 (poly synth)";
        InstNames[91] = "Pad 4 (choir)";
        InstNames[92] = "Pad 5 (bowed)";
        InstNames[93] = "Pad 6 (metallic)";
        InstNames[94] = "Pad 7 (halo)";
        InstNames[95] = "Pad 8 (sweep)";
        InstNames[96] = "FX 1 (rain)";
        InstNames[97] = "FX 2 (sound track)";
        InstNames[98] = "FX 3 (crystal)";
        InstNames[99] = "FX 4 (atmosphere)";
        InstNames[100] = "FX 5 (bright)";
        InstNames[101] = "FX 6 (goblins)";
        InstNames[102] = "FX 7 (echoes)";
        InstNames[103] = "FX 8 (sci-fi)";
        InstNames[104] = "Sitar";
        InstNames[105] = "Banjo";
        InstNames[106] = "Shamisen";
        InstNames[107] = "Koto";
        InstNames[108] = "Kalimba";
        InstNames[109] = "Bagpipe";
        InstNames[110] = "Fiddle";
        InstNames[111] = "Shanai";
        InstNames[112] = "Tinkle Bell";
        InstNames[113] = "Agogo";
        InstNames[114] = "Steel Drums";
        InstNames[115] = "Wood block";
        InstNames[116] = "Taiko Drum";
        InstNames[117] = "Melodic Tom";
        InstNames[118] = "Synth Drum ";
        InstNames[119] = "Reverse Cymbal";
        InstNames[120] = "Guitar Fret Noise ";
        InstNames[121] = "Breath Noise";
        InstNames[122] = "Seashore";
        InstNames[123] = "Bird Tweet";
        InstNames[124] = "Telephone Ring";
        InstNames[125] = "Helicopter";
        InstNames[126] = "Applause";
        InstNames[127] = "Gunshot";

        Drums[35] = "Acoustic Bass Drum";
        Drums[36] = "Bass Drum 1";
        Drums[37] = "Side Stick";
        Drums[38] = "Acoustic Snare";
        Drums[39] = "Hand Clap";
        Drums[40] = "Electric Snare";
        Drums[41] = "Low Floor Tom";
        Drums[42] = "Closed Hihat";
        Drums[43] = "High Floor Tom";
        Drums[44] = "Pedal Hihat";
        Drums[45] = "Low Tom";
        Drums[46] = "Open Hihat";
        Drums[47] = "Low-Mid Tom";
        Drums[48] = "Hi-Mid Tom";
        Drums[49] = "Crash Cymbal 1";
        Drums[50] = "High Tom";
        Drums[51] = "Ride Cymbal 1";
        Drums[52] = "Chinese Cymbal";
        Drums[53] = "Ride Bell";
        Drums[54] = "Tambourine";
        Drums[55] = "Splash Cymbal";
        Drums[56] = "Cowbell";
        Drums[57] = "Crash Cymbal 1";
        Drums[58] = "Vibraslap";
        Drums[59] = "Ride Cymbal 2";
        Drums[60] = "High Bongo";
        Drums[61] = "Low Bongo";
        Drums[62] = "Mute High Conga";
        Drums[63] = "Open High Conga";
        Drums[64] = "Low Conga";
        Drums[65] = "High Timbale";
        Drums[66] = "Low Timbale";
        Drums[67] = "High Agogo";
        Drums[68] = "Low Agogo";
        Drums[69] = "Cabasa";
        Drums[70] = "Maracas";
        Drums[71] = "Short Whistle";
        Drums[72] = "Long Whistle";
        Drums[73] = "Short Guiro";
        Drums[74] = "Long Guiro";
        Drums[75] = "Claves";
        Drums[76] = "High Woodblock";
        Drums[77] = "Low Woodblock";
        Drums[78] = "Mute Cuica";
        Drums[79] = "Open Cuica";
        Drums[80] = "Mute Triangle";
        Drums[81] = "Open Triangle  ";

        for (int i = 0; i <= 127; i += 1)
        {
            MidiMap[i] = i;
            MidiTrans[i] = 0;
            DrumMap[i] = i;
        }

        if (frmSappy.instance.MidiMapNode != null)
        {
            foreach (XmlElement n4 in frmSappy.instance.MidiMapNode.ChildNodes)
            {
                if (n4.Name == "inst")
                {
                    int i = int.Parse(n4.GetAttribute("from"));
                    MidiMap[i] = int.Parse(n4.GetAttribute("to"));
                    // TODO: (NOT SUPPORTED): On Error Resume Next
                    MidiTrans[i] = int.Parse(n4.GetAttribute("transpose"));
                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                }
                if (n4.Name == "drum")
                {
                    int i = int.Parse(n4.GetAttribute("from"));
                    DrumMap[i] = int.Parse(n4.GetAttribute("to"));
                }
            }
        }

        for (int i = 0; i <= 127; i += 1)
        {
            lstInsts.AddItem(i + " - " + InstNames[MidiMap[i]]);
            lstRemapTo.AddItem(i + " - " + InstNames[i]);
        }

        for (int i = 35; i <= 81; i += 1)
        {
            lstDrums.AddItem(NoteToName((byte)i) + " - " + Drums[DrumMap[i]]);
            lstDrumR.AddItem(NoteToName((byte)i) + " - " + Drums[i]);
        }

        lstInsts.SelectedIndex = 0;
        lstDrums.SelectedIndex = 0;
        MidiOpen();

        //Picture1[0].BorderStyle = 0;
        //Picture1[1].BorderStyle = 0;
        Option1[0].IsChecked = true;
        Option1_Click(0);

        Title = Properties.Resources._5000;
        SetCaptions(this);
    }

    private void lstDrumR_Click(object sender, RoutedEventArgs e) { lstDrumR_Click(); }
    private void lstDrumR_Click()
    {
        DrumMap[lstDrums.SelectedIndex + 35] = lstDrumR.SelectedIndex + 35;
        lstDrums.Items[lstDrums.SelectedIndex] = NoteToName((byte)(lstDrums.SelectedIndex + 35)) + " - " + Drums[lstDrumR.SelectedIndex + 35];
    }

    private void lstDrumR_DblClick(object sender, MouseButtonEventArgs e) { lstDrumR_DblClick(); }
    private void lstDrumR_DblClick()
    {
        if (Command1.Tag.ToString() != "") Timer1_Timer(this, EventArgs.Empty);
        Command1_Click();
    }

    private void lstDrums_Click(object sender, RoutedEventArgs e) { lstDrums_Click(); }
    private void lstDrums_Click()
    {
        lstDrumR.SelectedIndex = DrumMap[lstDrums.SelectedIndex + 35] - 35;
    }

    private void lstDrums_DblClick(object sender, MouseButtonEventArgs e) { lstDrums_DblClick(); }
    private void lstDrums_DblClick()
    {
        if (Command1.Tag.ToString() != "") Timer1_Timer(this, EventArgs.Empty);
        Command1_Click();
    }

    private void lstInsts_Click(object sender, RoutedEventArgs e) { lstInsts_Click(); }
    private void lstInsts_Click()
    {
        lstRemapTo.SelectedIndex = MidiMap[lstInsts.SelectedIndex];
        txtTranspose.Text = MidiTrans[lstInsts.SelectedIndex].ToString();
    }

    private void lstInsts_DblClick(object sender, MouseButtonEventArgs e) { lstInsts_DblClick(); }
    private void lstInsts_DblClick()
    {
        if (Command1.Tag.ToString() != "") Timer1_Timer(this, EventArgs.Empty);
        Command1_Click();
    }

    private void lstRemapTo_Click(object sender, RoutedEventArgs e) { lstRemapTo_Click(); }
    private void lstRemapTo_Click()
    {
        MidiMap[lstInsts.SelectedIndex] = lstRemapTo.SelectedIndex;
        lstInsts.Items[lstInsts.SelectedIndex] = lstInsts.SelectedIndex + " - " + InstNames[lstRemapTo.SelectedIndex];
    }

    private void lstRemapTo_DblClick(object sender, MouseButtonEventArgs e) { lstRemapTo_DblClick(); }
    private void lstRemapTo_DblClick()
    {
        if (Command1.Tag.ToString() != "") Timer1_Timer(this, EventArgs.Empty);
        Command1_Click();
    }

    private void Option1_Click(object sender, RoutedEventArgs e) { Option1_Click(Option1.IndexOf((RadioButton)sender)); }
    private void Option1_Click(int Index)
    {
        Picture1[Index].ZOrder(0);
    }

    private void Timer1_Timer(object sender, EventArgs e)
    {
        ToneOff(4, CInt(Command1.Tag));
        Command1.IsEnabled = true;
    }

    private void txtTranspose_LostFocus(object sender, RoutedEventArgs e) { txtTranspose_LostFocus(); }
    private void txtTranspose_LostFocus()
    {
        MidiTrans[lstInsts.SelectedIndex] = (int)Val(txtTranspose.Text);
    }
}
