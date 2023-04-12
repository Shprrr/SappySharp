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



public class cNoStatusBar {


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
  class POINTAPI{ 
  public int x;
  public int y;
}
  class RECT{ 
  public int left;
  public int tOp;
  public int Right;
  public int Bottom;
}
[DllImport("COMCTL32", EntryPoint="DrawStatusTextA")]
private static extern int DrawStatusText(int hdc, ref RECT lprc, string pszText, int uFlags);
[DllImport("COMCTL32")]
private static extern int ImageList_GetIcon(int hImageList, int ImgIndex, int fuFlags);
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_GetImageCount(int hIml);
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_GetImageRect(int hIml, int i, ref RECT prcImage);
[DllImport("COMCTL32.DLL")]
private static extern int ImageList_GetIconSize(int hIml, int cX, int cY);
[DllImport("user32")]
private static extern int DrawIconEx(int hdc, int xLeft, int yTop, int hIcon, int cxWidth, int cyWidth, int istepIfAniCur, int hbrFlickerFreeDraw, int diFlags);
int DI_MASK = 0x1;
int DI_IMAGE = 0x2;
int DI_NORMAL = 0x3;
int DI_COMPAT = 0x4;
int DI_DEFAULTSIZE = 0x8;
[DllImport("user32")]
private static extern int DestroyIcon(int hIcon);
[DllImport("user32", EntryPoint="DrawTextA")]
private static extern int DrawText(int hdc, string lpStr, int nCount, ref RECT lpRect, int wFormat);
int DT_CALCRECT = 0x400;
int DT_CENTER = 0x1;
int DT_VCENTER = 0x4;
int DT_SINGLELINE = 0x20;
int DT_RIGHT = 0x2;
int DT_BOTTOM = 0x8;
int DT_WORD_ELLIPSIS = 0x40000;
[DllImport("user32")]
private static extern int GetClientRect(int hwnd, ref RECT lpRect);
[DllImport("user32")]
private static extern int OffsetRect(ref RECT lpRect, int x, int y);
int SBT_NOBORDERS = 0x100;
int SBT_POPOUT = 0x200;
int SBT_RTLREADING = 0x400;
int SBT_OWNERDRAW = 0x1000;
[DllImport("user32")]
private static extern int GetSysColorBrush(int nIndex);
[DllImport("user32")]
private static extern int FillRect(int hdc, ref RECT lpRect, int hBrush);
[DllImport("gdi32")]
private static extern int DeleteObject(int hObject);
int COLOR_BTNFACE = 15;
[DllImport("kernel32", EntryPoint="RtlMoveMemory")]
private static extern void CopyMemory(ref dynamic pDest, ref dynamic pSrc, int ByteLen);

 // XP DrawTheme declares for XP version
[DllImport("kernel32")]
private static extern int GetVersion();
[DllImport("uxtheme.dll")]
private static extern int OpenThemeData(int hwnd, int pszClassList);
[DllImport("uxtheme.dll")]
private static extern int CloseThemeData(int hTheme);
[DllImport("uxtheme.dll")]
private static extern int DrawThemeBackground(int hTheme, int lHDC, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);
[DllImport("uxtheme.dll")]
private static extern int GetThemeBackgroundContentRect(int hTheme, int hdc, int iPartId, int iStateId, ref RECT pBoundingRect, ref RECT pContentRect);
[DllImport("uxtheme.dll")]
private static extern int DrawThemeText(int hTheme, int hdc, int iPartId, int iStateId, int pszText, int iCharCount, int dwTextFlag, int dwTextFlags2, ref RECT pRect);
[DllImport("uxtheme.dll")]
private static extern int DrawThemeIcon(int hTheme, int hdc, int iPartId, int iStateId, ref RECT pRect, int hIml, int iImageIndex);
int S_OK = 0;

