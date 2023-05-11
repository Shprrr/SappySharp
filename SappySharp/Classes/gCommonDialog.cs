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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

namespace SappySharp.Classes;

public partial class gCommonDialog
{
    public enum EErrorCommonDialog
    {
        eeBaseCommonDialog = 13450 // CommonDialog
    }

    [DllImport("kernel32", EntryPoint = "lstrlenA")]
    private static extern int lstrlen(string lpString);
    [LibraryImport("kernel32")]
    private static partial int GlobalAlloc(int wFlags, int dwBytes);
    [LibraryImport("kernel32")]
    private static partial int GlobalCompact(int dwMinFree);
    [LibraryImport("kernel32")]
    private static partial int GlobalFree(int hMem);
    [LibraryImport("kernel32")]
    private static partial int GlobalLock(int hMem);
    [LibraryImport("kernel32")]
    private static partial int GlobalReAlloc(int hMem, int dwBytes, int wFlags);
    [LibraryImport("kernel32")]
    private static partial int GlobalSize(int hMem);
    [LibraryImport("kernel32")]
    private static partial int GlobalUnlock(int hMem);
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(ref dynamic lpvDest, ref dynamic lpvSource, int cbCopy);
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemoryStr(ref dynamic lpvDest, string lpvSource, int cbCopy);

    private const int MAX_PATH = 260;
    private const int MAX_FILE = 260;

    class OPENFILENAME
    {
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

    [DllImport("COMDLG32", EntryPoint = "GetOpenFileNameA")]
    private static extern int GetOpenFileName(ref OPENFILENAME file);
    [DllImport("COMDLG32", EntryPoint = "GetSaveFileNameA")]
    private static extern int GetSaveFileName(ref OPENFILENAME file);
    [DllImport("COMDLG32", EntryPoint = "GetFileTitleA")]
    private static extern int GetFileTitle(string szFile, string szTitle, int cbBuf);

    public enum EOpenFile
    {
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

    class TCHOOSECOLOR
    {
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

    [DllImport("COMDLG32.DLL", EntryPoint = "ChooseColorA")]
    private static extern int ChooseColor(ref TCHOOSECOLOR Color);

    public enum EChooseColor
    {
        CC_RGBInit = 0x1
    , CC_FullOpen = 0x2
    , CC_PreventFullOpen = 0x4
    , CC_ColorShowHelp = 0x8
    // Win95 only
    , CC_SolidColor = 0x80
    , CC_AnyColor = 0x100
    // End Win95 only
    , CC_ENABLEHOOK = 0x10
    , CC_ENABLETEMPLATE = 0x20
    , CC_EnableTemplateHandle = 0x40
    }
    [LibraryImport("USER32")]
    private static partial int GetSysColor(int nIndex);

    class TCHOOSEFONT
    {
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
    [DllImport("COMDLG32", EntryPoint = "ChooseFontA")]
    private static extern int ChooseFont(ref TCHOOSEFONT chfont);

    private const int LF_FACESIZE = 32;
    class LOGFONT
    {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;
        public byte[] lfFaceName = new byte[LF_FACESIZE];
    }

    [Flags]
    public enum EChooseFont
    {
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
    // Win95 only
    , CF_SelectScript = 0x400000
    , CF_NoScriptSel = 0x800000
    , CF_NoVertFonts = 0x1000000

    , CF_InitToLogFontStruct = 0x40
    , CF_Apply = 0x200
    , CF_EnableHook = 0x8
    , CF_EnableTemplate = 0x10
    , CF_EnableTemplateHandle = 0x20
    , CF_FontNotSupported = 0x238
    }

    // These are extra nFontType bits that are added to what is returned to the
    // EnumFonts callback routine

    public enum EFontType
    {
        Simulated_FontType = 0x8000
    , Printer_FontType = 0x4000
    , Screen_FontType = 0x2000
    , Bold_FontType = 0x100
    , Italic_FontType = 0x200
    , Regular_FontType = 0x400
    }

    class TPRINTDLG
    {
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
    private const int DMCOLLATE_FALSE = 0;
    private const int DMCOLLATE_TRUE = 1;

    [DllImport("COMDLG32.DLL", EntryPoint = "PrintDlgA")]
    private static extern int PrintDlg(ref TPRINTDLG prtdlg);

