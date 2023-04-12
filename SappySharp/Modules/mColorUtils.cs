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



static class mColorUtils {


[DllImport("OLEPRO32.DLL")]
private static extern int OleTranslateColor(int OLE_COLOR, int HPALETTE, ref int pccolorref);
int CLR_INVALID = -1;

  public static void RGBToHLS(int r, int g, int b, ref decimal h, ref decimal s, ref decimal l) {
  decimal Max = 0;
decimal Min = 0;
decimal delta = 0;
decimal rR = 0;
decimal rG = 0;
decimal rB = 0;
  rR = r / 255;rG = g / 255;rB = b / 255;
  Max = Maximum(rR, rG, rB);
  Min = Minimum(rR, rG, rB);
  l = (Max + Min) / 2;
    if(Max == Min) {
    s = 0;
    h = 0;
    } else {
      if(l <= 0.5m) {
      s = (Max - Min) / (Max + Min);
      } else {
      s = (Max - Min) / (2 - Max - Min);
    }
    delta = Max - Min;
      if(rR == Max) {
      h = (rG - rB) / delta;
      } else if(rG == Max) {
      h = 2 + (rB - rR) / delta;
      } else if(rB == Max) {
      h = 4 + (rR - rG) / delta;
    }
  }
}

  public static void HLSToRGB(decimal h, decimal s, decimal l, ref int r, ref int g, ref int b) {
  decimal rR = 0;
decimal rG = 0;
decimal rB = 0;
decimal Min = 0;
decimal Max = 0;
    if(s == 0) {
    rR = l;rG = l;rB = l;
    } else {
      if(l <= 0.5m) {
      Min = l * (1 - s);
      } else {
      Min = l - s * (1 - l);
    }
    Max = 2 * l - Min;
      if((h < 1)) {
      rR = Max;
        if((h < 0)) {
        rG = Min;
        rB = rG - h * (Max - Min);
        } else {
        rB = Min;
        rG = h * (Max - Min) + rB;
      }
      } else if((h < 3)) {
      rG = Max;
        if((h < 2)) {
        rB = Min;
        rR = rB - (h - 2) * (Max - Min);
        } else {
        rR = Min;
        rB = (h - 2) * (Max - Min) + rR;
      }
      } else {
      rB = Max;
        if((h < 4)) {
        rR = Min;
        rG = rR - (h - 4) * (Max - Min);
        } else {
        rG = Min;
        rR = (h - 4) * (Max - Min) + rG;
      }
    }
  }
  r = rR * 255;g = rG * 255;b = rB * 255;
}

  private static decimal Maximum(ref decimal rR, ref decimal rG, ref decimal rB) {
decimal _Maximum = 0;
    if((rR > rG)) {
      if((rR > rB)) {
      _Maximum = rR;
      } else {
      _Maximum = rB;
    }
    } else {
      if((rB > rG)) {
      _Maximum = rB;
      } else {
      _Maximum = rG;
    }
  }
return _Maximum;
}
  private static decimal Minimum(ref decimal rR, ref decimal rG, ref decimal rB) {
decimal _Minimum = 0;
    if((rR < rG)) {
      if((rR < rB)) {
      _Minimum = rR;
      } else {
      _Minimum = rB;
    }
    } else {
      if((rB < rG)) {
      _Minimum = rB;
      } else {
      _Minimum = rG;
    }
  }
return _Minimum;
}

  public static void SplitRGB(ref int lColor, ref int lRed, ref int lGreen, ref int lBlue) {
  lRed = (lColor && 0xFF);
  lGreen = (lColor && 0xFF00) / 0x100;
  lBlue = (lColor && 0xFF0000) / 0x100 / 0x100;
}

  public static int TranslateColor(OLE_COLOR oClr, ref int hPal = 0) {
int _TranslateColor = 0;
  if(OleTranslateColor(oClr, hPal, _TranslateColor))_TranslateColor = CLR_INVALID;
return _TranslateColor;
}

  public static void SplitTest(ref int lColor) {
  int lRed = 0;
int lGreen = 0;
int lBlue = 0;
  lRed = (lColor && 0xFF);
  lGreen = (lColor && 0xFF00) / 0x100;
  lBlue = (lColor && 0xFF0000) / 0x100 / 0x100;
}

  public static void Colorize(ref PictureBox Victim, ref decimal hue, ref decimal sat = 1) {
  int X = 0;
  int Y = 0;
  int r = 0;
  int g = 0;
  int b = 0;
  decimal h = 0;
  decimal l = 0;
  decimal s = 0;
  int C = 0;
  
    for (X = 0; X <= Victim.ScaleWidth; X += 1) {
      for (Y = 0; Y <= Victim.ScaleHeight; Y += 1) {
       // c = Victim.point(x, y)
      C = GetPixel(Victim.hdc, X, Y);
      SplitRGB(ref C, ref r, ref g, ref b);
      RGBToHLS(r, g, b, ref h, ref s, ref l);
      HLSToRGB(hue, sat, l, ref r, ref g, ref b);
      C = RGB(r, g, b);
      SetPixel(Victim.hdc, X, Y, C);
       // Victim.PSet (x, y), c
    }
  }
}

  public static int ChangeBrightness(ref int lColor, ref decimal iDelta) {
int _ChangeBrightness = 0;
  int X = 0;
  int Y = 0;
  int r = 0;
  int g = 0;
  int b = 0;
  decimal h = 0;
  decimal l = 0;
  decimal s = 0;
  SplitRGB(ref lColor, ref r, ref g, ref b);
  RGBToHLS(r, g, b, ref h, ref s, ref l);
  l = l + iDelta;
  if(l < 0)l = 0;
  if(l > 1)l = 1;
  HLSToRGB(h, s, l, ref r, ref g, ref b);
  _ChangeBrightness = RGB(r, g, b);
return _ChangeBrightness;
}



}