using System;
using System.Runtime.InteropServices;
using System.Windows;
using static Microsoft.VisualBasic.Constants;
using static MidiLib;
using static modSappy;

namespace SappySharp.Forms;

public partial class frmSelectMidiOut : Window
{
    private static frmSelectMidiOut _instance;
    public static frmSelectMidiOut instance { set { _instance = null; } get { return _instance ??= new frmSelectMidiOut(); } }
    public static void Load() { if (_instance == null) { dynamic A = instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmSelectMidiOut() { InitializeComponent(); }

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
    // __________________
    // |  MidiOUT dialog  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 100% by Kyoufu Kawa.                          |
    // | Last update: July 20th, 2006                       |
    // |____________________________________________________|

    // ###########################################################################################


    [LibraryImport("winmm.dll")]
    private static partial int midiOutGetNumDevs();
    [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
    private static extern int midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS lpCaps, int uSize);

    class MIDIOUTCAPS
    {
        public int wMid;
        public int wPid;
        public int vDriverVersion;
        public string szPname; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (32)
        public int wTechnology;
        public int wVoices;
        public int wNotes;
        public int wChannelMask;
        public int dwSupport;
    }

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        ClickSound();
        WantedMidiDevice = List1.SelectedIndex;
        WriteSettingI("MIDI Device", WantedMidiDevice);
        Unload();
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        if (midiOutGetNumDevs() == 0)
        {
            List1.AddItem("No devices.");
            return;
        }

        MIDIOUTCAPS myCaps = null;
        for (int i = 1; i <= midiOutGetNumDevs(); i += 1)
        {
            midiOutGetDevCaps(i - 1, ref myCaps, 52); // LenB(myCaps)
            List1.AddItem(myCaps.szPname); // Trim(myCaps.szPname)
        }

        List1.SelectedIndex = WantedMidiDevice;

        SetCaptions(this);
        Title = Properties.Resources._9000;
    }

    private void Form_Paint(object sender, EventArgs e)
    {
        DrawSkin(this);
    }

    private void List1_Click(object sender, RoutedEventArgs e) { List1_Click(); }
    private void List1_Click()
    {
        if (midiOutGetNumDevs() == 0) Label1.Content = "";

        MIDIOUTCAPS myCaps = null;
        midiOutGetDevCaps(List1.SelectedIndex, ref myCaps, 52);
        Label1.Content = myCaps.wTechnology switch
        {
            0 => Properties.Resources._9000,
            1 => Properties.Resources._9001,
            2 => Properties.Resources._9002,
            3 => Properties.Resources._9003,
            4 => Properties.Resources._9004,
            5 => Properties.Resources._9005,
            6 => Properties.Resources._9006,
            7 => Properties.Resources._9007,
            _ => throw new NotImplementedException()
        };
        switch (myCaps.wTechnology)
        {
            case 1:
                Label1.Content = Label1.Content + vbCrLf + "W00t!";
                break;
            case 3:
                Label1.Content = Label1.Content + vbCrLf + "Oooh...";
                break;
            case 4:
                Label1.Content = Label1.Content + vbCrLf + "Lame...";
                break;
            case 6:
                Label1.Content = Label1.Content + vbCrLf + "Sweet!";
                break;
            case 7:
                Label1.Content = Label1.Content + vbCrLf + "Cool!";
                break;
        }
    }

    // Private Const MOD_MIDIPORT As Long = 1
    // Private Const MOD_SYNTH As Long = 2
    // Private Const MOD_SQSYNTH As Long = 3
    // Private Const MOD_FMSYNTH As Long = 4
    // Private Const MOD_MAPPER As Long = 5
    // Private Const MOD_WAVETABLE As Long = 6
    // Private Const MOD_SWSYNTH As Long = 7
}
