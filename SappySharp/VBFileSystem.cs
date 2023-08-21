using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SappySharp;

internal static class VBFileSystem
{
    public static FileStream File4;
    public static FileStream File42;
    public static FileStream File43;
    public static FileStream File44;
    public static FileStream File96;
    public static FileStream File98;
    public static FileStream File99;

    public static void FileClose(FileStream fs) => fs?.Close();
    public static bool EOF(FileStream fs) => fs.Position >= fs.Length;

    public static void ChDir(string path) => Microsoft.VisualBasic.FileSystem.ChDir(path);
    public static string Dir(string path) => Microsoft.VisualBasic.FileSystem.Dir(path);

    public static int Read(this FileStream fs, out string value, int length)
    {
        byte[] buffer = new byte[length];
        int lengthRead = fs.Read(buffer, 0, buffer.Length);
        value = Encoding.Default.GetString(buffer);
        return lengthRead;
    }

    public static int Read<T>(this FileStream fs, out T value) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] buffer = new byte[size];

        int lengthRead = fs.Read(buffer, 0, buffer.Length);
        value = FromBytes<T>(buffer);

        return lengthRead;
    }

    private static T FromBytes<T>(byte[] arr) where T : struct
    {
        T str = default;
        GCHandle h = default;

        try
        {
            h = GCHandle.Alloc(arr, GCHandleType.Pinned);

            str = Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
        }
        finally
        {
            if (h.IsAllocated)
            {
                h.Free();
            }
        }

        return str;
    }

    public static void Write(this FileStream fs, string value)
    {
        byte[] buffer = Encoding.Default.GetBytes(value + Environment.NewLine);
        fs.Write(buffer, 0, buffer.Length);
    }

    public static void Write(this FileStream fs, int value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        fs.Write(buffer, 0, buffer.Length);
    }

    public static void Write(this FileStream fs, long value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        fs.Write(buffer, 0, buffer.Length);
    }
}
