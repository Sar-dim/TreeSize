﻿using NamespaceHere;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TreeSize.Explorer;
using TreeSize.Files;
using TreeSize.Helpers;

namespace TreeSize.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<FilesControl> FileItems { get; set; }

        public MainViewModel()
        {
            FileItems = new ObservableCollection<FilesControl>();
        }

        public async void TryNavigateToPath(string path)
        {
            if (path == string.Empty)
            {
                ClearFiles();

                foreach (FileModel drive in Fetcher.GetDrives())
                {
                    FilesControl fc = CreateFileControl(drive);
                    AddFile(fc);
                }
            } 
            else if (path.IsFile())
            {
                MessageBox.Show($"Opening {path}");
            } 
            else if (path.IsDirectory())
            {
                ClearFiles();
                string backPath = path.Substring(0, path.LastIndexOf('\\'));
                if (!backPath.Contains('\\'))
                {
                    backPath += "\\";
                    DriveInfo dInfo = new DriveInfo(backPath);
                    FileModel dModel = new FileModel()
                    {
                        Icon = IconHelpers.GetIconOfFile(backPath, true, true),
                        Name = dInfo.Name,
                        Path = dInfo.Name,
                        DateModified = DateTime.Now,
                        Type = FileType.Drive,
                        SizeBytes = dInfo.TotalSize
                    };
                    FilesControl back = CreateFileControl(dModel);
                    AddFile(back);
                }
                else
                {
                    DirectoryInfo dInfo = new DirectoryInfo(backPath);
                    FileModel dModel = new FileModel()
                    {
                        Icon = IconHelpers.GetIconOfFile(backPath, true, true),
                        Name = dInfo.Name,
                        Path = dInfo.FullName,
                        DateCreated = dInfo.CreationTime,
                        DateModified = dInfo.LastWriteTime,
                        Type = FileType.Folder,
                        SizeBytes = Fetcher.DirSize(new DirectoryInfo(backPath), 1)
                    };
                    FilesControl back = CreateFileControl(dModel);
                    AddFile(back);
                }
                List<FileModel> directories = new List<FileModel>();
                await Fetcher.GetDirectories(path, directories);
                foreach (FileModel dir in directories)
                {
                    FilesControl fc = CreateFileControl(dir);    
                    AddFile(fc);
                }

                foreach (FileModel file in Fetcher.GetFiles(path))
                {
                    FilesControl fc = CreateFileControl(file);
                    AddFile(fc);
                }
            }
            else
            {
                MessageBox.Show("Something get wrong");
            }
        }

        public void NavigateFromModel(FileModel file)
        {
            TryNavigateToPath(file.Path);
        }

        public void AddFile(FilesControl file)
        {
            FileItems.Add(file);
        }

        public void RemoveFile(FilesControl file)
        {
            FileItems.Remove(file);
        }

        public void ClearFiles()
        {
            FileItems.Clear();
        }

        public FilesControl CreateFileControl(FileModel fModel)
        {
            FilesControl fc = new FilesControl(fModel);
            SetupFileControlCallbacks(fc);
            return fc;
        }

        public void SetupFileControlCallbacks(FilesControl fc)
        {
            fc.NavigateToPathCallback = NavigateFromModel;
        }
    }
}
