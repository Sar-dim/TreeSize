using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TreeSize.Files
{
    public class FileModel
    {
        public Icon Icon { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public FileType Type { get; set; }

        public long SizeBytes { get; set; }

        public bool IsFile => Type == FileType.File;
        public bool IsFolder => Type == FileType.Folder;
        public bool IsDrive => Type == FileType.Drive;
        public bool IsShortcut => Type == FileType.Shortcut;
    }
}
