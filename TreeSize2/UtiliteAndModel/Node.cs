using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace UtiliteAndModel
{
    public class Node
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public long Size { get; set; }
        public bool WasOpened { get; set; }
        public ItemType Type { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}
