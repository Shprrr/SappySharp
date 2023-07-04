using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Microsoft.VisualBasic.Information;

static partial class mColorUtils
{
    [LibraryImport("OLEPRO32.DLL")]
    private static partial int OleTranslateColor(int OLE_COLOR, int HPALETTE, ref int pccolorref);
    public const int CLR_INVALID = -1;

    public static void RGBToHLS(int r, int g, int b, ref decimal h, ref decimal s, ref decimal l)
    {
        decimal rR = r / 255m; decimal rG = g / 255m; decimal rB = b / 255m;
        decimal Max = Maximum(rR, rG, rB);
        decimal Min = Minimum(rR, rG, rB);
        l = (Max + Min) / 2;
        if (Max == Min)
        {
            s = 0;
            h = 0;
        }
        else
        {
            if (l <= 0.5m)
            {
                s = (Max - Min) / (Max + Min);
            }
            else
            {
                s = (Max - Min) / (2 - Max - Min);
            }
            decimal delta = Max - Min;
            if (rR == Max)
            {
                h = (rG - rB) / delta;
            }
            else if (rG == Max)
            {
                h = 2 + (rB - rR) / delta;
            }
            else if (rB == Max)
            {
                h = 4 + (rR - rG) / delta;
            }
        }
    }

    public static void HLSToRGB(decimal h, decimal s, decimal l, ref int r, ref int g, ref int b)
    {
        decimal rR;
        decimal rG;
        decimal rB;
        decimal Min;
        if (s == 0)
        {
            rR = l; rG = l; rB = l;
        }
        else
        {
            if (l <= 0.5m)
            {
                Min = l * (1 - s);
            }
            else
            {
                Min = l - s * (1 - l);
            }
            decimal Max = 2 * l - Min;
            if (h < 1)
            {
                rR = Max;
                if (h < 0)
                {
                    rG = Min;
                    rB = rG - h * (Max - Min);
                }
                else
                {
                    rB = Min;
                    rG = h * (Max - Min) + rB;
                }
            }
            else if (h < 3)
            {
                rG = Max;
                if (h < 2)
                {
                    rB = Min;
                    rR = rB - (h - 2) * (Max - Min);
                }
                else
                {
                    rR = Min;
                    rB = (h - 2) * (Max - Min) + rR;
                }
            }
            else
            {
                rB = Max;
                if (h < 4)
                {
                    rR = Min;
                    rG = rR - (h - 4) * (Max - Min);
                }
                else
                {
                    rG = Min;
                    rR = (h - 4) * (Max - Min) + rG;
                }
            }
        }
        r = (int)(rR * 255); g = (int)(rG * 255); b = (int)(rB * 255);
    }

    private static decimal Maximum(decimal rR, decimal rG, decimal rB)
    {
        decimal _Maximum;
        if (rR > rG)
        {
            if (rR > rB)
            {
                _Maximum = rR;
            }
            else
            {
                _Maximum = rB;
            }
        }
        else
        {
            if (rB > rG)
            {
                _Maximum = rB;
            }
            else
            {
                _Maximum = rG;
            }
        }
        return _Maximum;
    }
    private static decimal Minimum(decimal rR, decimal rG, decimal rB)
    {
        decimal _Minimum;
        if (rR < rG)
        {
            if (rR < rB)
            {
                _Minimum = rR;
            }
            else
            {
                _Minimum = rB;
            }
        }
        else
        {
            if (rB < rG)
            {
                _Minimum = rB;
            }
            else
            {
                _Minimum = rG;
            }
        }
        return _Minimum;
    }

    public static void SplitRGB(int lColor, ref int lRed, ref int lGreen, ref int lBlue)
    {
        lRed = lColor & 0xFF;
        lGreen = (lColor & 0xFF00) / 0x100;
        lBlue = (lColor & 0xFF0000) / 0x100 / 0x100;
    }

