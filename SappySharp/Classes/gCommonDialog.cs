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



public class gCommonDialog {



  public enum EErrorCommonDialog{ 
  eeBaseCommonDialog = 13450 // CommonDialog
}

[DllImport("kernel32", EntryPoint="lstrlenA")]
private static extern int lstrlen(string lpString);
[DllImport("kernel32")]
private static extern int GlobalAlloc(int wFlags, int dwBytes);
[DllImport("kernel32")]
private static extern int GlobalCompact(int dwMinFree);
[DllImport("kernel32")]
private static extern int GlobalFree(int hMem);
[DllImport("kernel32")]
private static extern int GlobalLock(int hMem);
[DllImport("kernel32")]
private static extern int GlobalReAlloc(int hMem, int dwBytes, int wFlags);
[DllImport("kernel32")]
private static extern int GlobalSize(int hMem);
[DllImport("kernel32")]
private static extern int GlobalUnlock(int hMem);
[DllImport("kernel32", EntryPoint="RtlMoveMemory")]
private static extern void CopyMemory(ref dynamic lpvDest, ref dynamic lpvSource, int cbCopy);
[DllImport("kernel32", EntryPoint="RtlMoveMemory")]
private static extern void CopyMemoryStr(ref dynamic lpvDest, string lpvSource, int cbCopy);

int MAX_PATH = 260;
int MAX_FILE = 260;

  class OPENFILENAME{ 
  public int lStructSize; // Filled with UDT size
  public int hWndOwner; // Tied to Owner
  public int hInstance; // Ignored (used only by templates)
  public string lpstrFilter; // Tied to Filter
  public string lpstrCustomFilter; // Ignored (exercise for reader)
  public int nMaxCustFilter; // Ignored (exercise for reader)
  public int nFilterIndex; // Tied to FilterIndex
  public string lpstrFile; // Tied to FileName
  public int nMaxFile; // Handled internally
  public string lpstrFileTitle; // Tied to FileTitle
  public int nMaxFileTitle; // Handled internally
  public string lpstrInitialDir; // Tied to InitDir
  public string lpstrTitle; // Tied to DlgTitle
  public int flags; // Tied to Flags
  public int nFileOffset; // Ignored (exercise for reader)
  public int nFileExtension; // Ignored (exercise for reader)
  public string lpstrDefExt; // Tied to DefaultExt
  public int lCustData; // Ignored (needed for hooks)
  public int lpfnHook; // Ignored (good luck with hooks)
  public int lpTemplateName; // Ignored (good luck with templates)
}

[DllImport("COMDLG32", EntryPoint="GetOpenFileNameA")]
private static extern int GetOpenFileName(ref OPENFILENAME file);
[DllImport("COMDLG32", EntryPoint="GetSaveFileNameA")]
private static extern int GetSaveFileName(ref OPENFILENAME file);
[DllImport("COMDLG32", EntryPoint="GetFileTitleA")]
private static extern int GetFileTitle(string szFile, string szTitle, int cbBuf);

  public enum EOpenFile{ 
  OFN_READONLY = 0x1
  , OFN_OVERWRITEPROMPT = 0x2
  , OFN_HIDEREADONLY = 0x4
  , OFN_NOCHANGEDIR = 0x8
  , OFN_SHOWHELP = 0x10
  , OFN_ENABLEHOOK = 0x20
  , OFN_ENABLETEMPLATE = 0x40
  , OFN_ENABLETEMPLATEHANDLE = 0x80
  , OFN_NOVALIDATE = 0x100
  , OFN_ALLOWMULTISELECT = 0x200
  , OFN_EXTENSIONDIFFERENT = 0x400
  , OFN_PATHMUSTEXIST = 0x800
  , OFN_FILEMUSTEXIST = 0x1000
  , OFN_CREATEPROMPT = 0x2000
  , OFN_SHAREAWARE = 0x4000
  , OFN_NOREADONLYRETURN = 0x8000
  , OFN_NOTESTFILECREATE = 0x10000
  , OFN_NONETWORKBUTTON = 0x20000
  , OFN_NOLONGNAMES = 0x40000
  , OFN_EXPLORER = 0x80000
  , OFN_NODEREFERENCELINKS = 0x100000
  , OFN_LONGNAMES = 0x200000
}

  class TCHOOSECOLOR{ 
  public int lStructSize;
  public int hWndOwner;
  public int hInstance;
  public int rgbResult;
  public int lpCustColors;
  public int flags;
  public int lCustData;
  public int lpfnHook;
  public int lpTemplateName;
}

[DllImport("COMDLG32.DLL", EntryPoint="ChooseColorA")]
private static extern int ChooseColor(ref TCHOOSECOLOR Color);

  public enum EChooseColor{ 
  CC_RGBInit = 0x1
  , CC_FullOpen = 0x2
  , CC_PreventFullOpen = 0x4
  , CC_ColorShowHelp = 0x8
  ,  // Win95 only
  , CC_SolidColor = 0x80
  , CC_AnyColor = 0x100
  ,  // End Win95 only
  , CC_ENABLEHOOK = 0x10
  , CC_ENABLETEMPLATE = 0x20
  , CC_EnableTemplateHandle = 0x40
}
[DllImport("USER32")]
private static extern int GetSysColor(int nIndex);

