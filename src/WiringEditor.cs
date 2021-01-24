using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Point = Microsoft.Xna.Framework.Point;

namespace Runner {
    public class WiringEditor {

        public static int editLayer = 0;

        public List<SelectRect> rects { get; set; }
        public List<WireConnection> connections { get; set; }

        public Point clickStart, clickEnd;

        public bool selecting, wiring;

        public WiringEditor() {
            Logger.log(rects == null);
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {

            foreach (var rect in rects) {
                rect.render(camera, spriteBatch);
            }
            
            foreach (var wire in connections) {
                wire.render(camera, spriteBatch);
            }
            
            if (selecting)
                genRect().render(camera, spriteBatch);
            
            if (wiring)
                genWire().render(camera, spriteBatch);
        }

        public void saveWiring() {
            DataSerializer.Serialize("Wiring", this);
        }
        
        public SelectRect genRect() {

            int sX, sY, eX, eY;

            Point clickEnd = new Point(this.clickEnd.X + 1, this.clickEnd.Y + 1);
            
            if (clickStart.X < clickEnd.X) {
                sX = clickStart.X;
                eX = clickEnd.X;
            }
            else {
                sX = clickEnd.X;
                eX = clickStart.X;
            }
            
            if (clickStart.Y < clickEnd.Y) {
                sY = clickStart.Y;
                eY = clickEnd.Y;
            }
            else {
                sY = clickEnd.Y;
                eY = clickStart.Y;
            }

            return new SelectRect(new Rectangle(sX, sY, eX - sX, eY - sY), editLayer);
        }
        
        public WireConnection genWire() {

            return new WireConnection(clickStart, clickEnd);
        }

        public Point pointAt(Vector2 mousePos) {
            return ChunkMap.blockIndices(Runner.camera.toWorld(mousePos, editLayer - 2));
        }

        public void input(MouseInfo mouse, KeyInfo keys, float deltaTime) {

            if (keys.pressed(Keys.D1)) editLayer = 0;
            if (keys.pressed(Keys.D2)) editLayer = 1;
            if (keys.pressed(Keys.D3)) editLayer = 2;
            

            Point mousePoint = pointAt(mouse.pos);
            
            if (mouse.leftPressed) {
                if (keys.down(Keys.LeftShift)) {
                    wiring = true;
                }
                else {
                    selecting = true;
                }
                clickStart = mousePoint;
            }

            clickEnd = mousePoint;
            if (mouse.leftUnpressed) {
                if (selecting)
                    rects.Add(genRect());
                if (wiring)
                    connections.Add(genWire());
                
                selecting = false;
                wiring = false;
            }

            if (mouse.rightPressed) {
                Point pointClick = mousePoint;

                for (int i = rects.Count - 1; i >= 0; i--) {
                    if (rects[i].layer == editLayer && rects[i].rect.Contains(pointClick)) {
                        rects.RemoveAt(i);
                    }
                }
            }

            
            for (int i = connections.Count - 1; i >= 0; i--) {
                WireConnection wire = connections[i];
                bool hasFrom = false, hasTo = false;
                
                foreach (var rect in rects) {
                    Rectangle bounds = rect.rect;
                    
                    if (bounds.Contains(wire.from)) hasFrom = true;
                    if (bounds.Contains(wire.to)) hasTo = true;
                    
                }

                if (!hasFrom || !hasTo) {
                    connections.RemoveAt(i);
                }
            }
        }
    }
}