    public static int TranslateColor(int oClr, int hPal = 0)
    {
        int _TranslateColor = 0;
        if (OleTranslateColor(oClr, hPal, ref _TranslateColor) != 0) _TranslateColor = CLR_INVALID;
        return _TranslateColor;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PixelColor
    {
        // 32 bit BGRA 
        [FieldOffset(0), System.Diagnostics.DebuggerDisplay("{ColorBGRA,h}")] public uint ColorBGRA;
        // 8 bit components
        [FieldOffset(0)] public byte Blue;
        [FieldOffset(1)] public byte Green;
        [FieldOffset(2)] public byte Red;
        [FieldOffset(3)] public byte Alpha;
    }
    public static void CopyPixels(this BitmapSource source, PixelColor[] pixels) => CopyPixels(source, pixels, 0, 0, source.PixelWidth, source.PixelHeight);
    public static void CopyPixels(this BitmapSource source, PixelColor[] pixels, int x, int y, int width, int height)
    {
        byte[] pixelBytes = new byte[height * width * 4];
        source.CopyPixels(new Int32Rect(x, y, width, height), pixelBytes, width * 4, 0);
        for (int i = 0; i < width * height; i++)
        {
            pixels[i] = new PixelColor
            {
                Blue = pixelBytes[i * 4 + 0],
                Green = pixelBytes[i * 4 + 1],
                Red = pixelBytes[i * 4 + 2],
                Alpha = pixelBytes[i * 4 + 3],
            };
        }
    }
    public static BitmapSource Colorize(BitmapSource source, decimal hue, decimal sat = 1)
    {
        int r = 0;
        int g = 0;
        int b = 0;
        decimal h = 0;
        decimal l = 0;
        decimal s = 0;

        if (source.Format != PixelFormats.Bgra32)
            source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

        WriteableBitmap writeable = new(source);
        PixelColor[] pixels = new PixelColor[writeable.PixelWidth * writeable.PixelHeight];
        writeable.CopyPixels(pixels);
        for (int X = 0; X < writeable.PixelWidth; X++)
        {
            for (int Y = 0; Y < writeable.PixelHeight; Y++)
            {
                PixelColor C = pixels[Y * writeable.PixelWidth + X];
                SplitRGB((int)C.ColorBGRA, ref r, ref g, ref b);
                RGBToHLS(r, g, b, ref h, ref s, ref l);
                HLSToRGB(hue, sat, l, ref r, ref g, ref b);
                C.Red = (byte)r;
                C.Green = (byte)g;
                C.Blue = (byte)b;
                pixels[Y * writeable.PixelWidth + X] = C;
            }
        }
        writeable.WritePixels(new Int32Rect(0, 0, writeable.PixelWidth, writeable.PixelHeight), pixels, writeable.BackBufferStride, 0);
        return writeable;
    }

    public static int ChangeBrightness(int lColor, decimal iDelta)
    {
        int r = 0;
        int g = 0;
        int b = 0;
        decimal h = 0;
        decimal l = 0;
        decimal s = 0;
        SplitRGB(lColor, ref r, ref g, ref b);
        RGBToHLS(r, g, b, ref h, ref s, ref l);
        l += iDelta;
        if (l < 0) l = 0;
        if (l > 1) l = 1;
        HLSToRGB(h, s, l, ref r, ref g, ref b);
        return RGB(r, g, b);
    }

    public static void BitBlt(this WriteableBitmap bitmap, int x, int y, int width, int height, BitmapSource source, int sourceX, int sourceY)
    {
        PixelColor[] pixels = new PixelColor[width * height];
        source.CopyPixels(pixels, sourceX, sourceY, width, height);
        bitmap.WritePixels(new Int32Rect(x, y, width, height), pixels, width * 4, 0);
    }

    public static void StretchBlt(this WriteableBitmap bitmap, int x, int y, int width, int height, BitmapSource source, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
    {
        PixelColor[] pixels = new PixelColor[width * height];
        TransformedBitmap transformed = new(new CroppedBitmap(source, new Int32Rect(sourceX, sourceY, sourceWidth, sourceHeight)), new ScaleTransform((double)width / sourceWidth, (double)height / sourceHeight));
        transformed.CopyPixels(pixels);
        bitmap.WritePixels(new Int32Rect(x, y, width, height), pixels, width * 4, 0);
    }
}