 // =========================================================================
 // Implementation of fake status bar:
 // =========================================================================
  public enum ENSBRPanelStyleConstants{ 
  estbrStandard = 0x0
  , estbrNoBorders = SBT_NOBORDERS
  , estbrRaisedBorder = SBT_POPOUT
  , estbrOwnerDraw = SBT_OWNERDRAW
}
  class tStatusPanel{ 
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
List<tStatusPanel> m_tPanels = new List<tStatusPanel>();
int m_iPanelCount = 0;
bool m_bSizeGrip = false;
int m_hIml = 0;
int m_ptrVb6ImageList = 0;
PictureBox m_pic = null;
int m_lIconSize = 0;
dynamic m_obj = null;
int m_lLeft = 0;
int m_lTop = 0;
int m_lHeight = 0;
bool m_bSimpleMode = false;
string m_sSimpleText = "";

bool m_bIsXpOrAbove = false;
bool m_bUseXpStyles = false;

int OwnerDraw(ByVal hdc = 0;
int iLeft = 0;
int iTop = 0;
int iRight = 0;
int iBottom = 0;
Boolean) bDoDefault = null;

  private void GetWindowsVersion(ref int lMajor = 0, ref int lMinor = 0, ref int lRevision = 0, ref int lBuildNumber = 0) {
  int lR = 0;
  lR = GetVersion();
  lBuildNumber = (lR && 0x7F000000) / 0x1000000;
  if((lR && 0x80000000))lBuildNumber = lBuildNumber || 0x80;
  lRevision = (lR && 0xFF0000) / 0x10000;
  lMinor = (lR && 0xFF00) / 0x100;
  lMajor = (lR && 0xFF);
}

