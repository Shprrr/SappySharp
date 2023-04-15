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
using System.Reflection;
using System.Buffers.Binary;

namespace SappySharp.Classes;

public class cRegistry
{
    // =========================================================
    // Class:    cRegistry
    // Author:   Steve McMahon
    // Date  :   21 Feb 1997

    // A nice class wrapper around the registry functions
    // Allows searching,deletion,modification and addition
    // of Keys or Values.

    // Updated 29 April 1998 for VB5.
    // * Fixed GPF in EnumerateValues
    // * Added support for all registry types, not just strings
    // * Put all declares in local class
    // * Added VB5 Enums
    // * Added CreateKey and DeleteKey methods

    // Updated 2 January 1999
    // * The CreateExeAssociation method failed to set up the
    // association correctly if the optional document icon
    // was not provided.
    // * Added new parameters to CreateExeAssociation to set up
    // other standard handlers: Print, Add, New
    // * Provided the CreateAdditionalEXEAssociations method
    // to allow non-standard menu items to be added (for example,
    // right click on a .VBP file.  VB installs Run and Make
    // menu items).

    // Updated 8 February 2000
    // * Ensure CreateExeAssociation and related items sets up the
    // registry keys in the
    // HKEY_LOCAL_MACHINE\SOFTWARE\Classes
    // branch as well as the HKEY_CLASSES_ROOT branch.

    // Updated 23 January 2004
    // * Added remote registry connection.  Thanks to Yaron Lavi
    // for providing the code.
    // * Fixed problem with saving zero length strings.  Thanks to
    // Shane Marsden for the fix.
    // * Fixed problem with truncation of binary data.  Thanks to
    // Morten Egelund Rasmussen.




    // ---------------------------------------------------------------------------
    // vbAccelerator - free, advanced source code for VB programmers.
    // http://vbaccelerator.com
    // =========================================================

    // Registry Specific Access Rights
    public const int KEY_QUERY_VALUE = 0x1;
    public const int KEY_SET_VALUE = 0x2;
    public const int KEY_CREATE_SUB_KEY = 0x4;
    public const int KEY_ENUMERATE_SUB_KEYS = 0x8;
    public const int KEY_NOTIFY = 0x10;
    public const int KEY_CREATE_LINK = 0x20;
    public const int KEY_ALL_ACCESS = 0x3F;

    // Open/Create Options
    public const int REG_OPTION_NON_VOLATILE = 0;
    public const int REG_OPTION_VOLATILE = 0x1;

    // Key creation/open disposition
    public const int REG_CREATED_NEW_KEY = 0x1;
    public const int REG_OPENED_EXISTING_KEY = 0x2;

    // masks for the predefined standard access types
    public const int STANDARD_RIGHTS_ALL = 0x1F0000;
    public const int SPECIFIC_RIGHTS_ALL = 0xFFFF;

    // Define severity codes
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_INVALID_DATA = 13;
    public const int ERROR_MORE_DATA = 234; // dderror
    public const int ERROR_NO_MORE_ITEMS = 259;


    // Structures Needed For Registry Prototypes
    class SECURITY_ATTRIBUTES
    {
        public int nLength;
        public int lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    class FILETIME
    {
        public int dwLowDateTime;
        public int dwHighDateTime;
    }

    // Registry Function Prototypes
    [DllImport("advapi32", EntryPoint = "RegOpenKeyExA")]
    private static extern int RegOpenKeyEx(int hKey, string lpSubKey, int ulOptions, int samDesired, ref int phkResult);

    [DllImport("advapi32", EntryPoint = "RegSetValueExA")]
    private static extern int RegSetValueExStr(int hKey, string lpValueName, int Reserved, int dwType, string szData, int cbData);
    [DllImport("advapi32", EntryPoint = "RegSetValueExA")]
    private static extern int RegSetValueExLong(int hKey, string lpValueName, int Reserved, int dwType, ref int szData, int cbData);
    [DllImport("advapi32", EntryPoint = "RegSetValueExA")]
    private static extern int RegSetValueExByte(int hKey, string lpValueName, int Reserved, int dwType, ref byte szData, int cbData);

