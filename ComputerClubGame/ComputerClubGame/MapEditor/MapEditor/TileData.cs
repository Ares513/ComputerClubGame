using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace MapEditor
{
    public class Map
    {
        public class TileReference { 
            public int TileSetIndex, TileIndex; 
            public TileReference(int tset, int tile) { TileSetIndex = tset; TileIndex = tile; } 
        }
        public class TileStack : List<TileReference> { 
            
        }

        public List<TileSet> tileSets = new List<TileSet>();
        public List<List<TileStack>> tiles; // indexed tiles[y][x]
        public int width = 0, height = 0;

        public Map()
        {
            tiles = new List<List<TileStack>>();
            width = 0; height = 0;
        }
        public Map(int width, int height)
        {
            tiles = new List<List<TileStack>>(height);
            for (int i = 0; i < height; i++)
            {
                tiles.Add(new List<TileStack>(width));
                for (int j = 0; j < width; j++) tiles[i].Add(new TileStack());
            }
            this.width = width; this.height = height;
        }

        public void InsertRow(int index) {
            tiles.Insert(index, new List<TileStack>(width));
            for (int j = 0; j < width; j++) tiles[index].Add(new TileStack());
            height++;
        }
        public void InsertColumn(int index)
        {
            for (int i = 0; i < height; i++) tiles[i].Insert(index, new TileStack());
            width++;
        }
        public void RemoveRow(int index)
        {
            tiles.RemoveAt(index);
            height--;
        }
        public void RemoveColumn(int index)
        {
            for (int y = 0; y < height; y++) tiles[y].RemoveAt(index);
            width--;
        }

        public Tile GetTile(TileReference t)
        {
            return tileSets[t.TileSetIndex].getTileAt(t.TileIndex);
        }

        public int GetTileIndexByHeight(int x, int y, int height) {
            int h = 0;
            int i = 0;
            for (i = 0; i < tiles[y][x].Count; i++)
            {
                if (!GetTile(tiles[y][x][i]).IsFloor) h++;
                if (h > height) break;
            }
            return i;
        }

        public int GetHeight(int x, int y, int index)
        {
            int h = 0;
            for (int i = 0; i < index; i++)
            {
                if (GetTile(tiles[y][x][i]).IsBlock) h++;
            }
            return h;
        }

        public int GetHeight(int x, int y)
        {
            return GetHeight(x, y, tiles[y][x].Count);
        }

        public delegate TileSet GetTileSetFromFileName(string filename);
        public static Map FromStream(Stream s, GetTileSetFromFileName gettset)
        {
            int numtsets = s.ReadByte();
            List<string> tsetfiles = new List<string>(numtsets);
            for (int i = 0; i < numtsets; i++)
            {
                tsetfiles.Add(MainWindow.ReadFilePath(s, ".tls"));
            }
            Map m = new Map(ReadHalf(s), ReadHalf(s));
            foreach (string path in tsetfiles)
            {
                TileSet tset = gettset(path);
                if (tset == null) return null;
                m.tileSets.Add(tset);
            }
            for (int y = 0; y < m.height; y++)
            {
                for (int x = 0; x < m.width; x++)
                {
                    int tcount = ReadHalf(s);
                    for (int i = 0; i < tcount; i++)
                    {
                        m.tiles[y][x].Add(new TileReference(s.ReadByte(), s.ReadByte()));
                    }
                }
            }
            return m;
        }
        public void ToStream(Stream s)
        {
            s.WriteByte((byte)tileSets.Count);
            for (int i = 0; i < tileSets.Count; i++)
            {
                MainWindow.WriteFilePath(s, tileSets[i].SaveTileSetPath);
            }
            WriteHalf(s, width); WriteHalf(s, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    WriteHalf(s,tiles[y][x].Count);
                    foreach (TileReference tref in tiles[y][x]) {
                        s.WriteByte((byte)tref.TileSetIndex); s.WriteByte((byte)tref.TileIndex);
                    }
                }
            }
        }

        // Big-endian
        private static int ReadHalf(Stream s)
        {
            return (s.ReadByte() << 8) | s.ReadByte();
        }
        private static void WriteHalf(Stream s, int x)
        {
            s.WriteByte((byte)((x >> 8) & 255)); s.WriteByte((byte)(x & 255));
        }
    }

    public class TileSet : List<Tile>
    {
        public string SaveTileSetPath = null;
        public string BaseTexturePath = null;

        public Tile getTileAt(int x, int y)
        {
            foreach (Tile t in this) {
                if (t.LeftX <= x && t.TopY <= y && t.RightX >= x && t.BottomY >= y) return t;
            }
            return null;
        }
        public Tile getTileAt(int index)
        {
            return getTileAt(index & 15, (index >> 4) & 15);
        }

        public static TileSet FromStream(Stream s)
        {
            TileSet tset = new TileSet();
            tset.BaseTexturePath = MainWindow.ReadFilePath(s,".png");
            int numTiles = s.ReadByte();
            for (int i = 0; i < numTiles; i++) tset.Add(Tile.FromStream(s));
            return tset;
        }

        public void ToStream(Stream s)
        {
            MainWindow.WriteFilePath(s, BaseTexturePath);
            s.WriteByte((byte)Count);
            foreach (Tile t in this) t.ToStream(s);
        }
    }

    public class Tile : INotifyPropertyChanged
    {
        private bool A = false, B = false, C = false, D = false;
        private bool _passable = true;
        //private bool _topper = false;
        private int _originX, _originY, _leftX, _topY, _rightX, _bottomY;

        public bool back { get { return A; } set { if (A != value) { A = value; NotifyPropertyChanged("back"); } } }
        public bool left { get { return B; } set { if (B != value) { B = value; NotifyPropertyChanged("left"); } } }
        public bool right { get { return C; } set { if (C != value) { C = value; NotifyPropertyChanged("right"); } } }
        public bool front { get { return D; } set { if (D != value) { D = value; NotifyPropertyChanged("front"); } } }

        public bool passable { get { return _passable; } set { if (_passable != value) { _passable = value; NotifyPropertyChanged("passable"); } } }
        //public bool topper { get { return _topper; } set { if (_topper != value) { _topper = value; NotifyPropertyChanged("topper"); } } }

        public int PositionX { get { return _originX; } set { OriginX = value; LeftX = value; RightX = value; } }
        public int PositionY { get { return _originY; } set { OriginY = value; TopY = value; BottomY = value; } }

        public int OriginX { get { return _originX; } set { if (_originX != value) { _originX = value; NotifyPropertyChanged("OriginX"); } } }
        public int OriginY { get { return _originY; } set { if (_originY != value) { _originY = value; NotifyPropertyChanged("OriginY"); } } }
        public int OriginIndex { get { return OriginX | (OriginY << 4); } set { OriginX = value & 15; OriginY = (value >> 4) & 15; } }
        public int LeftX { get { return _leftX; } set { if (_leftX != value) { _leftX = value; NotifyPropertyChanged("LeftX"); } } }
        public int RightX { get { return _rightX; } set { if (_rightX != value) { _rightX = value; NotifyPropertyChanged("RightX"); } } }
        public int TopY { get { return _topY; } set { if (_topY != value) { _topY = value; NotifyPropertyChanged("TopY"); } } }
        public int BottomY { get { return _bottomY; } set { if (_bottomY != value) { _bottomY = value; NotifyPropertyChanged("BottomY"); } } }

        public bool IsBlock { get { return back && left && right && front; } }
        public bool IsFloor { get { return !back && !left && !right && !front; } }
        public bool IsSlope { get { return !IsBlock && !IsFloor; } }

        public bool IsLargeTile { get { return !(LeftX == OriginX && OriginX == RightX && TopY == OriginY && OriginY == BottomY); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "") 
            { if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }

        public static Tile FromStream(Stream s) {
            Tile t = new Tile();

            int b1 = s.ReadByte(), b2 = s.ReadByte();
            t.PositionX = b1 & 15; t.PositionY = (b1 >> 4) & 15;
            t.back = (b2 & 1) != 0; t.left = (b2 & 2) != 0; t.right = (b2 & 4) != 0; t.front = (b2 & 8) != 0;
            //t.topper = (b2 & 16) != 0; 
            t.passable = (b2 & 32) != 0;
            if ((b2 & 64) != 0)
            {
                int b3 = s.ReadByte(), b4 = s.ReadByte();
                t.LeftX = b3 & 15; t.TopY = (b3 >> 4) & 15;
                t.RightX = b4 & 15; t.BottomY = (b4 >> 4) & 15;
            }

            return t;
        }

        public void ToStream(Stream s)
        {
            s.WriteByte((byte)(OriginY << 4 | OriginX));
            s.WriteByte((byte)((back ? 1 : 0) | (left ? 2 : 0) | (right ? 4 : 0) | (front ? 8 : 0) | /*(topper ? 16 : 0) |*/ (passable ? 32 : 0) | (IsLargeTile ? 64 : 0)));
            if (IsLargeTile)
            {
                s.WriteByte((byte)(TopY << 4 | LeftX));
                s.WriteByte((byte)(BottomY << 4 | RightX));
            }
        }
    }

    public class IsoCamera {
        private Vector Offset;
    
        private Vector iVec = new Vector(-32, 16), jVec = new Vector(32, 16), kVec = new Vector(0, -32);
        private const int tileUnit = 32;

        public IsoCamera(Vector screenCoord, Vector3 mapCoord){
            Align(screenCoord, mapCoord);
        }

        public IsoCamera() {
            Align(new Vector(0,0), new Vector3(0,0,0));
        }

        public void Align(Vector screenCoord, Vector3 mapCoord) {
            Offset =
                mapCoord.X * iVec +
                mapCoord.Y * jVec +
                mapCoord.Z * kVec -
                screenCoord;
        }

        public Point MapToScreen(Vector3 mapCoord){
            return new Point(0, 0) + mapCoord.X * iVec + mapCoord.Y * jVec + mapCoord.Z * kVec - Offset;
        }

        public IsoRay ScreenToWorldRay(Vector screenCoord){
            return new IsoRay((screenCoord + Offset) / tileUnit);
        }

        public Vector3 ScreenToMap(Vector screenCoord, Single z){
            return ScreenToWorldRay(screenCoord).AtZ(z);
        }
    }

    public class IsoRay
    {
        private Vector3 XY, YZ, ZX;

        public IsoRay(Vector scaled)
        {
            XY = new Vector3(scaled.Y - scaled.X / 2, scaled.Y + scaled.X / 2, 0);
            YZ = new Vector3(0, scaled.X, -scaled.Y + scaled.X / 2);
            ZX = new Vector3(-scaled.X, 0, -scaled.Y - scaled.X / 2);
        }

        public Vector3 AtX(Double x)
        {
            return YZ + new Vector3(x);
        }
        public Vector3 AtY(Double y)
        {
            return ZX + new Vector3(y);
        }
        public Vector3 AtZ(Double z)
        {
            return XY + new Vector3(z);
        }
    }

    public class Vector3
    {
        public Double X,Y,Z;
        public Vector3(Double val) { X = Y = Z = val; }
        public Vector3(Double x, Double y, Double z) { X = x; Y = y; Z = z; }
        public static Vector3 operator +(Vector3 lhs, Vector3 rhs) { return new Vector3(lhs.X+rhs.X, lhs.Y+rhs.Y, lhs.Z+rhs.Z); }
    }
}