  public bool SimpleMode{ 
get {
bool _SimpleMode = default(bool);
_SimpleMode = m_bSimpleMode;
return _SimpleMode;
}
set {
m_bSimpleMode = bSimple;
Draw();
}
}

  
  public string SimpleText{ 
get {
string _SimpleText = default(string);
_SimpleText = m_sSimpleText;
return _SimpleText;
}
set {
m_sSimpleText = sText;
  if(((m_bSimpleMode))) {
  Draw();
}
}
}

  

public bool AllowXPStyles{ 
get {
bool _AllowXPStyles = default(bool);
_AllowXPStyles = m_bIsXpOrAbove;
return _AllowXPStyles;
}
set {
  if(((bState))) {
    if(! ((m_bIsXpOrAbove))) {
    Err.Raise(vbObjectError + 1052, App.EXEName + ".vbalStatusBar", "XP Styles not supported on this Windows installation.");
    } else {
    m_bUseXpStyles = true;
  }
  } else {
  m_bUseXpStyles = false;
}
}
}



public bool SizeGrip{ 
get {
bool _SizeGrip = default(bool);
_SizeGrip = m_bSizeGrip;
return _SizeGrip;
}
set {
m_bSizeGrip = bSizeGrip;
Draw();
}
}



public int AddPanel(ENSBRPanelStyleConstants eStyle = estbrStandard, string sText = "", int iImgIndex = -1, int lMinWidth = 64, bool bSpring = false, bool bFitContents = false, int lItemData = 0, string sKey = "", dynamic vKeyBefore) {
int _AddPanel = 0;
int iIndex = 0;
int i = 0;
bool bEnabled = false;
RECT tR = null;

if((m_iPanelCount >= 0xFF)) {
Err.Raise(vbObjectError + 1051, App.EXEName + ".vbalStatusBar", "Too many panels.");
return _AddPanel;
}

if(! IsMissing(vKeyBefore)) {
 // Determine if vKeyBefore is valid:
iIndex = PanelIndex(vKeyBefore);
if((iIndex > 0)) {
 // ok. Insert a space:
m_iPanelCount = m_iPanelCount + 1;
// TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
for (i = m_iPanelCount; i <= -1; i += iIndex + 1) {
LSet(m_tPanels[i] == m_tPanels[i - 1]);
}
m_tPanels[iIndex].hIcon = 0;
} else {
 // Failed
return _AddPanel;
}
} else {
 // Insert a space at the end:
m_iPanelCount = m_iPanelCount + 1;
// TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
iIndex = m_iPanelCount;
}

 // Set up the info:
if(((bSpring))) {
for (i = 1; i <= m_iPanelCount; i += 1) {
if((i != iIndex)) {
m_tPanels[i].bSpring = false;
}
}
}

// TODO: (NOT SUPPORTED): With m_tPanels(iIndex)
.bFit = bFitContents;
.bSpring = bSpring;
.eStyle = eStyle;
.iImgIndex = iImgIndex;
.lMinWidth = lMinWidth;
.lItemData = lItemData;
.sKey = sKey;
.sText = sText;
// TODO: (NOT SUPPORTED): End With

 // Add the information to the status bar:
pEvaluateIdealSize(iIndex);
pResizeStatus();

 // Now ensure the text, style, tooltip and icon are actually correct:
PanelText(iIndex) = m_tPanels[iIndex].sText;
PanelIcon(iIndex) = m_tPanels[iIndex].iImgIndex;

Draw();

return _AddPanel;
}

public void Draw() {
int i = 0;
int iEnd = 0;
int lHDC = 0;
int lX = 0;
int lY = 0;
int hBr = 0;
RECT tR = null;
RECT tOR = null;
RECT tBR = null;
StdFont fntThis = null;
bool bEnd = false;
int hTheme = 0;
int hR = 0;
RECT rc = null;
RECT rcContent = null;
bool bUseXpStyles = false;
bool bDoDefault = false;
bDoDefault = true;

GetClientRect(m_obj.hwnd, tR);

if(((m_bUseXpStyles))) {
bUseXpStyles = true;
hTheme = OpenThemeData(m_obj.hwnd, StrPtr("Status"));
if((hTheme == 0)) {
bUseXpStyles = false;
} else {
 // draw the background for the status bar:
hR = DrawThemeBackground(hTheme, m_obj.hdc, 4, 0, tR, tR);
if((hR != S_OK)) {
bUseXpStyles = false;
}
}
}

if(! ((bUseXpStyles))) {
hBr = GetSysColorBrush(COLOR_BTNFACE);
FillRect(m_obj.hdc, tR, hBr);
DeleteObject(hBr);
}

LSet(tOR == tR);

pResizeStatus();
lHDC = m_obj.hdc;
if(((m_bSimpleMode))) {
if(((bUseXpStyles))) {
hR = DrawThemeBackground(hTheme, m_obj.hdc, 2, 0, tR, tR);
hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, 2, 0, tR, rcContent);
hR = DrawThemeText(hTheme, m_obj.hdc, 2, 0, StrPtr(" " + m_sSimpleText), -1, DT_VCENTER || DT_SINGLELINE, 0, rcContent);
} else {
DrawText(lHDC, m_sSimpleText, -1, tR, DT_VCENTER || DT_SINGLELINE);
}
} else {
int iPart = 0;
for (i = 1; i <= m_iPanelCount; i += 1) {
if((i == m_iPanelCount)) {
iPart = 2;
} else {
iPart = 1;
}
// TODO: (NOT SUPPORTED): With m_tPanels(i)
LSet(tBR == tR);
if((tBR.Right > tOR.Right)) {
tBR.Right = tOR.Right - 1;
bEnd = true;
}
if((hIcon != 0)) {
if(! ((bUseXpStyles))) {
DrawStatusText(lHDC, tBR, "", eStyle);
  if((eStyle && estbrOwnerDraw) == estbrOwnerDraw) {
  RaiseEvent(OwnerDraw(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault));
}
  if(bDoDefault) {
   // Draw the icon:
  lY = tBR.tOp + 1 + (tBR.Bottom - tBR.tOp - 2 - m_lIconSize) / 2;
  lX = tBR.left + 2;
  DrawIconEx(lHDC, lX, lY, hIcon, m_lIconSize, m_lIconSize, 0, 0, DI_NORMAL);
   // Draw the text:
    if((Len(sText) > 0)) {
    tBR.left = tBR.left + m_lIconSize + 4;
    DrawText(lHDC, sText, -1, tBR, DT_VCENTER || DT_SINGLELINE || DT_WORD_ELLIPSIS);
  }
}
} else {
hR = DrawThemeBackground(hTheme, m_obj.hdc, iPart, 0, tBR, tBR);
  if((eStyle && estbrOwnerDraw) == estbrOwnerDraw) {
  RaiseEvent(OwnerDraw(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault));
}
  if(bDoDefault) {
  hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, iPart, 0, tBR, rcContent);
  
   // Fails...
   // hR = DrawThemeIcon(hTheme, m_obj.hdc, 0, '            0, tBR, m_hIml, .iImgIndex)
  lY = tBR.tOp + 2 + (tBR.Bottom - tBR.tOp - 2 - m_lIconSize) / 2;
  lX = tBR.left + 2;
  DrawIconEx(lHDC, lX, lY, hIcon, m_lIconSize, m_lIconSize, 0, 0, DI_NORMAL);
  rcContent.left = rcContent.left + m_lIconSize + 4;
  hR = DrawThemeText(hTheme, m_obj.hdc, 1, 0, StrPtr(" " + sText), -1, DT_VCENTER || DT_SINGLELINE || DT_WORD_ELLIPSIS, 0, rcContent);
}
}
} else {
if(! ((bUseXpStyles))) {
  if((eStyle && estbrOwnerDraw) == estbrOwnerDraw) {
  DrawStatusText(lHDC, tBR, "", eStyle);
  RaiseEvent(OwnerDraw(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault));
}
  if(bDoDefault) {
  DrawStatusText(lHDC, tBR, sText, eStyle);
}
} else {
hR = DrawThemeBackground(hTheme, m_obj.hdc, iPart, 0, tBR, tBR);
  if((eStyle && estbrOwnerDraw) == estbrOwnerDraw) {
  RaiseEvent(OwnerDraw(lHDC, tBR.left, tBR.tOp, tBR.Right, tBR.Bottom, bDoDefault));
}
  if(bDoDefault) {
  hR = GetThemeBackgroundContentRect(hTheme, m_obj.hdc, iPart, 0, tBR, rcContent);
  
  hR = DrawThemeText(hTheme, m_obj.hdc, 1, 0, StrPtr(" " + sText), -1, DT_VCENTER || DT_SINGLELINE, 0, rcContent);
}
}
}
if(bEnd) {
break;
}
// TODO: (NOT SUPPORTED): End With
}

}

if(((m_bSizeGrip))) {
if(((bUseXpStyles))) {
LSet(tOR == tR);
tOR.left = tR.Right - (tR.Bottom - tR.tOp);
hR = DrawThemeBackground(hTheme, m_obj.hdc, 3, 0, tOR, tOR);
} else {
fntThis = new StdFont();
// TODO: (NOT SUPPORTED): With fntThis
.name = m_obj.Font.name;
.Size = m_obj.Font.Size;
.Bold = m_obj.Font.Bold;
.Italic = m_obj.Font.Italic;
.Underline = m_obj.Font.Underline;
// TODO: (NOT SUPPORTED): End With
m_obj.Font.name = "Marlett";
m_obj.Font.Size = fntThis.Size * 4 / 3;
m_obj.ForeColor = vb3DHighlight;
OffsetRect(tOR, -2, -1);
DrawText(lHDC, "o", 1, tOR, DT_BOTTOM || DT_RIGHT || DT_SINGLELINE);
m_obj.ForeColor = vbButtonShadow;
 // OffsetRect tOR, 1, 0
DrawText(lHDC, "p", 1, tOR, DT_BOTTOM || DT_RIGHT || DT_SINGLELINE);
m_obj.Font = fntThis;
m_obj.ForeColor = vbWindowText;
}
}

if(((hTheme))) {
CloseThemeData(hTheme);
}

}

