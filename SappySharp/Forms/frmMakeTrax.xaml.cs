using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static modSappy;
using static SappySharp.VBFileSystem;
using static VBExtension;

namespace SappySharp.Forms;

public partial class frmMakeTrax : Window
{
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
        Close();
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        ClickSound();
        int lstTracksSelCount = lstTracks.Items.Cast<ComboboxItem>().Count(i => i.Value == 1);
        if (lstTracksSelCount == 0)
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
        for (int i = 0; i < lstTracks.Items.Count; i++)
        {
            if (lstTracks.itemData(i) == 1)
            {
                string t = lstTracks.itemText(i);
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
        File99.Seek((int)Val("&H" + Hex("&H" + FixHex(txtHeaderOffset.Text, 6))), SeekOrigin.Begin);
        File99.WriteByte((byte)lstTracksSelCount);
        File99.WriteByte(MyNumblocks);
        File99.WriteByte(MyPriority);
        File99.WriteByte(MyReverb);
        File99.Write((int)(Val("&H" + FixHex(txtVoicegroup.Text, 6)) + 0x8000000));
        for (int i = 0; i < lstTracksSelCount; i++)
        {
            File99.Write(Tracks[i] + 0x8000000);
        }
        if (MsgBox(Properties.Resources._2013, vbYesNo) == vbYes)
        {
            Scribe("Updating song table...");
            int p = (int)Val("&H" + Hex("&H" + FixHex(txtHeaderOffset.Text, 6)));
            p += 0x8000000;
            File99.Seek(SongTableEntry, SeekOrigin.Begin);
            File99.Write(p);
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
            lstTracks.AddItem(Path.GetFileName(file));
        }
        SetCaptions(this);
        Title = Properties.Resources._2000;
    }

    private static void InsertTrack(string t, int o)
    {
        File99.Seek(o, SeekOrigin.Begin);
        File98 = new FileStream(t, FileMode.Open);
        do
        {
            byte b = (byte)File98.ReadByte();
            File99.WriteByte(b);
            if (b == 0xB1) break;
            if (b == 0xB2 || b == 0xB3 || b == 0xB5)
            {
                //Scribe(Properties.Resources._2014);
                if (b == 0xB5)
                {
                    b = (byte)File98.ReadByte();
                    File99.WriteByte(b);
                }
                File98.Read(out int p);
                //Scribe(Replace(Replace(Properties.Resources._2015, "$OLD", "0x" + FixHex(p, 8)), "$NEW", "0x" + FixHex(p + o, 6)));
                p = p + 0x8000000 + o;
                File99.Write(p);
            }
            DoEvents();
        } while (!EOF(File98));
        FileClose(File98);
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