    [DllImport("advapi32")]
    private static extern int RegCloseKey(int hKey);

    [DllImport("advapi32", EntryPoint = "RegQueryValueExA")]
    private static extern int RegQueryValueExStr(int hKey, string lpValueName, int lpReserved, ref int lpType, string szData, ref int lpcbData);
    [DllImport("advapi32", EntryPoint = "RegQueryValueExA")]
    private static extern int RegQueryValueExLong(int hKey, string lpValueName, int lpReserved, ref int lpType, ref int szData, ref int lpcbData);
    [DllImport("advapi32", EntryPoint = "RegQueryValueExA")]
    private static extern int RegQueryValueExByte(int hKey, string lpValueName, int lpReserved, ref int lpType, ref byte szData, ref int lpcbData);

    [DllImport("advapi32", EntryPoint = "RegCreateKeyExA")]
    private static extern int RegCreateKeyEx(int hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, int samDesired, ref SECURITY_ATTRIBUTES lpSecurityAttributes, ref int phkResult, ref int lpdwDisposition);

    // Private Declare Function RegEnumKeyEx Lib "advapi32.dll" Alias "RegEnumKeyExA" (ByVal hKey As Long, ByVal dwIndex As Long, ByVal lpName As String, lpcbName As Long, ByVal lpReserved As Long, ByVal lpClass As String, lpcbClass As Long, lpftLastWriteTime As FILETIME) As Long

    [DllImport("advapi32.dll", EntryPoint = "RegEnumKeyA")]
    private static extern int RegEnumKey(int hKey, int dwIndex, string lpName, int cbName);

    [DllImport("advapi32.dll", EntryPoint = "RegEnumValueA")]
    private static extern int RegEnumValue(int hKey, int dwIndex, string lpValueName, ref int lpcbValueName, int lpReserved, int lpType, int lpData, int lpcbData);

    // Private Declare Function RegEnumValueLong Lib "advapi32.dll" Alias "RegEnumValueA" (ByVal hKey As Long, ByVal dwIndex As Long, ByVal lpValueName As String, lpcbValueName As Long, ByVal lpReserved As Long, lpType As Long, lpData As Long, lpcbData As Long) As Long
    // Private Declare Function RegEnumValueStr Lib "advapi32.dll" Alias "RegEnumValueA" (ByVal hKey As Long, ByVal dwIndex As Long, ByVal lpValueName As String, lpcbValueName As Long, ByVal lpReserved As Long, lpType As Long, ByVal lpData As String, lpcbData As Long) As Long
    // Private Declare Function RegEnumValueByte Lib "advapi32.dll" Alias "RegEnumValueA" (ByVal hKey As Long, ByVal dwIndex As Long, ByVal lpValueName As String, lpcbValueName As Long, ByVal lpReserved As Long, lpType As Long, lpData As Byte, lpcbData As Long) As Long

    [DllImport("advapi32.dll", EntryPoint = "RegConnectRegistryA")]
    private static extern int RegConnectRegistry(string lpMachineName, int hKey, ref int phKey);

