using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static VBExtension;

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

    // Other declares:
    [DllImport("kernel32", EntryPoint = "ExpandEnvironmentStringsA")]
    private static extern int ExpandEnvironmentStrings(string lpSrc, string lpDst, int nSize);


    public const RegistryValueKind RegistryValueKindDWordBigEndian = (RegistryValueKind)5;

    private string _sMachine = "";

    public string Machine
    {
        get => _sMachine;
        set
        {
            if (Len(value) < 3)
            {
                _sMachine = "";
            }
            else if (Left(value, 2) != "\\\\")
            {
                _sMachine = "\\\\" + value;
            }
            else
            {
                _sMachine = value;
            }
        }
    }

    public bool KeyExists
    {
        get
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
            RegistryKey subKey = registryKey.OpenSubKey(SectionKey);

            if (subKey != null)
            {
                subKey.Close();
                return true;
            }
            else
                return false;
        }
    }


    public bool CreateKey()
    {
        // Open or Create the key
        RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
        RegistryKey subKey = registryKey.CreateSubKey(SectionKey);

        // Close the key
        subKey.Close();
        return true;
    }

    public bool DeleteKey()
    {
        RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
        registryKey.DeleteSubKey(SectionKey);
        return true;
    }

    public bool DeleteValue()
    {
        RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
        RegistryKey subKey = registryKey.OpenSubKey(SectionKey);
        if (subKey == null)
        {
            Err().Raise(26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to open key '" + ClassKey + "',Section: '" + SectionKey + "' for delete access");
        }
        else
            subKey.DeleteValue(ValueKey);

        return true;
    }

    public dynamic Value
    {
        get
        {
            RegistryKey registryKey;

            if (Machine != "")
                registryKey = RegistryKey.OpenRemoteBaseKey(ClassKey, Machine);
            else
                registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
            RegistryKey subKey = registryKey.OpenSubKey(SectionKey);

            object value = subKey?.GetValue(ValueKey);
            if (value == null)
                return Default;

            ValueType = subKey.GetValueKind(ValueKey);
            return ValueType switch
            {
                RegistryValueKind.DWord => value,
                // Unlikely, but you never know
                RegistryValueKindDWordBigEndian => (dynamic)SwapEndian((int)value),
                RegistryValueKind.String or RegistryValueKind.MultiString => value,
                RegistryValueKind.ExpandString => ExpandEnvStr((string)value),
                // Catch REG_BINARY and anything else
                _ => value,
            };
        }
        set
        {
            RegistryKey registryKey;

            // If a remote machine (m_sMachine<>"") then try to connect to the remote registry:
            if (Machine != "")
                registryKey = RegistryKey.OpenRemoteBaseKey(ClassKey, Machine);
            else
                // Open or Create the key
                registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
            RegistryKey subKey = registryKey.CreateSubKey(SectionKey);

            switch (ValueType)
            {
                case RegistryValueKind.Binary:
                    if (VarType(value) == vbArray | vbByte)
                    {
                        subKey.SetValue(ValueKey, value, RegistryValueKind.Binary);
                    }
                    else
                    {
                        Err().Raise(26001);
                    }
                    break;

                case RegistryValueKind.DWord:
                case RegistryValueKindDWordBigEndian:
                    if (VarType(value) == vbInteger || VarType(value) == vbLong)
                        subKey.SetValue(ValueKey, (int)value, RegistryValueKind.DWord);
                    break;

                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    string s = value;
                    RegistryValueKind kind = RegistryValueKind.String;
                    // Assume anything with two non-adjacent percents is expanded string
                    int iPos = InStr(s, "%");
                    if (iPos != 0 && InStr(iPos + 2, s, "%") != 0)
                        kind = RegistryValueKind.ExpandString;
                    subKey.SetValue(ValueKey, s, kind);
                    break;

                // User should convert to a compatible type before calling
                default:
                    Err().Raise(vbObjectError + 1048 + 26001, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", "Failed to set registry value Key: '" + ClassKey + "',Section: '" + SectionKey + "',Key: '" + ValueKey + "' to value: '" + value + "'");
                    break;
            }

            // Close the key
            subKey.Close();
        }
    }

    public bool EnumerateValues(List<string> sKeyNames, out int iKeyCount)
    {
        // Log "EnterEnumerateValues"

        iKeyCount = 0;
        sKeyNames.Clear();

        RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
        RegistryKey subKey = registryKey.OpenSubKey(SectionKey);
        if (subKey != null)
        {
            // Log "OpenedKey:" & m_hClassKey & "," & m_sSectionKey

            foreach (string sName in subKey.GetSubKeyNames())
            {
                // Log "Enumerated value:" & sName

                iKeyCount++;
                sKeyNames.Add(sName);
            }
            subKey.Close();
        }

        // Log "Exit Enumerate Values"
        return true;
    }

    public bool EnumerateSections(List<string> sSect, out int iSectCount)
    {
        iSectCount = 0;
        sSect.Clear();

        RegistryKey subKey = null;
        try
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(ClassKey, RegistryView.Default);
            subKey = registryKey.OpenSubKey(SectionKey);
            if (subKey != null)
            {
                foreach (string name in subKey.GetSubKeyNames())
                {
                    iSectCount++;
                    sSect.Add(name);
                }
                subKey.Close();
            }
        }
        catch (Exception)
        {
            subKey?.Close();
            Err().Raise(vbObjectError + 1048 + 26002, System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location) + ".cRegistry", Err().Description);
            return false;
        }
        return true;
    }

    private void pSetClassValue(string sValue)
    {
        ClassKey = RegistryHive.ClassesRoot;
        Value = sValue;
        string sSection = SectionKey;
        ClassKey = RegistryHive.LocalMachine;
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
        ValueType = RegistryValueKind.String;
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

    public void CreateAdditionalEXEAssociations(string sClassName, List<dynamic> vItems)
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
                    ValueType = RegistryValueKind.String;
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

    public RegistryValueKind ValueType { get; set; }

    public RegistryHive ClassKey { get; set; }

    public string SectionKey { get; set; } = "";

    public string ValueKey { get; set; } = "";

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // A Wrapper Property for the VALUE property .
    // Assign all passed values to the class properties and call
    // ME.VALUE to return value
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    public dynamic GetValueEx(RegistryHive ClassKey, string SectionKey, string ValueKey, RegistryValueKind ValueType, dynamic Default)
    {
        this.ClassKey = ClassKey;
        this.SectionKey = SectionKey;
        this.ValueKey = ValueKey;
        this.ValueType = ValueType;
        this.Default = Default;
        return Value;
    }
    public void SetValueEx(RegistryHive ClassKey, string SectionKey, string ValueKey, RegistryValueKind ValueType, dynamic Default, dynamic NewValue)
    {
        this.ClassKey = ClassKey;
        this.SectionKey = SectionKey;
        this.ValueKey = ValueKey;
        this.ValueType = ValueType;
        this.Default = Default;

        if (this.ValueType == RegistryValueKind.String)
        {
            if (CStr(NewValue) == "")
            {
                NewValue = Default;
            }
        }
        Value = NewValue;
    }

    public dynamic Default { get; set; } = null;

    private static int SwapEndian(int dw)
    {
        return BinaryPrimitives.ReverseEndianness(dw);
    }

    private static string ExpandEnvStr(string sData)
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