  class TCHOOSEFONT{ 
  public int lStructSize; // Filled with UDT size
  public int hWndOwner; // Caller's window handle
  public int hdc; // Printer DC/IC or NULL
  public int lpLogFont; // Pointer to LOGFONT
  public int iPointSize; // 10 * size in points of font
  public int flags; // Type flags
  public int rgbColors; // Returned text color
  public int lCustData; // Data passed to hook function
  public int lpfnHook; // Pointer to hook function
  public int lpTemplateName; // Custom template name
  public int hInstance; // Instance handle for template
  public string lpszStyle; // Return style field
  public int nFontType; // Font type bits
  public int iAlign; // Filler
  public int nSizeMin; // Minimum point size allowed
  public int nSizeMax; // Maximum point size allowed
}
[DllImport("COMDLG32", EntryPoint="ChooseFontA")]
private static extern int ChooseFont(ref TCHOOSEFONT chfont);

int LF_FACESIZE = 32;
  class LOGFONT{ 
  public int lfHeight;
  public int lfWidth;
  public int lfEscapement;
  public int lfOrientation;
  public int lfWeight;
  public Byte lfItalic;
  public Byte lfUnderline;
  public Byte lfStrikeOut;
  public Byte lfCharSet;
  public Byte lfOutPrecision;
  public Byte lfClipPrecision;
  public Byte lfQuality;
  public Byte lfPitchAndFamily;
  public Byte lfFaceName(LF_FACESIZE);
}

  public enum EChooseFont{ 
  CF_ScreenFonts = 0x1
  , CF_PrinterFonts = 0x2
  , CF_BOTH = 0x3
  , CF_FontShowHelp = 0x4
  , CF_UseStyle = 0x80
  , CF_EFFECTS = 0x100
  , CF_AnsiOnly = 0x400
  , CF_NoVectorFonts = 0x800
  , CF_NoOemFonts = CF_NoVectorFonts
  , CF_NoSimulations = 0x1000
  , CF_LimitSize = 0x2000
  , CF_FixedPitchOnly = 0x4000
  , CF_WYSIWYG = 0x8000 // Must also have ScreenFonts And PrinterFonts
  , CF_ForceFontExist = 0x10000
  , CF_ScalableOnly = 0x20000
  , CF_TTOnly = 0x40000
  , CF_NoFaceSel = 0x80000
  , CF_NoStyleSel = 0x100000
  , CF_NoSizeSel = 0x200000
  ,  // Win95 only
  , CF_SelectScript = 0x400000
  , CF_NoScriptSel = 0x800000
  , CF_NoVertFonts = 0x1000000
  , 
  , CF_InitToLogFontStruct = 0x40
  , CF_Apply = 0x200
  , CF_EnableHook = 0x8
  , CF_EnableTemplate = 0x10
  , CF_EnableTemplateHandle = 0x20
  , CF_FontNotSupported = 0x238
}

 // These are extra nFontType bits that are added to what is returned to the
 // EnumFonts callback routine

  public enum EFontType{ 
  Simulated_FontType = 0x8000
  , Printer_FontType = 0x4000
  , Screen_FontType = 0x2000
  , Bold_FontType = 0x100
  , Italic_FontType = 0x200
  , Regular_FontType = 0x400
}

  class TPRINTDLG{ 
  public int lStructSize;
  public int hWndOwner;
  public int hDevMode;
  public int hDevNames;
  public int hdc;
  public int flags;
  public int nFromPage;
  public int nToPage;
  public int nMinPage;
  public int nMaxPage;
  public int nCopies;
  public int hInstance;
  public int lCustData;
  public int lpfnPrintHook;
  public int lpfnSetupHook;
  public int lpPrintTemplateName;
  public int lpSetupTemplateName;
  public int hPrintTemplate;
  public int hSetupTemplate;
}

 // DEVMODE collation selections
int DMCOLLATE_FALSE = 0;
int DMCOLLATE_TRUE = 1;

[DllImport("COMDLG32.DLL", EntryPoint="PrintDlgA")]
private static extern int PrintDlg(ref TPRINTDLG prtdlg);

  public enum EPrintDialog{ 
  PD_ALLPAGES = 0x0
  , PD_SELECTION = 0x1
  , PD_PAGENUMS = 0x2
  , PD_NOSELECTION = 0x4
  , PD_NOPAGENUMS = 0x8
  , PD_COLLATE = 0x10
  , PD_PRINTTOFILE = 0x20
  , PD_PRINTSETUP = 0x40
  , PD_NOWARNING = 0x80
  , PD_RETURNDC = 0x100
  , PD_RETURNIC = 0x200
  , PD_RETURNDEFAULT = 0x400
  , PD_SHOWHELP = 0x800
  , PD_ENABLEPRINTHOOK = 0x1000
  , PD_ENABLESETUPHOOK = 0x2000
  , PD_ENABLEPRINTTEMPLATE = 0x4000
  , PD_ENABLESETUPTEMPLATE = 0x8000
  , PD_ENABLEPRINTTEMPLATEHANDLE = 0x10000
  , PD_ENABLESETUPTEMPLATEHANDLE = 0x20000
  , PD_USEDEVMODECOPIES = 0x40000
  , PD_USEDEVMODECOPIESANDCOLLATE = 0x40000
  , PD_DISABLEPRINTTOFILE = 0x80000
  , PD_HIDEPRINTTOFILE = 0x100000
  , PD_NONETWORKBUTTON = 0x200000
}

