using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace HugeFilesDirs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var drives = DriveInfo.GetDrives();

            TreeViewFileSystem.ItemsSource = drives.Select(x => new Item()
            {
                Name = x.Name,
                Type = "Folder",
                Size = x.TotalSize,
                Path = x.RootDirectory.FullName,
                Items = new List<Item>()
            });

        }

        private void TreeViewFileSystem_OnExpanded(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            var data = item.DataContext as Item;

            if (data.Type != "File")
            {
                var dir = new DirectoryInfo(data.Path);

                try
                {
                    foreach (var di in dir.GetDirectories())
                    {
                        item.Items.Add(new Item()
                        {
                            Name = di.Name,
                            Type = "Folder",
                            Size = GetSize(GetAllFiles(di.FullName)),
                            Path = di.FullName,
                            Items = new List<Item>()
                        });
                    }

                    long size;
                    long.TryParse(TextBoxSize.Text, out size);

                    foreach (var fi in dir.GetFiles().Where(SkipUnaccessible))
                    {
                        if (fi.Length < size) continue;

                        item.Items.Add(new Item()
                        {
                            Name = fi.Name,
                            Type = "File",
                            Size = fi.Length,
                            Items = null
                        });
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                }
            }
        }

        private IEnumerable<string> GetAllFiles(string path)
        {
            try
            {
                return System.IO.Directory.EnumerateFiles(path).Union(
                    System.IO.Directory.EnumerateDirectories(path).SelectMany(d =>
                    {
                        try
                        {
                            return GetAllFiles(d);
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            return Enumerable.Empty<String>();
                        }
                    }));
            }
            catch (UnauthorizedAccessException e)
            {
                return Enumerable.Empty<String>();
            }
        }

        private long GetSize(IEnumerable<string> filePath)
        {
            return filePath.Where(x=>!string.IsNullOrEmpty(x)).Sum(x=>new FileInfo(x).Length);
        }

        private bool SkipUnaccessible(FileInfo fi)
        {
            long length;
            try
            {
                length = fi.Length;
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                return false;
            }
        }
    }
}
