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
using System.ComponentModel;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for TileEditor.xaml
    /// </summary>
    public partial class TileEditor : UserControl
    {
        const int mouseBorder = 12;

        private bool editable = true;
        public bool Editable
        {
            get { return editable; }
            set
            {
                editable = value;
            }
        }

        public TileEditor()
        {
            InitializeComponent();
        }

        private Tile baseTile;
        public Tile Tile
        {
            get { return baseTile; }
            set
            {
                if (baseTile != value)
                {
                    if (baseTile != null) { baseTile.PropertyChanged -= HandleBaseTilePropertyChanged; baseTile = null; }
                    if (value != null) { 
                        baseTile = value; baseTile.PropertyChanged += HandleBaseTilePropertyChanged;
                        Refresh();
                    }
                }
            }
        }

        private Point backPoint { get { return new Point(32, (baseTile != null && baseTile.back) ? 0 : 32); } }
        private Point leftPoint { get { return new Point(0, (baseTile != null && baseTile.left) ? 16 : 48); } }
        private Point rightPoint { get { return new Point(64, (baseTile != null && baseTile.right) ? 16 : 48); } }
        private Point frontPoint { get { return new Point(32, (baseTile != null && baseTile.front) ? 32 : 64); } }

        public event EventHandler Changed;
        protected virtual void OnChanged()
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        private void Refresh() { CalculatePosition(); UpdateGraphics(); OnChanged(); }
        private void CalculatePosition()
        {
            Margin = new Thickness(64 * baseTile.LeftX, 64 * baseTile.TopY, 0, 0);
            Origin.Margin = new Thickness(64 * Math.Max(baseTile.OriginX - baseTile.LeftX, 0), 64 * Math.Max(baseTile.OriginY - baseTile.TopY, 0), 0, 0);
            Width = 64 * Math.Max(baseTile.RightX - baseTile.LeftX + 1, 0); Height = 64 * Math.Max(baseTile.BottomY - baseTile.TopY + 1, 0);
        }
        private void UpdateGraphics()
        {
            DownMid.Y1 = (backPoint.Y + leftPoint.Y) / 2; DownMid.Y2 = (rightPoint.Y + frontPoint.Y) / 2;
            UpMid.Y1 = (leftPoint.Y + frontPoint.Y) / 2; UpMid.Y2 = (backPoint.Y + rightPoint.Y) / 2;
            Topology.Resources["back"] = backPoint; Topology.Resources["left"] = leftPoint; Topology.Resources["right"] = rightPoint; Topology.Resources["front"] = frontPoint;
            
            UpMid.Visibility = DownMid.Visibility = (!baseTile.back && !baseTile.left && !baseTile.right && baseTile.front) ? Visibility.Hidden : Visibility.Visible;

            //Topology.Visibility = Skeleton.Visibility = baseTile.passable ? Visibility.Visible : Visibility.Hidden;
            Floor.Visibility = baseTile.passable ? Visibility.Hidden : Visibility.Visible;

            //TopperFill.Visibility = baseTile.topper ? Visibility.Visible : Visibility.Hidden;
        }
       
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!Editable) return;

            //if (!baseTile.passable) return;

            Point loc = e.GetPosition(Topology);

            if (IsMouseNear(loc, new Point(0, 16))) baseTile.left = true;
            if (IsMouseNear(loc, new Point(0, 48))) baseTile.left = false;
            if (IsMouseNear(loc, new Point(32, 0))) baseTile.back = true;
            if (IsMouseNear(loc, new Point(32, 32))) if (loc.Y < 32) baseTile.back = false; else baseTile.front = true;
            if (IsMouseNear(loc, new Point(32, 64))) baseTile.front = false;
            if (IsMouseNear(loc, new Point(64, 16))) baseTile.right = true;
            if (IsMouseNear(loc, new Point(64, 48))) baseTile.right = false;
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (!Editable) return;

            /*if (IsMouseOverFloor(e.GetPosition(Topology))) */baseTile.passable = !baseTile.passable;
            //else baseTile.topper = !baseTile.topper;
        }

        private bool IsMouseNear(Point mouseLoc, Point p) {
            return (mouseLoc - p).Length <= mouseBorder;
        }
        private bool IsMouseOverFloor(Point mouseLoc)
        {
            Vector p = mouseLoc - new Point(32, 48);
            return Math.Abs(p.Y*2)+Math.Abs(p.X) <= 32;
        }

        private void HandleBaseTilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Refresh();
        }
    }
}