  class DEVNAMES{ 
  public int wDriverOffset;
  public int wDeviceOffset;
  public int wOutputOffset;
  public int wDefault;
}

int CCHDEVICENAME = 32;
int CCHFORMNAME = 32;
  class DevMode{ 
  public string dmDeviceName; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (0)
  public int dmSpecVersion;
  public int dmDriverVersion;
  public int dmSize;
  public int dmDriverExtra;
  public int dmFields;
  public int dmOrientation;
  public int dmPaperSize;
  public int dmPaperLength;
  public int dmPaperWidth;
  public int dmScale;
  public int dmCopies;
  public int dmDefaultSource;
  public int dmPrintQuality;
  public int dmColor;
  public int dmDuplex;
  public int dmYResolution;
  public int dmTTOption;
  public int dmCollate;
  public string dmFormName; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (0)
  public int dmUnusedPadding;
  public int dmBitsPerPel;
  public int dmPelsWidth;
  public int dmPelsHeight;
  public int dmDisplayFlags;
  public int dmDisplayFrequency;
}

 // New Win95 Page Setup dialogs are up to you
  class POINTL{ 
  public int x;
  public int y;
}
  class RECT{ 
  public int Left;
  public int TOp;
  public int Right;
  public int Bottom;
}


  class TPAGESETUPDLG{ 
  public int lStructSize                ;
  public int hWndOwner                  ;
  public int hDevMode                   ;
  public int hDevNames                  ;
  public int flags                      ;
  public POINTL ptPaperSize                ;
  public RECT rtMinMargin                ;
  public RECT rtMargin                   ;
  public int hInstance                  ;
  public int lCustData                  ;
  public int lpfnPageSetupHook          ;
  public int lpfnPagePaintHook          ;
  public int lpPageSetupTemplateName    ;
  public int hPageSetupTemplate         ;
}

 // EPaperSize constants same as vbPRPS constants
  public enum EPaperSize{ 
  epsLetter = 1 // Letter, 8 1/2 x 11 in.
  , epsLetterSmall // Letter Small, 8 1/2 x 11 in.
  , epsTabloid // Tabloid, 11 x 17 in.
  , epsLedger // Ledger, 17 x 11 in.
  , epsLegal // Legal, 8 1/2 x 14 in.
  , epsStatement // Statement, 5 1/2 x 8 1/2 in.
  , epsExecutive // Executive, 7 1/2 x 10 1/2 in.
  , epsA3 // A3, 297 x 420 mm
  , epsA4 // A4, 210 x 297 mm
  , epsA4Small // A4 Small, 210 x 297 mm
  , epsA5 // A5, 148 x 210 mm
  , epsB4 // B4, 250 x 354 mm
  , epsB5 // B5, 182 x 257 mm
  , epsFolio // Folio, 8 1/2 x 13 in.
  , epsQuarto // Quarto, 215 x 275 mm
  , eps10x14 // 10 x 14 in.
  , eps11x17 // 11 x 17 in.
  , epsNote // Note, 8 1/2 x 11 in.
  , epsEnv9 // Envelope #9, 3 7/8 x 8 7/8 in.
  , epsEnv10 // Envelope #10, 4 1/8 x 9 1/2 in.
  , epsEnv11 // Envelope #11, 4 1/2 x 10 3/8 in.
  , epsEnv12 // Envelope #12, 4 1/2 x 11 in.
  , epsEnv14 // Envelope #14, 5 x 11 1/2 in.
  , epsCSheet // C size sheet
  , epsDSheet // D size sheet
  , epsESheet // E size sheet
  , epsEnvDL // Envelope DL, 110 x 220 mm
  , epsEnvC3 // Envelope C3, 324 x 458 mm
  , epsEnvC4 // Envelope C4, 229 x 324 mm
  , epsEnvC5 // Envelope C5, 162 x 229 mm
  , epsEnvC6 // Envelope C6, 114 x 162 mm
  , epsEnvC65 // Envelope C65, 114 x 229 mm
  , epsEnvB4 // Envelope B4, 250 x 353 mm
  , epsEnvB5 // Envelope B5, 176 x 250 mm
  , epsEnvB6 // Envelope B6, 176 x 125 mm
  , epsEnvItaly // Envelope, 110 x 230 mm
  , epsenvmonarch // Envelope Monarch, 3 7/8 x 7 1/2 in.
  , epsEnvPersonal // Envelope, 3 5/8 x 6 1/2 in.
  , epsFanfoldUS // U.S. Standard Fanfold, 14 7/8 x 11 in.
  , epsFanfoldStdGerman // German Standard Fanfold, 8 1/2 x 12 in.
  , epsFanfoldLglGerman // German Legal Fanfold, 8 1/2 x 13 in.
  , epsUser = 256 // User-defined
}

 // EPrintQuality constants same as vbPRPQ constants
  public enum EPrintQuality{ 
  epqDraft = -1
  , epqLow = -2
  , epqMedium = -3
  , epqHigh = -4
}

  public enum EOrientation{ 
  eoPortrait = 1
  , eoLandscape
}

[DllImport("COMDLG32", EntryPoint="PageSetupDlgA")]
private static extern bool PageSetupDlg(ref TPAGESETUPDLG lppage);