    public enum EPrintDialog
    {
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

    class DEVNAMES
    {
        public int wDriverOffset;
        public int wDeviceOffset;
        public int wOutputOffset;
        public int wDefault;
    }

    private const int CCHDEVICENAME = 32;
    private const int CCHFORMNAME = 32;
    class DevMode
    {
        public string dmDeviceName; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (CCHDEVICENAME)
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
        public string dmFormName; // TODO: (NOT SUPPORTED) Fixed Length String not supported: (CCHFORMNAME)
        public int dmUnusedPadding;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
    }

    // New Win95 Page Setup dialogs are up to you
    class POINTL
    {
        public int x;
        public int y;
    }
    class RECT
    {
        public int Left;
        public int TOp;
        public int Right;
        public int Bottom;
    }


    class TPAGESETUPDLG
    {
        public int lStructSize;
        public int hWndOwner;
        public int hDevMode;
        public int hDevNames;
        public int flags;
        public POINTL ptPaperSize;
        public RECT rtMinMargin;
        public RECT rtMargin;
        public int hInstance;
        public int lCustData;
        public int lpfnPageSetupHook;
        public int lpfnPagePaintHook;
        public int lpPageSetupTemplateName;
        public int hPageSetupTemplate;
    }

    // EPaperSize constants same as vbPRPS constants
    public enum EPaperSize
    {
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
    public enum EPrintQuality
    {
        epqDraft = -1
    , epqLow = -2
    , epqMedium = -3
    , epqHigh = -4
    }

    public enum EOrientation
    {
        eoPortrait = 1
    , eoLandscape
    }

    [DllImport("COMDLG32", EntryPoint = "PageSetupDlgA")]
    private static extern bool PageSetupDlg(ref TPAGESETUPDLG lppage);

    public enum EPageSetup
    {
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

    public enum EPageSetupUnits
    {
        epsuInches
    , epsuMillimeters
    }

    // Common dialog errors

    [LibraryImport("COMDLG32")]
    private static partial int CommDlgExtendedError();

    public enum EDialogError
    {
        CDERR_DIALOGFAILURE = 0xFFFF

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

    , CFERR_CHOOSEFONTCODES = 0x2000
    , CFERR_NOFONTS = 0x2001
    , CFERR_MAXLESSTHANMIN = 0x2002

    , FNERR_FILENAMECODES = 0x3000
    , FNERR_SUBCLASSFAILURE = 0x3001
    , FNERR_INVALIDFILENAME = 0x3002
    , FNERR_BUFFERTOOSMALL = 0x3003

    , CCERR_CHOOSECOLORCODES = 0x5000
    }

    // Array of custom colors lasts for life of app
    int[] alCustom = new int[15];
    bool fNotFirst = false;

    public enum EPrintRange
    {
        eprAll
    , eprPageNumbers
    , eprSelection
    }
    int m_lApiReturn = 0;
    int m_lExtendedError = 0;
    DevMode m_dvmode = null;

    public int APIReturn => m_lApiReturn;

    public int ExtendedError => m_lExtendedError;

#if fComponent
    public gCommonDialog()
    {
        InitColors();
    }
#endif

