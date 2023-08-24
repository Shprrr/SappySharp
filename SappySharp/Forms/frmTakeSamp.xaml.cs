using System;
using System.IO;
using System.Windows;
using Microsoft.VisualBasic;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static modSappy;
using static SappySharp.VBFileSystem;
using static VBExtension;

namespace SappySharp.Forms;

public partial class frmTakeSamp : Window
{
    public frmTakeSamp() { InitializeComponent(); }

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
    // ___________________
    // |  Sample exporter  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 100% by Kyoufu Kawa, based on Maple.          |
    // | Last update: February 1st, 2005                    |
    // |____________________________________________________|

    // ###########################################################################################

    public int SingleSong = 0;

    struct tInst
    {
        public byte SndType;
        public byte Shit1;
        public byte Shit2;
        public byte Shit3;
        public int WavePtr;
        public int MoreShit;
    }

    readonly bool[] DidWeAlreadyDumpThisOne = new bool[0xFFFFFF];

    private static int ConFreq(int freq)
    {
        for (int k = 1; k <= 10; k++)
        {
            freq /= 2;
        }
        return freq;
    }

    private static void SaveSampleRAW(string Filename, int hdr1, int hdr2, int freq, int loopstart, int Length)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
        byte[] theStuff = new byte[Length];
        FileOpen(98, Filename, OpenMode.Binary);
        File99.Read(theStuff, 0, Length);
        FilePut(98, theStuff);
        FileClose(98);
        return;
    Fucksocks:;
        if (Err().Number == 75)
        {
            MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
            FileClose(98);
            return;
        }
        MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
        // TODO: (NOT SUPPORTED): Resume Next
    }

    private static void SaveSampleWAV(string Filename, int hdr1, int hdr2, int freq, int loopstart, int Length)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
        byte[] theStuff = new byte[Length];
        FileOpen(98, Filename, OpenMode.Binary);
        FilePut(98, (byte)0x52);
        FilePut(98, (byte)0x49);
        FilePut(98, (byte)0x46);
        FilePut(98, (byte)0x46);
        FilePut(98, (byte)0x3E);
        FilePut(98, (byte)0x2B);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x57);
        FilePut(98, (byte)0x41);
        FilePut(98, (byte)0x56);
        FilePut(98, (byte)0x45);
        FilePut(98, (byte)0x66);
        FilePut(98, (byte)0x6D);
        FilePut(98, (byte)0x74);
        FilePut(98, (byte)0x20);
        FilePut(98, (byte)0x10);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x1);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x1);
        FilePut(98, (byte)0x0);
        FilePut(98, ConFreq(freq));
        FilePut(98, ConFreq(freq));
        FilePut(98, (byte)0x1);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x8);
        FilePut(98, (byte)0x0);
        FilePut(98, (byte)0x64);
        FilePut(98, (byte)0x61);
        FilePut(98, (byte)0x74);
        FilePut(98, (byte)0x61);
        FilePut(98, Length + 1);
        File99.Read(theStuff, 0, Length);
        for (int k = 0; k < Length; k++)
        {
            theStuff[k] = (byte)(theStuff[k] ^ 128);
        }
        FilePut(98, theStuff);
        FileClose(98);
        return;
    Fucksocks:;
        if (Err().Number == 75)
        {
            MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
            FileClose(98);
            return;
        }
        MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
        // TODO: (NOT SUPPORTED): Resume Next
    }

    private static void SaveSampleITS(string Filename, int hdr1, int hdr2, int freq, int loopstart, int Length)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
        byte[] theStuff = new byte[Length];
        string IMPS = "IMPS"; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (4)
        string DOSName = new('\0', 12); // TODO: (NOT SUPPORTED) Fixed Length String not supported: (12)
        string SampName = new('\0', 26); // TODO: (NOT SUPPORTED) Fixed Length String not supported: (26)

        FileOpen(98, Filename, OpenMode.Binary);
        File99.Read(theStuff, 0, Length);

        FilePut(98, IMPS, StringIsFixedLength: true);
        FilePut(98, DOSName, StringIsFixedLength: true);
        FilePut(98, (byte)0);
        FilePut(98, (byte)64); // GvL
        if (loopstart != 0)
        {
            FilePut(98, (byte)16);
        }
        else
        {
            FilePut(98, (byte)0);
        }
        FilePut(98, (byte)64); // Vol
        FilePut(98, SampName, StringIsFixedLength: true);
        FilePut(98, (byte)1); // Cvt
        FilePut(98, (byte)0); // DfP
        FilePut(98, Length); // CHECK!
        FilePut(98, loopstart); // CHECK!
        if (loopstart != 0)
        {
            FilePut(98, Length); // CHECK!
        }
        else
        {
            FilePut(98, 0); // CHECK!
        }
        FilePut(98, ConFreq(freq)); // C5Speed
        FilePut(98, 0); // Sustain start
        FilePut(98, 0); // Sustain end
        FilePut(98, 0x50);
        FilePut(98, 0); // Vibe settings

        FilePut(98, theStuff);
        FileClose(98);
        return;
    Fucksocks:;
        if (Err().Number == 75)
        {
            MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
            FileClose(98);
            return;
        }
        MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
        // TODO: (NOT SUPPORTED): Resume Next
    }

    private static void SaveSampleASM(string Filename, int hdr1, int hdr2, int freq, int loopstart, int Length)
    {
        // TODO: (NOT SUPPORTED): On Error GoTo Fucksocks
        byte[] theStuff = new byte[Length];
        string aByteStr;
        File99.Read(theStuff, 0, Length);
        FileOpen(98, Filename, OpenMode.Output);
        PrintLine(98, Properties.Resources._7030);
        PrintLine(98, "#TONE NAME     : " + Path.GetFileNameWithoutExtension(Filename));
        PrintLine(98, "#FREQUENCY     : " + freq);
        PrintLine(98, "#BASE NOTE#    : 60");
        PrintLine(98, "#START ADRESS  : 000000");
        PrintLine(98, "#LOOP ADDRESS  : " + Right("000000" + loopstart, 6));
        PrintLine(98, "#END ADDRESS   : " + Right("000000" + Length, 6));
        if (hdr2 == 0x0)
        {
            aByteStr = "1Shot";
        }
        else if (hdr2 == 0x4000)
        {
            aByteStr = "Fwd";
        }
        else
        {
            aByteStr = "Maple chokes on 0x" + Right("0000" + Hex(hdr2), 4);
        }
        PrintLine(98, "#LOOP MODE     : " + aByteStr);
        PrintLine(98, "#FINE TUNE     : 0");
        PrintLine(98, "#WAVE EXP/COMP : 1");
        PrintLine(98, "#VOL EXP/COMP  : 1");
        PrintLine(98, "");
        PrintLine(98, vbTab + ".section .rodata");
        PrintLine(98, vbTab + ".Global" + vbTab + Path.GetFileNameWithoutExtension(Filename));
        PrintLine(98, vbTab + ".Align" + vbTab + "2");
        PrintLine(98, "");
        PrintLine(98, Path.GetFileNameWithoutExtension(Filename) + ":");
        PrintLine(98, vbTab + ".short" + vbTab + "0x" + Right("0000" + Hex(hdr1), 4));
        PrintLine(98, vbTab + ".short" + vbTab + "0x" + Right("0000" + Hex(hdr2), 4));
        PrintLine(98, vbTab + ".Int" + vbTab + freq);
        PrintLine(98, vbTab + ".Int" + vbTab + loopstart);
        PrintLine(98, vbTab + ".Int" + vbTab + Length);
        PrintLine(98, "");
        for (int j = 0; j < Length; j += 8)
        {
            aByteStr = vbTab + ".byte ";
            // TODO: (NOT SUPPORTED): On Error Resume Next
            for (int k = 0; k <= 7 && j + k < Length; k++)
            {
                aByteStr = aByteStr + "0x" + Right("00" + Hex(theStuff.GetValue(j + k)), 2) + ",";
            }
            // TODO: (NOT SUPPORTED): On Error GoTo 0
            aByteStr = Left(aByteStr, Len(aByteStr) - 1); // chop off trailing ,
            PrintLine(98, aByteStr);
        }
        PrintLine(98, "");
        PrintLine(98, vbTab + ".end");
        FileClose(98);
        return;
    Fucksocks:;
        if (Err().Number == 75)
        {
            MsgBox("Access denied. Make sure \"" + Filename + "\" is not already open.");
            FileClose(98);
            return;
        }
        MsgBox("Runtime error " + Err().Number + vbCrLf + vbCrLf + Err().Description);
        // TODO: (NOT SUPPORTED): Resume Next
    }

    private void DumpVoiceGroup(int anInstrumentLong, int numsamples = 256)
    {
        for (int j = 0; j <= numsamples; j += 1)
        {
            File99.Seek(anInstrumentLong + 12 * j, SeekOrigin.Begin);
            File99.Read(out tInst anInstrument);
            if (anInstrument.SndType == 0)
            {
                if (anInstrument.WavePtr > 0x8000000)
                {
                    if (!DidWeAlreadyDumpThisOne[j])
                    {
                        DidWeAlreadyDumpThisOne[j] = true;
                        Transcribe("Wave instrument found! #" + j);
                        File99.Seek(anInstrument.WavePtr - 0x8000000, SeekOrigin.Begin);
                        File99.Read(out short hdr1);
                        File99.Read(out short hdr2);
                        File99.Read(out int hdr3);
                        File99.Read(out int hdr4); // loop
                        File99.Read(out int hdr5); // length
                        if (hdr5 > 0x10000)
                        {
                            hdr5 = 0x10000;
                            Transcribe("Warning: sample cut off.");
                        }
                        string Blargh = txtNamePat.Text;
                        Blargh = Replace(Blargh, "$I", Trim(Str(j)));
                        Blargh = Replace(Blargh, "$P", Hex(anInstrument.WavePtr - 0x8000000 + 16));
                        Transcribe("Saving as " + Blargh + "...");
                        if (cboSaveAs.SelectedIndex == 0) SaveSampleRAW(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
                        if (cboSaveAs.SelectedIndex == 1) SaveSampleWAV(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
                        if (cboSaveAs.SelectedIndex == 2) SaveSampleITS(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
                        if (cboSaveAs.SelectedIndex == 3) SaveSampleASM(Blargh, hdr1, hdr2, hdr3, hdr4, hdr5);
                        DoEvents();
                    }
                }
                // ElseIf anInstrument.SndType = &H40 Then //Key Split
                // If anInstrument.WavePtr > &H8000000 Then
                // Transcribe "Got a KEYSPLIT!"
                // Transcribe "Forking out..."
                // Transcribe "Note that we're saving only the first FOUR samples!"
                // DumpVoiceGroup anInstrument.WavePtr - &H8000000, 4
                // End If
            }
        }
    }

    private void cboSaveAs_Click(object sender, RoutedEventArgs e) { cboSaveAs_Click(); }
    private void cboSaveAs_Click()
    {
        int dot = InStr(txtNamePat.Text, ".");
        if (dot > 0) txtNamePat.Text = Left(txtNamePat.Text, dot - 1);
        if (cboSaveAs.SelectedIndex == 0) txtNamePat.Text += ".raw";
        if (cboSaveAs.SelectedIndex == 1) txtNamePat.Text += ".wav";
        if (cboSaveAs.SelectedIndex == 2) txtNamePat.Text += ".its";
        if (cboSaveAs.SelectedIndex == 3) txtNamePat.Text += ".s";
        lblFileDesc.Text = cboSaveAs.SelectedIndex switch
        {
            0 => Properties.Resources._7020,
            1 => Properties.Resources._7021,
            2 => Properties.Resources._7022,
            3 => Properties.Resources._7023,
            _ => throw new NotImplementedException()
        };
    }

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        txtNamePat.Text = Replace(txtNamePat.Text, "$p", "$P");
        txtNamePat.Text = Replace(txtNamePat.Text, "$i", "$I");
        int i = 2;
        if (InStr(txtNamePat.Text, "$I") != 0) i--;
        if (InStr(txtNamePat.Text, "$P") != 0) i--;
        if (i == 2)
        {
            MsgBox(Properties.Resources._3011);
            IncessantNoises("TaskFail"); // Bee-owee-owee-oweeeeeohh....
            return;
        }

        ClickSound();

        txtLog.Move(8, 8);
        txtLog.Visibility = Visibility.Visible;
        MousePointer = 11;

        for (int k = 0; k < 0xFFFFFF; k++)
        {
            DidWeAlreadyDumpThisOne[k] = false;
        }
        File99.Seek(SingleSong, SeekOrigin.Begin);
        File99.Read(out int aFillerLong);
        File99.Read(out int anInstrumentLong);
        // TODO: (NOT SUPPORTED): On Error GoTo SillyPointers
        anInstrumentLong -= 0x8000000;
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        Transcribe("Song's instrument pointer is " + Hex(anInstrumentLong));
        if (anInstrumentLong > 0) DumpVoiceGroup(anInstrumentLong);
        Blargh:;

        MousePointer = 0;
        Command1.IsEnabled = false;
        // Command1.FontBold = False
        Command2.Content = "Exit";
        // Command2.FontBold = True
        Command2.IsDefault = true;
        IncessantNoises("TaskComplete");
        return;

    SillyPointers:;
        Transcribe("Silly pointer!");
        goto Blargh;
    }

    private void Command2_Click(object sender, RoutedEventArgs e) { Command2_Click(); }
    private void Command2_Click()
    {
        ClickSound();
        Close();
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        SetCaptions(this);
        Title = Properties.Resources._7000;
        cboSaveAs.AddItem(Properties.Resources._7010);
        cboSaveAs.AddItem(Properties.Resources._7011);
        cboSaveAs.AddItem(Properties.Resources._7012);
        cboSaveAs.AddItem(Properties.Resources._7013);

        cboSaveAs.SelectedIndex = 0;

        if (Properties.Resources._10000 == "<NLPLZ>" || Properties.Resources._10000 == "<SPLZ>" || Properties.Resources._10000 == "<DPLZ>")
        {
            Label9.Visibility = Visibility.Hidden;
            lblFileDesc.Margin = new Thickness(lblFileDesc.Margin.Left, Label9.Margin.Top, lblFileDesc.Margin.Right, lblFileDesc.Margin.Bottom);
            lblFileDesc.Height += 16;
        }
    }

    private void Transcribe(string t)
    {
        if (Len(txtLog.Text) > 32000) txtLog.Text = "Resetting log." + vbCrLf;
        txtLog.Text = txtLog.Text + t + vbCrLf;
        txtLog.SelectionStart = Len(txtLog.Text);
    }

    private void Picture1_Paint(object sender, RoutedEventArgs e)
    {
        DrawSkin(Picture1);
    }
    private void Picture2_Paint(object sender, RoutedEventArgs e)
    {
        DrawSkin(Picture2);
    }
}
