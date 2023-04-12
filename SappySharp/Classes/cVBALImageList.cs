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



public class cVBALImageList {


 // =========================================================================
 // vbAccelerator Image List Control Demonstrator
 // Copyright © 1998 Steve McMahon (steve@dogma.demon.co.uk)

 // Implements an Image List control in VB using COMCTL32.DLL

 // Visit vbAccelerator at www.dogma.demon.co.uk
 // =========================================================================

 // -----------
 // API
 // -----------
 // General:
[DllImport("user32")]
private static extern int GetWindowWord(int hwnd, int nIndex);
var GWW_HINSTANCE = ((-6));

 // GDI object functions:
[DllImport("gdi32")]
private static extern int SelectObject(int hdc, int hObject);
[DllImport("gdi32")]
private static extern int DeleteObject(int hObject);
[DllImport("user32")]
private static extern int DestroyCursor(int hCursor);
[DllImport("gdi32", EntryPoint="GetObjectA")]
private static extern int GetObjectAPI(int hObject, int nCount, ref dynamic lpObject);
[DllImport("user32")]
private static extern int GetDC(int hwnd);
[DllImport("gdi32")]
private static extern int DeleteDC(int hdc);
[DllImport("user32")]
private static extern int ReleaseDC(int hwnd, int hdc);
[DllImport("gdi32")]
private static extern int CreateCompatibleDC(int hdc);
[DllImport("gdi32")]
private static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);
[DllImport("gdi32")]
private static extern int GetDeviceCaps(int hdc, int nIndex);
int BITSPIXEL = 12;
int LOGPIXELSX = 88; // Logical pixels/inch in X
int LOGPIXELSY = 90; // Logical pixels/inch in Y
 // System metrics:
[DllImport("user32")]
private static extern int GetSystemMetrics(int nIndex);
int SM_CXICON = 11;
int SM_CYICON = 12;
int SM_CXFRAME = 32;
int SM_CYCAPTION = 4;
int SM_CYFRAME = 33;
int SM_CYBORDER = 6;
int SM_CXBORDER = 5;

 // Region paint and fill functions:
[DllImport("gdi32")]
private static extern int PaintRgn(int hdc, int hRgn);
[DllImport("gdi32")]
private static extern int ExtFloodFill(int hdc, int x, int y, int crColor, int wFillType);
int FLOODFILLBORDER = 0;
int FLOODFILLSURFACE = 1;
[DllImport("gdi32")]
private static extern int GetPixel(int hdc, int x, int y);

 // Pen functions:
[DllImport("gdi32")]
private static extern int CreatePen(int nPenStyle, int nWidth, int crColor);
int PS_DASH = 1;
int PS_DASHDOT = 3;
int PS_DASHDOTDOT = 4;
int PS_DOT = 2;
int PS_SOLID = 0;
int PS_NULL = 5;

 // Brush functions:
[DllImport("gdi32")]
private static extern int CreateSolidBrush(int crColor);
[DllImport("gdi32")]
private static extern int CreatePatternBrush(int hBitmap);

 // Line functions:
[DllImport("gdi32")]
private static extern int LineTo(int hdc, int x, int y);
  class POINTAPI{ 
  public int x;
  public int y;
}
[DllImport("gdi32")]
private static extern int MoveToEx(int hdc, int x, int y, ref POINTAPI lpPoint);

 // Colour functions:
[DllImport("gdi32")]
private static extern int SetTextColor(int hdc, int crColor);
[DllImport("gdi32")]
private static extern int SetBkColor(int hdc, int crColor);
[DllImport("gdi32")]
private static extern int SetBkMode(int hdc, int nBkMode);
int OPAQUE = 2;
int TRANSPARENT = 1;
[DllImport("user32")]
private static extern int GetSysColor(int nIndex);
int COLOR_ACTIVEBORDER = 10;
int COLOR_ACTIVECAPTION = 2;
int COLOR_ADJ_MAX = 100;
int COLOR_ADJ_MIN = -100;
int COLOR_APPWORKSPACE = 12;
int COLOR_BACKGROUND = 1;
int COLOR_BTNFACE = 15;
int COLOR_BTNHIGHLIGHT = 20;
int COLOR_BTNSHADOW = 16;
int COLOR_BTNTEXT = 18;
int COLOR_CAPTIONTEXT = 9;
int COLOR_GRAYTEXT = 17;
int COLOR_HIGHLIGHT = 13;
int COLOR_HIGHLIGHTTEXT = 14;
int COLOR_INACTIVEBORDER = 11;
int COLOR_INACTIVECAPTION = 3;
int COLOR_INACTIVECAPTIONTEXT = 19;
int COLOR_MENU = 4;
int COLOR_MENUTEXT = 7;
int COLOR_SCROLLBAR = 0;
int COLOR_WINDOW = 5;
int COLOR_WINDOWFRAME = 6;
int COLOR_WINDOWTEXT = 8;
int COLORONCOLOR = 3;

 // Shell Extract icon functions:
[DllImport("shell32.dll", EntryPoint="FindExecutableA")]
private static extern int FindExecutable(string lpFile, string lpDirectory, string lpResult);
[DllImport("shell32.dll", EntryPoint="ExtractIconA")]
private static extern int ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

 // Icon functions:
