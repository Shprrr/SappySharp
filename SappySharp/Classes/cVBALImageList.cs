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
using System.Diagnostics;
using stdole;

namespace SappySharp.Classes;

public partial class cVBALImageList : IDisposable
{
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
    [LibraryImport("user32")]
    private static partial int GetWindowWord(int hwnd, int nIndex);
    private const int GWW_HINSTANCE = -6;

    // GDI object functions:
    [LibraryImport("gdi32")]
    private static partial int SelectObject(int hdc, int hObject);
    [LibraryImport("gdi32")]
    private static partial int DeleteObject(int hObject);
    [LibraryImport("user32")]
    private static partial int DestroyCursor(int hCursor);
    [DllImport("gdi32", EntryPoint = "GetObjectA")]
    private static extern int GetObjectAPI(int hObject, int nCount, ref dynamic lpObject);
    [LibraryImport("user32")]
    private static partial int GetDC(int hwnd);
    [LibraryImport("gdi32")]
    private static partial int DeleteDC(int hdc);
    [LibraryImport("user32")]
    private static partial int ReleaseDC(int hwnd, int hdc);
    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleDC(int hdc);
    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);
    [LibraryImport("gdi32")]
    private static partial int GetDeviceCaps(int hdc, int nIndex);
    private const int BITSPIXEL = 12;
    private const int LOGPIXELSX = 88; // Logical pixels/inch in X
    private const int LOGPIXELSY = 90; // Logical pixels/inch in Y
    // System metrics:
    [LibraryImport("user32")]
    private static partial int GetSystemMetrics(int nIndex);
    private const int SM_CXICON = 11;
    private const int SM_CYICON = 12;
    private const int SM_CXFRAME = 32;
    private const int SM_CYCAPTION = 4;
    private const int SM_CYFRAME = 33;
    private const int SM_CYBORDER = 6;
    private const int SM_CXBORDER = 5;

    // Region paint and fill functions:
    [LibraryImport("gdi32")]
    private static partial int PaintRgn(int hdc, int hRgn);
    [LibraryImport("gdi32")]
    private static partial int ExtFloodFill(int hdc, int x, int y, int crColor, int wFillType);
    private const int FLOODFILLBORDER = 0;
    private const int FLOODFILLSURFACE = 1;
    [LibraryImport("gdi32")]
    private static partial int GetPixel(int hdc, int x, int y);

    // Pen functions:
    [LibraryImport("gdi32")]
    private static partial int CreatePen(int nPenStyle, int nWidth, int crColor);
    private const int PS_DASH = 1;
    private const int PS_DASHDOT = 3;
    private const int PS_DASHDOTDOT = 4;
    private const int PS_DOT = 2;
    private const int PS_SOLID = 0;
    private const int PS_NULL = 5;

    // Brush functions:
    [LibraryImport("gdi32")]
    private static partial int CreateSolidBrush(int crColor);
    [LibraryImport("gdi32")]
    private static partial int CreatePatternBrush(int hBitmap);

    // Line functions:
    [LibraryImport("gdi32")]
    private static partial int LineTo(int hdc, int x, int y);
    class POINTAPI
    {
        public int x;
        public int y;
    }
    [DllImport("gdi32")]
    private static extern int MoveToEx(int hdc, int x, int y, ref POINTAPI lpPoint);

    // Colour functions:
    [LibraryImport("gdi32")]
    private static partial int SetTextColor(int hdc, int crColor);
    [LibraryImport("gdi32")]
    private static partial int SetBkColor(int hdc, int crColor);
    [LibraryImport("gdi32")]
    private static partial int SetBkMode(int hdc, int nBkMode);
    private const int OPAQUE = 2;
    private const int TRANSPARENT = 1;
    [LibraryImport("user32")]
    private static partial int GetSysColor(int nIndex);
    private const int COLOR_ACTIVEBORDER = 10;
    private const int COLOR_ACTIVECAPTION = 2;
    private const int COLOR_ADJ_MAX = 100;
    private const int COLOR_ADJ_MIN = -100;
    private const int COLOR_APPWORKSPACE = 12;
    private const int COLOR_BACKGROUND = 1;
    private const int COLOR_BTNFACE = 15;
    private const int COLOR_BTNHIGHLIGHT = 20;
    private const int COLOR_BTNSHADOW = 16;
    private const int COLOR_BTNTEXT = 18;
    private const int COLOR_CAPTIONTEXT = 9;
    private const int COLOR_GRAYTEXT = 17;
    private const int COLOR_HIGHLIGHT = 13;
    private const int COLOR_HIGHLIGHTTEXT = 14;
    private const int COLOR_INACTIVEBORDER = 11;
    private const int COLOR_INACTIVECAPTION = 3;
    private const int COLOR_INACTIVECAPTIONTEXT = 19;
    private const int COLOR_MENU = 4;
    private const int COLOR_MENUTEXT = 7;
    private const int COLOR_SCROLLBAR = 0;
    private const int COLOR_WINDOW = 5;
    private const int COLOR_WINDOWFRAME = 6;
    private const int COLOR_WINDOWTEXT = 8;
    private const int COLORONCOLOR = 3;

    // Shell Extract icon functions:
    [DllImport("shell32.dll", EntryPoint = "FindExecutableA")]
    private static extern int FindExecutable(string lpFile, string lpDirectory, string lpResult);
    [DllImport("shell32.dll", EntryPoint = "ExtractIconA")]
    private static extern int ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

    // Icon functions:
    [LibraryImport("user32")]
    private static partial int DrawIcon(int hdc, int x, int y, int hIcon);
    [LibraryImport("user32")]
    private static partial int DestroyIcon(int hIcon);
    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DrawIconEx(int hdc, int xLeft, int yTop, int hIcon, int cxWidth, int cyWidth, int istepIfAniCur, int hbrFlickerFreeDraw, int diFlags);
    [DllImport("user32", EntryPoint = "LoadImageA")]
    private static extern int LoadImage(int hInst, string lpsz, int un1, int n1, int n2, int un2);
    [LibraryImport("user32", EntryPoint = "LoadImageA")]
    private static partial int LoadImageLong(int hInst, int lpsz, int un1, int n1, int n2, int un2);
    private const int LR_LOADMAP3DCOLORS = 0x1000;
    private const int LR_LOADFROMFILE = 0x10;
    private const int LR_LOADTRANSPARENT = 0x20;
    private const int LR_COPYRETURNORG = 0x4;

    // Blitting functions
    [LibraryImport("gdi32")]
    private static partial int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
    private const int SRCAND = 0x8800C6;
    private const int SRCCOPY = 0xCC0020;
    private const int SRCERASE = 0x440328;
    private const int SRCINVERT = 0x660046;
    private const int SRCPAINT = 0xEE0086;
    private const int BLACKNESS = 0x42;
    private const int WHITENESS = 0xFF0062;
    [LibraryImport("gdi32")]
    private static partial int PatBlt(int hdc, int x, int y, int nWidth, int nHeight, int dwRop);
    [LibraryImport("user32", EntryPoint = "LoadBitmapA")]
    private static partial int LoadBitmapBynum(int hInstance, int lpBitmapName);
    class BITMAP // 14 bytes
    {
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
    class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }
    [DllImport("user32")]
    private static extern int PtInRect(ref RECT lpRect, int ptX, int ptY);
    [DllImport("user32", EntryPoint = "DrawTextA")]
    private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
    private const int DT_BOTTOM = 0x8;
    private const int DT_CENTER = 0x1;
    private const int DT_LEFT = 0x0;
    private const int DT_CALCRECT = 0x400;
    private const int DT_WORDBREAK = 0x10;
    private const int DT_VCENTER = 0x4;
    private const int DT_TOP = 0x0;
    private const int DT_TABSTOP = 0x80;
    private const int DT_SINGLELINE = 0x20;
    private const int DT_RIGHT = 0x2;
    private const int DT_NOCLIP = 0x100;
    private const int DT_INTERNAL = 0x1000;
    private const int DT_EXTERNALLEADING = 0x200;
    private const int DT_EXPANDTABS = 0x40;
    private const int DT_CHARSTREAM = 4;
    private const int DT_NOPREFIX = 0x800;
    class DRAWTEXTPARAMS
    {
        public int cbSize;
        public int iTabLength;
        public int iLeftMargin;
        public int iRightMargin;
        public int uiLengthDrawn;
    }
    [DllImport("user32", EntryPoint = "DrawTextExA")]
    private static extern int DrawTextEx(int hdc, string lpsz, int n, ref RECT lpRect, int un, ref DRAWTEXTPARAMS lpDrawTextParams);
    [DllImport("user32", EntryPoint = "DrawTextExA")]
    private static extern int DrawTextExAsNull(int hdc, string lpsz, int n, ref RECT lpRect, int un, int lpDrawTextParams);
    private const int DT_EDITCONTROL = 0x2000;
    private const int DT_PATH_ELLIPSIS = 0x4000;
    private const int DT_END_ELLIPSIS = 0x8000;
    private const int DT_MODIFYSTRING = 0x10000;
    private const int DT_RTLREADING = 0x20000;
    private const int DT_WORD_ELLIPSIS = 0x40000;

    class SIZEAPI
    {
        public int cX;
        public int cY;
    }
    [DllImport("gdi32", EntryPoint = "GetTextExtentPoint32A")]
    private static extern int GetTextExtentPoint32(int hdc, string lpsz, int cbString, ref SIZEAPI lpSize);
    [LibraryImport("gdi32")]
    private static partial int GetStockObject(int nIndex);
    private const int ANSI_FIXED_FONT = 11;
    private const int ANSI_VAR_FONT = 12;
    private const int SYSTEM_FONT = 13;
    private const int DEFAULT_GUI_FONT = 17; // win95 only
    [DllImport("user32")]
    private static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);
    [LibraryImport("gdi32")]
    private static partial int Rectangle(int hdc, int X1, int y1, int x2, int y2);
    [DllImport("user32")]
    private static extern int DrawEdge(int hdc, ref RECT qrc, int edge, int grfFlags);
    private const int BF_LEFT = 1;
    private const int BF_TOP = 2;
    private const int BF_RIGHT = 4;
    private const int BF_BOTTOM = 8;
    private const int BF_RECT = BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM;
    private const int BF_MIDDLE = 2048;
    private const int BDR_SUNKENINNER = 8;
    private const int BDR_SUNKENOUTER = 2;
    private const int BDR_RAISEDOUTER = 1;
    private const int BDR_RAISEDINNER = 4;

    [LibraryImport("user32")]
    private static partial int ShowWindow(int hwnd, int nCmdShow);
    private const int SW_SHOWNOACTIVATE = 4;

    // Scrolling and region functions:
    [DllImport("user32")]
    private static extern int ScrollDC(int hdc, int dx, int dy, ref RECT lprcScroll, ref RECT lprcClip, int hrgnUpdate, ref RECT lprcUpdate);
    [LibraryImport("user32")]
    private static partial int SetWindowRgn(int hwnd, int hRgn, int bRedraw);
    [LibraryImport("gdi32")]
    private static partial int SelectClipRgn(int hdc, int hRgn);
    [LibraryImport("gdi32")]
    private static partial int CreateRectRgn(int X1, int y1, int x2, int y2);
    [DllImport("gdi32")]
    private static extern int CreateRectRgnIndirect(ref RECT lpRect);
    [DllImport("gdi32")]
    private static extern int CreatePolyPolygonRgn(ref POINTAPI lpPoint, ref int lpPolyCounts, int nCount, int nPolyFillMode);
    [DllImport("gdi32")]
    private static extern void CreatePolygonRgn(ref POINTAPI lpPoint, int nCount, int nPolyFillMode);
    [LibraryImport("gdi32")]
    private static partial int SaveDC(int hdc);
    [LibraryImport("gdi32")]
    private static partial int RestoreDC(int hdc, int hSavedDC);
    [DllImport("gdi32", EntryPoint = "CreateDCA")]
    private static extern int CreateDCAsNull(string lpDriverName, int lpDeviceName, int lpOutput, int lpInitData);

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
    private const int FW_NORMAL = 400;
    private const int FW_BOLD = 700;
    private const int FF_DONTCARE = 0;
    private const int DEFAULT_QUALITY = 0;
    private const int DEFAULT_PITCH = 0;
    private const int DEFAULT_CHARSET = 1;
    [DllImport("gdi32", EntryPoint = "CreateFontIndirectA")]
    private static extern int CreateFontIndirect(ref LOGFONT lpLogFont);
    [LibraryImport("kernel32")]
    private static partial int MulDiv(int nNumber, int nNumerator, int nDenominator);
    [DllImport("user32")]
    private static extern int DrawFocusRect(int hdc, ref RECT lpRect);

    [LibraryImport("user32", EntryPoint = "DrawStateA")]
    private static partial int DrawState(int hdc, int hBrush, int lpDrawStateProc, int lParam, int wParam, int x, int y, int cX, int cY, int fuFlags);

    // /* Image type */
    private const int DST_COMPLEX = 0x0;
    private const int DST_TEXT = 0x1;
    private const int DST_PREFIXTEXT = 0x2;
    private const int DST_ICON = 0x3;
    private const int DST_BITMAP = 0x4;

    // /* State type */
    private const int DSS_NORMAL = 0x0;
    private const int DSS_UNION = 0x10; // Dither
    private const int DSS_DISABLED = 0x20;
    private const int DSS_MONO = 0x80; // Draw in colour of brush specified in hBrush
    private const int DSS_RIGHT = 0x8000;

    [LibraryImport("olepro32.dll")]
    private static partial int OleTranslateColor(int OLE_COLOR, int HPALETTE, ref int pccolorref);
    private const int CLR_INVALID = -1;

    // Image list functions:
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_GetBkColor(int hImageList);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_ReplaceIcon(int hImageList, int i, int hIcon);
    [LibraryImport("COMCTL32", EntryPoint = "ImageList_Draw")]
    private static partial int ImageList_Convert(int hImageList, int ImgIndex, int hDCDest, int x, int y, int flags);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Create(int MinCx, int MinCy, int flags, int cInitial, int cGrow);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_AddMasked(int hImageList, int hbmImage, int crMask);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Replace(int hImageList, int ImgIndex, int hbmImage, int hBmMask);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Add(int hImageList, int hbmImage, ref int hBmMask);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Remove(int hImageList, int ImgIndex);
    class IMAGEINFO
    {
        public int hBitmapImage;
        public int hBitmapMask;
        public int cPlanes;
        public int cBitsPerPixel;
        public RECT rcImage;
    }
    [DllImport("COMCTL32.DLL")]
    private static extern int ImageList_GetImageInfo(int hIml, int i, ref IMAGEINFO pImageInfo);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_AddIcon(int hIml, int hIcon);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_GetIcon(int hImageList, int ImgIndex, int fuFlags);
    [LibraryImport("COMCTL32")]
    private static partial void ImageList_SetImageCount(int hImageList, ref int uNewCount);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_GetImageCount(int hImageList);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Destroy(int hImageList);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_GetIconSize(int hImageList, ref int cX, ref int cY);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_SetIconSize(int hImageList, ref int cX, ref int cY);

    // ImageList functions:
    // Draw:
    [LibraryImport("COMCTL32.DLL")]
    private static partial int ImageList_Draw(int hIml, int i, int hdcDst, int x, int y, int fStyle);
    private const int ILD_NORMAL = 0;
    private const int ILD_TRANSPARENT = 1;
    private const int ILD_BLEND25 = 2;
    private const int ILD_SELECTED = 4;
    private const int ILD_FOCUS = 4;
    private const int ILD_MASK = 0x10;
    private const int ILD_IMAGE = 0x20;
    private const int ILD_ROP = 0x40;
    private const int ILD_OVERLAYMASK = 3840;
    [DllImport("COMCTL32.DLL")]
    private static extern int ImageList_GetImageRect(int hIml, int i, ref RECT prcImage);
    // Messages:
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_DrawEx(int hIml, int i, int hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, int fStyle);
    [DllImport("COMCTL32", EntryPoint = "ImageList_LoadImageA")]
    private static extern void ImageList_LoadImage(int hInst, string lpbmp, int cX, int cGrow, int crMask, int uType, int uFlags);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_SetBkColor(int hImageList, int clrBk);

    private const int ILC_MASK = 0x1;

    private const int CLR_DEFAULT = -16777216;
    private const int CLR_HILIGHT = -16777216;
    private const int CLR_NONE = -1;

    private const int ILCF_MOVE = 0x0;
    private const int ILCF_SWAP = 0x1;
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_Copy(int himlDst, int iDst, int himlSrc, int iSrc, int uFlags);

    [DllImport("kernel32", EntryPoint = "GetTempFileNameA")]
    private static extern int GetTempFileName(string lpszPath, string lpPrefixString, int wUnique, string lpTempFileName);
    [DllImport("kernel32", EntryPoint = "GetTempPathA")]
    private static extern int GetTempPath(int nBufferLength, string lpBuffer);
    int MAX_PATH = 260;
    [DllImport("kernel32", EntryPoint = "lstrlenA")]
    private static extern int lstrlen(string lpString);

    struct PictDesc
    {
        public int cbSizeofStruct;
        public int picType;
        public int hImage;
        public int xExt;
        public int yExt;
    }
    [DllImport("olepro32.dll")]
    private static extern int OleCreatePictureIndirect(ref PictDesc lpPictDesc, ref Guid riid, int fPictureOwnsHandle, ref IPicture ipic);

    // -----------
    // ENUMS
    // -----------
    public enum eilIconState
    {
        Normal = 0
    , Disabled = 1
    }

    public enum ImageTypes
    {
        IMAGE_BITMAP = 0
    , IMAGE_ICON = 1
    , IMAGE_CURSOR = 2
    }

    public enum eilColourDepth
    {
        ILC_COLOR = 0x0
    , ILC_COLOR4 = 0x4
    , ILC_COLOR8 = 0x8
    , ILC_COLOR16 = 0x10
    , ILC_COLOR24 = 0x18
    , ILC_COLOR32 = 0x20
    }

    public enum eilSwapTypes
    {
        eilCopy = ILCF_MOVE
    , eilSwap = ILCF_SWAP
    }

    // ------------------
    // Private variables:
    // ------------------
    int m_hIml = 0;
    int m_lIconSizeX = 0;
    int m_lIconSizeY = 0;
    eilColourDepth m_eColourDepth;
    string[] m_sKey = Array.Empty<string>();
    int m_HDC = 0;
    private bool _disposedValue;

    public int OwnerHDC
    {
        set => m_HDC = value;
    }

    public eilColourDepth SystemColourDepth
    {
        get
        {
            int lR;
            int lHDC = CreateDCAsNull("DISPLAY", 0, 0, 0);
            lR = GetDeviceCaps(lHDC, BITSPIXEL);
            DeleteDC(lHDC);
            return (eilColourDepth)lR;
        }
    }

    public void SwapOrCopyImage(dynamic vKeySrc, dynamic vKeyDst, eilSwapTypes eSwap = eilSwapTypes.eilSwap)
    {
        if (m_hIml != 0)
        {
            int lDst = ItemIndex(vKeySrc);
            if (lDst > -1)
            {
                int lSrc = ItemIndex(vKeyDst);
                if (lSrc > -1)
                {
                    ImageList_Copy(m_hIml, lDst, m_hIml, lSrc, (int)eSwap);
                    string sKeyDst = m_sKey[lDst];
                    string sKeySrc = m_sKey[lSrc];
                    m_sKey[lDst] = sKeySrc;
                    m_sKey[lSrc] = sKeyDst;
                }
            }
        }
    }

    public bool Create()
    {
        bool _Create = false;

        // Do we already have an image list?  Kill it if we have:
        Destroy();

        // Create the Imagelist:
        m_hIml = ImageList_Create(m_lIconSizeX, m_lIconSizeY, ILC_MASK | (int)m_eColourDepth, 4, 4);
        if (m_hIml != 0 && m_hIml != -1)
        {
            // Ok
            _Create = true;
        }
        else
        {
            m_hIml = 0;
        }

        return _Create;
    }
    public void Destroy()
    {
        // Kill the image list if we have one:
        if (hIml != 0)
        {
            ImageList_Destroy(hIml);
            m_hIml = 0;
        }
        Array.Clear(m_sKey);
    }
    public void DrawImage(dynamic vKey, int hdc, int xPixels, int yPixels, bool bSelected = false, bool bCut = false, bool bDisabled = false, int oCutDitherColour = vbWindowBackground, int hExternalIml = 0)
    {
        int lhIml;

        // Draw the image at 1 based index or key supplied in vKey.
        // on the hDC at xPixels,yPixels with the supplied options.
        // You can even draw an ImageList from another ImageList control
        // if you supply the handle to hExternalIml with this function.

        int iImgIndex = ItemIndex(vKey);
        if (iImgIndex > -1)
        {
            if (hExternalIml != 0)
            {
                lhIml = hExternalIml;
            }
            else
            {
                lhIml = hIml;
            }

            int lFlags = ILD_TRANSPARENT;
            if (bSelected || bCut)
            {
                lFlags |= ILD_SELECTED;
            }

            if (bCut)
            {
                // Draw dithered:
                int lColor = TranslateColor(oCutDitherColour);
                if (lColor == -1) lColor = GetSysColor(COLOR_WINDOW);
                ImageList_DrawEx(lhIml, iImgIndex, hdc, xPixels, yPixels, 0, 0, CLR_NONE, lColor, lFlags);
            }
            else if (bDisabled)
            {
                // extract a copy of the icon:
                int hIcon = ImageList_GetIcon(hIml, iImgIndex, 0);
                // Draw it disabled at x,y:
                DrawState(hdc, 0, 0, hIcon, 0, xPixels, yPixels, m_lIconSizeX, m_lIconSizeY, DST_ICON | DSS_DISABLED);
                // Clear up the icon:
                DestroyIcon(hIcon);
            }
            else
            {
                // Standard draw:
                ImageList_Draw(lhIml, iImgIndex, hdc, xPixels, yPixels, lFlags);
            }
        }
    }

    public int IconSizeX
    {
        // Returns the icon width
        get => m_lIconSizeX;
        // Sets the icon width.  NB no change at runtime unless you
        // call Create and add all the images in again.
        set => m_lIconSizeX = value;
    }

    public int IconSizeY
    {
        // Returns the icon height:
        get => m_lIconSizeY;
        // Sets the icon height.  NB no change at runtime unless you
        // call Create and add all the images in again.
        set => m_lIconSizeY = value;
    }

    public eilColourDepth ColourDepth
    {
        // Returns the ColourDepth:
        get => m_eColourDepth;
        // Sets the ColourDepth.  NB no change at runtime unless you
        // call Create and rebuild the image list.
        set => m_eColourDepth = value;
    }

    public int ImageCount
    {
        get
        {
            int _ImageCount = default;
            // Returns the number of images in the ImageList:
            if (hIml != 0)
            {
                _ImageCount = ImageList_GetImageCount(hIml);
            }
            return _ImageCount;
        }
    }

    public void RemoveImage(dynamic vKey)
    {
        // Removes an image from the ImageList:
        if (hIml != 0)
        {
            int lIndex = ItemIndex(vKey);
            ImageList_Remove(hIml, lIndex);
            // Fix up the keys:
            for (int i = lIndex; i <= ImageCount - 1; i += 1)
            {
                m_sKey[i] = m_sKey[i + 1];
            }
            pEnsureKeys();
        }
    }

    public bool KeyExists(string sKey)
    {
        bool _KeyExists = default;
        int iL;
        if (ImageCount > 0)
        {
            // TODO: (NOT SUPPORTED): On Error Resume Next
            int iU = m_sKey.Length;
            if (Err().Number != 0)
            {
                iU = 0;
            }
            if (iU != ImageCount - 1)
            {
                pEnsureKeys();
            }
            for (iL = 0; iL <= ImageCount - 1; iL += 1)
            {
                if (m_sKey[iL] == sKey)
                {
                    _KeyExists = true;
                    break;
                }
            }
        }
        return _KeyExists;
    }

    public int ItemIndex(dynamic vKey)
    {
        int _ItemIndex;
        int lR;
        // Returns the 0 based Index for the selected
        // Image list item:
        if (IsNumeric(vKey))
        {
            lR = vKey;
            if (lR > 0 && lR <= ImageCount)
            {
                _ItemIndex = lR - 1;
            }
            else
            {
                // error
                Err().Raise(9, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalImageList");
                _ItemIndex = -1;
            }
        }
        else
        {
            lR = -1;
            for (int i = 0; i <= ImageCount - 1; i += 1)
            {
                if (m_sKey[i] == vKey)
                {
                    lR = i;
                    break;
                }
            }
            if (lR > 0 && lR <= ImageCount)
            {
                _ItemIndex = lR;
            }
            else
            {
                Err().Raise(9, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalImageList");
                _ItemIndex = -1;
            }
        }
        return _ItemIndex;
    }

    public dynamic ItemKey(int iIndex)
    {
        dynamic _ItemKey = default;
        // Returns the Key for an image:
        if (iIndex > 0 && iIndex <= ImageCount)
        {
            _ItemKey = m_sKey[iIndex - 1];
        }
        else
        {
            Err().Raise(9, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalImageList");
        }
        return _ItemKey;
    }
    public void ItemKey(int iIndex, dynamic vKey)
    {
        // Sets the Key for the an image:
        iIndex--;
        if (iIndex > 0 && iIndex < ImageCount)
        {
            SetKey(iIndex, vKey);
        }
        else
        {
            Err().Raise(9, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalImageList");
        }
    }

    public IPicture ItemPicture(dynamic vKey)
    {
        IPicture _ItemPicture = default;
        int lIndex;
        // Returns a StdPicture for an image in the ImageList:
        lIndex = ItemIndex(vKey);
        if (lIndex > -1)
        {
            int hIcon = ImageList_GetIcon(m_hIml, lIndex, ILD_TRANSPARENT);
            if (hIcon != 0)
            {
                _ItemPicture = IconToPicture(hIcon);
                // Don't destroy the icon - it is now owned by
                // the picture object
            }
        }

        return _ItemPicture;
    }

    public int ItemCopyOfIcon(dynamic vKey)
    {
        int _ItemCopyOfIcon = default;
        int lIndex;
        // Returns a hIcon for an image in the ImageList.  User must
        // call DestroyIcon on the returned handle.
        lIndex = ItemIndex(vKey);
        if (lIndex > -1)
        {
            _ItemCopyOfIcon = ImageList_GetIcon(m_hIml, lIndex, ILD_TRANSPARENT);
        }
        return _ItemCopyOfIcon;
    }

    public void Clear()
    {
        // Recreates the image list.
        Create();
    }
    public int AddFromFile(string sFIleName, ImageTypes iType, dynamic vKey = null, bool bMapSysColors = false, int lBackColor = -1, dynamic vKeyAfter = null)
    {
        int _AddFromFile;

        // Adds an image or series of images from a file:
        if (hIml != 0)
        {
            int un2 = LR_LOADFROMFILE;
            // Load the image from file:
            if (bMapSysColors)
            {
                un2 |= LR_LOADMAP3DCOLORS;
            }
            int hImage = LoadImage((int)Marshal.GetHINSTANCE(typeof(App).Module), sFIleName, (int)iType, 0, 0, un2);
            _AddFromFile = AddFromHandle(hImage, iType, vKey, lBackColor, vKeyAfter);
            switch (iType)
            {
                case ImageTypes.IMAGE_ICON:
                    DestroyIcon(hImage);
                    break;
                case ImageTypes.IMAGE_CURSOR:
                    DestroyCursor(hImage);
                    break;
                case ImageTypes.IMAGE_BITMAP:
                    DeleteObject(hImage);
                    break;
            }
        }
        else
        {
            // no image list...
            _AddFromFile = 0;
        }

        return _AddFromFile;
    }
    public int AddFromResourceID(int lID, int hInst, ImageTypes iType, dynamic vKey = null, bool bMapSysColors = false, int lBackColor = -1, dynamic vKeyAfter = null)
    {
        int _AddFromResourceID;
        int un2 = 0;
        int iX = 0;
        int iY = 0;

        // Adds an image or series of images from a resource id.  Note this will
        // only work when working on a resource in a compiled executable:
        if (hIml != 0)
        {
            // Load the image from file:
            if (bMapSysColors)
            {
                un2 |= LR_LOADMAP3DCOLORS;
            }
            // Choose the icon closest to the image list size:
            if (iType != ImageTypes.IMAGE_BITMAP)
            {
                iX = m_lIconSizeX;
                iY = m_lIconSizeY;
            }
            if (hInst == 0)
            {
                // Assume we're trying to pick a shared
                // resource
                un2 |= LR_COPYRETURNORG;
            }
            int hImage = LoadImageLong(hInst, lID, (int)iType, iX, iY, un2);
            _AddFromResourceID = AddFromHandle(hImage, iType, vKey, lBackColor, vKeyAfter);
            switch (iType)
            {
                case ImageTypes.IMAGE_ICON:
                    DestroyIcon(hImage);
                    break;
                case ImageTypes.IMAGE_CURSOR:
                    DestroyCursor(hImage);
                    break;
                case ImageTypes.IMAGE_BITMAP:
                    DeleteObject(hImage);
                    break;
            }
        }
        else
        {
            // no image list...
            _AddFromResourceID = 0;
        }

        return _AddFromResourceID;
    }

    public bool AddFromHandle(int hImage, ImageTypes iType, dynamic vKey = null, int lBackColor = -1, dynamic vKeyAfter = null)
    {
        bool _AddFromHandle = false;
        int lR = 0;
        int lDst = 0;
        bool bInsert = false;

        // Adds an image or series of images from a GDI image handle.
        if (m_hIml != 0)
        {
            if (hImage != 0)
            {
                int iOrigCount = ImageCount;

                bool bOk = true;
                if (vKeyAfter != null)
                {
                    if (ImageCount > 0)
                    {
                        if (vKeyAfter == 0)
                        {
                            bInsert = false;
                            lDst = 0;
                        }
                        else
                        {
                            bInsert = true;
                            bOk = false;
                            lDst = ItemIndex(vKeyAfter);
                            if (lDst > -1)
                            {
                                bOk = true;
                            }
                        }
                    }
                }

                if (bOk)
                {
                    if (iType == ImageTypes.IMAGE_BITMAP)
                    {
                        // And add it to the image list:
                        if (lBackColor == -1)
                        {
                            // Ideally Determine the top left pixel of the
                            // bitmap and use as back colour...
                            int lHDCDisp = CreateDCAsNull("DISPLAY", 0, 0, 0);
                            if (lHDCDisp != 0)
                            {
                                int lHDC = CreateCompatibleDC(lHDCDisp);
                                DeleteDC(lHDCDisp);
                                if (lHDC != 0)
                                {
                                    int hBmpOld = SelectObject(lHDC, hImage);
                                    if (hBmpOld != 0)
                                    {
                                        // Get the colour of the 0,0 pixel:
                                        lBackColor = GetPixel(lHDC, 0, 0);
                                        SelectObject(lHDC, hBmpOld);
                                    }
                                    DeleteObject(lHDC);
                                }
                            }
                        }
                        lR = ImageList_AddMasked(hIml, hImage, lBackColor);
                    }
                    else if (iType == ImageTypes.IMAGE_ICON || iType == ImageTypes.IMAGE_CURSOR)
                    {
                        // Add the icon:
                        lR = ImageList_AddIcon(hIml, hImage);
                    }
                }

                if (lR > -1)
                {
                    if (bInsert)
                    {
                        if (lDst < ImageCount - 1)
                        {
                            // We are inserting and have to swap all
                            // the images.
                            pEnsureKeys();
                            int iCount = ImageCount;
                            for (int i = iOrigCount - 1; i >= lDst; i--)
                            {
                                for (int j = i; j <= i + iCount - iOrigCount - 1; j += 1)
                                {
                                    ImageList_Copy(m_hIml, j + 1, m_hIml, j, (int)eilSwapTypes.eilSwap);
                                    (m_sKey[j + 1], m_sKey[j]) = (m_sKey[j], m_sKey[j + 1]);
                                }
                            }

                        }
                    }
                }

            }
            else
            {
                lR = -1;
            }
        }
        else
        {
            lR = -1;
        }

        if (lR != -1)
        {
            if (bInsert)
            {
                SetKey(lDst, vKey);
            }
            else
            {
                SetKey(lR, vKey);
            }
            _AddFromHandle = lR != -1;
        }
        pEnsureKeys();

        return _AddFromHandle;
    }
    public int AddFromPictureBox(int hdc, ref dynamic pic, dynamic vKey, int LeftPixels = 0, int TopPixels = 0, int lBackColor = -1)
    {
        BITMAP tBM = null;
        int lAColor = 0;
        RECT tR = null;

        // Adds an image or series of images from an area of a PictureBox
        // or other Device Context:
        int lR = -1;
        if (hIml != 0)
        {
            // Create a DC to hold the bitmap to transfer into the image list:
            int lHDC = CreateCompatibleDC(hdc);
            if (lHDC != 0)
            {
                int lhBmp = CreateCompatibleBitmap(hdc, m_lIconSizeX, m_lIconSizeY);
                if (lhBmp != 0)
                {
                    // Get the backcolor to use:
                    if (lBackColor == -1)
                    {
                        // None specified, use the colour at 0,0:
                        lBackColor = GetPixel(pic.hdc, 0, 0);
                    }
                    else
                    {
                        // Try to get the specified backcolor:
                        if (OleTranslateColor(lBackColor, 0, ref lAColor) != 0)
                        {
                            // Failed- use default of silver
                            lBackColor = 0xC0C0C0;
                        }
                        else
                        {
                            // Set to GDI version of OLE Color
                            lBackColor = lAColor;
                        }
                    }
                    // Select the bitmap into the DC
                    int lhBmpOld = SelectObject(lHDC, lhBmp);
                    // Clear the background:
                    int hBrush = CreateSolidBrush(lBackColor);
                    tR.Right = m_lIconSizeX; tR.Bottom = m_lIconSizeY;
                    FillRect(lHDC, ref tR, hBrush);
                    DeleteObject(hBrush);

                    // Get the source picture's dimension:
                    dynamic lpObject = tBM;
                    GetObjectAPI(pic.Picture.Handle, Marshal.SizeOf(tBM), ref lpObject);
                    tBM = lpObject;
                    int lW = 16;
                    int lH = 16;
                    if (lW + LeftPixels > tBM.bmWidth)
                    {
                        lW = tBM.bmWidth - LeftPixels;
                    }
                    if (lH + TopPixels > tBM.bmHeight)
                    {
                        lH = tBM.bmHeight - TopPixels;
                    }
                    if (lW > 0 && lH > 0)
                    {
                        // Blt from the picture into the bitmap:
                        lR = BitBlt(lHDC, 0, 0, lW, lH, hdc, LeftPixels, TopPixels, SRCCOPY);
                        Debug.Assert(lR != 0);
                    }

                    // We now have the image in the bitmap, so select it out of the DC:
                    SelectObject(lHDC, lhBmpOld);
                    // And add it to the image list:
                    AddFromHandle(lhBmp, ImageTypes.IMAGE_BITMAP, vKey, lBackColor);

                    DeleteObject(lhBmp);
                }
                // Clear up the DC:
                DeleteDC(lHDC);
            }
        }

        if (lR != -1)
        {
            SetKey(lR, vKey);
        }

        int _AddFromPictureBox = lR + 1;
        pEnsureKeys();

        return _AddFromPictureBox;
    }
    private void SetKey(int lIndex, dynamic vKey)
    {
        string sKey;

        if (vKey == null)
        {
            sKey = "";
        }
        else
        {
            sKey = vKey;
        }

        if (m_hIml != 0)
        {
            // TODO: (NOT SUPPORTED): On Error Resume Next
            int lI = m_sKey.Length;
            if (Err().Number == 0)
            {
                if (lIndex + 1 > lI)
                {
                    Array.Resize(ref m_sKey, lIndex + 1);
                }
            }
            else
            {
                Array.Resize(ref m_sKey, lIndex + 1);
            }

            for (lI = 0; lI < m_sKey.Length; lI += 1)
            {
                if (lI != lIndex)
                {
                    if (Trim(m_sKey[lI]) != "")
                    {
                        if (m_sKey[lI] == vKey)
                        {
                            Err().Raise(457);
                            return;
                        }
                    }
                }
            }
            m_sKey[lIndex] = vKey;
        }
    }
    /// <summary>
    /// Returns the ImageList handle
    /// </summary>
    public int hIml => m_hIml;

    public IPicture ImagePictureStrip(dynamic vStartKey = null, dynamic vEndKey = null, int oBackColor = vbButtonFace)
    {
        IPicture _ImagePictureStrip = default;
        int iStart;
        int iEnd;
        RECT tR = null;

        if (m_hIml != 0)
        {
            if (vStartKey == null)
            {
                iStart = 0;
            }
            else
            {
                iStart = ItemIndex(vStartKey);
            }
            if (vEndKey == null)
            {
                iEnd = ImageCount - 1;
            }
            else
            {
                iEnd = ItemIndex(vEndKey);
            }

            if (iEnd > iStart && iEnd > -1)
            {
                int lParenthDC = m_HDC;
                int lHDC = CreateCompatibleDC(lParenthDC);
                if (lHDC != 0)
                {
                    int lSizeX = ImageCount * m_lIconSizeX;
                    int lhBmp = CreateCompatibleBitmap(lParenthDC, lSizeX, m_lIconSizeY);
                    if (lhBmp != 0)
                    {
                        int lhBmpOld = SelectObject(lHDC, lhBmp);
                        if (lhBmpOld != 0)
                        {
                            int lColor = TranslateColor(oBackColor);
                            tR.Bottom = m_lIconSizeY;
                            tR.Right = lSizeX;
                            int hBr = CreateSolidBrush(lColor);
                            FillRect(lHDC, ref tR, hBr);
                            DeleteObject(hBr);
                            for (int iImgIndex = iStart; iImgIndex <= iEnd; iImgIndex += 1)
                            {
                                ImageList_Draw(m_hIml, iImgIndex, lHDC, iImgIndex * m_lIconSizeX, 0, ILD_TRANSPARENT);
                            }
                            SelectObject(lHDC, lhBmpOld);
                            _ImagePictureStrip = BitmapToPicture(lhBmp);
                        }
                        else
                        {
                            DeleteObject(lhBmp);
                        }
                    }
                    DeleteDC(lHDC);
                }
            }
        }

        return _ImagePictureStrip;
    }

    public IPicture IconToPicture(int hIcon)
    {
        if (hIcon == 0) return null;

        // This is all magic if you ask me:
        IPicture NewPic = null;
        PictDesc PicConv = new();
        Guid IGuid = typeof(IPicture).GUID; // Fill in magic IPicture GUID {7BF80980-BF32-101A-8BBB-00AA00300CAB}

        PicConv.cbSizeofStruct = Len(PicConv);
        PicConv.picType = vbPicTypeIcon;
        PicConv.hImage = hIcon;

        OleCreatePictureIndirect(ref PicConv, ref IGuid, 1, ref NewPic);

        return NewPic;
    }

    public IPicture BitmapToPicture(int hBmp)
    {
        if (hBmp == 0) return null;

        IPicture NewPic = null;
        PictDesc tPicConv = new();
        Guid IGuid = new(0x20400, 0, 0, 0xC0, 0, 0, 0, 0, 0, 0, 0x46); // Fill in IDispatch Interface ID {00020400-0000-0000-C000-000000000046}

        // Fill PictDesc structure with necessary parts:
        tPicConv.cbSizeofStruct = Len(tPicConv);
        tPicConv.picType = vbPicTypeBitmap;
        tPicConv.hImage = hBmp;

        // Create a picture object:
        OleCreatePictureIndirect(ref tPicConv, ref IGuid, 1, ref NewPic);

        // Return it:
        return NewPic;
    }

    public int TranslateColor(int clr, int hPal = 0)
    {
        int _TranslateColor = 0;
        if (OleTranslateColor(clr, hPal, ref _TranslateColor) != 0)
        {
            _TranslateColor = CLR_INVALID;
        }
        return _TranslateColor;
    }
    private void pEnsureKeys()
    {
        if (m_hIml != 0)
        {
            int iCount = ImageCount;
            // TODO: (NOT SUPPORTED): On Error Resume Next
            int iU = m_sKey.Length;
            if (Err().Number != 0) iU = -1;
            Err().Clear();
            if (iU != iCount - 1)
            {
                Array.Resize(ref m_sKey, iCount - 1);
            }
        }
    }

    public cVBALImageList()
    {
        m_lIconSizeX = 16;
        m_lIconSizeY = 16;
        m_eColourDepth = eilColourDepth.ILC_COLOR;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Delete managed code.
            }

            // Delete non managed code.
            Destroy();
            _disposedValue = true;
        }
    }

    ~cVBALImageList()
    {
        // Don't change this. Change 'Dispose(bool disposing)'
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Don't change this. Change 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
