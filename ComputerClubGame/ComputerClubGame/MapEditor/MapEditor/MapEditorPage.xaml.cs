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
using System.IO;
using Microsoft.Win32;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MapEditor.xaml
    /// </summary>
    public partial class MapEditorPage : UserControl, MapEditor.IEditorPage
    {
        private IsoCamera cam = new IsoCamera();
        private int clipZ = 0;
        private Map baseMap;

        private Map.TileReference currentTile = null;

        public MapEditorPage()
        {
            InitializeComponent();

            baseMap = new Map();
            
            RebuildMap();
        }

        #region Editor File Operations
        public event EventHandler TitleChanged;
        public string TitleName { get { return (SaveFilePath == null ? "New Map" : System.IO.Path.GetFileNameWithoutExtension(SaveFilePath)); } }
        public string Title { get { return TitleName + (Saved ? "" : " *"); } }
        protected void OnTitleChanged()
        {
            if (TitleChanged != null) TitleChanged(this, EventArgs.Empty);
        }

        private void OnModify() { Saved = false; }

        private bool _saved = true;
        private bool Saved
        {
            get { return _saved; }
            set { if (value != _saved) { _saved = value; OnTitleChanged(); } }
        }

        private string saveFilePath;
        public string OpenFilePath
        {
            get { return saveFilePath; }
            set { OpenEditor(value); }
        }
        public string SaveFilePath
        {
            get { return saveFilePath; }
            set { if (SaveFilePath != value) { saveFilePath = value; Saved = false; OnTitleChanged(); } }
        }

        public void NewEditor()
        {
            CloseEditor();
            baseMap = new Map(5,5);
            RebuildMap();
        }
        public void OpenEditor(string file)
        {
            CloseEditor();

            using (FileStream openFile = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                baseMap = Map.FromStream(openFile, getTileSetFromFileName);
            }
            foreach (TileSet tset in baseMap.tileSets)
            {
                TileSetEditor ed = new TileSetEditor();
                ed.Editable = false;
                ed.OpenFromTileSet(tset);
                ed.TileSelected += OnTileSelected;

                TabItem tab = new TabItem();
                tab.Header = ed.Title;
                tab.Content = ed;
                TileSets.SelectedIndex = TileSets.Items.Add(tab);
            }
            RebuildMap();
            ZClip.Value = ZClip.Maximum;
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
                baseMap.ToStream(saveFile);
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

            foreach (TabItem tab in TileSets.Items)
            {
                (tab.Content as TileSetEditor).CloseEditor();
            }
            TileSets.Items.Clear();
            baseMap = new Map();
            RebuildMap();
            baseMap = null;
            SaveFilePath = null;

            return true;
        }

        public bool PromptSavePath()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = MainWindow.ContentDir;
            dlg.FileOk += MainWindow.ValidateFileName;
            dlg.Filter = "Maps|*.map";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                SaveFilePath = dlg.FileName;
                return true;
            }

            return false;
        }

        private TileSet getTileSetFromFileName(string path) {
            // TODO: check if the path exists and deal with missing tile sets
            using (FileStream openFile = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                TileSet tset = TileSet.FromStream(openFile);
                tset.SaveTileSetPath = path;
                return tset;
            }
        }

        #endregion

        #region Edit Operations
        private void RebuildMap()
        {
            Rect canvRect = new Rect(
                new Point(cam.MapToScreen(new Vector3(baseMap.width + 1, 0, 0)).X, cam.MapToScreen(new Vector3(0, 0, 0)).Y),
                new Point(cam.MapToScreen(new Vector3(0, baseMap.height + 1, 0)).X, cam.MapToScreen(new Vector3(baseMap.width + 1, baseMap.height + 1, 0)).Y));
            Tiles.Children.Clear();

            int maxHeight = 0;
            for (int i = 0; i < baseMap.tiles.Count; i++)
            {
                for (int j = 0; j < baseMap.tiles[i].Count; j++)
                {
                    int height = 0;
                    foreach (Map.TileReference tref in baseMap.tiles[i][j])
                    {
                        Tile tile = baseMap.GetTile(tref);

                        Point loc = cam.MapToScreen(new Vector3(j, i, height));
                        Rect tRect = new Rect(loc.X - (64 * (tile.OriginX - tile.LeftX) + 32), loc.Y - (64 * (tile.OriginY - tile.TopY) + 32),
                            64 * (tile.RightX - tile.LeftX + 1), 64 * (tile.BottomY - tile.TopY + 1));

                        Rectangle rec = new Rectangle();
                        Canvas.SetLeft(rec, tRect.Left); Canvas.SetTop(rec, tRect.Top);
                        Panel.SetZIndex(rec, i + j + height);
                        rec.Width = tRect.Width; rec.Height = tRect.Height;
                        rec.Fill = ((TileSets.Items[tref.TileSetIndex] as TabItem).Content as TileSetEditor).getTileBrush(tile);
                        rec.Opacity = (height > clipZ || (height == clipZ && !tile.IsFloor)) ? .5*Math.Pow(.7,height-clipZ) : 1;

                        Tiles.Children.Add(rec);

                        canvRect.Union(tRect);

                        if (tile.IsBlock) height++;
                    }
                    if (height > maxHeight) maxHeight = height;
                }
            }
            canvRect.Union(cam.MapToScreen(new Vector3(0, 0, clipZ)));
            canvas.Width = canvRect.Width;
            canvas.Height = canvRect.Height;
            Canvas.SetLeft(Origin, -canvRect.Left);
            Canvas.SetTop(Origin, -canvRect.Top);

            PointCollection mapBorder = new PointCollection();
            mapBorder.Add(cam.MapToScreen(new Vector3(0, 0, clipZ))); mapBorder.Add(cam.MapToScreen(new Vector3(baseMap.width, 0, clipZ)));
            mapBorder.Add(cam.MapToScreen(new Vector3(baseMap.width, baseMap.height, clipZ))); mapBorder.Add(cam.MapToScreen(new Vector3(0, baseMap.height, clipZ)));
            MapEdge.Points = mapBorder;

            ZClip.Maximum = maxHeight+1;

            PreviewPaint();
        }

        public void OnEraserClicked(object sender, RoutedEventArgs e)
        {
            currentTile = null;
        }
        public void OnAddRowClicked(object sender, RoutedEventArgs e)
        {
            baseMap.InsertRow(baseMap.height);
            OnModify();
            RebuildMap();
        }
        public void OnAddColClicked(object sender, RoutedEventArgs e)
        {
            baseMap.InsertColumn(baseMap.width);
            OnModify();
            RebuildMap();
        }
        public void OnRemoveRowClicked(object sender, RoutedEventArgs e)
        {
            if (baseMap.height == 0) return;
            for (int x = 0; x < baseMap.width; x++) if (baseMap.tiles[baseMap.height - 1][x].Count != 0) return;
            baseMap.RemoveRow(baseMap.height - 1);
            OnModify();
            RebuildMap();
        }
        public void OnRemoveColClicked(object sender, RoutedEventArgs e)
        {
            if (baseMap.width == 0) return;
            for (int y = 0; y < baseMap.height; y++) if (baseMap.tiles[y][baseMap.width-1].Count != 0) return;
            baseMap.RemoveColumn(baseMap.width - 1);
            OnModify();
            RebuildMap();
        }

        public void OnAddTileSetClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = MainWindow.ContentDir;
            dlg.FileOk += MainWindow.ValidateFileName;

            dlg.Filter = "Tile Sets|*.tls";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                TileSetEditor tset = new TileSetEditor();
                tset.Editable = false;
                tset.OpenEditor(dlg.FileName);
                tset.TileSelected += OnTileSelected;
                baseMap.tileSets.Add(tset.Tiles);
                
                TabItem tab = new TabItem();
                tab.Header = tset.Title;
                tab.Content = tset;
                TileSets.SelectedIndex = TileSets.Items.Add(tab);
                OnModify();
            }

            dlg = null;
        }
        public void OnRemoveTileSetClicked(object sender, RoutedEventArgs e)
        {
            int tseti = TileSets.SelectedIndex;
            if (TileSets.Items.Count == 0) return;
            int num = 0;
            for (int y = 0; y < baseMap.height; y++)
            {
                for (int x = 0; x < baseMap.width; x++)
                {
                    foreach (Map.TileReference tref in baseMap.tiles[y][x])
                    {
                        if (tref.TileSetIndex == tseti) num++;
                    }
                }
            }
            if (num > 0) return; // TODO
            baseMap.tileSets.RemoveAt(tseti);
            (TileSets.SelectedContent as TileSetEditor).CloseEditor();
            TileSets.Items.RemoveAt(tseti);
            for (int y = 0; y < baseMap.height; y++)
            {
                for (int x = 0; x < baseMap.width; x++)
                {
                    Map.TileStack tstack = new Map.TileStack();
                    foreach (Map.TileReference tref in baseMap.tiles[y][x])
                    {
                        if (tref.TileSetIndex > tseti) tref.TileSetIndex--;
                        if (tref.TileSetIndex != tseti) tstack.Add(tref);
                    }
                    baseMap.tiles[y][x] = tstack;
                }
            }
            OnModify();
            RebuildMap();
        }

        public void OnZClipValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            clipZ = (int)ZClip.Value;
            RebuildMap();
        }

        private void OnTileSelected(object sender, TileSelectedArgs e)
        {
            // assume sender is the tsettab TileSetEditor
            currentTile = new Map.TileReference(0, e.t.OriginIndex);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (canvas.IsMouseOver)
            {
                PaintCurrentTile();
            }
            PreviewPaint();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            PreviewPaint();

        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
        }

        private void PreviewPaint()
        {
            Vector3 selectedLoc = SelectedLoc(Mouse.GetPosition(Origin));
            if (selectedLoc == null)
            {
                PreviewHigh.Visibility = PreviewLow.Visibility = Visibility.Hidden;
            }
            else
            {
                // selectedLoc within bounds
                PreviewHigh.Visibility = PreviewLow.Visibility = Visibility.Visible;
                int x = (int)selectedLoc.X, y = (int)selectedLoc.Y, z = (int)selectedLoc.Z;
                Point previewHighPt = cam.MapToScreen(selectedLoc);
                Canvas.SetLeft(PreviewHigh, previewHighPt.X); Canvas.SetTop(PreviewHigh, previewHighPt.Y);
                int maxheight = baseMap.GetHeight(x, y);
                Point previewLowPt = cam.MapToScreen(new Vector3(x, y, Math.Min(maxheight,z)));
                Canvas.SetLeft(PreviewLow, previewLowPt.X); Canvas.SetTop(PreviewLow, previewLowPt.Y);
            }
        }

        private void PaintCurrentTile(bool insertDuplicate = true)
        {
            Vector3 selectedLoc = SelectedLoc(Mouse.GetPosition(Origin));
            if (selectedLoc == null) return;
            // selectedLoc within bounds
            int x = (int)selectedLoc.X, y = (int)selectedLoc.Y, z = (int)selectedLoc.Z;
            if (currentTile == null)
            { // Eraser
                RemoveTile(x, y, z);
            }
            else
            {
                InsertTile(x, y, z, currentTile);
            }
            //Map.TileStack col = baseMap.tiles[y][x];
            /*int zimin = baseMap.GetTileIndexByHeight(x, y, z), zimax = baseMap.GetTileIndexByHeight(x, y, z + 1);
            if (z == -1 && col.Count == 0 && CanInsertTile(col, zimax, currentTile))
            {
                InsertTile(col, zimax, currentTile);
                RebuildMap();
            }*/
        }

        /*private bool CanReplaceTile(Map.TileStack col, int index, Map.TileReference t)
        {
            return index < col.Count;
        }*/
        private void ReplaceTile(Map.TileStack col, int index, Map.TileReference t) // requires rebuild of map after
        {
            col[index] = t;
            OnModify();
        }

        /*private bool CanInsertTile(Map.TileStack col, int index, Map.TileReference t)
        {
            return index <= col.Count;
        }*/
        /*private void InsertTile(Map.TileStack col, int index, Map.TileReference t) // requires rebuild of map after
        {
            col.Insert(index, t);
        }*/
        private void InsertTile(int x, int y, int z, Map.TileReference t)
        {
            Map.TileStack col = baseMap.tiles[y][x];
            int zindex = baseMap.GetTileIndexByHeight(x, y, z);
            Tile tile = baseMap.GetTile(t);
            if (tile.IsSlope && zindex != col.Count) return;
            if (zindex > 0 && baseMap.GetTile(col[zindex - 1]).IsSlope) return;
            if (!tile.passable && zindex != col.Count && baseMap.GetTile(col[zindex]).passable) return;
            if (zindex > 0 && !baseMap.GetTile(col[zindex - 1]).passable && tile.passable) return;
            col.Insert(zindex, t);
            OnModify();
            RebuildMap();
        }

        /*private bool CanRemoveTile(Map.TileStack col, int index)
        {
            return index < col.Count;
        }*/
        private void RemoveTile(int x, int y, int z)
        {
            Map.TileStack col = baseMap.tiles[y][x];
            int zindex = baseMap.GetTileIndexByHeight(x, y, z);
            if (zindex == 0) return;
            zindex--;
            col.RemoveAt(zindex);
            OnModify();
            RebuildMap();
        }

        private Vector3 SelectedLoc(Point p)// p relative to Origin
        {
            IsoRay ray = cam.ScreenToWorldRay(new Vector(p.X, p.Y));
            /*Vector3[] poss = { ray.AtX(clipPt.X), ray.AtY(clipPt.Y), ray.AtZ(clipPt.Z) };
            //Vector3 left = ray.AtX(clipPt.X), right = ray.AtY(clipPt.Y), top = ray.AtZ(clipPt.Z);
            for (int i = 0; i < poss.Length; i++)
            {
                if (0 < poss[i].X && 0 < poss[i].Y && -1 < poss[i].Z && poss[i].X <= clipPt.X && poss[i].Y <= clipPt.Y && poss[i].Z <= clipPt.Z)
                {
                    return new Vector3(Math.Ceiling(poss[i].X)-1, Math.Ceiling(poss[i].Y)-1, Math.Ceiling(poss[i].Z)-1);
                }
            }*/
            Vector3 top = ray.AtZ(clipZ);
            if (0 <= top.X && 0 <= top.Y && top.X < baseMap.width && top.Y < baseMap.height)
            {
                return new Vector3(Math.Floor(top.X), Math.Floor(top.Y), top.Z);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}