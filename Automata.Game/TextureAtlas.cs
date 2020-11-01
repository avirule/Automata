using System.Collections.Generic;
using System.IO;
using Automata.Engine;
using Automata.Engine.Numerics;
using Automata.Engine.Rendering.OpenGL.Textures;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Automata.Game
{
    public class TextureAtlas : Singleton<TextureAtlas>
    {
        private readonly Dictionary<string, int> _TextureDepths;

        public Texture2DArray<Rgba32>? Blocks { get; private set; }

        public TextureAtlas() => _TextureDepths = new Dictionary<string, int>();

        public void Initialize(IList<(string group, string path)> texturePaths)
        {
            const string group_with_sprite_name_format = "{0}:{1}";

            Blocks = new Texture2DArray<Rgba32>(8u, 8u, (uint)texturePaths.Count, Texture.WrapMode.Repeat, Texture.FilterMode.Point);

            int depth = 0;

            foreach ((string group, string path) in texturePaths)
            {
                Image<Rgba32> sprite = Image.Load<Rgba32>(path);
                Blocks.SetPixels(new Vector3i(0, 0, depth), new Vector2i(8, 8), ref sprite.GetPixelRowSpan(0)[0]);

                string formattedName = string.Format(group_with_sprite_name_format, group, Path.GetFileNameWithoutExtension(path));

                // it shouldn't be too uncommon for multiple identical paths to be parsed out
                // as it just means multiple blocks are using the same texture
                if (_TextureDepths.ContainsKey(formattedName))
                {
                    continue;
                }

                if (_TextureDepths.TryAdd(formattedName, depth))
                {
                    Log.Debug(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(TextureAtlas),
                        $"Registered texture: \"{formattedName}\" depth {depth}"));
                }
                else
                {
                    Log.Warning(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(TextureAtlas),
                        $"Failed to register texture: \"{formattedName}\" depth {depth}"));
                }

                depth += 1;
            }

            Log.Debug(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(TextureAtlas), $"Registered {_TextureDepths.Count} textures."));
        }

        public int GetTileDepth(string tileName) => _TextureDepths[tileName];
    }
}