public void RemovePanel(dynamic vKey) {
 _RemovePanel = null;
int iIndex = 0;
int i = 0;
iIndex = PanelIndex(vKey);
if((iIndex > 0)) {
if((m_tPanels(iIndex).hIcon != 0)) {
DestroyIcon(m_tPanels(iIndex).hIcon);
}
for (i = iIndex; i <= m_iPanelCount - 1; i += 1) {
LSet(m_tPanels(i) == m_tPanels(i + 1));
}
m_iPanelCount = m_iPanelCount - 1;
if((m_iPanelCount > 0)) {
// TODO: (NOT SUPPORTED): ReDim Preserve m_tPanels(1 To m_iPanelCount) As tStatusPanel
}
Draw();
}
return _RemovePanel;
}


public void SetLeftTopOffsets(int lLeft, int lTop) {
m_lLeft = lLeft;
m_lTop = lTop;
}

public dynamic ImageList{ 
set {
m_hIml = 0;
m_ptrVb6ImageList = 0;
  if((VarType(vImageList) == vbLong)) {
   // Assume a handle to an image list:
  m_hIml = vImageList;
  } else if((VarType(vImageList) == vbObject)) {
   // Assume a VB image list:
  // TODO: (NOT SUPPORTED): On Error Resume Next
   // Get the image list initialised..
  vImageList.ListImages(1).Draw 0, 0, 0, 1;
  m_hIml = vImageList.hImageList;
    if((Err().Number == 0)) {
     // Check for VB6 image list:
      if((TypeName(vImageList) == "ImageList")) {
        if((vImageList.ListImages.count != ImageList_GetImageCount(m_hIml))) {
        dynamic O = null;
        O = vImageList;
        m_ptrVb6ImageList = ObjPtr(O);
      }
    }
    } else {
    Trace("Failed to Get Image list Handle", "cVGrid.ImageList");
  }
  // TODO: (NOT SUPPORTED): On Error GoTo 0
}
  if((m_hIml != 0)) {
    if((m_ptrVb6ImageList != 0)) {
    m_lIconSize = vImageList.ImageHeight;
    } else {
    RECT rc = null;
    ImageList_GetImageRect(m_hIml, 0, rc);
    m_lIconSize = rc.Bottom - rc.tOp;
  }
}
}
}