[DllImport("user32")]
private static extern int DrawIcon(int hdc, int x, int y, int hIcon);
[DllImport("user32")]
private static extern int DestroyIcon(int hIcon);
[DllImport("user32")]
private static extern bool DrawIconEx(int hdc, int xLeft, int yTop, int hIcon, int cxWidth, int cyWidth, int istepIfAniCur, int hbrFlickerFreeDraw, int diFlags);
[DllImport("user32", EntryPoint="LoadImageA")]
private static extern int LoadImage(int hInst, string lpsz, int un1, int n1, int n2, int un2);
[DllImport("user32", EntryPoint="LoadImageA")]
private static extern int LoadImageLong(int hInst, int lpsz, int un1, int n1, int n2, int un2);
int LR_LOADMAP3DCOLORS = 0x1000;
int LR_LOADFROMFILE = 0x10;
int LR_LOADTRANSPARENT = 0x20;
int LR_COPYRETURNORG = 0x4;

 // Blitting functions
[DllImport("gdi32")]
private static extern int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
int SRCAND = 0x8800C6;
int SRCCOPY = 0xCC0020;
int SRCERASE = 0x440328;
int SRCINVERT = 0x660046;
int SRCPAINT = 0xEE0086;
int BLACKNESS = 0x42;
int WHITENESS = 0xFF0062;
[DllImport("gdi32")]
private static extern int PatBlt(int hdc, int x, int y, int nWidth, int nHeight, int dwRop);
[DllImport("user32", EntryPoint="LoadBitmapA")]
private static extern int LoadBitmapBynum(int hInstance, int lpBitmapName);
  class BITMAP{  // 14 bytes
  public int bmType;
  public int bmWidth;
  public int bmHeight;
  public int bmWidthBytes;
  public int bmPlanes;
  public int bmBitsPixel;
  public int bmBits;
}
[DllImport("gdi32")]
private static extern int CreateBitmapIndirect(ref BITMAP lpBitmap);

 // Text functions:
  class RECT{ 
  public int left;
  public int tOp;
  public int Right;
  public int Bottom;
}
[DllImport("user32")]
private static extern int PtInRect(ref RECT lpRect, int ptX, int ptY);
[DllImport("user32", EntryPoint="DrawTextA")]
private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
int DT_BOTTOM = 0x8;
int DT_CENTER = 0x1;
int DT_LEFT = 0x0;
int DT_CALCRECT = 0x400;
int DT_WORDBREAK = 0x10;
int DT_VCENTER = 0x4;
int DT_TOP = 0x0;
int DT_TABSTOP = 0x80;
int DT_SINGLELINE = 0x20;
int DT_RIGHT = 0x2;
int DT_NOCLIP = 0x100;
int DT_INTERNAL = 0x1000;
int DT_EXTERNALLEADING = 0x200;
int DT_EXPANDTABS = 0x40;
int DT_CHARSTREAM = 4;
int DT_NOPREFIX = 0x800;
  class DRAWTEXTPARAMS{ 
  public int cbSize;
  public int iTabLength;
  public int iLeftMargin;
  public int iRightMargin;
  public int uiLengthDrawn;
}
[DllImport("user32", EntryPoint="DrawTextExA")]
private static extern int DrawTextEx(int hdc, string lpsz, int n, ref RECT lpRect, int un, ref DRAWTEXTPARAMS lpDrawTextParams);
[DllImport("user32", EntryPoint="DrawTextExA")]
private static extern int DrawTextExAsNull(int hdc, string lpsz, int n, ref RECT lpRect, int un, int lpDrawTextParams);
int DT_EDITCONTROL = 0x2000;
int DT_PATH_ELLIPSIS = 0x4000;
int DT_END_ELLIPSIS = 0x8000;
int DT_MODIFYSTRING = 0x10000;
int DT_RTLREADING = 0x20000;
int DT_WORD_ELLIPSIS = 0x40000;

  class SIZEAPI{ 
  public int cX;
  public int cY;
}
[DllImport("gdi32", EntryPoint="GetTextExtentPoint32A")]
private static extern int GetTextExtentPoint32(int hdc, string lpsz, int cbString, ref SIZEAPI lpSize);
[DllImport("gdi32")]
private static extern int GetStockObject(int nIndex);
int ANSI_FIXED_FONT = 11;
int ANSI_VAR_FONT = 12;
int SYSTEM_FONT = 13;
int DEFAULT_GUI_FONT = 17; // win95 only
[DllImport("user32")]
private static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);
[DllImport("gdi32")]
private static extern int Rectangle(int hdc, int X1, int y1, int x2, int y2);
[DllImport("user32")]
private static extern int DrawEdge(int hdc, ref RECT qrc, int edge, int grfFlags);
int BF_LEFT = 1;
int BF_TOP = 2;
int BF_RIGHT = 4;
int BF_BOTTOM = 8;
var BF_RECT = BF_LEFT || BF_TOP || BF_RIGHT || BF_BOTTOM;
int BF_MIDDLE = 2048;
int BDR_SUNKENINNER = 8;
int BDR_SUNKENOUTER = 2;
int BDR_RAISEDOUTER = 1;
int BDR_RAISEDINNER = 4;

[DllImport("user32")]
private static extern int ShowWindow(int hwnd, int nCmdShow);
int SW_SHOWNOACTIVATE = 4;

 // Scrolling and region functions:
[DllImport("user32")]
private static extern int ScrollDC(int hdc, int dx, int dy, ref RECT lprcScroll, ref RECT lprcClip, int hrgnUpdate, ref RECT lprcUpdate);
[DllImport("user32")]
private static extern int SetWindowRgn(int hwnd, int hRgn, int bRedraw);
[DllImport("gdi32")]
private static extern int SelectClipRgn(int hdc, int hRgn);
[DllImport("gdi32")]
private static extern int CreateRectRgn(int X1, int y1, int x2, int y2);
[DllImport("gdi32")]
private static extern int CreateRectRgnIndirect(ref RECT lpRect);
[DllImport("gdi32")]
private static extern int CreatePolyPolygonRgn(ref POINTAPI lpPoint, ref int lpPolyCounts, int nCount, int nPolyFillMode);
[DllImport("gdi32")]
private static extern void CreatePolygonRgn(ref POINTAPI lpPoint, int nCount, int nPolyFillMode);
[DllImport("gdi32")]
private static extern int SaveDC(int hdc);
[DllImport("gdi32")]
private static extern int RestoreDC(int hdc, int hSavedDC);
[DllImport("gdi32", EntryPoint="CreateDCA")]
private static extern int CreateDCAsNull(string lpDriverName, ref dynamic lpDeviceName, ref dynamic lpOutput, ref dynamic lpInitData);

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
int FW_NORMAL = 400;
int FW_BOLD = 700;
int FF_DONTCARE = 0;
int DEFAULT_QUALITY = 0;
int DEFAULT_PITCH = 0;
int DEFAULT_CHARSET = 1;
[DllImport("gdi32", EntryPoint="CreateFontIndirectA")]
private static extern void CreateFontIndirect&(ref LOGFONT lpLogFont);
[DllImport("kernel32")]
private static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);
[DllImport("user32")]
private static extern int DrawFocusRect(int hdc, ref RECT lpRect);

[DllImport("user32", EntryPoint="DrawStateA")]
private static extern int DrawState(int hdc, int hBrush, int lpDrawStateProc, int lParam, int wParam, int x, int y, int cX, int cY, int fuFlags);

 // /* Image type */
int DST_COMPLEX = 0x0;
int DST_TEXT = 0x1;
int DST_PREFIXTEXT = 0x2;
int DST_ICON = 0x3;
int DST_BITMAP = 0x4;

 // /* State type */
int DSS_NORMAL = 0x0;
int DSS_UNION = 0x10; // Dither
int DSS_DISABLED = 0x20;
int DSS_MONO = 0x80; // Draw in colour of brush specified in hBrush
int DSS_RIGHT = 0x8000;

[DllImport("olepro32.dll")]
private static extern int OleTranslateColor(int OLE_COLOR, int HPALETTE, ref int pccolorref);
int CLR_INVALID = -1;

 // Image list functions:
[DllImport("COMCTL32")]
private static extern int ImageList_GetBkColor(int hImageList);
[DllImport("COMCTL32")]
private static extern int ImageList_ReplaceIcon(int hImageList, int i, int hIcon);
[DllImport("COMCTL32", EntryPoint="ImageList_Draw")]
private static extern int ImageList_Convert(int hImageList, int ImgIndex, int hDCDest, int x, int y, int flags);
[DllImport("COMCTL32")]
private static extern int ImageList_Create(int MinCx, int MinCy, int flags, int cInitial, int cGrow);
[DllImport("COMCTL32")]
private static extern int ImageList_AddMasked(int hImageList, int hbmImage, int crMask);
[DllImport("COMCTL32")]
private static extern int ImageList_Replace(int hImageList, int ImgIndex, int hbmImage, int hBmMask);
[DllImport("COMCTL32")]
private static extern int ImageList_Add(int hImageList, int hbmImage, ref int hBmMask);
[DllImport("COMCTL32")]
private static extern int ImageList_Remove(int hImageList, int ImgIndex);
  class IMAGEINFO{ 
  public int hBitmapImage;
  public int hBitmapMask;
  public int cPlanes;
  public int cBitsPerPixel;
  public RECT rcImage;
}
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_GetImageInfo(int hIml, int i, ref IMAGEINFO pImageInfo);
[DllImport("COMCTL32")]
private static extern int ImageList_AddIcon(int hIml, int hIcon);
[DllImport("COMCTL32")]
private static extern int ImageList_GetIcon(int hImageList, int ImgIndex, int fuFlags);
[DllImport("COMCTL32")]
private static extern void ImageList_SetImageCount(int hImageList, ref int uNewCount);
[DllImport("COMCTL32")]
private static extern int ImageList_GetImageCount(int hImageList);
[DllImport("COMCTL32")]
private static extern int ImageList_Destroy(int hImageList);
[DllImport("COMCTL32")]
private static extern int ImageList_GetIconSize(int hImageList, ref int cX, ref int cY);
[DllImport("COMCTL32")]
private static extern int ImageList_SetIconSize(int hImageList, ref int cX, ref int cY);

 // ImageList functions:
 // Draw:
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_Draw(int hIml, int i, int hdcDst, int x, int y, int fStyle);
int ILD_NORMAL = 0;
int ILD_TRANSPARENT = 1;
int ILD_BLEND25 = 2;
int ILD_SELECTED = 4;
int ILD_FOCUS = 4;
int ILD_MASK = 0x10;
int ILD_IMAGE = 0x20;
int ILD_ROP = 0x40;
int ILD_OVERLAYMASK = 3840;
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_GetImageRect(int hIml, int i, ref RECT prcImage);
 // Messages:
