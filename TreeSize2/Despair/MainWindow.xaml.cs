using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using UtiliteAndModel;

namespace Despair
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			//PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Critical;
			InitializeComponent();
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo driveInfo in drives)
			{
				root.Items.Add(TreeBuilder.CreateNode(driveInfo, driveInfo.TotalSize, ItemType.Drive));
			}
		}
		public async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
		{
			TreeViewItem item = e.OriginalSource as TreeViewItem;
            if ((item.DataContext as Node) == null)
            {
				return;
            }
			var expandedItem = item.DataContext as Node;
			if (expandedItem.Type == ItemType.File)
            {
                try
                {
					Process.Start(expandedItem.FullName);
					return;
				}
                catch (Exception)
                {
					return;
                }
			}
			if (!expandedItem.WasOpened)
			{
				expandedItem.Nodes.Clear();
				item.ItemsSource = new ObservableCollection<Node>();
				expandedItem.WasOpened = true;
				DirectoryInfo expandedDir = TreeBuilder.TypeOfDirectory(expandedItem);
				try
				{
					ObservableCollection<Node> tempCollection = TreeBuilder.InitDirectory(expandedDir);
					expandedItem.Nodes = tempCollection;
					item.ItemsSource = tempCollection;
					List<Task> tasks = new List<Task>();
					Cursor = Cursors.Wait;
                    foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                    {
						tasks.Add(Task.Factory.StartNew(() => TreeBuilder.InitSize(tempCollection, subDir)));
                    }
					while (tasks.Count > 0)
                    {
						Task finishedTask = await Task.WhenAny(tasks);
						item.ItemsSource = tempCollection;
						tasks.Remove(finishedTask);
                        item.Items.Refresh();
					}
					Cursor = Cursors.Arrow;
				}
				catch { }
			}
		}	
	}
}

