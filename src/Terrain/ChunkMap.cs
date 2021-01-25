using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public 
        class ChunkMap {
        public readonly Dictionary<Point, Chunk> chunks;

        public ChunkMap() {
            chunks = new Dictionary<Point, Chunk>();
        }

        public Chunk getChunk(Point indices) {

            Chunk chunk = getRawChunk(indices);

            if (!chunk.loaded)
                chunk.load();
            
            return chunk;
        }

        public void update(float deltaTime) {
            for (int layer = 0; layer < 3; layer++) {
                Camera camera = Runner.camera;
                Vector2 diff = camera.screenCenter / (camera.scale * camera.farMult(layer - 2));
                Point from = chunkIndices(camera.pos - diff);
                Point to = chunkIndices(camera.pos + diff);

                for (int i = from.X; i <= to.X; i++) {
                    for (int j = from.Y; j <= to.Y; j++) {
                        getChunk(new Point(i, j)).update(deltaTime);
                    }
                }
            }
        }

        public void removeBlocks(Vector2 position, Vector2 vel) {

            Point blockInd = ChunkMap.blockIndices(position);
            var (blockX, blockY) = blockInd;

            Point block = new Point(Util.intMod(blockInd.X, Chunk.chunkSize),
                Util.intMod(blockInd.Y, Chunk.chunkSize));
            var (x, y) = block;

            if (blockX < 0 || blockX >= mapWidth() || blockY < 0 || blockY >= mapHeight()) return;
            
            if (chunks.Keys.Contains(chunkIndices(position))) {

                Tile[,,] tiles = getChunk(position).tiles;

                Vector2 pos = tiles[x, y, 0].pos;
                for (int i = 0; i < 3; i++) {
                    Tile tile = tiles[x, y, i];

                    Camera camera = Runner.camera;
                    Vector2 diff = camera.screenCenter / (camera.scale * camera.farMult(i - 2));
                    

                    if (Util.between(pos, camera.pos - diff, camera.pos + diff)) { // checks if on-screen before creating particle
                        if (tile.tileType != Tile.type.Air)
                            Runner.particles[i].Add(new BlockParticle(tile, vel));
                    }

                    tiles[x, y, i] = new Tile(Tile.type.Air, pos, i);
                }
            }

            int ID = (int) Tile.type.Air;
            for (int i = 0; i < 3; i++) {
                Chunk.mapData[i][blockX, blockY] = ID;
            }
        }

        public static int mapWidth() {
            return Chunk.mapData[0].GetLength(0);
        }
        
        public static int mapHeight() {
            return Chunk.mapData[0].GetLength(1);
        }

        public Chunk getRawChunk(Point indices) { // returns chunk without fully loading it (generates texture-less tiles)
            chunks.TryGetValue(indices, out var chunk);

            if (chunk == null) {
                return addChunk(indices);
            }

            return chunk;
        }
        
        public Chunk getRawChunk(Vector2 position) { // finds chunk containing these coordinates

            return getRawChunk(chunkIndices(position));
        }

        public Chunk getChunk(Vector2 position) { // finds chunk containing these coordinates

            return getChunk(chunkIndices(position));
        }

        public static Point chunkIndices(Vector2 position) { // finds indices of chunk containing these coordinates
            var (x, y) = position / Chunk.chunkSize;
            return new Point((int) Math.Floor(x), (int) Math.Floor(y));
        }

        public static Point blockIndices(Vector2 position) {
            var (x, y) = position;
            return new Point((int) Math.Floor(x), (int) Math.Floor(y));
        }

        public Chunk addChunk(Point indices) {
            
            var (x, y) = indices;
            Chunk chunk = new Chunk(new Vector2(x, y));

            chunks[indices] = chunk;
            
            return chunk;
        }

        public void render(Camera camera, SpriteBatch spriteBatch, int layer) {

            for (int i = 0; i < 3; i++) {
                Tile.layerColors[i] = Runner.player.getLayer() == i
                    ? Tile.baseLayerColors[i]
                    : Color.Lerp(Tile.baseLayerColors[i], Color.Black, 0.5F);
            }
            
            Player player = Runner.player;
            const float min = 0.5F;
            Tile.layerColors[1] = new Color(Tile.layerColors[1], min + (1 - min) * (1 -player.midPercentOcluded));
            Tile.layerColors[2] = new Color(Tile.layerColors[2], min + (1 - min) * (1 - player.frontPercentOcluded));

            Vector2 diff = camera.screenCenter / (camera.scale * camera.farMult(layer - 2));
            Point from = chunkIndices(camera.pos - diff);
            Point to = chunkIndices(camera.pos + diff);

            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    getChunk(new Point(i, j)).render(camera, spriteBatch, layer);
                }
            }
        }

        public Tile getTile(Vector2 pos, int layer) {
            return getTile(blockIndices(pos), layer);
        }

        public Tile getTile(Point indices, int layer) {
            var (x, y) = indices;
            Chunk chunk = getChunk(chunkIndices(new Vector2(x, y)));
            return chunk.tiles[Util.intMod(x, Chunk.chunkSize), Util.intMod(y, Chunk.chunkSize), layer];
        }
        
        public Tile getRawTile(Vector2 pos, int layer) {
            return getRawTile(blockIndices(pos), layer);
        }
        
        public Tile getRawTile(Point indices, int layer) {
            var (x, y) = indices;
            Chunk chunk = getRawChunk(chunkIndices(new Vector2(x, y)));
            return chunk.tiles[Util.intMod(x, Chunk.chunkSize), Util.intMod(y, Chunk.chunkSize), layer];
        }


        public bool pointCollide(Vector2 pos, int layer) {
            return getTile(pos, layer).isSolid();
        }

        public bool rectangleCollide(Vector2 center, Vector2 dimen, int layer) { // TODO: optimise please (go chunk by chunk)
            Vector2 diff = dimen / 2;
            Point from = blockIndices(center - diff);
            Point to = blockIndices(center + diff);
            
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile tile = getTile(new Point(i, j), layer);

                    if (tile.isSolid())
                        return true;
                }
            }

            return false;
        }
    }
}