using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using PortControlLibrary;
using SappySharp.Classes;
using SappySharp.UserControls;
using SSubTimer6;
using stdole;
using vbalExplorerBarLib6;
using static System.Math;
using static mColorUtils;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static MidiLib;
using static modSappy;
using static mTrace;
using static SapPlayer;
using static SappySharp.Classes.clsSappyDecoder;
using static SappySharp.Classes.cVBALImageList;
using static SappySharp.Classes.NoteInfo;
using static SappySharp.VBFileSystem;
using static VBExtension;

namespace SappySharp.Forms;

public partial class frmSappy : Window, ISubclass
{
    public static frmSappy instance { get; set; }
    public void Unload() { Close(); instance = null; }
    public frmSappy()
    {
        instance = this;
        InitializeComponent();
        timPlay.IsEnabled = false;
        timPlay.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        timPlay.Tick += timPlay_Timer;
    }

    public List<Button> cmdSpeed { get => VBExtension.controlArray<Button>(this, "cmdSpeed"); }

    public DispatcherTimer timPlay { get; set; } = new DispatcherTimer();

    public List<MenuItem> mnuOutput { get => VBExtension.controlArray<MenuItem>(this, "mnuOutput"); }

    public List<ChannelViewer> cvwChannel => cvwChannels.Children.OfType<ChannelViewer>().ToList();

    public ExplorerBarCtl ebr { get; private set; }

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
    // |  Main interface  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 99% by Kyoufu Kawa.                           |
    // | Last update: July 22nd, 2006                       |
    // |____________________________________________________|

    // ###########################################################################################