public void Create(ref dynamic objThis) {
int lHDC = 0;
int lWidth = 0;
int lHeight = 0;
RECT tR = null;

m_obj = objThis;

 // Check if required methods are supported:
// TODO: (NOT SUPPORTED): On Error Resume Next
lHDC = m_obj.hdc;
lWidth = m_obj.ScaleWidth;
lHeight = m_obj.ScaleHeight;
if((Err().Number != 0)) {
m_obj = null;
Err.Raise(9, App.EXEName + ".cNoStatusBar", "Invalid object passed to Create.");
} else {
 // Get the height of the font and store:
DrawText(lHDC, "Xy", 2, tR, DT_CALCRECT);
m_lHeight = tR.Bottom - tR.tOp + 10;
}

}

public StdFont Font{ 
get {
StdFont _Font = default(StdFont);
_Font = m_obj._Font;
return _Font;
}
set {
im(tR As RECT);
m_obj.Font = fntThis;
 // Get the height of the font and store:
DrawText(m_obj.hdc, "Xy", 2, tR, DT_CALCRECT);
m_lHeight = tR.Bottom - tR.tOp + 10;
}
}



public int Height{ 
get {
int _Height = default(int);
_Height = m_lHeight * Screen.TwipsPerPixelY;
return _Height;
}
}


public int PanelCount{ 
get {
int _PanelCount = default(int);
_PanelCount = m_iPanelCount;
return _PanelCount;
}
}