[DllImport("COMCTL32")]
private static extern int ImageList_DrawEx(int hIml, int i, int hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, int fStyle);
[DllImport("COMCTL32", EntryPoint="ImageList_LoadImageA")]
private static extern void ImageList_LoadImage(int hInst, string lpbmp, int cX, int cGrow, int crMask, int uType, int uFlags);
[DllImport("COMCTL32")]
private static extern int ImageList_SetBkColor(int hImageList, int clrBk);

int ILC_MASK = 0x1;

int CLR_DEFAULT = -16777216;
int CLR_HILIGHT = -16777216;
int CLR_NONE = -1;

int ILCF_MOVE = 0x0;
int ILCF_SWAP = 0x1;
[DllImport("COMCTL32")]
private static extern int ImageList_Copy(int himlDst, int iDst, int himlSrc, int iSrc, int uFlags);

[DllImport("kernel32", EntryPoint="GetTempFileNameA")]
private static extern int GetTempFileName(string lpszPath, string lpPrefixString, int wUnique, string lpTempFileName);
[DllImport("kernel32", EntryPoint="GetTempPathA")]
private static extern int GetTempPath(int nBufferLength, string lpBuffer);
int MAX_PATH = 260;
[DllImport("kernel32", EntryPoint="lstrlenA")]
private static extern int lstrlen(string lpString);

  class PictDesc{ 
  public int cbSizeofStruct;
  public int picType;
  public int hImage;
  public int xExt;
  public int yExt;
}
  class Guid{ 
  public int Data1;
  public int Data2;
  public int Data3;
  public Byte Data4; //  TODO: (NOT SUPPORTED) Array ranges not supported: Data4(0 To 7)
}
[DllImport("olepro32.dll")]
private static extern int OleCreatePictureIndirect(ref PictDesc lpPictDesc, ref Guid riid, int fPictureOwnsHandle, ref IPicture ipic);

 // -----------
 // ENUMS
 // -----------
  public enum eilIconState{ 
  Normal = 0
  , Disabled = 1
}

  public enum ImageTypes{ 
  IMAGE_BITMAP = 0
  , IMAGE_ICON = 1
  , IMAGE_CURSOR = 2
}

  public enum eilColourDepth{ 
  ILC_COLOR = 0x0
  , ILC_COLOR4 = 0x4
  , ILC_COLOR8 = 0x8
  , ILC_COLOR16 = 0x10
  , ILC_COLOR24 = 0x18
  , ILC_COLOR32 = 0x20
}

  public enum eilSwapTypes{ 
  eilCopy = ILCF_MOVE
  , eilSwap = ILCF_SWAP
}

 // ------------------
 // Private variables:
 // ------------------