    [StructLayout(LayoutKind.Sequential)]
    class RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct tSongHeader
    {
        public byte NumTracks;
        public byte NumBlocks;
        public byte Priority;
        public byte Reverb;
        public int VoiceGroup;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I4)]
        public int[] Tracks = new int[32];

        public tSongHeader()
        {
        }
    }

    private class tPlaylist
    {
        public int NumSongs;
        public string[] SongName = new string[1024];
        public int[] SongIcon = new int[1024];
        public int[] SongNo = new int[1024];
    }

    private static tPlaylist[] playlist = new tPlaylist[256];
    private static int NumPLs = 0;
    private static int[] MidiMap = new int[128];
    private static int[] MidiMapTrans = new int[128];
    private static int[] DrumMap = new int[128];
    private static int[] BleedingEars = new int[128];
    private static int BECnt = 0;
    public XmlElement MidiMapNode = null;
    public XmlElement MidiMapsDaddy = null;

    private static tSongHeader SongHead;
    private static int SongHeadOrg = 0;

    public int SongTbl = 0;
    public string gamecode = "";
    public string myFile = "";
    public string xfile = "";

    private static bool DontLoadDude = false;
    private static int[] TaskMenus = new int[16];
    private Dictionary<object, string> _helpTexts = new();

    public cVBALImageList imlImages = null;
    public cVBALImageList imlStatusbar = null;
    public XmlDocument x = new();
    public XmlElement rootNode = null;

    public const int WM_SIZING = 0x214;
    public static int mywidth = 0;

    public const int WM_APPCOMMAND = 0x319;
    public const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
    public const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
    public const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
    public const int APPCOMMAND_MEDIA_STOP = 13;
    public const int APPCOMMAND_VOLUME_DOWN = 9;
    public const int APPCOMMAND_VOLUME_UP = 10;

    public const int WM_MOUSEWHEEL = 0x20A;

    public clsSappyDecoder SappyDecoder = null;
    public string[] DutyCycleWave = new string[4];
    public int[] DutyCycle = new int[4];
    public bool mm = false;

    private static bool Playing = false;
    private static int TotalMinutes = 0;
    private static int TotalSeconds = 0;
    private static int loopsToGo = 0;
    private static string songinfo = "";
    private static string justthesongname = "";
    private static int WantToRecord = 0;
    private static string WantToRecordTo = "";
    private static int FullWidth = 0;
    private static int ClassicWidth = 0;

    [LibraryImport("winmm.dll")]
    private static partial int midiOutGetNumDevs();

    // Private WithEvents HookedDialog As cCommonDialog
    // Private m_bInIDE As Boolean

    private void cbxSongs_Change(object sender, SelectionChangedEventArgs e) { cbxSongs_Change(); }
    private void cbxSongs_Change()
    {
        if (DontLoadDude) return;
        if (cbxSongs.ItemData(cbxSongs.SelectedIndex) == 9999) return; // don't try to load playlists
        txtSong.Text = cbxSongs.ItemData(cbxSongs.SelectedIndex).ToString();
        LoadSong(int.Parse(txtSong.Text));
    }

    private void chkMute_Click(object sender, RoutedEventArgs e) { chkMute_Click(); }
    private void chkMute_Click()
    {
        if ((string)chkMute.Tag == "^_^") return;
        // If Playing = True Then
        //   For i = 0 To SappyDecoder.SappyChannels.count - 1
        //     cvwChannel[i].mute = chkMute.muteChanged
        //   Next i
        // Else
        chkMute.Tag = "O.O";
        for (int i = 0; i <= cvwChannel.Count - 1; i++)
        {
            cvwChannel[i].mute = chkMute.IsChecked.GetValueOrDefault() ? 1 : 0;
        }
        chkMute.Tag = "-_-";
        // End If
    }

    private void cmdPlay_Click(object sender, RoutedEventArgs e) { cmdPlay_Click(); }
    private void cmdPlay_Click()
    {
        cmdStop_Click();
        MousePointer = 11;
        SappyDecoder.outputtype = mnuOutput[1].IsChecked ? SongOutputTypes.sotWave : SongOutputTypes.sotMIDI;
        SappyDecoder.ClearMidiPatchMap();
        for (int i = 0; i < 128; i++)
        {
            // SappyDecoder.MidiMap(i) = MidiMap(i)
            SappyDecoder.SetMidiPatchMap(i, MidiMap[i], MidiMapTrans[i]);
            SappyDecoder.SetMidiDrumMap(i, DrumMap[i]);
        }
        for (int i = 0; i <= BECnt; i++)
        {
            SappyDecoder.AddEarPiercer(BleedingEars[i]);
        }
        if (mnuGBMode.IsChecked)
        {
            for (int i = 0; i <= 126; i++)
            {
                // SappyDecoder.MidiMap(i) = IIf(i Mod 2 = 1, 80, 81) '80
                SappyDecoder.SetMidiPatchMap(i, i % 2 == 1 ? 80 : 81, 0);
            }
        }

        linProgress.X2 = -1;

        SappyDecoder.GlobalVolume = (int)(VolumeSlider1.Value * 5.1m);
        SappyDecoder.PlaySong(myFile, int.Parse(txtSong.Text), SongTbl, WantToRecord != 0, WantToRecordTo);

        WantToRecord = 2;

        simple.Text = "";

        for (int i = 0; i <= SappyDecoder.SappyChannels.count - 1; i++)
        {
            SappyDecoder.SappyChannels[i + 1].mute = cvwChannel[i].mute != 1;
            cvwChannel[i].Note = "";
            cvwChannel[i].pan = 0;
            cvwChannel[i].volume = "0";
            cvwChannel[i].patch = "0";
        }

        lblSpeed.Content = SappyDecoder.Tempo;

        TotalMinutes = 0;
        TotalSeconds = 0;
        timPlay.IsEnabled = true;

        loopsToGo = GetSettingI("Song Repeats");
        Playing = true;
        cmdPlay.setImage(imlImages.ItemPicture(20).ToImageSource());

        if (GetSettingI("mIRC Now Playing") != 0)
        {
            File43 = File.OpenWrite(AppContext.BaseDirectory + "\\sappy.stt");
            File43.Write(songinfo);
            FileClose(File43);
        }

        if (GetSettingI("MSN Now Playing") != 0)
        {
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            TellMSN(justthesongname + (SappyDecoder.outputtype == SongOutputTypes.sotMIDI ? " (midi)" : ""), "Sappy " + assemblyName.Version.Major + "." + assemblyName.Version.Minor, ebr.Bars["Info"].Items["Game"].Text + " (" + gamecode + ")"); // ebr.Bars["Info"].Items["Creator"].Text
        }

        mnuOutput[0].IsEnabled = false;
        mnuOutput[1].IsEnabled = false;
        mnuGBMode.IsEnabled = false;
        mnuSelectMIDI.IsEnabled = false;
        mnuMidiMap.IsEnabled = false;
        MousePointer = 0;
    }

    private void cmdNextSong_Click(object sender, RoutedEventArgs e) { cmdNextSong_Click(); }
    private void cmdNextSong_Click()
    {
        if (mnuSeekPlaylist.IsChecked)
        {
            if (cbxSongs.Items.Count == 1) return;
            if (cbxSongs.SelectedIndex == cbxSongs.Items.Count - 1)
            {
                cbxSongs.SelectedIndex = 1;
            }
            else
            {
                cbxSongs.SelectedIndex++;
                do
                {
                    if (cbxSongs.ItemData(cbxSongs.SelectedIndex) == 9999)
                    {
                        if (cbxSongs.SelectedIndex == cbxSongs.Items.Count - 1)
                        {
                            cbxSongs.SelectedIndex = 1;
                        }
                        else
                        {
                            cbxSongs.SelectedIndex++;
                        }
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }
            cbxSongs_Change();
        }
        else
        {
            if (txtSong.Text == "1024") return;
            txtSong.Text = (int.Parse(txtSong.Text) + 1).ToString();
            LoadSong(int.Parse(txtSong.Text));
        }
    }

    private void cmdPrevSong_Click(object sender, RoutedEventArgs e) { cmdPrevSong_Click(); }
    private void cmdPrevSong_Click()
    {
        if (mnuSeekPlaylist.IsChecked)
        {
            if (cbxSongs.Items.Count == 1) return;
            if (cbxSongs.SelectedIndex == 0)
            {
                cbxSongs.SelectedIndex = cbxSongs.Items.Count - 1;
            }
            else
            {
                cbxSongs.SelectedIndex--;
                do
                {
                    if (cbxSongs.ItemData(cbxSongs.SelectedIndex) == 9999)
                    {
                        if (cbxSongs.SelectedIndex == 0)
                        {
                            cbxSongs.SelectedIndex = cbxSongs.Items.Count - 1;
                        }
                        else
                        {
                            cbxSongs.SelectedIndex--;
                        }
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }
            cbxSongs_Change();
        }
        else
        {
            if (txtSong.Text == "0") return; // 181205 update: Someguy complained about lack of Track Zero in Sonic
            txtSong.Text = (int.Parse(txtSong.Text) - 1).ToString();
            LoadSong(int.Parse(txtSong.Text));
        }
    }

    private void cmdSpeed_Click(object sender, RoutedEventArgs e) { cmdSpeed_Click(cmdSpeed.IndexOf((Button)sender)); }
    private void cmdSpeed_Click(int Index)
    {
        if (Index == 0) SappyDecoder.Tempo = Int(SappyDecoder.Tempo / 2);
        if (Index == 1) SappyDecoder.Tempo = Int(SappyDecoder.Tempo * 2);
        lblSpeed.Content = SappyDecoder.Tempo;
    }

    private void cmdStop_Click(object sender, RoutedEventArgs e) { cmdStop_Click(); }
    private void cmdStop_Click()
    {
        if (Playing)
        {
            Playing = false;
            SappyDecoder.StopSong();
            linProgress.X2 = -1;
        }
        if (mnuAutovance.IsChecked) cmdPrevSong_Click();

        if (WantToRecord == 2) WantToRecord = 0;

        Playing = false;
        timPlay.IsEnabled = false;
        cmdPlay.setImage(imlImages.ItemPicture(19).ToImageSource());
        for (int i = 0; i <= cvwChannel.Count - 1; i++)
        {
            cvwChannel[i].volume = "0";
            cvwChannel[i].pan = 0;
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next
        if (GetSetting("mIRC Now Playing") != null)
        {
            File44 = File.OpenWrite(AppContext.BaseDirectory + "\\sappy.stt");
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            File44.Write(assemblyName.Version.Major + "." + assemblyName.Version.Minor + " | | | | Not running | ");
            FileClose(File44);
        }

        if (GetSettingI("MSN Now Playing") != 0) ShutMSN();
        // TODO: (NOT SUPPORTED): On Error GoTo 0

        mnuOutput[0].IsEnabled = true;
        mnuOutput[1].IsEnabled = true;
        mnuSelectMIDI.IsEnabled = true;
        mnuMidiMap.IsEnabled = true;
        mnuGBMode.IsEnabled = mnuOutput[0].IsChecked;
    }

    private void cPop_Click(ref int ItemNumber)
    {
        cExplorerBarItem itm = null;
        for (int i = 1; i <= 16; i++)
        {
            if (ItemNumber == TaskMenus[i])
            {
                //itm = ebr.Bars["Tasks"].Items[cPop.get_MenuKey(ItemNumber)]; // See SubMenuTasks_Click
                ebr_ItemClick(ref itm);
            }
        }
    }

    private void menu_GotFocus(object sender, RoutedEventArgs e)
    {
        simple.Text = _helpTexts[sender];
    }

    private void menu_LostFocus(object sender, RoutedEventArgs e)
    {
        simple.Text = "";
    }

    private void cvwChannel_MuteChanged(int Index)
    {
        // If Playing = False Then Exit Sub
        // TODO: (NOT SUPPORTED): On Error Resume Next
        if (SappyDecoder.SappyChannels.count < 1) goto FlickIt; // Exit Sub
        SappyDecoder.SappyChannels[Index + 1].mute = cvwChannel[Index].mute != 1;

    FlickIt:;
        if ((string)chkMute.Tag == "O.O") return;
        int j = 0;
        for (int i = 0; i <= SappyDecoder.SappyChannels.count - 1; i++)
        {
            if (cvwChannel[i].mute == 1) j++;
        }
        chkMute.Tag = "^_^";
        if (j == SappyDecoder.SappyChannels.count)
        {
            chkMute.IsChecked = true;
        }
        else if (j == 0)
        {
            chkMute.IsChecked = false;
        }
        else
        {
            chkMute.IsChecked = null;
        }
        chkMute.Tag = "-_-";
    }

    private void ebr_BarClick(ref cExplorerBar bar)
    {
        WriteSettingI("Bar " + bar.Index + " state", (int)bar.State);
    }

    private class cExplorerBarItemTemp : cExplorerBarItem
    {
        public IFont get_Font() => throw new NotImplementedException();
        public void let_Font(ref IFont value) => throw new NotImplementedException();
        public void set_Font(ref IFont value) => throw new NotImplementedException();
        public dynamic get_Control() => throw new NotImplementedException();
        public void let_Control(ref object value) => throw new NotImplementedException();
        public void set_Control(ref object value) => throw new NotImplementedException();
        public void EnsureVisible() => throw new NotImplementedException();

        public EExplorerBarItemTypes ItemType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int IconIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ToolTipText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Bold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SpacingAfter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint TextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint TextColorOver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CanClick { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Key { get; set; }

        public string Tag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ItemData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    private void SubMenuTasks_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;
        cExplorerBarItem item = new cExplorerBarItemTemp { Key = menuItem.Name };
        ebr_ItemClick(ref item);
    }
    private void ebr_ItemClick(ref cExplorerBarItem itm)
    {
        int i = 0;
        if (itm.Key == "taketrax")
        {
            frmTakeTrax instance = new();
            for (i = 0; i < SongHead.NumTracks; i++)
            {
                instance.lstTracks.AddItem("0x" + FixHex(SongHead.Tracks[i], 6));
            }
            instance.ShowDialog();
        }
        if (itm.Key == "maketrax")
        {
            frmMakeTrax instance = new();
            instance.txtHeaderOffset.Text = "0x" + FixHex(SongHeadOrg, 6);
            instance.txtTrack.Text = "0x" + FixHex(SongHead.Tracks[i], 6);
            instance.MyNumblocks = SongHead.NumBlocks;
            instance.MyPriority = SongHead.Priority;
            instance.MyReverb = SongHead.Reverb;
            instance.txtVoicegroup.Text = "0x" + FixHex(SongHead.VoiceGroup, 6);
            instance.SongTableEntry = SongTbl + int.Parse(txtSong.Text) * 8;
            instance.ShowDialog();
        }
        if (itm.Key == "takesamp")
        {
            frmTakeSamp instance = new()
            {
                SingleSong = SongHeadOrg
            };
            instance.ShowDialog();
        }
        if (itm.Key == "codetrax")
        {
            frmAssembler instance = new()
            {
                SongTableEntry = SongTbl + int.Parse(txtSong.Text) * 8
            };
            instance.txtVoicegroup.Text = "0x" + FixHex(SongHead.VoiceGroup, 6);
            instance.ShowDialog();
        }
        if (itm.Key == "makemidi") PrepareRecording();

        if (itm.Key == "Game")
        {
            string s = ebr.Bars["Info"].Items["Game"].Text;
            s = InputBox(Properties.Resources._210, DefaultResponse: s);
            if (s == "") goto hell;
            ebr.Bars["Info"].Items["Game"].Text = s;
            SaveNewRomHeader("name", s);
        }
        if (itm.Key == "Creator")
        {
            string s = ebr.Bars["Info"].Items["Creator"].Text;
            if (s == Properties.Resources._63) s = ""; else s = Mid(s, Len(Properties.Resources._65) + 1);
            s = InputBox(Properties.Resources._211, DefaultResponse: s);
            if (s == "") goto hell;
            ebr.Bars["Info"].Items["Creator"].Text = Properties.Resources._65 + s;
            SaveNewRomHeader("creator", s);
        }
        if (itm.Key == "Tagger")
        {
            string s = ebr.Bars["Info"].Items["Tagger"].Text;
            if (s == Properties.Resources._64) s = ""; else s = Mid(s, Len(Properties.Resources._66) + 1);
            s = InputBox(Properties.Resources._212, DefaultResponse: s);
            if (s == "") goto hell;
            ebr.Bars["Info"].Items["Tagger"].Text = Properties.Resources._66 + s;
            SaveNewRomHeader("tagger", s);
        }
    hell:;
        cmdPlay.SetFocus();
    }

    internal class AxHostConverter : System.Windows.Forms.AxHost
    {
        private AxHostConverter() : base("") { }

        public static IPictureDisp ImageToPictureDisp(System.Drawing.Image image)
        {
            return (IPictureDisp)GetIPictureDispFromPicture(image);
        }

        public static System.Drawing.Image PictureToImage(IPicture picture)
        {
            return GetPictureFromIPicture(picture);
        }

        public static System.Drawing.Icon PictureToIcon(IPicture picture)
        {
            return System.Drawing.Icon.FromHandle(picture.Handle);
        }
    }
    private void Form_Load(object sender, RoutedEventArgs e)
    {
        // To call an OCX control.
        WindowsFormsHost ebrHost = new();
        ebr = new ExplorerBarCtl();
        ebr.BarClick += ebr_BarClick;
        ebr.ItemClick += ebr_ItemClick;
        ebrHost.Child = ebr;
        ebrContainer.Children.Add(ebrHost);

        int i = 0;
        string regset = "";

        Trace("frmSappy/Form_Load()");
        Trace("- Set icon");
        Icon = Properties.Resources.APP.ToImageSource();

        // Trace "- DoomClock"
        // Dim y As Integer
        // Dim D As Integer
        // Dim m As Integer
        // Dim thresh As Integer
        // D = Val(Format(Now, "dd"))
        // m = Val(Format(Now, "mm"))
        // y = Val(Format(Now, "yyyy"))
        // thresh = 2007
        // If (y > thresh) Or (y = thresh And D >= 25 And m = 12) Then
        //   Trace "- DoomClock triggered."
        //   ShellExecute 0, vbNullString, "http://helmetedrodent.kickassgamers.com", vbNullString, "", 1
        //   End
        // End If

        Trace("- Set dimensions");
        cbxSongs.Height = 330;
        mywidth = (int)Width; // remember for wmSize subclass

        Trace("- Create duty cycle waves");
        DutyCycleWave[0] = new string(VBExtension.Chr(0), 14) + new string(VBExtension.Chr(255), 2);
        DutyCycleWave[1] = new string(VBExtension.Chr(0), 12) + new string(VBExtension.Chr(255), 4);
        DutyCycleWave[2] = new string(VBExtension.Chr(0), 16) + new string(VBExtension.Chr(255), 16);
        DutyCycleWave[3] = new string(VBExtension.Chr(0), 4) + new string(VBExtension.Chr(255), 12);

        Trace("- Create Sappy engine");
        SappyDecoder = new clsSappyDecoder();
        SappyDecoder.Beat += SappyDecoder_Beat;
        SappyDecoder.ChangedTempo += SappyDecoder_ChangedTempo;
        SappyDecoder.Loading += SappyDecoder_Loading;
        SappyDecoder.SongFinish += SappyDecoder_SongFinish;
        SappyDecoder.SongLoop += SappyDecoder_SongLoop;
        SappyDecoder.UpdateDisplay += SappyDecoder_UpdateDisplay;

        Trace("- Get settings");
        mnuSeekPlaylist.IsChecked = GetSettingI("Seek by Playlist") == 1;
        mnuAutovance.IsChecked = GetSettingI("AutoAdvance") == 1;
        regset = GetSetting("Driver");
        if (Trim(regset) == "") regset = "FMOD";
        mnuOutput[0].IsChecked = regset == "MIDI";
        mnuOutput[1].IsChecked = regset == "FMOD";
        mnuGBMode.IsChecked = GetSettingI("MIDI in GB Mode") == 1;
        i = GetSettingI("Window Height");
        if (i > 0) Height = i;
        WantedMidiDevice = GetSettingI("MIDI Device");
        i = GetSettingI("FMOD Volume");
        if (i > 0) VolumeSlider1.Value = i;

        FullWidth = (int)Width;
        if (Properties.Resources._10000 == "<NLPLZ>" || Properties.Resources._10000 == "<SPLZ>" || Properties.Resources._10000 == "<DPLZ>")
        {
            FullWidth += 16 * Screen.TwipsPerPixelX;
            ebrContainer.Width += 16 * Screen.TwipsPerPixelX;
        }
        ClassicWidth = (int)(FullWidth - ebrContainer.Width - 10);
        HandleClassicMode();

        xfile = GetSetting("XML File");
        if (Trim(xfile) == "") xfile = "sappy.xml";
        ChDir(AppContext.BaseDirectory);
        if (Dir(AppContext.BaseDirectory + "\\" + xfile) == "")
        {
            Trace("- Oh shit...");
            if (MsgBox(Replace(Properties.Resources._204, "$XML", xfile), vbYesNo) == vbYes)
            {
                File4 = File.OpenWrite(xfile);
                File4.Write("<sappy xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://helmetedrodent.kickassgamers.com/sappy.xsd\">");
                File4.Write("</sappy>");
                FileClose(File4);
            }
            else
            {
                End();
            }
        }
        xfile = AppContext.BaseDirectory + "\\" + xfile;

        Trace("- Localize");
        SetCaptions(this);
        Title = Properties.Resources._0;
        linProgress.X2 = -1;

        Trace("- Attach messages");
        GSubclass gSubclass = new();
        gSubclass.AttachMessage(this, (int)this.hWnd(), WM_SIZING);
        gSubclass.AttachMessage(this, (int)this.hWnd(), WM_APPCOMMAND);
        gSubclass.AttachMessage(this, (int)this.hWnd(), WM_MOUSEWHEEL);

        Trace("- Load tool pics");
        StdPicture stdPic = new();
        imlImages = new cVBALImageList
        {
            OwnerHDC = (int)this.hWnd(),
            ColourDepth = eilColourDepth.ILC_COLOR8,
            IconSizeX = 16,
            IconSizeY = 16
        }; // vbalImages
        imlImages.Create();

        stdPic = (StdPicture)AxHostConverter.ImageToPictureDisp(Properties.Resources.TOOLICONS);
        imlImages.AddFromHandle(stdPic.Handle, ImageTypes.IMAGE_BITMAP, lBackColor: 0xFF00FF);
        //cmdPrevSong.ImageList = imlImages.hIml;
        //cmdNextSong.ImageList = imlImages.hIml;
        //cmdStop.ImageList = imlImages.hIml;
        //cmdPlay.ImageList = imlImages.hIml;
        //cmdPrevSong.Icon = 16;
        //cmdNextSong.Icon = 17;
        //cmdStop.Icon = 18;
        //cmdPlay.Icon = 19;
        cmdPrevSong.setImage(imlImages.ItemPicture(16).ToImageSource());
        cmdNextSong.setImage(imlImages.ItemPicture(17).ToImageSource());
        cmdStop.setImage(imlImages.ItemPicture(18).ToImageSource());
        cmdPlay.setImage(imlImages.ItemPicture(19).ToImageSource());
        Trace("- Load status pics");
        imlStatusbar = new cVBALImageList
        {
            OwnerHDC = (int)this.hWnd(),
            ColourDepth = eilColourDepth.ILC_COLOR8,
            IconSizeX = 16,
            IconSizeY = 16
        }; // vbalStatusBar
        imlStatusbar.Create();

        stdPic = (StdPicture)AxHostConverter.ImageToPictureDisp(Properties.Resources.STATUSICONS);
        imlStatusbar.AddFromHandle(stdPic.Handle, ImageTypes.IMAGE_BITMAP, lBackColor: 0xFF00FF);
        FixStatusBar();

        Trace("- Set menu icons and help");
        mnuFileOpen.Icon = new Image { Source = imlImages.ItemPicture(1).ToImageSource() };
        mnuOutput_0.Icon = new Image { Source = imlImages.ItemPicture(3).ToImageSource() };
        mnuOutput_1.Icon = new Image { Source = imlImages.ItemPicture(4).ToImageSource() };
        mnuSeekPlaylist.Icon = new Image { Source = imlImages.ItemPicture(5).ToImageSource() };
        mnuAutovance.Icon = new Image { Source = imlImages.ItemPicture(6).ToImageSource() };
        mnuGBMode.Icon = new Image { Source = imlImages.ItemPicture(7).ToImageSource() };
        mnuHelpHelp.Icon = new Image { Source = imlImages.ItemPicture(8).ToImageSource() };
        mnuHelpOnline.Icon = new Image { Source = imlImages.ItemPicture(9).ToImageSource() };
        mnuImportLST.Icon = new Image { Source = imlImages.ItemPicture(21).ToImageSource() };
        mnuSelectMIDI.Icon = new Image { Source = imlImages.ItemPicture(23).ToImageSource() };
        mnuSettings.Icon = new Image { Source = imlImages.ItemPicture(25).ToImageSource() };
        mnuMidiMap.Icon = new Image { Source = imlImages.ItemPicture(24).ToImageSource() };
        _helpTexts.Add(mnuFileOpen, Properties.Resources._70);
        _helpTexts.Add(mnuFileExit, Properties.Resources._71);
        _helpTexts.Add(mnuOutput_0, Properties.Resources._72);
        _helpTexts.Add(mnuOutput_1, Properties.Resources._73);
        _helpTexts.Add(mnuHelpAbout, Properties.Resources._75);
        _helpTexts.Add(mnuHelpOnline, Properties.Resources._76);

        // Not setting any images for Japanese systems until further notice.
        if (Properties.Resources._10000 != "<JAPPLZ>")
        {
            //cbxSongs.ImageList = imlImages; //TODO: Add icon to Items
        }

        Trace("- Skinning");
        decimal hue = 0;
        decimal sat = 0;
        int skinno = 0;
        skinno = GetSettingI("Skin");
        // If regset <> "" Then skinno = Val(regset) Else skinno = 0
        System.Drawing.Bitmap skin = skinno switch
        {
            0 => Properties.Resources.Skin100,
            1 => Properties.Resources.Skin101,
            2 => Properties.Resources.Skin102,
            _ => throw new NotImplementedException()
        };
        picSkin.Source = ConvertBitmap(skin);
        regset = GetSetting("Skin Hue");
        if (regset != null) hue = (decimal)Val(Replace(regset, ",", ".")); else hue = 3.5m;
        regset = GetSetting("Skin Saturation");
        if (regset != null) sat = (decimal)Val(Replace(regset, ",", ".")); else sat = 0.4m;
        picSkin.Source = Colorize((BitmapSource)picSkin.Source, hue, sat);

        // Dim woogy As Control
        // For Each woogy In Me.Controls
        //   If TypeName(woogy) = "CommandButton" Then
        //     woogy.MousePointer = 99
        //     woogy.MouseIcon = LoadResPicture("HAND", vbResCursor)
        //   End If
        // Next woogy

        Trace("- Set up task bar");
        Color color = GetPixelColor((BitmapSource)picSkin.Source, 6, 16);
        ebr.BackColorStart = (uint)RGB(color.R, color.G, color.B);
        color = GetPixelColor((BitmapSource)picSkin.Source, 6, 32);
        ebr.BackColorEnd = (uint)RGB(color.R, color.G, color.B);
        ebr.UseExplorerStyle = GetSettingI("Force Nice Bar") == 0;
        //TODO: Commenting these COM instructions because it didn't work.
        //ebr.Bars.Add("Tasks", Properties.Resources._50);
        //hIml = imlImages.hIml;
        //ebr.set_ImageList(ref hIml);
        //ebr.Bars["Tasks"].CanExpand = false;
        //ebr.Bars["Tasks"].State = EExplorerBarStates.eBarCollapsed;
        //Trace("- Add tasks");
        //ebr.Bars["Tasks"].Items.Add("taketrax", Properties.Resources._52, 9);
        //ebr.Bars["Tasks"].Items.Add("maketrax", Properties.Resources._53, 10);
        //ebr.Bars["Tasks"].Items.Add("takesamp", Properties.Resources._54, 11);
        //ebr.Bars["Tasks"].Items.Add("codetrax", Properties.Resources._55, 12);
        //ebr.Bars["Tasks"].Items.Add("makemidi", Properties.Resources._51, 2);
        //ebr.Bars["Tasks"].Items["taketrax"].ToolTipText = Properties.Resources._81;
        //ebr.Bars["Tasks"].Items["maketrax"].ToolTipText = Properties.Resources._82;
        //ebr.Bars["Tasks"].Items["takesamp"].ToolTipText = Properties.Resources._83;
        //ebr.Bars["Tasks"].Items["codetrax"].ToolTipText = Properties.Resources._84;
        //ebr.Bars["Tasks"].Items["makemidi"].ToolTipText = Properties.Resources._80;
        //for (i = 1; i <= ebr.Bars["Tasks"].Items.Count; i += 1)
        //{
        //    TaskMenus[i] = cPop.AddItem(ebr.Bars["Tasks"].Items[i].Text, ebr.Bars["Tasks"].Items[i].Key, ebr.Bars["Tasks"].Items[i].ToolTipText, lParentIndex: cPop.get_MenuIndex("mnuTasks"), lIconIndex: ebr.Bars["Tasks"].Items[i].IconIndex, bChecked: false, bEnabled: false);
        //}
        //ebr.Bars["Tasks"].State = (EExplorerBarStates)GetSettingI("Bar " + ebr.Bars["Tasks"].Index + " state");
        //Trace("- Set up info bar");
        //ebr.Bars.Add("Info", Properties.Resources._60);
        //ebr.Bars["Info"].CanExpand = false;
        //ebr.Bars["Info"].State = EExplorerBarStates.eBarCollapsed;
        //ebr.Bars["Info"].Items.Add("Game", Properties.Resources._61, IconIndex: 0);
        //ebr.Bars["Info"].Items.Add("Code", Properties.Resources._62, IconIndex: 1);
        //ebr.Bars["Info"].Items.Add("Creator", Properties.Resources._63, IconIndex: 0);
        //ebr.Bars["Info"].Items.Add("Tagger", Properties.Resources._64, IconIndex: 0);
        //ebr.Bars["Info"].Items.Add("SongTbl", "0x000000", IconIndex: 1);
        //ebr.Bars["Info"].Items.Add("Screen", IconIndex: 2);
        //ebr.Bars["Info"].Items["Game"].Bold = true;
        //ebr.Bars["Info"].Items["SongTbl"].SpacingAfter = 8;
        //object newControl = picScreenshot.Source;
        //ebr.Bars["Info"].Items["Screen"].let_Control(ref newControl);
        //ebr.Bars["Info"].Items["Screen"].SpacingAfter = 6;
        //ebr.Bars["Info"].State = (EExplorerBarStates)GetSettingI("Bar " + ebr.Bars["Info"].Index + " state");
        //TODO: Temporary because COM code is commented.
        mnuTasks.Items.Add(new MenuItem { Name = "taketrax", Header = Properties.Resources._52 });
        mnuTasks.Items.Add(new MenuItem { Name = "maketrax", Header = Properties.Resources._53 });
        mnuTasks.Items.Add(new MenuItem { Name = "takesamp", Header = Properties.Resources._54 });
        mnuTasks.Items.Add(new MenuItem { Name = "codetrax", Header = Properties.Resources._55 });
        mnuTasks.Items.Add(new MenuItem { Name = "makemidi", Header = Properties.Resources._51 });
        foreach (MenuItem item in mnuTasks.Items) item.Click += SubMenuTasks_Click;

        Trace("- Create channel views");
        cvwChannelTemplate.MuteChanged += (sender, e) => cvwChannel_MuteChanged(0);
        for (i = 1; i < 32; i++)
        {
            cvwChannels.Children.Add(new ChannelViewer());
            cvwChannel[i].MuteChanged += (sender, e) => cvwChannel_MuteChanged(i);
            cvwChannel[i].mute = 1;
            cvwChannel[i].volume = "0";
            cvwChannel[i].pan = 0;
            cvwChannel[i].Visibility = Visibility.Hidden;
        }

        // Trace "- Load XML"
        // Set x = New DOMDocument26
        // x.Load xfile
        // x.preserveWhiteSpace = True
        // If x.parseError.errorcode <> 0 Then
        //   MsgBox Replace(Replace(LoadResString(208), "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText)
        //   End
        // End If
        // For Each blah In x.childNodes
        //   If blah.baseName = "sappy" Then
        //     Set rootNode = blah
        //     Exit For
        //   End If
        // Next

        Trace("- Finalizing");
        // VolumeSlider1.SetValue 50

        if (midiOutGetNumDevs() == 0) // got no midi
        {
            mnuOutput[0].IsEnabled = false;
            mnuOutput[1].IsChecked = true;
        }

        picScreenshot.Source = ConvertBitmap(Properties.Resources.NOPIC);

        // TODO: (NOT SUPPORTED): On Error Resume Next
        Trace("- Handle startup rom");
        i = GetSettingI("Reload ROM");
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        if (Command() != "")
        {
            myFile = Replace(Command(), "\"", "");
            mnuFileOpen.Tag = "BrotherMaynard";
            Trace("- Gonna open (argument)");
            mnuFileOpen_Click();
            return;
        }

        if (i == 1)
        {
            myFile = Replace(GetSetting("Last ROM"), "\"", "");
            if (Dir(myFile) == "") myFile = "";
            if (myFile != "")
            {
                mnuFileOpen.Tag = "BrotherMaynard";
                Trace("- Gonna open (reloader)");
                mnuFileOpen_Click();
            }
        }

        Trace("- Done loadin'");
        // frmMidiMapper.Show 1, Me
        // frmSelectMidiOut.Show 1
    }

    private void Form_Resize(object sender, SizeChangedEventArgs e)
    {
        if (WindowState == WindowState.Minimized) return;
        picChannels.Height = mainGrid.ActualHeight - picChannels.Margin.Top;
    }

    private void Form_Unload(object sender, RoutedEventArgs e)
    {
        int cancel = e.Handled ? 1 : 0;
        Form_Unload(ref cancel);
        e.Handled = cancel != 0;
    }
    private void Form_Unload(ref int Cancel)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        Trace("- Form_Unload() ENGAGE!");
        Trace("- Calling SappyDecoder.StopSong");
        SappyDecoder.StopSong();
        Trace("- Calling ShutMSN");
        ShutMSN();
        Trace("- Terminating SappyDecoder");
        SappyDecoder = null;
        FileClose(File99);
        Trace("- Saving window height");
        WriteSettingI("Window Height", (int)Height);
        if (GetSettingI("mIRC Now Playing") != 0)
        {
            Trace("- Updating mIRC information");
            File42 = File.OpenWrite(AppContext.BaseDirectory + "\\sappy.stt");
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            File42.Write(assemblyName.Version.Major + "." + assemblyName.Version.Minor + " | | | | Not running | ");
            FileClose(File42);
        }
        Trace("- Detaching messages");
        GSubclass gSubclass = new();
        gSubclass.DetachMessage(this, (int)this.hWnd(), WM_SIZING);
        gSubclass.DetachMessage(this, (int)this.hWnd(), WM_APPCOMMAND);
        gSubclass.DetachMessage(this, (int)this.hWnd(), WM_MOUSEWHEEL);
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        Trace("- Killing forms");
        if (OwnedWindows.Count > 1)
        {
            foreach (Window Form in OwnedWindows)
            {
                Trace("- ..." + Form.Name);
                Form.Close();
            }
        }
        Trace("- Will I dream?");
        Application.Current.Shutdown();
    }

    public EMsgResponse MsgResponse
    {
        get
        {
            return EMsgResponse.emrPostProcess;
        }
        set
        {
            // This property procedure must exist to properly implement
            // the Subclassing Assistant, even though it does nothing.
        }
    }

    public int WindowProc(int hwnd, int iMsg, int wParam, int lParam)
    {
        int _ISubclass_WindowProc = 0;

        if (iMsg == WM_SIZING)
        {
            RECT myRect = new();
            Marshal.PtrToStructure(lParam, myRect);
            myRect.right = myRect.left + mywidth; // fix width
            if (myRect.bottom - myRect.top < 280) myRect.bottom = myRect.top + 280; // limit height
            Marshal.StructureToPtr(myRect, lParam, true);
        }

        if (iMsg == WM_APPCOMMAND)
        {
            // Okay... debug shows that the AppCommand's actual "command" code is in the first byte.
            // Since our track control buttons don't go over 0xF, we can't go over 0xF0000...
            if (lParam <= 0xF0000) // ...so first we ensure that part...
            {
                switch (Val("&H" + Left(Hex(lParam), 1))) // ...then cut out and evaluate the first nibble.
                {
                    case APPCOMMAND_MEDIA_NEXTTRACK:
                        cmdNextSong.setValue(true);
                        break;
                    case APPCOMMAND_MEDIA_PREVIOUSTRACK:
                        cmdPrevSong.setValue(true);
                        break;
                    case APPCOMMAND_MEDIA_PLAY_PAUSE:
                        cmdPlay.setValue(true);
                        break;
                    case APPCOMMAND_MEDIA_STOP:
                        cmdStop.setValue(true);
                        break;
                    case APPCOMMAND_VOLUME_DOWN:
                        VolumeSlider1.Value -= 5;
                        break;
                    case APPCOMMAND_VOLUME_UP:
                        VolumeSlider1.Value += 5;
                        // Case Else: Trace "Got an unhandled AppCommand: 0x" & Hex(lParam)
                        break;
                }
            }
            // TODO: Figure out how to "eat" the message, so pressing Play won't trigger background players.
        }

        if (iMsg == WM_MOUSEWHEEL)
        {
            if (wParam < 0)
            {
                VolumeSlider1.Value -= 5;
            }
            else if (wParam > 0)
            {
                VolumeSlider1.Value += 5;
            }
        }

        return _ISubclass_WindowProc;
    }

    private void lblExpand_Click(object sender, MouseButtonEventArgs e) { lblExpand_Click(); }
    private void lblExpand_Click()
    {
        if ((string)lblExpand.Content == "6")
        {
            lblExpand.Content = "5";
        }
        else
        {
            lblExpand.Content = "6";
        }
        for (int i = 0; i < cvwChannel.Count; i++)
        {
            cvwChannel[i].Expand((string)lblExpand.Content == "5");
        }
    }

    private void mnuAutovance_Click(object sender, RoutedEventArgs e) { mnuAutovance_Click(); }
    private void mnuAutovance_Click()
    {
        mnuAutovance.IsChecked = !mnuAutovance.IsChecked;
        WriteSettingI("AutoAdvance", mnuAutovance.IsChecked ? 1 : 0);
    }

    private void mnuFileExit_Click(object sender, RoutedEventArgs e) { mnuFileExit_Click(); }
    private void mnuFileExit_Click()
    {
        Unload();
    }

    private void mnuFileOpen_Click(object sender, RoutedEventArgs e) { mnuFileOpen_Click(); }
    private void mnuFileOpen_Click()
    {
        if ((string)mnuFileOpen.Tag == "BrotherMaynard") goto skipABit;
        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = Properties.Resources._2 + "|*.gba";
        if (!gCommonDialog.VBGetOpenFileName(ref myFile, ref fileTile, ref readOnly, ref filter, ref filterIndex)) return;

        skipABit:;
        FileClose(File99);

        SongTbl = 0;
        //TODO: Commenting these COM instructions because it didn't work.
        //ebr.Bars["Tasks"].CanExpand = false;
        //// ebr.Bars["Tasks"].State = eBarCollapsed
        //ebr.Bars["Info"].CanExpand = false;
        //// ebr.Bars["Info"].State = eBarCollapsed
        cbxSongs.IsEnabled = false;
        cmdPrevSong.IsEnabled = false;
        cmdNextSong.IsEnabled = false;
        txtSong.IsEnabled = false;
        chkMute.IsEnabled = false;
        cmdPlay.IsEnabled = false;
        cmdStop.IsEnabled = false;
        //TODO: Commenting these COM instructions because it didn't work.
        //for (int i = 1; i <= 5; i += 1)
        //{
        //    cPop.set_Enabled(TaskMenus[i], false);
        //}

        File99 = File.Open(myFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        File99.Seek(0xAC, SeekOrigin.Begin);
        File99.Read(out string code, 4);
        if (Asc(Mid(code, 1, 1)) == 0)
        {
            MsgBox(Properties.Resources._209);
            FileClose(File99);
            return;
        }
        DontLoadDude = true;

        string axe = "";
        if (Dir(code + ".xml") != "") axe = code + ".xml";
        if (Dir(AppContext.BaseDirectory + "\\" + code + ".xml") != "") axe = AppContext.BaseDirectory + "\\" + code + ".xml";
        LoadGameFromXML(ref code, axe);

        DontLoadDude = false;
        gamecode = UCase(code);
        if (SongTbl == 0)
        {
            if (Left(gamecode, 3) == "AGS" || Left(gamecode, 3) == "AGF" || Left(gamecode, 3) == "BMG")
            {
                // Autoscan don't like Golden Sun games :P
                MsgBox(Properties.Resources._110, vbExclamation);
                FileClose(File99);
                return;
            }
            if (MsgBox(Replace(Properties.Resources._205, "$CODE", gamecode), vbOKCancel | vbInformation) == vbCancel)
            {
                mnuFileOpen.Tag = "";
                FileClose(File99);
                return;
            }
            simple.Text = Properties.Resources._105;
            FindMST();
            simple.Text = "";
            if (SongTbl == 0) // still?
            {
                MsgBox(Properties.Resources._206);
                FileClose(File99);
                return;
            }
            MsgBox(Replace(Properties.Resources._207, "$TBL", Hex(SongTbl)));
            SaveBareBonesGameToXML();
            DontLoadDude = true;
            LoadGameFromXML(ref code);
            DontLoadDude = false;
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next // until we get the translations
        if (axe != "") simple.Text = Properties.Resources._111;
        // TODO: (NOT SUPPORTED): On Error GoTo 0

        //TODO: Commenting these COM instructions because it didn't work.
        //ebr.Bars["Tasks"].CanExpand = true;
        //// ebr.Bars["Tasks"].State = eBarExpanded
        //ebr.Bars["Info"].CanExpand = true;
        //// ebr.Bars["Info"].State = eBarExpanded
        cbxSongs.IsEnabled = true;
        cmdPrevSong.IsEnabled = true;
        cmdNextSong.IsEnabled = true;
        txtSong.IsEnabled = true;
        chkMute.IsEnabled = true;
        cmdPlay.IsEnabled = true;
        cmdStop.IsEnabled = true;
        txtSong.Text = playlist[0].SongNo[0].ToString();
        LoadSong(int.Parse(txtSong.Text));
        //TODO: Commenting these COM instructions because it didn't work.
        //for (int i = 1; i <= 5; i += 1)
        //{
        //    cPop.set_Enabled(TaskMenus[i], true);
        //}

        mnuFileOpen.Tag = "";

        // TODO: (NOT SUPPORTED): On Error Resume Next
        if (GetSettingI("Reload ROM") != 0)
        {
            WriteSetting("Last ROM", myFile);
        }

        DontLoadDude = false;
        // TODO: (NOT SUPPORTED): On Error GoTo 0
    }

    public void LoadSong(int i)
    {
        // TODO: (NOT SUPPORTED): On Error Return
        if (File99 == null || !File99.CanRead) return;

        File99.Seek(SongTbl + i * 8, SeekOrigin.Begin);
        byte[] buffer = new byte[sizeof(int)];
        File99.Read(buffer, 0, buffer.Length);
        int l = BitConverter.ToInt32(buffer, 0);
        l -= 0x8000000;
        SongHeadOrg = l;
        File99.Seek(l, SeekOrigin.Begin);
        File99.Read(out SongHead);

        for (int k = 0; k < 32; k += 1)
        {
            cvwChannel[k].Visibility = Visibility.Hidden;
        }

        for (int k = 0; k <= SongHead.NumTracks - 1; k += 1)
        {
            cvwChannel[k].Visibility = Visibility.Visible;
            cvwChannel[k].Location = SongHead.Tracks[k] - 0x8000000;
            cvwChannel[k].Note = "...";
            cvwChannel[k].Delay = "0";
            cvwChannel[k].pan = 0;
            cvwChannel[k].patch = "0";
            cvwChannel[k].Velocity = 0;
            cvwChannel[k].Vibrato = 0;
            cvwChannel[k].volume = "0";
        }

        lblDef.Content = "0x" + FixHex(SongTbl + i * 8, 6);
        lblLoc.Content = "0x" + FixHex(SongHeadOrg, 6);
        lblInst.Content = "0x" + FixHex(SongHead.VoiceGroup - 0x8000000, 6);

        string n = "?";
        lblSongName.Text = Replace(Properties.Resources._106, "$INDEX", i.ToString());
        justthesongname = "Track " + i;
        for (int k = 0; k <= NumPLs; k += 1)
        {
            for (l = 0; l <= playlist[k].NumSongs; l += 1)
            {
                if (playlist[k].SongNo[l] == i)
                {
                    DontLoadDude = true;
                    n = playlist[k].SongName[l];
                    justthesongname = n;
                    lblSongName.Text = Replace(Replace(Properties.Resources._107, "$NAME", n), "$INDEX", i.ToString());
                    for (int m = 0; m <= cbxSongs.Items.Count - 1; m += 1)
                    {
                        // If cbxSongs.List[m] = playlist[k].SongName[l] Then
                        if (cbxSongs.ItemData(m) == playlist[k].SongNo[l])
                        {
                            cbxSongs.SelectedIndex = m;
                            DontLoadDude = false;
                            goto ExitForGood;
                        }
                    }
                    DontLoadDude = false;
                }
            }
        }
    ExitForGood:;

        // Do mIRC string...
        AssemblyName assemblyName = Application.ResourceAssembly.GetName();
        songinfo = assemblyName.Version.Major + "." + assemblyName.Version.Minor +
              "|" + Path.GetFileNameWithoutExtension(myFile) +
              "|" + gamecode +
              "|" + txtSong +
              "|" + /*ebr.Bars["Info"].Items["Game"].Text +*/ //TODO: Commenting these COM instructions because it didn't work.
              "|" + n + (SappyDecoder.outputtype == SongOutputTypes.sotMIDI ? " (midi)" : "");

        if (Playing)
        {
            cmdPlay_Click();
        }
    }

    public void LoadGameFromXML(ref string gamecode, string newxfile = "")
    {
        if (newxfile == "") newxfile = xfile;
        Trace("Loading from " + newxfile + "...");
        x = new();
        x.Load(newxfile);
        x.PreserveWhitespace = true;
        //if (x.parseError.errorCode != 0)
        //{
        //    MsgBox(Replace(Replace(Properties.Resources._208, "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
        //    End();
        //}
        foreach (XmlElement n1 in x.ChildNodes)
        {
            if (n1.Name == "sappy")
            {
                rootNode = n1;
                break;
            }
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next
        //TODO: Commenting these COM instructions because it didn't work.
        //ebr.Bars["Info"].Items["Code"].Text = Properties.Resources._61;
        //ebr.Bars["Info"].Items["Game"].Text = Properties.Resources._62;
        //ebr.Bars["Info"].Items["Creator"].Text = Properties.Resources._63;
        //ebr.Bars["Info"].Items["Tagger"].Text = Properties.Resources._64;
        //ebr.Bars["Info"].Items["SongTbl"].Text = "0x000000";
        // Set picScreenshot.Picture = Nothing
        picScreenshot.Source = ConvertBitmap(Properties.Resources.NOPIC);
        cbxSongs.Clear();
        for (int j = 0; j < 255; j += 1)
        {
            playlist[j] = new();
            for (int i = 0; i < 1024; i++)
            {
                playlist[j].SongName[i] = "";
            }
        }
        playlist[0].NumSongs = 1;
        playlist[0].SongName[0] = Properties.Resources._109;
        playlist[0].SongNo[0] = 1;
        NumPLs = 1;
        for (int i = 0; i < 127; i++)
        {
            MidiMap[i] = i;
            MidiMapTrans[i] = 0;
            DrumMap[i] = i;
            BleedingEars[i] = -1;
        }
        BECnt = 0;
        MidiMapNode = null;
        MidiMapsDaddy = null;
        foreach (XmlElement n1 in rootNode.ChildNodes)
        {
            if (n1.Name == "rom")
            {
                NumPLs = 0;
                foreach (XmlAttribute n3 in n1.Attributes)
                {
                    if (n3.Name == "code")
                    {
                        if (LCase(n3.Value) != LCase(gamecode))
                        {
                            goto BrotherMaynard;
                        }
                        //TODO: Commenting these COM instructions because it didn't work.
                        //ebr.Bars["Info"].Items["Code"].Text = "Gamecode " + UCase(n3.InnerText);
                        //gamecode = UCase(n3.InnerText);
                    }
                    //if (n3.Name == "name")
                    //{
                    //    ebr.Bars["Info"].Items["Game"].Text = n3.InnerText;
                    //}
                    //if (n3.Name == "creator")
                    //{
                    //    ebr.Bars["Info"].Items["Creator"].Text = Properties.Resources._65 + n3.InnerText;
                    //}
                    //if (n3.Name == "tagger")
                    //{
                    //    ebr.Bars["Info"].Items["Tagger"].Text = Properties.Resources._66 + n3.InnerText;
                    //}
                    if (n3.Name == "songtable")
                    {
                        SongTbl = (int)Val("&H" + FixHex(n3.InnerText, 6));
                        if (Val("&H" + FixHex(n3.InnerText, 6)) != Val("&H" + FixHex(n3.InnerText, 6) + "&"))
                        {
                            MsgBox("Song pointer in an unsupported location. " + Hex(Val("&H" + FixHex(n3.InnerText, 6) + "&")) + " is read as " + Hex(Val("&H" + FixHex(n3.InnerText, 6))) + ".");
                            return;
                        }
                        //ebr.Bars["Info"].Items["SongTbl"].Text = Properties.Resources._67 + "0x" + Hex(SongTbl);
                    }
                    if (n3.Name == "screenshot")
                    {
                        // TODO: (NOT SUPPORTED): On Error Resume Next
                        picScreenshot.Tag = n3.Value;
                        picScreenshot.Source = new BitmapImage(new(new Uri("file://" + AppDomain.CurrentDomain.BaseDirectory), n3.Value));
                        // TODO: (NOT SUPPORTED): On Error GoTo 0
                    }
                }

                // TODO: (NOT SUPPORTED): On Error GoTo BrotherMaynard
                MidiMapsDaddy = n1;
                foreach (XmlElement n2 in n1.ChildNodes)
                {
                    if (n2.Name == "playlist")
                    {
                        if (n2.GetAttribute("steal") != "")
                        {
                            cbxSongs.AddItem(n2.GetAttribute("name"), 9999);//, 13, 13
                            playlist[NumPLs].NumSongs = 0;
                            foreach (XmlElement s1 in rootNode.ChildNodes)
                            {
                                if (s1.Name == "rom" && s1.GetAttribute("code") == n2.GetAttribute("steal"))
                                {
                                    foreach (XmlElement s2 in s1.ChildNodes)
                                    {
                                        if (s2.Name == "playlist" && s2.GetAttribute("name") == n2.GetAttribute("name"))
                                        {
                                            foreach (XmlElement s4 in s2.ChildNodes)
                                            {
                                                if (s4.Name == "song")
                                                {
                                                    playlist[NumPLs].SongName[playlist[NumPLs].NumSongs] = s4.InnerText;
                                                    playlist[NumPLs].SongNo[playlist[NumPLs].NumSongs] = (int)Val("&H" + FixHex(s4.GetAttribute("track"), 4));
                                                    playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
                                                    cbxSongs.AddItem(s4.InnerText, (int)Val("&H" + FixHex(s4.GetAttribute("track"), 4)));//, 14, 14, 1
                                                } // stealing song
                                            } // stealing playlist children
                                            goto StolenIt;
                                        } // stealing playlist
                                    } // stealing rom children
                                    MsgBox("Couldn't find playlist \"" + n2.GetAttribute("name") + "\" for gamecode \"" + n2.GetAttribute("steal") + "\".");
                                } // stealing rom
                            } // stealing library
                            NumPLs++;
                        }
                        else
                        {
                            cbxSongs.AddItem(n2.GetAttribute("name"), 9999);//, 13, 13
                            int picon = 14;
                            if (n2.GetAttribute("icon") == "1") picon = 25;
                            playlist[NumPLs].NumSongs = 0;
                            foreach (XmlElement n4 in n2.ChildNodes)
                            {
                                if (n4.Name == "song")
                                {
                                    int Icon = picon;
                                    if (n4.GetAttribute("icon") == "0") Icon = 14;
                                    if (n4.GetAttribute("icon") == "1") Icon = 25;
                                    playlist[NumPLs].SongName[playlist[NumPLs].NumSongs] = n4.InnerText;
                                    playlist[NumPLs].SongNo[playlist[NumPLs].NumSongs] = (int)Val("&H" + FixHex(n4.GetAttribute("track"), 4));
                                    playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
                                    cbxSongs.AddItem(n4.InnerText, (int)Val("&H" + FixHex(n4.GetAttribute("track"), 4)));//, Icon, Icon, 1
                                } // song
                            } // playlist songs
                            NumPLs++;
                        } // stealing check
                    } // playlist
                      // TODO: (NOT SUPPORTED): On Error Resume Next

                StolenIt:;
                    // We could get other tags here, like MidiMap.
                    if (n2.Name == "midimap")
                    {
                        MidiMapNode = n2;
                        foreach (XmlElement n4 in n2.ChildNodes)
                        {
                            if (n4.Name == "inst")
                            {
                                int i = int.Parse(n4.GetAttribute("from"));
                                MidiMap[i] = int.Parse(n4.GetAttribute("to"));
                                // TODO: (NOT SUPPORTED): On Error Resume Next
                                MidiMapTrans[i] = int.Parse(n4.GetAttribute("transpose"));
                                // TODO: (NOT SUPPORTED): On Error GoTo 0
                            } // inst
                        } // midimap children
                    } // midimap

                    if (n2.Name == "bleedingears")
                    {
                        foreach (XmlElement n4 in n2.ChildNodes)
                        {
                            if (n4.Name == "inst")
                            {
                                foreach (XmlAttribute n3 in n4.Attributes)
                                {
                                    if (n3.Name == "id")
                                    {
                                        BleedingEars[BECnt] = int.Parse(n3.Value);
                                        BECnt++;
                                    }
                                    if (n3.Name == "from")
                                    {
                                        for (int i = int.Parse(n3.Value); i <= int.Parse(n4.GetAttribute("to")); i++)
                                        {
                                            BleedingEars[BECnt] = i;
                                            BECnt++;
                                        }
                                    }
                                }
                                // i = n4.getAttribute("id")
                                // BleedingEars(BECnt) = i
                                // BECnt = BECnt + 1
                            } // inst
                        } // bleedingears children
                    } // bleedingears

                } // rom children
                  // TODO: (NOT SUPPORTED): On Error GoTo 0
            }
        BrotherMaynard:;
        }
        if (cbxSongs.Items.Count == 0) cbxSongs.AddItem("No songs defined", 1);//, 1, 1
        cbxSongs.SelectedIndex = 0;
    }

    private void SaveBareBonesGameToXML()
    {
        File99.Seek(0xA0, SeekOrigin.Begin);
        File99.Read(out string gamename, 12);
        XmlElement n1 = x.CreateElement("rom");

        n1.SetAttribute("code", gamecode);
        n1.SetAttribute("name", gamename);
        n1.SetAttribute("songtable", "0x" + Hex(SongTbl)); // FixHex(SongTbl, 6)

        XmlElement n3 = x.CreateElement("playlist");
        XmlAttribute n4 = x.CreateAttribute("name");
        n4.InnerText = "Main";
        n3.Attributes.SetNamedItem(n4);
        n1.AppendChild(n3);

        rootNode.InsertBefore(n1, null);
        x.Save(xfile);
    }

    private void SaveNewRomHeader(string att, string nV)
    {
        string axe = xfile;
        if (Dir(gamecode + ".xml") != "") axe = gamecode + ".xml";
        if (Dir(AppContext.BaseDirectory + "\\" + gamecode + ".xml") != "") axe = AppContext.BaseDirectory + "\\" + gamecode + ".xml";
        Trace("Saving to " + axe + "...");
        x = new();
        x.Load(axe);
        x.PreserveWhitespace = true;
        //if (x.parseError.errorCode != 0)
        //{
        //    MsgBox(Replace(Replace(Properties.Resources._208, "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
        //    End();
        //}
        foreach (XmlElement n1 in x.ChildNodes)
        {
            if (n1.Name == "sappy")
            {
                rootNode = n1;
                break;
            }
        }


        foreach (XmlElement n1 in rootNode.ChildNodes)
        {
            if (n1.Name == "rom" && n1.GetAttribute("code") == gamecode)
            {
                n1.SetAttribute(att, nV);
            }
        }
        x.Save(axe);
    }

    private void FindMST()
    {
        // Thumbcode to find:
        // 400B 4018 8388 5900 C918 8900 8918 0A68 0168 101C 00F0 ---- 01BC 0047 MPlayTBL SongTBL
        int off = 0;
        int match = 0;
        MousePointer = 11;
        File99.Seek(0, SeekOrigin.Begin);
        do
        {
            if ((File99.Position + 1) % 0x10000 == 1)
            {
                frame.Text = "0x" + Hex(File99.Position);
            }
            byte[] buffer = new byte[sizeof(int)];
            File99.Read(buffer, 0, buffer.Length);
            int anArm = BitConverter.ToInt32(buffer);
            if (match == 0)
            {
                if (anArm == 0x18400B40) match = 1; else match = 0;
            }
            else if (match == 1)
            {
                if (anArm == 0x598883) match = 2; else match = 0;
            }
            else if (match == 2)
            {
                if (anArm == 0x8918C9) match = 3; else match = 0;
            }
            else if (match == 3)
            {
                if (anArm == 0x680A1889) match = 4; else match = 0;
            }
            else if (match == 4)
            {
                if (anArm == 0x1C106801) match = 5; else match = 0;
            }
            else if (match == 5)
            {
                // skip over the jump
                match = 6;
            }
            else if (match == 6)
            {
                if (anArm == 0x4700BC01)
                {
                    match = 7;
                    off = (int)File99.Position;
                }
                else
                {
                    match = 0;
                }
            }
            if (match == 7) // mPlayTBL
            {
                File99.Seek(off, SeekOrigin.Begin);
                File99.Read(buffer, 0, buffer.Length); int aPointer = BitConverter.ToInt32(buffer);
                File99.Read(buffer, 0, buffer.Length); aPointer = BitConverter.ToInt32(buffer);
                SongTbl = aPointer - 0x8000000;
                MousePointer = 0;
                return;
            }
            DoEvents();
        } while (!EOF(File99));
        MousePointer = 0;
    }

    private void mnuGBMode_Click(object sender, RoutedEventArgs e) { mnuGBMode_Click(); }
    private void mnuGBMode_Click()
    {
        mnuGBMode.IsChecked = !mnuGBMode.IsChecked;
        WriteSettingI("MIDI In GB Mode", Abs(mnuGBMode.IsChecked ? 1 : 0));
    }

    private void mnuHelpAbout_Click(object sender, RoutedEventArgs e) { mnuHelpAbout_Click(); }
    private void mnuHelpAbout_Click()
    {
        new frmAbout().ShowDialog();
    }

    private void mnuHelpHelp_Click(object sender, RoutedEventArgs e) { mnuHelpHelp_Click(); }
    private void mnuHelpHelp_Click()
    {
        ShellExecute(0, vbNullString, AppContext.BaseDirectory + "\\sappy.chm", vbNullString, "", 1);
    }

    private void mnuHelpOnline_Click(object sender, RoutedEventArgs e) { mnuHelpOnline_Click(); }
    private void mnuHelpOnline_Click()
    {
        ShellExecute(0, vbNullString, "http://helmetedrodent.kickassgamers.com", vbNullString, "", 1);
    }

    private void mnuImportLST_Click(object sender, RoutedEventArgs e) { mnuImportLST_Click(); }
    private void mnuImportLST_Click()
    {
        string myFile = "";

        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = "Sappy.LST|sappy.lst";
        if (!gCommonDialog.VBGetOpenFileName(ref myFile, ref fileTile, ref readOnly, ref filter, ref filterIndex)) return;
        string myDir = Left(myFile, Len(myFile) - Len(Path.GetFileNameWithoutExtension(myFile)));

        x.Save(Left(xfile, Len(xfile) - 3) + "bak");

        using (StreamReader File96 = File.OpenText(myFile))
        {
            do
            {
                string c = File96.ReadLine(); // code
                if (c == "ENDFILE") break;
                string n = File96.ReadLine(); // name
                string e = File96.ReadLine(); // engine
                string p = File96.ReadLine(); // playlist
                string m = File96.ReadLine(); // map
                string s = File96.ReadLine(); // songlist
                string F = File96.ReadLine(); // first
                string l = File96.ReadLine(); // last
                if (c != "****" && Right(c, 1) != "ÿ")
                {
                    if (e == "sapphire")
                    {
                        XmlElement myNewRom = x.CreateElement("rom");
                        myNewRom.SetAttribute("code", c); // 181205 update: OOPS!
                        myNewRom.SetAttribute("name", n);
                        myNewRom.SetAttribute("songtable", "0x" + Hex(s) + "");
                        if (p == "blank")
                        {
                            XmlElement myNewList = x.CreateElement("playlist");
                            myNewList.SetAttribute("name", "No playlist");
                            myNewRom.AppendChild(myNewList);
                        }
                        else
                        {
                            if (Dir(myDir + p + ".lst") == "")
                            {
                                XmlElement myNewList = x.CreateElement("playlist");
                                myNewList.SetAttribute("name", "404");
                                myNewRom.AppendChild(myNewList);
                            }
                            else
                            {
                                using (StreamReader File95 = File.OpenText(myDir + p + ".lst"))
                                {
                                    do
                                    {
                                        string y = File95.ReadLine();
                                        if (y == "ENDFILE") break;
                                        XmlElement myNewList = x.CreateElement("playlist");
                                        myNewList.SetAttribute("name", y);
                                        do
                                        {
                                            y = File95.ReadLine();
                                            if (y != "ENDFILE")
                                            {
                                                if (y != "END")
                                                {
                                                    XmlElement myNewSong = x.CreateElement("song");
                                                    myNewSong.SetAttribute("track", Val("&H" + Left(y, 4)).ToString());
                                                    myNewSong.InnerText = Mid(y, 6);
                                                    myNewList.AppendChild(myNewSong);
                                                }
                                            }
                                        } while (!(y == "END"));
                                        myNewRom.AppendChild(myNewList);
                                    } while (true);
                                }
                            }
                        }
                        rootNode.AppendChild(myNewRom);
                    }
                }
            SkipThisShit:;
            } while (true);
            x.Save(xfile);
        }
        IncessantNoises("TaskComplete");

        LoadGameFromXML(ref gamecode);
    }

    private void mnuMidiMap_Click(object sender, RoutedEventArgs e) { mnuMidiMap_Click(); }
    private void mnuMidiMap_Click()
    {
        new frmMidiMapper().ShowDialog();
    }

    private void mnuOutput_Click(object sender, RoutedEventArgs e) { mnuOutput_Click(mnuOutput.IndexOf((MenuItem)sender)); }
    private void mnuOutput_Click(int Index)
    {
        if (Index == 0)
        {
            mnuOutput[0].IsChecked = true;
            mnuOutput[1].IsChecked = false;
            WriteSetting("Driver", "MIDI");
        }
        else
        {
            mnuOutput[0].IsChecked = false;
            mnuOutput[1].IsChecked = true;
            WriteSetting("Driver", "FMOD");
        }
        mnuGBMode.IsEnabled = mnuOutput[0].IsChecked;
    }

    private void mnuSeekPlaylist_Click(object sender, RoutedEventArgs e) { mnuSeekPlaylist_Click(); }
    private void mnuSeekPlaylist_Click()
    {
        mnuSeekPlaylist.IsChecked = !mnuSeekPlaylist.IsChecked;
        WriteSettingI("Seek by Playlist", Abs(mnuSeekPlaylist.IsChecked ? 1 : 0));
    }

    private void mnuSelectMIDI_Click(object sender, RoutedEventArgs e) { mnuSelectMIDI_Click(); }
    private void mnuSelectMIDI_Click()
    {
        new frmSelectMidiOut().ShowDialog();
    }

    private void mnuSettings_Click(object sender, RoutedEventArgs e) { mnuSettings_Click(); }
    private void mnuSettings_Click()
    {
        new frmOptions().ShowDialog();
    }

    private void picScreenshot_DblClick(object sender, MouseButtonEventArgs e) { if (e.ClickCount != 2) return; picScreenshot_DblClick(); }
    private void picScreenshot_DblClick()
    {
        string s = (string)picScreenshot.Tag;
        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = Properties.Resources._1 + "|*.BMP;*.GIF;*.JPG";
        if (gCommonDialog.VBGetOpenFileName(ref s, ref fileTile, ref readOnly, ref filter, ref filterIndex))
        {
            picScreenshot.Source = new BitmapImage(new(s));
            picScreenshot.Tag = s;
            SaveNewRomHeader("screenshot", s);
        }
    }

    private void picStatusbar_DblClick(object sender, MouseButtonEventArgs e) { picStatusbar_DblClick(); }
    private void picStatusbar_DblClick()
    {
        frmOptions instance = new()
        {
            Tag = "repsplz"
        };
        instance.ShowDialog();
    }

    private void picTop_Paint(object sender, RoutedEventArgs e)
    {
        RedrawSkin();
    }

    private void SappyDecoder_Beat(int Beats)
    {
        // If mnuOutput(0).Checked Then ToneOn 9, 37, 64
    }

    private void SappyDecoder_ChangedTempo(int newtempo)
    {
        lblSpeed.Content = SappyDecoder.Tempo;
    }

    private void SappyDecoder_Loading(int status)
    {
        string statusText = status switch
        {
            0 => Properties.Resources._8000,
            1 => Properties.Resources._8001,
            2 => Properties.Resources._8002,
            _ => throw new NotImplementedException(),
        };
        simple.Text = statusText;
        // If status = 1 Then cStatusBar.PanelText("simple") = cStatusBar.PanelText("simple") & " (" & progress & "/" & total & ")"
    }

    private void SappyDecoder_SongFinish()
    {
        Playing = false;
        timPlay.IsEnabled = false;
        cmdPlay.setImage(imlImages.ItemPicture(19).ToImageSource());
        mnuOutput[0].IsEnabled = true;
        mnuOutput[1].IsEnabled = true;
        mnuGBMode.IsEnabled = mnuOutput[0].IsChecked;
        linProgress.X2 = -1;
        ShutMSN();
    }

    private void SappyDecoder_SongLoop()
    {
        if (loopsToGo == 0) return;
        loopsToGo--;
        if (loopsToGo == 0) cmdStop_Click();
    }

    private void SappyDecoder_UpdateDisplay()
    {
        // Some of this from Drew's Sappy 2 interface.
        string it = "";

        for (int c = 1; c <= SappyDecoder.SappyChannels.count; c++)
        {
            int ct = 0;
            string ns = "";
            if (SappyDecoder.SappyChannels[c].Notes.count > 0)
            {
                foreach (SNote n in SappyDecoder.SappyChannels[c].Notes)
                {
                    if (SappyDecoder.NoteInfo[n.NoteID].Enabled && !SappyDecoder.NoteInfo[n.NoteID].NoteOff)
                    {
                        ct += (int)((decimal)SappyDecoder.NoteInfo[n.NoteID].Velocity / 0x7F * (SappyDecoder.NoteInfo[n.NoteID].EnvPosition / 0xFF) * 0x7F);
                        ns = ns + NoteToName(SappyDecoder.NoteInfo[n.NoteID].NoteNumber) + " ";
                    }
                    it = SappyDecoder.NoteInfo[n.NoteID].outputtype switch
                    {
                        NoteOutputTypes.notDirect => "Direct",
                        NoteOutputTypes.notNoise => "Noise",
                        NoteOutputTypes.notSquare1 => "Square1",
                        NoteOutputTypes.notSquare2 => "Square2",
                        NoteOutputTypes.notWave => "Wave",
                        _ => "",
                    };
                }
                ct /= SappyDecoder.SappyChannels[c].Notes.count;
            }
            ct = (int)(ct / 127m * (SappyDecoder.SappyChannels[c].MainVolume / 127m) * 255);
            cvwChannel[c - 1].Delay = SappyDecoder.SappyChannels[c].WaitTicks.ToString();
            cvwChannel[c - 1].volume = ct.ToString();
            cvwChannel[c - 1].pan = SappyDecoder.SappyChannels[c].Panning - 64;
            cvwChannel[c - 1].patch = SappyDecoder.SappyChannels[c].PatchNumber.ToString();
            cvwChannel[c - 1].Location = SappyDecoder.SappyChannels[c].TrackPointer + SappyDecoder.SappyChannels[c].ProgramCounter;
            if (ns != "") cvwChannel[c - 1].Note = ns;
            cvwChannel[c - 1].iType = it;
        }

        int totallen = 0;
        int totalplayed = 0;
        for (int c = 1; c <= SappyDecoder.SappyChannels.count; c += 1)
        {
            totallen += SappyDecoder.SappyChannels[c].TrackLengthInBytes;
            totalplayed += SappyDecoder.SappyChannels[c].ProgramCounter;
        }
        int totalpercent = (int)(326d / totallen * totalplayed);
        linProgress.X2 = totalpercent;
        // Caption = totalplayed & " / " & totallen & " -> " & totalpercent & "%"
        // Dim totalplayed As Long
        // With SappyDecoder.SappyChannels(1)
        //   tl = .TrackLengthInBytes - (.ProgramCounter + .TrackPointer)
        //   Caption = tl
        // End With

        crud.Text = loopsToGo.ToString();
        time.Text = Right("00" + TotalMinutes, 2) + ":" + Right("00" + TotalSeconds, 2) + " (" + SappyDecoder.Beats + ")";
        frame.Text = SappyDecoder.TotalTicks.ToString();

        // If SappyDecoder.TotalTicks < 96 Then Debug.Print (96 - SappyDecoder.SappyChannels(2).WaitTicks) & " vs " & SappyDecoder.TotalTicks
    }

    private void timPlay_Timer(object sender, EventArgs e)
    {
        TotalSeconds++;
        if (TotalSeconds == 60)
        {
            TotalMinutes++;
            TotalSeconds = 0;
        }
    }

    private void txtSong_LostFocus(object sender, RoutedEventArgs e) { txtSong_LostFocus(); }
    private void txtSong_LostFocus()
    {
        LoadSong((int)Val(txtSong.Text));
    }

    private void VolumeSlider1_Change(int NewValue)
    {
        decimal VolumeScalar = 5.1m;
        SappyDecoder.GlobalVolume = (int)(NewValue * VolumeScalar);
        WriteSettingI("FMOD Volume", NewValue);
    }

    public void RedrawSkin()
    {
        RECT panelRect = new()
        {
            left = 0,
            top = 0,
            right = (int)picTop.Width,
            bottom = (int)picTop.Height
        };

        WriteableBitmap bitmap = new((int)picTop.Width, (int)picTop.Height, 72, 72, PixelFormats.Bgra32, null);
        bitmap.BitBlt(panelRect.left, panelRect.top, 2, 2, (BitmapSource)picSkin.Source, 6, 0);
        bitmap.StretchBlt(panelRect.left + 2, panelRect.top, panelRect.right - 4, 2, (BitmapSource)picSkin.Source, 6, 2, 2, 2);
        bitmap.BitBlt(panelRect.left + panelRect.right - 2, panelRect.top, 2, 2, (BitmapSource)picSkin.Source, 6, 4);
        bitmap.StretchBlt(panelRect.left, panelRect.top + 2, 2, panelRect.bottom - 4, (BitmapSource)picSkin.Source, 6, 6, 2, 2);
        bitmap.StretchBlt(panelRect.left + 2, panelRect.top + 2, panelRect.right - 4, panelRect.bottom - 4, (BitmapSource)picSkin.Source, 0, 0, 6, 62);
        bitmap.StretchBlt(panelRect.left + panelRect.right - 2, panelRect.top + 2, 2, panelRect.bottom - 4, (BitmapSource)picSkin.Source, 6, 8, 2, 2);
        bitmap.BitBlt(panelRect.left, panelRect.top + panelRect.bottom - 2, 2, 2, (BitmapSource)picSkin.Source, 6, 10);
        bitmap.StretchBlt(panelRect.left + 2, panelRect.top + panelRect.bottom - 2, panelRect.right - 4, 2, (BitmapSource)picSkin.Source, 6, 12, 2, 2);
        bitmap.BitBlt(panelRect.left + panelRect.right - 2, panelRect.top + panelRect.bottom - 2, 2, 2, (BitmapSource)picSkin.Source, 6, 14);
        picTop.Source = bitmap;
        Color color = GetPixelColor((BitmapSource)picSkin.Source, 5, 42);
        VolumeSlider1.BackColor = RGB(color.R, color.G, color.B);
    }

    public void HandleClassicMode()
    {
        if (GetSettingI("Hide Bar") != 0)
        {
            ebrContainer.Visibility = Visibility.Hidden;
            Width = ClassicWidth;
            mywidth = (int)Width; // remember for wmSize subclass
            panelTop.Move(0);
            picChannels.Move(0);
            cbxSongs.Height = 330 / Screen.TwipsPerPixelY;
        }
        else
        {
            ebrContainer.Visibility = Visibility.Visible;
            Width = FullWidth;
            mywidth = (int)Width; // remember for wmSize subclass
            panelTop.Move(ebrContainer.Width);
            picChannels.Move(ebrContainer.Width);
            cbxSongs.Height = 330 / Screen.TwipsPerPixelY;
        }
    }

    public void FixStatusBar()
    {
        simple.Text = "";
        frame.Text = "0";
        crud.Text = "0";
        crudIcon.Source = imlStatusbar.ItemPicture(1).ToImageSource();
        time.Text = "00:00 (0)";
        timeIcon.Source = imlStatusbar.ItemPicture(2).ToImageSource();
    }

    private void PrepareRecording()
    {
        string target = "";
        string fileTitle = lblSongName.Text + ".mid";
        string filter = "Type 0 MIDI (*.mid)|*.mid";
        int filterIndex = 1;
        if (!gCommonDialog.VBGetSaveFileName(ref target, ref fileTitle, ref filter, ref filterIndex, DefaultExt: "mid")) return;
        WantToRecord = 1;
        WantToRecordTo = target;
        cmdPlay.setValue(true);

        // Set HookedDialog = New cCommonDialog
        // With HookedDialog
        //   .CancelError = False
        //   .DefaultExt = "mid"
        //   .DialogTitle = LoadResString(51)
        //   .Filter = "Type 0 MIDI (*.mid)|*.mid"
        //   .Filename = "Song " & txtSong & ".mid"
        //   .flags = EOpenFile.OFN_EXPLORER Or EOpenFile.OFN_NOCHANGEDIR
        //   .hwnd = Me.hwnd
        //   .HookDialog = True
        //   If InIDE() Then
        //     .cdLoadLibrary App.Path & "\sappy.exe"
        //   Else
        //     .hInstance = App.hInstance
        //   End If
        //   .TemplateName = 42
        //   .ShowSave
        //   If InIDE() Then .cdFreeLibrary
        //   If .Filename = "" Then Exit Sub
        //   WantToRecordTo = .Filename
        // End With
        // WantToRecord = 1
        // cmdPlay.value = True
    }

    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongA")]
    private static partial int GetWindowLong(IntPtr hWnd, int nIndex);
    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongA")]
    private static partial int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;

    private void Window_SourceInitialized(object sender, EventArgs e)
    {
        nint hwnd = new WindowInteropHelper((Window)sender).Handle;
        int value = GetWindowLong(hwnd, GWL_STYLE);
        _ = SetWindowLong(hwnd, GWL_STYLE, value & ~WS_MAXIMIZEBOX);
    }

    // Private Property Get InIDE() As Boolean
    // ' debug.assert doesn't go when compiled:
    // Debug.Assert (InIDESub())
    // ' so this will return false
    // InIDE = m_bInIDE
    // End Property
    // Private Property Get InIDESub() As Boolean
    // m_bInIDE = True
    // InIDESub = True
    // End Property
}
