using VB6 = Microsoft.VisualBasic.Compatibility.VB6;
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
using static Microsoft.VisualBasic.Globals;
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
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ColorConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.DrawStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.FillStyleConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.GlobalModule;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.Printer;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterCollection;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.PrinterObjectConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.ScaleModeConstants;
using static Microsoft.VisualBasic.PowerPacks.Printing.Compatibility.VB6.SystemColorConstants;
using ADODB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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



static class modSappy {
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



[DllImport("gdi32.dll")]
public static extern int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
[DllImport("gdi32.dll")]
public static extern int StretchBlt(int hdc, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, int dwRop);
[DllImport("kernel32", EntryPoint="RtlMoveMemory")]
public static extern void CopyMemory(ref dynamic Destination, ref dynamic Source, int Length);
[DllImport("shell32.dll", EntryPoint="ShellExecuteA")]
public static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
[DllImport("user32", EntryPoint="SetWindowLongA")]
public static extern int SetWindowLong(int hwnd, int nIndex, int dwNewLong);
[DllImport("user32")]
public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags);
[DllImport("user32", EntryPoint="GetWindowLongA")]
public static extern int GetWindowLong(int hwnd, int nIndex);
[DllImport("kernel32.dll", EntryPoint="GetComputerNameA")]
public static extern int GetComputerName(string lpBuffer, ref int nSize);
[DllImport("kernel32.dll", EntryPoint="GetWindowsDirectoryA")]
public static extern int GetWindowsDirectory(string lpBuffer, int nSize);
[DllImport("advapi32.dll", EntryPoint="GetUserNameA")]
public static extern int GetUserName(string lpBuffer, ref int nSize);
[DllImport("kernel32.dll")]
public static extern int GetVersion();
[DllImport("COMCTL32.DLL")]
public static extern bool InitCommonControlsEx(ref tagInitCommonControlsEx iccex);
[DllImport("user32", EntryPoint="LoadImageA")]
public static extern int LoadImageAsString(int hInst, string lpsz, int uType, int cxDesired, int cyDesired, int fuLoad);
[DllImport("user32", EntryPoint="SendMessageA")]
public static extern int SendMessageLong(int hwnd, int wMsg, int wParam, int lParam);
[DllImport("user32")]
public static extern int GetWindow(int hwnd, int wCmd);
[DllImport("user32")]
public static extern int GetSystemMetrics(int nIndex);
[DllImport("kernel32")]
public static extern int GetUserDefaultLCID();
[DllImport("user32.dll")]
public static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);

[DllImport("user32", EntryPoint="SendMessageA")]
private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
[DllImport("user32", EntryPoint="FindWindowA")]
private static extern int FindWindow(string lpClassName, string lpWindowName);
[DllImport("user32", EntryPoint="FindWindowExA")]
private static extern int FindWindowEx(int hWnd1, int hWnd2, string lpsz1, string lpsz2);

  public class RECT{ 
  public int left;
  public int tOp;
  public int Right;
  public int Bottom;
}

  class COPYDATASTRUCT{ 
  public int dwData;
  public int cbData;
  public int lpData;
}

public const int WM_COPYDATA = 0x4A;

[DllImport("winmm.dll", EntryPoint="sndPlaySoundA")]
public static extern int sndPlaySound(string lpszSoundName, int uFlags);
public const int SND_ASYNC = 0x1;

