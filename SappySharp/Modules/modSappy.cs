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
using System.Diagnostics;
using SappySharp.Properties;
using SappySharp.Classes;
using Microsoft.Win32;

static partial class modSappy
{
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
    // __________________________________
    // |  Support functions and API shit  |
    // |¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    // | Code-by-Kawa % impossible to gauge.                |
    // | Last update: September 21st, 2006                  |
    // |____________________________________________________|

    // ###########################################################################################



    [LibraryImport("gdi32.dll")]
    public static partial int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
    [LibraryImport("gdi32.dll")]
    public static partial int StretchBlt(int hdc, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, int dwRop);
    [DllImport("shell32.dll", EntryPoint = "ShellExecuteA")]
    public static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
    [LibraryImport("user32", EntryPoint = "SetWindowLongA")]
    public static partial int SetWindowLong(int hwnd, int nIndex, int dwNewLong);
    [LibraryImport("user32")]
    public static partial int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags);
    [LibraryImport("user32", EntryPoint = "GetWindowLongA")]
    public static partial int GetWindowLong(int hwnd, int nIndex);
    [DllImport("kernel32.dll", EntryPoint = "GetComputerNameA")]
    public static extern int GetComputerName(string lpBuffer, ref int nSize);
    [DllImport("kernel32.dll", EntryPoint = "GetWindowsDirectoryA")]
    public static extern int GetWindowsDirectory(string lpBuffer, int nSize);
    [DllImport("advapi32.dll", EntryPoint = "GetUserNameA")]
    public static extern int GetUserName(string lpBuffer, ref int nSize);
    [LibraryImport("kernel32.dll")]
    public static partial int GetVersion();
    [DllImport("COMCTL32.DLL")]
    public static extern bool InitCommonControlsEx(ref tagInitCommonControlsEx iccex);
    [DllImport("user32", EntryPoint = "LoadImageA")]
    public static extern int LoadImageAsString(IntPtr hInst, string lpsz, int uType, int cxDesired, int cyDesired, int fuLoad);
    [LibraryImport("user32", EntryPoint = "SendMessageA")]
    public static partial int SendMessageLong(int hwnd, int wMsg, int wParam, int lParam);
    [LibraryImport("user32")]
    public static partial int GetWindow(int hwnd, int wCmd);
    [LibraryImport("user32")]
    public static partial int GetSystemMetrics(int nIndex);
    [LibraryImport("kernel32")]
    public static partial int GetUserDefaultLCID();
    [DllImport("user32.dll")]
    public static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);

    [LibraryImport("user32", EntryPoint = "SendMessageA")]
    private static partial int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
    [DllImport("user32", EntryPoint = "FindWindowA")]
    private static extern int FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32", EntryPoint = "FindWindowExA")]
    private static extern int FindWindowEx(int hWnd1, int hWnd2, string lpsz1, string lpsz2);

    public class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }

    class COPYDATASTRUCT
    {
        public int dwData;
        public int cbData;
        public int lpData;
    }

    public const int WM_COPYDATA = 0x4A;

    [DllImport("winmm.dll", EntryPoint = "sndPlaySoundA")]
    public static extern int sndPlaySound(string lpszSoundName, int uFlags);
    public const int SND_ASYNC = 0x1;

    public const int SWP_NOMOVE = 0x2;
    public const int SWP_NOSIZE = 0x1;
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_CLIENTEDGE = 0x200;
    public const int WS_EX_STATICEDGE = 0x20000;
    public const int SWP_FRAMECHANGED = 0x20;
    public const int SWP_NOACTIVATE = 0x10;
    public const int SWP_NOZORDER = 0x4;

    // XP styles stuff
    [StructLayout(LayoutKind.Sequential)]
    public class tagInitCommonControlsEx
    {
        public int lngSize;
        public uint lngICC;
    }
    public const int ICC_USEREX_CLASSES = 0x200;

    // Pretty icon stuff
    public const int SM_CXICON = 11;
    public const int SM_CYICON = 12;

    public const int SM_CXSMICON = 49;
    public const int SM_CYSMICON = 50;

    public const int LR_DEFAULTCOLOR = 0x0;
    public const int LR_MONOCHROME = 0x1;
    public const int LR_COLOR = 0x2;
    public const int LR_COPYRETURNORG = 0x4;
    public const int LR_COPYDELETEORG = 0x8;
    public const int LR_LOADFROMFILE = 0x10;
    public const int LR_LOADTRANSPARENT = 0x20;
    public const int LR_DEFAULTSIZE = 0x40;
    public const int LR_VGACOLOR = 0x80;
    public const int LR_LOADMAP3DCOLORS = 0x1000;
    public const int LR_CREATEDIBSECTION = 0x2000;
    public const int LR_COPYFROMRESOURCE = 0x4000;
    public const int LR_SHARED = 0x8000;

    public const int IMAGE_ICON = 1;

    public const int WM_SETICON = 0x80;

    public const int ICON_SMALL = 0;
    public const int ICON_BIG = 1;

    public const int GW_OWNER = 4;

    public static bool MSNPlaying = false;
    public static string ProperFont = "";
    public static int ProperFontS = 0;

    [LibraryImport("gdi32.dll")]
    public static partial int SetPixel(int hdc, int x, int y, int crColor);
    [LibraryImport("gdi32.dll")]
    public static partial int GetPixel(int hdc, int x, int y);

    public static void MainStart()
    {
        System.Reflection.AssemblyName assemblyName = Application.ResourceAssembly.GetName();
        Trace("This is " + assemblyName.Name + " version " + assemblyName.Version.Major + "." + assemblyName.Version.Minor + "." + assemblyName.Version.Revision);
        Trace("----------------------------------------");
        Trace("Startup Procedure Engaged!");
        Trace("Calling InitCommonControlsVB");
        InitCommonControlsVB();
        Trace("Retrieving font data from Registry");
        ProperFont = GetSetting("Window Font");
        if (ProperFont == "") ProperFont = "Lucida Sans Unicode";
        ProperFontS = GetSettingI("Window Font Size");
        if (ProperFontS == 0) ProperFontS = 8;
        Trace("On to the main form...");
        //new frmSappy().Show(); // Called by App.xaml
    }

    // -------------------------------
    // SetIcon
    // -------

    // Replaces the given window's icon with one loaded from a resource file, and loaded -properly-
    // as to include proper scaling and shadows. VB6 doesn't quite cut it.
    // Does not work in IDE mode, wherein it turns the icon into a generic Windows app icon.

    // Found on vbAccellerator.

    public static void SetIcon(int hwnd, string sIconResName, bool bSetAsAppIcon = true)
    {
        int lhWndTop = 0;

        if (bSetAsAppIcon)
        {
            // Find VB's hidden parent window:
            int lhWnd = hwnd;
            lhWndTop = lhWnd;
            while (!(lhWnd == 0))
            {
                lhWnd = GetWindow(lhWnd, GW_OWNER);
                if (!(lhWnd == 0))
                {
                    lhWndTop = lhWnd;
                }
            }
        }

        int cX = GetSystemMetrics(SM_CXICON);
        int cY = GetSystemMetrics(SM_CYICON);
        nint hInstance = Process.GetCurrentProcess().Handle;
        int hIconLarge = LoadImageAsString(hInstance, sIconResName, IMAGE_ICON, cX, cY, LR_SHARED);
        if (bSetAsAppIcon)
        {
            SendMessageLong(lhWndTop, WM_SETICON, ICON_BIG, hIconLarge);
        }
        SendMessageLong(hwnd, WM_SETICON, ICON_BIG, hIconLarge);

        cX = GetSystemMetrics(SM_CXSMICON);
        cY = GetSystemMetrics(SM_CYSMICON);
        int hIconSmall = LoadImageAsString(hInstance, sIconResName, IMAGE_ICON, cX, cY, LR_SHARED);
        if (bSetAsAppIcon)
        {
            SendMessageLong(lhWndTop, WM_SETICON, ICON_SMALL, hIconSmall);
        }
        SendMessageLong(hwnd, WM_SETICON, ICON_SMALL, hIconSmall);
    }

    // -------------------------------
    // InitCommonControlsVB
    // --------------------

    // Allows the use of XP styles without using any Common Controls.

    // Found on vbAccellerator.

    public static bool InitCommonControlsVB()
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        tagInitCommonControlsEx iccex = new();
        iccex.lngSize = Marshal.SizeOf(iccex);
        iccex.lngICC = ICC_USEREX_CLASSES;
        InitCommonControlsEx(ref iccex);
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        return Err().Number == 0;
    }

    // -------------------------------
    // FixHex
    // ------

    // Given a value and the padding length, returns a padded Hex value.

    public static string FixHex(string s, int i)
    {
        string Bleh = Replace(s, "0x", "&H");
        if (Left(Bleh, 2) != "&H")
        {
            Bleh = "&H" + Hex(Val(s));
        }
        string _FixHex = Right("00000000" + Bleh, i);
        _FixHex = Replace(_FixHex, "&", "");
        _FixHex = Replace(_FixHex, "H", "");
        return _FixHex;
    }
    public static string FixHex(int s, int i) => Right("00000000" + s.ToString("X8"), i);

    // -------------------------------
    // SetCaptions
    // -----------

    // Loads resource strings for each suitable control on a given form and sets its font
    // to whatever your locale needs.

    public static void SetCaptions(Window target)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        foreach (FrameworkElement ctl in target.getControls())
        {
            if (ctl is not TextBlock and not ContentControl and not HeaderedItemsControl) continue;

            if ((string)ctl.Tag != "[NoLocal]")
            {
                DependencyProperty textProperty = ctl switch
                {
                    TextBlock => TextBlock.TextProperty,
                    HeaderedItemsControl => HeaderedItemsControl.HeaderProperty,
                    ContentControl => ContentControl.ContentProperty,
                    _ => throw new NotImplementedException()
                };

                string oldText = ctl.GetValue(textProperty) as string;
                if (Left(oldText, 1) == "[")
                {
                    int i = (int)Val(Mid(oldText, 2, 4));
                    string newText = Resources.ResourceManager.GetString(i.ToString())?.Replace('&', '_');
                    ctl.SetValue(textProperty, newText);
                }
                SetProperFont(ctl);
            }
        }
        // TODO: (NOT SUPPORTED): On Error GoTo 0
    }

    // -------------------------------
    // SetProperFont
    // -------------

    // Sets a control to the Japanese codepage if running on a Japanese system.
    // This allows the use of DBCS strings in their captions.

    // Adapted from an article in the VB docs on MSDN.

    public static void SetProperFont(FrameworkElement obj)
    {
        // If GetUserDefaultLCID = &H411 Then
        try
        {
            if (obj is not Control ctl) return;
            if (Resources._10000 == "<JAPPLZ>")
            {
                //obj.Charset = 128;
                ctl.FontFamily = new("MS Gothic"); // ChrW(&HFF2D) + ChrW(&HFF33) + ChrW(&H20) + ChrW(&HFF30) + ChrW(&H30B4) + ChrW(&H30B7) + ChrW(&H30C3) + ChrW(&H30AF)
                ctl.FontSize = 9;
            }
            else
            { // If (LoadResString(10000)) = "<NLPLZ>" Then
                ctl.FontFamily = new(ProperFont); // "Lucida Sans Unicode" '"Comic Sans MS"
                ctl.FontSize = ProperFontS;
            }
        }
        catch (Exception ex)
        {
            Err().Number = ex.HResult;
        }
        return;
    }

    // -------------------------------
    // ClickSound
    // ----------

    // Just a macro...

    public static void ClickSound()
    {
        IncessantNoises("ButtonClick");
    }

    // -------------------------------
    // IncessantNoises
    // ---------------

    // Plays the given sound, as specified in the Sounds CPL.

    public static void IncessantNoises(string n)
    {
        cRegistry myReg = new();

        if (GetSettingI("Incessant Sound Override") != 0) return;

        myReg.ClassKey = RegistryHive.CurrentUser;
        myReg.SectionKey = "AppEvents\\Schemes\\Apps\\Sappy2k5\\Sappy2k5-" + n + "\\.current";
        myReg.ValueKey = "";
        myReg.ValueType = RegistryValueKind.String;
        sndPlaySound(myReg.Value, SND_ASYNC);
        // Dim s As String
        // s = GetSetting("Incessant Sound Override")
        // If s = " 1" Then Exit Sub

        // s = GetKeyValue(HKEY_CURRENT_USER, "AppEvents\Schemes\Apps\Sappy2k5\Sappy2k5-" & n & "\.current", "")
        // If Trim(s) <> "" Then
        // sndPlaySound s, SND_ASYNC
        // End If
    }

    // -------------------------------
    // TellMSN and ShutMSN
    // -------------------

    // Wrappers around SetMusicInfo to better manage Now Playing information in MSN Messenger.

    public static void TellMSN(string sTitle, string sArtist, string sAlbum)
    {
        MSNPlaying = true;
        SetMusicInfo(sArtist, sAlbum, sTitle, "", "{0} - {2} - {1}", true);
    }
    public static void ShutMSN()
    {
        if (MSNPlaying == false)
        {
            Trace("Spurious ShutMSN.");
            return;
        }
        SetMusicInfo("", "", "", "", "", false);
        MSNPlaying = false;
    }

    public static void SetMusicInfo(string r_sArtist, string r_sAlbum, string r_sTitle, string r_sWMContentID = vbNullString, string r_sFormat = "{0} - {1}", bool r_bShow = true)
    {
        COPYDATASTRUCT udtData = null;
        int hMSGRUI = 0;

        string sBuffer = "\\0Music\\0" + (r_bShow ? 1 : 0) + "\\0" + r_sFormat + "\\0" + r_sArtist + "\\0" + r_sTitle + "\\0" + r_sAlbum + "\\0" + r_sWMContentID + "\\0" + vbNullChar;
        Trace("Sending to Messenger: \"" + sBuffer + "\"");

        udtData.dwData = 0x547;
        var ghBuffer = GCHandle.Alloc(sBuffer, GCHandleType.Pinned);
        udtData.lpData = (int)ghBuffer.AddrOfPinnedObject();
        udtData.cbData = Marshal.SizeOf(sBuffer);
        var ghData = GCHandle.Alloc(udtData, GCHandleType.Pinned);

        do
        {
            hMSGRUI = FindWindowEx(0, hMSGRUI, "MsnMsgrUIManager", vbNullString);
            if (hMSGRUI > 0) SendMessage(hMSGRUI, WM_COPYDATA, 0, (int)ghData.AddrOfPinnedObject());
        } while (!(hMSGRUI == 0));

        ghBuffer.Free();
        ghData.Free();
    }

    public static void DrawSkin(FrameworkElement Victim)
    {
        BitBlt((int)Victim.hWnd(), 0, 0, 2, 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 0, vbSrcCopy);
        StretchBlt((int)Victim.hWnd(), 2, 0, (int)(Victim.Width - 4), 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 2, 2, 2, vbSrcCopy);
        BitBlt((int)Victim.hWnd(), (int)(Victim.Width - 2), 0, 2, 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 4, vbSrcCopy);
        StretchBlt((int)Victim.hWnd(), 0, 2, 2, (int)(Victim.Height - 4), (int)frmSappy.instance.picSkin.hWnd(), 6, 6, 2, 2, vbSrcCopy);
        StretchBlt((int)Victim.hWnd(), 2, 2, (int)(Victim.Width - 4), (int)(Victim.Height - 4), (int)frmSappy.instance.picSkin.hWnd(), 0, 0, 6, 62, vbSrcCopy);
        StretchBlt((int)Victim.hWnd(), (int)(Victim.Width - 2), 2, 2, (int)(Victim.Height - 4), (int)frmSappy.instance.picSkin.hWnd(), 6, 8, 2, 2, vbSrcCopy);
        BitBlt((int)Victim.hWnd(), 0, (int)(Victim.Height - 2), 2, 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 10, vbSrcCopy);
        StretchBlt((int)Victim.hWnd(), 2, (int)(Victim.Height - 2), (int)(Victim.Width - 4), 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 12, 2, 2, vbSrcCopy);
        BitBlt((int)Victim.hWnd(), (int)(Victim.Width - 2), (int)(Victim.Height - 2), 2, 2, (int)frmSappy.instance.picSkin.hWnd(), 6, 14, vbSrcCopy);
    }

    public static void SetAllSkinButtons(Window Victim)
    {
        // Dim ct As Control
        // For Each ct In Victim.Controls
        // If TypeName(ct) = "SkinButton" Then ct.SkinDC = frmSappy.picSkin.hdc
        // Next
        Trace("Stop calling SetAllSkinButtons! You don't have to do that anymore! --> Victim: " + Victim.Name);
    }

    public static string InputBox(string Prompt, ref string Title, string Default)
    {
        if (Title == "") Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        frmInputBox.instance.Label1.Content = Prompt;
        frmInputBox.instance.Title = Title;
        frmInputBox.instance.Text1.Text = Default;
        frmInputBox.instance.Text1.SelectionStart = 0;
        frmInputBox.instance.Text1.SelectionLength = Len(frmInputBox.instance.Text1);
        frmInputBox.instance.ShowDialog();
        return frmInputBox.instance.Text1.Text;
    }

    public static string GetSetting(string name)
    {
        cRegistry myReg = new()
        {
            ClassKey = RegistryHive.CurrentUser,
            Default = null,
            SectionKey = "Software\\Helmeted Rodent\\Sappy 2006",
            ValueKey = name,
            ValueType = RegistryValueKind.String
        };
        return myReg.Value;
    }
    public static int GetSettingI(string name)
    {
        cRegistry myReg = new()
        {
            ClassKey = RegistryHive.CurrentUser,
            Default = 0,
            SectionKey = "Software\\Helmeted Rodent\\Sappy 2006",
            ValueKey = name,
            ValueType = RegistryValueKind.DWord
        };
        return (int)myReg.Value;
    }
    public static void WriteSetting(string name, dynamic Value)
    {
        _ = new cRegistry()
        {
            ClassKey = RegistryHive.CurrentUser,
            SectionKey = "Software\\Helmeted Rodent\\Sappy 2006",
            ValueKey = name,
            ValueType = RegistryValueKind.String,
            Value = Value
        };
    }
    public static void WriteSettingI(string name, int Value)
    {
        _ = new cRegistry()
        {
            ClassKey = RegistryHive.CurrentUser,
            SectionKey = "Software\\Helmeted Rodent\\Sappy 2006",
            ValueKey = name,
            ValueType = RegistryValueKind.DWord,
            Value = Int(Value)
        };
    }
}
