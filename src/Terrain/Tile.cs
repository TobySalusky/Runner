using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class Tile {

        public readonly type tileType;
        public readonly Vector2 pos; // marks top-left location

        public static Texture2D fullAtlas;
        public Texture2D texture;
        private Rectangle atlasRect; // TODO:

        public const int pixelCount = 8;
        public const float pixelSize = 1 / 8F;

        public int layer;
        
        public bool solid;

        static Tile() {
            genAtlas();
        }

        public enum type {
            
            // blocks
            Air,
            StoneBrick

        }

        public static Dictionary<Color, int> genTileTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.Red, type.StoneBrick);

            return table;
        }

        public static void genAtlas() {

            var types = Util.GetValues<type>();

            fullAtlas = new Texture2D(Runner.getGraphicsDeviceManager(), 24, 24 * (types.Count() - 1));
            var colorArr = new Color[fullAtlas.Width * fullAtlas.Height];

            int i = 0;
            foreach (var tile in types) {
                if (tile == type.Air) {
                    continue;
                }

                string identifier = tile.ToString();

                Texture2D atlas = (!Textures.has(identifier) && identifier.Contains("Back"))
                    ? genBackAtlas(identifier)
                    : Textures.get(identifier);
                var atlasCol = Util.colorArray(atlas);

                for (int x = 0; x < atlas.Width; x++) {
                    for (int y = 0; y < atlas.Height; y++) {

                        int index = x + y * atlas.Width;
                        int fullIndex = x + (y + atlas.Height * i) * atlas.Width;

                        colorArr[fullIndex] = atlasCol[index];
                    }
                }

                i++;
            }

            fullAtlas.SetData(colorArr);
        }

        private static Texture2D genBackAtlas(string identifier) {
            string blockIdentifier = identifier.Substring(0, identifier.IndexOf("Back"));

            Texture2D texture = Textures.get(blockIdentifier);
            var arr = Util.colorArray(texture);
            var newArr = new Color[arr.Length];

            for (int i = 0; i < arr.Length; i++) {
                Color col = arr[i];
                newArr[i] = Color.Lerp(arr[i], new Color(Color.Black, col.A / 255F), 0.3F);
            }

            var back = new Texture2D(Runner.getGraphicsDeviceManager(), texture.Width, texture.Height);
            back.SetData(newArr);

            return back;
        }

        private static void tableAdd(Dictionary<Color, int> dict, Color color, type tileType) {
            dict[color] = (int) tileType;
        }

        public Tile(type tileType, Vector2 pos, int layer) {

            this.tileType = tileType;
            this.pos = pos;
            
            solid = tileType != type.Air;

            this.layer = layer;
        }

        public bool isSolid() {
            return solid;
        }

        public void findTexture() {

            texture = fullAtlas;

            if (texture != null)
                findAtlasRect(findAtlasIndex());
        }

        public float zPos() {
            return layer - 2;
        }

        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: make more efficient
            
            if (tileType == type.Air) return;
            
            
            float mult = camera.farMult(zPos());
            int drawSize = (int) (camera.scale * mult);
            Vector2 drawPos = camera.screenCenter + (pos - camera.pos) * camera.scale * mult;
            
            Rectangle rect = new Rectangle((int) drawPos.X, (int) drawPos.Y, drawSize, drawSize);
                
            spriteBatch.Draw(texture, rect, atlasRect, Color.White);
        }

        /*public Rectangle textureAtlasRect() {
            return new Rectangle(0, 24 * (int) (tileType - 1), 24, 24);
        }*/

        private void findAtlasRect(int index) {
            int rows = 3, cols = 3;

            int col = index % cols;
            int row = index / cols;

            int size = texture.Width / cols; // assumes square blocks

            atlasRect = new Rectangle(size * col, size * row, size, size);
            atlasRect.Y += 24 * (int) (tileType - 1);
        }

        private int findAtlasIndex() {

            if (airAbove() && !airBelow() && airLeft() && !airRight())
                return 0;
            if (airAbove() && !airBelow() && !airLeft() && !airRight())
                return 1;
            if (airAbove() && !airBelow() && !airLeft() && airRight())
                return 2;
            if (!airAbove() && !airBelow() && airLeft() && !airRight())
                return 3;
            if (!airAbove() && !airBelow() && !airLeft() && airRight())
                return 5;
            if (!airAbove() && airBelow() && airLeft() && !airRight())
                return 6;
            if (!airAbove() && airBelow() && !airLeft() && !airRight())
                return 7;
            if (!airAbove() && airBelow() && !airLeft() && airRight())
                return 8;
            
            return 4;
        }

        private bool isAirAt(Vector2 pos, int layer) {

            return
                Runner.map.getRawTile(pos, layer).tileType == type.Air;
        }
        
        private bool solidAt(Vector2 pos, int layer) {

            return
                Runner.map.getRawTile(pos, layer).isSolid();
        }

        private bool sameAt(Vector2 pos, int layer) {

            return
                Runner.map.getRawTile(pos, layer).tileType == tileType;
        }
        private bool sameAbove() {
            return sameAt(pos - Vector2.UnitY, layer);
        }
        
        private bool sameBelow() {
            return sameAt(pos + Vector2.UnitY, layer);
        }
        
        private bool sameLeft() {
            return sameAt(pos - Vector2.UnitX, layer);
        }
        
        private bool sameRight() {
            return sameAt(pos + Vector2.UnitX, layer);
        }
        
        
        private bool airAbove() {
            return isAirAt(pos - Vector2.UnitY, layer);
        }
        
        private bool airBelow() {
            return isAirAt(pos + Vector2.UnitY, layer);
        }
        
        private bool airLeft() {
            return isAirAt(pos - Vector2.UnitX, layer);
        }
        
        private bool airRight() {
            return isAirAt(pos + Vector2.UnitX, layer);
        }
    }
}