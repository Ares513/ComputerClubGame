using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MapProcessorLib
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// </summary>
    [ContentImporter(".tls", DisplayName = "TileSet Importer")]
    public class TileSetImporter : ContentImporter<TileSetSource>
    {
        public override TileSetSource Import(string filename, ContentImporterContext context)
        {
            return new TileSetSource(System.IO.File.ReadAllBytes(filename));
        }
    }

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// </summary>
    [ContentImporter(".map", DisplayName = "Map Importer")]
    public class MapImporter : ContentImporter<MapSource>
    {
        public override MapSource Import(string filename, ContentImporterContext context)
        {
            return new MapSource(System.IO.File.ReadAllBytes(filename));
        }
    }
    
    public class TileSetSource
    {
        public byte[] data;
        public TileSetSource(byte[] source)
        {
            data = source;
        }
    }

    public class MapSource
    {
        public byte[] data;
        public MapSource(byte[] source)
        {
            data = source;
        }
    }

    [ContentTypeWriter]
    class TileSetWriter : ContentTypeWriter<TileSetSource>
    {
        protected override void Write(ContentWriter output, TileSetSource value)
        {
            output.Write(value.data);
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TileSetSource).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ComputerClubGame.TileSetReader, ComputerClubGame," +
                " Version=1.0.0.0, Culture=neutral";
        }
    }

    [ContentTypeWriter]
    class MapWriter : ContentTypeWriter<MapSource>
    {
        protected override void Write(ContentWriter output, MapSource value)
        {
            output.Write(value.data);
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(MapSource).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ComputerClubGame.MapReader, ComputerClubGame," +
                " Version=1.0.0.0, Culture=neutral";
        }
    }
}
