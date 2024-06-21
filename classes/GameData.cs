using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.classes
{
    public class GameData
    {
        public List<Tile> TileList { get; set; }
        public List<Nation> NationList { get; set; }


        public void SaveGameData()
        {
            string fileName = "Saves/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("GameData");
                file.WriteLine("TileList");
                foreach (Tile tile in TileList)
                {
                    file.WriteLine(tile.ToString());
                }
                file.WriteLine("NationList");
                foreach (Nation nation in NationList)
                {
                    file.WriteLine(nation.ToString());
                }
            }
        }
    }
}
