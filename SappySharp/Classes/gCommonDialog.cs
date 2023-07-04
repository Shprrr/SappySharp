using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Strings;

namespace SappySharp.Classes;

public static class gCommonDialog
{
    private const int MAX_PATH = 260;

    [DllImport("COMDLG32", EntryPoint = "GetFileTitleA")]
    private static extern int GetFileTitle(string szFile, string szTitle, int cbBuf);

    public static bool VBGetOpenFileName(ref string Filename, ref string FileTitle, ref bool ReadOnly, ref string Filter /*= "All (*.*)| *.*"*/, ref int FilterIndex, string InitDir = null, string DlgTitle = null, string DefaultExt = null, bool FileMustExist = true, bool MultiSelect = false, bool HideReadOnly = false)
    {
        OpenFileDialog fileDialog = new()
        {
            CheckFileExists = FileMustExist,
            DefaultExt = DefaultExt,
            ShowReadOnly = !HideReadOnly,
            InitialDirectory = InitDir,
            FileName = Filename,
            Filter = Filter,
            FilterIndex = FilterIndex,
            Multiselect = MultiSelect,
            ReadOnlyChecked = ReadOnly,
            Title = DlgTitle
        };

        bool result = fileDialog.ShowDialog() == DialogResult.OK;

        Filename = fileDialog.FileName;
        FileTitle = fileDialog.SafeFileName;
        FilterIndex = fileDialog.FilterIndex;
        ReadOnly = fileDialog.ReadOnlyChecked;

        return result;
    }

    public static bool VBGetSaveFileName(ref string Filename, ref string FileTitle, ref string Filter /*= "All (*.*)| *.*"*/, ref int FilterIndex, bool OverWritePrompt = true, string InitDir = null, string DlgTitle = null, string DefaultExt = null)
    {
        SaveFileDialog fileDialog = new()
        {
            DefaultExt = DefaultExt,
            InitialDirectory = InitDir,
            FileName = Filename,
            Filter = Filter,
            FilterIndex = FilterIndex,
            OverwritePrompt = OverWritePrompt,
            Title = DlgTitle
        };

        bool result = fileDialog.ShowDialog() == DialogResult.OK;

        Filename = fileDialog.FileName;
        FileTitle = System.IO.Path.GetFileName(fileDialog.FileName);
        FilterIndex = fileDialog.FilterIndex;

        return result;
    }

    public static string VBGetFileTitle(string sFile)
    {
        string sFileTitle = new('\0', MAX_PATH);
        int cFileTitle = GetFileTitle(sFile, sFileTitle, MAX_PATH);
        if (cFileTitle != 0)
        {
            return "";
        }
        else
        {
            return Left(sFileTitle, InStr(sFileTitle, vbNullChar) - 1);
        }
    }
}