  public enum EPageSetup{ 
  PSD_Defaultminmargins = 0x0 // Default (printer's)
  , PSD_InWinIniIntlMeasure = 0x0
  , PSD_MINMARGINS = 0x1
  , PSD_MARGINS = 0x2
  , PSD_INTHOUSANDTHSOFINCHES = 0x4
  , PSD_INHUNDREDTHSOFMILLIMETERS = 0x8
  , PSD_DISABLEMARGINS = 0x10
  , PSD_DISABLEPRINTER = 0x20
  , PSD_NoWarning = 0x80
  , PSD_DISABLEORIENTATION = 0x100
  , PSD_ReturnDefault = 0x400
  , PSD_DISABLEPAPER = 0x200
  , PSD_ShowHelp = 0x800
  , PSD_EnablePageSetupHook = 0x2000
  , PSD_EnablePageSetupTemplate = 0x8000
  , PSD_EnablePageSetupTemplateHandle = 0x20000
  , PSD_EnablePagePaintHook = 0x40000
  , PSD_DisablePagePainting = 0x80000
}

  public enum EPageSetupUnits{ 
  epsuInches
  , epsuMillimeters
}

 // Common dialog errors

[DllImport("COMDLG32")]
private static extern int CommDlgExtendedError();

  public enum EDialogError{ 
  CDERR_DIALOGFAILURE = 0xFFFF
  , 
  , CDERR_GENERALCODES = 0x0
  , CDERR_STRUCTSIZE = 0x1
  , CDERR_INITIALIZATION = 0x2
  , CDERR_NOTEMPLATE = 0x3
  , CDERR_NOHINSTANCE = 0x4
  , CDERR_LOADSTRFAILURE = 0x5
  , CDERR_FINDRESFAILURE = 0x6
  , CDERR_LOADRESFAILURE = 0x7
  , CDERR_LOCKRESFAILURE = 0x8
  , CDERR_MEMALLOCFAILURE = 0x9
  , CDERR_MEMLOCKFAILURE = 0xA
  , CDERR_NOHOOK = 0xB
  , CDERR_REGISTERMSGFAIL = 0xC
  , 
  , PDERR_PRINTERCODES = 0x1000
  , PDERR_SETUPFAILURE = 0x1001
  , PDERR_PARSEFAILURE = 0x1002
  , PDERR_RETDEFFAILURE = 0x1003
  , PDERR_LOADDRVFAILURE = 0x1004
  , PDERR_GETDEVMODEFAIL = 0x1005
  , PDERR_INITFAILURE = 0x1006
  , PDERR_NODEVICES = 0x1007
  , PDERR_NODEFAULTPRN = 0x1008
  , PDERR_DNDMMISMATCH = 0x1009
  , PDERR_CREATEICFAILURE = 0x100A
  , PDERR_PRINTERNOTFOUND = 0x100B
  , PDERR_DEFAULTDIFFERENT = 0x100C
  , 
  , CFERR_CHOOSEFONTCODES = 0x2000
  , CFERR_NOFONTS = 0x2001
  , CFERR_MAXLESSTHANMIN = 0x2002
  , 
  , FNERR_FILENAMECODES = 0x3000
  , FNERR_SUBCLASSFAILURE = 0x3001
  , FNERR_INVALIDFILENAME = 0x3002
  , FNERR_BUFFERTOOSMALL = 0x3003
  , 
  , CCERR_CHOOSECOLORCODES = 0x5000
}

 // Array of custom colors lasts for life of app
List<int> alCustom = new List<int>();
bool fNotFirst = false; //  TODO: (NOT SUPPORTED) Array ranges not supported: alCustom(0 To 15)

  public enum EPrintRange{ 
  eprAll
  , eprPageNumbers
  , eprSelection
}
int m_lApiReturn = 0;
int m_lExtendedError = 0;
DevMode m_dvmode = null;

  public int APIReturn{ 
get {
int _APIReturn = default(int);
 // return object's APIReturn property
_APIReturn = m_lApiReturn;
return _APIReturn;
}
}

  public int ExtendedError{ 
get {
int _ExtendedError = default(int);
 // return object's ExtendedError property
_ExtendedError = m_lExtendedError;
return _ExtendedError;
}
}


#If fComponent Then;
  private void Class_Initialize() {
  InitColors();
}
#End If;