public void GetPanelRect(dynamic vKey, ref int iLeftPixels, ref int iTopPixels, ref int iRightPixels, ref int iBottomPixels) {
int iPanel = 0;
RECT tR = null;
iPanel = PanelIndex(vKey);
if((iPanel > 0)) {
// TODO: (NOT SUPPORTED): With m_tPanels(iPanel).tR
iLeftPixels = left;
iTopPixels = tOp;
iRightPixels = Right;
iBottomPixels = Bottom;
// TODO: (NOT SUPPORTED): End With
}
}

public dynamic PanelKey{ 
get {
dynamic _PanelKey = default(dynamic);
im(iPanel As Long);
  if((lIndex > 0) && (lIndex <= m_iPanelCount)) {
  _PanelKey = m_tPanels(lIndex).sKey;
  } else {
  Err.Raise(vbObjectError + 1050, App.EXEName + ".vbalStatusBar", "Invalid Panel Index: " + lIndex);
}

return _PanelKey;
}
set {
  if((lIndex > 0) && (lIndex <= m_iPanelCount)) {
  m_tPanels(lIndex).sKey = vKey;
  } else {
  Err.Raise(vbObjectError + 1050, App.EXEName + ".vbalStatusBar", "Invalid Panel Index: " + lIndex);
}

}
}


public int PanelExists{ 
get {
int _PanelExists = default(int);
// TODO: (NOT SUPPORTED): On Error Resume Next
int i = 0;
i = PanelIndex(vKey);
_PanelExists = ((i > 0) && (Err().Number == 0));
// TODO: (NOT SUPPORTED): Err.Clear
// TODO: (NOT SUPPORTED): On Error GoTo 0
return _PanelExists;
}
}

public int PanelIndex{ 
get {
int _PanelIndex = default(int);
im(i As Long);
int iFound = 0;

  if((IsNumeric(vKey))) {
    if((vKey > 0) && (vKey <= m_iPanelCount)) {
    _PanelIndex = vKey;
    } else {
    Err.Raise(vbObjectError + 1050, App.EXEName + ".vbalStatusBar", "Invalid Panel Index: " + vKey);
  }
  } else {
    for (i = 1; i <= m_iPanelCount; i += 1) {
      if(m_tPanels(i).sKey == vKey) {
      iFound = i;
      break;
    }
  }
    if((iFound > 0)) {
    _PanelIndex = iFound;
    } else {
    Err.Raise(vbObjectError + 1050, App.EXEName + ".vbalStatusBar", "Invalid Panel Index: " + vKey);
  }
}

return _PanelIndex;
}
}

public string PanelText{ 
get {
string _PanelText = default(string);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelText = m_tPanels(iPanel).sText;
}
return _PanelText;
}
set {
im(iPanel As Long);
int iPartuType = 0;
int lR = 0;
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  m_tPanels(iPanel).sText = sText;
  Draw();
}
}
}


public bool PanelSpring{ 
get {
bool _PanelSpring = default(bool);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelSpring = m_tPanels(iPanel).bSpring;
}
return _PanelSpring;
}
set {
im(iPanel As Long);
int i = 0;
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
    if((m_tPanels(iPanel).bSpring != bState)) {
      for (i = 1; i <= m_iPanelCount; i += 1) {
        if(i == iPanel) {
        m_tPanels(iPanel).bSpring = bState;
        } else {
        m_tPanels(iPanel).bSpring = false;
      }
    }
    pEvaluateIdealSize(iPanel);
    pResizeStatus();
  }
}
}
}


public bool PanelFitToContents{ 
get {
bool _PanelFitToContents = default(bool);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelFitToContents = m_tPanels(iPanel).bFit;
}
return _PanelFitToContents;
}
set {
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
    if((m_tPanels(iPanel).bFit != bState)) {
    m_tPanels(iPanel).bFit = bState;
    pEvaluateIdealSize(iPanel);
    pResizeStatus();
  }
}
}
}


