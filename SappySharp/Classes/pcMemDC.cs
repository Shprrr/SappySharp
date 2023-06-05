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
using stdole;

namespace SappySharp.Classes;

public partial class pcMemDC : IDisposable
{
    // cMemDC - flicker free drawing

    class BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public int bmPlanes;
        public int bmBitsPixel;
        public int bmBits;
    }
    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleBitmap(int hDC, int nWidth, int nHeight);
    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleDC(int hDC);
    [DllImport("gdi32", EntryPoint = "CreateDCA")]
    private static extern int CreateDCAsNull(string lpDriverName, int lpDeviceName, int lpOutput, int lpInitData);
    [DllImport("gdi32", EntryPoint = "CreateDCA")]
    private static extern int CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, int lpInitData);
    [LibraryImport("gdi32")]
    private static partial int SelectObject(int hDC, int hObject);
    [LibraryImport("gdi32")]
    private static partial int DeleteObject(int hObject);
    [LibraryImport("gdi32")]
    private static partial int DeleteDC(int hDC);
    [LibraryImport("gdi32")]
    private static partial int BitBlt(int hDestDC, int x, int y, int nWidth, int nHeight, int hSrcDC, int xSrc, int ySrc, int dwRop);
    [DllImport("gdi32", EntryPoint = "GetObjectA")]
    private static extern int GetObjectAPI(int hObject, int nCount, ref dynamic lpObject);

    public static int m_hDC = 0;
    public static int m_hBmp = 0;
    public static int m_hBmpOld = 0;
    public static int m_lWidth = 0;
    public static int m_lHeight = 0;
    private bool _disposedValue;

    public int Width
    {
        get => m_lWidth;
        set
        {
            if (value > m_lWidth)
            {
                m_lWidth = value;
                pCreate(m_lWidth, m_lHeight);
            }
        }
    }

    public int Height
    {
        get => m_lHeight;
        set
        {
            if (value > m_lHeight)
            {
                m_lHeight = value;
                pCreate(m_lWidth, m_lHeight);
            }
        }
    }

    public int hDC => m_hDC;

    public void Draw(int hDC, int xSrc = 0, int ySrc = 0, int WidthSrc = 0, int HeightSrc = 0, int xDst = 0, int yDst = 0)
    {
        if (WidthSrc <= 0) WidthSrc = m_lWidth;
        if (HeightSrc <= 0) HeightSrc = m_lHeight;
        BitBlt(hDC, xDst, yDst, WidthSrc, HeightSrc, m_hDC, xSrc, ySrc, vbSrcCopy);
    }
    public void CreateFromPicture(IPicture sPic)
    {
        dynamic getObject = null;
        BITMAP tB = null;
        GetObjectAPI(sPic.Handle, Len(tB), ref getObject);
        tB = getObject;
        Width = tB.bmWidth;
        Height = tB.bmHeight;
        int lhDCC = CreateDCAsNull("DISPLAY", 0, 0, 0);
        int lhDC = CreateCompatibleDC(lhDCC);
        int lhBmpOld = SelectObject(lhDC, sPic.Handle);
        BitBlt(m_hDC, 0, 0, tB.bmWidth, tB.bmHeight, lhDC, 0, 0, vbSrcCopy);
        SelectObject(lhDC, lhBmpOld);
        DeleteDC(lhDC);
        DeleteDC(lhDCC);
    }
    private void pCreate(int Width, int Height)
    {
        pDestroy();
        int lhDCC = CreateDC("DISPLAY", "", "", 0);
        if (!(lhDCC == 0))
        {
            m_hDC = CreateCompatibleDC(lhDCC);
            if (!(m_hDC == 0))
            {
                m_hBmp = CreateCompatibleBitmap(lhDCC, Width, Height);
                if (!(m_hBmp == 0))
                {
                    m_hBmpOld = SelectObject(m_hDC, m_hBmp);
                    if (!(m_hBmpOld == 0))
                    {
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
    private void pDestroy()
    {
        if (m_hBmpOld != 0)
        {
            SelectObject(m_hDC, m_hBmpOld);
            m_hBmpOld = 0;
        }
        if (m_hBmp != 0)
        {
            DeleteObject(m_hBmp);
            m_hBmp = 0;
        }
        if (m_hDC != 0)
        {
            DeleteDC(m_hDC);
            m_hDC = 0;
        }
        m_lWidth = 0;
        m_lHeight = 0;
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
            pDestroy();
            _disposedValue = true;
        }
    }

    ~pcMemDC()
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
