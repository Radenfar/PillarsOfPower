using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PillarsOfPower.classes
{
    public class Tile
    {
        public List<int> Coordinates { get; set; }
        public int index { get; private set; }
        public bool IsVisited { get; set; }
        public string Terrain { get; private set; }
        public Nation? CurrentOwner { get; private set; }
        public int Development { get; private set; }
        public int Population { get; private set; }
        public int RecruitablePopulation { get; private set; }
        public int Losses { get; private set; }
        public List<Tile> BorderingTiles { get; private set; }
        public List<Tile> ExpansionTiles { get; private set; }

        public Tile(List<int> coordinates, int index_, string terrain)
        {
            Coordinates = coordinates;
            index = index_;
            Terrain = terrain;
            CurrentOwner = null;
            Development = 0;
            Population = 0;
            RecruitablePopulation = 0;
            Losses = 0;
            BorderingTiles = new List<Tile>();
            ExpansionTiles = new List<Tile>();
        }

        public void SetDevPop(int NewDevelopment, int NewPopulation)
        {
            this.Development = NewDevelopment;
            this.Population = NewPopulation;
            this.RecruitablePopulation = (int)NewPopulation / 2;
        }

        public void SetOwner(Nation newOwner)
        {
            CurrentOwner = newOwner;
        }

        public void SetBorders(List<Tile> neighboringTiles)
        {
            BorderingTiles = neighboringTiles;
        }

        public void SetTerrain(string NewTerrain)
        {
              this.Terrain = NewTerrain;
        }

        public void SetExpansionTiles(List<Tile> expansionBorders)
        {
            this.ExpansionTiles = expansionBorders;
        }

        public void TakeLosses(int losses)
        {
            Losses += losses;
            RecruitablePopulation -= losses;
            Population -= losses;
            if (RecruitablePopulation < 0)
            {
                RecruitablePopulation = 0;
            }
            if (Population < 0)
            {
                Population = 0;
            }
        }

        public void ZeroDev()
        {
            this.Development = 0;
            this.Population = 0;
            this.RecruitablePopulation = 0;
            this.Losses = 0;
            
        }

        public void SkipTurn()
        {
            if (Population > 0)
            {
                int increaseAmount = (int)Math.Floor(Population * 0.01); // 1% of the population
                RecruitablePopulation += increaseAmount;
                Population += increaseAmount;
            }
        }
    }
}
