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
using System.Windows.Threading;
using MSXML2;
using SappySharp.Classes;
using System.Reflection;
using SappySharp.UserControls;
using System.Windows.Forms.Integration;
using vbalExplorerBarLib6;
using cPopMenu6;
using stdole;
using SSubTimer6;

namespace SappySharp.Forms;

public partial class frmSappy : Window, ISubclass
{
    private static frmSappy _instance;
    public static frmSappy instance { set { _instance = null; } get { return _instance ??= new frmSappy(); } }
    public static void Load() { if (_instance == null) { dynamic A = instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmSappy()
    {
        InitializeComponent();
        timPlay.IsEnabled = false;
        timPlay.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        timPlay.Tick += timPlay_Timer;
    }

    public List<Button> cmdSpeed { get => VBExtension.controlArray<Button>(this, "cmdSpeed"); }

    public DispatcherTimer timPlay { get; set; } = new DispatcherTimer();

    public List<MenuItem> mnuOutput { get => VBExtension.controlArray<MenuItem>(this, "mnuOutput"); }

    public List<ChannelViewer> cvwChannel { get; private set; } = new();

    public vbalExplorerBarCtl ebr { get; private set; }

    public PopMenu cPop { get; private set; }

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
    // |  Main interface  |
    // |����������������������������������������������������|
    // | Code 99% by Kyoufu Kawa.                           |
    // | Last update: July 22nd, 2006                       |
    // |____________________________________________________|

    // ###########################################################################################


    class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }

    struct tSongHeader
    {
        public byte NumTracks;
        public byte NumBlocks;
        public byte Priority;
        public byte Reverb;
        public int VoiceGroup;
        public int[] Tracks = new int[32];

        public tSongHeader()
        {
        }
    }

    class tPlaylist
    {
        public int NumSongs;
        public string[] SongName = new string[1024];
        public int[] SongIcon = new int[1024];
        public int[] SongNo = new int[1024];
    }

    private static tPlaylist[] playlist = new tPlaylist[255];
    private static int NumPLs = 0;
    private static int[] MidiMap = new int[127];
    private static int[] MidiMapTrans = new int[127];
    private static int[] DrumMap = new int[127];
    private static int[] BleedingEars = new int[127];
    private static int BECnt = 0;
    public IXMLDOMElement MidiMapNode = null;
    public IXMLDOMElement MidiMapsDaddy = null;

    private static tSongHeader SongHead;
    private static int SongHeadOrg = 0;

    public int SongTbl = 0;
    public string gamecode = "";
    public string myFile = "";
    public string xfile = "";

    private static bool DontLoadDude = false;
    private static int[] TaskMenus = new int[16];

    public cVBALImageList imlImages = null;
    public cVBALImageList imlStatusbar = null;
    public static cNoStatusBar cStatusBar = null;
    public IXMLDOMDocument2 x = new DOMDocument();
    public IXMLDOMElement rootNode = null;

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
    [LibraryImport("gdi32.dll")]
    private static partial int GetPixel(int hdc, int x, int y);

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
        for (int i = 0; i <= cvwChannel.Count - 1; i += 1)
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
        for (int i = 0; i <= 127; i += 1)
        {
            // SappyDecoder.MidiMap(i) = MidiMap(i)
            SappyDecoder.SetMidiPatchMap(i, MidiMap[i], MidiMapTrans[i]);
            SappyDecoder.SetMidiDrumMap(i, DrumMap[i]);
        }
        for (int i = 0; i <= BECnt; i += 1)
        {
            SappyDecoder.AddEarPiercer(BleedingEars[i]);
        }
        if (mnuGBMode.IsChecked)
        {
            for (int i = 0; i <= 126; i += 1)
            {
                // SappyDecoder.MidiMap(i) = IIf(i Mod 2 = 1, 80, 81) '80
                SappyDecoder.SetMidiPatchMap(i, i % 2 == 1 ? 80 : 81, 0);
            }
        }

        linProgress.X2 = -1;

        SappyDecoder.GlobalVolume = (int)(VolumeSlider1.Value * 5.1m);
        SappyDecoder.PlaySong(myFile, int.Parse(txtSong.Text), SongTbl, WantToRecord != 0, WantToRecordTo);

        WantToRecord = 2;

        cStatusBar.PanelText("simple", "");

        for (int i = 0; i <= SappyDecoder.SappyChannels.count - 1; i += 1)
        {
            SappyDecoder.SappyChannels[i + 1].mute = IIf(cvwChannel[i].mute == 1, false, true);
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
        cmdPlay.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(20))));

        if (GetSettingI("mIRC Now Playing") != 0)
        {
            FileOpen(43, Assembly.GetExecutingAssembly().Location + "\\sappy.stt", OpenMode.Output);
            Print(43, songinfo);
            FileClose(43);
        }

        if (GetSettingI("MSN Now Playing") != 0)
        {
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            TellMSN(justthesongname + IIf(SappyDecoder.outputtype == SongOutputTypes.sotMIDI, " (midi)", ""), "Sappy " + assemblyName.Version.Major + "." + assemblyName.Version.Minor, ebr.Bars["Info"].Items["Game"].Text + " (" + gamecode + ")"); // ebr.Bars["Info"].Items["Creator"].Text
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
        if (Playing == true)
        {
            Playing = false;
            SappyDecoder.StopSong();
            linProgress.X2 = -1;
        }
        if (mnuAutovance.IsChecked) cmdPrevSong_Click();

        if (WantToRecord == 2) WantToRecord = 0;

        Playing = false;
        timPlay.IsEnabled = false;
        cmdPlay.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(19))));
        for (int i = 0; i <= cvwChannel.Count - 1; i += 1)
        {
            cvwChannel[i].volume = "0";
            cvwChannel[i].pan = 0;
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next
        if (GetSetting("mIRC Now Playing") != null)
        {
            FileOpen(44, Assembly.GetExecutingAssembly().Location + "\\sappy.stt", OpenMode.Output);
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            Print(44, assemblyName.Version.Major + "." + assemblyName.Version.Minor + " | | | | Not running | ");
            FileClose(44);
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
        for (int i = 1; i <= 16; i += 1)
        {
            if (ItemNumber == TaskMenus[i])
            {
                itm = ebr.Bars["Tasks"].Items[cPop.MenuKey[ItemNumber]];
                ebr_ItemClick(ref itm);
            }
        }
    }

    private void cPop_ItemHighlight(ref int ItemNumber, ref bool bEnabled, ref bool bSeparator)
    {
        cStatusBar.PanelText("simple", cPop.HelpText[ItemNumber]);
        // cStatusBar.SimpleMode = True
        // cStatusBar.SimpleText = cPop.HelpText(ItemNumber)
        picStatusbar.Refresh();
    }

    private void cPop_MenuExit()
    {
        cStatusBar.PanelText("simple", "");
        // cStatusBar.SimpleMode = False
        picStatusbar.Refresh();
    }

    private void cvwChannel_MuteChanged(int Index)
    {
        // If Playing = False Then Exit Sub
        // TODO: (NOT SUPPORTED): On Error Resume Next
        if (SappyDecoder.SappyChannels.count < 1) goto FlickIt; // Exit Sub
        SappyDecoder.SappyChannels[Index + 1].mute = IIf(cvwChannel[Index].mute == 1, false, true);

    FlickIt:;
        if ((string)chkMute.Tag == "O.O") return;
        int j = 0;
        for (int i = 0; i <= SappyDecoder.SappyChannels.count - 1; i += 1)
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

    private void cvwChannel_Resize(int Index)
    {
        for (int i = 1; i <= cvwChannel.Count - 1; i += 1)
        {
            cvwChannel[i].Margin = new(cvwChannel[i].Margin.Left, cvwChannel[i - 1].Margin.Top + cvwChannel[i - 1].Height, cvwChannel[i].Margin.Right, cvwChannel[i].Margin.Bottom);
        }
    }

    private void ebr_BarClick(ref cExplorerBar bar)
    {
        WriteSettingI("Bar " + bar.Index + " state", (int)bar.State);
    }

    private void ebr_ItemClick(ref cExplorerBarItem itm)
    {
        int i = 0;
        if (itm.Key == "taketrax")
        {
            for (i = 0; i <= SongHead.NumTracks - 1; i += 1)
            {
                frmTakeTrax.instance.lstTracks.AddItem("0x" + FixHex(SongHead.Tracks[i], 6));
            }
            frmTakeTrax.instance.ShowDialog();
        }
        if (itm.Key == "maketrax")
        {
            frmMakeTrax.instance.txtHeaderOffset.Text = "0x" + FixHex(SongHeadOrg, 6);
            frmMakeTrax.instance.txtTrack.Text = "0x" + FixHex(SongHead.Tracks[i], 6);
            frmMakeTrax.instance.MyNumblocks = SongHead.NumBlocks;
            frmMakeTrax.instance.MyPriority = SongHead.Priority;
            frmMakeTrax.instance.MyReverb = SongHead.Reverb;
            frmMakeTrax.instance.txtVoicegroup.Text = "0x" + FixHex(SongHead.VoiceGroup, 6);
            frmMakeTrax.instance.SongTableEntry = SongTbl + int.Parse(txtSong.Text) * 8;
            frmMakeTrax.instance.ShowDialog();
        }
        if (itm.Key == "takesamp")
        {
            frmTakeSamp.instance.SingleSong = SongHeadOrg;
            frmTakeSamp.instance.ShowDialog();
        }
        if (itm.Key == "codetrax")
        {
            frmAssembler.instance.SongTableEntry = SongTbl + int.Parse(txtSong.Text) * 8;
            frmAssembler.instance.txtVoicegroup.Text = "0x" + FixHex(SongHead.VoiceGroup, 6);
            frmAssembler.instance.ShowDialog();
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

        public static System.Drawing.Image PictureDispToImage(IPictureDisp pictureDisp)
        {
            return GetPictureFromIPicture(pictureDisp);
        }
    }
    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        // To call an OCX control.
        WindowsFormsHost ebrHost = new();
        ebr = new vbalExplorerBarCtl();
        ebr.BarClick += ebr_BarClick;
        ebr.ItemClick += ebr_ItemClick;
        ebrHost.Child = (System.Windows.Forms.Control)ebr;
        ebrContainer.Children.Add(ebrHost);

        WindowsFormsHost cPopHost = new();
        cPop = new PopMenu();
        cPop.Click += cPop_Click;
        cPop.ItemHighlight += cPop_ItemHighlight;
        cPop.MenuExit += cPop_MenuExit;
        cPopHost.Child = (System.Windows.Forms.Control)cPop;
        cPopContainer.Children.Add(cPopHost);

        int i = 0;
        string regset = "";

        Trace("frmSappy/Form_Load()");
        Trace("- Set icon");
        SetIcon((int)this.hWnd(), "APP", true);

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
        mywidth = (int)(Width / Screen.TwipsPerPixelX); // remember for wmSize subclass

        Trace("- Create duty cycle waves");
        DutyCycleWave[0] = new string(Chr(0), 14) + new string(Chr(255), 2);
        DutyCycleWave[1] = new string(Chr(0), 12) + new string(Chr(255), 4);
        DutyCycleWave[2] = new string(Chr(0), 16) + new string(Chr(255), 16);
        DutyCycleWave[3] = new string(Chr(0), 4) + new string(Chr(255), 12);

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
                FileOpen(4, xfile, OpenMode.Output);
                Print(4, "<sappy xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://helmetedrodent.kickassgamers.com/sappy.xsd\">");
                Print(4, "</sappy>");
                FileClose(4);
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
        lblPC.FontSize = 7;
        lblDel.FontSize = 7;
        lblNote.FontSize = 7;
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
            OwnerHDC = this.hWnd(),
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
        cmdPrevSong.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(16))));
        cmdNextSong.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(17))));
        cmdStop.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(18))));
        cmdPlay.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(19))));
        Trace("- Load status pics");
        imlStatusbar = new cVBALImageList
        {
            OwnerHDC = this.hWnd(),
            ColourDepth = eilColourDepth.ILC_COLOR8,
            IconSizeX = 16,
            IconSizeY = 16
        }; // vbalStatusBar
        imlStatusbar.Create();

        stdPic = (StdPicture)AxHostConverter.ImageToPictureDisp(Properties.Resources.STATUSICONS);
        imlStatusbar.AddFromHandle(stdPic.Handle, ImageTypes.IMAGE_BITMAP, lBackColor: 0xFF00FF);

        Trace("- Create status bar");
        cStatusBar = new cNoStatusBar();
        cStatusBar.Create(picStatusbar);
        cStatusBar.ImageList = imlStatusbar.hIml;
        cStatusBar.AllowXPStyles = true;
        cStatusBar.Font = new System.Drawing.Font(lblSong.FontFamily.ToString(), (float)lblSong.FontSize,
            (lblSong.FontWeight > FontWeights.Regular ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular)
            | (lblSong.FontStyle == FontStyles.Italic ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular));
        FixStatusBar();

        Trace("- Create menu");
        cPop.SubClassMenu(this);
        if (GetSettingI("Nice Menus") == 0)
        {
            // cPop.HighlightStyle =
            cPop.OfficeXpStyle = false;
        }
        else
        {
            cPop.OfficeXpStyle = true;
        }

        Trace("- Set menu icons and help");
        object hIml = imlImages.hIml;
        cPop.set_ImageList(ref hIml);
        cPop.ItemIcon["mnuFileOpen"] = 0;
        cPop.ItemIcon["mnuOutput(0)"] = 2;
        cPop.ItemIcon["mnuOutput(1)"] = 3;
        cPop.ItemIcon["mnuSeekPlaylist"] = 4;
        // cPop.ItemIcon["mnuAutovance"] = 5
        cPop.ItemIcon["mnuGBMode"] = 6;
        cPop.ItemIcon["mnuHelpHelp"] = 7;
        cPop.ItemIcon["mnuHelpOnline"] = 8;
        cPop.ItemIcon["mnuImportLST"] = 20;
        cPop.ItemIcon["mnuSelectMIDI"] = 22;
        cPop.ItemIcon["mnuSettings"] = 24;
        cPop.ItemIcon["mnuMidiMap"] = 23;
        cPop.HelpText["mnuFileOpen"] = Properties.Resources._70;
        cPop.HelpText["mnuFileExit"] = Properties.Resources._71;
        cPop.HelpText["mnuOutput(0)"] = Properties.Resources._72;
        cPop.HelpText["mnuOutput(1)"] = Properties.Resources._73;
        cPop.HelpText["mnuHelpAbout"] = Properties.Resources._75;
        cPop.HelpText["mnuHelpOnline"] = Properties.Resources._76;

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
        if (regset != "") hue = (decimal)Val(Replace(regset, ",", ".")); else hue = 3.5m;
        regset = GetSetting("Skin Saturation");
        if (regset != "") sat = (decimal)Val(Replace(regset, ",", ".")); else sat = 0.4m;
        Colorize(picSkin, hue, sat);

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
        ebr.Bars.Add("Tasks", Properties.Resources._50);
        hIml = imlImages.hIml;
        ebr.set_ImageList(ref hIml);
        ebr.Bars["Tasks"].CanExpand = false;
        ebr.Bars["Tasks"].State = EExplorerBarStates.eBarCollapsed;
        Trace("- Add tasks");
        ebr.Bars["Tasks"].Items.Add("taketrax", Properties.Resources._52, 9);
        ebr.Bars["Tasks"].Items.Add("maketrax", Properties.Resources._53, 10);
        ebr.Bars["Tasks"].Items.Add("takesamp", Properties.Resources._54, 11);
        ebr.Bars["Tasks"].Items.Add("codetrax", Properties.Resources._55, 12);
        ebr.Bars["Tasks"].Items.Add("makemidi", Properties.Resources._51, 2);
        ebr.Bars["Tasks"].Items["taketrax"].ToolTipText = Properties.Resources._81;
        ebr.Bars["Tasks"].Items["maketrax"].ToolTipText = Properties.Resources._82;
        ebr.Bars["Tasks"].Items["takesamp"].ToolTipText = Properties.Resources._83;
        ebr.Bars["Tasks"].Items["codetrax"].ToolTipText = Properties.Resources._84;
        ebr.Bars["Tasks"].Items["makemidi"].ToolTipText = Properties.Resources._80;
        for (i = 1; i <= ebr.Bars["Tasks"].Items.Count; i += 1)
        {
            TaskMenus[i] = cPop.AddItem(ebr.Bars["Tasks"].Items[i].Text, ebr.Bars["Tasks"].Items[i].Key, ebr.Bars["Tasks"].Items[i].ToolTipText, lParentIndex: cPop.MenuIndex["mnuTasks"], lIconIndex: ebr.Bars["Tasks"].Items[i].IconIndex, bChecked: false, bEnabled: false);
        }
        ebr.Bars["Tasks"].State = (EExplorerBarStates)GetSettingI("Bar " + ebr.Bars["Tasks"].Index + " state");
        Trace("- Set up info bar");
        ebr.Bars.Add("Info", Properties.Resources._60);
        ebr.Bars["Info"].CanExpand = false;
        ebr.Bars["Info"].State = EExplorerBarStates.eBarCollapsed;
        ebr.Bars["Info"].Items.Add("Game", Properties.Resources._61, IconIndex: 0);
        ebr.Bars["Info"].Items.Add("Code", Properties.Resources._62, IconIndex: 1);
        ebr.Bars["Info"].Items.Add("Creator", Properties.Resources._63, IconIndex: 0);
        ebr.Bars["Info"].Items.Add("Tagger", Properties.Resources._64, IconIndex: 0);
        ebr.Bars["Info"].Items.Add("SongTbl", "0x000000", IconIndex: 1);
        ebr.Bars["Info"].Items.Add("Screen", IconIndex: 2);
        ebr.Bars["Info"].Items["Game"].Bold = true;
        ebr.Bars["Info"].Items["SongTbl"].SpacingAfter = 8;
        object newControl = picScreenshot.Source;
        ebr.Bars["Info"].Items["Screen"].let_Control(ref newControl);
        ebr.Bars["Info"].Items["Screen"].SpacingAfter = 6;
        ebr.Bars["Info"].State = (EExplorerBarStates)GetSettingI("Bar " + ebr.Bars["Info"].Index + " state");

        Trace("- Create channel views");
        cvwChannel.Add(cvwChannelTemplate);
        cvwChannelTemplate.MuteChanged += (sender, e) => cvwChannel_MuteChanged(0);
        cvwChannelTemplate.Resize += (sender, e) => cvwChannel_Resize(0);
        for (i = 1; i < 32; i += 1)
        {
            cvwChannel.Add(new ChannelViewer());
            cvwChannel[i].MuteChanged += (sender, e) => cvwChannel_MuteChanged(i);
            cvwChannel[i].Resize += (sender, e) => cvwChannel_Resize(i);
            cvwChannel[i].Margin = new Thickness(cvwChannel[i - 1].Margin.Left, cvwChannel[i - 1].Margin.Top + cvwChannel[i - 1].Height, cvwChannel[i - 1].Margin.Right, cvwChannel[i - 1].Margin.Bottom);
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
        picChannels.Height = Height - picChannels.Margin.Top - picStatusbar.Height;
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
        Trace("- Killing menu subclass");
        cPop.UnsubclassMenu();
        FileClose(99);
        Trace("- Saving window height");
        WriteSettingI("Window Height", (int)Height);
        if (GetSettingI("mIRC Now Playing") != 0)
        {
            Trace("- Updating mIRC information");
            FileOpen(42, AppContext.BaseDirectory + "\\sappy.stt", OpenMode.Output);
            AssemblyName assemblyName = Application.ResourceAssembly.GetName();
            Print(42, assemblyName.Version.Major + "." + assemblyName.Version.Minor + " | | | | Not running | ");
            FileClose(42);
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
            RECT myRect = null;
            dynamic dest = myRect;
            dynamic source = lParam;
            CopyMemory(ref dest, ref source, Marshal.SizeOf(myRect)); // get the Rect pointed to in lParam
            myRect = dest;
            myRect.Right = myRect.left + mywidth; // fix width
            if (myRect.Bottom - myRect.tOp < 280) myRect.Bottom = myRect.tOp + 280; // limit height
            dest = lParam;
            source = myRect;
            CopyMemory(ref dest, ref source, Marshal.SizeOf(myRect)); // put our edited Rect back in lParam
            lParam = dest;
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
        for (int i = 0; i <= cvwChannel.Count - 1; i += 1)
        {
            cvwChannel[i].Expand((string)lblExpand.Content == "5");
        }
        for (int i = 1; i <= cvwChannel.Count - 1; i += 1)
        {
            cvwChannel[i].Margin = new(cvwChannel[i].Margin.Left, cvwChannel[i - 1].Margin.Top + cvwChannel[i - 1].Height, cvwChannel[i].Margin.Right, cvwChannel[i].Margin.Bottom);
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
        gCommonDialog cc = new();
        string code = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (4)

        if ((string)mnuFileOpen.Tag == "BrotherMaynard") goto skipABit;
        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = Properties.Resources._2 + "|*.gba";
        string initDir = null;
        string dlgTitle = null;
        string defaultExt = null;
        int flags = 0;
        if (cc.VBGetOpenFileName(ref myFile, ref fileTile, ref readOnly, ref filter, ref filterIndex, ref initDir, ref dlgTitle, ref defaultExt, ref flags) == false) return;

        skipABit:;
        FileClose(99);

        SongTbl = 0;
        ebr.Bars["Tasks"].CanExpand = false;
        // ebr.Bars["Tasks"].State = eBarCollapsed
        ebr.Bars["Info"].CanExpand = false;
        // ebr.Bars["Info"].State = eBarCollapsed
        cbxSongs.IsEnabled = false;
        cmdPrevSong.IsEnabled = false;
        cmdNextSong.IsEnabled = false;
        txtSong.IsEnabled = false;
        chkMute.IsEnabled = false;
        cmdPlay.IsEnabled = false;
        cmdStop.IsEnabled = false;
        for (int i = 1; i <= 5; i += 1)
        {
            cPop.Enabled[TaskMenus[i]] = false;
        }

        FileOpen(99, myFile, OpenMode.Binary);
        FileGet(99, ref code, 0xAC + 1);
        if (Asc(Mid(code, 1, 1)) == 0)
        {
            MsgBox(Properties.Resources._209);
            FileClose(99);
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
                FileClose(99);
                return;
            }
            if (MsgBox(Replace(Properties.Resources._205, "$CODE", gamecode), vbOKCancel | vbInformation) == vbCancel)
            {
                mnuFileOpen.Tag = "";
                FileClose(99);
                return;
            }
            cStatusBar.PanelText("simple", Properties.Resources._105);
            picStatusbar.Refresh();
            FindMST();
            cStatusBar.PanelText("simple", "");
            picStatusbar.Refresh();
            if (SongTbl == 0)
            { // still?
                MsgBox(Properties.Resources._206);
                FileClose(99);
                return;
            }
            MsgBox(Replace(Properties.Resources._207, "$TBL", Hex(SongTbl)));
            SaveBareBonesGameToXML();
            DontLoadDude = true;
            LoadGameFromXML(ref code);
            DontLoadDude = false;
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next // until we get the translations
        if (axe != "") cStatusBar.PanelText("simple", Properties.Resources._111);
        // TODO: (NOT SUPPORTED): On Error GoTo 0

        ebr.Bars["Tasks"].CanExpand = true;
        // ebr.Bars["Tasks"].State = eBarExpanded
        ebr.Bars["Info"].CanExpand = true;
        // ebr.Bars["Info"].State = eBarExpanded
        cbxSongs.IsEnabled = true;
        cmdPrevSong.IsEnabled = true;
        cmdNextSong.IsEnabled = true;
        txtSong.IsEnabled = true;
        chkMute.IsEnabled = true;
        cmdPlay.IsEnabled = true;
        cmdStop.IsEnabled = true;
        txtSong.Text = playlist[0].SongNo[0].ToString();
        LoadSong(int.Parse(txtSong.Text));
        for (int i = 1; i <= 5; i += 1)
        {
            cPop.Enabled[TaskMenus[i]] = true;
        }

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
        int l = 0;

        // TODO: (NOT SUPPORTED): On Error GoTo hell

        FileGet(99, ref l, SongTbl + i * 8 + 1);
        l -= 0x8000000;
        SongHeadOrg = l;
        ValueType sh = SongHead;
        FileGet(99, ref sh, l + 1);
        SongHead = (tSongHeader)sh;

        for (int k = 0; k <= 32; k += 1)
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
        lblSongName.Content = Replace(Properties.Resources._106, "$INDEX", i.ToString());
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
                    lblSongName.Content = Replace(Replace(Properties.Resources._107, "$NAME", n), "$INDEX", i.ToString());
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
        gCommonDialog cc = new();
        AssemblyName assemblyName = Application.ResourceAssembly.GetName();
        songinfo = assemblyName.Version.Major + "." + assemblyName.Version.Minor +
              "|" + cc.VBGetFileTitle(myFile) +
              "|" + gamecode +
              "|" + txtSong +
              "|" + ebr.Bars["Info"].Items["Game"].Text +
              "|" + n + IIf(SappyDecoder.outputtype == SongOutputTypes.sotMIDI, " (midi)", "");

        if (Playing == true)
        {
            cmdPlay_Click();
        }

    hell:;
    }

    public void LoadGameFromXML(ref string gamecode, string newxfile = "")
    {
        int i = 0;
        int j = 0;
        int Icon = 0;
        int picon = 0;

        if (newxfile == "") newxfile = xfile;
        Trace("Loading from " + newxfile + "...");
        x = new DOMDocument26();
        x.load(newxfile);
        x.preserveWhiteSpace = true;
        if (x.parseError.errorCode != 0)
        {
            MsgBox(Replace(Replace(Properties.Resources._208, "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
            End();
        }
        foreach (IXMLDOMElement n1 in x.childNodes)
        {
            if (n1.baseName == "sappy")
            {
                rootNode = n1;
                break;
            }
        }

        // TODO: (NOT SUPPORTED): On Error Resume Next
        ebr.Bars["Info"].Items["Code"].Text = Properties.Resources._61;
        ebr.Bars["Info"].Items["Game"].Text = Properties.Resources._62;
        ebr.Bars["Info"].Items["Creator"].Text = Properties.Resources._63;
        ebr.Bars["Info"].Items["Tagger"].Text = Properties.Resources._64;
        ebr.Bars["Info"].Items["SongTbl"].Text = "0x000000";
        // Set picScreenshot.Picture = Nothing
        picScreenshot.Source = ConvertBitmap(Properties.Resources.NOPIC);
        cbxSongs.Clear();
        for (j = 0; j <= 255; j += 1)
        {
            for (i = 0; i <= 1024; i += 1)
            {
                playlist[j].SongName[i] = "";
                playlist[j].SongNo[i] = 0;
            }
            playlist[0].NumSongs = 0;
        }
        playlist[0].NumSongs = 1;
        playlist[0].SongName[0] = Properties.Resources._109;
        playlist[0].SongNo[0] = 1;
        NumPLs = 1;
        for (i = 0; i <= 127; i += 1)
        {
            MidiMap[i] = i;
            MidiMapTrans[i] = 0;
            DrumMap[i] = i;
            BleedingEars[i] = -1;
        }
        BECnt = 0;
        MidiMapNode = null;
        MidiMapsDaddy = null;
        foreach (IXMLDOMElement n1 in rootNode.childNodes)
        {
            if (n1.baseName == "rom")
            {
                NumPLs = 0;
                foreach (IXMLDOMAttribute n3 in n1.attributes)
                {
                    if (n3.baseName == "code")
                    {
                        if (LCase(n3.value) != LCase(gamecode))
                        {
                            goto BrotherMaynard;
                        }
                        ebr.Bars["Info"].Items["Code"].Text = "Gamecode " + UCase(n3.text);
                        gamecode = UCase(n3.text);
                    }
                    if (n3.baseName == "name")
                    {
                        ebr.Bars["Info"].Items["Game"].Text = n3.text;
                    }
                    if (n3.baseName == "creator")
                    {
                        ebr.Bars["Info"].Items["Creator"].Text = Properties.Resources._65 + n3.text;
                    }
                    if (n3.baseName == "tagger")
                    {
                        ebr.Bars["Info"].Items["Tagger"].Text = Properties.Resources._66 + n3.text;
                    }
                    if (n3.baseName == "songtable")
                    {
                        SongTbl = (int)Val("&H" + FixHex(n3.text, 6));
                        if (Val("&H" + FixHex(n3.text, 6)) != Val("&H" + FixHex(n3.text, 6) + "&"))
                        {
                            MsgBox("Song pointer in an unsupported location. " + Hex(Val("&H" + FixHex(n3.text, 6) + "&")) + " is read as " + Hex(Val("&H" + FixHex(n3.text, 6))) + ".");
                            return;
                        }
                        ebr.Bars["Info"].Items["SongTbl"].Text = Properties.Resources._67 + "0x" + Hex(SongTbl);
                    }
                    if (n3.baseName == "screenshot")
                    {
                        // TODO: (NOT SUPPORTED): On Error Resume Next
                        picScreenshot.Tag = n3.value;
                        picScreenshot.Source = new BitmapImage(new(n3.value));
                        // TODO: (NOT SUPPORTED): On Error GoTo 0
                    }
                }

                // TODO: (NOT SUPPORTED): On Error GoTo BrotherMaynard
                MidiMapsDaddy = n1;
                foreach (IXMLDOMElement n2 in n1.childNodes)
                {
                    if (n2.baseName == "playlist")
                    {
                        if (n2.getAttribute("steal") != "")
                        {
                            cbxSongs.AddItem((string)n2.getAttribute("name"), 9999);//, 13, 13
                            playlist[NumPLs].NumSongs = 0;
                            foreach (IXMLDOMElement s1 in rootNode.childNodes)
                            {
                                if (s1.baseName == "rom" && s1.getAttribute("code") == n2.getAttribute("steal"))
                                {
                                    foreach (IXMLDOMElement s2 in s1.childNodes)
                                    {
                                        if (s2.baseName == "playlist" && s2.getAttribute("name") == n2.getAttribute("name"))
                                        {
                                            foreach (IXMLDOMElement s4 in s2.childNodes)
                                            {
                                                if (s4.baseName == "song")
                                                {
                                                    playlist[NumPLs].SongName[playlist[NumPLs].NumSongs] = s4.text;
                                                    playlist[NumPLs].SongNo[playlist[NumPLs].NumSongs] = Val("&H" + FixHex(s4.getAttribute("track"), 4));
                                                    playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
                                                    cbxSongs.AddItem(s4.text, (int)Val("&H" + FixHex(s4.getAttribute("track"), 4)));//, 14, 14, 1
                                                } // stealing song
                                            } // stealing playlist children
                                            goto StolenIt;
                                        } // stealing playlist
                                    } // stealing rom children
                                    MsgBox("Couldn't find playlist \"" + n2.getAttribute("name") + "\" for gamecode \"" + n2.getAttribute("steal") + "\".");
                                } // stealing rom
                            } // stealing library
                            NumPLs++;
                        }
                        else
                        {
                            cbxSongs.AddItem((string)n2.getAttribute("name"), 9999);//, 13, 13
                            picon = 14;
                            if (n2.getAttribute("icon") == "1") picon = 25;
                            playlist[NumPLs].NumSongs = 0;
                            foreach (IXMLDOMElement n4 in n2.childNodes)
                            {
                                if (n4.baseName == "song")
                                {
                                    Icon = picon;
                                    if (n4.getAttribute("icon") == "0") Icon = 14;
                                    if (n4.getAttribute("icon") == "1") Icon = 25;
                                    playlist[NumPLs].SongName[playlist[NumPLs].NumSongs] = n4.text;
                                    playlist[NumPLs].SongNo[playlist[NumPLs].NumSongs] = Val("&H" + FixHex(n4.getAttribute("track"), 4));
                                    playlist[NumPLs].NumSongs = playlist[NumPLs].NumSongs + 1;
                                    cbxSongs.AddItem(n4.text, (int)Val("&H" + FixHex(n4.getAttribute("track"), 4)));//, Icon, Icon, 1
                                } // song
                            } // playlist songs
                            NumPLs++;
                        } // stealing check
                    } // playlist
                      // TODO: (NOT SUPPORTED): On Error Resume Next

                StolenIt:;
                    // We could get other tags here, like MidiMap.
                    if (n2.baseName == "midimap")
                    {
                        MidiMapNode = n2;
                        foreach (IXMLDOMElement n4 in n2.childNodes)
                        {
                            if (n4.baseName == "inst")
                            {
                                i = n4.getAttribute("from");
                                MidiMap[i] = n4.getAttribute("to");
                                // TODO: (NOT SUPPORTED): On Error Resume Next
                                MidiMapTrans[i] = n4.getAttribute("transpose");
                                // TODO: (NOT SUPPORTED): On Error GoTo 0
                            } // inst
                        } // midimap children
                    } // midimap

                    if (n2.baseName == "bleedingears")
                    {
                        foreach (IXMLDOMElement n4 in n2.childNodes)
                        {
                            if (n4.baseName == "inst")
                            {
                                foreach (IXMLDOMAttribute n3 in n4.attributes)
                                {
                                    if (n3.baseName == "id")
                                    {
                                        BleedingEars[BECnt] = n3.value;
                                        BECnt++;
                                    }
                                    if (n3.baseName == "from")
                                    {
                                        for (i = n3.value; i <= n4.getAttribute("to"); i += 1)
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
        string gamename = ""; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (12)
        FileGet(99, ref gamename, 0xA1);
        IXMLDOMElement n1 = x.createElement("rom");

        n1.setAttribute("code", gamecode);
        n1.setAttribute("name", gamename);
        n1.setAttribute("songtable", "0x" + Hex(SongTbl)); // FixHex(SongTbl, 6)

        IXMLDOMElement n3 = x.createElement("playlist");
        IXMLDOMAttribute n4 = x.createAttribute("name");
        n4.text = "Main";
        n3.attributes.setNamedItem(n4);
        n1.appendChild(n3);

        rootNode.insertBefore(n1, null);
        x.save(xfile);
    }

    private void SaveNewRomHeader(string att, string nV)
    {
        string axe = "";

        axe = xfile;
        if (Dir(gamecode + ".xml") != "") axe = gamecode + ".xml";
        if (Dir(AppContext.BaseDirectory + "\\" + gamecode + ".xml") != "") axe = AppContext.BaseDirectory + "\\" + gamecode + ".xml";
        Trace("Saving to " + axe + "...");
        x = new DOMDocument26();
        x.load(axe);
        x.preserveWhiteSpace = true;
        if (x.parseError.errorCode != 0)
        {
            MsgBox(Replace(Replace(Properties.Resources._208, "$ERROR", x.parseError.reason), "$CAUSE", x.parseError.srcText));
            End();
        }
        foreach (IXMLDOMElement n1 in x.childNodes)
        {
            if (n1.baseName == "sappy")
            {
                rootNode = n1;
                break;
            }
        }


        foreach (IXMLDOMElement n1 in rootNode.childNodes)
        {
            if (n1.baseName == "rom" && n1.getAttribute("code") == gamecode)
            {
                n1.setAttribute(att, nV);
            }
        }
        x.save(axe);
    }

    private void FindMST()
    {
        // Thumbcode to find:
        // 400B 4018 8388 5900 C918 8900 8918 0A68 0168 101C 00F0 ---- 01BC 0047 MPlayTBL SongTBL
        int anArm = 0;
        int aPointer = 0;
        int off = 0;
        int match = 0;
        MousePointer = 11;
        Seek(99, 1);
        do
        {
            if (Seek(99) % 0x10000 == 1)
            {
                cStatusBar.PanelText("frame", "0x" + Hex(Seek(99) - 1));
                picStatusbar.Refresh();
            }
            FileGet(99, ref anArm);
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
                    off = (int)Seek(99);
                }
                else
                {
                    match = 0;
                }
            }
            if (match == 7) // mPlayTBL
            {
                Seek(99, off);
                FileGet(99, ref aPointer);
                FileGet(99, ref aPointer);
                SongTbl = aPointer - 0x8000000;
                MousePointer = 0;
                return;
            }
            DoEvents();
        } while (!EOF(99));
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
        frmAbout.instance.ShowDialog();
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
        gCommonDialog cc = new();
        string myFile = "";
        string c = "";
        string n = "";
        string e = "";
        string p = "";
        string m = "";
        string s = "";
        string F = "";
        string l = "";

        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = "Sappy.LST|sappy.lst";
        string initDir = null;
        string dlgTitle = null;
        string defaultExt = null;
        int flags = 0;
        if (cc.VBGetOpenFileName(ref myFile, ref fileTile, ref readOnly, ref filter, ref filterIndex, ref initDir, ref dlgTitle, ref defaultExt, ref flags) == false) return;
        string myDir = Left(myFile, Len(myFile) - Len(cc.VBGetFileTitle(myFile)));

        x.save(Left(xfile, Len(xfile) - 3) + "bak");

        FileOpen(96, myFile, OpenMode.Input);
        do
        {
            Input(96, ref c); // code
            if (c == "ENDFILE") break;
            Input(96, ref n); // name
            Input(96, ref e); // engine
            Input(96, ref p); // playlist
            Input(96, ref m); // map
            Input(96, ref s); // songlist
            Input(96, ref F); // first
            Input(96, ref l); // last
            if (c != "****" && Right(c, 1) != "�")
            {
                if (e == "sapphire")
                {
                    IXMLDOMElement myNewRom = x.createElement("rom");
                    myNewRom.setAttribute("code", c); // 181205 update: OOPS!
                    myNewRom.setAttribute("name", n);
                    myNewRom.setAttribute("songtable", "0x" + Hex(s) + "");
                    if (p == "blank")
                    {
                        IXMLDOMElement myNewList = x.createElement("playlist");
                        myNewList.setAttribute("name", "No playlist");
                        myNewRom.appendChild(myNewList);
                    }
                    else
                    {
                        if (Dir(myDir + p + ".lst") == "")
                        {
                            IXMLDOMElement myNewList = x.createElement("playlist");
                            myNewList.setAttribute("name", "404");
                            myNewRom.appendChild(myNewList);
                        }
                        else
                        {
                            FileOpen(95, myDir + p + ".lst", OpenMode.Input);
                            do
                            {
                                string y = LineInput(95);
                                if (y == "ENDFILE") break;
                                IXMLDOMElement myNewList = x.createElement("playlist");
                                myNewList.setAttribute("name", y);
                                do
                                {
                                    y = LineInput(95);
                                    if (y != "ENDFILE")
                                    {
                                        if (y != "END")
                                        {
                                            IXMLDOMElement myNewSong = x.createElement("song");
                                            myNewSong.setAttribute("track", Val("&H" + Left(y, 4)));
                                            myNewSong.text = Mid(y, 6);
                                            myNewList.appendChild(myNewSong);
                                        }
                                    }
                                } while (!(y == "END"));
                                myNewRom.appendChild(myNewList);
                            } while (true);

                            FileClose(95);
                        }
                    }
                    rootNode.appendChild(myNewRom);
                }
            }
        SkipThisShit:;
        } while (true);
        x.save(xfile);
        FileClose(96);
        IncessantNoises("TaskComplete");

        LoadGameFromXML(ref gamecode);
    }

    private void mnuMidiMap_Click(object sender, RoutedEventArgs e) { mnuMidiMap_Click(); }
    private void mnuMidiMap_Click()
    {
        frmMidiMapper.instance.ShowDialog();
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
        frmSelectMidiOut.instance.ShowDialog();
    }

    private void mnuSettings_Click(object sender, RoutedEventArgs e) { mnuSettings_Click(); }
    private void mnuSettings_Click()
    {
        frmOptions.instance.ShowDialog();
    }

    private void picChannels_Paint()
    {
        // StretchBlt picChannels.hdc, 0, 0, picChannels.ScaleWidth, 17, picSkin.hdc, 6, 16, 2, 17, vbSrcCopy
    }

    private void picScreenshot_DblClick(object sender, MouseButtonEventArgs e) { if (e.ClickCount != 2) return; picScreenshot_DblClick(); }
    private void picScreenshot_DblClick()
    {
        gCommonDialog cc = new();
        string s = (string)picScreenshot.Tag;
        string fileTile = null;
        bool readOnly = false;
        int filterIndex = 1;
        string filter = Properties.Resources._1 + "|*.BMP;*.GIF;*.JPG";
        string initDir = null;
        string dlgTitle = null;
        string defaultExt = null;
        int flags = 0;
        if (cc.VBGetOpenFileName(ref s, ref fileTile, ref readOnly, ref filter, ref filterIndex, ref initDir, ref dlgTitle, ref defaultExt, ref flags) == true)
        {
            s = cc.VBGetFileTitle(s);
            picScreenshot.Source = new BitmapImage(new(s));
            picScreenshot.Tag = s;
            SaveNewRomHeader("screenshot", s);
        }
    }

    private void picStatusbar_DblClick(object sender, MouseButtonEventArgs e) { picStatusbar_DblClick(); }
    private void picStatusbar_DblClick()
    {
        if ((int)picStatusbar.Tag >= 402 && (int)picStatusbar.Tag <= 436)
        {
            frmOptions.instance.Tag = "repsplz";
            frmOptions.instance.ShowDialog();
        }
    }

    private void picStatusbar_MouseMove(object sender, MouseEventArgs e) => CallMouseMove(e, this, picStatusbar_MouseMove);
    private void picStatusbar_MouseMove(int Button, int Shift, double x, double y)
    {
        picStatusbar.Tag = x + IIf(ebrContainer.Visibility == Visibility.Hidden, ebrContainer.Width, 0);
    }

    private void picStatusBar_Paint(object sender, DrawingContext e)
    {
        cStatusBar.Draw();
    }

    private void picTop_Paint(object sender, DrawingContext e)
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
        cStatusBar.PanelText("simple", statusText);
        // If status = 1 Then cStatusBar.PanelText("simple") = cStatusBar.PanelText("simple") & " (" & progress & "/" & total & ")"
        picStatusbar.Refresh();
    }

    private void SappyDecoder_SongFinish()
    {
        Playing = false;
        timPlay.IsEnabled = false;
        cmdPlay.setImage(ConvertBitmap((System.Drawing.Bitmap)AxHostConverter.PictureDispToImage((IPictureDisp)imlImages.ItemPicture(19))));
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

        for (int c = 1; c <= SappyDecoder.SappyChannels.count; c += 1)
        {
            int ct = 0;
            string ns = "";
            if (SappyDecoder.SappyChannels[c].Notes.count > 0)
            {
                foreach (SNote n in SappyDecoder.SappyChannels[c].Notes)
                {
                    if (SappyDecoder.NoteInfo[n.NoteID].Enabled == true && SappyDecoder.NoteInfo[n.NoteID].NoteOff == false)
                    {
                        ct += (int)(SappyDecoder.NoteInfo[n.NoteID].Velocity / 0x7F * (SappyDecoder.NoteInfo[n.NoteID].EnvPosition / 0xFF) * 0x7F);
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
            ct = ct / 127 * (SappyDecoder.SappyChannels[c].MainVolume / 127) * 255;
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
        int totalpercent = 326 / totallen * totalplayed;
        linProgress.X2 = totalpercent;
        // Caption = totalplayed & " / " & totallen & " -> " & totalpercent & "%"
        // Dim totalplayed As Long
        // With SappyDecoder.SappyChannels(1)
        //   tl = .TrackLengthInBytes - (.ProgramCounter + .TrackPointer)
        //   Caption = tl
        // End With

        cStatusBar.PanelText("crud", loopsToGo.ToString());
        cStatusBar.PanelText("time", Right("00" + TotalMinutes, 2) + ":" + Right("00" + TotalSeconds, 2) + " (" + SappyDecoder.Beats + ")");
        cStatusBar.PanelText("frame", SappyDecoder.TotalTicks.ToString());

        // If SappyDecoder.TotalTicks < 96 Then Debug.Print (96 - SappyDecoder.SappyChannels(2).WaitTicks) & " vs " & SappyDecoder.TotalTicks
        picStatusbar.Refresh();
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
            tOp = 0,
            Right = (int)picTop.Width,
            Bottom = (int)picTop.Height
        };
        BitBlt((int)picTop.hWnd(), panelRect.left, panelRect.tOp, 2, 2, (int)picSkin.hWnd(), 6, 0, vbSrcCopy);
        StretchBlt((int)picTop.hWnd(), panelRect.left + 2, panelRect.tOp, panelRect.Right - 4, 2, (int)picSkin.hWnd(), 6, 2, 2, 2, vbSrcCopy);
        BitBlt((int)picTop.hWnd(), panelRect.left + panelRect.Right - 2, panelRect.tOp, 2, 2, (int)picSkin.hWnd(), 6, 4, vbSrcCopy);
        StretchBlt((int)picTop.hWnd(), panelRect.left, panelRect.tOp + 2, 2, panelRect.Bottom - 4, (int)picSkin.hWnd(), 6, 6, 2, 2, vbSrcCopy);
        StretchBlt((int)picTop.hWnd(), panelRect.left + 2, panelRect.tOp + 2, panelRect.Right - 4, panelRect.Bottom - 4, (int)picSkin.hWnd(), 0, 0, 6, 62, vbSrcCopy);
        StretchBlt((int)picTop.hWnd(), panelRect.left + panelRect.Right - 2, panelRect.tOp + 2, 2, panelRect.Bottom - 4, (int)picSkin.hWnd(), 6, 8, 2, 2, vbSrcCopy);
        BitBlt((int)picTop.hWnd(), panelRect.left, panelRect.tOp + panelRect.Bottom - 2, 2, 2, (int)picSkin.hWnd(), 6, 10, vbSrcCopy);
        StretchBlt((int)picTop.hWnd(), panelRect.left + 2, panelRect.tOp + panelRect.Bottom - 2, panelRect.Right - 4, 2, (int)picSkin.hWnd(), 6, 12, 2, 2, vbSrcCopy);
        BitBlt((int)picTop.hWnd(), panelRect.left + panelRect.Right - 2, panelRect.tOp + panelRect.Bottom - 2, 2, 2, (int)picSkin.hWnd(), 6, 14, vbSrcCopy);
        // TODO: (NOT SUPPORTED): On Error Resume Next
        VolumeSlider1.BackColor = GetPixel((int)picSkin.hWnd(), 5, 42);
    }

    public void HandleClassicMode()
    {
        if (GetSettingI("Hide Bar") != 0)
        {
            ebrContainer.Visibility = Visibility.Hidden;
            Width = ClassicWidth;
            mywidth = (int)(Width / Screen.TwipsPerPixelX); // remember for wmSize subclass
            picTop.Move(0);
            picChannels.Move(0);
            cbxSongs.Height = 330 / Screen.TwipsPerPixelY;
        }
        else
        {
            ebrContainer.Visibility = Visibility.Visible;
            Width = FullWidth;
            mywidth = (int)(Width / Screen.TwipsPerPixelX); // remember for wmSize subclass
            picTop.Move(ebrContainer.Width);
            picChannels.Move(ebrContainer.Width);
            cbxSongs.Height = 330 / Screen.TwipsPerPixelY;
        }
    }

    public void FixStatusBar()
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        cStatusBar.RemovePanel("simple");
        cStatusBar.RemovePanel("frame");
        cStatusBar.RemovePanel("crud");
        cStatusBar.RemovePanel("time");
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        cStatusBar.AddPanel(ENSBRPanelStyleConstants.estbrNoBorders, "", bSpring: true, sKey: "simple");
        cStatusBar.AddPanel(ENSBRPanelStyleConstants.estbrStandard, "0", bSpring: false, sKey: "frame");
        cStatusBar.AddPanel(ENSBRPanelStyleConstants.estbrStandard, "0", 0, 24, false, sKey: "crud");
        cStatusBar.AddPanel(ENSBRPanelStyleConstants.estbrStandard, "00:00 (0)", 1, 64, false, sKey: "time");
        picStatusbar.Refresh();
    }

    private void PrepareRecording()
    {
        string target = "";
        gCommonDialog cc = new();
        string fileTitle = lblSongName.Content + ".mid";
        string filter = "Type 0 MIDI (*.mid)|*.mid";
        int filterIndex = 1;
        int flags = 0;
        if (cc.VBGetSaveFileName(ref target, ref fileTitle, ref filter, ref filterIndex, ref flags, DefaultExt: "mid") == false) return;
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