int m_hIml = 0;
int m_lIconSizeX = 0;
int m_lIconSizeY = 0;
eilColourDepth m_eColourDepth = null;
List<string> m_sKey = new List<string>();
int m_HDC = 0;

  public dynamic OwnerHDC{ 
set {
m_HDC = lHDC;
}
}


  public eilColourDepth SystemColourDepth{ 
get {
eilColourDepth _SystemColourDepth = default(eilColourDepth);
im(lR As Long);
int lHDC = 0;
lHDC = CreateDCAsNull("DISPLAY", ByVal 0&, ByVal 0&, ByVal 0&);
lR = GetDeviceCaps(lHDC, BITSPIXEL);
DeleteDC(lHDC);
_SystemColourDepth = lR;
return _SystemColourDepth;
}
}


  public void SwapOrCopyImage(dynamic vKeySrc, dynamic vKeyDst, eilSwapTypes eSwap = eilSwap) {
  int lDst = 0;
  int lSrc = 0;
  string sKeyDst = "";
  string sKeySrc = "";
  
    if((m_hIml != 0)) {
    lDst = ItemIndex(vKeySrc);
      if((lDst > -1)) {
      lSrc = ItemIndex(vKeyDst);
        if((lSrc > -1)) {
        ImageList_Copy(m_hIml, lDst, m_hIml, lSrc, eSwap);
        sKeyDst = m_sKey[lDst];
        sKeySrc = m_sKey[lSrc];
        m_sKey[lDst] = sKeySrc;
        m_sKey[lSrc] = sKeyDst;
      }
    }
  }
}

  public bool Create() {
bool _Create = false;
  
   // Do we already have an image list?  Kill it if we have:
  Destroy();
  
   // Create the Imagelist:
  m_hIml = ImageList_Create(m_lIconSizeX, m_lIconSizeY, ILC_MASK || m_eColourDepth, 4, 4);
    if((m_hIml != 0) && (m_hIml != -1)) {
     // Ok
    _Create = true;
    } else {
    m_hIml = 0;
  }
  
return _Create;
}
  public void Destroy() {
   // Kill the image list if we have one:
    if((hIml != 0)) {
    ImageList_Destroy(hIml);
    m_hIml = 0;
  }
  m_sKey.Clear();
}
  public void DrawImage(dynamic vKey, int hdc, int xPixels, int yPixels, var bSelected = false, var bCut = false, var bDisabled = false, OLE_COLOR oCutDitherColour = vbWindowBackground, int hExternalIml = 0) {
  int hIcon = 0;
  int lFlags = 0;
  int lhIml = 0;
  int lColor = 0;
  int iImgIndex = 0;
  
   // Draw the image at 1 based index or key supplied in vKey.
   // on the hDC at xPixels,yPixels with the supplied options.
   // You can even draw an ImageList from another ImageList control
   // if you supply the handle to hExternalIml with this function.
  
  iImgIndex = ItemIndex(vKey);
    if((iImgIndex > -1)) {
      if((hExternalIml != 0)) {
      lhIml = hExternalIml;
      } else {
      lhIml = hIml;
    }
    
    lFlags = ILD_TRANSPARENT;
      if(((bSelected)) || ((bCut))) {
      lFlags = lFlags || ILD_SELECTED;
    }
    
      if(((bCut))) {
       // Draw dithered:
      lColor = TranslateColor(oCutDitherColour);
      if((lColor == -1))lColor = GetSysColor(COLOR_WINDOW);
      ImageList_DrawEx(lhIml, iImgIndex, hdc, xPixels, yPixels, 0, 0, CLR_NONE, lColor, lFlags);
      } else if(((bDisabled))) {
       // extract a copy of the icon:
      hIcon = ImageList_GetIcon(hIml, iImgIndex, 0);
       // Draw it disabled at x,y:
      DrawState(hdc, 0, 0, hIcon, 0, xPixels, yPixels, m_lIconSizeX, m_lIconSizeY, DST_ICON || DSS_DISABLED);
       // Clear up the icon:
      DestroyIcon(hIcon);
      
      } else {
       // Standard draw:
      ImageList_Draw(lhIml, iImgIndex, hdc, xPixels, yPixels, lFlags);
    }
  }
}

  public int IconSizeX{ 
get {
int _IconSizeX = default(int);
 // Returns the icon width
_IconSizeX = m_lIconSizeX;
return _IconSizeX;
}
set {
 // Sets the icon width.  NB no change at runtime unless you
 // call Create and add all the images in again.
m_lIconSizeX = lSizeX;
}
}

  
  public int IconSizeY{ 
get {
int _IconSizeY = default(int);
 // Returns the icon height:
_IconSizeY = m_lIconSizeY;
return _IconSizeY;
}
set {
 // Sets the icon height.  NB no change at runtime unless you
 // call Create and add all the images in again.
m_lIconSizeY = lSizeY;
}
}

  
  
  

  public int ImageCount{ 
get {
int _ImageCount = default(int);
 // Returns the number of images in the ImageList:
  if((hIml != 0)) {
  _ImageCount = ImageList_GetImageCount(hIml);
}
return _ImageCount;
}
}

public void RemoveImage(dynamic vKey) {
int lIndex = 0;
int i = 0;
 // Removes an image from the ImageList:
  if((hIml != 0)) {
  lIndex = ItemIndex(vKey);
  ImageList_Remove(hIml, lIndex);
   // Fix up the keys:
    for (i = lIndex; i <= ImageCount - 1; i += 1) {
    m_sKey(i) = m_sKey(i + 1);
  }
  pEnsureKeys();
}

}
public bool KeyExists{ 
get {
bool _KeyExists = default(bool);
im(iL As Long);
int iU = 0;
  if(ImageCount > 0) {
  // TODO: (NOT SUPPORTED): On Error Resume Next
  iU = m_sKey.Count;
    if(Err().Number != 0) {
    iU = 0;
  }
    if((iU != ImageCount - 1)) {
    pEnsureKeys();
  }
    for (iL = 0; iL <= ImageCount - 1; iL += 1) {
      if(m_sKey(iL) == sKey) {
      _KeyExists = true;
      break;
    }
  }
}
return _KeyExists;
}
}


public int ItemIndex{ 
get {
int _ItemIndex = default(int);
im(lR As Long);
int i = 0;
 // Returns the 0 based Index for the selected
 // Image list item:
  if((IsNumeric(vKey))) {
  lR = vKey;
    if((lR > 0) && (lR <= ImageCount)) {
    _ItemIndex = lR - 1;
    } else {
     // error
    Err.Raise(9, App.EXEName + ".vbalImageList");
    _ItemIndex = -1;
  }
  } else {
  lR = -1;
    for (i = 0; i <= ImageCount - 1; i += 1) {
      if((m_sKey(i) == vKey)) {
      lR = i;
      break;
    }
  }
    if((lR > 0) && (lR <= ImageCount)) {
    _ItemIndex = lR;
    } else {
    Err.Raise(9, App.EXEName + ".vbalImageList");
    _ItemIndex = -1;
  }
}
return _ItemIndex;
}
}

public dynamic ItemKey{ 
get {
dynamic _ItemKey = default(dynamic);
 // Returns the Key for an image:
  if((iIndex > 0) && (iIndex <= ImageCount)) {
  _ItemKey = m_sKey(iIndex - 1);
  } else {
  Err.Raise(9, App.EXEName + ".vbalImageList");
}
return _ItemKey;
}
set {
 // Sets the Key for the an image:
iIndex = iIndex - 1;
  if((iIndex > 0) && (iIndex < ImageCount)) {
  SetKey(iIndex, vKey);
  } else {
  Err.Raise(9, App.EXEName + ".vbalImageList");
}
}
}


