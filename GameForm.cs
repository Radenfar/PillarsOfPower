using Microsoft.Win32;
using PillarsOfPower.classes;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace PillarsOfPower
{
    public partial class GameForm : Form
    {
        List<Tile> TileList;
        List<Nation> NationList;
        bool ToolTips;
        int Turn;
        List<Nation> AliveNations;
        List<Nation> PlayerNations;
        private Label CurrentSelectionLabel;
        private Button SelectNationButton;
        private Button AttackButton;
        private Button AllyButton;
        private Button SkipButton;
        private Button backtomeButton;
        private Button FutureButtonB;
        private ToolStripComboBox MapModesBox;
        private DataGridView NationInfo;
        private TextBox PlayerNationInfoPanel;
        private TextBox WorldEventsPanel;
        private Label AttackToolTipLabel;
        private Label AllyToolTipLabel;
        private Label TurnLabel;
        private Button ExitButton;
        private Nation? lastClickedNation;
        private Tile? clickedTile;
        private string MapMode;
        private Dictionary<string, Image> TerrainImages;
        private Image OceanBase;

        public GameForm(List<Tile> TileList, List<Nation> NationList, bool ToolTips, int Turn)
        {
            this.TileList = TileList;
            this.NationList = NationList;
            this.ToolTips = ToolTips;
            this.Turn = Turn;
            this.clickedTile = null;
            this.MapMode = "Political";

            // SET UP ALIVE NATIONS LIST
            this.AliveNations = new List<Nation>();
            this.PlayerNations = new List<Nation>();
            foreach (Nation nation in NationList)
            {
                if (nation.IsAlive)
                {
                    this.AliveNations.Add(nation);
                } else if (nation.IsPlayer)
                {
                    this.PlayerNations.Add(nation);
                }
            }

            // LOAD TERRAIN IMAGES
            this.TerrainImages = new Dictionary<string, Image>();
            this.TerrainImages["Land"] = Image.FromFile("land.png");
            this.TerrainImages["Mountains"] = Image.FromFile("mountains.png");
            this.TerrainImages["Hills"] = Image.FromFile("hills.png");
            this.TerrainImages["Forest"] = Image.FromFile("forest.png");
            this.TerrainImages["Capital"] = Image.FromFile("capital.png");
            this.TerrainImages["LandWhite"] = Image.FromFile("landwhite.png");
            this.TerrainImages["MountainsWhite"] = Image.FromFile("mountainswhite.png");
            this.TerrainImages["HillsWhite"] = Image.FromFile("hillswhite.png");
            this.TerrainImages["ForestWhite"] = Image.FromFile("forestwhite.png");
            this.TerrainImages["CapitalWhite"] = Image.FromFile("capitalwhite.png");
            this.TerrainImages["Blank"] = Image.FromFile("transparent.png");
            this.OceanBase = Image.FromFile("oceanbase.png");
            InitializeComponent();

            // HERE: CREATE ANY LABELS RELEVANT AND ADD THEM TO CONTROLS AS SHOWN IN BELOW EXAMPLE
            // CURRENT SELECTION LABEL
            CurrentSelectionLabel = new Label();
            CurrentSelectionLabel.AutoSize = true;
            CurrentSelectionLabel.Location = new Point(760, 10);
            CurrentSelectionLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            CurrentSelectionLabel.Text = "Current Selection: None";
            this.Controls.Add(CurrentSelectionLabel);

            // SELECT NATION BUTTON
            SelectNationButton = new Button();
            SelectNationButton.AutoSize = false;
            SelectNationButton.Location = new Point(760, 40);
            SelectNationButton.Size = new Size(480, 30);
            SelectNationButton.Font = new Font("Arial", 12, FontStyle.Bold);
            SelectNationButton.Text = "Confirm Selection";
            this.Controls.Add(SelectNationButton);
            SelectNationButton.Click += SelectNationButton_Click;

            // ATTACK BUTTON
            AttackButton = new Button();
            AttackButton.Visible = false;
            AttackButton.Enabled = false;
            AttackButton.AutoSize = true;
            AttackButton.Location = new Point((720 + 40), 40);
            AttackButton.Font = new Font("Arial", 12, FontStyle.Bold);
            AttackButton.Text = "Attack";
            this.Controls.Add(AttackButton);
            AttackButton.Click += AttackButton_Click;

            // ALLY BUTTON
            AllyButton = new Button();
            AllyButton.Visible = false;
            AllyButton.Enabled = false;
            AllyButton.AutoSize = true;
            AllyButton.Location = new Point(940, 40);
            AllyButton.Font = new Font("Arial", 12, FontStyle.Bold);
            AllyButton.Text = "Ally/Break Ally";
            this.Controls.Add(AllyButton);
            AllyButton.Click += AllyButton_Click;

            // SKIP BUTTON
            SkipButton = new Button();
            SkipButton.Visible = false;
            SkipButton.Enabled = false;
            SkipButton.AutoSize = true;
            SkipButton.Location = new Point((this.Width - 40 - SkipButton.Width), 40);
            SkipButton.Font = new Font("Arial", 12, FontStyle.Bold);
            SkipButton.Text = "Skip";
            this.Controls.Add(SkipButton);
            SkipButton.Click += SkipButton_Click;

            // NATION INFOTABLE
            NationInfo = new DataGridView();
            NationInfo.Dock = DockStyle.Fill;
            NationInfo.AllowUserToAddRows = false;
            NationInfo.RowHeadersVisible = false;
            NationInfo.ReadOnly = true;
            NationInfo.AllowUserToResizeColumns = false;
            NationInfo.AllowUserToResizeRows = false;
            NationInfo.AutoGenerateColumns = false;
            NationInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            NationInfo.CellFormatting += NationInfo_CellFormatting;
            NationInfo.SelectionChanged += NationInfo_SelectionChanged;
            // Add columns to the DataGridViewM
            NationInfo.Columns.Add("ColorColumn", "Colour");
            NationInfo.Columns.Add("NameColumn", "Name");
            NationInfo.Columns.Add("IdeologyColumn", "Ideology");
            NationInfo.Columns.Add("ManpowerColumn", "Manpower");
            NationInfo.Columns.Add("ArmySpiritColumn", "ArmySpirit");
            NationInfo.Columns.Add("DiplomaticPowerColumn", "DiplomaticPower");
            // Bind data from AliveNations list to DataGridView
            NationInfo.DataSource = AliveNations;
            NationInfo.Columns["ColorColumn"].DataPropertyName = "Color";
            NationInfo.Columns["NameColumn"].DataPropertyName = "ShortHand";
            NationInfo.Columns["IdeologyColumn"].DataPropertyName = "Ideology";
            NationInfo.Columns["ManpowerColumn"].DataPropertyName = "Manpower";
            NationInfo.Columns["ArmySpiritColumn"].DataPropertyName = "ArmySpirit";
            NationInfo.Columns["DiplomaticPowerColumn"].DataPropertyName = "DiplomaticPower";
            foreach (DataGridViewColumn col in NationInfo.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            NationInfo.Parent = new Panel();
            NationInfo.Parent.Location = new Point(760, 80);
            NationInfo.Parent.Size = new Size(480, 300);
            NationInfo.Parent.BackColor = Color.Transparent;
            NationInfo.ColumnHeadersVisible = true;
            NationInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(NationInfo.Parent);

            //FUTUREBUTTONA
            backtomeButton = new Button();
            backtomeButton.AutoSize = true;
            backtomeButton.Location = new Point(760, 390);
            backtomeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            backtomeButton.Text = "Back to player";
            this.Controls.Add(backtomeButton);
            backtomeButton.Click += BackToMeButtonClick;

            //FUTUREBUTTONB
            FutureButtonB = new Button();
            FutureButtonB.AutoSize = true;
            FutureButtonB.Location = new Point(940, 390);
            FutureButtonB.Font = new Font("Arial", 12, FontStyle.Bold);
            FutureButtonB.Text = "FutureContent";
            this.Controls.Add(FutureButtonB);
            FutureButtonB.Click += FutureButtonB_Click;

            //MAP MODES BOX
            Panel MMPanel = new Panel();
            MMPanel.Location = new Point(1120, 390);

            MapModesBox = new ToolStripComboBox();
            MapModesBox.Items.AddRange(new string[] { "Political", "Terrain", "Tile Manpower", "Nation Manpower", "Diplomatic Power", "Army Spirit", "Ideology", "Alliances" });
            MapModesBox.SelectedItem = "Political";
            MapModesBox.DropDownStyle = ComboBoxStyle.DropDownList;
            MapModesBox.SelectedIndexChanged += MapModesBox_SelectionChanged;

            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Items.Add(MapModesBox);

            MMPanel.Controls.Add(toolStrip);
            this.Controls.Add(MMPanel);

            // PLAYER INFORMATION PANEL
            Label PlayerInfoLabel = new Label();
            PlayerInfoLabel.AutoSize = true;
            PlayerInfoLabel.Location = new Point(760, 470);
            PlayerInfoLabel.Text = "Player Information:";
            PlayerInfoLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            PlayerNationInfoPanel = new TextBox();
            PlayerNationInfoPanel.Multiline = true;
            PlayerNationInfoPanel.ReadOnly = true;
            PlayerNationInfoPanel.ScrollBars = ScrollBars.Vertical;
            PlayerNationInfoPanel.Location = new Point(760, 500);
            PlayerNationInfoPanel.Size = new Size(240, 150);
            PlayerNationInfoPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(PlayerInfoLabel);
            this.Controls.Add(PlayerNationInfoPanel);
            UpdatePlayerNationInfoPanel();

            //WORLD EVENTS PANEL
            Label WorldEventsLabel = new Label();
            WorldEventsLabel.AutoSize = true;
            WorldEventsLabel.Location = new Point(1000, 470);
            WorldEventsLabel.Text = "World Events:";
            WorldEventsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            WorldEventsPanel = new TextBox();
            WorldEventsPanel.Multiline = true;
            WorldEventsPanel.ReadOnly = true;
            WorldEventsPanel.ScrollBars = ScrollBars.Vertical;
            WorldEventsPanel.Location = new Point(1000, 500);
            WorldEventsPanel.Size = new Size(240, 150);
            WorldEventsPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(WorldEventsLabel);
            this.Controls.Add(WorldEventsPanel);

            //ATTACK TOOLTIPS LABEL
            AttackToolTipLabel = new Label();
            AttackToolTipLabel.AutoSize = true;
            AttackToolTipLabel.Location = new Point(760, 680);
            AttackToolTipLabel.Text = "Attack";
            AttackToolTipLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            if (ToolTips == false)
            {
                AttackToolTipLabel.Visible = false;
                AttackToolTipLabel.Enabled = false;
            }
            this.Controls.Add(AttackToolTipLabel);

            //ALLY TOOLTIPS LABEL
            AllyToolTipLabel = new Label();
            AllyToolTipLabel.AutoSize = true;
            AllyToolTipLabel.Location = new Point(900, 680);
            AllyToolTipLabel.Text = "Ally";
            AllyToolTipLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            if (ToolTips == false)
            {
                AllyToolTipLabel.Visible = false;
                AllyToolTipLabel.Enabled = false;
            }
            this.Controls.Add(AllyToolTipLabel);

            //TURN LABEL
            TurnLabel = new Label();
            TurnLabel.AutoSize = true;
            TurnLabel.Location = new Point(1000, 680);
            TurnLabel.Text = "Turn: " + Turn;
            TurnLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(TurnLabel);


            //EXIT BUTTON
            ExitButton = new Button();
            ExitButton.AutoSize = true;
            ExitButton.Location = new Point(1160, 670);
            ExitButton.Font = new Font("Arial", 12, FontStyle.Bold);
            ExitButton.Text = "Exit";
            this.Controls.Add(ExitButton);
            ExitButton.Click += ExitButton_Click;

            this.MouseClick += GameForm_MouseClick;
        }

        private void UpdateToolTipLabel(string text, bool success, string method)
        {
            if (success)
            {
                if (method == "ally")
                {
                    AttackToolTipLabel.ForeColor = Color.Black;
                    AllyToolTipLabel.ForeColor = Color.Green;
                } else if (method == "attack")
                {
                    AllyToolTipLabel.ForeColor = Color.Black;
                    AttackToolTipLabel.ForeColor = Color.Green;
                }
            } else
            {
                if (method == "ally")
                {
                    AttackToolTipLabel.ForeColor = Color.Black;
                    AllyToolTipLabel.ForeColor = Color.Red;
                } else if (method == "attack")
                {
                    AllyToolTipLabel.ForeColor = Color.Black;
                    AttackToolTipLabel.ForeColor = Color.Red;
                }
            }
        }

        private void UpdateWorldEventsPanel(string eventDescription)
        {
            WorldEventsPanel.AppendText(eventDescription + "\r\n");
        }

        private void UpdatePlayerNationInfoPanel()
        {
            if (lastClickedNation == null)
            {
                PlayerNationInfoPanel.Text = "No nation selected.";
            } else
            {
                PlayerNationInfoPanel.Text = lastClickedNation.FullName + ":\r\n";
                PlayerNationInfoPanel.AppendText("Manpower: " + lastClickedNation.Manpower.ToString("N0") + "\r\n");
                PlayerNationInfoPanel.AppendText("Ideology: " + lastClickedNation.Ideology + "\r\n");
                PlayerNationInfoPanel.AppendText("Army Spirit: " + lastClickedNation.ArmySpirit + "\r\n");
                PlayerNationInfoPanel.AppendText("Diplomatic Power: " + lastClickedNation.DiplomaticPower + "\r\n");
                PlayerNationInfoPanel.AppendText("Population: " + lastClickedNation.Population.ToString("N0") + "\r\n");
                PlayerNationInfoPanel.AppendText("Number of Tiles: " + lastClickedNation.Tiles.Count + "\r\n");
                PlayerNationInfoPanel.AppendText("Allied Manpower: " + lastClickedNation.GetAlliedManpower().ToString("N0") + "\r\n");
                PlayerNationInfoPanel.AppendText("Ally Slots Available: " + (lastClickedNation.AllySlots - lastClickedNation.Allies.Count) + "/" + lastClickedNation.AllySlots + "\r\n");
                PlayerNationInfoPanel.AppendText("Allies: " + "\r\n");
                foreach (Nation ally in lastClickedNation.Allies)
                {
                    PlayerNationInfoPanel.AppendText("- " + ally.ShortHand + " " + "(" + ally.Ideology + ")" + "\r\n");
                }
                PlayerNationInfoPanel.SelectionStart = 0;
                PlayerNationInfoPanel.ScrollToCaret();
            }
        }

        private void NationInfo_SelectionChanged(object sender, EventArgs e)
        {
            if (NationInfo.SelectedRows.Count > 0)
            {
                Nation SelectedNation = NationInfo.SelectedRows[0].DataBoundItem as Nation;
                if (lastClickedNation == null)
                {
                    lastClickedNation = SelectedNation;
                    UpdateTiles(lastClickedNation.Tiles);
                    CurrentSelectionLabel.Text = "Current Selection: " + lastClickedNation.FullName;
                }
                else
                {
                    // De-highlight the previously clicked nation and update its tiles
                    if (lastClickedNation != SelectedNation)
                    {
                        List<Tile> prevOwnedTiles = lastClickedNation.Tiles;
                        lastClickedNation = null;
                        UpdateTiles(prevOwnedTiles);
                    }
                    // Update the clicked nation's tiles for highlighting
                    lastClickedNation = SelectedNation;
                    CurrentSelectionLabel.Text = "Current Selection: " + lastClickedNation.FullName;
                    List<Tile> ownedTiles = lastClickedNation.Tiles;
                    UpdateTiles(ownedTiles);
                }
                UpdatePlayerNationInfoPanel();
            }
        }

        private void HighlightSelectedNation(Nation selectedNation)
        {
            NationInfo.ClearSelection();
            if (selectedNation != null)
            {
                foreach (DataGridViewRow row in NationInfo.Rows)
                {
                    Nation nation = row.DataBoundItem as Nation;
                    if (nation == selectedNation)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void NationInfo_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                Nation nation = NationInfo.Rows[e.RowIndex].DataBoundItem as Nation;
                if (nation != null)
                {
                    e.Value = null; // Clear cell text
                    e.CellStyle.BackColor = nation.color;
                }
            }
            if (e.ColumnIndex == NationInfo.Columns["ManpowerColumn"].Index && e.RowIndex >= 0)
            {
                if (e.Value != null && e.Value is int manpower)
                {
                    // Format the manpower value with thousands separator
                    e.Value = manpower.ToString("N0");
                    e.FormattingApplied = true;
                }
            }
            if (e.ColumnIndex == NationInfo.Columns["DiplomaticPowerColumn"].Index && e.RowIndex >= 0)
            {
                if (e.Value != null && e.Value is float diplomaticPower)
                {
                    // Format the diplomatic power value to 1 decimal place
                    e.Value = diplomaticPower.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void UpdateTiles(List<Tile> ChangedTiles)
        {
            foreach (Tile tile in ChangedTiles)
            {
                Rectangle TileRegion = new Rectangle(tile.Coordinates[0], tile.Coordinates[1], 20, 20);
                this.Invalidate(TileRegion);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.DrawImage(this.OceanBase, 0, 0, this.OceanBase.Width, this.OceanBase.Height);

            switch (MapMode)
            {
                case "Political":
                    mapmodePolitical(g);
                    break;
                case "Terrain":
                    mapmodeTerrain(g);
                    break;
                case "Tile_Manpower":
                    mapmodeTileManpower(g);
                    break;
                case "Nation_Manpower":
                    mapmodeNationManpower(g);
                    break;
                case "Diplomatic_Power":
                    mapmodeDiplomaticPower(g);
                    break;
                case "Army_Spirit":
                    mapmodeArmySpirit(g);
                    break;
                case "Ideology":
                    mapmodeIdeology(g);
                    break;
                case "Alliances":
                    mapmodeAlliances(g);
                    break;
                    // Add more cases for additional map modes if needed
            }
        }

        private void mapmodePolitical(Graphics g)
        {
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];
                if (tile.CurrentOwner != null && tile.Terrain != "Ocean")
                {
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        Color tint = tile.CurrentOwner.color;
                        if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                        {
                            if (clickedTile != null && tile == clickedTile)
                            {
                                tint = Color.Red;
                            }
                            else
                            {
                                tint = Color.Yellow;
                            }
                            image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                                    terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                                    terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                                    terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                                    terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                                    this.TerrainImages["Blank"];
                        }
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            if (clickedTile != null && clickedTile.Terrain != "Ocean" && tile.CurrentOwner.GetAllianceTiles().Contains(clickedTile))
                            {
                                // PLACEHOLDER: Potential future highlight for alliance tiles. Removed this initially as buggy as heck.
                                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                                {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                                });
                                imageAttributes.SetColorMatrix(colorMatrix);
                            }
                            else
                            {
                                // Apply the regular tint for non-bordering tiles
                                ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                                {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                                });
                                imageAttributes.SetColorMatrix(colorMatrix);
                            }
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
                else
                {
                    g.DrawImage(image, x, y, 20, 20);
                }
            }
        }

        private void mapmodeTerrain(Graphics g)
        {
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                g.DrawImage(image, x, y, 20, 20);
            }
        }

        private void mapmodeTileManpower(Graphics g)
        {
            int minManpower = TileList[0].RecruitablePopulation;
            int maxManpower = TileList[0].RecruitablePopulation;
            foreach (Tile tile in TileList)
            {
                if (tile.CurrentOwner != null && tile.RecruitablePopulation < minManpower)
                {
                    minManpower = tile.RecruitablePopulation;
                } else if (tile.CurrentOwner != null && tile.RecruitablePopulation > maxManpower)
                {
                    maxManpower = tile.RecruitablePopulation;
                }
            }
            int manpowerRange = maxManpower - minManpower;
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    int manpower = tile.RecruitablePopulation;
                    int red = 255 - (int)(255 * (manpower - minManpower) / manpowerRange);
                    int green = (int)(255 * (manpower - minManpower) / manpowerRange);
                    Color tint = Color.FromArgb(red, green, 0);
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
            }
        }

        private void mapmodeIdeology(Graphics g)
        {
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];
                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        Color ideologyColor;

                        string ideology = tile.CurrentOwner.Ideology;
                        switch (ideology)
                        {
                            case "Fascist":
                                ideologyColor = Color.Gold;
                                break;
                            case "Communist":
                                ideologyColor = Color.Red;
                                break;
                            case "Monarchist":
                                ideologyColor = Color.DarkViolet;
                                break;
                            case "Democratic":
                                ideologyColor = Color.Blue;
                                break;
                            default:
                                ideologyColor = Color.Transparent;
                                break;
                        }

                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                                new float[] { (float)ideologyColor.R / 255, 0, 0, 0, 0 },
                                new float[] { 0, (float)ideologyColor.G / 255, 0, 0, 0 },
                                new float[] { 0, 0, (float)ideologyColor.B / 255, 0, 0 },
                                new float[] { 0, 0, 0, 1, 0 },
                                new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
                else
                {
                    g.DrawImage(image, x, y, 20, 20);
                }
            }
        }

        private void mapmodeNationManpower(Graphics g)
        {
            int maxManpower = AliveNations.Max(nation => nation.Manpower);
            int minManpower = AliveNations.Min(nation => nation.Manpower);
            int manpowerRange = maxManpower - minManpower;

            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    int manpower = tile.CurrentOwner.Manpower;
                    int red = 255 - (int)(255 * (manpower - minManpower) / manpowerRange);
                    int green = (int)(255 * (manpower - minManpower) / manpowerRange);
                    Color tint = Color.FromArgb(red, green, 0);
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
            }
        }

        private void mapmodeDiplomaticPower(Graphics g)
        {
            float maxDiplomaticPower = AliveNations.Max(nation => nation.DiplomaticPower);
            float minDiplomaticPower = AliveNations.Min(nation => nation.DiplomaticPower);
            float diplomaticPowerRange = maxDiplomaticPower - minDiplomaticPower;

            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    float diplomaticPower = tile.CurrentOwner.DiplomaticPower;
                    int red = 255 - (int)(255 * (diplomaticPower - minDiplomaticPower) / diplomaticPowerRange);
                    int green = (int)(255 * (diplomaticPower - minDiplomaticPower) / diplomaticPowerRange);
                    Color tint = Color.FromArgb(red, green, 0);
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0 , (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                                               0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
            }
        }

        private void mapmodeArmySpirit(Graphics g)
        {
            float maxArmySpirit = AliveNations.Max(nation => nation.ArmySpirit);
            float minArmySpirit = AliveNations.Min(nation => nation.ArmySpirit);
            float armySpiritRange = maxArmySpirit - minArmySpirit;

            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    float armySpirit = tile.CurrentOwner.ArmySpirit;
                    int red = 255 - (int)(255 * (armySpirit - minArmySpirit) / armySpiritRange);
                    int green = (int)(255 * (armySpirit - minArmySpirit) / armySpiritRange);
                    Color tint = Color.FromArgb(red, green, 0);
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
            }
        }

        private void mapmodeAlliances(Graphics g)
        {
            // First of all, cluster the nations into alliances
            List<List<Nation>> alliances = new List<List<Nation>>();
            foreach (Nation nation in NationList)
            {
                if (nation.IsAlive)
                {
                    if (nation.Allies.Count == 0)
                    {
                        alliances.Add(new List<Nation> { nation });
                    }
                    else
                    {
                        bool added = false;
                        for (int i = 0; i < alliances.Count; i++)
                        {
                            if (alliances[i].Contains(nation))
                            {
                                added = true;
                                break;
                            }
                            else if (alliances[i].Intersect(nation.Allies).Any())
                            {
                                alliances[i].Add(nation);
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                        {
                            alliances.Add(new List<Nation> { nation });
                        }
                    }
                }
            }

            // Identify the colour of each alliance based on the member with the highest diplomatic power
            Dictionary<Nation, Color> allianceColours = new Dictionary<Nation, Color>();
            foreach (List<Nation> alliance in alliances)
            {
                Nation maxDiplomaticPowerNation = alliance[0];
                foreach (Nation nation in alliance)
                {
                    if (nation.DiplomaticPower > maxDiplomaticPowerNation.DiplomaticPower)
                    {
                        maxDiplomaticPowerNation = nation;
                    }
                }
                Color allianceColour = maxDiplomaticPowerNation.color;
                foreach (Nation nation in alliance)
                {
                    allianceColours[nation] = allianceColour;
                }
            }

            // Now draw the tiles with the alliance colours
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                string terrain = tile.Terrain;

                Image image = terrain == "Land" ? this.TerrainImages["Land"] :
                                terrain == "Mountains" ? this.TerrainImages["Mountains"] :
                                terrain == "Hills" ? this.TerrainImages["Hills"] :
                                terrain == "Forest" ? this.TerrainImages["Forest"] :
                                terrain == "Capital" ? this.TerrainImages["Capital"] :
                                this.TerrainImages["Blank"];

                if (lastClickedNation != null && tile.CurrentOwner == lastClickedNation)
                {
                    image = terrain == "Land" ? this.TerrainImages["LandWhite"] :
                            terrain == "Mountains" ? this.TerrainImages["MountainsWhite"] :
                            terrain == "Hills" ? this.TerrainImages["HillsWhite"] :
                            terrain == "Forest" ? this.TerrainImages["ForestWhite"] :
                            terrain == "Capital" ? this.TerrainImages["CapitalWhite"] :
                            this.TerrainImages["Blank"];
                }

                if (tile.CurrentOwner != null)
                {
                    Color tint = allianceColours[tile.CurrentOwner];
                    using (Image tintedImage = new Bitmap(image.Width, image.Height))
                    {
                        using (Graphics gr = Graphics.FromImage(tintedImage))
                        {
                            ImageAttributes imageAttributes = new ImageAttributes();
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                            new float[] { (float)tint.R / 255, 0, 0, 0, 0 },
                            new float[] { 0, (float)tint.G / 255, 0, 0, 0 },
                            new float[] { 0, 0, (float)tint.B / 255, 0, 0 },
                            new float[] { 0, 0, 0, 1, 0 },
                            new float[] { 0, 0, 0, 0, 1 }
                            });
                            imageAttributes.SetColorMatrix(colorMatrix);
                            gr.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                                               0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        g.DrawImage(tintedImage, x, y, 20, 20);
                    }
                }
            }
        }

        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            clickedTile = null;
            foreach (Tile tile in TileList)
            {
                int x = tile.Coordinates[0];
                int y = tile.Coordinates[1];
                if (e.X >= x && e.X <= x + 20 && e.Y >= y && e.Y <= y + 20)
                {
                    clickedTile = tile;
                    break;
                }
            }

            if (clickedTile != null)
            {
                if (clickedTile.CurrentOwner != null)
                {
                    // De-highlight the previously clicked nation and update its tiles
                    if (lastClickedNation != null && lastClickedNation != clickedTile.CurrentOwner)
                    {
                        // needs to be some way of handling errors being highlighted
                        List<Tile> prevOwnedTiles = lastClickedNation.Tiles;
                        lastClickedNation = null;
                        UpdateTiles(prevOwnedTiles);
                    }

                    // Update the clicked nation's tiles for highlighting
                    List<Tile> ownedTiles = clickedTile.CurrentOwner.Tiles;
                    lastClickedNation = clickedTile.CurrentOwner;
                    HighlightSelectedNation(lastClickedNation);
                    UpdateTiles(ownedTiles);
                }
                else
                {
                    // De-highlight the previously clicked nation (if any) since the clicked tile is not owned
                    if (lastClickedNation != null)
                    {
                        List<Tile> prevOwnedTiles = lastClickedNation.Tiles;
                        lastClickedNation = null;
                        UpdateTiles(prevOwnedTiles);
                    }
                }
                // HERE: DISPLAY ANY INFORMATION RELEVANT TO THE CLICKED TILE
                if (clickedTile.CurrentOwner == null)
                {
                    CurrentSelectionLabel.Text = "Current Selection: None";
                }
                else
                {
                    CurrentSelectionLabel.Text = "Current Selection: " + clickedTile.CurrentOwner.FullName;
                }
            }
        }

        private void SelectNationButton_Click(object sender, EventArgs e)
        {
            if (lastClickedNation != null)
            {
                lastClickedNation.IsPlayer = true;
                PlayerNations.Add(lastClickedNation);
                SelectNationButton.Enabled = false;
                SelectNationButton.Visible = false;
                AttackButton.Enabled = true;
                AttackButton.Visible = true;
                AllyButton.Enabled = true;
                AllyButton.Visible = true;
                SkipButton.Enabled = true;
                SkipButton.Visible = true;
                UpdatePlayerNationInfoPanel();
            }
            else
            {
                CurrentSelectionLabel.Text = "You must first select a nation!";
            }
        }   

        private void AttackButton_Click(object sender, EventArgs e)
        {
            if (lastClickedNation != null && lastClickedNation != PlayerNations[0] && clickedTile != null)
            {
                //CurrentSelectionLabel.Text = "Attacking " + lastClickedNation.ShortHand + " in " + clickedTile.Terrain;
                GameData attackgd = new GameData();
                attackgd.TileList = TileList;
                attackgd.NationList = NationList;
                ActionResponse attackaction = PlayerNations[0].PlayerAttack(PlayerNations[0], clickedTile.CurrentOwner, clickedTile, attackgd);
                if (attackaction.Outcome)
                {
                    UpdatePlayerNationInfoPanel();
                    string WorldEvent = "- " + Turn + ": " + attackaction.Message;
                    UpdateWorldEventsPanel(WorldEvent);
                    NationList = attackaction.gameData.NationList;
                    TileList = attackaction.gameData.TileList;
                    NationInfo.DataSource = NationList;
                    NationInfo.Refresh();
                    List<Tile> TileChange = new List<Tile>();
                    TileChange.Add(clickedTile);
                    UpdateTiles(TileChange);
                }
                else if (attackaction.Outcome == false && attackaction.Message.Contains("defeat")) 
                {
                    // this is where you lost
                    // right now, it just says 'Action not performed'
                    UpdatePlayerNationInfoPanel();
                    string WorldEvent = "- " + Turn + ": " + attackaction.Message;
                    UpdateWorldEventsPanel(WorldEvent);
                    NationList = attackaction.gameData.NationList;
                    TileList = attackaction.gameData.TileList;
                    NationInfo.Refresh();
                }
                else
                {
                    //this is some other outcome -> a few potentials, but just put the message on the currentselectionlabel
                    CurrentSelectionLabel.Text = attackaction.Message;
                }
            }
            else if (lastClickedNation != null && lastClickedNation == PlayerNations[0])
            {
                CurrentSelectionLabel.Text = "You cannot attack yourself!";
            }
            else
            {
                CurrentSelectionLabel.Text = "You must first select a nation!";
            }
            DoAITurns();
        }

        private void AllyButton_Click(object sender, EventArgs e)
        {
            CurrentSelectionLabel.Text = "";
            if (lastClickedNation != null && lastClickedNation != PlayerNations[0])
            {
                CurrentSelectionLabel.Text = "Allying " + lastClickedNation.ShortHand;
                GameData allygd = new GameData();
                allygd.TileList = TileList;
                allygd.NationList = NationList;
                ActionResponse allyaction = PlayerNations[0].PlayerAlly(PlayerNations[0], lastClickedNation, allygd);
                UpdatePlayerNationInfoPanel();
                string WorldEvent = "- " + Turn + ": " + allyaction.Message;
                UpdateWorldEventsPanel(WorldEvent);
                NationList = allyaction.gameData.NationList;
                TileList = allyaction.gameData.TileList;
                NationInfo.DataSource = NationList;
                NationInfo.Refresh();
            } else if (lastClickedNation != null && lastClickedNation == PlayerNations[0])
            {
                CurrentSelectionLabel.Text += "You cannot ally yourself";
            }   
            else
            {
                CurrentSelectionLabel.Text += "Select a valid Ally Target";
            }
            //Refresh the map
            this.Invalidate();
            DoAITurns();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            CurrentSelectionLabel.Text = "Skipping Turn...";
            GameData skipgd = new GameData();
            skipgd.TileList = TileList;
            skipgd.NationList = NationList;
            ActionResponse response = PlayerNations[0].PlayerSkip(PlayerNations[0], skipgd);
            UpdatePlayerNationInfoPanel();
            string WorldEvent = "- " + Turn + ": " + response.Message;
            UpdateWorldEventsPanel(WorldEvent);
            //update nationinfo datagridview
            NationList = response.gameData.NationList;
            TileList = response.gameData.TileList;
            NationInfo.DataSource = NationList;
            NationInfo.Refresh();
            DoAITurns();
        }

        private void BackToMeButtonClick(object sender, EventArgs e)
        {
            CurrentSelectionLabel.Text = "Current Selection: Focus Player";
            lastClickedNation = PlayerNations[0];
            HighlightSelectedNation(lastClickedNation);
            UpdatePlayerNationInfoPanel();
            this.Invalidate();
        }   

        private void FutureButtonB_Click(object sender, EventArgs e)
        {
            CurrentSelectionLabel.Text = "Current Selection: Future B";
        }

        private void MapModesBox_SelectionChanged(object sender, EventArgs e)
        {
            // Update the map mode based on the selected item in the dropdown list
            string selectedMode = MapModesBox.SelectedItem.ToString();
            CurrentSelectionLabel.Text = "Current Selection: Map Mode - " + selectedMode;

            // Update the map based on the selected mode (e.g., change colors, display different information, etc.)
            // You can implement this logic in the OnPaint event or a separate method.

            // Example: Set the map colors based on the selected mode
            switch (selectedMode)   
            {
                case "Political":
                    MapMode = "Political";
                    break;
                case "Terrain":
                    MapMode = "Terrain";
                    break;
                case "Tile Manpower":
                    MapMode = "Tile_Manpower";
                    break;
                case "Nation Manpower":
                    MapMode = "Nation_Manpower";
                    break;
                case "Diplomatic Power":
                    MapMode = "Diplomatic_Power";
                    break;
                case "Army Spirit":
                    MapMode = "Army_Spirit";
                    break;
                case "Ideology":
                    MapMode = "Ideology";
                    break;
                case "Alliances":
                    MapMode = "Alliances";
                    break;
                default:
                    MapMode = "Political";
                    break;
            }
            this.Invalidate();
        }

        private void DoAITurns()
        {
            foreach (Nation nation in NationList)
            {
                if (nation.IsPlayer == false && nation.IsAlive == true)
                {
                    GameData aigd = new GameData();
                    aigd.TileList = TileList;
                    aigd.NationList = NationList;
                    ActionResponse airesponse = nation.AIAction(aigd);
                    string WorldEvent = "- " + Turn + ": " + airesponse.Message;
                    UpdateWorldEventsPanel(WorldEvent);
                    NationList = airesponse.gameData.NationList;
                    TileList = airesponse.gameData.TileList;
                    NationInfo.DataSource = NationList;
                    NationInfo.Refresh();
                    this.Invalidate();
                    UpdatePlayerNationInfoPanel();
                }
            }
            Turn++;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}