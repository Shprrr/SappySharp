using System;
using System.Runtime.InteropServices;
using static VBConstants;

namespace SappySharp.Classes;

public partial class pcMemDC : IDisposable
{
    // cMemDC - flicker free drawing

    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleBitmap(int hDC, int nWidth, int nHeight);
    [LibraryImport("gdi32")]
    private static partial int CreateCompatibleDC(int hDC);
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