public IPicture ItemPicture{ 
get {
IPicture _ItemPicture = default(IPicture);
im(lIndex As Long);
int hIcon = 0;
 // Returns a StdPicture for an image in the ImageList:
lIndex = ItemIndex(vKey);
  if((lIndex > -1)) {
  hIcon = ImageList_GetIcon(m_hIml, lIndex, ILD_TRANSPARENT);
    if((hIcon != 0)) {
    _ItemPicture = IconToPicture(hIcon);
     // Don't destroy the icon - it is now owned by
     // the picture object
  }
}

return _ItemPicture;
}
}

public int ItemCopyOfIcon{ 
get {
int _ItemCopyOfIcon = default(int);
im(lIndex As Long);
 // Returns a hIcon for an image in the ImageList.  User must
 // call DestroyIcon on the returned handle.
lIndex = ItemIndex(vKey);
  if((lIndex > -1)) {
  _ItemCopyOfIcon = ImageList_GetIcon(m_hIml, lIndex, ILD_TRANSPARENT);
}
return _ItemCopyOfIcon;
}
}

public void Clear() {
 // Recreates the image list.
Create();
}
public int AddFromFile(string sFIleName, ImageTypes iType, dynamic vKey, bool bMapSysColors = false, OLE_COLOR lBackColor = -1, dynamic vKeyAfter) {
int _AddFromFile = 0;
int hImage = 0;
int un2 = 0;
int lR = 0;

 // Adds an image or series of images from a file:
if((hIml != 0)) {
un2 = LR_LOADFROMFILE;
 // Load the image from file:
if(bMapSysColors) {
un2 = un2 || LR_LOADMAP3DCOLORS;
}
hImage = LoadImage(App.hInstance, sFIleName, iType, 0, 0, un2);
_AddFromFile = AddFromHandle(hImage, iType, vKey, lBackColor, vKeyAfter);
switch(iType) {
case IMAGE_ICON: 
DestroyIcon(hImage);
break;
case IMAGE_CURSOR: 
DestroyCursor(hImage);
break;
case IMAGE_BITMAP: 
DeleteObject(hImage);
break;
}
} else {
 // no image list...
_AddFromFile = false;
}

return _AddFromFile;
}
public int AddFromResourceID(int lID, int hInst, ImageTypes iType, dynamic vKey, bool bMapSysColors = false, OLE_COLOR lBackColor = -1, dynamic vKeyAfter) {
int _AddFromResourceID = 0;
int hImage = 0;
int un2 = 0;
int lR = 0;
int iX = 0;
int iY = 0;

 // Adds an image or series of images from a resource id.  Note this will
 // only work when working on a resource in a compiled executable:
if((hIml != 0)) {
 // Load the image from file:
if(bMapSysColors) {
un2 = un2 || LR_LOADMAP3DCOLORS;
}
 // Choose the icon closest to the image list size:
if(iType != IMAGE_BITMAP) {
iX = m_lIconSizeX;
iY = m_lIconSizeY;
}
if(hInst == 0) {
 // Assume we're trying to pick a shared
 // resource
un2 = un2 || LR_COPYRETURNORG;
}
hImage = LoadImageLong(hInst, lID, iType, iX, iY, un2);
_AddFromResourceID = AddFromHandle(hImage, iType, vKey, lBackColor, vKeyAfter);
switch(iType) {
case IMAGE_ICON: 
DestroyIcon(hImage);
break;
case IMAGE_CURSOR: 
DestroyCursor(hImage);
break;
case IMAGE_BITMAP: 
DeleteObject(hImage);
break;
}
} else {
 // no image list...
_AddFromResourceID = false;
}

return _AddFromResourceID;
}

