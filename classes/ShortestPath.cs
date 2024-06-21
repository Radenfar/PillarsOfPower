using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.classes
{
    public class ShortestPath
    {
        private List<Tile> tileList;

        public ShortestPath(List<Tile> tileList)
        {
            this.tileList = tileList;
        }

        public int Get(Tile tile)
        {
            foreach (Tile visitedTile in tileList)
            {
                visitedTile.IsVisited = false;
            }

            if (tile.Terrain == "Ocean")
            {
                return 0;
            }

            int distance = 1;
            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(tile);

            while (queue.Count > 0)
            {
                int queueCount = queue.Count;
                for (int i = 0; i < queueCount; i++)
                {
                    Tile current = queue.Dequeue();

                    if (current.BorderingTiles.Count < 8)
                    {
                        return distance;
                    }

                    if (!current.IsVisited)
                    {
                        current.IsVisited = true;
                        foreach (Tile borderTile in current.BorderingTiles)
                        {
                            queue.Enqueue(borderTile);
                        }
                    }
                }

                distance++;
            }

            return distance;
        }
    }
}
