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
using System.Diagnostics;
using SappySharp;

static partial class mTrace
{
    [DllImport("user32", EntryPoint = "GetPropA")]
    private static extern int GetProp(int hwnd, string lpString);
    private delegate int EnumWindowsDelegate(int hwnd, int lParam);
    [LibraryImport("user32")]
    private static partial int EnumWindows(EnumWindowsDelegate lpEnumFunc, int lParam);
    [DllImport("user32", EntryPoint = "SendMessageTimeoutA")]
    private static extern int SendMessageTimeout(int hwnd, int msg, int wParam, IntPtr lParam, int fuFlags, int uTimeout, ref int lpdwResult);
    public const int SMTO_NORMAL = 0x0;
    public const int WM_COPYDATA = 0x4A;
    public class COPYDATASTRUCT
    {
        public int dwData;
        public int cbData;
        public int lpData;
    }
    public const string THISAPPID = "vbAcceleratorVBTRACER";


    public static int m_hWnd = 0;
    public static bool m_bInitialised = false;

#if TRACEMODE
    public static void Trace(params dynamic[] args)
    {
        if (DoTrace())
        {
            SendTraceMessage(args);
        }
    }

    public static void Assert(bool condition, params dynamic[] args)
    {
        if (!(m_hWnd == 0))
        {
            Debug.Assert(condition);
            SendTraceMessage(args, "Assert Failed");
        }
    }

    private static bool DoTrace()
    {
        bool _DoTrace = false;
        if (!m_bInitialised)
        {
            FindTraceWindow();
            m_bInitialised = true;
        }
        _DoTrace = !(m_hWnd == 0);
        return _DoTrace;
    }

    private static void SendTraceMessage(params dynamic[] args)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next
        int i = 0;
        int j = 0;
        string sPrint = "";
        for (i = 0; i <= args.Length; i += 1)
        {
            if ((VarType(args[i]) && vbArray) == vbArray)
            {
                for (j = 0; j <= args[i].Count; j += 1)
                {
                    sPrint = sPrint + args[i](j) + vbTab;
                }
            }
            else
            {
                sPrint = sPrint + args[i] + vbTab;
            }
        }
        sPrint = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ": " + Marshal.GetHINSTANCE(typeof(App).Module) + ": " + Environment.CurrentManagedThreadId + ": " + Format(DateTime.Now, "yyyymmdd hhnnss") + ": " + sPrint;

        COPYDATASTRUCT tCDS = new();
        int lR = 0;
        byte[] b = Encoding.Unicode.GetBytes(sPrint);
        tCDS.dwData = 0;
        tCDS.cbData = b.Length + 1;
        var gh = GCHandle.Alloc(b[0], GCHandleType.Pinned);
        tCDS.lpData = (int)gh.AddrOfPinnedObject();
        var ghCDS = GCHandle.Alloc(tCDS, GCHandleType.Pinned);

        // Give in if not response
        lR = SendMessageTimeout(m_hWnd, WM_COPYDATA, 0, ghCDS.AddrOfPinnedObject(), SMTO_NORMAL, 5000, ref lR);
        gh.Free();
        ghCDS.Free();
    }

    private static int FindTraceWindow()
    {
        int _FindTraceWindow = 0;
        // Enumerate top-level windows:
        m_hWnd = 0;
        EnumWindows(EnumWindowsProc, 0);
        return _FindTraceWindow;
    }
    private static int EnumWindowsProc(int hwnd, int lParam)
    {
        int _EnumWindowsProc = 0;
        // Customised windows enumeration procedure.  Stops
        // when it finds another application with the Window
        // property set, or when all windows are exhausted.
        if (IsTraceWindow(hwnd))
        {
            _EnumWindowsProc = 0;
            m_hWnd = hwnd;
        }
        else
        {
            _EnumWindowsProc = 1;
        }
        return _EnumWindowsProc;
    }

    private static bool IsTraceWindow(int hwnd)
    {
        bool _IsTraceWindow = false;
        _IsTraceWindow = GetProp(hwnd, THISAPPID + "_TRACEWIN") == 1;
        return _IsTraceWindow;
    }
#else
    public static void Trace(params dynamic[] args)
    {

    }
    public static void Assert(bool condition)
    {

    }
#endif
}
