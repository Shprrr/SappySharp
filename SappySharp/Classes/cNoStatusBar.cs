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
using System.Drawing;
using Image = System.Windows.Controls.Image;
using SystemColors = System.Drawing.SystemColors;

namespace SappySharp.Classes;

public partial class cNoStatusBar : IDisposable
{
    // =========================================================================
    // vbAccelerator NoStatusbar class
    // Copyright © 1998-2002 Steve McMahon (steve@vbaccelerator.com)

    // This class draws a status bar onto a PictureBox, UserControl
    // or form.  Code derived from the vbAccelerator Status Bar
    // control, a full VB implementation of the COMCTL32.DLL Status Bar.

    // * Text and icons in panels
    // * Simple mode support
    // * Height calculation available
    // * Size gripper

    // Visit vbAccelerator at http://vbaccelerator.com
    // =========================================================================


    // =========================================================================
    // Declares, constants and types required for fake status bar:
    // =========================================================================
    class POINTAPI
    {
        public int x;
        public int y;
    }
    class RECT
    {
        public int left;
        public int tOp;
        public int Right;
        public int Bottom;
    }
    [DllImport("COMCTL32", EntryPoint = "DrawStatusTextA")]
    private static extern int DrawStatusText(int hdc, ref RECT lprc, string pszText, int uFlags);
    [LibraryImport("COMCTL32")]
    private static partial int ImageList_GetIcon(int hImageList, int ImgIndex, int fuFlags);
    [LibraryImport("COMCTL32.DLL")]
    private static partial int ImageList_GetImageCount(int hIml);
    [DllImport("COMCTL32.DLL")]
    private static extern int ImageList_GetImageRect(int hIml, int i, ref RECT prcImage);
    [LibraryImport("COMCTL32.DLL")]
    private static partial int ImageList_GetIconSize(int hIml, int cX, int cY);
    [LibraryImport("user32")]
    private static partial int DrawIconEx(int hdc, int xLeft, int yTop, int hIcon, int cxWidth, int cyWidth, int istepIfAniCur, int hbrFlickerFreeDraw, int diFlags);
    private const int DI_MASK = 0x1;
    private const int DI_IMAGE = 0x2;
    private const int DI_NORMAL = 0x3;
    private const int DI_COMPAT = 0x4;
    private const int DI_DEFAULTSIZE = 0x8;
    [LibraryImport("user32")]
    private static partial int DestroyIcon(int hIcon);
    [DllImport("user32", EntryPoint = "DrawTextA")]
    private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
    private const int DT_CALCRECT = 0x400;
    private const int DT_CENTER = 0x1;
    private const int DT_VCENTER = 0x4;
    private const int DT_SINGLELINE = 0x20;
    private const int DT_RIGHT = 0x2;
    private const int DT_BOTTOM = 0x8;
    private const int DT_WORD_ELLIPSIS = 0x40000;
    [DllImport("user32")]
    private static extern int GetClientRect(int hwnd, ref RECT lpRect);
    [DllImport("user32")]
    private static extern int OffsetRect(ref RECT lpRect, int x, int y);
    private const int SBT_NOBORDERS = 0x100;
    private const int SBT_POPOUT = 0x200;
    private const int SBT_RTLREADING = 0x400;
    private const int SBT_OWNERDRAW = 0x1000;
    [LibraryImport("user32")]
    private static partial int GetSysColorBrush(int nIndex);
    [DllImport("user32")]
    private static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);
    [LibraryImport("gdi32")]
    private static partial int DeleteObject(int hObject);
    private const int COLOR_BTNFACE = 15;
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(ref dynamic pDest, ref dynamic pSrc, int ByteLen);

    // XP DrawTheme declares for XP version
    [LibraryImport("kernel32")]
    private static partial int GetVersion();
    [DllImport("uxtheme.dll")]
    public static extern int OpenThemeData(int hWnd, string classList);
    [LibraryImport("uxtheme.dll")]
    private static partial int CloseThemeData(int hTheme);
    [DllImport("uxtheme.dll")]
    private static extern int DrawThemeBackground(int hTheme, int lHDC, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);
    [DllImport("uxtheme.dll")]
    private static extern int GetThemeBackgroundContentRect(int hTheme, int hdc, int iPartId, int iStateId, ref RECT pBoundingRect, ref RECT pContentRect);
    [DllImport("uxtheme.dll")]
    private static extern int DrawThemeText(int hTheme, int hdc, int iPartId, int iStateId, string text, int iCharCount, int dwTextFlag, int dwTextFlags2, ref RECT pRect);
    [DllImport("uxtheme.dll")]
    private static extern int DrawThemeIcon(int hTheme, int hdc, int iPartId, int iStateId, ref RECT pRect, int hIml, int iImageIndex);
    private const int S_OK = 0;

    // =========================================================================
    // Implementation of fake status bar:
    // =========================================================================
    public enum ENSBRPanelStyleConstants
    {
        estbrStandard = 0x0
    , estbrNoBorders = SBT_NOBORDERS
    , estbrRaisedBorder = SBT_POPOUT
    , estbrOwnerDraw = SBT_OWNERDRAW
    }
    class tStatusPanel
    {
        public int lID;
        public string sKey;
        public int lItemData;
        public int iImgIndex;
        public int hIcon;
        public string sText;
        public string sToolTipText;
        public int lMinWidth;
        public int lIdealWidth;
        public int lSetWidth;
        public bool bSpring;
        public bool bFit;
        public ENSBRPanelStyleConstants eStyle;
        public bool bState;
        public RECT tR;
    }
    List<tStatusPanel> m_tPanels = new();
    int m_iPanelCount = 0;
    bool m_bSizeGrip = false;
    int m_hIml = 0;
    int m_ptrVb6ImageList = 0;
    Image m_pic = null;
    int m_lIconSize = 0;
    dynamic m_obj = null;
    int m_lLeft = 0;
    int m_lTop = 0;
    int m_lHeight = 0;
    bool m_bSimpleMode = false;
    string m_sSimpleText = "";

    bool m_bIsXpOrAbove = false;
    bool m_bUseXpStyles = false;
    private bool _disposedValue;

    public delegate int OwnerDrawEventHandler(int hdc, int iLeft, int iTop, int iRight, int iBottom, bool bDoDefault);
    public event OwnerDrawEventHandler OwnerDraw;

    private void GetWindowsVersion(ref int lMajor, ref int lMinor, ref int lRevision, ref int lBuildNumber)
    {
        int lR = GetVersion();
        lBuildNumber = (lR & 0x7F000000) / 0x1000000;
        if ((lR & 0x80000000) != 0) lBuildNumber |= 0x80;
        lRevision = (lR & 0xFF0000) / 0x10000;
        lMinor = (lR & 0xFF00) / 0x100;
        lMajor = lR & 0xFF;
    }

    public bool SimpleMode
    {
        get => m_bSimpleMode;
        set
        {
            m_bSimpleMode = value;
            Draw();
        }
    }

    public string SimpleText
    {
        get => m_sSimpleText;
        set
        {
            m_sSimpleText = value;
            if (m_bSimpleMode)
            {
                Draw();
            }
        }
    }

    public bool AllowXPStyles
    {
        get => m_bIsXpOrAbove;
        set
        {
            if (value)
            {
                if (!m_bIsXpOrAbove)
                {
                    Err().Raise(vbObjectError + 1052, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "XP Styles not supported on this Windows installation.");
                }
                else
                {
                    m_bUseXpStyles = true;
                }
            }
            else
            {
                m_bUseXpStyles = false;
            }
        }
    }

    public bool SizeGrip
    {
        get => m_bSizeGrip;
        set
        {
            m_bSizeGrip = value;
            Draw();
        }
    }

    public int AddPanel(ENSBRPanelStyleConstants eStyle = ENSBRPanelStyleConstants.estbrStandard, string sText = "", int iImgIndex = -1, int lMinWidth = 64, bool bSpring = false, bool bFitContents = false, int lItemData = 0, string sKey = "", dynamic vKeyBefore = null)
    {
        int _AddPanel = 0;
        int iIndex = 0;
        int i = 0;

        if (m_iPanelCount >= 0xFF)
        {
            Err().Raise(vbObjectError + 1051, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "Too many panels.");
            return _AddPanel;
        }

        if (!IsMissing(vKeyBefore))
        {
            // Determine if vKeyBefore is valid:
            iIndex = PanelIndex(vKeyBefore);
            if (iIndex > 0)
            {
                // ok. Insert a space:
                m_iPanelCount++;
                // TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
                m_tPanels.Add(new());
                for (i = m_iPanelCount - 1; i >= iIndex + 1; i--)
                {
                    m_tPanels[i] = m_tPanels[i - 1];
                }
                m_tPanels[iIndex].hIcon = 0;
            }
            else
            {
                // Failed
                return _AddPanel;
            }
        }
        else
        {
            // Insert a space at the end:
            m_iPanelCount++;
            // TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
            m_tPanels.Add(new());
            iIndex = m_iPanelCount;
        }

        // Set up the info:
        if (bSpring)
        {
            for (i = 1; i <= m_iPanelCount; i += 1)
            {
                if (i != iIndex)
                {
                    m_tPanels[i].bSpring = false;
                }
            }
        }

        m_tPanels[iIndex].bFit = bFitContents;
        m_tPanels[iIndex].bSpring = bSpring;
        m_tPanels[iIndex].eStyle = eStyle;
        m_tPanels[iIndex].iImgIndex = iImgIndex;
        m_tPanels[iIndex].lMinWidth = lMinWidth;
        m_tPanels[iIndex].lItemData = lItemData;
        m_tPanels[iIndex].sKey = sKey;
        m_tPanels[iIndex].sText = sText;

        // Add the information to the status bar:
        pEvaluateIdealSize(iIndex);
        pResizeStatus();

        // Now ensure the text, style, tooltip and icon are actually correct:
        PanelText(iIndex, m_tPanels[iIndex].sText);
        PanelIcon(iIndex, m_tPanels[iIndex].iImgIndex);

        Draw();

        return _AddPanel;
    }

    public void Draw()
    {
        int i = 0;
        int lHDC = 0;
        int lX = 0;
        int lY = 0;
        int hBr = 0;
        RECT tR = null;
        RECT tOR = null;
        RECT tBR = null;
        Font fntThis = null;
        bool bEnd = false;
        int hTheme = 0;
        int hR = 0;
        RECT rcContent = null;
        bool bUseXpStyles = false;
        bool bDoDefault = false;
        bDoDefault = true;

        GetClientRect(m_obj.hwnd, ref tR);

        if (m_bUseXpStyles)
        {
            bUseXpStyles = true;
            hTheme = OpenThemeData(m_obj.hwnd, "Status");
            if (hTheme == 0)
            {
                bUseXpStyles = false;
            }
            else
            {
                // draw the background for the status bar:
                hR = DrawThemeBackground(hTheme, m_obj.hdc, 4, 0, ref tR, ref tR);
                if (hR != S_OK)
                {
                    bUseXpStyles = false;
                }
            }
        }

        if (!bUseXpStyles)
        {
            hBr = GetSysColorBrush(COLOR_BTNFACE);
            FillRect(m_obj.hdc, ref tR, hBr);
            DeleteObject(hBr);
        }

        tOR = tR;

        pResizeStatus();
        lHDC = m_obj.hdc;
        if (m_bSimpleMode)
        {
            if (bUseXpStyles)
            {
                hR = DrawThemeBackground(hTheme, m_obj.hdc, 2, 0, ref tR, ref tR);
                hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, 2, 0, ref tR, ref rcContent);
                hR = DrawThemeText(hTheme, m_obj.hdc, 2, 0, " " + m_sSimpleText, -1, DT_VCENTER | DT_SINGLELINE, 0, ref rcContent);
            }
            else
            {
                DrawText(lHDC, m_sSimpleText, -1, ref tR, DT_VCENTER | DT_SINGLELINE);
            }
        }
        else
        {
            int iPart = 0;
            for (i = 1; i <= m_iPanelCount; i += 1)
            {
                if (i == m_iPanelCount)
                {
                    iPart = 2;
                }
                else
                {
                    iPart = 1;
                }
                tBR = m_tPanels[i].tR;
                if (tBR.Right > tOR.Right)
                {
                    tBR.Right = tOR.Right - 1;
                    bEnd = true;
                }
                if (m_tPanels[i].hIcon != 0)
                {
                    if (!bUseXpStyles)
                    {
                        DrawStatusText(lHDC, ref tBR, "", (int)m_tPanels[i].eStyle);
                        if ((m_tPanels[i].eStyle & ENSBRPanelStyleConstants.estbrOwnerDraw) == ENSBRPanelStyleConstants.estbrOwnerDraw)
                        {
                            OwnerDraw.Invoke(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault);
                        }
                        if (bDoDefault)
                        {
                            // Draw the icon:
                            lY = tBR.tOp + 1 + (tBR.Bottom - tBR.tOp - 2 - m_lIconSize) / 2;
                            lX = tBR.left + 2;
                            DrawIconEx(lHDC, lX, lY, m_tPanels[i].hIcon, m_lIconSize, m_lIconSize, 0, 0, DI_NORMAL);
                            // Draw the text:
                            if (Len(m_tPanels[i].sText) > 0)
                            {
                                tBR.left = tBR.left + m_lIconSize + 4;
                                DrawText(lHDC, m_tPanels[i].sText, -1, ref tBR, DT_VCENTER | DT_SINGLELINE | DT_WORD_ELLIPSIS);
                            }
                        }
                    }
                    else
                    {
                        hR = DrawThemeBackground(hTheme, m_obj.hdc, iPart, 0, ref tBR, ref tBR);
                        if ((m_tPanels[i].eStyle & ENSBRPanelStyleConstants.estbrOwnerDraw) == ENSBRPanelStyleConstants.estbrOwnerDraw)
                        {
                            OwnerDraw.Invoke(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault);
                        }
                        if (bDoDefault)
                        {
                            hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, iPart, 0, ref tBR, ref rcContent);

                            // Fails...
                            // hR = DrawThemeIcon(hTheme, m_obj.hdc, 0, 0, tBR, m_hIml, m_tPanels[i].iImgIndex)
                            lY = tBR.tOp + 2 + (tBR.Bottom - tBR.tOp - 2 - m_lIconSize) / 2;
                            lX = tBR.left + 2;
                            DrawIconEx(lHDC, lX, lY, m_tPanels[i].hIcon, m_lIconSize, m_lIconSize, 0, 0, DI_NORMAL);
                            rcContent.left = rcContent.left + m_lIconSize + 4;
                            hR = DrawThemeText(hTheme, m_obj.hdc, 1, 0, " " + m_tPanels[i].sText, -1, DT_VCENTER | DT_SINGLELINE | DT_WORD_ELLIPSIS, 0, ref rcContent);
                        }
                    }
                }
                else
                {
                    if (!bUseXpStyles)
                    {
                        if ((m_tPanels[i].eStyle & ENSBRPanelStyleConstants.estbrOwnerDraw) == ENSBRPanelStyleConstants.estbrOwnerDraw)
                        {
                            DrawStatusText(lHDC, ref tBR, "", (int)m_tPanels[i].eStyle);
                            OwnerDraw.Invoke(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault);
                        }
                        if (bDoDefault)
                        {
                            DrawStatusText(lHDC, ref tBR, m_tPanels[i].sText, (int)m_tPanels[i].eStyle);
                        }
                    }
                    else
                    {
                        hR = DrawThemeBackground(hTheme, m_obj.hdc, iPart, 0, ref tBR, ref tBR);
                        if ((m_tPanels[i].eStyle & ENSBRPanelStyleConstants.estbrOwnerDraw) == ENSBRPanelStyleConstants.estbrOwnerDraw)
                        {
                            OwnerDraw.Invoke(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault);
                        }
                        if (bDoDefault)
                        {
                            hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, iPart, 0, ref tBR, ref rcContent);

                            hR = DrawThemeText(hTheme, m_obj.hdc, 1, 0, " " + m_tPanels[i].sText, -1, DT_VCENTER | DT_SINGLELINE, 0, ref rcContent);
                        }
                    }
                }
                if (bEnd)
                {
                    break;
                }
            }

        }

        if (m_bSizeGrip)
        {
            if (bUseXpStyles)
            {
                tOR = tR;
                tOR.left = tR.Right - (tR.Bottom - tR.tOp);
                hR = DrawThemeBackground(hTheme, m_obj.hdc, 3, 0, ref tOR, ref tOR);
            }
            else
            {
                fntThis = new(m_obj.Font, System.Drawing.FontStyle.Regular);
                //fntThis.Name = m_obj.Font.Name;
                //fntThis.Size = m_obj.Font.Size;
                //fntThis.Bold = m_obj.Font.Bold;
                //fntThis.Italic = m_obj.Font.Italic;
                //fntThis.Underline = m_obj.Font.Underline;
                m_obj.Font.Name = "Marlett";
                m_obj.Font.Size = fntThis.Size * 4 / 3;
                m_obj.ForeColor = SystemColors.ControlLightLight;
                OffsetRect(ref tOR, -2, -1);
                DrawText(lHDC, "o", 1, ref tOR, DT_BOTTOM | DT_RIGHT | DT_SINGLELINE);
                m_obj.ForeColor = SystemColors.ControlDark;
                // OffsetRect tOR, 1, 0
                DrawText(lHDC, "p", 1, ref tOR, DT_BOTTOM | DT_RIGHT | DT_SINGLELINE);
                m_obj.Font = fntThis;
                m_obj.ForeColor = SystemColors.WindowText;
            }
        }

        if (hTheme != 0)
        {
            CloseThemeData(hTheme);
        }
    }

    public void RemovePanel(dynamic vKey)
    {
        int iIndex = PanelIndex(vKey);
        if (iIndex > 0)
        {
            if (m_tPanels[iIndex].hIcon != 0)
            {
                DestroyIcon(m_tPanels[iIndex].hIcon);
            }
            for (int i = iIndex; i <= m_iPanelCount - 1; i += 1)
            {
                m_tPanels[i] = m_tPanels[i + 1];
            }
            m_iPanelCount--;
            if (m_iPanelCount > 0)
            {
                // TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
                m_tPanels.RemoveAt(m_tPanels.Count - 1);
            }
            Draw();
        }
    }

    public void SetLeftTopOffsets(int lLeft, int lTop)
    {
        m_lLeft = lLeft;
        m_lTop = lTop;
    }

    public dynamic ImageList
    {
        set
        {
            m_hIml = 0;
            m_ptrVb6ImageList = 0;
            if (VarType(value) == vbLong)
            {
                // Assume a handle to an image list:
                m_hIml = value;
            }
            else if (VarType(value) == vbObject)
            {
                // Assume a VB image list:
                // TODO: (NOT SUPPORTED): On Error Resume Next
                // Get the image list initialised..
                value.ListImages[1].Draw(0, 0, 0, 1);
                m_hIml = value.hImageList;
                if (Err().Number == 0)
                {
                    // Check for VB6 image list:
                    if (TypeName(value) == "ImageList")
                    {
                        if (value.ListImages.count != ImageList_GetImageCount(m_hIml))
                        {
                            dynamic O = value;
                            var gch = GCHandle.Alloc(O, GCHandleType.Pinned);
                            m_ptrVb6ImageList = GCHandle.ToIntPtr(gch);
                            gch.Free();
                        }
                    }
                }
                else
                {
                    Trace("Failed to Get Image list Handle", "cVGrid.ImageList");
                }
                // TODO: (NOT SUPPORTED): On Error GoTo 0
            }
            if (m_hIml != 0)
            {
                if (m_ptrVb6ImageList != 0)
                {
                    m_lIconSize = value.ImageHeight;
                }
                else
                {
                    RECT rc = null;
                    ImageList_GetImageRect(m_hIml, 0, ref rc);
                    m_lIconSize = rc.Bottom - rc.tOp;
                }
            }
        }
    }

    public void Create(dynamic objThis)
    {
        RECT tR = null;

        m_obj = objThis;

        // Check if required methods are supported:
        // TODO: (NOT SUPPORTED): On Error Resume Next
        int lHDC = m_obj.hdc;
        int lWidth = m_obj.ScaleWidth;
        int lHeight = m_obj.ScaleHeight;
        if (Err().Number != 0)
        {
            m_obj = null;
            Err().Raise(9, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".cNoStatusBar", "Invalid object passed to Create.");
        }
        else
        {
            // Get the height of the font and store:
            DrawText(lHDC, "Xy", 2, ref tR, DT_CALCRECT);
            m_lHeight = tR.Bottom - tR.tOp + 10;
        }
    }

    public Font Font
    {
        get => m_obj.Font;
        set
        {
            RECT tR = default;
            m_obj.Font = value;
            // Get the height of the font and store:
            DrawText(m_obj.hdc, "Xy", 2, ref tR, DT_CALCRECT);
            m_lHeight = tR.Bottom - tR.tOp + 10;
        }
    }

    public int Height => m_lHeight * Screen.TwipsPerPixelY;

    public int PanelCount => m_iPanelCount;

    public void GetPanelRect(dynamic vKey, ref int iLeftPixels, ref int iTopPixels, ref int iRightPixels, ref int iBottomPixels)
    {
        int iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            iLeftPixels = m_tPanels[iPanel].tR.left;
            iTopPixels = m_tPanels[iPanel].tR.tOp;
            iRightPixels = m_tPanels[iPanel].tR.Right;
            iBottomPixels = m_tPanels[iPanel].tR.Bottom;
        }
    }

    public dynamic PanelKey(int lIndex)
    {
        dynamic _PanelKey = default;
        if (lIndex > 0 && lIndex <= m_iPanelCount)
        {
            _PanelKey = m_tPanels[lIndex].sKey;
        }
        else
        {
            Err().Raise(vbObjectError + 1050, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "Invalid Panel Index: " + lIndex);
        }

        return _PanelKey;
    }
    public void PanelKey(int lIndex, dynamic vKey)
    {
        if (lIndex > 0 && lIndex <= m_iPanelCount)
        {
            m_tPanels[lIndex].sKey = vKey;
        }
        else
        {
            Err().Raise(vbObjectError + 1050, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "Invalid Panel Index: " + lIndex);
        }
    }

    public bool PanelExists(dynamic vKey)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        int i = PanelIndex(vKey);
        bool _PanelExists = i > 0 && Err().Number == 0;
        Err().Clear();
        // TODO: (NOT SUPPORTED): On Error GoTo 0
        return _PanelExists;
    }

    public int PanelIndex(dynamic vKey)
    {
        int _PanelIndex = default;
        int iFound = 0;

        if (IsNumeric(vKey))
        {
            if (vKey > 0 && vKey <= m_iPanelCount)
            {
                _PanelIndex = vKey;
            }
            else
            {
                Err().Raise(vbObjectError + 1050, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "Invalid Panel Index: " + vKey);
            }
        }
        else
        {
            for (int i = 1; i <= m_iPanelCount; i += 1)
            {
                if (m_tPanels[i].sKey == vKey)
                {
                    iFound = i;
                    break;
                }
            }
            if (iFound > 0)
            {
                _PanelIndex = iFound;
            }
            else
            {
                Err().Raise(vbObjectError + 1050, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".vbalStatusBar", "Invalid Panel Index: " + vKey);
            }
        }

        return _PanelIndex;
    }

    public string PanelText(dynamic vKey)
    {
        string _PanelText = default;
        int iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelText = m_tPanels[iPanel].sText;
        }
        return _PanelText;
    }
    public void PanelText(dynamic vKey, string sText)
    {
        int iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            m_tPanels[iPanel].sText = sText;
            Draw();
        }
    }

    public bool PanelSpring(dynamic vKey)
    {
        bool _PanelSpring = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelSpring = m_tPanels[iPanel].bSpring;
        }
        return _PanelSpring;
    }
    public void PanelSpring(dynamic vKey, bool bState)
    {
        int iPanel;
        int i = 0;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            if (m_tPanels[iPanel].bSpring != bState)
            {
                for (i = 1; i <= m_iPanelCount; i += 1)
                {
                    if (i == iPanel)
                    {
                        m_tPanels[iPanel].bSpring = bState;
                    }
                    else
                    {
                        m_tPanels[iPanel].bSpring = false;
                    }
                }
                pEvaluateIdealSize(iPanel);
                pResizeStatus();
            }
        }
    }

    public bool PanelFitToContents(dynamic vKey)
    {
        bool _PanelFitToContents = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelFitToContents = m_tPanels[iPanel].bFit;
        }
        return _PanelFitToContents;
    }
    public void PanelFitToContents(dynamic vKey, bool bState)
    {
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            if (m_tPanels[iPanel].bFit != bState)
            {
                m_tPanels[iPanel].bFit = bState;
                pEvaluateIdealSize(iPanel);
                pResizeStatus();
            }
        }
    }

    public int PanelIcon(dynamic vKey)
    {
        int _PanelIcon = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelIcon = m_tPanels[iPanel].iImgIndex;
        }
        return _PanelIcon;
    }
    public void PanelIcon(dynamic vKey, int iImgIndex)
    {
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            if (m_tPanels[iPanel].hIcon != 0)
            {
                DestroyIcon(m_tPanels[iPanel].hIcon);
            }
            m_tPanels[iPanel].hIcon = 0;
            m_tPanels[iPanel].iImgIndex = iImgIndex;
            if (iImgIndex > -1)
            {
                int hIcon = 0;
                if (!(m_ptrVb6ImageList == 0))
                {
                    dynamic O = null;
                    // TODO: (NOT SUPPORTED): On Error Resume Next
                    O = ObjectFromPtr(m_ptrVb6ImageList);
                    if (!(O == null))
                    {
                        hIcon = O.ListImages(iImgIndex + 1).ExtractIcon();
                    }
                    // TODO: (NOT SUPPORTED): On Error GoTo 0
                }
                else
                {
                    // extract a copy of the icon and add to sbar:
                    hIcon = ImageList_GetIcon(m_hIml, iImgIndex, 0);
                }
                m_tPanels[iPanel].hIcon = hIcon;
            }
            Draw();
        }
    }

    public int PanelhIcon(dynamic vKey)
    {
        int _PanelhIcon = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            // Returns a hIcon if any:
            _PanelhIcon = m_tPanels[iPanel].hIcon;
        }
        return _PanelhIcon;
    }
    public void PanelhIcon(dynamic vKey, int hIcon)
    {
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            // Destroy existing hIcon:
            if (m_tPanels[iPanel].hIcon != 0)
            {
                DestroyIcon(m_tPanels[iPanel].hIcon);
            }
            m_tPanels[iPanel].hIcon = hIcon;
            Draw();
        }
    }

    private dynamic ObjectFromPtr(int lPtr)
    {
        dynamic _ObjectFromPtr;
        dynamic oTemp = default;
        dynamic src = lPtr;
        CopyMemory(ref oTemp, ref src, 4);
        _ObjectFromPtr = oTemp;
        src = 0;
        CopyMemory(ref oTemp, ref src, 4);
        return _ObjectFromPtr;
    }

    public ENSBRPanelStyleConstants PanelStyle(dynamic vKey)
    {
        ENSBRPanelStyleConstants _PanelStyle = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelStyle = m_tPanels[iPanel].eStyle;
        }
        return _PanelStyle;
    }
    public void PanelStyle(dynamic vKey, ENSBRPanelStyleConstants eStyle)
    {
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            iPanel--;
            m_tPanels[iPanel].eStyle = eStyle;
            Draw();
        }
    }

    public int PanelMinWidth(dynamic vKey)
    {
        int _PanelMinWidth = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelMinWidth = m_tPanels[iPanel].lMinWidth;
        }
        return _PanelMinWidth;
    }

    public int PanelIdealWidth(dynamic vKey)
    {
        int _PanelIdealWidth = default;
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            _PanelIdealWidth = m_tPanels[iPanel].lIdealWidth;
        }
        return _PanelIdealWidth;
    }
    public void PanelIdealWidth(dynamic vKey, int lWidth)
    {
        int iPanel;
        iPanel = PanelIndex(vKey);
        if (iPanel > 0)
        {
            m_tPanels[iPanel].lIdealWidth = lWidth;
            pResizeStatus();
        }
    }

    private void pEvaluateIdealSize(int iStartPanel, int iEndPanel = -1)
    {
        RECT tR = null;

        if (m_iPanelCount > 0)
        {
            if (iEndPanel < iStartPanel)
            {
                iEndPanel = iStartPanel;
            }
            int lHDC = m_obj.hdc;
            for (int i = iStartPanel; i <= iEndPanel; i += 1)
            {
                DrawText(lHDC, m_tPanels[i].sText, Len(m_tPanels[i].sText), ref tR, DT_CALCRECT);
                m_tPanels[i].lIdealWidth = tR.Right - tR.left + 12;
                if (m_tPanels[i].lIdealWidth < m_tPanels[i].lMinWidth)
                {
                    m_tPanels[i].lIdealWidth = m_tPanels[i].lMinWidth;
                }
            }
        }
    }
    private void pResizeStatus()
    {
        RECT tR = null;
        int i;
        int iSpringIndex = 0;
        int[] lpParts;

        if (m_iPanelCount > 0)
        {

            GetClientRect(m_obj.hwnd, ref tR);
            tR.left += m_lLeft;
            tR.tOp += m_lTop;

            // Initiallly set to minimum widths:
            lpParts = new int[m_iPanelCount - 1];
            if (m_tPanels[1].bFit)
            {
                lpParts[0] = m_tPanels[1].lIdealWidth;
            }
            else
            {
                lpParts[0] = m_tPanels[1].lMinWidth;
            }
            if (m_tPanels[1].hIcon != 0)
            {
                lpParts[0] = lpParts[0] + m_lIconSize;
            }
            if (m_tPanels[1].bSpring)
            {
                iSpringIndex = 1;
            }
            for (i = 2; i <= m_iPanelCount; i += 1)
            {
                if (m_tPanels[i].bFit)
                {
                    lpParts[i - 1] = lpParts[i - 2] + m_tPanels[i].lIdealWidth;
                }
                else
                {
                    lpParts[i - 1] = lpParts[i - 2] + m_tPanels[i].lMinWidth;
                }
                if (m_tPanels[i].bSpring)
                {
                    iSpringIndex = i;
                }
                if (m_tPanels[i].hIcon != 0)
                {
                    // Add space for the icon:
                    lpParts[i - 1] = lpParts[i - 1] + m_lIconSize;
                }
                if (i == m_iPanelCount)
                {
                    lpParts[i - 1] = lpParts[i - 1] + (tR.Bottom - tR.tOp) / 2;
                }
            }

            // Will all bars fit in at maximum size?
            if (lpParts[m_iPanelCount - 1] > tR.Right)
            {
                // Draw all panels at min width
            }
            else
            {
                // Spring the spring panel to fit:
                if (iSpringIndex == 0)
                {
                    iSpringIndex = m_iPanelCount;
                }
                lpParts[iSpringIndex - 1] = lpParts[iSpringIndex - 1] + (tR.Right - lpParts[m_iPanelCount - 1]);
                for (i = iSpringIndex + 1; i <= m_iPanelCount; i += 1)
                {
                    if (m_tPanels[i].bFit)
                    {
                        lpParts[i - 1] = lpParts[i - 2] + m_tPanels[i].lIdealWidth;
                    }
                    else
                    {
                        lpParts[i - 1] = lpParts[i - 2] + m_tPanels[i].lMinWidth;
                    }
                    if (m_tPanels[i].hIcon != 0)
                    {
                        // Add space for the icon:
                        lpParts[i - 1] = lpParts[i - 1] + m_lIconSize;
                    }
                    if (i == m_iPanelCount)
                    {
                        lpParts[i - 1] = lpParts[i - 1] + (tR.Bottom - tR.tOp) / 2;
                    }
                }
            }

            m_tPanels[1].lSetWidth = lpParts[0];
            for (i = 2; i <= m_iPanelCount; i += 1)
            {
                m_tPanels[i].lSetWidth = lpParts[i - 1] - lpParts[i - 2];
            }

            // Set the sizes:
            for (i = 1; i <= m_iPanelCount; i += 1)
            {
                if (i == 1)
                {
                    m_tPanels[i].tR.left = tR.left;
                }
                else
                {
                    m_tPanels[i].tR.left = lpParts[i - 2];
                }
                if (i == m_iPanelCount)
                {
                    m_tPanels[i].tR.Right = lpParts[i - 1];
                }
                else
                {
                    m_tPanels[i].tR.Right = lpParts[i - 1] - 1;
                }
                m_tPanels[i].tR.tOp = tR.tOp;
                m_tPanels[i].tR.Bottom = tR.Bottom;
            }
        }
    }

    public cNoStatusBar()
    {
        int lMajor = 0;
        int lMinor = 0;
        int lRevision = 0;
        int lBuidNumber = 0;
        GetWindowsVersion(ref lMajor, ref lMinor, ref lRevision, ref lBuidNumber);
        if (lMajor > 5)
        {
            m_bIsXpOrAbove = true;
            // Fix for W2k bug:
        }
        else if (lMajor == 5 && lMinor >= 1)
        {
            m_bIsXpOrAbove = true;
        }
        if (m_bIsXpOrAbove)
        {
            m_bUseXpStyles = true;
        }
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
            // Delete any icons owned by the sbar:
            for (int i = 1; i <= m_iPanelCount; i += 1)
            {
                if (m_tPanels[i].hIcon != 0)
                {
                    DestroyIcon(m_tPanels[i].hIcon);
                    m_tPanels[i].hIcon = 0;
                }
            }
            _disposedValue = true;
        }
    }

    ~cNoStatusBar()
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
