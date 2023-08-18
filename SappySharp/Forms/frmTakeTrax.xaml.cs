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

public partial class frmTakeTrax : Window
{
    public frmTakeTrax() { InitializeComponent(); }

    // ______________
    // |  SAPPY 2006  |
    // |����������������������������������������������������|
    // | Interface code � 2006 by Kyoufu Kawa               |
    // | Player code � 200X by DJ �ouch�                    |
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
    // |  Track exporter  |
    // |����������������������������������������������������|
    // | Code 100% by Kyoufu Kawa, based on Juicer.         |
    // | Last update: February 1st, 2005                    |
    // |____________________________________________________|

    // ###########################################################################################


    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        ClickSound();
        Close();
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        if (lstTracks.Items.Cast<ComboboxItem>().All(i => i.Value == 0))
        {
            MsgBox(Properties.Resources._3005);
            IncessantNoises("TaskFail"); // Bee-owee-owee-oweeeeeohh....
            return;
        }
        if (txtFilename.Text == "")
        {
            MsgBox(Properties.Resources._3006);
            IncessantNoises("TaskFail"); // Bee-owee-owee-oweeeeeohh....
            return;
        }

        txtFilename.Text = Replace(txtFilename.Text, "$t", "$T");
        txtFilename.Text = Replace(txtFilename.Text, "$p", "$P");
        int i = 2;
        if (InStr(txtFilename.Text, "$T") != 0) i--;
        if (InStr(txtFilename.Text, "$P") != 0) i--;
        if (i == 2)
        {
            MsgBox(Properties.Resources._3011);
            IncessantNoises("TaskFail"); // Bee-owee-owee-oweeeeeohh....
            return;
        }

        ClickSound();

        txtLog.Margin = new(txtLog.Margin.Left, 8, txtLog.Margin.Right, txtLog.Margin.Bottom);
        txtLog.Visibility = Visibility.Visible;
        Scribe(Properties.Resources._3007);
        Scribe(new string('�', Len(Properties.Resources._3007)));
        for (i = 0; i < lstTracks.Items.Count; i++)
        {
            if (lstTracks.itemData(i) == 1)
            {
                Scribe(Replace(Properties.Resources._3008, "$TRACK", i.ToString()));
                string t = txtFilename.Text;
                t = Replace(t, "$T", i.ToString());
                t = Replace(t, "$P", lstTracks.itemText(i));
                Scribe(Replace(Properties.Resources._3009, "$FILE", t));
                DumpTrack((int)Val("&H" + FixHex(lstTracks.itemText(i), 6)), t);
            }
            else
            {
                Scribe(Replace(Properties.Resources._3010, "$TRACK", i.ToString()));
            }
        }
        Scribe(Properties.Resources._7);
        Command2.IsEnabled = false;
        // Command1.FontBold = False
        Command1.Content = Properties.Resources._6;
        // Command1.FontBold = True
        Command1.IsDefault = true;
        IncessantNoises("TaskComplete");
        // Unload Me
    }

    private void Scribe(string t)
    {
        txtLog.Text = txtLog.Text + t + vbCrLf;
        txtLog.SelectionStart = Len(txtLog.Text);
    }

    private static void DumpTrack(int o, string t)
    {
        FileOpen(98, t, OpenMode.Binary);
        File99.Seek(o, System.IO.SeekOrigin.Begin);
        do
        {
            File99.Read(out byte b);
            FilePut(98, b);
            if (b == 0xB1) // fine
            {
                // Scribe(Properties.Resources._3012);
                break;
            }
            if (b == 0xB2 || b == 0xB3 || b == 0xB5) // goto/patt/rept
            {
                if (b == 0xB5)
                {
                    File99.Read(out b); // num reps
                    FilePut(98, b);
                }

                File99.Read(out int p);
                p = p - 0x8000000 - o;
                FilePut(98, p);
            }
            DoEvents();
        } while (EOF(98));
        FileClose(98);
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        SetCaptions(this);
        Title = Properties.Resources._3000;
    }

    private void Picture1_Paint(object sender, RoutedEventArgs e)
    {
        DrawSkin(Picture1);
    }
}
