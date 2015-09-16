using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string ContentDir { get; private set; }

        static MainWindow()
        {
            ContentDir = System.IO.Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName,
                "ComputerClubGame\\ComputerClubGameContent\\");
        }

        public static string GetRelPath(string path)
        {
            path = System.IO.Path.ChangeExtension(path, null);
            if (!path.StartsWith(ContentDir)) throw new Exception("Bad Path");
            return path.Substring(ContentDir.Length);
        }
        public static string GetAbsPath(string path, string ext)
        {
            string newPath;
            newPath = System.IO.Path.Combine(ContentDir, path);
            newPath = System.IO.Path.ChangeExtension(newPath, ext);
            return newPath;
        }

        public static void WriteFilePath(Stream s, string abspath)
        {
            string relPath = GetRelPath(abspath);
            int PathLength = Encoding.UTF8.GetByteCount(relPath);
            if (PathLength > 256) throw new Exception("Path too long");
            s.WriteByte((byte)PathLength);
            s.Write(Encoding.UTF8.GetBytes(relPath), 0, PathLength);
        }
        public static string ReadFilePath(Stream s, string ext)
        {
            int PathLength = s.ReadByte();
            byte[] relPathBytes = new byte[PathLength];
            s.Read(relPathBytes, 0, PathLength);
            string relPath = Encoding.UTF8.GetString(relPathBytes);
            return GetAbsPath(relPath, ext);
        }

        public static void ValidateFileName(object sender, CancelEventArgs e)
        {
            if (!((FileDialog)sender).FileName.StartsWith(ContentDir))
            {
                e.Cancel = true;
                MessageBox.Show("Must select a file within the Content directory of this project");
            }
        }

        void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (TabItem t in EditorTabs.Items)
            {
                bool result = (t.Content as IEditorPage).CloseEditor();
                if (!result)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            foreach (TabItem t in EditorTabs.Items) {
                ((TileSetEditor)t.Content).TitleChanged += HandleEditorTitleChanged;
                t.Header = ((TileSetEditor)t.Content).Title;
            }
        }

        void CanExecuteWhenHasTab(object sender, CanExecuteRoutedEventArgs e)
        {
            if (EditorTabs == null || EditorTabs.Items.Count == 0) e.CanExecute = false;
            else e.CanExecute = true;
        }
        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }

        void ExecuteNewTileSet(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ContentDir;
            dlg.FileOk += ValidateFileName;

            dlg.Filter = "Image Files|*.png";
            
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                CreateNewTileSetEditor(dlg.FileName);
            }
            dlg = null;
        }
        void ExecuteNewMap(object target, ExecutedRoutedEventArgs e)
        {
            CreateNewMapEditor();
        }
        void ExecuteOpen(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ContentDir;
            dlg.FileOk += ValidateFileName;

            dlg.Filter = "All Map Files (.map, .tls, .png)|*.map;*.tls;*.png|Maps|*.map|Tile Sets|*.tls|Image Files (.png)|*.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                string ext = System.IO.Path.GetExtension(fileName);
                if (ext == ".png")
                {
                    CreateNewTileSetEditor(fileName);
                }
                else if (ext == ".tls")
                {
                    OpenTileSetEditor(fileName);
                }
                else if (ext == ".map")
                {
                    OpenMapEditor(fileName);
                }
            }

            dlg = null;
        }
        void ExecuteOpenTileSet(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ContentDir;
            dlg.FileOk += ValidateFileName;

            dlg.Filter = "Tile Sets|*.tls";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                OpenTileSetEditor(dlg.FileName);
            }

            dlg = null;
        }
        void ExecuteOpenMap(object target, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ContentDir;
            dlg.FileOk += ValidateFileName;

            dlg.Filter = "Maps|*.map";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                OpenMapEditor(dlg.FileName);
            }

            dlg = null;
        }
        void ExecuteSave(object target, ExecutedRoutedEventArgs e)
        {
            (EditorTabs.SelectedContent as IEditorPage).SaveEditor();
        }
        void ExecuteClose(object target, ExecutedRoutedEventArgs e)
        {
            TabItem tab = e.Parameter as TabItem;
            if (tab == null) tab = EditorTabs.SelectedItem as TabItem;

            IEditorPage ed = tab.Content as IEditorPage;
            if (ed.CloseEditor()) EditorTabs.Items.Remove(tab);
        }

        void CreateNewTileSetEditor(string fileName) {
            TileSetEditor ed = new TileSetEditor();
            ed.NewEditor(fileName);
            AttachNewEditor(ed);
        }
        void OpenTileSetEditor(string fileName)
        {
            TileSetEditor ed = new TileSetEditor();
            ed.OpenEditor(fileName);
            AttachNewEditor(ed);
        }
        void CreateNewMapEditor()
        {
            MapEditorPage ed = new MapEditorPage();
            ed.NewEditor();
            AttachNewEditor(ed);
        }
        void OpenMapEditor(string fileName)
        {
            MapEditorPage ed = new MapEditorPage();
            ed.OpenEditor(fileName);
            AttachNewEditor(ed);
        }

        void AttachNewEditor(IEditorPage ed)
        {
            TabItem tab = new TabItem();
            ed.TitleChanged += HandleEditorTitleChanged;
            tab.Header = ed.Title;
            tab.HeaderTemplate = EditorTabs.FindResource("tabHeader") as DataTemplate;
            tab.Content = ed;
            EditorTabs.SelectedIndex = EditorTabs.Items.Add(tab);
        }

        void HandleEditorTitleChanged(object sender, EventArgs e)
        {
            foreach (TabItem t in EditorTabs.Items) t.Header = (t.Content as IEditorPage).Title;
        }
    }
}