    public bool VBGetOpenFileName(ref string Filename, ref string FileTitle, ref bool ReadOnly, ref string Filter /*= "All (*.*)| *.*"*/, ref int FilterIndex, ref string InitDir, ref string DlgTitle, ref string DefaultExt, ref int flags, bool FileMustExist = true, bool MultiSelect = false, bool HideReadOnly = false, int Owner = -1)
    {
        bool _VBGetOpenFileName;

        OPENFILENAME opfile = null;
        string s = "";

        m_lApiReturn = 0;
        m_lExtendedError = 0;

        opfile.lStructSize = Len(opfile);

        // Add in specific flags and strip out non-VB flags

        opfile.flags = (FileMustExist ? 1 : 0 * (int)EOpenFile.OFN_FILEMUSTEXIST) | (MultiSelect ? 1 : 0 * (int)EOpenFile.OFN_ALLOWMULTISELECT) | (ReadOnly ? 1 : 0 * (int)EOpenFile.OFN_READONLY) | (HideReadOnly ? 1 : 0 * (int)EOpenFile.OFN_HIDEREADONLY) | (flags & CInt(~((int)EOpenFile.OFN_ENABLEHOOK | (int)EOpenFile.OFN_ENABLETEMPLATE)));
        // Owner can take handle of owning window
        if (Owner != -1) opfile.hWndOwner = Owner;
        // InitDir can take initial directory string
        opfile.lpstrInitialDir = InitDir;
        // DefaultExt can take default extension
        opfile.lpstrDefExt = DefaultExt;
        // DlgTitle can take dialog box title
        opfile.lpstrTitle = DlgTitle;

        // To make Windows-style filter, replace | and : with nulls
        for (int i = 1; i <= Len(Filter); i += 1)
        {
            string ch = Mid(Filter, i, 1);
            if (ch == "|" || ch == ":")
            {
                s += vbNullChar;
            }
            else
            {
                s += ch;
            }
        }
        // Put double null at end
        s = s + vbNullChar + vbNullChar;
        opfile.lpstrFilter = s;
        opfile.nFilterIndex = FilterIndex;

        // Pad file and file title buffers to maximum path
        s = Filename + new string('\0', MAX_PATH - Len(Filename));
        opfile.lpstrFile = s;
        opfile.nMaxFile = MAX_PATH;
        s = FileTitle + new string('\0', MAX_FILE - Len(FileTitle));
        opfile.lpstrFileTitle = s;
        opfile.nMaxFileTitle = MAX_FILE;
        // All other fields set to zero

        m_lApiReturn = GetOpenFileName(ref opfile);
        switch (m_lApiReturn)
        {
            case 1:
                // Success
                _VBGetOpenFileName = true;
                Filename = StrZToStr(opfile.lpstrFile);
                FileTitle = StrZToStr(opfile.lpstrFileTitle);
                flags = opfile.flags;
                // Return the filter index
                FilterIndex = opfile.nFilterIndex;
                // Look up the filter the user selected and return that
                Filter = FilterLookup(opfile.lpstrFilter, FilterIndex);
                if ((opfile.flags & (int)EOpenFile.OFN_READONLY) != 0) ReadOnly = true;
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
        return _VBGetOpenFileName;
    }
    private string StrZToStr(string s) => Left(s, lstrlen(s));

    bool VBGetSaveFileName(ref string Filename, ref string FileTitle, ref string Filter /*= "All (*.*)| *.*"*/, ref int FilterIndex, string InitDir, string DlgTitle, string DefaultExt, ref int flags, bool OverWritePrompt = true, int Owner = -1)
    {
        bool _VBGetSaveFileName;

        OPENFILENAME opfile = null;
        string s = "";

        m_lApiReturn = 0;
        m_lExtendedError = 0;

        opfile.lStructSize = Len(opfile);

        // Add in specific flags and strip out non-VB flags
        opfile.flags = (OverWritePrompt ? 1 : 0 * (int)EOpenFile.OFN_OVERWRITEPROMPT) | (int)EOpenFile.OFN_HIDEREADONLY | (flags & CInt(~((int)EOpenFile.OFN_ENABLEHOOK | (int)EOpenFile.OFN_ENABLETEMPLATE)));
        // Owner can take handle of owning window
        if (Owner != -1) opfile.hWndOwner = Owner;
        // InitDir can take initial directory string
        opfile.lpstrInitialDir = InitDir;
        // DefaultExt can take default extension
        opfile.lpstrDefExt = DefaultExt;
        // DlgTitle can take dialog box title
        opfile.lpstrTitle = DlgTitle;

        // Make new filter with bars (|) replacing nulls and double null at end
        for (int i = 1; i <= Len(Filter); i += 1)
        {
            string ch = Mid(Filter, i, 1);
            if (ch == "|" || ch == ":")
            {
                s += vbNullChar;
            }
            else
            {
                s += ch;
            }
        }
        // Put double null at end
        s = s + vbNullChar + vbNullChar;
        opfile.lpstrFilter = s;
        opfile.nFilterIndex = FilterIndex;

        // Pad file and file title buffers to maximum path
        s = Filename + new string('\0', MAX_PATH - Len(Filename));
        opfile.lpstrFile = s;
        opfile.nMaxFile = MAX_PATH;
        s = FileTitle + new string('\0', MAX_FILE - Len(FileTitle));
        opfile.lpstrFileTitle = s;
        opfile.nMaxFileTitle = MAX_FILE;
        // All other fields zero

        m_lApiReturn = GetSaveFileName(ref opfile);
        switch (m_lApiReturn)
        {
            case 1:
                _VBGetSaveFileName = true;
                Filename = StrZToStr(opfile.lpstrFile);
                FileTitle = StrZToStr(opfile.lpstrFileTitle);
                flags = opfile.flags;
                // Return the filter index
                FilterIndex = opfile.nFilterIndex;
                // Look up the filter the user selected and return that
                Filter = FilterLookup(opfile.lpstrFilter, FilterIndex);
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
        return _VBGetSaveFileName;
    }

    private string FilterLookup(string sFilters, int iCur)
    {
        string _FilterLookup = "";
        string s;
        int iStart = 1;
        if (sFilters == "") return _FilterLookup;
        do
        {
            // Cut out both parts marked by null character
            int iEnd = InStr(iStart, sFilters, vbNullChar);
            if (iEnd == 0) return _FilterLookup;
            iEnd = InStr(iEnd + 1, sFilters, vbNullChar);
            if (iEnd != 0)
            {
                s = Mid(sFilters, iStart, iEnd - iStart);
            }
            else
            {
                s = Mid(sFilters, iStart);
            }
            iStart = iEnd + 1;
            if (iCur == 1)
            {
                _FilterLookup = s;
                return _FilterLookup;
            }
            iCur--;
        } while (iCur != 0);
        return _FilterLookup;
    }

    public string VBGetFileTitle(string sFile)
    {
        string sFileTitle = new('\0', MAX_PATH);
        int cFileTitle = GetFileTitle(sFile, sFileTitle, MAX_PATH);
        if (cFileTitle != 0)
        {
            return "";
        }
        else
        {
            return Left(sFileTitle, InStr(sFileTitle, vbNullChar) - 1);
        }
    }

    // ChooseColor wrapper
    bool VBChooseColor(ref int Color, int flags, bool AnyColor = true, bool FullOpen = false, bool DisableFullOpen = false, int Owner = -1)
    {
        bool _VBChooseColor;

        TCHOOSECOLOR chclr = null;
        chclr.lStructSize = Len(chclr);

        // Color must get reference variable to receive result
        // Flags can get reference variable or constant with bit flags
        // Owner can take handle of owning window
        if (Owner != -1) chclr.hWndOwner = Owner;

        // Assign color (default uninitialized value of zero is good default)
        chclr.rgbResult = Color;

        // Mask out unwanted bits
        int afMask = CInt(~((int)EChooseColor.CC_ENABLEHOOK | (int)EChooseColor.CC_ENABLETEMPLATE));
        // Pass in flags
        chclr.flags = afMask & ((int)EChooseColor.CC_RGBInit | IIf(AnyColor, (int)EChooseColor.CC_AnyColor, (int)EChooseColor.CC_SolidColor) | (FullOpen ? 1 : 0 * (int)EChooseColor.CC_FullOpen) | (DisableFullOpen ? 1 : 0 * (int)EChooseColor.CC_PreventFullOpen));

        // If first time, initialize to white
        if (fNotFirst == false) InitColors();

        GCHandle gch = GCHandle.Alloc(alCustom[0], GCHandleType.Pinned);
        chclr.lpCustColors = (int)gch.AddrOfPinnedObject();
        gch.Free();
        // All other fields zero

        m_lApiReturn = ChooseColor(ref chclr);
        switch (m_lApiReturn)
        {
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

    private void InitColors()
    {
        // Initialize with first 16 system interface colors
        for (int i = 0; i <= 15; i += 1)
        {
            alCustom[i] = GetSysColor(i);
        }
        fNotFirst = true;
    }

    // Property to read or modify custom colors (use to save colors in registry)
    public int CustomColor(int i)
    {
        // If first time, initialize to white
        if (fNotFirst == false) InitColors();
        if (i >= 0 && i <= 15)
        {
            return alCustom[i];
        }
        else
        {
            return -1;
        }
    }
    public void CustomColor(int i, int iValue)
    {
        // If first time, initialize to system colors
        if (fNotFirst == false) InitColors();
        if (i >= 0 && i <= 15)
        {
            alCustom[i] = iValue;
        }
    }

    // ChooseFont wrapper
    bool VBChooseFont(ref Font CurFont, ref int PrinterDC, ref int Color, ref int flags, int Owner = -1, int MinSize = 0, int MaxSize = 0)
    {
        bool _VBChooseFont;

        m_lApiReturn = 0;
        m_lExtendedError = 0;

        // Unwanted Flags bits
        var CF_FontNotSupported = EChooseFont.CF_Apply | EChooseFont.CF_EnableHook | EChooseFont.CF_EnableTemplate;

        // Flags can get reference variable or constant with bit flags
        // PrinterDC can take printer DC
        if (PrinterDC == -1)
        {
            PrinterDC = 0;
            if ((flags & (int)EChooseFont.CF_PrinterFonts) != 0) PrinterDC = Printer.hdc;
        }
        else
        {
            flags |= (int)EChooseFont.CF_PrinterFonts;
        }
        // Must have some fonts
        if ((flags & (int)EChooseFont.CF_PrinterFonts) == 0) flags |= (int)EChooseFont.CF_ScreenFonts;
        // Color can take initial color, receive chosen color
        if (Color != vbBlack) flags |= (int)EChooseFont.CF_EFFECTS;
        // MinSize can be minimum size accepted
        if (MinSize != 0) flags |= (int)EChooseFont.CF_LimitSize;
        // MaxSize can be maximum size accepted
        if (MaxSize != 0) flags |= (int)EChooseFont.CF_LimitSize;

        // Put in required internal flags and remove unsupported
        flags = (flags | (int)EChooseFont.CF_InitToLogFontStruct) & ~(int)CF_FontNotSupported;

        // Initialize LOGFONT variable
        LOGFONT fnt = null;
        var PointsPerTwip = 1440 / 72;
        fnt.lfHeight = (int)-(CurFont.Size * (PointsPerTwip / Screen.TwipsPerPixelY));
        fnt.lfWeight = CurFont.Bold ? 700 : 400;
        fnt.lfItalic = (byte)(CurFont.Italic ? 1 : 0);
        fnt.lfUnderline = (byte)(CurFont.Underline ? 1 : 0);
        fnt.lfStrikeOut = (byte)(CurFont.Strikeout ? 1 : 0);
        // Other fields zero
        string name = CurFont.Name;
        StrToBytes(ref fnt.lfFaceName, ref name);

        // Initialize TCHOOSEFONT variable
        TCHOOSEFONT cf = null;
        cf.lStructSize = Len(cf);
        if (Owner != -1) cf.hWndOwner = Owner;
        cf.hdc = PrinterDC;
        GCHandle gch = GCHandle.Alloc(fnt, GCHandleType.Pinned);
        cf.lpLogFont = (int)gch.AddrOfPinnedObject();
        gch.Free();
        cf.iPointSize = (int)(CurFont.Size * 10);
        cf.flags = flags;
        cf.rgbColors = Color;
        cf.nSizeMin = MinSize;
        cf.nSizeMax = MaxSize;

        // All other fields zero
        m_lApiReturn = ChooseFont(ref cf);
        switch (m_lApiReturn)
        {
            case 1:
                // Success
                _VBChooseFont = true;
                flags = cf.flags;
                Color = cf.rgbColors;
                // CurFont.Italic = cf.nFontType And Italic_FontType
                //CurFont.Weight = fnt.lfWeight;
                System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
                if ((cf.nFontType & (int)EFontType.Bold_FontType) != 0) fontStyle |= System.Drawing.FontStyle.Bold;
                if (fnt.lfItalic != 0) fontStyle |= System.Drawing.FontStyle.Italic;
                if (fnt.lfStrikeOut != 0) fontStyle |= System.Drawing.FontStyle.Strikeout;
                if (fnt.lfUnderline != 0) fontStyle |= System.Drawing.FontStyle.Underline;
                CurFont = new Font(BytesToStr(fnt.lfFaceName), cf.iPointSize / 10, fontStyle);
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
    bool VBPrintDlg(ref int hdc, ref EPrintRange PrintRange, bool DisablePageNumbers, ref int FromPage, ref int ToPage, bool DisableSelection, ref int Copies, bool ShowPrintToFile, ref bool PrintToFile, ref bool Collate, bool PreventWarning, int Owner, dynamic Printer, int flags, bool DisablePrintToFile = true)
    {
        bool _VBPrintDlg;

        m_lApiReturn = 0;
        m_lExtendedError = 0;

        // Set PRINTDLG flags
        int afFlags = (DisablePageNumbers ? 1 : 0 * (int)EPrintDialog.PD_NOPAGENUMS) | (DisablePrintToFile ? 1 : 0 * (int)EPrintDialog.PD_DISABLEPRINTTOFILE) | (DisableSelection ? 1 : 0 * (int)EPrintDialog.PD_NOSELECTION) | (PrintToFile ? 1 : 0 * (int)EPrintDialog.PD_PRINTTOFILE) | ((ShowPrintToFile ? 0 : 1) * (int)EPrintDialog.PD_HIDEPRINTTOFILE) | (PreventWarning ? 1 : 0 * (int)EPrintDialog.PD_NOWARNING) | (Collate ? 1 : 0 * (int)EPrintDialog.PD_COLLATE) | (int)EPrintDialog.PD_USEDEVMODECOPIESANDCOLLATE | (int)EPrintDialog.PD_RETURNDC;
        if (PrintRange == EPrintRange.eprPageNumbers)
        {
            afFlags |= (int)EPrintDialog.PD_PAGENUMS;
        }
        else if (PrintRange == EPrintRange.eprSelection)
        {
            afFlags |= (int)EPrintDialog.PD_SELECTION;
        }
        // Mask out unwanted bits
        int afMask = CInt(~((int)EPrintDialog.PD_ENABLEPRINTHOOK | (int)EPrintDialog.PD_ENABLEPRINTTEMPLATE));
        afMask &= CInt(~((int)EPrintDialog.PD_ENABLESETUPHOOK | (int)EPrintDialog.PD_ENABLESETUPTEMPLATE));

        // Fill in PRINTDLG structure
        TPRINTDLG pd = null;
        pd.lStructSize = Len(pd);
        pd.hWndOwner = Owner;
        pd.flags = afFlags & afMask;
        pd.nFromPage = FromPage;
        pd.nToPage = ToPage;
        pd.nMinPage = 1;
        pd.nMaxPage = 0xFFFF;

        // Show Print dialog
        m_lApiReturn = PrintDlg(ref pd);
        switch (m_lApiReturn)
        {
            case 1:
                _VBPrintDlg = true;
                // Return dialog values in parameters
                hdc = pd.hdc;
                if ((pd.flags & (int)EPrintDialog.PD_PAGENUMS) != 0)
                {
                    PrintRange = EPrintRange.eprPageNumbers;
                }
                else if ((pd.flags & (int)EPrintDialog.PD_SELECTION) != 0)
                {
                    PrintRange = EPrintRange.eprSelection;
                }
                else
                {
                    PrintRange = EPrintRange.eprAll;
                }
                FromPage = pd.nFromPage;
                ToPage = pd.nToPage;
                PrintToFile = (pd.flags & (int)EPrintDialog.PD_PRINTTOFILE) != 0;
                // Get DEVMODE structure from PRINTDLG
                int pDevMode = GlobalLock(pd.hDevMode);
                dynamic dest = m_dvmode;
                dynamic source = pDevMode;
                CopyMemory(ref dest, ref source, Len(m_dvmode));
                m_dvmode = dest;
                GlobalUnlock(pd.hDevMode);
                // Get Copies and Collate settings from DEVMODE structure
                Copies = m_dvmode.dmCopies;
                Collate = m_dvmode.dmCollate == DMCOLLATE_TRUE;

                // Set default printer properties
                // TODO: (NOT SUPPORTED): On Error Resume Next
                if (!(Printer == null))
                {
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
    private DevMode DevModeValue => m_dvmode;


    // PageSetupDlg wrapper
    bool VBPageSetupDlg(int Owner, bool DisableMargins, bool DisableOrientation, bool DisablePaper, bool DisablePrinter, ref int LeftMargin, ref int MinLeftMargin, ref int RightMargin, ref int MinRightMargin, ref int TopMargin, ref int MinTopMargin, ref int BottomMargin, ref int MinBottomMargin, ref EPaperSize PaperSize, ref EOrientation Orientation, ref EPrintQuality PrintQuality, dynamic Printer, int flags, EPageSetupUnits Units = EPageSetupUnits.epsuInches)
    {
        bool _VBPageSetupDlg = false;

        m_lApiReturn = 0;
        m_lExtendedError = 0;
        // Mask out unwanted bits
        int afMask = ~((int)EPageSetup.PSD_EnablePagePaintHook | (int)EPageSetup.PSD_EnablePageSetupHook | (int)EPageSetup.PSD_EnablePageSetupTemplate);
        // Set TPAGESETUPDLG flags
        int afFlags = (DisableMargins ? 1 : 0 * (int)EPageSetup.PSD_DISABLEMARGINS) | (DisableOrientation ? 1 : 0 * (int)EPageSetup.PSD_DISABLEORIENTATION) | (DisablePaper ? 1 : 0 * (int)EPageSetup.PSD_DISABLEPAPER) | (DisablePrinter ? 1 : 0 * (int)EPageSetup.PSD_DISABLEPRINTER) | (int)EPageSetup.PSD_MARGINS | (int)EPageSetup.PSD_MINMARGINS & afMask;
        int lUnits;
        if (Units == EPageSetupUnits.epsuInches)
        {
            afFlags |= (int)EPageSetup.PSD_INTHOUSANDTHSOFINCHES;
            lUnits = 1000;
        }
        else
        {
            afFlags |= (int)EPageSetup.PSD_INHUNDREDTHSOFMILLIMETERS;
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
        if (PageSetupDlg(ref psd))
        {
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
            int pDevMode = GlobalLock(psd.hDevMode);
            dynamic dest = dvmode;
            dynamic source = pDevMode;
            CopyMemory(ref dest, ref source, Len(dvmode));
            dvmode = dest;
            GlobalUnlock(psd.hDevMode);
            PaperSize = (EPaperSize)dvmode.dmPaperSize;
            Orientation = (EOrientation)dvmode.dmOrientation;
            PrintQuality = (EPrintQuality)dvmode.dmPrintQuality;
            // Set default printer properties
            // TODO: (NOT SUPPORTED): On Error Resume Next
            if (!(Printer == null))
            {
                Printer.Copies = dvmode.dmCopies;
                Printer.Orientation = dvmode.dmOrientation;
                Printer.PaperSize = dvmode.dmPaperSize;
                Printer.PrintQuality = dvmode.dmPrintQuality;
            }
            // TODO: (NOT SUPPORTED): On Error GoTo 0
        }

        return _VBPageSetupDlg;
    }

#if !fComponent
    private void ErrRaise(int e)
    {
        string sText = "";
        string sSource;
        if (e > 1000)
        {
            sSource = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".CommonDialog";
            Err().Raise(COMError(e), sSource, sText);
        }
        else
        {
            // Raise standard Visual Basic error
            sSource = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".VBError";
            Err().Raise(e, sSource);
        }
    }
#endif

    private void StrToBytes(ref byte[] ab, ref string s)
    {
        if (IsArrayEmpty(ab))
        {
            // Assign to empty array
            ab = Encoding.Unicode.GetBytes(s);
        }
        else
        {
            // Copy to existing array, padding or truncating if necessary
            int cab = ab.Length - 0 + 1;
            if (Len(s) < cab) s += new string('\0', cab - Len(s));
            // If UnicodeTypeLib Then
            //     Dim st As String
            //     st = StrConv(s, vbFromUnicode)
            //     CopyMemoryStr ab(LBound(ab)), st, cab
            // Else
            dynamic dest = ab[0];
            CopyMemoryStr(ref dest, s, cab);
            ab[0] = dest;
            // End If
        }
    }

    private string BytesToStr(byte[] ab) => Encoding.Unicode.GetString(ab);

    private int COMError(int e) => e | vbObjectError;

    private bool IsArrayEmpty(dynamic va)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        dynamic v = va[0];
        return Err().Number != 0;
    }
}