public int PanelIcon{ 
get {
int _PanelIcon = default(int);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelIcon = m_tPanels(iPanel).iImgIndex;
}
return _PanelIcon;
}
set {
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
    if((m_tPanels(iPanel).hIcon != 0)) {
    DestroyIcon(m_tPanels(iPanel).hIcon);
  }
  m_tPanels(iPanel).hIcon = 0;
  m_tPanels(iPanel).iImgIndex = iImgIndex;
    if((iImgIndex > -1)) {
    int hIcon = 0;
      if(! (m_ptrVb6ImageList == 0)) {
      dynamic O = null;
      // TODO: (NOT SUPPORTED): On Error Resume Next
      O = ObjectFromPtr(m_ptrVb6ImageList);
        if(! (O == null)) {
        hIcon = O.ListImages(iImgIndex + 1).ExtractIcon();
      }
      // TODO: (NOT SUPPORTED): On Error GoTo 0
      } else {
       // extract a copy of the icon and add to sbar:
      hIcon = ImageList_GetIcon(m_hIml, iImgIndex, 0);
    }
    m_tPanels(iPanel).hIcon = hIcon;
  }
  Draw();
}
}
}

public int PanelhIcon{ 
get {
int _PanelhIcon = default(int);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
   // Returns a hIcon if any:
  _PanelhIcon = m_tPanels(iPanel).hIcon;
}
return _PanelhIcon;
}
set {
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
   // Destroy existing hIcon:
    if((m_tPanels(iPanel).hIcon != 0)) {
    DestroyIcon(m_tPanels(iPanel).hIcon);
  }
  m_tPanels(iPanel).hIcon = hIcon;
  Draw();
}
}
}


private dynamic ObjectFromPtr{ 
get {
dynamic _ObjectFromPtr = default(dynamic);
im(oTemp As Object);
CopyMemory(oTemp, lPtr, 4);
_ObjectFromPtr = oTemp;
CopyMemory(oTemp, 0, 4);
return _ObjectFromPtr;
}
}


public ENSBRPanelStyleConstants PanelStyle{ 
get {
ENSBRPanelStyleConstants _PanelStyle = default(ENSBRPanelStyleConstants);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelStyle = m_tPanels(iPanel).eStyle;
}
return _PanelStyle;
}
set {
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  iPanel = iPanel - 1;
  m_tPanels(iPanel).eStyle = eStyle;
  Draw();
}
}
}


public int PanelMinWidth{ 
get {
int _PanelMinWidth = default(int);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelMinWidth = m_tPanels(iPanel).lMinWidth;
}
return _PanelMinWidth;
}
}

public int PanelIdealWidth{ 
get {
int _PanelIdealWidth = default(int);
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  _PanelIdealWidth = m_tPanels(iPanel).lIdealWidth;
}
return _PanelIdealWidth;
}
set {
im(iPanel As Long);
iPanel = PanelIndex(vKey);
  if((iPanel > 0)) {
  m_tPanels(iPanel).lIdealWidth = lWidth;
  pResizeStatus();
}
}
}


