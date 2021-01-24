using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class Chunk {

        public static int[][,] mapData; // TODO: do this in chunks or something (perhaps load on the fly) because this can be a huge memory-use
        
        public const int chunkSize = 8;
        public readonly Tile[,,] tiles = new Tile[chunkSize, chunkSize, 3];
        
        private readonly Vector2 indices; // should always be ints

        public bool loaded, updateNeeded;
        
        public Chunk(Vector2 indices) {
            this.indices = indices;

            genTiles();
        }

        public static void loadMapData() { // TODO: change colors to ints here

            mapData = new int[3][,];
            for (int k = 0; k < 3; k++) {
                mapData[k] = loadColorMap(Textures.get("MapData" + k), Tile.genTileTable());
            }
        }

        public static int[,] loadColorMap(Texture2D texture, Dictionary<Color, int> table) { // TODO: change colors to ints here
            
            var colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            
            var arrayID = new int[texture.Width,texture.Height];
            for (int row = 0; row < texture.Height; row++)
            {
                for (int col = 0; col < texture.Width; col++)
                {
                    arrayID[col, row] = colorToID(table, colorData[row * texture.Width + col]);
                }
            }

            return arrayID;
        }

        private void genTiles() {
            Vector2 topLeft = indices * chunkSize;
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    for (int k = 0; k < 3; k++) {
                        Tile tile = genTile((int) (topLeft.X + i), (int) (topLeft.Y + j), k);
                        tiles[i, j, k] = tile;
                        if (tile.updateNeeded())
                            updateNeeded = true;
                    }
                }
            }
        }

        public void load() {
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    for (int k = 0; k < 3; k++) {
                        tiles[i, j, k].findTexture();
                    }
                }
            }
            
            loaded = true;
        }

        public void update(float deltaTime) {

            if (!updateNeeded) return;
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    for (int k = 0; k < 3; k++) {
                        tiles[i, j, k].update(deltaTime);
                    }
                }
            }
        }

        private Tile genTile(int x, int y, int layer) {
            const int airID = (int) Tile.type.Air;
            int ID = airID;

            int[,] layerData = mapData[layer];
            if (x >= 0 && x < layerData.GetLength(0) && y >= 0 && y < layerData.GetLength(1)) {
                ID = layerData[x, y];
            }

            //ID = (int) ((y > 5) ? Tile.type.StoneBrick : Tile.type.Air);

            Tile.type tileType = (Tile.type) ID;

            if (tileType == Tile.type.Button)
                return new ButtonTile(tileType, new Vector2(x, y), layer);
            
            if (tileType == Tile.type.Door)
                return new Door(tileType, new Vector2(x, y), layer);
            

            return new Tile(tileType, new Vector2(x, y), layer);
        }

        private static int colorToID(Dictionary<Color, int> table, Color color) {
            
            return table.GetValueOrDefault(color, (int) Tile.type.Air);
        }

        public void render(Camera camera, SpriteBatch spriteBatch, int layer) {
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    tiles[i, j, layer].render(camera, spriteBatch);
                }
            }
        }
    }
}