  bool VBGetOpenFileName(ref string Filename, ref string FileTitle, ref bool FileMustExist = true, ref bool MultiSelect = false, ref bool ReadOnly = false, ref bool HideReadOnly = false, ref string Filter = "All (*.*)| *.*", ref int FilterIndex = 1, ref string InitDir, ref string DlgTitle, ref string DefaultExt, ref int Owner = -1, ref int flags = 0) {
bool _VBGetOpenFileName = false;
  
  OPENFILENAME opfile = null;
string s = "";
int afFlags = 0;
  
  m_lApiReturn = 0;
  m_lExtendedError = 0;
  
  // TODO: (NOT SUPPORTED): With opfile
  .lStructSize = Len(opfile);
  
   // Add in specific flags and strip out non-VB flags
  
  .flags = (-FileMustExist * OFN_FILEMUSTEXIST) || (-MultiSelect * OFN_ALLOWMULTISELECT) || (-ReadOnly * OFN_READONLY) || (-HideReadOnly * OFN_HIDEREADONLY) || (flags && CLng(! (OFN_ENABLEHOOK || OFN_ENABLETEMPLATE)));
   // Owner can take handle of owning window
  if(Owner != -1).hWndOwner = Owner;
   // InitDir can take initial directory string
  .lpstrInitialDir = InitDir;
   // DefaultExt can take default extension
  .lpstrDefExt = DefaultExt;
   // DlgTitle can take dialog box title
  .lpstrTitle = DlgTitle;
  
   // To make Windows-style filter, replace | and : with nulls
  string ch = "";
int i = 0;
    for (i = 1; i <= Len(Filter); i += 1) {
    ch = Mid$(Filter, i, 1);
      if(ch == "|" || ch == ":") {
      s = s + vbNullChar;
      } else {
      s = s + ch;
    }
  }
   // Put double null at end
  s = s + vbNullChar + vbNullChar;
  .lpstrFilter = s;
  .nFilterIndex = FilterIndex;
  
   // Pad file and file title buffers to maximum path
  s = Filename + String$(MAX_PATH - Len(Filename), 0);
  .lpstrFile = s;
  .nMaxFile = MAX_PATH;
  s = FileTitle + String$(MAX_FILE - Len(FileTitle), 0);
  .lpstrFileTitle = s;
  .nMaxFileTitle = MAX_FILE;
   // All other fields set to zero
  
  m_lApiReturn = GetOpenFileName(opfile);
      switch(m_lApiReturn) {
      case 1: 
       // Success
      _VBGetOpenFileName = true;
      Filename = StrZToStr(lpstrFile);
      FileTitle = StrZToStr(lpstrFileTitle);
      flags = flags;
       // Return the filter index
      FilterIndex = nFilterIndex;
       // Look up the filter the user selected and return that
      Filter = FilterLookup(lpstrFilter, FilterIndex);
      if((flags && OFN_READONLY))ReadOnly = true;
      break;
case 0: 
       // Cancelled
      _VBGetOpenFileName = false;
      Filename = "";
      FileTitle = "";
      flags = 0;
      FilterIndex = -1;
      Filter = "";
      break;
default: 
       // Extended error
      m_lExtendedError = CommDlgExtendedError();
      _VBGetOpenFileName = false;
      Filename = "";
      FileTitle = "";
      flags = 0;
      FilterIndex = -1;
      Filter = "";
break;
}
// TODO: (NOT SUPPORTED): End With
return _VBGetOpenFileName;
}
private string StrZToStr(ref string s) {
string _StrZToStr = "";
_StrZToStr = Left$(s, lstrlen(s));
return _StrZToStr;
}

bool VBGetSaveFileName(ref string Filename, ref string FileTitle, ref bool OverWritePrompt = true, ref string Filter = "All (*.*)| *.*", ref int FilterIndex = 1, ref string InitDir, ref string DlgTitle, ref string DefaultExt, ref int Owner = -1, ref int flags) {
bool _VBGetSaveFileName = false;

OPENFILENAME opfile = null;
string s = "";

m_lApiReturn = 0;
m_lExtendedError = 0;

// TODO: (NOT SUPPORTED): With opfile
.lStructSize = Len(opfile);

 // Add in specific flags and strip out non-VB flags
.flags = (-OverWritePrompt * OFN_OVERWRITEPROMPT) || OFN_HIDEREADONLY || (flags && CLng(! (OFN_ENABLEHOOK || OFN_ENABLETEMPLATE)));
 // Owner can take handle of owning window
if(Owner != -1).hWndOwner = Owner;
 // InitDir can take initial directory string
.lpstrInitialDir = InitDir;
 // DefaultExt can take default extension
.lpstrDefExt = DefaultExt;
 // DlgTitle can take dialog box title
.lpstrTitle = DlgTitle;

 // Make new filter with bars (|) replacing nulls and double null at end
string ch = "";
int i = 0;
  for (i = 1; i <= Len(Filter); i += 1) {
  ch = Mid$(Filter, i, 1);
    if(ch == "|" || ch == ":") {
    s = s + vbNullChar;
    } else {
    s = s + ch;
  }
}
 // Put double null at end
s = s + vbNullChar + vbNullChar;
.lpstrFilter = s;
.nFilterIndex = FilterIndex;

 // Pad file and file title buffers to maximum path
s = Filename + String$(MAX_PATH - Len(Filename), 0);
.lpstrFile = s;
.nMaxFile = MAX_PATH;
s = FileTitle + String$(MAX_FILE - Len(FileTitle), 0);
.lpstrFileTitle = s;
.nMaxFileTitle = MAX_FILE;
 // All other fields zero

m_lApiReturn = GetSaveFileName(opfile);
    switch(m_lApiReturn) {
    case 1: 
    _VBGetSaveFileName = true;
    Filename = StrZToStr(lpstrFile);
    FileTitle = StrZToStr(lpstrFileTitle);
    flags = flags;
     // Return the filter index
    FilterIndex = nFilterIndex;
     // Look up the filter the user selected and return that
    Filter = FilterLookup(lpstrFilter, FilterIndex);
    break;
case 0: 
     // Cancelled:
    _VBGetSaveFileName = false;
    Filename = "";
    FileTitle = "";
    flags = 0;
    FilterIndex = 0;
    Filter = "";
    break;
default: 
     // Extended error:
    _VBGetSaveFileName = false;
    m_lExtendedError = CommDlgExtendedError();
    Filename = "";
    FileTitle = "";
    flags = 0;
    FilterIndex = 0;
    Filter = "";
break;
}
// TODO: (NOT SUPPORTED): End With
return _VBGetSaveFileName;
}

private string FilterLookup(string sFilters, int iCur) {
string _FilterLookup = "";
int iStart = 0;
int iEnd = 0;
string s = "";
iStart = 1;
if(sFilters == "")return _FilterLookup;
do {
 // Cut out both parts marked by null character
iEnd = InStr(iStart, sFilters, vbNullChar);
if(iEnd == 0)return _FilterLookup;
iEnd = InStr(iEnd + 1, sFilters, vbNullChar);
  if(iEnd) {
  s = Mid$(sFilters, iStart, iEnd - iStart);
  } else {
  s = Mid$(sFilters, iStart);
}
iStart = iEnd + 1;
  if(iCur == 1) {
  _FilterLookup = s;
  return _FilterLookup;
}
iCur = iCur - 1;
} while(iCur);
return _FilterLookup;
}

string VBGetFileTitle(ref string sFile) {
string _VBGetFileTitle = "";
string sFileTitle = "";
int cFileTitle = 0;

cFileTitle = MAX_PATH;
sFileTitle = String$(MAX_PATH, 0);
cFileTitle = GetFileTitle(sFile, sFileTitle, MAX_PATH);
if(cFileTitle) {
_VBGetFileTitle = "";
} else {
_VBGetFileTitle = Left$(sFileTitle, InStr(sFileTitle, vbNullChar) - 1);
}

return _VBGetFileTitle;
}