public const int SWP_NOMOVE = 0x2;
public const int SWP_NOSIZE = 0x1;
public const var GWL_EXSTYLE = ((-20));
public const int WS_EX_CLIENTEDGE = 0x200;
public const int WS_EX_STATICEDGE = 0x20000;
public const int SWP_FRAMECHANGED = 0x20;
public const int SWP_NOACTIVATE = 0x10;
public const int SWP_NOZORDER = 0x4;

 // XP styles stuff
  public class tagInitCommonControlsEx{ 
  public int lngSize;
  public int lngICC;
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

[DllImport("gdi32.dll")]
public static extern int SetPixel(int hdc, int x, int y, int crColor);
[DllImport("gdi32.dll")]
public static extern int GetPixel(int hdc, int x, int y);

  public static void Main() {
  Trace("This is " + App.ProductName + " version " + App.Major + "." + App.Minor + "." + App.Revision);
  Trace("----------------------------------------");
  Trace("Startup Procedure Engaged!");
  Trace("Calling InitCommonControlsVB");
  InitCommonControlsVB();
  Trace("Retrieving font data from Registry");
  ProperFont = GetSetting(ref "Window Font");
  if(ProperFont == "")ProperFont = "Lucida Sans Unicode";
  ProperFontS = GetSettingI(ref "Window Font Size");
  if(ProperFontS == 0)ProperFontS = 8;
  Trace("On to the main form...");
  frmSappy.Show();
}

 // -------------------------------
 // SetIcon
 // -------

 // Replaces the given window's icon with one loaded from a resource file, and loaded -properly-
 // as to include proper scaling and shadows. VB6 doesn't quite cut it.
 // Does not work in IDE mode, wherein it turns the icon into a generic Windows app icon.

 // Found on vbAccellerator.

  public static void SetIcon(int hwnd, string sIconResName, bool bSetAsAppIcon = true) {
  int lhWndTop = 0;
  int lhWnd = 0;
  int cX = 0;
  int cY = 0;
  int hIconLarge = 0;
  int hIconSmall = 0;
  
    if(((bSetAsAppIcon))) {
     // Find VB's hidden parent window:
    lhWnd = hwnd;
    lhWndTop = lhWnd;
      while(! (lhWnd == 0)) {
      lhWnd = GetWindow(lhWnd, GW_OWNER);
        if(! (lhWnd == 0)) {
        lhWndTop = lhWnd;
      }
    }
  }
  
  cX = GetSystemMetrics(SM_CXICON);
  cY = GetSystemMetrics(SM_CYICON);
  hIconLarge = LoadImageAsString(App.hInstance, sIconResName, IMAGE_ICON, cX, cY, LR_SHARED);
    if(((bSetAsAppIcon))) {
    SendMessageLong(lhWndTop, WM_SETICON, ICON_BIG, hIconLarge);
  }
  SendMessageLong(hwnd, WM_SETICON, ICON_BIG, hIconLarge);
  
  cX = GetSystemMetrics(SM_CXSMICON);
  cY = GetSystemMetrics(SM_CYSMICON);
  hIconSmall = LoadImageAsString(App.hInstance, sIconResName, IMAGE_ICON, cX, cY, LR_SHARED);
    if(((bSetAsAppIcon))) {
    SendMessageLong(lhWndTop, WM_SETICON, ICON_SMALL, hIconSmall);
  }
  SendMessageLong(hwnd, WM_SETICON, ICON_SMALL, hIconSmall);
}

 // -------------------------------
 // InitCommonControlsVB
 // --------------------

 // Allows the use of XP styles without using any Common Controls.

 // Found on vbAccellerator.

  public static bool InitCommonControlsVB() {
bool _InitCommonControlsVB = false;
  // TODO: (NOT SUPPORTED): On Error Resume Next
  tagInitCommonControlsEx iccex = null;
  // TODO: (NOT SUPPORTED): With iccex
  .lngSize = LenB(iccex);
  .lngICC = ICC_USEREX_CLASSES;
  // TODO: (NOT SUPPORTED): End With
  InitCommonControlsEx(iccex);
  _InitCommonControlsVB = (Err().Number == 0);
  // TODO: (NOT SUPPORTED): On Error GoTo 0
return _InitCommonControlsVB;
}

 // -------------------------------
 // FixHex
 // ------

 // Given a value and the padding length, returns a padded Hex value.

  public static void FixHex(ref dynamic s, ref int i) {
 _FixHex = null;
  string Bleh = "";
  Bleh = Replace(s, "0x", "&H");
    if(left(Bleh, 2) != "&H") {
    Bleh = "&H" + Hex(Val(s));
  }
  _FixHex = Right("00000000" + Bleh, i);
  _FixHex = Replace(_FixHex, "&", "");
  _FixHex = Replace(_FixHex, "H", "");
return _FixHex;
}

 // -------------------------------
 // SetCaptions
 // -----------

 // Loads resource strings for each suitable control on a given form and sets its font
 // to whatever your locale needs.

  public static void SetCaptions(ref Form target) {
  int i = 0;
  dynamic ctl = null;
  // TODO: (NOT SUPPORTED): On Error Resume Next
    foreach (var iterctl in target.Controls) {
ctl = iterctl;
      if(ctl.Tag != "[NoLocal]") {
        if(left(ctl.Caption, 1) == "[") {
        i = Val(Mid(ctl.Caption, 2, 4));
        ctl.Caption = LoadResString(i);
      }
      SetProperFont(ref ctl.Font);
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

  public static void SetProperFont(ref dynamic obj) {
  // TODO: (NOT SUPPORTED): On Error GoTo ErrorSetProperFont
   // If GetUserDefaultLCID = &H411 Then
    if((LoadResString(10000)) == "<JAPPLZ>") {
    obj.Charset = 128;
    obj.name = "MS Gothic"; // ChrW(&HFF2D) + ChrW(&HFF33) + ChrW(&H20) + ChrW(&HFF30) + ChrW(&H30B4) + ChrW(&H30B7) + ChrW(&H30C3) + ChrW(&H30AF)
    obj.Size = 9;
    } else { // If (LoadResString(10000)) = __S1 Then
    obj.name = ProperFont; // __S1 '__S2
    obj.Size = ProperFontS;
  }
  return;
  ErrorSetProperFont:;
  Err.Number = Err;
}

 // -------------------------------
 // ClickSound
 // ----------

 // Just a macro...

  public static void ClickSound() {
  IncessantNoises(ref "ButtonClick");
}

 // -------------------------------
 // IncessantNoises
 // ---------------

 // Plays the given sound, as specified in the Sounds CPL.

  public static void IncessantNoises(ref string n) {
  cRegistry myReg = new cRegistry(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  
  if(GetSettingI(ref "Incessant Sound Override"))return;
  
  // TODO: (NOT SUPPORTED): With myReg
  .ClassKey = HKEY_CURRENT_USER;
  .SectionKey = "AppEvents\\Schemes\\Apps\\Sappy2k5\\Sappy2k5-" + n + "\\.current";
  .ValueKey = "";
  .ValueType = REG_SZ;
  sndPlaySound(Value, SND_ASYNC);
  // TODO: (NOT SUPPORTED): End With
   // Dim s As String
   // s = GetSetting(__S1)
   // If s = __S1 Then Exit Sub
  
   // s = GetKeyValue(HKEY_CURRENT_USER, __S1 & n & __S2, __S3)
   // If Trim(s) <> __S1 Then
   // sndPlaySound s, SND_ASYNC
   // End If
}

 // -------------------------------
 // TellMSN and ShutMSN
 // -------------------

 // Wrappers around SetMusicInfo to better manage Now Playing information in MSN Messenger.

  public static void TellMSN(ref string sTitle, ref string sArtist, ref string sAlbum) {
  MSNPlaying = true;
  SetMusicInfo(ref sArtist, ref sAlbum, ref sTitle, ref "", ref "{0} - {2} - {1}", ref true);
}
  public static void ShutMSN() {
    if(MSNPlaying == false) {
    Trace("Spurious ShutMSN.");
    return;
  }
  SetMusicInfo(ref "", ref "", ref "", ref "", ref "", ref false);
  MSNPlaying = false;
}

  public static void SetMusicInfo(ref string r_sArtist, ref string r_sAlbum, ref string r_sTitle, ref string r_sWMContentID = vbNullString, ref string r_sFormat = "{0} - {1}", ref bool r_bShow = true) {
  COPYDATASTRUCT udtData = null;
  string sBuffer = "";
  int hMSGRUI = 0;
  
  sBuffer = "\\0Music\\0" + Abs(r_bShow) + "\\0" + r_sFormat + "\\0" + r_sArtist + "\\0" + r_sTitle + "\\0" + r_sAlbum + "\\0" + r_sWMContentID + "\\0" + vbNullChar;
  Trace("Sending to Messenger: \"" + sBuffer + "\"");
  
  udtData.dwData = 0x547;
  udtData.lpData = StrPtr(sBuffer);
  udtData.cbData = LenB(sBuffer);
  
    do {
    hMSGRUI = FindWindowEx(0, hMSGRUI, "MsnMsgrUIManager", vbNullString);
    if((hMSGRUI > 0))Call(SendMessage(hMSGRUI, WM_COPYDATA, 0, VarPtr(udtData)));
  } while(!((hMSGRUI == 0)));
}

  public static void DrawSkin(ref dynamic Victim) {
  // TODO: (NOT SUPPORTED): With Victim
  .ScaleMode = 3;
  BitBlt(hdc, 0, 0, 2, 2, frmSappy.instance.picSkin.hdc, 6, 0, vbSrcCopy);
  StretchBlt(hdc, 2, 0, ScaleWidth - 4, 2, frmSappy.instance.picSkin.hdc, 6, 2, 2, 2, vbSrcCopy);
  BitBlt(hdc, ScaleWidth - 2, 0, 2, 2, frmSappy.instance.picSkin.hdc, 6, 4, vbSrcCopy);
  StretchBlt(hdc, 0, 2, 2, ScaleHeight - 4, frmSappy.instance.picSkin.hdc, 6, 6, 2, 2, vbSrcCopy);
  StretchBlt(hdc, 2, 2, ScaleWidth - 4, ScaleHeight - 4, frmSappy.instance.picSkin.hdc, 0, 0, 6, 62, vbSrcCopy);
  StretchBlt(hdc, ScaleWidth - 2, 2, 2, Height - 4, frmSappy.instance.picSkin.hdc, 6, 8, 2, 2, vbSrcCopy);
  BitBlt(hdc, 0, ScaleHeight - 2, 2, 2, frmSappy.instance.picSkin.hdc, 6, 10, vbSrcCopy);
  StretchBlt(hdc, 2, ScaleHeight - 2, ScaleWidth - 4, 2, frmSappy.instance.picSkin.hdc, 6, 12, 2, 2, vbSrcCopy);
  BitBlt(hdc, ScaleWidth - 2, ScaleHeight - 2, 2, 2, frmSappy.instance.picSkin.hdc, 6, 14, vbSrcCopy);
  // TODO: (NOT SUPPORTED): End With
}

  public static void SetAllSkinButtons(ref Form Victim) {
   // Dim ct As Control
   // For Each ct In Victim.Controls
   // If TypeName(ct) = __S1 Then ct.SkinDC = frmSappy.picSkin.hdc
   // Next
  Trace("Stop calling SetAllSkinButtons! You don't have to do that anymore! --> Victim: " + Victim.name);
}

  public static string InputBox(ref string Prompt, ref string Title, ref string Default) {
string _InputBox = "";
  if(Title == "")Title = App.Title;
  frmInputBox.Label1.Content = Prompt;
  frmInputBox.Caption = Title;
  frmInputBox.Text1.Text = Default;
  frmInputBox.Text1.SelectionStart = 0;
  frmInputBox.Text1.SelectionLength = Len(frmInputBox.instance.Text1);
  frmInputBox.instance.ShowDialog();
  _InputBox = frmInputBox.instance.Text1;
return _InputBox;
}

  public static string GetSetting(ref string name) {
string _GetSetting = "";
  cRegistry myReg = new cRegistry(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  // TODO: (NOT SUPPORTED): With myReg
  .ClassKey = HKEY_CURRENT_USER;
  .SectionKey = "Software\\Helmeted Rodent\\Sappy 2006";
  .ValueKey = name;
  .ValueType = REG_SZ;
  _GetSetting = Value;
  // TODO: (NOT SUPPORTED): End With
return _GetSetting;
}
  public static int GetSettingI(ref string name) {
int _GetSettingI = 0;
  cRegistry myReg = new cRegistry(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  // TODO: (NOT SUPPORTED): With myReg
  .ClassKey = HKEY_CURRENT_USER;
  .SectionKey = "Software\\Helmeted Rodent\\Sappy 2006";
  .ValueKey = name;
  .ValueType = REG_DWORD;
  _GetSettingI = Value;
  // TODO: (NOT SUPPORTED): End With
return _GetSettingI;
}
  public static void WriteSetting(ref string name, ref var Value) {
  cRegistry myReg = new cRegistry(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  // TODO: (NOT SUPPORTED): With myReg
  .ClassKey = HKEY_CURRENT_USER;
  .SectionKey = "Software\\Helmeted Rodent\\Sappy 2006";
  .ValueKey = name;
  .ValueType = REG_SZ;
  .Value = Value;
  // TODO: (NOT SUPPORTED): End With
}
  public static void WriteSettingI(ref string name, ref var Value) {
  cRegistry myReg = new cRegistry(); // TODO: (NOT SUPPORTED) Dimmable 'New' not supported on variable declaration.  Instantiated only on declaration.  Please ensure usages
  // TODO: (NOT SUPPORTED): With myReg
  .ClassKey = HKEY_CURRENT_USER;
  .SectionKey = "Software\\Helmeted Rodent\\Sappy 2006";
  .ValueKey = name;
  .ValueType = REG_DWORD;
  .Value = Int(Value);
  // TODO: (NOT SUPPORTED): End With
}



}