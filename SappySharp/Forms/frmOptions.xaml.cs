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

namespace SappySharp.Forms;

public partial class frmOptions : Window
{
    private static frmOptions _instance;
    public static frmOptions instance { set { _instance = null; } get { return _instance ??= new frmOptions(); } }
    public static void Load() { if (_instance == null) { dynamic A = frmOptions.instance; } }
    public static void Unload() { if (_instance != null) instance.Close(); _instance = null; }
    public frmOptions() { InitializeComponent(); }

    public List<Image> picPage { get => VBExtension.controlArray<Image>(this, "picPage"); }

    public List<Image> picSkin { get => VBExtension.controlArray<Image>(this, "picSkin"); }

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
    // |  Options dialog  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code 100% by Kyoufu Kawa.                          |
    // | Last update: April 1st, 2006                       |
    // |____________________________________________________|

    // ###########################################################################################


    private const int WM_MOUSEWHEEL = 0x20A;

    private void cbxPresets_Click(object sender, RoutedEventArgs e) { cbxPresets_Click(); }
    private void cbxPresets_Click()
    {
        int r = 0;
        int g = 0;
        int b = 0;
        decimal H = 0;
        decimal l = 0;
        decimal s = 0;
        int color = (int)cbxPresets.Items[cbxPresets.SelectedIndex];
        SplitRGB(ref color, ref r, ref g, ref b);
        cbxPresets.Items[cbxPresets.SelectedIndex] = color;
        RGBToHLS(r, g, b, ref H, ref l, ref s);
        HScroll1.Value = (double)(H * 10); // - 1
        HScroll2.Value = (double)(s * 10); // - 1
    }

    private void chkNiceBar_Click(object sender, RoutedEventArgs e) { chkNiceBar_Click(); }
    private void chkNiceBar_Click()
    {
        Image1.Margin = new Thickness(chkNiceBar.IsChecked.GetValueOrDefault() ? -32 : 0, Image1.Margin.Top, Image1.Margin.Right, Image1.Margin.Bottom);
    }