    [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKeyA")]
    private static extern int RegQueryInfoKey(int hKey, string lpClass, ref int lpcbClass, int lpReserved, ref int lpcSubKeys, ref int lpcbMaxSubKeyLen, ref int lpcbMaxClassLen, ref int lpcValues, ref int lpcbMaxValueNameLen, ref int lpcbMaxValueLen, ref int lpcbSecurityDescriptor, ref int lpftLastWriteTime);

    [DllImport("advapi32.dll", EntryPoint = "RegDeleteKeyA")]
    private static extern int RegDeleteKey(int hKey, string lpSubKey);

    [DllImport("advapi32.dll", EntryPoint = "RegDeleteValueA")]
    private static extern int RegDeleteValue(int hKey, string lpValueName);

    // Other declares:
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(ref dynamic lpvDest, ref dynamic lpvSource, int cbCopy);
    [DllImport("kernel32", EntryPoint = "ExpandEnvironmentStringsA")]
    private static extern int ExpandEnvironmentStrings(string lpSrc, string lpDst, int nSize);


    public enum ERegistryClassConstants : uint
    {
        HKEY_CLASSES_ROOT = 0x80000000
    , HKEY_CURRENT_USER = 0x80000001
    , HKEY_LOCAL_MACHINE = 0x80000002
    , HKEY_USERS = 0x80000003
    }

    public enum ERegistryValueTypes
    {
        // Predefined Value Types
        REG_NONE = 0 // No value type
    , REG_SZ = 1 // Unicode nul terminated string
    , REG_EXPAND_SZ = 2 // Unicode nul terminated string w/enviornment var
    , REG_BINARY = 3 // Free form binary
    , REG_DWORD = 4 // 32-bit number
    , REG_DWORD_LITTLE_ENDIAN = 4 // 32-bit number (same as REG_DWORD)
    , REG_DWORD_BIG_ENDIAN = 5 // 32-bit number
    , REG_LINK = 6 // Symbolic Link (unicode)
    , REG_MULTI_SZ = 7 // Multiple Unicode strings
    , REG_RESOURCE_LIST = 8 // Resource list in the resource map
    , REG_FULL_RESOURCE_DESCRIPTOR = 9 // Resource list in the hardware description
    , REG_RESOURCE_REQUIREMENTS_LIST = 10
    }

    public static int m_hClassKey;
    public static string m_sSectionKey = "";
    public static string m_sValueKey = "";
    public static dynamic m_vValue = null;
    // Private m_sSetValue            As String
    public static dynamic m_vDefault = null;
    public static ERegistryValueTypes m_eValueType;
    public static string m_sMachine = "";

    public string Machine
    {
        get => m_sMachine;
        set
        {
            if (Len(value) < 3)
            {
                m_sMachine = "";
            }
            else if (Left(value, 2) != "\\\\")
            {
                m_sMachine = "\\\\" + value;
            }
            else
            {
                m_sMachine = value;
            }
        }
    }

    public bool KeyExists
    {
        get
        {
            bool _KeyExists;
            int hKey = 0;

            if (RegOpenKeyEx(m_hClassKey, m_sSectionKey, 0, 1, ref hKey) == ERROR_SUCCESS)
            {
                _KeyExists = true;
                RegCloseKey(hKey);
            }
            else
            {
                _KeyExists = false;
            }

            return _KeyExists;
        }
    }


    public bool CreateKey()
    {
        bool _CreateKey = false;
        SECURITY_ATTRIBUTES tSA = null;
        int hKey = 0;
        int lCreate = 0;

        // Open or Create the key
        int e = RegCreateKeyEx(m_hClassKey, m_sSectionKey, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, ref tSA, ref hKey, ref lCreate);
        if (e != 0)
        {
            Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to create registry Key: '" + m_sSectionKey);
        }
        else
        {
            _CreateKey = e == ERROR_SUCCESS;
            // Close the key
            RegCloseKey(hKey);
        }
        return _CreateKey;
    }

    public bool DeleteKey()
    {
        bool _DeleteKey = false;

        int e = RegDeleteKey(m_hClassKey, m_sSectionKey);
        if (e != 0)
        {
            Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to delete registry Key: '" + m_hClassKey + "',Section: '" + m_sSectionKey);
        }
        else
        {
            _DeleteKey = e == ERROR_SUCCESS;
        }

        return _DeleteKey;
    }

    public bool DeleteValue()
    {
        bool _DeleteValue = false;
        int hKey = 0;

        int e = RegOpenKeyEx(m_hClassKey, m_sSectionKey, 0, KEY_ALL_ACCESS, ref hKey);
        if (e != 0)
        {
            Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to open key '" + m_hClassKey + "',Section: '" + m_sSectionKey + "' for delete access");
        }
        else
        {
            e = RegDeleteValue(hKey, m_sValueKey);
            if (e != 0)
            {
                Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to delete registry Key: '" + m_hClassKey + "',Section: '" + m_sSectionKey + "',Key: '" + m_sValueKey);
            }
            else
            {
                _DeleteValue = e == ERROR_SUCCESS;
            }
        }
        return _DeleteValue;
    }

    private dynamic _Value = default;
    public dynamic Value
    {
        get
        {
            dynamic vValue;
            int cData = 0;
            string sData;
            int ordType = 0;
            int e;
            int hKey = 0;

            if (m_sMachine != "")
            {
                e = RegConnectRegistry(m_sMachine, m_hClassKey, ref hKey);
                if (e != 0)
                {
                    _Value = m_vDefault;
                    return _Value;
                }
            }

            RegOpenKeyEx(m_hClassKey, m_sSectionKey, 0, KEY_QUERY_VALUE, ref hKey);

            int szData = 0;
            e = RegQueryValueExLong(hKey, m_sValueKey, 0, ref ordType, ref szData, ref cData);
            if (e != 0 && e != ERROR_MORE_DATA)
            {
                _Value = m_vDefault;
                return _Value;
            }

            m_eValueType = (ERegistryValueTypes)ordType;
            switch (m_eValueType)
            {
                case ERegistryValueTypes.REG_DWORD:
                    int iData = 0;
                    RegQueryValueExLong(hKey, m_sValueKey, 0, ref ordType, ref iData, ref cData);
                    vValue = CLng(iData);
                    break;

                case ERegistryValueTypes.REG_DWORD_BIG_ENDIAN:  // Unlikely, but you never know
                    int dwData = 0;
                    RegQueryValueExLong(hKey, m_sValueKey, 0, ref ordType, ref dwData, ref cData);
                    vValue = SwapEndian(dwData);
                    break;

                case ERegistryValueTypes.REG_SZ:
                case ERegistryValueTypes.REG_MULTI_SZ:  // Same thing to Visual Basic
                    sData = new string('\0', cData - 1);
                    RegQueryValueExStr(hKey, m_sValueKey, 0, ref ordType, sData, ref cData);
                    vValue = sData;
                    break;

                case ERegistryValueTypes.REG_EXPAND_SZ:
                    sData = new string('\0', cData - 1);
                    RegQueryValueExStr(hKey, m_sValueKey, 0, ref ordType, sData, ref cData);
                    vValue = ExpandEnvStr(ref sData);
                    break;

                // Catch REG_BINARY and anything else
                default:
                    byte bData = default;
                    RegQueryValueExByte(hKey, m_sValueKey, 0, ref ordType, ref bData, ref cData);
                    vValue = new List<byte> { bData };
                    break;
            }
            _Value = vValue;

            return _Value;
        }
        set
        {
            int ordType;
            int c;
            int hKey = 0;
            int e;
            int lCreate = 0;
            SECURITY_ATTRIBUTES tSA = null;

            // If a remote machine (m_sMachine<>"") then try to connect to the remote registry:
            if (m_sMachine != "")
            {
                e = RegConnectRegistry(m_sMachine, m_hClassKey, ref hKey);
                if (e != 0)
                {
                    _Value = m_vDefault;
                    return;
                }
            }

            // Open or Create the key
            e = RegCreateKeyEx(m_hClassKey, m_sSectionKey, 0, "", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, ref tSA, ref hKey, ref lCreate);

            if (e != 0)
            {
                Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to set registry value Key: '" + m_hClassKey + "',Section: '" + m_sSectionKey + "',Key: '" + m_sValueKey + "' to value: '" + m_vValue + "'");
            }
            else
            {
                switch (m_eValueType)
                {
                    case ERegistryValueTypes.REG_BINARY:
                        if (VarType(value) == vbArray | vbByte)
                        {
                            List<byte> ab;
                            ab = value;
                            ordType = (int)ERegistryValueTypes.REG_BINARY;
                            c = ab.Count - 0 + 1; // Bugfix by Morten Egelund Rasmussen (Dec. 11/2003)
                            byte bData = ab[0];
                            e = RegSetValueExByte(hKey, m_sValueKey, 0, ordType, ref bData, c);
                        }
                        else
                        {
                            Err().Raise(26001);
                        }
                        break;

                    case ERegistryValueTypes.REG_DWORD:
                    case ERegistryValueTypes.REG_DWORD_BIG_ENDIAN:
                        if (VarType(value) == vbInteger || VarType(value) == vbLong)
                        {
                            int i = value;
                            ordType = (int)ERegistryValueTypes.REG_DWORD;
                            e = RegSetValueExLong(hKey, m_sValueKey, 0, ordType, ref i, 4);
                        }
                        break;

                    case ERegistryValueTypes.REG_SZ:
                    case ERegistryValueTypes.REG_EXPAND_SZ:
                        string s = value;
                        ordType = (int)ERegistryValueTypes.REG_SZ;
                        // Assume anything with two non-adjacent percents is expanded string
                        int iPos = InStr(s, "%");
                        if (iPos != 0)
                        {
                            if (InStr(iPos + 2, s, "%") != 0) ordType = (int)ERegistryValueTypes.REG_EXPAND_SZ;
                        }
                        c = Len(s) + 1;
                        s += vbNullChar; // Thanks to Shane Marsden
                        e = RegSetValueExStr(hKey, m_sValueKey, 0, ordType, s, c);
                        break;

                    // User should convert to a compatible type before calling
                    default:
                        e = ERROR_INVALID_DATA;
                        break;
                }

                if (e == 0)
                {
                    m_vValue = value;
                }
                else
                {
                    Err().Raise(vbObjectError + 1048 + 26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to set registry value Key: '" + m_hClassKey + "',Section: '" + m_sSectionKey + "',Key: '" + m_sValueKey + "' to value: '" + m_vValue + "'");
                }

                // Close the key
                RegCloseKey(hKey);
            }
        }
    }

    public bool EnumerateValues(ref List<string> sKeyNames, ref int iKeyCount)
    {
        int hKey = 0;
        int lNameSize = 0;
        int cJunk = 0;
        int cNameMax = 0;
        int ft = 0;

        // Log "EnterEnumerateValues"

        iKeyCount = 0;
        sKeyNames.Clear();

        int lIndex = 0;
        int lResult = RegOpenKeyEx(m_hClassKey, m_sSectionKey, 0, KEY_QUERY_VALUE, ref hKey);
        if (lResult == ERROR_SUCCESS)
        {
            // Log "OpenedKey:" & m_hClassKey & "," & m_sSectionKey
            lResult = RegQueryInfoKey(hKey, "", ref cJunk, 0, ref cJunk, ref cJunk, ref cJunk, ref cJunk, ref cNameMax, ref cJunk, ref cJunk, ref ft);
            while (lResult == ERROR_SUCCESS)
            {
                // Set buffer space
                lNameSize = cNameMax + 1;
                string sName = new('\0', lNameSize);
                if (lNameSize == 0) lNameSize = 1;

                // Log "Requesting Next Value"

                // Get value name:
                lResult = RegEnumValue(hKey, lIndex, sName, ref lNameSize, 0, 0, 0, 0);
                // Log "RegEnumValue returned:" & lResult
                if (lResult == ERROR_SUCCESS)
                {
                    // Although in theory you can also retrieve the actual
                    // value and type here, I found it always (ultimately) resulted in
                    // a GPF, on Win95 and NT.  Why?  Can anyone help?

                    sName = Left(sName, lNameSize);
                    // Log "Enumerated value:" & sName

                    iKeyCount++;
                    // TODO: (NOT SUPPORTED): ReDim Preserve sKeyNames(1 To iKeyCount) As String
                    sKeyNames[iKeyCount] = sName;
                }
                lIndex++;
            }
        }
        if (hKey != 0)
        {
            RegCloseKey(hKey);
        }

        // Log "Exit Enumerate Values"
        return true;

    EnumerateValuesError:;
        if (hKey != 0)
        {
            RegCloseKey(hKey);
        }
        Err().Raise(vbObjectError + 1048 + 26003, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", Err().Description);
        return false;
    }

    public bool EnumerateSections(ref List<string> sSect, ref int iSectCount)
    {
        int hKey = 0;

        // TODO: (NOT SUPPORTED): On Error GoTo EnumerateSectionsError

        iSectCount = 0;
        sSect.Clear();

        int lIndex = 0;

        int lResult = RegOpenKeyEx(m_hClassKey, m_sSectionKey, 0, KEY_ENUMERATE_SUB_KEYS, ref hKey);
        while (lResult == ERROR_SUCCESS)
        {
            // Set buffer space
            string szBuffer = new('\0', 255);
            int lBuffSize = Len(szBuffer);

            // Get next value
            lResult = RegEnumKey(hKey, lIndex, szBuffer, lBuffSize);

            if (lResult == ERROR_SUCCESS)
            {
                iSectCount++;
                // TODO: (NOT SUPPORTED): ReDim Preserve sSect(1 To iSectCount) As String
                int iPos = InStr(szBuffer, "\0");
                if (iPos > 0)
                {
                    sSect[iSectCount] = Left(szBuffer, iPos - 1);
                }
                else
                {
                    sSect[iSectCount] = Left(szBuffer, lBuffSize);
                }
            }

            lIndex++;
        }
        if (hKey != 0)
        {
            RegCloseKey(hKey);
        }

        return true;

    EnumerateSectionsError:;
        if (hKey != 0)
        {
            RegCloseKey(hKey);
        }
        Err().Raise(vbObjectError + 1048 + 26002, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", Err().Description);
        return false;
    }

    private void pSetClassValue(string sValue)
    {
        ClassKey = ERegistryClassConstants.HKEY_CLASSES_ROOT;
        Value = sValue;
        string sSection = SectionKey;
        ClassKey = ERegistryClassConstants.HKEY_LOCAL_MACHINE;
        SectionKey = "SOFTWARE\\Classes\\" + sSection;
        Value = sValue;
        SectionKey = sSection;
    }

    public void CreateEXEAssociation(string sExePath, string sClassName, string sClassDescription, string sAssociation, string sOpenMenuText = "&Open", bool bSupportPrint = false, string sPrintMenuText = "&Print", bool bSupportNew = false, string sNewMenuText = "&New", bool bSupportInstall = false, string sInstallMenuText = "", int lDefaultIconIndex = -1)
    {
        // Check if path is wrapped in quotes:
        sExePath = Trim(sExePath);
        if (Left(sExePath, 1) != "\"")
        {
            sExePath = "\"" + sExePath;
        }
        if (Right(sExePath, 1) != "\"")
        {
            sExePath += "\"";
        }

        // Create the .File to Class association:
        SectionKey = "." + sAssociation;
        ValueType = ERegistryValueTypes.REG_SZ;
        ValueKey = "";
        pSetClassValue(sClassName);

        // Create the Class shell open command:
        SectionKey = sClassName;
        pSetClassValue(sClassDescription);

        SectionKey = sClassName + "\\shell\\open";
        if (sOpenMenuText == "") sOpenMenuText = "&Open";
        ValueKey = "";
        pSetClassValue(sOpenMenuText);
        SectionKey = sClassName + "\\shell\\open\\command";
        ValueKey = "";
        pSetClassValue(sExePath + " \"%1\"");

        if (bSupportPrint)
        {
            SectionKey = sClassName + "\\shell\\print";
            if (sPrintMenuText == "") sPrintMenuText = "&Print";
            ValueKey = "";
            pSetClassValue(sPrintMenuText);
            SectionKey = sClassName + "\\shell\\print\\command";
            ValueKey = "";
            pSetClassValue(sExePath + " /p \"%1\"");
        }

        if (bSupportInstall)
        {
            if (sInstallMenuText == "")
            {
                sInstallMenuText = "&Install " + sAssociation;
            }
            SectionKey = sClassName + "\\shell\\add";
            ValueKey = "";
            pSetClassValue(sInstallMenuText);
            SectionKey = sClassName + "\\shell\\add\\command";
            ValueKey = "";
            pSetClassValue(sExePath + " /a \"%1\"");
        }

        if (bSupportNew)
        {
            SectionKey = sClassName + "\\shell\\new";
            ValueKey = "";
            if (sNewMenuText == "") sNewMenuText = "&New";
            pSetClassValue(sNewMenuText);
            SectionKey = sClassName + "\\shell\\new\\command";
            ValueKey = "";
            pSetClassValue(sExePath + " /n \"%1\"");
        }

        if (lDefaultIconIndex > -1)
        {
            SectionKey = sClassName + "\\DefaultIcon";
            ValueKey = "";
            pSetClassValue(sExePath + "," + CStr(lDefaultIconIndex));
        }
    }

    public void CreateAdditionalEXEAssociations(string sClassName, ref List<dynamic> vItems)
    {
        // TODO: (NOT SUPPORTED): On Error Resume Next

        int iItems = vItems.Count + 1;
        if (iItems % 3 != 0 || Err().Number != 0)
        {
            Err().Raise(vbObjectError + 1048 + 26004, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Invalid parameter list passed to CreateAdditionalEXEAssociations - expected Name/Text/Command");
        }
        else
        {
            // Check if it exists:
            SectionKey = sClassName;
            if (!KeyExists)
            {
                Err().Raise(vbObjectError + 1048 + 26005, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Error - attempt to create additional associations before class defined.");
            }
            else
            {
                for (int iItem = 0; iItem >= 3; iItem += iItems - 1)
                {
                    ValueType = ERegistryValueTypes.REG_SZ;
                    SectionKey = sClassName + "\\shell\\" + vItems[iItem];
                    ValueKey = "";
                    pSetClassValue(vItems[iItem + 1]);
                    SectionKey = sClassName + "\\shell\\" + vItems[iItem] + "\\command";
                    ValueKey = "";
                    pSetClassValue(vItems[iItem + 2]);
                }
            }
        }
    }

    public ERegistryValueTypes ValueType
    {
        get => m_eValueType;
        set => m_eValueType = value;
    }

    public ERegistryClassConstants ClassKey
    {
        get => (ERegistryClassConstants)m_hClassKey;
        set => m_hClassKey = (int)value;
    }

    public string SectionKey
    {
        get => m_sSectionKey;
        set => m_sSectionKey = value;
    }

    public string ValueKey
    {
        get => m_sValueKey;
        set => m_sValueKey = value;
    }

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // A Wrapper Property for the VALUE property .
    // Assign all passed values to the class properties and call
    // ME.VALUE to return value
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    public dynamic GetValueEx(ERegistryClassConstants ClassKey, string SectionKey, string ValueKey, ERegistryValueTypes ValueType, dynamic Default)
    {
        this.ClassKey = ClassKey;
        this.SectionKey = SectionKey;
        this.ValueKey = ValueKey;
        this.ValueType = ValueType;
        this.Default = Default;
        return Value;
    }
    public void SetValueEx(ERegistryClassConstants ClassKey, string SectionKey, string ValueKey, ERegistryValueTypes ValueType, dynamic Default, dynamic NewValue)
    {
        this.ClassKey = ClassKey;
        this.SectionKey = SectionKey;
        this.ValueKey = ValueKey;
        this.ValueType = ValueType;
        this.Default = Default;

        if (this.ValueType == ERegistryValueTypes.REG_SZ)
        {
            if (CStr(NewValue) == "")
            {
                NewValue = Default;
            }
        }
        Value = NewValue;
    }

    public dynamic Default
    {
        get => m_vDefault;
        set => m_vDefault = value;
    }

    private int SwapEndian(int dw)
    {
        return BinaryPrimitives.ReverseEndianness(dw);
    }

    private string ExpandEnvStr(ref string sData)
    {
        int c = 0;

        // Get the length
        string s = ""; // Needed to get around Windows 95 limitation
        c = ExpandEnvironmentStrings(sData, s, c);

        // Expand the string
        s = new string('\0', c - 1);
        ExpandEnvironmentStrings(sData, s, c);

        return s;
    }
}
