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
using static SappySharp.Classes.cNoStatusBar;
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
using System.IO;

namespace SappySharp.Forms;

public partial class frmMakeTrax : Window
{
    private static frmMakeTrax _instance;
    public static frmMakeTrax instance { set { _instance = null; } get { return _instance ??= new frmMakeTrax(); } }
    public static void Load() { if (_instance == null) { dynamic A = frmMakeTrax.instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmMakeTrax() { InitializeComponent(); }

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
    // |  Track importer  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 100% by Kyoufu Kawa, based on old Juicer.     |
    // | Last update: December 15th, 2005                   |
    // |____________________________________________________|

    // ###########################################################################################

    public byte MyNumblocks = 0;
    public byte MyPriority = 0;
    public byte MyReverb = 0;
    public int SongTableEntry = 0;

    private int[] Tracks = new int[32];

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        ClickSound();
        Unload();
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        ClickSound();
        if (lstTracks.SelectedItems.Count == 0)
        {
            MsgBox(Properties.Resources._2007);
            return;
        }
        if (txtTrack.Text == "")
        {
            MsgBox(Properties.Resources._2008);
            return;
        }
        if (txtHeaderOffset.Text == "")
        {
            MsgBox(Properties.Resources._2009);
            return;
        }

        txtLog.Margin = new Thickness(txtLog.Margin.Left, 8, txtLog.Margin.Right, txtLog.Margin.Bottom);
        txtLog.Visibility = Visibility.Visible;
        Scribe(Properties.Resources._2010);
        Scribe(new string('¯', Len(Properties.Resources._2010)));
        int j = 0;
        for (int i = 0; i <= lstTracks.Items.Count - 1; i += 1)
        {
            if (lstTracks.Selected(i))
            {
                string t = (string)lstTracks.Items[i];
                Scribe(Replace(Replace(Properties.Resources._2011, "$FILE", t), "$TO", "0x" + Hex("&H" + FixHex(txtTrack.Text, 6))));
                InsertTrack(t, CInt(Val("&H" + Hex("&H" + FixHex(txtTrack.Text, 6)))));
                Tracks[j] = (int)Val("&H" + Hex("&H" + FixHex(txtTrack.Text, 6)));
                FileOpen(3, t, OpenMode.Binary);
                int p = (int)(Val("&H" + Hex("&H" + FixHex(txtTrack.Text, 6))) + LOF(3));
                txtTrack.Text = "0x" + Hex(p);
                FileClose(3);
                j++;
            }
        }
        Scribe(Properties.Resources._2012);
        Seek(99, (int)(Val("&H" + Hex("&H" + FixHex(txtHeaderOffset.Text, 6))) + 1));
        FilePutObject(99, (byte)lstTracks.SelectedItems.Count);
        FilePutObject(99, MyNumblocks);
        FilePutObject(99, MyPriority);
        FilePutObject(99, MyReverb);
        FilePutObject(99, CLng(Val("&H" + FixHex(txtVoicegroup.Text, 6)) + 0x8000000));
        for (int i = 0; i <= lstTracks.SelectedItems.Count - 1; i += 1)
        {
            FilePutObject(99, Tracks[i] + 0x8000000);
        }
        if (MsgBox(Properties.Resources._2013, vbYesNo) == vbYes)
        {
            Scribe("Updating song table...");
            int p = (int)Val("&H" + Hex("&H" + FixHex(txtHeaderOffset.Text, 6)));
            p += 0x8000000;
            FilePutObject(99, p, SongTableEntry + 1);
        }
        Scribe(Properties.Resources._7);
        Command2.IsEnabled = false;
        // Command1.FontBold = False
        Command1.Content = Properties.Resources._6;
        // Command1.FontBold = True
        Command1.IsDefault = true;
        frmSappy.instance.LoadSong(int.Parse(frmSappy.instance.txtSong.Text));
        IncessantNoises("TaskComplete");
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            lstTracks.AddItem(file);
        }
        SetCaptions(this);
        Title = Properties.Resources._2000;
    }

    private void InsertTrack(string t, int o)
    {
        byte b = 0;
        int p = 0;
        Seek(99, o + 1);
        FileOpen(98, t, OpenMode.Binary);
        do
        {
            FileGet(98, ref b);
            FilePutObject(99, b);
            if (b == 0xB1) break;
            if (b == 0xB2 || b == 0xB3 || b == 0xB5)
            {
                //Scribe(Properties.Resources._2014);
                if (b == 0xB5)
                {
                    FileGet(98, ref b);
                    FilePutObject(99, b);
                }
                FileGet(98, ref p);
                //Scribe(Replace(Replace(Properties.Resources._2015, "$OLD", "0x" + FixHex(p, 8)), "$NEW", "0x" + FixHex(p + o, 6)));
                p = p + 0x8000000 + o;
                FilePutObject(99, p);
            }
            DoEvents();
        } while (!EOF(98));
        FileClose(98);
    }

    private void Scribe(string t)
    {
        txtLog.Text = txtLog.Text + t + vbCrLf;
        txtLog.SelectionStart = Len(txtLog.Text);
    }

    private void Picture1_Paint(object sender, RoutedEventArgs e)
    {
        DrawSkin(Picture1);
    }
}
