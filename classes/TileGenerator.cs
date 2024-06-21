using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace PillarsOfPower.classes
{
    public class TileGenerator
    /* 
     - The map is generated as a 720x720 grid of 20x20 pixel tiles. The 20x20 pixels correspond to 20x20 base images for the terrain types.
     - Edge tiles (at 0 or 700 in x or y) are set as "Ocean".
     - The center tile is set as "Land".
     - The rest of the tiles are set as "Land" or "Ocean" based on a probability function that increases the likelihood of a tile being "Land" the closer it is to the center tile.
     - Each tile is given a list of bordering tiles and a list of expansion tiles. Bordering tiles are any tile that shares a side with the current tile. Expansion tiles are bordering tiles that are also "Land".
     */
    {
        private List<Tile> tileList;
        private static readonly Random random = new Random();

        public TileGenerator()
        {
            tileList = GetTiles();
        }

        private List<Tile> UpdateTileList(List<Tile> tileList, int x, int y, int index, string terrain)
        {
            List<int> curCoordinates = new List<int>() { x, y };
            string curTerrain = terrain;
            Tile newTile = new Tile(curCoordinates, index, curTerrain);
            tileList.Add(newTile);
            return tileList;
        }

        public List<Tile> GetTiles()
        {
            List<Tile> tileList = new List<Tile>();
            int index = 0;
            for (int y = 0; y < 720; y += 20)
            {
                for (int x = 0; x < 720; x += 20)
                {
                    if (x == 0 || x == 700 || y == 0 || y == 700)
                    {
                        tileList = UpdateTileList(tileList, x, y, index, "Ocean");
                        index++;
                    }
                    else if (x == 360 && y == 360)
                    {
                        tileList = UpdateTileList(tileList, x, y, index, "Land");
                        index++;
                    }
                    else
                    {
                        int xDifference = Math.Abs(x - 360);
                        int yDifference = Math.Abs(y - 360);
                        if (xDifference == 0)
                        {
                            tileList = UpdateTileList(tileList, x, y, index, "Land");
                            index++;
                        }
                        else
                        {
                            float xChance = ((float)xDifference * xDifference) / (360 * 360);
                            float yChance = ((float)yDifference * yDifference) / (360 * 360);
                            int xRandom = random.Next(1, 3);
                            int yRandom = random.Next(1, 3);
                            float chance = (xChance / xRandom) + (yChance / yRandom);
                            if ((random.NextDouble()) > chance)
                            {
                                tileList = UpdateTileList(tileList, x, y, index, "Land");
                                index++;
                            }
                            else
                            {
                                tileList = UpdateTileList(tileList, x, y, index, "Ocean");
                                index++;
                            }
                        }
                    }
                }
            }

            // Loop through and set borders and expansion tiles for land tiles.
            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].Terrain != "Ocean")
                {
                    List<Tile> newBorders = new List<Tile>();
                    List<Tile> expansionBorders = new List<Tile>();
                    int[] expansionBorderOffsets = new int[] { i - 36, i + 36, i - 1, i + 1 };
                    int[] offsets = new int[] { i - 35, i + 35, i + 37, i - 37 };
                    foreach (int offset in offsets)
                    {
                        if (tileList[offset].Terrain != "Ocean")
                        {
                            newBorders.Add(tileList[offset]);
                        }
                    }
                    foreach (int offset_ in expansionBorderOffsets)
                    {
                        if (tileList[offset_].Terrain != "Ocean")
                        {
                            newBorders.Add(tileList[offset_]);
                            expansionBorders.Add(tileList[offset_]);
                        }
                    }
                    tileList[i].SetBorders(newBorders);
                    tileList[i].SetExpansionTiles(expansionBorders);
                }
            }

            // If a land tile doesn't have any borders, set it as "Ocean".
            foreach (Tile curTile in tileList)
            {
                if (curTile.Terrain != "Ocean" && curTile.BorderingTiles.Count == 0)
                {
                    curTile.SetTerrain("Ocean");
                }
            }

            ContinentFinder cf = new ContinentFinder(tileList);
            List<Tile> continent = ContinentFinder.GetContinent();
            List<Tile> subsect = tileList.Where(element => !continent.Contains(element)).ToList();
            foreach (Tile island in subsect)
            {
                if (island.Terrain != "Ocean")
                {
                    island.SetTerrain("Ocean");
                    island.ZeroDev();
                }
            }
            // Use the border stuff to determine mountains, hills, etc.
            ShortestPath sp = new ShortestPath(tileList);
            foreach (Tile tile in tileList)
            {
                if (tile.Terrain == "Land")
                {
                    int distance = sp.Get(tile);
                    if (distance > 1)
                    {
                        double mdistancePerc = (double)distance / 20;
                        double mNewRandom = random.NextDouble();
                        if ((mdistancePerc > mNewRandom) && !(tile.Terrain == "Capital"))
                        {
                            tile.SetTerrain("Mountains");
                        }
                        double hdistancePerc = (double)distance / 5;
                        double hNewRandom = random.NextDouble();
                        if ((hdistancePerc > hNewRandom) && !(tile.Terrain == "Capital") && !(tile.Terrain == "Mountains"))
                        {
                            tile.SetTerrain("Hills");
                        }
                    }
                }
            }

            // Line algorithm for trees.
            List<Tile> landTiles = tileList.Where(tile => tile.Terrain == "Land").ToList();
            int R = 250;
            int N = landTiles.Count;
            int quotient = (N - 1) / (R - 1);
            int remainder = (N - 1) % (R - 1);
            index = 0;
            do
            {
                landTiles[index].SetTerrain("Forest");
            }
            while ((index += quotient + (remainder-- > 0 ? 1 : 0)) < N);

            // Finally, now terrain types have changed, update populations.
            foreach (Tile tile in tileList)
            {
                int NewDevelopment = 0;
                if (tile.Terrain == "Ocean")
                {
                    tile.ZeroDev();
                }
                else if (tile.Terrain == "Capital")
                {
                    NewDevelopment = random.Next(10, 30);
                }
                else if (tile.Terrain == "Land")
                {
                    NewDevelopment = random.Next(5, 25);
                }
                else if (tile.Terrain == "Forest")
                {
                    NewDevelopment = random.Next(5, 20);
                }
                else if (tile.Terrain == "Hills")
                {
                    NewDevelopment = random.Next(3, 15);
                }
                else
                {
                    NewDevelopment = random.Next(3, 5);
                }
                if (NewDevelopment > 0)
                {
                    int MinPopRange = 10000 + (32142 * (NewDevelopment - 3));
                    int MaxPopRange = MinPopRange + 32142;
                    int NewPopulation = random.Next(MinPopRange, MaxPopRange);
                    tile.SetDevPop(NewDevelopment, NewPopulation);
                }
            }
            return tileList;
        }
    }
}
