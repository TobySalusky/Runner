﻿using System;
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
        public Rectangle atlasRect; // TODO:

        public const int pixelCount = 16;
        public const float pixelSize = 1 / 16F;

        public int layer;
        
        public bool solid;

        // TODO: add layer color skins
        public static Color[] baseLayerColors = {new Color(179, 153, 218), new Color(172, 195, 214), new Color(166, 202, 162)};
        //public static Color[] baseLayerColors = {Color.DarkGray, Color.Cyan, Color.DarkGray};
        //public static Color[] baseLayerColors = {Color.HotPink, Color.LightPink, Color.LightGray};
        public static Color[] layerColors = new Color[3];
        
        static Tile() {
            genAtlas();
        }

        public enum type {
            
            // blocks
            Air,
            StoneBrick,
            Glass,
            Spike,
            Button,
            Door,
            NextStage,
            CrackedBrick,
            GravityButton,
            Rubber
        }

        public static readonly type[] nonSolid = {
            type.Air,
            type.Spike,
            type.Button,
            type.NextStage,
            type.GravityButton
        };
        
        public static readonly type[] nonFullBlock = {
            type.Air,
            type.Spike,
            type.Button,
            type.NextStage,
            type.GravityButton
        };
        
        public static type[] transparent = {
            type.Air,
            type.Glass,
            type.NextStage,
            type.Door,
        };

        public static Dictionary<Color, int> genTileTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.Red, type.StoneBrick); // TODO: add cracked brick
            tableAdd(table, Color.White, type.Glass);
            tableAdd(table, Color.Black, type.Spike);
            tableAdd(table, Color.Blue, type.Button);
            tableAdd(table, new Color(0F, 1F, 0F, 1F), type.Door);
            tableAdd(table, new Color(1F, 1F, 0F, 1F), type.NextStage);
            tableAdd(table, Color.Aqua, type.CrackedBrick);
            tableAdd(table, Color.Purple, type.GravityButton);
            tableAdd(table, Color.Pink, type.Rubber);

            return table;
        }

        public static void exportTilePalette() {
            Textures.exportTexture(scaleUp(tilePalette(genTileTable()), 3), Paths.texturePath, "TilePalette");
        }

        public static Texture2D scaleUp(Texture2D texture, int mult) {
            Texture2D newTexture = new Texture2D(Runner.getGraphicsDeviceManager(), texture.Width * mult, texture.Height * mult);
            
            var col = new Color[newTexture.Width * newTexture.Height];

            var oldCol = Util.colorArray(texture);
            for (int x = 0; x < texture.Width; x++) {
                for (int y = 0; y < texture.Height; y++) {
                    Color thisCol = oldCol[x + y * texture.Width];

                    for (int i = 0; i < mult; i++) {
                        for (int j = 0; j < mult; j++) {
                            int index = (x * mult + i) + (y * mult + j) * newTexture.Width;
                            col[index] = thisCol;
                        }
                    }
                }
            }

            newTexture.SetData(col);
            return newTexture;
        }
        
        public static Texture2D tilePalette(Dictionary<Color, int> dict) {
            Texture2D texture = new Texture2D(Runner.getGraphicsDeviceManager(), 4, dict.Keys.Count / 4 + 1);
            
            var col = new Color[texture.Width * texture.Height];

            for (int i = 0; i < col.Length; i++) { 
                col[i] = new Color(1F,1F,1F,0F);
            }

            int j = 0;
            foreach (Color key in dict.Keys) {
                col[j] = key;
                j++;
            }

            texture.SetData(col);
            return texture;
        }

        public static void genAtlas() {

            var types = Util.GetValues<type>();

            fullAtlas = new Texture2D(Runner.getGraphicsDeviceManager(), 48, 48 * (types.Count() - 1));
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
            solid = !nonSolid.Contains(tileType);

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
            
            if (tileType == type.Air || tileType == type.NextStage) return;

            float mult = camera.farMult(zPos());
            int drawSize = (int) Math.Round(camera.scale * mult);
            Vector2 drawPos = camera.screenCenter + (pos - camera.pos) * camera.scale * mult;
            
            Rectangle rect = new Rectangle((int) Math.Round(drawPos.X), (int) Math.Round(drawPos.Y), drawSize, drawSize);
                
            spriteBatch.Draw(texture, rect, atlasRect, layerColors[layer]);
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
            atlasRect.Y += 48 * (int) (tileType - 1);
        }

        public virtual int findAtlasIndex() {

            if (tileType == type.Spike) { // DIRECTIONAL BLOCKS
                if (!airBelow())
                    return 1;
                if (!airAbove())
                    return 7;
                if (!airRight())
                    return 3;
                if (!airLeft())
                    return 5;
                return 1;
            }

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

        public bool isAirAt(Vector2 pos, int layer) {

            type tileType = Runner.map.getRawTile(pos, layer).tileType;
            return nonFullBlock.Contains(tileType);
        }
        
        public bool solidAt(Vector2 pos, int layer) {

            return
                Runner.map.getRawTile(pos, layer).isSolid();
        }

        public bool sameAt(Vector2 pos, int layer) {

            return
                Runner.map.getRawTile(pos, layer).tileType == tileType;
        }
        public bool sameAbove() {
            return sameAt(pos - Vector2.UnitY, layer);
        }
        
        public bool sameBelow() {
            return sameAt(pos + Vector2.UnitY, layer);
        }
        
        public bool sameLeft() {
            return sameAt(pos - Vector2.UnitX, layer);
        }
        
        public bool sameRight() {
            return sameAt(pos + Vector2.UnitX, layer);
        }
        
        
        public bool airAbove() {
            return isAirAt(pos - Vector2.UnitY, layer);
        }
        
        public bool airBelow() {
            return isAirAt(pos + Vector2.UnitY, layer);
        }
        
        public bool airLeft() {
            return isAirAt(pos - Vector2.UnitX, layer);
        }
        
        public bool airRight() {
            return isAirAt(pos + Vector2.UnitX, layer);
        }

        public virtual void update(float deltaTime) {
            
        }

        public virtual void buttonPulsed() { }

        public virtual bool updateNeeded() {
            return false;
        }

        public virtual void blockUpdate() { }
    }

    public class FallingBlock : Tile {
        public FallingBlock(type tileType, Vector2 pos, int layer) : base(tileType, pos, layer) { }

        public bool triggered;
        public float timeLeft;

        public override int findAtlasIndex() {
            if (!airLeft() && !airRight()) return 1;
            if (!airRight()) return 0;

            return 2;
        }

        public override bool updateNeeded() {
            return true;
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);

            if (triggered) {
                timeLeft -= deltaTime;

                if (timeLeft <= 0) { 
                    Runner.map.removeBlock(pos, Vector2.Zero, layer); // TODO: only make chunk update while trigger is true
                    chainFall(-Vector2.UnitX, layer);
                    chainFall(Vector2.UnitX, layer);
                    chainFall(-Vector2.UnitY, layer);
                    chainFall(Vector2.UnitY, layer);
                    if (layer != 0) chainFall(Vector2.Zero, layer - 1);
                    if (layer != 2) chainFall(Vector2.Zero, layer + 1);
                }
            }
        }

        public void chainFall(Vector2 offset, int layer) {
            Tile tile = Runner.map.getRawTile(pos + offset, layer);
            if (tile.tileType == tileType) {
                ((FallingBlock)tile).startFall();
            }
            else {
                tile.blockUpdate();
            }
        }

        public void startFall() {
            if (!triggered) {
                triggered = true;
                timeLeft = 0.5F;
            }
        }

        public override void buttonPulsed() {
            startFall();
        }
    }

    public class Door : Tile {
        public Door(type tileType, Vector2 pos, int layer) : base(tileType, pos, layer) {
        }


        public override int findAtlasIndex() {
            int index;
            if (sameAbove() && sameBelow())
                index = 3;
            else if (sameBelow())
                index = 0;
            else
                index = 6;

            if (solid) index++;

            return index;
        }

        public override void buttonPulsed() {
            solid = !solid;
            findTexture();
            SoundPlayer.play("DoorFlip");
        }
        
        
    }
    
    public class GravitySwitcher : Tile {

        public bool pressed;
        
        public GravitySwitcher(type tileType, Vector2 pos, int layer) : base(tileType, pos, layer) {
        }

        public override int findAtlasIndex() {
            int index = pressed ? 1 : 0;

            if (!airAbove()) index += 3;

            return index;
        }

        public void activate() {
            if (!pressed) {
                SoundPlayer.play("ButtonPress");
                pressed = true;
                Runner.player.gravityDir = (!airBelow()) ? -1 : 1;

                findTexture();
            }
        }
        
        public override void blockUpdate() {
            if (airAbove() && airBelow()) {
                Runner.map.removeBlock(pos, Vector2.Zero, layer);
            }
        }
    }

    public class ButtonTile : Tile {

        public Button button = new Button();
        public ButtonTile(type tileType, Vector2 pos, int layer) : base(tileType, pos, layer) {
        }

        public override int findAtlasIndex() {
            if (button.pressed) {
                return 1;
            }
            return 0;
        }

        public void activate() {
            button.activate();
            findTexture();
        }

        public class Button {
            public List<Action> actions = new List<Action>();
            public bool pressed;

            public void activate() {

                if (!pressed) {
                    pressed = true;
                    foreach (var action in actions) {
                        action.Invoke();
                    }
                    
                    SoundPlayer.play("ButtonPress");
                }

            }
        }
    }
}