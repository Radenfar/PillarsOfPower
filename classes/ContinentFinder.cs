using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.classes
{
    public class ContinentFinder
    {
        private static List<Tile> TileList;

        public ContinentFinder(List<Tile> TileList)
        {
            ContinentFinder.TileList = TileList;
        }

        public static List<Tile> GetContinent()
        {
            List<Tile> continent = new List<Tile>();
            int rootIndex = 18 * 36 + 18; // Index of the center tile for a 36x36 map
            Tile rootTile = TileList[rootIndex];
            Stack<Tile> stack = new Stack<Tile>();
            stack.Push(rootTile);
            while (stack.Count > 0)
            {
                Tile current = stack.Pop();
                if (!current.IsVisited)
                {
                    current.IsVisited = true;
                    continent.Add(current);
                    foreach (Tile borderTile in current.BorderingTiles)
                    {
                        stack.Push(borderTile);
                    }
                }
            }
            return continent;
        }
    }
}
