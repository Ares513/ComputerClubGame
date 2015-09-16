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
using System.IO;
using Microsoft.Win32;


namespace MapEditor
{
    public class TileSelectedArgs
    {
        public Tile t;
        public TileSelectedArgs(Tile tile) { t = tile; }
    }
    public delegate void TileSelectedHandler(object sender, TileSelectedArgs e);

    /// <summary>
    /// Interaction logic for TileSetEditor.xaml
    /// </summary>
    public partial class TileSetEditor : UserControl, MapEditor.IEditorPage
    {
        private TileSet baseTileSet = null;
        public TileSet Tiles { get { return baseTileSet; } }

        private bool editable = true;
        public bool Editable
        {
            get { return editable; }
            set
            {
                editable = value;
                endCreateTile();
                Origin.Visibility = Editable ? Visibility.Visible : Visibility.Hidden;
                foreach (TileEditor t in TileEditorGroup.Children) t.Editable = Editable;
            }
        }

        public event TileSelectedHandler TileSelected;

        public TileSetEditor()
        {
            InitializeComponent();
            Area.Visibility = Visibility.Hidden;
        }

        #region Editor File Operations
        public event EventHandler TitleChanged;
        public string TitleName { get { return (SaveFilePath == null ? "New TileSet" : Path.GetFileNameWithoutExtension(SaveFilePath)); } }
        public string Title { get { return TitleName + (Saved ? "" : " *"); } }
        protected void OnTitleChanged()
        {
            if(TitleChanged != null) TitleChanged(this, EventArgs.Empty);
        }

        private void HandleTileChanged(object sender, EventArgs e) { OnModify(); }
        private void OnModify() { Saved = false; }
        
        private bool _saved = true;
        private bool Saved
        {
            get { return _saved; }
            set { if (value != _saved) { _saved = value; OnTitleChanged(); } }
        }
        // set BaseTexturePath to create a new TileSetEditor from a texture
        // set SaveFilePath to open a TileSetEditor
        //private string _saveFilePath = null;
        public string BaseTexturePath
        {
            get { return baseTileSet.BaseTexturePath; }
            set { NewEditor(value); }
        }
        public string OpenFilePath
        {
            get { return (baseTileSet == null) ? null : baseTileSet.SaveTileSetPath; }
            set { OpenEditor(value); }
        }
        public string SaveFilePath
        {
            get {return baseTileSet.SaveTileSetPath;}
            set { if (SaveFilePath != value) { baseTileSet.SaveTileSetPath = value; Saved = false; OnTitleChanged(); } }
        }

        public void NewEditor(string texturePath)
        {
            CloseEditor();

            baseTileSet = new TileSet();
            baseTileSet.BaseTexturePath = texturePath;
            baseImage.Source = new BitmapImage(new Uri(BaseTexturePath));
        }
        public void OpenEditor(string file)
        {
            CloseEditor();

            using (FileStream openFile = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                OpenFromTileSet(TileSet.FromStream(openFile));
            }
            SaveFilePath = file;
            Saved = true;
        }
        public bool SaveEditor()
        {
            if (SaveFilePath == null)
            {
                if (!PromptSavePath()) return false;
            }
            using (FileStream saveFile = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
            {
                baseTileSet.ToStream(saveFile);
            }
            Saved = true;
            return true;
        }
        public bool CloseEditor()
        {
            if (!Saved)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes to " + TitleName + " before closing?",
                    TitleName, MessageBoxButton.YesNoCancel, MessageBoxImage.None, MessageBoxResult.Cancel);
                if (result == MessageBoxResult.Cancel) return false;
                else if (result == MessageBoxResult.Yes)
                {
                    bool saveResult = SaveEditor();
                    if (!saveResult) return false;
                }
            }

            TileEditorGroup.Children.Clear();
            baseImage.Source = null;
            baseTileSet = null;
            
            return true;
        }

        public void OpenFromTileSet(TileSet tset)
        {
            baseTileSet = tset;
            baseImage.Source = new BitmapImage(new Uri(BaseTexturePath));
            foreach (Tile t in baseTileSet)
            {
                TileEditor ed = new TileEditor(); ed.Editable = Editable;
                ed.Tile = t; ed.Changed += HandleTileChanged;
                TileEditorGroup.Children.Add(ed);
            }
        }

