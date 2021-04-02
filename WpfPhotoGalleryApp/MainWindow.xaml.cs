using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.Text.Json;

namespace WpfPhotoGalleryApp
{

    public partial class MainWindow : Window
    {
        public ObservableCollection<Folder> Folders { get; set; }
        public ObservableCollection<Photo> Photos { get; set; }
        public FolderBrowserDialog SelectFolder { get; set; }

        public string FolderSavePath { get; set; } = "folder.json";
        public MainWindow()
        {
            InitializeComponent();
            SelectFolder = new FolderBrowserDialog();
            ReadFromFile();
            Photos = new ObservableCollection<Photo>();

            ListPhotos.ItemsSource = Photos;
            ListFolders.ItemsSource = Folders;

            
        }
        public void ReadFromFile()
        {
            try
            {
                if (File.Exists(FolderSavePath))
                {
                    var str = File.ReadAllText(FolderSavePath);
                    Folders = JsonSerializer.Deserialize<ObservableCollection<Folder>>(str);
                }
                else
                {
                    Folders = new ObservableCollection<Folder>();
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
               
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                var json = JsonSerializer.Serialize(Folders);
                File.WriteAllText(FolderSavePath, json);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var result = SelectFolder.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = System.IO.Path.GetFileName(SelectFolder.SelectedPath);
                string path = SelectFolder.SelectedPath;

                if (!IsExist(path))
                {
                    Folders.Add(new Folder(folderName, path));
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("This folder has already been added", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }                
            }
        }

        private bool IsExist(string path)
        {
            foreach (var Folder in Folders)
            {
                if (Folder.Path.Equals(path))
                {
                    return true;
                }
            }
            return false;
        }
        
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ListFolders.SelectedItem as Folder;

            if (item != null)
            {
                Folders.Remove(item);
            }                          
            else
                System.Windows.Forms.MessageBox.Show("You have not selected a folder", "Not delete", MessageBoxButtons.OK, MessageBoxIcon.Error);            
        }

        private void ListFolders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool isEmpty = true;
            var folder = ListFolders.SelectedItem as Folder;

            if (folder != null)
            {
                Photos.Clear();
                foreach (var item in System.IO.Directory.GetFiles(folder.Path))
                {
                    if (System.IO.Path.GetExtension(item) == ".bmp" || System.IO.Path.GetExtension(item) == ".png" ||
                        System.IO.Path.GetExtension(item) == ".jpg" || System.IO.Path.GetExtension(item) == ".svg" ||
                        System.IO.Path.GetExtension(item) == ".tiff")
                    {
                        System.IO.FileInfo info = new FileInfo(item);

                        string fileName = info.Name;
                        string path = item;
                        string lenght = " Size : " + (info.Length / (1024)).ToString() + " Kb";
                        string data = " Creation : " + info.CreationTime.ToString() + "\t Last Write :" + info.LastWriteTime.ToString();
                        Photos.Add(new Photo { Name = fileName, Path = path, Date = data, Size = lenght });
                        isEmpty = false;
                    }
                }

                if (isEmpty)
                System.Windows.Forms.MessageBox.Show("The folder does not contain a photo","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);                
            }
        }

        private void ListPhotos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = ListPhotos.SelectedIndex;

            if (index!=-1)
            {
                PhotoView view = new PhotoView(Photos,index);
                this.Hide();
                view.ShowDialog();
                this.Show();
            }
        }
    }
}