public bool AddFromHandle(int hImage, ImageTypes iType, dynamic vKey, OLE_COLOR lBackColor = -1, dynamic vKeyAfter) {
bool _AddFromHandle = false;
int lR = 0;
int lDst = 0;
bool bOk = false;
bool bInsert = false;
int i = 0;
int j = 0;
int iOrigCount = 0;
int iCount = 0;
string sSwapKey = "";

 // Adds an image or series of images from a GDI image handle.
if((m_hIml != 0)) {
if((hImage != 0)) {
iOrigCount = ImageCount;

bOk = true;
if(! IsMissing(vKeyAfter)) {
if((ImageCount > 0)) {
if(vKeyAfter == 0) {
bInsert = false;
lDst = 0;
} else {
bInsert = true;
bOk = false;
lDst = ItemIndex(vKeyAfter);
if((lDst > -1)) {
bOk = true;
}
}
}
}

if(((bOk))) {
if((iType == IMAGE_BITMAP)) {
 // And add it to the image list:
if((lBackColor == -1)) {
 // Ideally Determine the top left pixel of the
 // bitmap and use as back colour...
int lHDCDisp = 0;
int lHDC = 0;
int hBmpOld = 0;
lHDCDisp = CreateDCAsNull("DISPLAY", ByVal 0&, ByVal 0&, ByVal 0&);
if(lHDCDisp != 0) {
lHDC = CreateCompatibleDC(lHDCDisp);
DeleteDC(lHDCDisp);
if(lHDC != 0) {
hBmpOld = SelectObject(lHDC, hImage);
if(hBmpOld != 0) {
 // Get the colour of the 0,0 pixel:
lBackColor = GetPixel(lHDC, 0, 0);
SelectObject(lHDC, hBmpOld);
}
DeleteObject(lHDC);
}
}
}
lR = ImageList_AddMasked(hIml, hImage, lBackColor);
} else if((iType == IMAGE_ICON) || (iType == IMAGE_CURSOR)) {
 // Add the icon:
lR = ImageList_AddIcon(hIml, hImage);
}
}

if((lR > -1)) {
if(((bInsert))) {
if((lDst < ImageCount - 1)) {
 // We are inserting and have to swap all
 // the images.
pEnsureKeys();
iCount = ImageCount;
for (i = iOrigCount - 1; i <= -1; i += lDst) {
for (j = i; j <= i + iCount - iOrigCount - 1; j += 1) {
ImageList_Copy(m_hIml, j + 1, m_hIml, j, eilSwap);
sSwapKey = m_sKey(j);
m_sKey(j) = m_sKey(j + 1);
m_sKey(j + 1) = sSwapKey;
}
}

}
}
}

} else {
lR = -1;
}
} else {
lR = -1;
}

if((lR != -1)) {
if(bInsert) {
SetKey(lDst, vKey);
} else {
SetKey(lR, vKey);
}
_AddFromHandle = (lR != -1);
}
pEnsureKeys();

return _AddFromHandle;
}
public int AddFromPictureBox(int hdc, ref dynamic pic, dynamic vKey, int LeftPixels = 0, int TopPixels = 0, OLE_COLOR lBackColor = -1) {
int _AddFromPictureBox = 0;
int lHDC = 0;
int lhBmp = 0;
int lhBmpOld = 0;
BITMAP tBM = null;
int lAColor = 0;
int lW = 0;
int lH = 0;
int hBrush = 0;
RECT tR = null;
int lR = 0;
int lBPixel = 0;

 // Adds an image or series of images from an area of a PictureBox
 // or other Device Context:
lR = -1;
if((hIml != 0)) {
 // Create a DC to hold the bitmap to transfer into the image list:
lHDC = CreateCompatibleDC(hdc);
if((lHDC != 0)) {
lhBmp = CreateCompatibleBitmap(hdc, m_lIconSizeX, m_lIconSizeY);
if((lhBmp != 0)) {
 // Get the backcolor to use:
if((lBackColor == -1)) {
 // None specified, use the colour at 0,0:
lBackColor = GetPixel(pic.hdc, 0, 0);
} else {
 // Try to get the specified backcolor:
if(OleTranslateColor(lBackColor, 0, lAColor)) {
 // Failed- use default of silver
lBackColor = 0xC0C0C0;
} else {
 // Set to GDI version of OLE Color
lBackColor = lAColor;
}
}
 // Select the bitmap into the DC
lhBmpOld = SelectObject(lHDC, lhBmp);
 // Clear the background:
hBrush = CreateSolidBrush(lBackColor);
tR.Right = m_lIconSizeX;tR.Bottom = m_lIconSizeY;
FillRect(lHDC, tR, hBrush);
DeleteObject(hBrush);

 // Get the source picture's dimension:
GetObjectAPI(pic.Picture.Handle, LenB(tBM), tBM);
lW = 16;
lH = 16;
if((lW + LeftPixels > tBM.bmWidth)) {
lW = tBM.bmWidth - LeftPixels;
}
if((lH + TopPixels > tBM.bmHeight)) {
lH = tBM.bmHeight - TopPixels;
}
if((lW > 0) && (lH > 0)) {
 // Blt from the picture into the bitmap:
lR = BitBlt(lHDC, 0, 0, lW, lH, hdc, LeftPixels, TopPixels, SRCCOPY);
Debug.Assert((lR != 0));
}

 // We now have the image in the bitmap, so select it out of the DC:
SelectObject(lHDC, lhBmpOld);
 // And add it to the image list:
AddFromHandle(lhBmp, IMAGE_BITMAP, vKey, lBackColor);

DeleteObject(lhBmp);
}
 // Clear up the DC:
DeleteDC(lHDC);
}
}

if((lR != -1)) {
SetKey(lR, vKey);
}

_AddFromPictureBox = lR + 1;
pEnsureKeys();

return _AddFromPictureBox;
}
private void SetKey(int lIndex, dynamic vKey) {
string sKey = "";
int lI = 0;

if((IsEmpty(vKey) || IsMissing(vKey))) {
sKey = "";
} else {
sKey = vKey;
}

if((m_hIml != 0)) {

// TODO: (NOT SUPPORTED): On Error Resume Next
lI = m_sKey.Count;
if((Err().Number == 0)) {
if((lIndex > lI)) {
// TODO: (NOT SUPPORTED): ReDim Preserve m_sKey(0 To lIndex) As String
}
} else {
// TODO: (NOT SUPPORTED): ReDim Preserve m_sKey(0 To lIndex) As String
}

for (lI = 0; lI <= m_sKey.Count; lI += 1) {
if(! lI == lIndex) {
if(Trim$(m_sKey(lI)) != "") {
if(m_sKey(lI) == vKey) {
Err.Raise(457);
return;
}
}
}
}
m_sKey(lIndex) = vKey;
}
}
public int hIml{ 
get {
int _hIml = default(int);
 // Returns the ImageList handle:
_hIml = m_hIml;
return _hIml;
}
}

