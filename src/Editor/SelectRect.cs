using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class SelectRect {
        public Rectangle rect { get; set; }
        public int layer { get; set; }

        public SelectRect() { }

        public SelectRect (Rectangle rect, int layer) {
            this.rect = rect;
            this.layer = layer;
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {
            
            Vector2 drawDimen = new Vector2(rect.Width, rect.Height) * camera.scaleAt(layer - 2);
            Vector2 drawTL = camera.toScreen(new Vector2(rect.X, rect.Y), layer - 2);

            spriteBatch.Draw(Textures.get("pixel"), Util.tl(drawTL, drawDimen), Color.Lerp(new Color(1F, 0F, 0F, 0.5F), Tile.baseLayerColors[layer], 0.5F));
        }

        public void applyAll(Action<Tile> tileFunc) {
            Point from = new Point(rect.X, rect.Y);
            Point to = from + new Point(rect.Width, rect.Height);

            for (int x = from.X; x <= to.X; x++) {
                for (int y = from.Y; y <= to.Y; y++) {
                    Tile tile = Runner.map.getRawTile(new Point(x, y), layer);
                    tileFunc.Invoke(tile);
                }
            }
        }
        
        public void applyTrigger(List<SelectRect> wiredTo) {
            Point from = new Point(rect.X, rect.Y);
            Point to = from + new Point(rect.Width, rect.Height);

            Tile first = Runner.map.getRawTile(from, layer);

            Action<Tile> tileFunc = tile => DebugStats.uselessActivationSelectRectTiles++;

            
            switch (first.tileType) {
                
                case Tile.type.Button:
                    ButtonTile.Button button = new ButtonTile.Button();
                    void wiredFunc(Tile tile) => tile.buttonPulsed();
                    foreach (var shouldTrigger in wiredTo) {
                        button.actions.Add(() => shouldTrigger.applyAll(wiredFunc));
                    }

                    tileFunc = tile => ((ButtonTile)tile).button = button;
                    break;
            }

            for (int x = from.X; x < to.X; x++) {
                for (int y = from.Y; y < to.Y; y++) {
                    Tile tile = Runner.map.getRawTile(new Point(x, y), layer);
                    if (tile.tileType == first.tileType) {
                        tileFunc.Invoke(tile);
                    }
                }
            }
        }
    }

    public class WireConnection {
        public Point from { get; set; }
        public Point to { get; set; }

        public WireConnection() {
        }

        public WireConnection(Point from, Point to) {
            this.from = from;
            this.to = to;
        }

        public override string ToString() {
            return from + " " + to;
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {

            Vector2 fromVec = from.ToVector2();
            Vector2 toVec = to.ToVector2();
            
            Vector2 diff = toVec - fromVec;

            int count = (int) Util.mag(diff);
            diff = Vector2.Normalize(diff);

            int layer = WiringEditor.editLayer;
            
            Vector2 drawDimen = Vector2.One * camera.scaleAt(layer - 2);

            for (int i = 0; i < count; i++) {
                
                Vector2 drawTL = camera.toScreen(fromVec + diff * i, layer - 2);

                spriteBatch.Draw(Textures.get("pixel"), Util.tl(drawTL, drawDimen), Color.Lerp(new Color(0F, 0F, 1F, 0.5F), Tile.baseLayerColors[layer], 0.5F));
            }
        }
    }
}