 // ChooseColor wrapper
bool VBChooseColor(ref int Color, ref bool AnyColor = true, ref bool FullOpen = false, ref bool DisableFullOpen = false, ref int Owner = -1, ref int flags) {
bool _VBChooseColor = false;

TCHOOSECOLOR chclr = null;
chclr.lStructSize = Len(chclr);

 // Color must get reference variable to receive result
 // Flags can get reference variable or constant with bit flags
 // Owner can take handle of owning window
if(Owner != -1)chclr.hWndOwner = Owner;

 // Assign color (default uninitialized value of zero is good default)
chclr.rgbResult = Color;

 // Mask out unwanted bits
int afMask = 0;
afMask = CLng(! (CC_ENABLEHOOK || CC_ENABLETEMPLATE));
 // Pass in flags
chclr.flags = afMask && (CC_RGBInit || IIf(AnyColor, CC_AnyColor, CC_SolidColor) || (-FullOpen * CC_FullOpen) || (-DisableFullOpen * CC_PreventFullOpen));

 // If first time, initialize to white
if(fNotFirst == false)InitColors();

chclr.lpCustColors = VarPtr(alCustom(0));
 // All other fields zero

m_lApiReturn = ChooseColor(chclr);
  switch(m_lApiReturn) {
  case 1: 
   // Success
  _VBChooseColor = true;
  Color = chclr.rgbResult;
  break;
case 0: 
   // Cancelled
  _VBChooseColor = false;
  Color = -1;
  break;
default: 
   // Extended error
  m_lExtendedError = CommDlgExtendedError();
  _VBChooseColor = false;
  Color = -1;
break;
}

return _VBChooseColor;
}

private void InitColors() {
int i = 0;
 // Initialize with first 16 system interface colors
for (i = 0; i <= 15; i += 1) {
alCustom(i) = GetSysColor(i);
}
fNotFirst = true;
}

 // Property to read or modify custom colors (use to save colors in registry)
public int CustomColor{ 
get {
int _CustomColor = default(int);
 // If first time, initialize to white
if(fNotFirst == false)InitColors();
  if(i >= 0 && i <= 15) {
  _CustomColor = alCustom(i);
  } else {
  _CustomColor = -1;
}
return _CustomColor;
}
set {
 // If first time, initialize to system colors
if(fNotFirst == false)InitColors();
  if(i >= 0 && i <= 15) {
  alCustom(i) = iValue;
}
}
}




 // ChooseFont wrapper
bool VBChooseFont(ref Font CurFont, ref int PrinterDC = -1, ref int Owner = -1, ref int Color = vbBlack, ref int MinSize = 0, ref int MaxSize = 0, ref int flags = 0) {
bool _VBChooseFont = false;

m_lApiReturn = 0;
m_lExtendedError = 0;

 // Unwanted Flags bits
var CF_FontNotSupported = CF_Apply || CF_EnableHook || CF_EnableTemplate;

 // Flags can get reference variable or constant with bit flags
 // PrinterDC can take printer DC
if(PrinterDC == -1) {
PrinterDC = 0;
if(flags && CF_PrinterFonts)PrinterDC = Printer.hdc;
} else {
flags = flags || CF_PrinterFonts;
}
 // Must have some fonts
if((flags && CF_PrinterFonts) == 0)flags = flags || CF_ScreenFonts;
 // Color can take initial color, receive chosen color
if(Color != vbBlack)flags = flags || CF_EFFECTS;
 // MinSize can be minimum size accepted
if(MinSize)flags = flags || CF_LimitSize;
 // MaxSize can be maximum size accepted
if(MaxSize)flags = flags || CF_LimitSize;

 // Put in required internal flags and remove unsupported
flags = (flags || CF_InitToLogFontStruct) && ! CF_FontNotSupported;

 // Initialize LOGFONT variable
LOGFONT fnt = null;
var PointsPerTwip = 1440 / 72;
fnt.lfHeight = -(CurFont.Size * (PointsPerTwip / Screen.TwipsPerPixelY));
fnt.lfWeight = CurFont.Weight;
fnt.lfItalic = CurFont.Italic;
fnt.lfUnderline = CurFont.Underline;
fnt.lfStrikeOut = CurFont.Strikethrough;
 // Other fields zero
StrToBytes(fnt.lfFaceName, CurFont.name);

 // Initialize TCHOOSEFONT variable
TCHOOSEFONT cf = null;
cf.lStructSize = Len(cf);
if(Owner != -1)cf.hWndOwner = Owner;
cf.hdc = PrinterDC;
cf.lpLogFont = VarPtr(fnt);
cf.iPointSize = CurFont.Size * 10;
cf.flags = flags;
cf.rgbColors = Color;
cf.nSizeMin = MinSize;
cf.nSizeMax = MaxSize;

 // All other fields zero
m_lApiReturn = ChooseFont(cf);
switch(m_lApiReturn) {
case 1: 
 // Success
_VBChooseFont = true;
flags = cf.flags;
Color = cf.rgbColors;
CurFont.Bold = cf.nFontType && Bold_FontType;
 // CurFont.Italic = cf.nFontType And Italic_FontType
CurFont.Italic = fnt.lfItalic;
CurFont.Strikethrough = fnt.lfStrikeOut;
CurFont.Underline = fnt.lfUnderline;
CurFont.Weight = fnt.lfWeight;
CurFont.Size = cf.iPointSize / 10;
CurFont.name = BytesToStr(fnt.lfFaceName);
break;
case 0: 
 // Cancelled
_VBChooseFont = false;
break;
default: 
 // Extended error
m_lExtendedError = CommDlgExtendedError();
_VBChooseFont = false;
break;
}

return _VBChooseFont;
}

