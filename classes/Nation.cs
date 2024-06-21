using PillarsOfPower.interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PillarsOfPower.classes
{
    public class Nation : PlayerAction
    {
        public readonly Color color;
        private readonly string colorName;
        private readonly int index;
        public bool IsVictor { get; private set; }
        public bool IsVisited { get; private set; }
        public string FullName { get; } // Full sovereign name
        public string ShortHand { get; } // Name as given by just the naming function
        public List<Nation> Allies { get; } // List of allies, in the form of their individual nation objects
        public List<Tile> Tiles { get; } // List of held tiles in the form of their individual tile objects
        public string Ideology { get; } // String of nation's ideology for programming purposes and display
        public int Population { get; set; }
        public int Manpower { get; set; }
        public float ArmySpirit { get; set; }
        public float DiplomaticPower { get; set; }
        public int AllySlots { get; set; }
        private int TotalLosses { get; set; }
        public bool IsPlayer { get; set; } // Boolean of isPlayer so that the player turn is taken up by the correct nation
        public bool IsAlive { get; private set; } // Boolean of whether the country is on the board or not

        public Nation(int index, string fullName, string shortHand, string ideology, Color colour, string colorName)
        {
            this.index = index;
            this.FullName = fullName;
            this.ShortHand = shortHand;
            this.Allies = new List<Nation>();
            this.Tiles = new List<Tile>();
            this.Ideology = ideology;
            this.Population = 0;
            this.Manpower = 0;
            this.ArmySpirit = 0;
            InitialiseArmySpirit();
            this.DiplomaticPower = 0;
            CalculateDiplomaticPower(0, 0);
            this.AllySlots = 0;
            SetAllySlots();
            this.TotalLosses = 0;
            this.IsPlayer = false;
            this.IsAlive = true;
            this.IsVisited = false;
            this.color = colour;
            this.colorName = colorName;
            this.IsVictor = false;
        }

        private void InitialiseArmySpirit()
        {
            if (Ideology == "Fascist")
            {
                ArmySpirit = 50.0f;
            } else if (Ideology == "Communist")
            {
                ArmySpirit = 40.0f;
            } else if (Ideology == "Monarchist")
            {
                ArmySpirit = 30.0f;
            } else
            {
                ArmySpirit = 20.0f;
            }
        }

        public void CalculateDiplomaticPower(int MaxManpower, int MaxNumAllies)
        {
            if (MaxManpower == 0)
            {
                MaxManpower = 5000000; // 5 million
            }
            if (MaxNumAllies == 0)
            {
                MaxNumAllies = 1;
            }
            // Normalize the values to be between 0 and 1
            float normalizedArmySpirit = 0;
            float normalizedManpower = 0;
            float normalizedNumAllies = 0.5f;
            if (ArmySpirit > 0)
            {
                normalizedArmySpirit = ArmySpirit / 100;
            }
            if (Manpower > 0)
            {
                normalizedManpower = Manpower / MaxManpower;
            }
            if (Allies.Count > 0)
            {
                normalizedNumAllies = Allies.Count / (float)MaxNumAllies;
            }

            const float wArmySpirit = 1.0f;
            const float wMaxManpower = 1.0f;
            const float wNumAllies = 1.0f;

            // Calculate the Diplomatic Power with equal weights
            float sum_ = (wArmySpirit * normalizedArmySpirit) + (wNumAllies * normalizedNumAllies) + (wMaxManpower * normalizedManpower);
            if (sum_ > 0)
            {
                float WeightedAverage = sum_ / 3f;
                DiplomaticPower = 1f + (9f * WeightedAverage);
            }
            SetAllySlots();
        }

        private void SetAllySlots()
        {
            if (Ideology == "Fascist")
            {
                AllySlots = 0;
            } else if (Ideology == "Communist")
            {
                AllySlots = 1;
                AllySlots += (int)DiplomaticPower/2;
            }
            else if (Ideology == "Monarchist")
            {
                AllySlots = (int)DiplomaticPower;
            }
            else
            {
                AllySlots = 3;
                AllySlots += (int)DiplomaticPower;
            }
        }

        public void UpdateArmySpirit(int amount, int MaxManpower, int MaxNumAllies)
        {
            ArmySpirit += amount;
            if (ArmySpirit > 100)
            {
                ArmySpirit = 100;
            }
            else if (ArmySpirit <= 0)
            {
                ArmySpirit = 1;
            }
            CalculateDiplomaticPower(MaxManpower, MaxNumAllies);
        }

        private double GetIdeologyMultiplier()
        {
            switch (Ideology)
            {
                case "Fascist":
                    return 1.0;
                case "Communist":
                    return 0.8;
                case "Monarchist":
                    return 0.6;
                case "Democratic":
                    return 0.4;
                default:
                    return 0.4;
            }
        }

        public void ManPopHandler()
        {
            this.Population = Tiles.Sum(tile => tile.Population);
            double recruitablePopulation = Tiles.Sum(tile => tile.RecruitablePopulation);
            int ideologyMultiplier = (int)(GetIdeologyMultiplier() * 100);
            int totalManpower = (int) Math.Floor((recruitablePopulation / 100) * ideologyMultiplier); // Rounded down to ensure it's an integer
            this.Manpower = totalManpower;
        }

        public void TakeLosses(int lossAmount)
        {
            int lossesPerTile;
            if (Tiles.Count == 0)
            {
                lossesPerTile = lossAmount;
            } else
            {
                lossesPerTile = (int)lossAmount / Tiles.Count;

            }
            TotalLosses += (lossesPerTile * Tiles.Count);
            foreach (Tile tile in Tiles)
            {
                tile.TakeLosses(lossesPerTile);
            }
            ManPopHandler();
        }

        public void RemoveTile(Tile tile)
        {
            Tiles.Remove(tile);
            ManPopHandler();
        }

        public void SkipTurn()
        {
            foreach (Tile tile in Tiles)
            {
                tile.SkipTurn();
            }
            ManPopHandler();
        }

        public List<Tile> GetUnownedBorders()
        {
            List<Tile> borders = new List<Tile>();
            foreach (Tile tile in Tiles)
            {
                foreach (Tile borderTile in tile.BorderingTiles)
                {
                    if (borderTile.CurrentOwner == null)
                    {
                        borders.Add(borderTile);
                    }
                }
            }
            return borders;
        }

        public List<Tile> GetAllBorders()
        {
            List<Tile> borders = new List<Tile>();
            foreach (Tile tile in Tiles)
            {
                foreach (Tile borderTile in tile.BorderingTiles)
                {
                    if (borderTile.CurrentOwner != this)
                    {
                        borders.Add(borderTile);
                    }
                }
            }
            return borders;
        }

        public List<Tile> GetNonDiagonalUnownedBorders()
        {
            List<Tile> NonDiagonalTiles = new List<Tile>();
            foreach (Tile tile in Tiles)
            {
                foreach (Tile borderTile in tile.ExpansionTiles)
                {
                    if (borderTile.CurrentOwner == null)
                    {
                        NonDiagonalTiles.Add(borderTile);
                    }
                }
            }
            return NonDiagonalTiles;
        }

        public List<Nation> GetNeighbouringNations()
        {
            List<Nation> Neighbours = new List<Nation>();
            foreach (Tile tile in Tiles)
            {
                foreach (Tile borderTile in tile.BorderingTiles)
                {
                    if (borderTile.CurrentOwner != this && (Neighbours.Contains(borderTile.CurrentOwner) == false))
                    {
                        Neighbours.Add(borderTile.CurrentOwner);
                    }
                }
            }
            return Neighbours;
        }

        public List<Nation> GetPossibleAllies(GameData gameData)
        {
            int subjectRemainingAllySlots = AllySlots - Allies.Count;
            List<Nation> PossibleAllies = new List<Nation>();
            if (subjectRemainingAllySlots <= 0)
            {
                return PossibleAllies;
            }
            foreach (Nation nation in gameData.NationList)
            {
                int targetRemainingAllySlots = nation.AllySlots - nation.Allies.Count;
                if (((nation.Ideology == Ideology && nation.DoesBorder(this)) || (Ideology == "Democratic" && nation.Ideology == "Democratic") || (Ideology == "Monarchist" && nation.Ideology == "Democratic" && nation.DoesBorder(this)) || (Ideology == "Democratic" && nation.Ideology == "Monarchist" && nation.DoesBorder(this))) && !Allies.Contains(nation) && !(nation == this) && (targetRemainingAllySlots > 0) && (AllySlots > 0))
                {
                    PossibleAllies.Add(nation);
                }
            }   
            return PossibleAllies;
        }

        public List<Nation> GetPossibleTargets(GameData gameData)
        {
            List<Nation> PossibleTargets = new List<Nation>();
            foreach (Nation nation in gameData.NationList)
            {
                if (nation != this && nation.IsAlive && nation.DoesBorder(this) && !Allies.Contains(nation))
                {
                    PossibleTargets.Add(nation);
                }
            }
            return PossibleTargets;
        }   

        public int GetAlliedManpower()
        {
            int sum = Manpower;
            foreach (Nation ally in Allies)
            {
                sum += (int)ally.Manpower/10;
            }
            return sum;
        }

        public bool DoesBorder(Nation OtherNation)
        {
            foreach (Tile tile in Tiles)
            {
                foreach (Tile borderTile in tile.BorderingTiles)
                {
                    if (borderTile.CurrentOwner == OtherNation)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
            ManPopHandler();
        }

        public List<Tile> GetAllianceTiles()
        {
            List<Tile> AllianceTiles = new List<Tile>();
            foreach (Nation ally in Allies)
            {
                foreach (Tile tile in ally.Tiles)
                {
                    if (!AllianceTiles.Contains(tile))
                    {
                        AllianceTiles.Add(tile);
                    }
                }
            }
            return AllianceTiles;
        }

        public Color Color()
        {
            return color;
        }

        public string GetColor()
        {
            return colorName;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetVisited()
        {
            IsVisited = true;
        }

        public void Kill()
        {
            IsAlive = false;
            foreach (Nation ally in Allies)
            {
                ally.Allies.Remove(this);
            }
            Allies.Clear();
            Tiles.Clear();
        }

        public bool GetIsVisited()
        {
            return IsVisited;
        }

        public void SetVictor()
        {
            IsVictor = true;
        }

        public static List<int> CalculatePowers(Nation attacker, Nation defender, Tile targetTile)
        {
            // Initialise variables, set up the power calculation
            float DefenderTerrainModifier;
            float AttackPower = (attacker.Manpower / 10) * (attacker.ArmySpirit / 10);
            float DefensePower = (defender.Manpower / 10) * (defender.ArmySpirit / 10);

            // Add the power of each ally to their respective side
            foreach (Nation ally in attacker.Allies)
            {
                AttackPower += (ally.Manpower / 10) * (ally.ArmySpirit / 10);
            }
            foreach (Nation ally in defender.Allies)
            {
                DefensePower += (ally.Manpower / 10) * (ally.ArmySpirit / 10);
            }

            // Apply the terrain modifiers to the power
            if (targetTile.Terrain == "Land")
            {
                DefenderTerrainModifier = 1.0f;
            }
            else if (targetTile.Terrain == "Forest")
            {
                DefenderTerrainModifier = 1.1f;
            }
            else if (targetTile.Terrain == "Hills")
            {
                DefenderTerrainModifier = 1.2f;
            }
            else if (targetTile.Terrain == "Mountains")
            {
                DefenderTerrainModifier = 1.3f;
            }
            else
            {
                DefenderTerrainModifier = 1.5f;
            }
            DefensePower = DefensePower * DefenderTerrainModifier;
            
            // Return the powers as a list
            return new List<int> { (int)AttackPower, (int)DefensePower };
        }

        public static List<int> CalculateLosses(int Participation, int AttackPower, int DefensePower, int AttackManpower, int DefenseManpower)
        {
            // Initialise variables, set up the loss calculation
            int AttackLosses;
            int DefenseLosses;
            int TotalLosses;

            // Calculate the losses. Power ratio determines what percentage of the losses go to each side.
            TotalLosses = (AttackManpower + DefenseManpower) / Participation;
            float PowerRatio = AttackPower / DefensePower;
            float AttackLossPercentage = 1 / (1 + PowerRatio);
            AttackLosses = (int)(TotalLosses * AttackLossPercentage);
            DefenseLosses = TotalLosses - AttackLosses;
            return new List<int> { AttackLosses, DefenseLosses };
        }

        public ActionResponse PlayerAttack(Nation attacker, Nation defender, Tile TargetTile, GameData gameData)
        {
            ActionResponse response = new ActionResponse() { Outcome = false, Message = "Action not performed.", gameData = gameData };
            int AttackerIndex = gameData.NationList.IndexOf(attacker);
            int DefenderIndex = gameData.NationList.IndexOf(defender);
            if (attacker.Manpower <= 0)
            {
                response.Message = "You do not have enough manpower to attack.";
                return response;
            }
            if (!attacker.DoesBorder(defender) || !attacker.GetAllBorders().Contains(TargetTile))
            {
                //If nation is not bordering or tile is not bordering then can't attack
                response.Message = "You cannot attack a nation that you do not border.";
                return response;
            }
            if (attacker.Allies.Contains(defender))
            {
                //If nation is an ally then can't attack
                response.Message = "You cannot attack an ally.";
                return response;
            }
            if (!defender.Tiles.Contains(TargetTile))
            {
                //If tile is not owned by the target nation then can't attack
                response.Message = "Tile does not belong to the target nation.";
                return response;
            } else
            {
                // This is a valid attack, calculate the outcome
                // First, we calculate the attack and defense manpowers
                int Participation = 3; // This can be changed to a different value if needed
                int AttackManpower = attacker.Manpower;
                int DefenseManpower = defender.Manpower;
                Nation MaxManNation = gameData.NationList.OrderByDescending(n => n.Manpower).FirstOrDefault();
                Nation MaxAllyNation = gameData.NationList.OrderByDescending(n => n.Allies.Count).FirstOrDefault();

                foreach (Nation ally in attacker.Allies)
                {
                    AttackManpower += (int) (ally.Manpower / Participation);
                }
                foreach (Nation ally in defender.Allies)
                {
                    DefenseManpower += (int)(ally.Manpower / Participation);
                }

                // Now retrieve our powers from the static function
                List<int> Powers = CalculatePowers(attacker, defender, TargetTile);
                int AttackPower = Powers[0];
                int DefensePower = Powers[1];



                // Now retrieve our losses from the static function. Handles the case where either side has 0 power (losses will be 0)
                int AttackLosses;
                int DefenseLosses;
                if (AttackPower == 0 || DefensePower == 0)
                {
                    // If either side is 0 then no fighting will occur, whether it be attacker or defender.
                    AttackLosses = 0;
                    DefenseLosses = 0;
                } else
                {
                    List<int> Losses = CalculateLosses(Participation, AttackPower, DefensePower, AttackManpower, DefenseManpower);
                    AttackLosses = Losses[0];
                    DefenseLosses = Losses[1];
                }
                

                // Of the total for each, 66% goes to the main nation, 33% is distributed among the allies
                int AttackAllyLosses;
                int DefenseAllyLosses;
                if (attacker.Allies.Count == 0) // Handle div by 0 error
                {
                    AttackAllyLosses = 0;
                } else 
                {
                    AttackAllyLosses = (int)((AttackLosses / Participation) / attacker.Allies.Count);
                }
                
                if (defender.Allies.Count == 0) // Handle div by 0 error
                {
                    DefenseAllyLosses = 0;
                } else
                {
                    DefenseAllyLosses = (int)((DefenseLosses / Participation) / defender.Allies.Count);
                }

                // Now we handle the outcome of the battle
                if (AttackPower > DefensePower)
                {
                    //Battle is won, remove tile from defender and add to attacker. Change army spirit for each and update manpops for all nations involved
                    foreach (Nation ally in attacker.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(AttackAllyLosses);
                        AttackLosses -= AttackAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(5, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    foreach (Nation ally in defender.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(DefenseAllyLosses);
                        DefenseLosses -= DefenseAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(-5, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].TakeLosses(AttackLosses);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].TakeLosses(DefenseLosses);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].RemoveTile(TargetTile);
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].AddTile(TargetTile);
                    gameData.TileList[gameData.TileList.IndexOf(TargetTile)].SetOwner(attacker);
                    if (TargetTile.Terrain == "Capital")
                    {
                        gameData.NationList[gameData.NationList.IndexOf(attacker)].UpdateArmySpirit(20, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                        gameData.NationList[gameData.NationList.IndexOf(defender)].UpdateArmySpirit(-50, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    } else
                    {
                        gameData.NationList[gameData.NationList.IndexOf(attacker)].UpdateArmySpirit(10, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                        gameData.NationList[gameData.NationList.IndexOf(defender)].UpdateArmySpirit(-10, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    response.Outcome = true;
                    response.Message = attacker.ShortHand + " successfully attacked " + defender.ShortHand;
                    return response;
                } else if (AttackPower < DefensePower)
                {
                    //Battle is lost, give big army spirit debuff to attacker, small army spirit buff to defender, update manpop for all nations involved
                    foreach (Nation ally in attacker.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(AttackAllyLosses);
                        AttackLosses -= AttackAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(-5, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    foreach (Nation ally in defender.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(DefenseAllyLosses);
                        DefenseLosses -= DefenseAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(5, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].TakeLosses(AttackLosses);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].TakeLosses(DefenseLosses);
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].UpdateArmySpirit(-15, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].UpdateArmySpirit(15, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    response.Outcome = false;
                    response.Message = attacker.ShortHand + " was defeated by " + defender.ShortHand;
                    return response;
                } else
                {
                    //Battle is a draw, give small army spirit debuff to attacker, small army spirit buff to defender, update manpop
                    foreach (Nation ally in attacker.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(AttackAllyLosses);
                        AttackLosses -= AttackAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(-2, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    foreach (Nation ally in defender.Allies)
                    {
                        gameData.NationList[gameData.NationList.IndexOf(ally)].TakeLosses(DefenseAllyLosses);
                        DefenseLosses -= DefenseAllyLosses;
                        gameData.NationList[gameData.NationList.IndexOf(ally)].UpdateArmySpirit(2, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    }
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].TakeLosses(AttackLosses);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].TakeLosses(DefenseLosses);
                    gameData.NationList[gameData.NationList.IndexOf(attacker)].UpdateArmySpirit(-3, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                    gameData.NationList[gameData.NationList.IndexOf(defender)].UpdateArmySpirit(3, MaxManNation.Manpower, MaxAllyNation.Allies.Count);
                }
            } 

            return response;
        }

        public ActionResponse PlayerAlly(Nation Subject, Nation Target, GameData gameData)
        {
            ActionResponse response = new ActionResponse() { Outcome = false, Message = "Action not performed.", gameData = gameData };
            int SubjectIndex = gameData.NationList.IndexOf(Subject);
            int TargetIndex = gameData.NationList.IndexOf(Target);
            if (Subject.Allies.Contains(Target))
            {
                //If already allied, assume break and remove ally
                gameData.NationList[SubjectIndex].Allies.Remove(Target);
                gameData.NationList[TargetIndex].Allies.Remove(Subject);
                response.Outcome = true;
                response.Message = Subject.ShortHand + " broke alliance with " + Target.ShortHand;
                response.gameData = gameData;
            }
            else
            {
                if ((Subject.Ideology != "Fascist" && Target.Ideology != "Fascist") && ((Subject.AllySlots - Subject.Allies.Count) > 0 && (Target.AllySlots - Target.Allies.Count) > 0))
                {
                    // If (both you and the target are not fascist) and (you both have free ally slots)
                    if ((Subject.Ideology == "Communist" && Target.Ideology == "Communist") && Subject.DoesBorder(Target)) 
                    {
                        // If (both you and the target are communist) and (you border each other)
                        gameData.NationList[SubjectIndex].Allies.Add(Target);
                        gameData.NationList[TargetIndex].Allies.Add(Subject);
                        response.Outcome = true;
                        response.Message = Subject.ShortHand + " allied with " + Target.ShortHand;
                        response.gameData = gameData;
                    } else if ((Subject.Ideology == "Monarchist" && (Target.Ideology == "Monarchist" ^ Target.Ideology == "Democratic")) && Subject.DoesBorder(Target))
                    {
                        // If (you are monarchist and (the target is monarchist or democratic)) and (you border each other)
                        gameData.NationList[SubjectIndex].Allies.Add(Target);
                        gameData.NationList[TargetIndex].Allies.Add(Subject);
                        response.Outcome = true;
                        response.Message = Subject.ShortHand + " allied with " + Target.ShortHand;
                        response.gameData = gameData;
                    } else if (Subject.Ideology == "Democratic" && Target.Ideology == "Democratic")
                    {
                        //If you are both Democratic
                        gameData.NationList[SubjectIndex].Allies.Add(Target);
                        gameData.NationList[TargetIndex].Allies.Add(Subject);
                        response.Outcome = true;
                        response.Message = Subject.ShortHand + " allied with " + Target.ShortHand;
                        response.gameData = gameData;
                    } else if ((Subject.Ideology == "Democratic" && Target.Ideology == "Monarchist") && Subject.DoesBorder(Target))
                    {
                        //If (you are democratic and target is monarchist) and (you border each other)
                        gameData.NationList[SubjectIndex].Allies.Add(Target);
                        gameData.NationList[TargetIndex].Allies.Add(Subject);
                        response.Outcome = true;
                        response.Message = Subject.ShortHand + " allied with " + Target.ShortHand;
                        response.gameData = gameData;
                    } else
                    {
                        response.Message = "Invalid alliance target.";
                        response.gameData = gameData;
                    }
                } else
                {
                    //response.Message = "Either you or the target has no free ally slots.";
                    response.Message = "Bug Details: Subject: " + Subject.ShortHand + " Target: " + Target.ShortHand + " Subject Ally Slots: " + Subject.AllySlots + " Target Ally Slots: " + Target.AllySlots + " Subject Allies: " + Subject.Allies.Count + " Target Allies: " + Target.Allies.Count;
                    response.gameData = gameData;
                }
            }
            Nation MaxManNation = gameData.NationList.OrderByDescending(n => n.Manpower).FirstOrDefault();
            Nation MaxAllyNation = gameData.NationList.OrderByDescending(n => n.Allies.Count).FirstOrDefault();
            Subject.CalculateDiplomaticPower(MaxManNation.Manpower, MaxAllyNation.Allies.Count);
            Target.CalculateDiplomaticPower(MaxManNation.Manpower, MaxAllyNation.Allies.Count);
            return response;
        }

        public ActionResponse PlayerSkip(Nation subject, GameData gameData)
        {
            int SubjectIndex = gameData.NationList.IndexOf(subject);
            gameData.NationList[SubjectIndex].SkipTurn();
            if (gameData.NationList[SubjectIndex].ArmySpirit < 100)
            {
                gameData.NationList[SubjectIndex].ArmySpirit += 1;
            }
            gameData.NationList[SubjectIndex].CalculateDiplomaticPower(gameData.NationList.OrderByDescending(n => n.Manpower).FirstOrDefault().Manpower, gameData.NationList.OrderByDescending(n => n.Allies.Count).FirstOrDefault().Allies.Count);
            ActionResponse actionResponse = new ActionResponse() { Outcome = true, Message = subject.ShortHand + " skipped their turn.", gameData = gameData };
            return actionResponse;
        }

        public ActionResponse AIAction(GameData aiGameData)
        {
            // First: Can I ally anyone?
            List<Nation> PossibleAllies = GetPossibleAllies(aiGameData);
            if (PossibleAllies.Count > 0)
            {
                // choose the one with the highest diplomatic power
                Nation BestAlly = PossibleAllies.OrderByDescending(n => n.DiplomaticPower).FirstOrDefault();
                return PlayerAlly(this, BestAlly, aiGameData);
            }
            
            // Second: Can I attack anyone? If so, pick the easiest fight.
            List<Nation> PossibleTargets = GetPossibleTargets(aiGameData);
            Tile bestTarget = null;
            int bestAttackPower = 0;
            foreach (Tile tile in GetAllBorders())
            {
                if (PossibleTargets.Contains(tile.CurrentOwner))
                {
                    List<int> Powers = CalculatePowers(this, tile.CurrentOwner, tile);
                    int AttackPower = Powers[0];
                    int DefensePower = Powers[1];
                    if (AttackPower > DefensePower)
                    {
                        if (AttackPower > bestAttackPower)
                        {
                            bestAttackPower = AttackPower;
                            bestTarget = tile;
                        }
                    }
                }
            }
            if (bestTarget != null)
            {
                return PlayerAttack(this, bestTarget.CurrentOwner, bestTarget, aiGameData);
            }

            // Third: Skip turn
            return PlayerSkip(this, aiGameData);
        }
    }
}