    private void cmdOK_Click(object sender, RoutedEventArgs e) { cmdOK_Click(); }
    private void cmdOK_Click()
    {
        int i = int.Parse(txtReps.Text);

        frmSappy.instance.xfile = AppContext.BaseDirectory + "\\" + txtXFile.Text;
        WriteSetting("XML File", txtXFile.Text);
        WriteSettingI("Reload ROM", chkReload.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("mIRC Now Playing", chkMIRC.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("MSN Now Playing", chkMSN.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("Incessant Sound Override", chkSounds.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("Force Nice Bar", chkNiceBar.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("Hide Bar", chkHideBar.IsChecked.GetValueOrDefault() ? 1 : 0);
        WriteSettingI("Song Repeats", i);

        WriteSetting("Skin", picSkin[0].Source);
        WriteSetting("Skin Hue", HScroll1.Value / 10);
        WriteSetting("Skin Saturation", HScroll2.Value / 10);

        frmSappy.instance.HandleClassicMode();
        frmSappy.instance.FixStatusBar();
        frmSappy.instance.ebr.UseExplorerStyle = !chkNiceBar.IsChecked.GetValueOrDefault();
        frmSappy.instance.picSkin.Source = picSkin[0].Source;
        Colorize(frmSappy.instance.picSkin, (decimal)HScroll1.Value / 10, (decimal)HScroll2.Value / 10);
        frmSappy.instance.RedrawSkin();
        frmSappy.instance.ebr.Redraw = false;
        frmSappy.instance.ebr.BackColorStart = GetPixelColor((BitmapSource)frmSappy.instance.picSkin.Source, 6, 16);
        frmSappy.instance.ebr.BackColorEnd = GetPixelColor((BitmapSource)frmSappy.instance.picSkin.Source, 6, 32);
        frmSappy.instance.ebr.Redraw = true;

        ClickSound();
        Unload();
    }

    private void Command1_Click(object sender, RoutedEventArgs e) { Command1_Click(); }
    private void Command1_Click()
    {
        ClickSound();
        Unload();
    }

    private void Form_Load(object sender, RoutedEventArgs e) { Form_Load(); }
    private void Form_Load()
    {
        for (int i = 0; i < picPage.Count; i += 1)
        {
            //picPage[i].BorderStyle = 0;
            picPage[i].Visibility = Visibility.Hidden;
            picPage[i].Move(136, 32);
        }
        //TODO: Support MSN ?
        //MSNList1.AddItem(Properties.Resources._6011); // Playback
        //MSNList1.AddItem(Properties.Resources._6005); // Now Playing
        //MSNList1.AddItem(Properties.Resources._6008); // Interface
        //MSNList1.AddItem(Properties.Resources._6012); // Skin
        //// MSNList1.ListIndex = 0
        //// TODO: (NOT SUPPORTED): On Error Resume Next
        //i = GetSettingI("Settings Page");
        //if (i > MSNList1.ListCount - 1) i = 0;
        //MSNList1.SelectedIndex = i;

        //AttachMessage(this, hwnd, WM_MOUSEWHEEL);

        //picPage[3].ScaleMode = 3;

        SetCaptions(this);
        Title = Properties.Resources._6000;
        Picture1.Source = frmSappy.instance.imlStatusbar.ItemPicture(3);
        Picture2.Source = frmSappy.instance.imlStatusbar.ItemPicture(4);

        txtXFile.Text = GetSetting("XML File");
        if (txtXFile.Text == "") txtXFile.Text = "sappy.xml";
        // TODO: (NOT SUPPORTED): On Error Resume Next
        chkReload.IsChecked = GetSettingI("Reload ROM") != 0;
        chkMIRC.IsChecked = GetSettingI("mIRC Now Playing") != 0;
        chkMSN.IsChecked = GetSettingI("MSN Now Playing") != 0;
        chkSounds.IsChecked = GetSettingI("Incessant Sound Override") != 0;
        chkNiceBar.IsChecked = GetSettingI("Force Nice Bar") != 0;
        chkHideBar.IsChecked = GetSettingI("Hide Bar") != 0;
        txtReps.Text = GetSettingI("Song Repeats").ToString();
        if (txtReps.Text == "") txtReps.Text = "2";

        decimal hue;
        decimal sat;
        int skinno;
        // skinno = GetSettingI("Skin")
        string regset = GetSetting("Skin");
        if (regset != "") skinno = (int)Val(regset); else skinno = 0;
        regset = GetSetting("Skin Hue");
        if (regset != "") hue = (decimal)Val(Replace(regset, ",", ".")); else hue = 3.4m;
        regset = GetSetting("Skin Saturation");
        if (regset != "") sat = (decimal)Val(Replace(regset, ",", ".")); else sat = 0.4m;

        // TODO: (NOT SUPPORTED): On Error GoTo 0

        Image1.Source = ConvertBitmap(Properties.Resources.BARSAMPLES);
        Image1.Margin = new Thickness(chkNiceBar.IsChecked.GetValueOrDefault() ? -32 : 0, Image1.Margin.Top, Image1.Margin.Right, Image1.Margin.Bottom);

        HScroll1.Value = (double)(hue * 10);
        HScroll2.Value = (double)(sat * 10);

        picSkin[0].Tag = skinno;
        shpSkinSel.Move(picSkin[skinno].Margin.Left - 3);

        int r = 0;
        int g = 0;
        int b = 0;
        HLSToRGB(3.4m, 0.5m, 0.4m, ref r, ref g, ref b);
        cbxPresets.Items.Add(CreateComboBoxItemWithData("MSN-style", r, g, b)); // 0xB47C30 // 508CB4
        HLSToRGB(0, 1, 0, ref r, ref g, ref b);
        cbxPresets.Items.Add(CreateComboBoxItemWithData("Grayscale", r, g, b));
        HLSToRGB(0.4m, 1, 0.2m, ref r, ref g, ref b);
        cbxPresets.Items.Add(CreateComboBoxItemWithData("Rusty Nailz", r, g, b));
        HLSToRGB(2.4m, 1, 0.3m, ref r, ref g, ref b);
        cbxPresets.Items.Add(CreateComboBoxItemWithData("Green Dreem", r, g, b));
    }

    private static ComboBoxItem CreateComboBoxItemWithData(string text, int r, int g, int b)
    {
        ComboBoxItem item = new()
        {
            Content = text,
            Foreground = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b))
        };
        return item;
    }

    private void Form_Resize(object sender, SizeChangedEventArgs e)
    {
        if ((string)Tag == "repsplz")
        {
            //MSNList1.SelectedIndex = 0;
            txtReps.SetFocus();
            Tag = "";
        }
    }

    private void Form_Unload(ref int Cancel)
    {
        //DetachMessage(this, hwnd, WM_MOUSEWHEEL);
    }

    private void HScroll1_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        HScroll1_Scroll();
    }
    private void HScroll2_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        HScroll2_Scroll();
    }

    private void HScroll1_Scroll()
    {
        decimal i = (decimal)(HScroll1.Value / 10);
        Label10.Content = i;
        DrawColorBar();
    }
    private void HScroll2_Scroll()
    {
        decimal i = (decimal)(HScroll2.Value / 10);
        Label11.Content = i;
        DrawColorBar();
    }

    //TODO: Support MSN ?
    //private int ISubclass_WindowProc(int hwnd, int iMsg, int wParam, int lParam)
    //{
    //    int _ISubclass_WindowProc = 0;
    //    if (iMsg == WM_MOUSEWHEEL)
    //    {
    //        if (wParam > 0)
    //        {
    //            MSNList1.SelectedIndex = MSNList1.SelectedIndex > 0 ? MSNList1.SelectedIndex - 1 : 0;
    //            MSNList1.Redraw();
    //        }
    //        else if (wParam < 0)
    //        {
    //            MSNList1.SelectedIndex = MSNList1.SelectedIndex < MSNList1.ListCount - 1 ? MSNList1.SelectedIndex + 1 : MSNList1.ListCount - 1;
    //            MSNList1.Redraw();
    //        }
    //    }
    //    return _ISubclass_WindowProc;
    //}

    private void MSNList1_Click(object sender, RoutedEventArgs e) { MSNList1_Click(); }
    private void MSNList1_Click()
    {
        for (int i = 0; i < picPage.Count; i += 1)
        {
            picPage[i].Visibility = Visibility.Hidden;
        }
        //picPage[MSNList1.ListIndex].Source = true;
        //lblHeader.Content = MSNList1.List[MSNList1.ListIndex].DefaultProperty;
        //WriteSettingI("Settings Page", MSNList1.SelectedIndex);
    }

    private void picSkin_Click(object sender, MouseButtonEventArgs e) { picSkin_Click(picSkin.IndexOf((Image)sender)); }
    private void picSkin_Click(int Index)
    {
        shpSkinSel.Move(picSkin[Index].Margin.Left - 3);
        picSkin[0].Tag = Index;
    }

    private void picSkin_Rendering(object sender, DrawingContext e)
    {
        picSkin_Paint(picSkin.IndexOf((Image)sender));
    }
    private void picSkin_Paint(int Index)
    {
        System.Drawing.Bitmap skin = Index switch
        {
            0 => Properties.Resources.Skin100,
            1 => Properties.Resources.Skin101,
            2 => Properties.Resources.Skin102,
            _ => throw new NotImplementedException()
        };
        picSkinLoad.Source = ConvertBitmap(skin);
        BitBlt((int)picSkin[Index].hWnd(), 0, 0, 16, 16, (int)picSkinLoad.hWnd(), 8, 0, vbSrcCopy);
    }

    private void Picture5_Paint(object sender, RoutedEventArgs e)
    {
        DrawColorBar();
    }

    private void txtReps_LostFocus(object sender, RoutedEventArgs e) { txtReps_LostFocus(); }
    private void txtReps_LostFocus()
    {
        txtReps.Text = Val(txtReps.Text).ToString();
    }

    private void DrawColorBar()
    {
        int r = 0;
        int g = 0;
        int b = 0;
        System.Drawing.Bitmap image = new(100, 100);
        System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(image);
        for (int i = 0; i < 1; i += 1)
        {
            HLSToRGB((decimal)(HScroll1.Value / 10), (decimal)(HScroll2.Value / 10), i / 0.01m, ref r, ref g, ref b);
            graph.DrawLine(new System.Drawing.Pen(System.Drawing.Color.FromArgb(r, g, b)), new System.Drawing.Point(i, 0), new System.Drawing.Point(i + 1, 1));
        }
        Picture5.Source = ConvertBitmap(image);
    }
}