 // PrintDlg wrapper
bool VBPrintDlg(ref int hdc, ref EPrintRange PrintRange = eprAll, ref bool DisablePageNumbers, ref int FromPage = 1, ref int ToPage = 0xFFFF, ref bool DisableSelection, ref int Copies, ref bool ShowPrintToFile, ref bool DisablePrintToFile = true, ref bool PrintToFile, ref bool Collate, ref bool PreventWarning, ref int Owner, ref dynamic Printer, ref int flags) {
bool _VBPrintDlg = false;
int afFlags = 0;
int afMask = 0;

m_lApiReturn = 0;
m_lExtendedError = 0;

 // Set PRINTDLG flags
afFlags = (-DisablePageNumbers * PD_NOPAGENUMS) || (-DisablePrintToFile * PD_DISABLEPRINTTOFILE) || (-DisableSelection * PD_NOSELECTION) || (-PrintToFile * PD_PRINTTOFILE) || (-(! ShowPrintToFile) * PD_HIDEPRINTTOFILE) || (-PreventWarning * PD_NOWARNING) || (-Collate * PD_COLLATE) || PD_USEDEVMODECOPIESANDCOLLATE || PD_RETURNDC;
if(PrintRange == eprPageNumbers) {
afFlags = afFlags || PD_PAGENUMS;
} else if(PrintRange == eprSelection) {
afFlags = afFlags || PD_SELECTION;
}
 // Mask out unwanted bits
afMask = CLng(! (PD_ENABLEPRINTHOOK || PD_ENABLEPRINTTEMPLATE));
afMask = afMask && CLng(! (PD_ENABLESETUPHOOK || PD_ENABLESETUPTEMPLATE));

 // Fill in PRINTDLG structure
TPRINTDLG pd = null;
pd.lStructSize = Len(pd);
pd.hWndOwner = Owner;
pd.flags = afFlags && afMask;
pd.nFromPage = FromPage;
pd.nToPage = ToPage;
pd.nMinPage = 1;
pd.nMaxPage = 0xFFFF;

 // Show Print dialog
m_lApiReturn = PrintDlg(pd);
switch(m_lApiReturn) {
case 1: 
_VBPrintDlg = true;
 // Return dialog values in parameters
hdc = pd.hdc;
if((pd.flags && PD_PAGENUMS)) {
PrintRange = eprPageNumbers;
} else if((pd.flags && PD_SELECTION)) {
PrintRange = eprSelection;
} else {
PrintRange = eprAll;
}
FromPage = pd.nFromPage;
ToPage = pd.nToPage;
PrintToFile = (pd.flags && PD_PRINTTOFILE);
 // Get DEVMODE structure from PRINTDLG
int pDevMode = 0;
pDevMode = GlobalLock(pd.hDevMode);
CopyMemory(m_dvmode, ByVal pDevMode, Len(m_dvmode));
Call(GlobalUnlock(pd.hDevMode));
 // Get Copies and Collate settings from DEVMODE structure
Copies = m_dvmode.dmCopies;
Collate = (m_dvmode.dmCollate == DMCOLLATE_TRUE);

 // Set default printer properties
// TODO: (NOT SUPPORTED): On Error Resume Next
if(! (Printer == null)) {
Printer.Copies = Copies;
Printer.Orientation = m_dvmode.dmOrientation;
Printer.PaperSize = m_dvmode.dmPaperSize;
Printer.PrintQuality = m_dvmode.dmPrintQuality;
}
// TODO: (NOT SUPPORTED): On Error GoTo 0
break;
case 0: 
 // Cancelled
_VBPrintDlg = false;
break;
default: 
 // Extended error:
m_lExtendedError = CommDlgExtendedError();
_VBPrintDlg = false;
break;
}

return _VBPrintDlg;
}
private DevMode DevMode{ 
get {
DevMode _DevMode = default(DevMode);
_DevMode = m_dvmode;
return _DevMode;
}
}


