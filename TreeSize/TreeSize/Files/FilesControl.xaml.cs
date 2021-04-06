using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeSize.Files
{
    /// <summary>
    /// Логика взаимодействия для FileModel.xaml
    /// </summary>
    public partial class FilesControl : UserControl
    {
        public FileModel File
        {
            get => this.DataContext as FileModel;
            set => this.DataContext = value;
        }
        public Action<FileModel> NavigateToPathCallback { get; set; }
        public FilesControl()
        {
            InitializeComponent();
            File = new FileModel();
        }

        public FilesControl(FileModel fModel)
        {
            InitializeComponent();
            File = fModel;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left &&
                e.LeftButton == MouseButtonState.Pressed &&
                e.ClickCount == 2)
            {
                NavigateToPathCallback?.Invoke(File);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToPathCallback?.Invoke(File);
            }
        }
    }
}
