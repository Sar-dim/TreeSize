using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace UtiliteAndModel
{
    public static class TreeBuilder
    {
        public static DirectoryInfo TypeOfDirectory(Node node)
        {
            DirectoryInfo directoryInfo = null;
            if (node.Type == ItemType.Drive)
            {
                directoryInfo = new DriveInfo(node.Name).RootDirectory;
            }
            if (node.Type == ItemType.Folder)
            {
                directoryInfo = new DirectoryInfo(node.FullName);
            }
            return directoryInfo;
        } 
        public static ObservableCollection<Node> InitDirectory(DirectoryInfo directoryInfo)
        {
            ObservableCollection<Node> tempCollection = new ObservableCollection<Node>();
            foreach (DirectoryInfo subDir in directoryInfo.GetDirectories())
            {
                long size = 0;
                tempCollection.Add(CreateNode(subDir, size, ItemType.Folder));

            }
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                tempCollection.Add(CreateNode(file, file.Length, ItemType.File));
            }
            return tempCollection;
        }

        public static Node CreateNode(object o, long size, ItemType type)
        {
            Node node = new Node
            {
                Name = o.ToString(),
                Size = size,
                WasOpened = false,
                Type = type,
                Nodes = new ObservableCollection<Node>(),
                FullName = ""
            };
            if (o is DirectoryInfo)
            {
                node.Name = (o as DirectoryInfo).Name;
                node.FullName = (o as DirectoryInfo).FullName;
                node.Nodes.Add(new Node { Name = "Loading..." });
            }
            else if (o is DriveInfo)
            {
                node.FullName = (o as DriveInfo).Name;
                node.Nodes.Add(new Node { Name = "Loading..." });
            }
            else if (o is FileInfo)
            {
                node.Name = (o as FileInfo).Name;
                node.FullName = (o as FileInfo).FullName;
            }

            return node;
        }

        public static ObservableCollection<Node> InitSize(ObservableCollection<Node> nodes, DirectoryInfo directoryInfo)
        {
            var tempNode = TreeBuilder.CreateNode(directoryInfo, 0, ItemType.Folder);
            tempNode.Size = TreeBuilder.DirSize(directoryInfo, 1);
            foreach (var changingItem in nodes)
            {
                if (changingItem.FullName == tempNode.FullName)
                {
                    changingItem.Size = tempNode.Size;
                    break;
                }
            }
            return nodes;
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
                    Size += DirSize(di, deep, aLimit);
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
