using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Nothing_bad_happens_here_in_the_factory;

    public class Tileset
    {
        private readonly TextureRegion[] _tiles;

        /// Gets the width, in pixels, of each tile in this tileset.
        public int AnchoTile { get; }
        /// Gets the height, in pixels, of each tile in this tileset.
        public int LargoTile { get; }
        /// Gets the total number of columns in this tileset.
        public int Columnas { get; }
        /// Gets the total number of rows in this tileset.
        public int Filas { get; }
        /// Gets the total number of tiles in this tileset.
        public int Count { get; }
        public Tileset(TextureRegion textureRegion, int anchoTile, int largoTile)
        {
            AnchoTile = anchoTile;
            LargoTile = largoTile;
            Columnas = textureRegion.Width / anchoTile;
            Filas = textureRegion.Height / largoTile;
            Count = Columnas * Filas;

            // Create the texture regions that make up each individual tile
            _tiles = new TextureRegion[Count];

            for (int i = 0; i < Count; i++)
            {
                int x = i % Columnas * anchoTile;
                int y = i / Columnas * largoTile;
                _tiles[i] = new TextureRegion(textureRegion.Texture,new Rectangle(textureRegion.SourceRectangle.X + x, textureRegion.SourceRectangle.Y + y, anchoTile, largoTile) );
            }
        }
        public TextureRegion GetTile(int index) => _tiles[index]; //devuelve el texture region del tile segun el indice indicado
        public TextureRegion GetTile(int column, int row) //devuelve el texture region segun la columna y la fila indicada
        {
            int index = row * Columnas + column;
            return GetTile(index);
        }
    }