        public bool PromptSavePath()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = MainWindow.ContentDir;
            dlg.FileOk += MainWindow.ValidateFileName;
            dlg.Filter = "Tile Sets|*.tls";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                SaveFilePath = dlg.FileName;
                return true;
            }

            return false;
        }
        #endregion

        #region Edit Operations
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!Editable) return;

            if (e.Key == Key.Escape)
            {
                if (createTileInProgress)
                {
                    endCreateTile();
                }
            }

            if (e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Escape)
            {
                TileEditor t = getTileEditorAt(Mouse.GetPosition(TileEditorGroup));
                if (t != null) { TileEditorGroup.Children.Remove(t); OnModify(); }
            }
        }

        private bool createTileInProgress = false, findOriginInProgress = false;
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            double x = e.GetPosition(TileEditorGroup).X, y = e.GetPosition(TileEditorGroup).Y;
            int locX = (int)x/64;
            int locY = (int)y/64;
            if (x < 0 || x >= baseImage.ActualWidth || y < 0 || y >= baseImage.ActualHeight) return;

            Tile t = baseTileSet.getTileAt(locX, locY);
            if (t != null && TileSelected != null) TileSelected(this, new TileSelectedArgs(t));

            if (!Editable) return;

            if (!createTileInProgress)
            {
                if (isValidBounds(locX, locY, locX, locY))
                {
                    Area.Width = 64; Area.Height = 64;
                    Area.Margin = new Thickness(locX * 64, locY * 64, 0, 0);
                    Area.Visibility = Visibility.Visible;
                    createTileInProgress = true;
                }
            }
        }

        protected override void  OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            double x = e.GetPosition(TileEditorGroup).X, y = e.GetPosition(TileEditorGroup).Y;
            int locX = (int)x / 64;
            int locY = (int)y / 64;
            if (x < 0 || x >= baseImage.ActualWidth || y < 0 || y >= baseImage.ActualHeight) return;

            if (!Editable) return;

            if (createTileInProgress)
            {
                if (findOriginInProgress)
                {
                    TileEditor t = new TileEditor(); t.Tile = new Tile(); t.Editable = Editable;
                    t.Tile.LeftX = (int)Area.Margin.Left / 64; t.Tile.TopY = (int)Area.Margin.Top / 64;
                    t.Tile.RightX = t.Tile.LeftX + (int)Area.Width / 64 - 1; t.Tile.BottomY = t.Tile.TopY + (int)Area.Height / 64 - 1;
                    t.Tile.OriginX = (int)Origin.Margin.Left / 64; t.Tile.OriginY = (int)Origin.Margin.Top / 64;
                    t.Changed += HandleTileChanged;
                    TileEditorGroup.Children.Add(t);
                    OnModify();
                    endCreateTile();
                }
                else
                {
                    if (Area.Width == 64 && Area.Height == 64)
                    {
                        TileEditor t = new TileEditor(); t.Tile = new Tile(); t.Editable = Editable;
                        t.Tile.PositionX = (int)Area.Margin.Left / 64;
                        t.Tile.PositionY = (int)Area.Margin.Top / 64;
                        t.Changed += HandleTileChanged;
                        TileEditorGroup.Children.Add(t);
                        OnModify();
                        endCreateTile();
                    }
                    else
                    {
                        if (isValidBounds((int)Area.Margin.Left / 64, (int)Area.Margin.Top / 64, locX, locY))
                        {
                            findOriginInProgress = true;
                        }
                        else
                        {
                            endCreateTile();
                        }
                    }
                }
            }
        }

        protected override void  OnPreviewMouseMove(MouseEventArgs e)
        {
            double x = e.GetPosition(TileEditorGroup).X, y = e.GetPosition(TileEditorGroup).Y;
            int locX = (int)x / 64;
            int locY = (int)y / 64;
            if (x < 0 || x >= baseImage.ActualWidth || y < 0 || y >= baseImage.ActualHeight) return;

            if (!Editable) return;

            if (createTileInProgress)
            {
                if (!findOriginInProgress)
                {
                    if (isValidBounds((int)Area.Margin.Left / 64, (int)Area.Margin.Top / 64, locX, locY))
                    {
                        Area.Width = (locX + 1) * 64 - Area.Margin.Left;
                        Area.Height = (locY + 1) * 64 - Area.Margin.Top;
                    }
                }
            }
            if (findOriginInProgress)
            {
                if (locX * 64 >= Area.Margin.Left && locY * 64 >= Area.Margin.Top &&
                    (locX + 1) * 64 <= Area.Margin.Left + Area.Width && (locY + 1) * 64 <= Area.Margin.Top + Area.Height)
                {
                    Origin.Margin = new Thickness(locX * 64, locY * 64, 0, 0);
                }
            }
            else
            {
                Origin.Margin = new Thickness(locX * 64, locY * 64, 0, 0);
            }
        }

        private void endCreateTile()
        {
            Area.Visibility = Visibility.Hidden;
            createTileInProgress = findOriginInProgress = false;
        }

        private bool isValidBounds(int left, int top, int right, int bottom)
        {
            foreach (TileEditor t in TileEditorGroup.Children)
            {
                if (!(right < t.Tile.LeftX || left > t.Tile.RightX) && !(bottom < t.Tile.TopY || top > t.Tile.BottomY))
                {
                    return false;
                }
            }
            return true;
        }

        private TileEditor getTileEditorAt(Point p)
        {
            foreach (TileEditor t in TileEditorGroup.Children) {
                if (t.Tile.LeftX * 64 <= p.X && t.Tile.TopY * 64 <= p.Y && (t.Tile.RightX + 1) * 64 >= p.X && (t.Tile.BottomY + 1) * 64 >= p.Y) return t;
            }
            return null;
        }

        public ImageBrush getTileBrush(Tile t)
        {
            //Tile t = baseTileSet.getTileAt(index & 15, (index >> 4) & 15);
            //if (t == null) return null;
            ImageBrush b = new ImageBrush(baseImage.Source);
            b.ViewboxUnits = BrushMappingMode.Absolute;
            b.Viewbox = new Rect(64 * t.LeftX, 64 * t.TopY, 64 * (t.RightX - t.LeftX + 1), 64 * (t.BottomY - t.TopY + 1));
            return b;
        }
        #endregion
    }
}