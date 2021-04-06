using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TreeSize.Helpers
{
    public static class IconHelpers
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObjetc);

        [DllImport("shell32")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        public static ImageSource ToImageSource(this Icon ico)
        {
            Bitmap bitmap = ico.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource image =
                Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                MessageBox.Show("Failed to delete unused bitmap object");
            }

            return image;
        }

        public static Icon GetIconOfFile(string path, bool smallIcon, bool isDirectoryOrDrive)
        {
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (smallIcon)
                flags |= SHGFI_SMALLICON;

            uint attributes = FILE_ATTRIBUTE_NORMAL;
            if (isDirectoryOrDrive)
                attributes |= FILE_ATTRIBUTE_DIRECTORY;

            int success =
                SHGetFileInfo(
                    path,
                    attributes,
                    out SHFILEINFO shfi,
                    (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                    flags);

            if (success == 0)
                return null;

            return Icon.FromHandle(shfi.hIcon);
        }
    }
}
