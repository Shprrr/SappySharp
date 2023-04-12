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



public class pcMemDC {

 // cMemDC - flicker free drawing

  class BITMAP{ 
  public int bmType;
  public int bmWidth;
  public int bmHeight;
  public int bmWidthBytes;
  public int bmPlanes;
  public int bmBitsPixel;
  public int bmBits;
}
[DllImport("gdi32")]
private static extern int CreateCompatibleBitmap(int hDC, int nWidth, int nHeight);
[DllImport("gdi32")]
private static extern int CreateCompatibleDC(int hDC);
[DllImport("gdi32", EntryPoint="CreateDCA")]
private static extern int CreateDCAsNull(string lpDriverName, ref dynamic lpDeviceName, ref dynamic lpOutput, ref dynamic lpInitData);
[DllImport("gdi32", EntryPoint="CreateDCA")]
private static extern int CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, ref dynamic lpInitData);
[DllImport("gdi32")]
private static extern int SelectObject(int hDC, int hObject);
[DllImport("gdi32")]
private static extern int DeleteObject(int hObject);
[DllImport("gdi32")]
private static extern int DeleteDC(int hDC);
[DllImport("gdi32")]
private static extern int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
[DllImport("gdi32", EntryPoint="GetObjectA")]
private static extern int GetObjectAPI(int hObject, int nCount, ref dynamic lpObject);

public static int m_hDC = 0;
public static int m_hBmp = 0;
public static int m_hBmpOld = 0;
public static int m_lWidth = 0;
public static int m_lHeight = 0;

  
  



public int hDC{ 
get {
int _hDC = default(int);
_hDC = m_hDC;
return _hDC;
}
}

public void Draw(int hDC, int xSrc = 0, int ySrc = 0, int WidthSrc = 0, int HeightSrc = 0, int xDst = 0, int yDst = 0) {
if(WidthSrc <= 0)WidthSrc = m_lWidth;
if(HeightSrc <= 0)HeightSrc = m_lHeight;
BitBlt(hDC, xDst, yDst, WidthSrc, HeightSrc, m_hDC, xSrc, ySrc, vbSrcCopy);
}
public void CreateFromPicture(ref IPicture sPic) {
BITMAP tB = null;
int lhDCC = 0;
int lhDC = 0;
int lhBmpOld = 0;
GetObjectAPI(sPic.Handle, Len(tB), tB);
Width = tB.bmWidth;
Height = tB.bmHeight;
lhDCC = CreateDCAsNull("DISPLAY", ByVal 0&, ByVal 0&, ByVal 0&);
lhDC = CreateCompatibleDC(lhDCC);
lhBmpOld = SelectObject(lhDC, sPic.Handle);
BitBlt(m_hDC, 0, 0, tB.bmWidth, tB.bmHeight, lhDC, 0, 0, vbSrcCopy);
SelectObject(lhDC, lhBmpOld);
DeleteDC(lhDC);
DeleteDC(lhDCC);
}
private void pCreate(int Width, int Height) {
int lhDCC = 0;
pDestroy();
lhDCC = CreateDC("DISPLAY", "", "", ByVal 0&);
if(! (lhDCC == 0)) {
m_hDC = CreateCompatibleDC(lhDCC);
  if(! (m_hDC == 0)) {
  m_hBmp = CreateCompatibleBitmap(lhDCC, Width, Height);
    if(! (m_hBmp == 0)) {
    m_hBmpOld = SelectObject(m_hDC, m_hBmp);
      if(! (m_hBmpOld == 0)) {
      m_lWidth = Width;
      m_lHeight = Height;
      DeleteDC(lhDCC);
      return;
    }
  }
}
DeleteDC(lhDCC);
pDestroy();
}
}
private void pDestroy() {
if(! m_hBmpOld == 0) {
SelectObject(m_hDC, m_hBmpOld);
m_hBmpOld = 0;
}
if(! m_hBmp == 0) {
DeleteObject(m_hBmp);
m_hBmp = 0;
}
if(! m_hDC == 0) {
DeleteDC(m_hDC);
m_hDC = 0;
}
m_lWidth = 0;
m_lHeight = 0;
}

private void Class_Terminate() {
pDestroy();
}






}