 // PageSetupDlg wrapper
bool VBPageSetupDlg(ref int Owner, ref bool DisableMargins, ref bool DisableOrientation, ref bool DisablePaper, ref bool DisablePrinter, ref int LeftMargin, ref int MinLeftMargin, ref int RightMargin, ref int MinRightMargin, ref int TopMargin, ref int MinTopMargin, ref int BottomMargin, ref int MinBottomMargin, ref EPaperSize PaperSize = epsLetter, ref EOrientation Orientation = eoPortrait, ref EPrintQuality PrintQuality = epqDraft, ref EPageSetupUnits Units = epsuInches, ref dynamic Printer, ref int flags) {
bool _VBPageSetupDlg = false;
int afFlags = 0;
int afMask = 0;

m_lApiReturn = 0;
m_lExtendedError = 0;
 // Mask out unwanted bits
afMask = ! (PSD_EnablePagePaintHook || PSD_EnablePageSetupHook || PSD_EnablePageSetupTemplate);
 // Set TPAGESETUPDLG flags
afFlags = (-DisableMargins * PSD_DISABLEMARGINS) || (-DisableOrientation * PSD_DISABLEORIENTATION) || (-DisablePaper * PSD_DISABLEPAPER) || (-DisablePrinter * PSD_DISABLEPRINTER) || PSD_MARGINS || PSD_MINMARGINS && afMask;
int lUnits = 0;
if(Units == epsuInches) {
afFlags = afFlags || PSD_INTHOUSANDTHSOFINCHES;
lUnits = 1000;
} else {
afFlags = afFlags || PSD_INHUNDREDTHSOFMILLIMETERS;
lUnits = 100;
}

TPAGESETUPDLG psd = null;
 // Fill in PRINTDLG structure
psd.lStructSize = Len(psd);
psd.hWndOwner = Owner;
psd.rtMargin.TOp = TopMargin * lUnits;
psd.rtMargin.Left = LeftMargin * lUnits;
psd.rtMargin.Bottom = BottomMargin * lUnits;
psd.rtMargin.Right = RightMargin * lUnits;
psd.rtMinMargin.TOp = MinTopMargin * lUnits;
psd.rtMinMargin.Left = MinLeftMargin * lUnits;
psd.rtMinMargin.Bottom = MinBottomMargin * lUnits;
psd.rtMinMargin.Right = MinRightMargin * lUnits;
psd.flags = afFlags;

 // Show Print dialog
if(PageSetupDlg(psd)) {
_VBPageSetupDlg = true;
 // Return dialog values in parameters
TopMargin = psd.rtMargin.TOp / lUnits;
LeftMargin = psd.rtMargin.Left / lUnits;
BottomMargin = psd.rtMargin.Bottom / lUnits;
RightMargin = psd.rtMargin.Right / lUnits;
MinTopMargin = psd.rtMinMargin.TOp / lUnits;
MinLeftMargin = psd.rtMinMargin.Left / lUnits;
MinBottomMargin = psd.rtMinMargin.Bottom / lUnits;
MinRightMargin = psd.rtMinMargin.Right / lUnits;

 // Get DEVMODE structure from PRINTDLG
DevMode dvmode = null;
int pDevMode = 0;
pDevMode = GlobalLock(psd.hDevMode);
CopyMemory(dvmode, ByVal pDevMode, Len(dvmode));
Call(GlobalUnlock(psd.hDevMode));
PaperSize = dvmode.dmPaperSize;
Orientation = dvmode.dmOrientation;
PrintQuality = dvmode.dmPrintQuality;
 // Set default printer properties
// TODO: (NOT SUPPORTED): On Error Resume Next
if(! (Printer == null)) {
Printer.Copies = dvmode.dmCopies;
Printer.Orientation = dvmode.dmOrientation;
Printer.PaperSize = dvmode.dmPaperSize;
Printer.PrintQuality = dvmode.dmPrintQuality;
}
// TODO: (NOT SUPPORTED): On Error GoTo 0
}

return _VBPageSetupDlg;
}

#If fComponent = 0 Then;
private void ErrRaise(ref int e) {
string sText = "";
string sSource = "";
if(e > 1000) {
sSource = App.EXEName + ".CommonDialog";
Err.Raise(COMError(e), sSource, sText);
} else {
 // Raise standard Visual Basic error
sSource = App.EXEName + ".VBError";
Err.Raise(e, sSource);
}
}
#End If;


private void StrToBytes(ref List<Byte> ab, ref string s) {
if(IsArrayEmpty(ab)) {
 // Assign to empty array
ab = StrConv(s, vbFromUnicode);
} else {
int cab = 0;
 // Copy to existing array, padding or truncating if necessary
cab = ab.Count - 0 + 1;
if(Len(s) < cab)s = s + String$(cab - Len(s), 0);
 // If UnicodeTypeLib Then
 // Dim st As String
 // st = StrConv(s, vbFromUnicode)
 // CopyMemoryStr ab(LBound(ab)), st, cab
 // Else
CopyMemoryStr(ab[0], s, cab);
 // End If
}
}


private string BytesToStr(ref List<Byte> ab) {
string _BytesToStr = "";
_BytesToStr = StrConv(ab, vbUnicode);
return _BytesToStr;
}

private int COMError(ref int e) {
int _COMError = 0;
_COMError = e || vbObjectError;
return _COMError;
}

private bool IsArrayEmpty(ref dynamic va) {
bool _IsArrayEmpty = false;
dynamic v = null;
// TODO: (NOT SUPPORTED): On Error Resume Next
v = va(0);
_IsArrayEmpty = (Err != 0);
return _IsArrayEmpty;
}







}