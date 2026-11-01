
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nothing_bad_happens_here_in_the_factory;

    public class TextureRegion
    {
        public Texture2D Texture { get; }
        public Rectangle SourceRectangle { get; }

        public int Width => SourceRectangle.Width;
        public int Height => SourceRectangle.Height;

        public TextureRegion(Texture2D texture, Rectangle sourceRectangle)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
        }
    }

