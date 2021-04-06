using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shell32;

namespace TreeSize.Explorer
{
    public static class ExplorerHelper
    {
        public static bool IsFile(this string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }
        public static bool IsDirectory(this string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }
        public static bool IsDrive(this string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }
        public static string GetFileName(this string fullpath)
        {
            return Path.GetFileName(fullpath);
        }
        public static string GetParentDirectory(this string fullpath)
        {
            return Path.GetFileName(fullpath);
        }
        public static bool CheckPathIsShortcutFile(string path)
        {
            return File.Exists(GetShortcutTargetFolder(path));
        }
        public static string GetShortcutTargetFolder(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }
    }
}
