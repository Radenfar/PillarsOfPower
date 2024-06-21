using PillarsOfPower.classes;
using System.ComponentModel;

namespace PillarsOfPower
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            int Turn = 1;
            bool ToolTips = true;
            List<Tile> tileList = new TileGenerator().GetTiles();
            GameData gameData = new NationGenerator(tileList).GenerateNations();
            List<Nation> nationList = gameData.NationList;
            tileList = gameData.TileList;
            Application.Run(new GameForm(tileList, nationList, ToolTips, Turn));
        }
    }
}