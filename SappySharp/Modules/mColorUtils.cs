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

    public static void SplitTest(int lColor)
    {
        int lRed = lColor & 0xFF;
        int lGreen = (lColor & 0xFF00) / 0x100;
        int lBlue = (lColor & 0xFF0000) / 0x100 / 0x100;
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
    public static void CopyPixels(this BitmapSource source, PixelColor[] pixels)
    {
        int height = source.PixelHeight;
        int width = source.PixelWidth;
        byte[] pixelBytes = new byte[height * width * 4];
        source.CopyPixels(pixelBytes, width * 4, 0);
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[y * width + x] = new PixelColor
                {
                    Blue = pixelBytes[(y * width + x) * 4 + 0],
                    Green = pixelBytes[(y * width + x) * 4 + 1],
                    Red = pixelBytes[(y * width + x) * 4 + 2],
                    Alpha = pixelBytes[(y * width + x) * 4 + 3],
                };
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
}
