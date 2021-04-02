using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
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
using System.Windows.Shapes;

namespace WpfPhotoGalleryApp
{
    /// <summary>
    /// Логика взаимодействия для PhotoView.xaml
    /// </summary>
    public partial class PhotoView : Window
    {
        ObservableCollection<Photo> Photos { get; set; }
        public StringBuilder photoInfo { get; set; }
        public int CurrentPhoto { get; set; }
        public string path { get; set; }

        public PhotoView(ObservableCollection<Photo> photos , int index)
        {
            InitializeComponent();
            Photos = photos;
            CurrentPhoto = index;

            photoInfo = new StringBuilder(100);
            var uri = new Uri(Photos[CurrentPhoto].Path, UriKind.Absolute);
            ImageSource imSource = new BitmapImage(uri);
            PhotoImage.Source = imSource;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            CurrentPhoto--;
            if (CurrentPhoto<0)
            {
                CurrentPhoto = Photos.Count - 1 ;
            }

            var uri = new Uri(Photos[CurrentPhoto].Path, UriKind.Absolute);
            ImageSource imSource = new BitmapImage(uri);
            PhotoImage.Source = imSource;

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            CurrentPhoto++;
            if (CurrentPhoto == Photos.Count)
            {
                CurrentPhoto = 0;
            }

            var uri = new Uri(Photos[CurrentPhoto].Path, UriKind.Absolute);
            ImageSource imSource = new BitmapImage(uri);
            PhotoImage.Source = imSource;
        }

        private void Paint_Click(object sender, RoutedEventArgs e)
        {
            var filePath = Photos[CurrentPhoto].Path;
            ProcessStartInfo Info = new ProcessStartInfo()
            {
                FileName = "mspaint.exe",
                WindowStyle = ProcessWindowStyle.Normal,
                Arguments = filePath
            };
            Process.Start(Info);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            // otkrivayete ne vse fotki iz za urovnya dostupa
            PrintDialog printDlg = new PrintDialog();
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = "Print Photo";
            bool? result = printDlg.ShowDialog();
            if (result.Value) printDoc.Print();
        }

        private void InfoPhoto_Click(object sender, RoutedEventArgs e)
        {
            photoInfo.Clear();

            var filePath = Photos[CurrentPhoto].Path;
            System.IO.FileInfo info = new System.IO.FileInfo(filePath);

            photoInfo.Append("Photo \r\rName       :  "+ info.Name + "\r");          
            photoInfo.Append("Size        :  "+(info.Length / (1024)).ToString() + " Kb" + "\r");
            photoInfo.Append("Creation   :  " + info.CreationTime.ToString() + "\rLast Write :  " + info.LastWriteTime.ToString() + "\r");
            photoInfo.Append("Path  :  " + info.FullName);
            System.Windows.Forms.MessageBox.Show(photoInfo.ToString(),"Info", System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Information);
        }
    }
}