public IPicture ImagePictureStrip{ 
get {
IPicture _ImagePictureStrip = default(IPicture);
im(iStart As Long);
int iEnd = 0;
int iImgIndex = 0;
int lHDC = 0;
int lParenthDC = 0;
int lhBmp = 0;
int lhBmpOld = 0;
int lSizeX = 0;
int hBr = 0;
RECT tR = null;
int lColor = 0;

  if((m_hIml != 0)) {
    if((IsMissing(vStartKey))) {
    iStart = 0;
    } else {
    iStart = ItemIndex(vStartKey);
  }
    if((IsMissing(vEndKey))) {
    iEnd = ImageCount - 1;
    } else {
    iEnd = ItemIndex(vEndKey);
  }
  
    if((iEnd > iStart) && (iEnd > -1)) {
    lParenthDC = m_HDC;
    lHDC = CreateCompatibleDC(lParenthDC);
      if((lHDC != 0)) {
      lSizeX = ImageCount * m_lIconSizeX;
      lhBmp = CreateCompatibleBitmap(lParenthDC, lSizeX, m_lIconSizeY);
        if((lhBmp != 0)) {
        lhBmpOld = SelectObject(lHDC, lhBmp);
          if((lhBmpOld != 0)) {
          lColor = TranslateColor(oBackColor);
          tR.Bottom = m_lIconSizeY;
          tR.Right = lSizeX;
          hBr = CreateSolidBrush(lColor);
          FillRect(lHDC, tR, hBr);
          DeleteObject(hBr);
            for (iImgIndex = iStart; iImgIndex <= iEnd; iImgIndex += 1) {
            ImageList_Draw(m_hIml, iImgIndex, lHDC, iImgIndex * m_lIconSizeX, 0, ILD_TRANSPARENT);
          }
          SelectObject(lHDC, lhBmpOld);
          _ImagePictureStrip = BitmapToPicture(lhBmp);
          } else {
          DeleteObject(lhBmp);
        }
      }
      DeleteDC(lHDC);
    }
  }
}

return _ImagePictureStrip;
}
}


public IPicture IconToPicture(int hIcon) {
IPicture _IconToPicture = null;

if(hIcon == 0)return _IconToPicture;

 // This is all magic if you ask me:
Picture NewPic = null;
PictDesc PicConv = null;
Guid IGuid = null;

PicConv.cbSizeofStruct = Len(PicConv);
PicConv.picType = vbPicTypeIcon;
PicConv.hImage = hIcon;

 // Fill in magic IPicture GUID {7BF80980-BF32-101A-8BBB-00AA00300CAB}
// TODO: (NOT SUPPORTED): With IGuid
.Data1 = 0x7BF80980;
.Data2 = 0xBF32;
.Data3 = 0x101A;
Data4(0) = 0x8B;
Data4(1) = 0xBB;
Data4(2) = 0x0;
Data4(3) = 0xAA;
Data4(4) = 0x0;
Data4(5) = 0x30;
Data4(6) = 0xC;
Data4(7) = 0xAB;
// TODO: (NOT SUPPORTED): End With
OleCreatePictureIndirect(PicConv, IGuid, true, NewPic);

_IconToPicture = NewPic;

return _IconToPicture;
}

public IPicture BitmapToPicture(int hBmp) {
IPicture _BitmapToPicture = null;

if((hBmp == 0))return _BitmapToPicture;

Picture NewPic = null;
PictDesc tPicConv = null;
Guid IGuid = null;

 // Fill PictDesc structure with necessary parts:
// TODO: (NOT SUPPORTED): With tPicConv
.cbSizeofStruct = Len(tPicConv);
.picType = vbPicTypeBitmap;
.hImage = hBmp;
// TODO: (NOT SUPPORTED): End With

 // Fill in IDispatch Interface ID
// TODO: (NOT SUPPORTED): With IGuid
.Data1 = 0x20400;
Data4(0) = 0xC0;
Data4(7) = 0x46;
// TODO: (NOT SUPPORTED): End With

 // Create a picture object:
OleCreatePictureIndirect(tPicConv, IGuid, true, NewPic);

 // Return it:
_BitmapToPicture = NewPic;


return _BitmapToPicture;
}

public int TranslateColor(OLE_COLOR clr, ref int hPal = 0) {
int _TranslateColor = 0;
if(OleTranslateColor(clr, hPal, _TranslateColor)) {
_TranslateColor = CLR_INVALID;
}
return _TranslateColor;
}
private void pEnsureKeys() {
int iCount = 0;
int iU = 0;
if(m_hIml != 0) {
iCount = ImageCount;
// TODO: (NOT SUPPORTED): On Error Resume Next
iU = m_sKey.Count;
if((Err().Number != 0))iU = -1;
// TODO: (NOT SUPPORTED): Err.Clear
if((iU != iCount - 1)) {
// TODO: (NOT SUPPORTED): ReDim Preserve m_sKey(0 To iCount - 1) As String
}
}
}

private void Class_Initialize() {
m_lIconSizeX = 16;
m_lIconSizeY = 16;
m_eColourDepth = ILC_COLOR;
}

private void Class_Terminate() {
Destroy();
}



}