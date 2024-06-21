using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.classes
{
    public class NationGenerator
    {
        private List<Tile> tileList;

        public List<Nation> NationList { get; private set; }

        private static readonly Random random = new Random();

        public NationGenerator(List<Tile> tileList)
        {
            this.tileList = tileList;
            NationList = new List<Nation>();
        }

        private List<Tile> GetUnownedTiles(List<Tile> tileList)
        {
            List<Tile> unownedTiles = new List<Tile>();
            foreach (Tile tile in tileList)
            {
                if (tile.CurrentOwner == null && tile.Terrain != "Ocean")
                {
                    unownedTiles.Add(tile);
                }
            }
            return unownedTiles;
        }

        public GameData GenerateNations()
        {
            List<Tile> landTiles = new List<Tile>();
            foreach (Tile tile in tileList)
            {
                if (tile.Terrain != "Ocean")
                {
                    landTiles.Add(tile);
                }
            }

            Random random = new Random();
            int R = 48;
            int N = landTiles.Count;
            int quotient = (N - 1) / (R - 1);
            int remainder = (N - 1) % (R - 1);
            int index = 0;
            List<Tile> CapitalTiles = new List<Tile>();
            do
            {
                landTiles[index].SetTerrain("Capital");
                CapitalTiles.Add(landTiles[index]);
                int NewDevelopment = random.Next(10, 30);
                int MinPopRange = 100000 + (32142 * (NewDevelopment - 3));
                int MaxPopRange = MinPopRange + 32142;
                int NewPopulation = random.Next(MinPopRange, MaxPopRange);
                landTiles[index].SetDevPop(NewDevelopment, NewPopulation);
            } while ((index += quotient + (remainder-- > 0 ? 1 : 0)) < N);

            List<Color> colorList = new List<Color>
            {
                Color.FromArgb(255, 0, 0),         // Red
                Color.FromArgb(0, 255, 0),         // Green
                Color.FromArgb(245, 255, 250),     // Mint Cream
                Color.FromArgb(255, 165, 0),       // Orange
                Color.FromArgb(255, 0, 255),       // Magenta
                Color.FromArgb(0, 255, 255),       // Cyan
                Color.FromArgb(255, 99, 71),       // Tomato
                Color.FromArgb(30, 144, 255),      // Dodger Blue
                Color.FromArgb(50, 205, 50),       // Lime Green
                Color.FromArgb(255, 105, 180),     // Hot Pink
                Color.FromArgb(255, 127, 80),      // Coral
                Color.FromArgb(255, 0, 0),         // Red
                Color.FromArgb(0, 255, 0),         // Green
                Color.FromArgb(245, 255, 250),     // Mint Cream
                Color.FromArgb(255, 165, 0),       // Orange
                Color.FromArgb(255, 0, 255),       // Magenta
                Color.FromArgb(0, 255, 255),       // Cyan
                Color.FromArgb(255, 99, 71),       // Tomato
                Color.FromArgb(30, 144, 255),      // Dodger Blue
                Color.FromArgb(50, 205, 50),       // Lime Green
                Color.FromArgb(255, 105, 180),     // Hot Pink
                Color.FromArgb(255, 127, 80),      // Coral
                Color.FromArgb(255, 0, 0),         // Red
                Color.FromArgb(0, 255, 0),         // Green
                Color.FromArgb(245, 255, 250),     // Mint Cream
                Color.FromArgb(255, 165, 0),       // Orange
                Color.FromArgb(255, 0, 255),       // Magenta
                Color.FromArgb(0, 255, 255),       // Cyan
                Color.FromArgb(255, 99, 71),       // Tomato
                Color.FromArgb(30, 144, 255),      // Dodger Blue
                Color.FromArgb(50, 205, 50),       // Lime Green
                Color.FromArgb(255, 105, 180),     // Hot Pink
                Color.FromArgb(255, 127, 80),      // Coral
                Color.FromArgb(255, 0, 0),         // Red
                Color.FromArgb(0, 255, 0),         // Green
                Color.FromArgb(245, 255, 250),     // Mint Cream
                Color.FromArgb(255, 165, 0),       // Orange
                Color.FromArgb(255, 0, 255),       // Magenta
                Color.FromArgb(0, 255, 255),       // Cyan
                Color.FromArgb(255, 99, 71),       // Tomato
                Color.FromArgb(30, 144, 255),      // Dodger Blue
                Color.FromArgb(50, 205, 50),       // Lime Green
                Color.FromArgb(255, 105, 180),     // Hot Pink
                Color.FromArgb(255, 127, 80),      // Coral
                Color.FromArgb(255, 0, 0),         // Red
                Color.FromArgb(0, 255, 0),         // Green
                Color.FromArgb(245, 255, 250),     // Mint Cream
                Color.FromArgb(255, 165, 0),       // Orange
                Color.FromArgb(255, 0, 255),       // Magenta
                Color.FromArgb(0, 255, 255),       // Cyan
                Color.FromArgb(255, 99, 71)       // Tomato
            };

            // NATION CREATION
            List<string> shorthandList = new List<string>();
            string[] ends = { "ia", "land", "an", "istan", "a", "tina", "o", "al", "alie", "ey", "an", "om", "ea" };
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };
            string[] ideologies = { "Fascist", "Communist", "Monarchist", "Democratic" };

            for (int index_ = 0; index_ < 48; index_++)
            {
                string newname = "";
                while (!shorthandList.Contains(newname))
                {
                    int dice_roll = random.Next(1, 7); // Equivalent to random.nextInt((7-1) + 1) + 1
                    if (dice_roll <= 3)
                    {
                        int random_vowel = random.Next(vowels.Length); // Equivalent to random.nextInt(vowels.length)
                        newname += vowels[random_vowel];
                    }
                    else
                    {
                        int random_consonant = random.Next(consonants.Length); // Equivalent to random.nextInt(consonants.length)
                        newname += consonants[random_consonant];
                    }
                    for (int x = 0; x < dice_roll; x++)
                    {
                        string finalchar = newname.Substring(newname.Length - 1);
                        if (!consonants.Contains(finalchar))
                        {
                            int random_consonant2 = random.Next(consonants.Length);
                            newname += consonants[random_consonant2];
                        }
                        else
                        {
                            int random_vowel2 = random.Next(vowels.Length);
                            newname += vowels[random_vowel2];
                        }
                    }
                    int random_end = random.Next(ends.Length); // Equivalent to random.nextInt(ends.length)
                    newname += ends[random_end];
                    newname = newname.Substring(0, 1).ToUpper() + newname.Substring(1);
                    shorthandList.Add(newname);
                }

                // IDEOLOGIES AND LONGFORM
                int random_ideology = random.Next(ideologies.Length);
                string cur_ideology = ideologies[random_ideology];
                string[] starts;
                if (cur_ideology.Equals("Fascist"))
                {
                    starts = new string[] { "DRC ", "Empire of ", "Nation of ", "State of ", "Racial State of ", "Dictatorship of ", "Nationalist ", "Greater ", "Blackshirt ", "Nazi ", "Fascist " };
                }
                else if (cur_ideology.Equals("Communist"))
                {
                    starts = new string[] { "Soviet Republic of ", "People's Republic of ", "Union of ", "Revolutionary ", "People's Union of ", "Provincial Union of ", "Bolshevik ", "Marxist ", "Communist ", "Democratic state of ", "Social Democratic ", "Socialist ", "Provisional Committee of " };
                }
                else if (cur_ideology.Equals("Monarchist"))
                {
                    starts = new string[] { "Duchy of ", "Empire of ", "Heavenly Kingdom of ", "Realm of ", "Commonwealth of ", "Kingdom of ", "Kingdom of " };
                }
                else
                {
                    starts = new string[] { "Republic of ", "United States of ", "Federal States of ", "United ", "Democratic Republic of ", "Plurinational Republic of ", "Federation of ", "Parliamentary Republic of " };
                }
                string newfullname = starts[random.Next(starts.Length)] + newname;
                Nation newNation = new Nation(
                    index_,
                    newfullname,
                    newname,
                    cur_ideology,
                    colorList[index_],
                    colorList[index_].Name
                );
                NationList.Add(newNation);
                CapitalTiles[index_].SetOwner(newNation);
                newNation.AddTile(CapitalTiles[index_]);
            }


            List<Tile> unownedTiles = GetUnownedTiles(tileList);
            int previousUnownedCount = unownedTiles.Count;
            int repeatCount = 0;
            while (unownedTiles.Count > 0)
            {
                foreach (Nation nation in NationList)
                {
                    List<Tile> potentials = nation.GetNonDiagonalUnownedBorders();

                    // Check if non-diagonal borders are available
                    if (potentials.Count > 0)
                    {
                        int randomExpansion = random.Next(potentials.Count);
                        potentials[randomExpansion].SetOwner(nation);
                        nation.AddTile(potentials[randomExpansion]);
                    }
                    else
                    {
                        // If non-diagonal borders are not available, use diagonal borders
                        potentials = nation.GetUnownedBorders();
                        if (potentials.Count > 0)
                        {
                            int randomExpansion = random.Next(potentials.Count);
                            potentials[randomExpansion].SetOwner(nation);
                            nation.AddTile(potentials[randomExpansion]);
                        }
                    }

                    unownedTiles = GetUnownedTiles(tileList);

                    // Check if the count of unowned tiles remains the same
                    if (unownedTiles.Count == previousUnownedCount)
                    {
                        repeatCount++;
                        if (repeatCount >= 3)
                        {
                            // If the count remains the same for 3 iterations, switch to using diagonal borders
                            potentials = nation.GetUnownedBorders();
                            if (potentials.Count > 0)
                            {
                                int randomExpansion = random.Next(potentials.Count);
                                potentials[randomExpansion].SetOwner(nation);
                                nation.AddTile(potentials[randomExpansion]);
                            }
                        }
                    }
                    else
                    {
                        // Reset the repeat count if the count of unowned tiles changes
                        repeatCount = 0;
                    }
                    previousUnownedCount = unownedTiles.Count;
                }
            }
            //Update Diplo Power
            Nation MaxManNation = NationList.OrderByDescending(n => n.Manpower).FirstOrDefault();
            foreach (Nation nation in NationList)
            {
                nation.CalculateDiplomaticPower(MaxManNation.Manpower, 0);
            }

            return new GameData
            {
                TileList = tileList,
                NationList = NationList
            };
        }
    }
}