private void pEvaluateIdealSize(int iStartPanel, int iEndPanel = -1) {
int i = 0;
RECT tR = null;
int lHDC = 0;

if((m_iPanelCount > 0)) {
if((iEndPanel < iStartPanel)) {
iEndPanel = iStartPanel;
}
lHDC = m_obj.hdc;
for (i = iStartPanel; i <= iEndPanel; i += 1) {
DrawText(lHDC, m_tPanels(i).sText, Len(m_tPanels(i).sText), tR, DT_CALCRECT);
m_tPanels(i).lIdealWidth = tR.Right - tR.left + 12;
if((m_tPanels(i).lIdealWidth < m_tPanels(i).lMinWidth)) {
m_tPanels(i).lIdealWidth = m_tPanels(i).lMinWidth;
}
}
}
}
private void pResizeStatus() {
RECT tR = null;
int i = 0;
int iSpringIndex = 0;
List<int> lpParts = new List<int>();

if((m_iPanelCount > 0)) {

GetClientRect(m_obj.hwnd, tR);
tR.left = tR.left + m_lLeft;
tR.tOp = tR.tOp + m_lTop;

 // Initiallly set to minimum widths:
// TODO: (NOT SUPPORTED): ReDim lpParts(0 To m_iPanelCount - 1) As Long
if((m_tPanels(1).bFit)) {
lpParts[0] = m_tPanels(1).lIdealWidth;
} else {
lpParts[0] = m_tPanels(1).lMinWidth;
}
if((m_tPanels(1).hIcon)) {
lpParts[0] = lpParts[0] + m_lIconSize;
}
if((m_tPanels(1).bSpring)) {
iSpringIndex = 1;
}
for (i = 2; i <= m_iPanelCount; i += 1) {
if((m_tPanels(i).bFit)) {
lpParts(i - 1) = lpParts(i - 2) + m_tPanels(i).lIdealWidth;
} else {
lpParts(i - 1) = lpParts(i - 2) + m_tPanels(i).lMinWidth;
}
if((m_tPanels(i).bSpring)) {
iSpringIndex = i;
}
if((m_tPanels(i).hIcon != 0)) {
 // Add space for the icon:
lpParts(i - 1) = lpParts(i - 1) + m_lIconSize;
}
if((i == m_iPanelCount)) {
lpParts(i - 1) = lpParts(i - 1) + (tR.Bottom - tR.tOp) \ 2;
}
}

 // Will all bars fit in at maximum size?
if((lpParts(m_iPanelCount - 1) > tR.Right)) {
 // Draw all panels at min width
} else {
 // Spring the spring panel to fit:
if((iSpringIndex == 0)) {
iSpringIndex = m_iPanelCount;
}
lpParts(iSpringIndex - 1) = lpParts(iSpringIndex - 1) + (tR.Right - lpParts(m_iPanelCount - 1));
for (i = iSpringIndex + 1; i <= m_iPanelCount; i += 1) {
if((m_tPanels(i).bFit)) {
lpParts(i - 1) = lpParts(i - 2) + m_tPanels(i).lIdealWidth;
} else {
lpParts(i - 1) = lpParts(i - 2) + m_tPanels(i).lMinWidth;
}
if((m_tPanels(i).hIcon != 0)) {
 // Add space for the icon:
lpParts(i - 1) = lpParts(i - 1) + m_lIconSize;
}
if((i == m_iPanelCount)) {
lpParts(i - 1) = lpParts(i - 1) + (tR.Bottom - tR.tOp) \ 2;
}
}
}

m_tPanels(1).lSetWidth = lpParts[0];
for (i = 2; i <= m_iPanelCount; i += 1) {
m_tPanels(i).lSetWidth = lpParts[i - 1] - lpParts[i - 2];
}

 // Set the sizes:
for (i = 1; i <= m_iPanelCount; i += 1) {
// TODO: (NOT SUPPORTED): With m_tPanels(i).tR
if((i == 1)) {
.left = tR.left;
} else {
.left = lpParts[i - 2];
}
if((i == m_iPanelCount)) {
.Right = lpParts[i - 1];
} else {
.Right = lpParts[i - 1] - 1;
}
.tOp = tR.tOp;
.Bottom = tR.Bottom;
// TODO: (NOT SUPPORTED): End With
}

}

}


private void Class_Initialize() {
int lMajor = 0;
int lMinor = 0;
GetWindowsVersion(lMajor, lMinor);
if((lMajor > 5)) {
m_bIsXpOrAbove = true;
 // Fix for W2k bug:
} else if((lMajor == 5) && (lMinor >= 1)) {
m_bIsXpOrAbove = true;
}
if(((m_bIsXpOrAbove))) {
m_bUseXpStyles = true;
}
}

private void Class_Terminate() {
int i = 0;
int lR = 0;
 // Delete any icons owned by the sbar:
for (i = 1; i <= m_iPanelCount; i += 1) {
if((m_tPanels(i).hIcon != 0)) {
lR = DestroyIcon(m_tPanels(i).hIcon);
m_tPanels(i).hIcon = 0;
}
}
}


}