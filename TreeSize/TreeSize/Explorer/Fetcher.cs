using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeSize.Files;
using TreeSize.Helpers;

namespace TreeSize.Explorer
{
    public static class Fetcher
    {
        public static List<FileModel> GetFiles(string directory)
        {
            List<FileModel> files = new List<FileModel>();
            if (!directory.IsDirectory())
                return files;
            string currentFile = "";
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    currentFile = file;
                    if (Path.GetExtension(file) != ".lnk")
                    {
                        FileInfo fInfo = new FileInfo(file);
                        FileModel fModel = new FileModel()
                        {
                            Icon = IconHelpers.GetIconOfFile(file, true, false),
                            Name = fInfo.Name,
                            Path = fInfo.FullName,
                            DateCreated = fInfo.CreationTime,
                            DateModified = fInfo.LastWriteTime,
                            Type = FileType.File,
                            SizeBytes = fInfo.Length
                        };
                        files.Add(fModel);
                    }
                }
                return files;
            }

            catch (IOException io)
            {
                MessageBox.Show(
                    $"IO Exception getting files in directory: {io.Message}",
                    "Exception getting files in directory");
            }
            catch (UnauthorizedAccessException noAccess)
            {
                MessageBox.Show(
                    $"No access for a file: {noAccess.Message}",
                    "Exception getting files in directory");
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Failed to get files in '{directory}' || " +
                    $"Something to do with '{currentFile}'\n" +
                    $"Exception: {e.Message}", "Error");
            }

            return files;
        }
        public static async Task GetDirectories(string directory, List<FileModel> directories)
        {
            
            await Task.Run(() =>
            {
                if (!directory.IsDirectory())
                    return directories;
                string currentDirectory = "";
                try
                {
                    foreach (string dir in Directory.GetDirectories(directory))
                    {
                        currentDirectory = dir;

                        DirectoryInfo dInfo = new DirectoryInfo(dir);
                        FileModel dModel = new FileModel()
                        {
                            Icon = IconHelpers.GetIconOfFile(dir, true, true),
                            Name = dInfo.Name,
                            Path = dInfo.FullName,
                            DateCreated = dInfo.CreationTime,
                            DateModified = dInfo.LastWriteTime,
                            Type = FileType.Folder,
                            SizeBytes = DirSize(new DirectoryInfo(dir), 1)
                        };

                        directories.Add(dModel);
                    }

                    foreach (string file in Directory.GetFiles(directory))
                    {
                        if (Path.GetExtension(file) == ".lnk")
                        {
                            string realDirPath = ExplorerHelper.GetShortcutTargetFolder(file);
                            FileInfo dInfo = new FileInfo(realDirPath);
                            FileModel dModel = new FileModel()
                            {
                                Icon = IconHelpers.GetIconOfFile(realDirPath, true, true),
                                Name = dInfo.Name,
                                Path = dInfo.FullName,
                                DateCreated = dInfo.CreationTime,
                                DateModified = dInfo.LastWriteTime,
                                Type = FileType.File,
                                SizeBytes = 0
                            };

                            directories.Add(dModel);
                        }
                    }
                    return directories;
                }
                catch (IOException io)
                {
                    MessageBox.Show(
                        $"IO Exception getting folders in directory: {io.Message}",
                        "Exception getting folders in directory");
                }
                catch (UnauthorizedAccessException noAccess)
                {
                    MessageBox.Show(
                        $"No access for a directory: {noAccess.Message}",
                        "Exception getting folders in directory");
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        $"Failed to get directories in '{directory}' || " +
                        $"Something to do with '{currentDirectory}'\n" +
                        $"Exception: {e.Message}", "Error");
                }
                return directories;
            });
       
            
        }

        public static List<FileModel> GetDrives()
        {
            List<FileModel> drives = new List<FileModel>();
            try
            {

                foreach (string drive in Directory.GetLogicalDrives())
                {
                    DriveInfo dInfo = new DriveInfo(drive);

                    FileModel dModel = new FileModel()
                    {
                        Icon = IconHelpers.GetIconOfFile(drive, true, true),
                        Name = dInfo.Name,
                        Path = dInfo.Name,
                        DateModified = DateTime.Now,
                        Type = FileType.Drive,
                        SizeBytes = dInfo.TotalSize
                    };
                    drives.Add(dModel);
                }
                return drives;
            }
            catch (IOException io)
            {
                MessageBox.Show($"IO Exception getting drives: {io.Message}", "Exception getting drives");
            }
            catch (UnauthorizedAccessException noAccess)
            {
                MessageBox.Show($"No access for a hard drive: {noAccess.Message}", "");
            }
            return drives;
        }

        public static long DirSize(DirectoryInfo d, int deep, long aLimit = 0)
        {
            long Size = 0;
            try
            {
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Size += fi.Length;
                    if (aLimit > 0 && Size > aLimit)
                        return Size;
                }
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    if (deep < 3)
                    {
                        deep++;
                        Size += DirSize(di, deep, aLimit);
                    }
                    if (aLimit > 0 && Size > aLimit)
                        return Size;
                }
                return Size;
            }
            catch (Exception e)
            {

            }
            return 0;
        }
    }
}
