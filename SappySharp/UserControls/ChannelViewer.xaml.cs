using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;

namespace SappySharp.UserControls;

/// <summary>
/// Logique d'interaction pour ChannelViewer.xaml
/// </summary>
public partial class ChannelViewer : UserControl
{
    // ______________
    //|  SAPPY 2006  |
    //|¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    //| Interface code © 2006 by Kyoufu Kawa               |
    //| Player code © 200X by DJ ßouché                    |
    //| In-program graphics by Kyoufu Kawa                 |
    //| Thanks to SomeGuy, Majin Bluedragon and SlimeSmile |
    //|                                                    |
    //| This code is NOT in the Public Domain or whatever. |
    //| At least until Kyoufu Kawa releases it in the PD   |
    //| himself.  Until then, you're not supposed to even  |
    //| HAVE this code unless given to you by Kawa or any  |
    //| other Helmeted Rodent member.                      |
    //|____________________________________________________|
    // _______________________
    //|  Channel bar control  |
    //|¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯|
    //| Done with the ActiveX Control Interface Wizard.    |
    //| Last update: Juli 26th, 2006                       |
    //|____________________________________________________|
    //
    //###########################################################################################

    public ChannelViewer()
    {
        InitializeComponent();
    }

    dynamic m_Velocity = 0;
    dynamic m_Vibrato = 0;
    dynamic m_Pan = 0;

    public int mute
    {
        get => chkMute.IsChecked.GetValueOrDefault() ? 1 : 0;
        set
        {
            if (value == 2) value = 1;
            chkMute.IsChecked = value == 1;
        }
    }

    public string iType { get => (string)lblType.Content; set => lblType.Content = value; }

    public int Location
    {
        get => (int)Conversion.Val(Strings.Replace((string)lblPC.Content, "0x", "&H"));
        set => lblPC.Content = "0x" + Strings.Right("000000" + Conversion.Hex(value), 6);
    }

    public string Delay { get => (string)lblDel.Content; set => lblDel.Content = value; }

    public string patch { get => (string)lblPat.Content; set => lblPat.Content = value; }

    public string volume { get => (string)lblVol.Content; set => lblVol.Content = value; }

    public string Note { get => (string)lblNote.Content; set => lblNote.Content = value; }

    public dynamic Velocity { get => m_Velocity; set => m_Velocity = value; }

    public dynamic Vibrato { get => m_Vibrato; set => m_Vibrato = value; }

    public dynamic pan
    {
        get => m_Pan; set
        {
            m_Pan = value;
            DoMeter();
        }
    }

    private void lblExpand_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if ((string)lblExpand.Content == "6")
        {
            lblExpand.Content = "5";
            Height = 450 / VBExtension.Screen.TwipsPerPixelY;
        }
        else
        {
            lblExpand.Content = "6";
            Height = 246 / VBExtension.Screen.TwipsPerPixelY;
        }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Width = 4815 / VBExtension.Screen.TwipsPerPixelX;
        Height = ((string)lblExpand.Content == "6" ? 246 : 450) / VBExtension.Screen.TwipsPerPixelY;
    }

    public void Expand(bool expandOut)
    {
        if (expandOut)
        {
            lblExpand.Content = "5";
            Height = 450 / VBExtension.Screen.TwipsPerPixelY;
        }
        else
        {
            lblExpand.Content = "6";
            Height = 246 / VBExtension.Screen.TwipsPerPixelY;
        }
    }

    private void DoMeter()
    {
        int left = 0;
        if (m_Pan == 0)
        {
            left = (int)(64 + 128 - Conversion.Val((string)lblVol.Content) / 2);
            shpMeter.Width = Conversion.Val((string)lblVol.Content);
        }
        else if (m_Pan < 0)
        {
            left = (int)(64 + 128 - Conversion.Val((string)lblVol.Content) / 2);
            shpMeter.Width = Conversion.Val((string)lblVol.Content) / 2 + 1;
        }
        else if (m_Pan > 0)
        {
            left = 64 + 128;
            shpMeter.Width = Conversion.Val((string)lblVol.Content) / 2;
        }
        shpMeter.Margin = new(left, shpMeter.Margin.Top, shpMeter.Margin.Right, shpMeter.Margin.Bottom);
        shpMeter.Background = new SolidColorBrush(Color.FromRgb(0, (byte)(32 + Conversion.Val((string)lblVol.Content) / 3), 0));
    }
}
